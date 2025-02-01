using System;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using NBitcoin;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.Timers;
using System.Diagnostics;
using System.Drawing;
using QRCoder;

namespace PowerTaskerPC
{
    public partial class Form1 : Form
    {
        private string currentPublicKey = string.Empty;
        private string currentPrivateKey = string.Empty;
        private string savedRelayUrl = string.Empty;
        private bool startWithWindows = false;
        private NotifyIcon trayIcon;
        private System.Timers.Timer periodicTaskTimer;
        private PictureBox qrCodeBox;
        private Label qrCodeLabel;

        // path to the settings file
        private readonly string settingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PowerTaskerClient",
            "settings.json"
        );

        public static Form1 Instance { get; private set; }
        public Form1()
        {
            Instance = this;
            InitializeComponent();

            qrCodeBox = new PictureBox
            {
                Size = new Size(150, 150),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point(430, 210),
                Visible = false
            };
            pnlKeyPair.Controls.Add(qrCodeBox);

            qrCodeLabel = new Label
            {
                Text = "Private Key QR Code",
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                Location = new Point(430, 365),
                Visible = false
            };
            pnlKeyPair.Controls.Add(qrCodeLabel);


            // set fonts
            Font appFont = new Font("Segoe UI", 10f, FontStyle.Regular);
            Font headerFont = new Font("Segoe UI Semibold", 12f, FontStyle.Regular);

            this.Font = appFont;

            // header labels fonts
            this.lblRelayStatus.Font = headerFont;
            this.lblPublicKey.Font = headerFont;
            this.lblPrivateKey.Font = headerFont;
            this.lblPublicKeyKeyPair.Font = headerFont;
            this.lblStartup.Font = headerFont;

            // text box contents fonts
            this.txtRelayUrl.Font = new Font("Consolas", 10f, FontStyle.Regular);
            this.txtPrivateKey.Font = new Font("Consolas", 10f, FontStyle.Regular);
            this.txtPublicKey.Font = new Font("Consolas", 10f, FontStyle.Regular);

            // buttons fonts
            this.btnGenerateKeys.Font = appFont;
            this.btnCheckConnection.Font = appFont;
            this.btnGenerateKeyPair.Font = appFont;
            this.btnCopyPrivateKey.Font = appFont;
            this.btnCopyPublicKey.Font = appFont;
            this.btnFinishSetup.Font = appFont;


            this.Load += Form1_Load;
            InitializeTrayIcon();
            InitializePeriodicTask();
        }

