using Darkages;
using Darkages.Network;
using Darkages.Types;
using System;

namespace LoruleComponentService
{
    class Program 
    {

        public class ObjectClient : NetworkClient
        {
            public Proxy ProxyServer = new Proxy();
            public class Proxy : ServerContext
            {

            }
        }

        public class ObjectServer : NetworkServer<ObjectClient>
        {
            public ObjectServer() : base(1000)
            {

            }

            public override void Abort()
            {

            }

            public override void ClientDataReceived(ObjectClient client, NetworkPacket packet)
            {
                var objs = client.GetObjects<Monster>(i => true);

            }

            public override void ClientDisconnected(ObjectClient client)
            {
            }

            public override void ClientConnected(ObjectClient client)
            {
                Console.WriteLine("Object Server Connected.");
            }
        }

        static void Main(string[] args)
        {
            var server = new ObjectServer();
            server.Start(2617);

            System.Threading.Thread.CurrentThread.Join();
        }
    }
}
