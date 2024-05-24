using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleClient
{
    public class ClientVM : NotifyPropertyChangeHandler
    {
        // Properties
        /**********************************************************************************/
        private string ipServer = "127.0.0.1";
        private int port = 13000;
        private TcpClient client;
        private NetworkStream networkStream;
        private byte[] myBytes;
        private int sizeBytes = 256;
        private int receivedBytes;

        private string outputMessage = string.Empty;
        public string OutputMessage
        {
            get => outputMessage;
            set
            {
                outputMessage = value;
                NotifyPropertyChanged(nameof(OutputMessage));
            }
        }
        public ObservableCollection<string> Messages { get; private set; } = new ObservableCollection<string>();


        // Commands
        /**********************************************************************************/
        public ICommand SendMessageCommand { get; private set; }


        // Constructor
        /**********************************************************************************/
        public ClientVM()
        {
            SendMessageCommand = new RelayCommand(ProcessMessage);
        }


        // Methods
        /**********************************************************************************/
        private void ProcessMessage()
        {
            try
            {
                client = new TcpClient(ipServer, port);

                SendMessage();

                Messages.Add($"Client: {OutputMessage}");
                OutputMessage = string.Empty;

                string response = ReceiveResponse();
                Messages.Add($"Server: {response}, {receivedBytes} bytes");

                client.Close();
            }
            catch (Exception ex)
            {
                Messages.Add($"Error: {ex.Message}");
            }
        }
        private byte[] StringToBytes(string message)
        {
            return Encoding.ASCII.GetBytes(message);
        }
        private void SendMessage()
        {
            myBytes = StringToBytes(OutputMessage);
            networkStream = client.GetStream();
            networkStream.Write(myBytes, 0, myBytes.Length);
        }
        private string ReceiveResponse()
        {
            myBytes = new byte[sizeBytes];
            receivedBytes = networkStream.Read(myBytes, 0, myBytes.Length);
            return BytesToString(myBytes);
        }
        private string BytesToString(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes, 0, bytes.Length);
        }

    }
}