        public void ShowFromOtherInstance()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ShowFromOtherInstance));
            }
            else
            {
                Show();
                WindowState = FormWindowState.Normal;
                Activate();
            }
        }

        private void GenerateQRCode(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                qrCodeBox.Image = null;
                qrCodeBox.Visible = false;
                qrCodeLabel.Visible = false;
                return;
            }

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q))
            using (QRCode qrCode = new QRCode(qrCodeData))
            {
                Bitmap qrCodeImage = qrCode.GetGraphic(5);
                qrCodeBox.Image?.Dispose();
                qrCodeBox.Image = qrCodeImage;
                qrCodeBox.Visible = true;
                qrCodeLabel.Visible = true;
            }
        }


        private void InitializePeriodicTask()
        {
            // start timer interval 30sec
            periodicTaskTimer = new System.Timers.Timer(30000);
            periodicTaskTimer.Elapsed += PeriodicTaskTimer_Elapsed;
            periodicTaskTimer.AutoReset = true;
            periodicTaskTimer.Enabled = true;
        }

        private void PeriodicTaskTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!string.IsNullOrEmpty(savedRelayUrl) && !string.IsNullOrEmpty(currentPrivateKey))
            {
                RunPythonFetcher(currentPrivateKey, savedRelayUrl);
            }
        }

        private async void RunPythonFetcher(string privateKey, string relayUrl)
        {
            try
            {

                await new RemoteControl(privateKey, relayUrl).ProcessCommand();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to fetch command: {ex.Message}");
            }
        }

        private void InitializeTrayIcon()
        {
            trayIcon = new NotifyIcon();
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "powertasker.ico");
            if (File.Exists(iconPath))
            {
                trayIcon.Icon = new System.Drawing.Icon(iconPath);
            }
            else
            {
                trayIcon.Icon = System.Drawing.SystemIcons.Application;
            }
            trayIcon.Text = "Power Tasker";

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Show", null, ShowForm_Click);
            contextMenu.Items.Add("Exit", null, Exit_Click);

            trayIcon.ContextMenuStrip = contextMenu;
            trayIcon.DoubleClick += TrayIcon_DoubleClick;
            trayIcon.Visible = true;
        }

        private void ShowForm_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowForm_Click(sender, e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
            else
            {
                periodicTaskTimer?.Stop();
                periodicTaskTimer?.Dispose();
                base.OnFormClosing(e);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            LoadSettings();


            pnlHome.Visible = true;
            pnlKeyPair.Visible = false;

            
            UpdateHomeScreen();




            if (startWithWindows)
            {
                radioEnabled.Checked = true;
            }
            else
            {
                radioDisabled.Checked = true;
            }

            BeginInvoke(new Action(() =>
            {
                if (radioEnabled.Checked)
                {
                    radioEnabled.Focus();
                }
                else { radioDisabled.Focus(); }
            }));


            // check the relay URL on app startup
            if (!string.IsNullOrEmpty(savedRelayUrl))
            {
                txtRelayUrl.Text = savedRelayUrl;
                BtnCheckConnection_Click(this, EventArgs.Empty);
            }

            // open minimized inside tray
            if (startWithWindows)
            {
                BeginInvoke(new Action(() =>
                {
                    WindowState = FormWindowState.Minimized;
                    Hide();
                }));
            }
        }

        private void UpdateHomeScreen()
        {
            if (string.IsNullOrEmpty(currentPublicKey))
            {
                lblPublicKey.Text = "No Public Key Set";
                btnGenerateKeys.Text = "Generate Key Pair";
            }
            else
            {
                lblPublicKey.Text = $"Public Key: {currentPublicKey.Substring(0, 9)}...{currentPublicKey.Substring(currentPublicKey.Length - 9)}";

                btnGenerateKeys.Text = "View or Renew Keys";
            }
        }

        private async void BtnCheckConnection_Click(object sender, EventArgs e)
        {
            savedRelayUrl = txtRelayUrl.Text;
            SaveSettings();
            lblRelayStatus.Text = "Relay: Checking...";
            btnCheckConnection.Enabled = false;
            try
            {
                var handler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
                };

                using (var client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    var response = await client.GetAsync(txtRelayUrl.Text);
                    if (response.IsSuccessStatusCode)
                    {
                        lblRelayStatus.Text = "Relay: Connected";
                        lblRelayStatus.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        lblRelayStatus.Text = "Relay: Failed to Connect";
                        lblRelayStatus.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                // Add logging of the actual exception for debugging
                Console.WriteLine(ex.ToString());
                lblRelayStatus.Text = "Relay: Failed to Connect";
                lblRelayStatus.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                btnCheckConnection.Enabled = true;
            }
        }

        private void TxtRelayUrl_Leave(object sender, EventArgs e)
        {
            savedRelayUrl = txtRelayUrl.Text;
            SaveSettings();
        }

        private void BtnGenerateKeys_Click(object sender, EventArgs e)
        {
            pnlHome.Visible = false;
            pnlKeyPair.Visible = true;

            if (!string.IsNullOrEmpty(currentPrivateKey))
            {
                txtPrivateKey.Text = currentPrivateKey;
                txtPublicKey.Text = currentPublicKey;
                GenerateQRCode(currentPrivateKey);
            }
        }

        private void BtnGenerateKeyPair_Click(object sender, EventArgs e)
        {
            Key privateKeyObj = new Key();
            currentPrivateKey = privateKeyObj.ToHex();
            currentPublicKey = BitConverter.ToString(privateKeyObj.PubKey.Decompress().ToBytes()).Replace("-", "").ToLower();

            txtPrivateKey.Text = currentPrivateKey;
            txtPublicKey.Text = currentPublicKey;

            GenerateQRCode(currentPrivateKey);

            SaveSettings();
        }

        private void BtnCopyPrivateKey_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPrivateKey.Text))
            {
                Clipboard.SetText(txtPrivateKey.Text);
                MessageBox.Show("Private key copied to clipboard!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No private key to copy!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnCopyPublicKey_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPublicKey.Text))
            {
                Clipboard.SetText(txtPublicKey.Text);
                MessageBox.Show("Public key copied to clipboard!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No public key to copy!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ChkKeysSetup_CheckedChanged(object sender, EventArgs e)
        {
            btnFinishSetup.Enabled = chkKeysSetup.Checked;
        }

        private void BtnFinishSetup_Click(object sender, EventArgs e)
        {
            if (!chkKeysSetup.Checked || string.IsNullOrEmpty(txtPublicKey.Text))
            {
                MessageBox.Show("Please generate keys and confirm the setup.", "Setup Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            currentPublicKey = txtPublicKey.Text;
            currentPrivateKey = txtPrivateKey.Text;

            SaveSettings();

            pnlKeyPair.Visible = false;
            pnlHome.Visible = true;

            UpdateHomeScreen();

            chkKeysSetup.Checked = false;
            btnFinishSetup.Enabled = false;
        }

        private void SetStartup(bool enable)
        {
            string appPath = Application.ExecutablePath;
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (enable)
                rk.SetValue("PowerTaskerClient", appPath);
            else
                rk.DeleteValue("PowerTaskerClient", false);

            startWithWindows = enable;
            SaveSettings();
        }

        private void RadioEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (radioEnabled.Checked)
            {
                SetStartup(true);
            }
        }

        private void RadioDisabled_CheckedChanged(object sender, EventArgs e)
        {
            if (radioDisabled.Checked)
            {
                SetStartup(false);
            }
        }

        private void SaveSettings()
        {
            try
            {
                var settings = new
                {
                    RelayUrl = savedRelayUrl,
                    PublicKey = currentPublicKey,
                    PrivateKey = currentPrivateKey,
                    StartWithWindows = startWithWindows
                };

                Directory.CreateDirectory(Path.GetDirectoryName(settingsFilePath));
                File.WriteAllText(settingsFilePath, JsonConvert.SerializeObject(settings));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(settingsFilePath))
                {
                    var settings = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(settingsFilePath));
                    savedRelayUrl = settings.RelayUrl;
                    currentPublicKey = settings.PublicKey;
                    currentPrivateKey = settings.PrivateKey;
                    startWithWindows = settings.StartWithWindows;

                    txtRelayUrl.Text = savedRelayUrl;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}