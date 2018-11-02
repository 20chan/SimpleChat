using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static List<TcpClient> ClientList = new List<TcpClient>();

        static void Main(string[] args)
        {
            AsyncEchoServer().Wait();
        }

        async static Task AsyncEchoServer()
        {
            var listener = new TcpListener(IPAddress.Any, 7007);
            listener.Start();
            while (true)
            {
                TcpClient tc = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                ClientList.Add(tc);
                var task = Task.Factory.StartNew(AsyncTcpProcess, tc);
            }
        }

        async static void BroadcastExceptMe(byte[] message, int nbytes, TcpClient tc)
        {
            foreach (var client in ClientList)
            {
                if (client == tc)
                {
                    continue;
                }
                var stream = client.GetStream();
                await stream.WriteAsync(message, 0, nbytes).ConfigureAwait(false);
            }
        }

        async static void AsyncTcpProcess(object o)
        {
            TcpClient tc = o as TcpClient;

            int MAX_SIZE = 1024;
            NetworkStream stream = tc.GetStream();

            while (true)
            {
                try
                {
                    var buffer = new byte[MAX_SIZE];
                    var nbytes = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                    if (nbytes > 0)
                    {
                        var chat = ChatPacket.ParsePacket(buffer);
                        Console.WriteLine($"From {chat.Sender} \"{chat.Message}\" at {chat.Time}");
                        BroadcastExceptMe(buffer, nbytes, tc);
                    }
                }
                catch (IOException)
                {
                    Console.WriteLine(string.Format("connection closed by foreign : {0}", tc.Client.RemoteEndPoint.ToString()));
                    break;
                }
            }
            ClientList.Remove(tc);

            stream.Close();
            tc.Close();
        }
    }
}
