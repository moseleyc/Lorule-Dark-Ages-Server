using System;
using System.Net.Sockets;
using System.Threading;
using Darkages.Network;
using Darkages.Network.ServerFormats;

namespace CustomClientExample
{
    internal class Program
    {
        private static readonly DAClient client = new DAClient();

        private static void Main(string[] args)
        {
            client.Connect("127.0.0.1", 2610, out var error);

            if (error != SocketError.Success)
            {
                Console.WriteLine("Unable to connect.");
                return;
            }

            Thread.CurrentThread.Join();
        }
    }

    public class DAClient : Client<DAClient>
    {
        public override void Format7EHandler(ServerFormat7E format)
        {
            Console.WriteLine(format.Text);
        }
    }
}