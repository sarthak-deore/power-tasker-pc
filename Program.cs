using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Windows.Forms;

namespace PowerTaskerPC
{
    internal static class Program
    {
        private const string UniqueAppName = "PowerTaskerPC";

        [STAThread]
        static void Main()
        {
            // only one instance runs
            using (var mutex = new Mutex(true, UniqueAppName, out bool isFirstInstance))
            {
                if (isFirstInstance)
                {
                    Thread ipcThread = new Thread(ListenForOtherInstances)
                    {
                        IsBackground = true
                    };
                    ipcThread.Start();

                    // start the main application
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }
                else
                {
                    NotifyExistingInstance();
                }
            }
        }

        private static void NotifyExistingInstance()
        {
            try
            {
                using (var client = new NamedPipeClientStream(".", UniqueAppName, PipeDirection.Out))
                {
                    client.Connect(1000);
                    using (var writer = new StreamWriter(client))
                    {
                        writer.WriteLine("SHOW");
                        writer.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error notifying existing instance: {ex.Message}");
            }
        }

        private static void ListenForOtherInstances()
        {
            while (true)
            {
                try
                {
                    using (var server = new NamedPipeServerStream(UniqueAppName, PipeDirection.In, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
                    {
                        server.WaitForConnection();

                        try
                        {
                            using (var reader = new StreamReader(server))
                            {
                                string message = reader.ReadLine();
                                if (message == "SHOW")
                                {
                                    // show the main instance
                                    Form1.Instance?.Invoke(new Action(() =>
                                    {
                                        Form1.Instance.ShowFromOtherInstance();
                                    }));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error processing client message: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in named pipe server: {ex.Message}");
                }
            }
        }

    }
}
