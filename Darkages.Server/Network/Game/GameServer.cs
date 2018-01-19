using System;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Darkages.Network.Game.Components;

namespace Darkages.Network.Game
{
    [Serializable]
    public partial class GameServer
    {
        public static object ServerSyncObj = new object();
        public static TcpClient Proxy = null;

        private bool isRunning;
        private DateTime lastUpdate = DateTime.UtcNow;
        private Thread updateThread;

        static GameServer()
        {
            //Proxy = new TcpClient("127.0.0.1", 2617);
        }

        public GameServer(int capacity)
            : base(capacity)
        {
            Frames = ServerContext.Config.FRAMES;

            Components = new Collection<GameServerComponent>();
            UpdateSpan = TimeSpan.FromSeconds(1.0 / Frames);
            InitializeGameServer();
        }

        public Collection<GameServerComponent> Components { get; }
        public TimeSpan UpdateSpan { get; }

        public int Frames { get; set; }

        internal GameServerTimer ServerTimer { get; set; }

        private void AutoSave(GameClient client)
        {
            if ((DateTime.UtcNow - client.LastSave)
                .TotalSeconds > ServerContext.Config.SaveRate)
                client.Save();
        }

        private void DoUpdate()
        {
            isRunning = true;
            lastUpdate = DateTime.UtcNow;

            while (isRunning)
            {
                var delta = DateTime.UtcNow - lastUpdate;
                {
                    Update(delta);
                    lastUpdate = DateTime.UtcNow;
                }
                Thread.Sleep(UpdateSpan);
            }
        }

        public void InitializeGameServer()
        {
            ServerTimer = new GameServerTimer(TimeSpan.FromMilliseconds(ServerContext.Config.MinimalLatency));

            var daytimeComponent = new DaytimeComponent(this);
            Components.Add(daytimeComponent);

            var messageComponent = new MessageComponent(this);
            Components.Add(messageComponent);

            var pingComponent = new PingComponent(this);
            Components.Add(pingComponent);

            var MilethSpawner = new MonolithComponent(this);
            Components.Add(MilethSpawner);

            var objectComponent = new ObjectComponent(this);
            Components.Add(objectComponent);

            var mundaneComponent = new MundaneComponent(this);
            Components.Add(mundaneComponent);

            Console.WriteLine(Components.Count + " Server Components loaded.");
        }

        public void Update(TimeSpan elapsedTime)
        {
            ServerTimer.Update(elapsedTime);

            if (ServerTimer.Elapsed)
            {
                UpdateClients(elapsedTime);
                UpdateAreas(elapsedTime);
                UpdateComponents(elapsedTime);
            }
        }

        private void UpdateComponents(TimeSpan elapsedTime)
        {
            ThreadPool.QueueUserWorkItem(w =>
            {
                lock (Components)
                {
                    for (var i = 0; i < Components.Count; i++)
                        Components[i].Update(elapsedTime);
                }
            });
        }

        private static void UpdateAreas(TimeSpan elapsedTime)
        {
            foreach (var area in ServerContext.GlobalMapCache.Values)
            {
                area.Update(elapsedTime);

                UpdateGroundItems(area);
            }
        }

        private static void UpdateGroundItems(Area area)
        {
            var objects = area?.GetObjects(i => i != null && i.CurrentMapId == area.ID, Get.Items | Get.Money);

            if (objects != null)
                foreach (var obj in objects)
                    if (obj != null)
                        if (obj.AislingsNearby().Length > 0)
                            obj.CreationDate = DateTime.UtcNow;
        }

        private void UpdateClients(TimeSpan elapsedTime)
        {
            foreach (var client in Clients)
                if (client != null && client.Aisling != null)
                    client.Update(elapsedTime);
        }

        public override void ClientConnected(GameClient client)
        {
        }

        public override void ClientDisconnected(GameClient client)
        {
            if (client == null || client.Aisling == null)
                return;

            client.Aisling.LoggedIn = false;

            AutoSave(client);
            client.Aisling.Remove(true);

            base.ClientDisconnected(client);
        }

        public override void Abort()
        {
            base.Abort();

            isRunning = false;

            if (updateThread != null)
            {
                updateThread.Abort();
                updateThread.Join();
                updateThread = null;
            }
        }

        public override void Start(int port)
        {
            base.Start(port);

            if (isRunning)
                return;


            new TaskFactory().StartNew(DoUpdate);

            isRunning = true;
        }
    }
}