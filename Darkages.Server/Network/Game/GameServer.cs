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
        private readonly GameServerTimer ServerTimer;
        public static object ServerSyncObj = new object();
        public static TcpClient Proxy = null;

        public Collection<GameServerComponent> Components;
        public TimeSpan UpdateSpan, UpdateWorkerSpan;

        private DateTime lastUpdate       = DateTime.UtcNow;
        private DateTime lastworkerUpdate = DateTime.UtcNow;

        public int Frames, Cycles;
        private bool isRunning;

        static GameServer()
        {
            //Proxy = new TcpClient("127.0.0.1", 2617);
        }

        public GameServer(int capacity)
            : base(capacity)
        {
            Frames  = ServerContext.Config.FRAMES;
            Cycles  = (2 + Frames) / 3;

            Components       = new Collection<GameServerComponent>();
            UpdateSpan       = TimeSpan.FromSeconds(1.0 / Frames);
            UpdateWorkerSpan = TimeSpan.FromSeconds(1.0 / Cycles);

            ServerTimer = new GameServerTimer(TimeSpan.FromMilliseconds(ServerContext.Config.MinimalLatency));

            InitializeGameServer();
        }

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

        private void DoWork()
        {
            isRunning = true;
            lastworkerUpdate = DateTime.UtcNow;

            while (isRunning)
            {
                var delta = DateTime.UtcNow - lastworkerUpdate;
                {
                    UpdateAreas(delta);
                    UpdateComponents(delta);
                    lastworkerUpdate = DateTime.UtcNow;
                }
                Thread.Sleep(UpdateWorkerSpan);
            }
        }

        public void InitializeGameServer()
        {
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
            }
        }

        private void UpdateComponents(TimeSpan elapsedTime)
        {
            lock (Components)
            {
                for (var i = 0; i < Components.Count; i++)
                    Components[i].Update(elapsedTime);
            }
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
        }

        public override void Start(int port)
        {
            base.Start(port);

            if (isRunning)
                return;


            new TaskFactory().StartNew(DoUpdate);
            new TaskFactory().StartNew(DoWork);

            isRunning = true;
        }
    }
}