using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using GuerrillaNtp;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;

namespace PowerTaskerPC
{
    public class RemoteControl
    {
        private class TimeService
        {
            private readonly string[] NTP_SERVERS = new string[]
            {
                "time.google.com",
                "time.windows.com"
            };

            public string GetCurrentGMTTime()
            {
                foreach (var server in NTP_SERVERS)
                {
                    try
                    {
                        IPAddress[] addresses;
                        try
                        {
                            addresses = Dns.GetHostAddresses(server);
                            if (addresses == null || addresses.Length == 0)
                                continue;
                        }
                        catch
                        {
                            continue;
                        }

                        // filter for ipv4 only
                        var ipv4Address = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                        if (ipv4Address == null)
                            continue;

                        var client = new NtpClient(ipv4Address);
                        var clock = client.QueryAsync().Result;
                        var networkDateTime = clock.Now.DateTime;
                        var gmtDateTime = networkDateTime.ToUniversalTime();

                        if (networkDateTime.Year < 2020 || networkDateTime.Year > 2090)
                            continue;

                        return gmtDateTime.ToString("yyyyMMddHHmmss");
                    }
                    catch
                    {
                        continue;
                    }
                }

                // fallback: return current system time
                return DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            }
        }


        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly string _privateKeyHex;
        private readonly string _relayUrl;
        private readonly Dictionary<string, string> _commands;

        public RemoteControl(string privateKeyHex, string relayUrl)
        {
            _privateKeyHex = privateKeyHex;
            _relayUrl = relayUrl.TrimEnd('/');
            _commands = new Dictionary<string, string>
            {
                ["signout"] = "shutdown /l",
                ["sleep"] = "rundll32.exe powrprof.dll,SetSuspendState Sleep",
                ["shutdown"] = "shutdown /s /t 0",
                ["restart"] = "shutdown /r /t 0"
            };
        }

        private static string BytesToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        private static byte[] HexToBytes(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        private (string publicKey, string signature) GenerateSignature(string command)
        {
            try
            {
                var curve = ECNamedCurveTable.GetByName("secp256k1");
                var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

                var privateKeyBytes = HexToBytes(_privateKeyHex);
                var privateKey = new BigInteger(1, privateKeyBytes);
                var parameters = new ECPrivateKeyParameters(privateKey, domain);

                var q = parameters.Parameters.G.Multiply(privateKey);
                var publicKeyBytes = q.GetEncoded(false);
                var publicKeyHex = BytesToHex(publicKeyBytes);

                byte[] messageHash;
                using (var sha256 = SHA256.Create())
                {
                    messageHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(command));
                }

                var signer = new ECDsaSigner();
                signer.Init(true, parameters);
                var signature = signer.GenerateSignature(messageHash);

                var r = signature[0].ToByteArrayUnsigned();
                var s = signature[1].ToByteArrayUnsigned();

                var rPadded = new byte[32];
                var sPadded = new byte[32];
                Array.Copy(r, 0, rPadded, 32 - r.Length, r.Length);
                Array.Copy(s, 0, sPadded, 32 - s.Length, s.Length);

                var signatureHex = BytesToHex(rPadded.Concat(sPadded).ToArray());

                return (publicKeyHex, signatureHex);
            }
            catch
            {
                return (null, null);
            }
        }

        private bool VerifySignature(string signatureHex, string pubkeyHex, string command)
        {
            try
            {
                var curve = ECNamedCurveTable.GetByName("secp256k1");
                var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

                var pubkeyBytes = HexToBytes(pubkeyHex);
                var q = curve.Curve.DecodePoint(pubkeyBytes);
                var publicKey = new ECPublicKeyParameters(q, domain);

                var signatureBytes = HexToBytes(signatureHex);
                var rBytes = new byte[32];
                var sBytes = new byte[32];
                Array.Copy(signatureBytes, 0, rBytes, 0, 32);
                Array.Copy(signatureBytes, 32, sBytes, 0, 32);

                var r = new BigInteger(1, rBytes);
                var s = new BigInteger(1, sBytes);

                byte[] messageHash;
                using (var sha256 = SHA256.Create())
                {
                    messageHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(command));
                }

                var verifier = new ECDsaSigner();
                verifier.Init(false, publicKey);

                return verifier.VerifySignature(messageHash, r, s);
            }
            catch
            {
                return false;
            }
        }

        private void ExecuteCommand(string command)
        {
            if (_commands.TryGetValue(command, out var cmd))
            {
                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c {cmd}",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    process.Start();
                }
            }
        }

        public async Task ProcessCommand()
        {
            var ntpClient = new TimeService();
            var currentTime = ntpClient.GetCurrentGMTTime();
            if (currentTime == null) return;

            var initialCommand = $"get+{currentTime}";
            var (publicKeyHex, signatureHex) = GenerateSignature(initialCommand);
            if (string.IsNullOrEmpty(publicKeyHex) || string.IsNullOrEmpty(signatureHex)) return;

            var payload = new
            {
                pubkey = publicKeyHex,
                signature = signatureHex,
                command = initialCommand
            };

            try
            {
                var apiUrl = $"{_relayUrl}/api/request/fetch-command";
                var response = await _httpClient.PostAsync(
                    apiUrl,
                    new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                );

                if (!response.IsSuccessStatusCode) return;

                var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    await response.Content.ReadAsStringAsync()
                );

                if (responseData is null ||
                    !responseData.TryGetValue("pubkey", out var receivedPubkey) ||
                    !responseData.TryGetValue("signature", out var receivedSignature) ||
                    !responseData.TryGetValue("command", out var receivedCommand))
                    return;

                var parts = receivedCommand.Split(new[] { '+' }, StringSplitOptions.None);
                if (parts.Length != 2) return;

                var command = parts[0];
                var commandTime = DateTime.ParseExact(parts[1], "yyyyMMddHHmmss", null);
                var currentTimeObj = DateTime.ParseExact(currentTime, "yyyyMMddHHmmss", null);

                var timeDiff = Math.Abs((currentTimeObj - commandTime).TotalSeconds);
                if (timeDiff > 120) return;

                if (VerifySignature(receivedSignature, receivedPubkey, receivedCommand))
                {
                    ExecuteCommand(command);
                }
            }
            catch
            {
                return;
            }
        }
    }
}