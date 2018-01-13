using Darkages.Network.ServerFormats;
using System;

namespace CustomClientExample
{
    class Program
    {
        static readonly DAClient client = new DAClient();

        static void Main(string[] args)
        {
            client.Connect("127.0.0.1", 2610, out var error);

            if (error != System.Net.Sockets.SocketError.Success)
            {
                Console.WriteLine("Unable to connect.");
                return;
            }

            System.Threading.Thread.CurrentThread.Join();
        }
    }

    public class DAClient : Client<DAClient>
    {
        public DAClient() : base()
        {

        }

        public override void Format7EHandler(ServerFormat7E format)
        {
            Console.WriteLine(format.Text);
        }
    }
}
