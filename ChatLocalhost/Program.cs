using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChatLocalhost
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            byte[] data = new byte[1024];
            int recv;

            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

            Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            newsock.Bind(ipep);

            newsock.Listen(10);

            Console.WriteLine("Waiting for a client...");

            Socket client = await newsock.AcceptAsync();

            IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;

            Console.WriteLine("Connected with {0} at port {1}", clientep.Address, clientep.Port);

            string welcome = "Welcome to my test server";

            data = Encoding.UTF8.GetBytes(welcome);

             client.Send(data, SocketFlags.None);

            string input;

            // Odbieranie wiadomości od klienta w tle
            Task.Run(() =>
            {
                while (true)
                {
                    data = new byte[1024];
                    Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString());
                    recv = client.Receive(data, SocketFlags.None);
                    if (recv == 0)
                        break;

                    Console.WriteLine("Client: " + Encoding.UTF8.GetString(data, 0, recv));
                }
            });

            // Pętla do wysyłania wiadomości do klienta
            while (true)
            {
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString());
                input = Console.ReadLine();

                if (input == "exit")
                    break;

                client.Send(Encoding.UTF8.GetBytes(input), SocketFlags.None);
            }

            Console.WriteLine("Disconnected from {0}", clientep.Address);

            client.Shutdown(SocketShutdown.Both);
            client.Close();
            newsock.Close();

            Console.ReadLine();
        }
    }
}