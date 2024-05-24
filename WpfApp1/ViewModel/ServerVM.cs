using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;

namespace Task1_ServerApp
{
    public class ServerVM
    {
        // Declare a server
        private TcpListener server = null!;
        private int port;
        private IPAddress iPAddress;
        // Initiate a buffer
        byte[] bytes = new byte[256];
        string data = string.Empty;

        public ObservableCollection<string> Messages { get; private set; } = new ObservableCollection<string>();

        public ServerVM()
        {
            ConfigureServer();
            Thread thread = new Thread(StartServer);
            thread.Start();
        }

        private void ConfigureServer()
        {
            // Set port and ip address 
            int port = 13000;
            iPAddress = IPAddress.Parse("127.0.0.1");

            // Create server
            server = new TcpListener(iPAddress, port);
        }

        private void StartServer()
        {
            try
            {
                // Start server
                server.Start();

                while (true)
                {
                    WriteLogFromThread("Waiting for connection...");
                    // Create Tcp client when it connect to the server
                    TcpClient client = server.AcceptTcpClient();

                    WriteLogFromThread("Connected");

                    // Create network stream from client
                    NetworkStream networkStream = client.GetStream();

                    // Reading and transmitting information over the network
                    int i;
                    while ((i = networkStream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                        WriteLogFromThread($"Received: {data}, {i} bytes");

                        // Some information checking
                        data = data.ToUpper();

                        byte[] newBytes = Encoding.ASCII.GetBytes(data);
                        networkStream.Write(newBytes, 0, newBytes.Length);
                        WriteLogFromThread($"Sent: {data}");

                        client.Close();
                        WriteLogFromThread("Disconnected");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLogFromThread($"Error {ex.Message}");
            }
            finally
            {
                // Stop the server at exit
                server.Stop();
            }
        }

        private void WriteLogFromThread(string message)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                Messages.Add(message);
            });
        }
    }
}
