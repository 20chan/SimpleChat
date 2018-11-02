using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;

namespace Client
{
    public class ChatClient
    {
        public delegate void ChatboxUpdate(string str);
        public event ChatboxUpdate ChatUpdate;

        private TcpClient TcpClient;
        private NetworkStream Stream;
        private string Ip;
        private int Port;

        public ChatClient(string ip = "127.0.0.1", int port = 7007)
        {
            this.Ip = ip;
            this.Port = port;
            this.TcpClient = new TcpClient(Ip, port);
            this.Stream = this.TcpClient.GetStream();
            Task.Factory.StartNew(WaitForData);
        }

        public void Send(string str)
        {
            var chat = new ChatPacket(str, TcpClient.Client.LocalEndPoint.ToString());
            var buffer = ChatPacket.CreatePacket(chat);

            Stream.Write(buffer, 0, buffer.Length);
            ChatUpdate(chat.ToString());
        }

        private async void WaitForData ()
        {
            while (true)
            {
                int MAX_SIZE = 1024;
                byte[] buffer = new byte[MAX_SIZE];
                int nbytes = await Stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                var chat = ChatPacket.ParsePacket(buffer);
                ChatUpdate(chat.ToString());
            }
        }

        public void Close()
        {
            this.Stream.Close();
            this.TcpClient.Close();
        }
    }
}
