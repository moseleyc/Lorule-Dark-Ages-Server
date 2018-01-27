using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Darkages.Network.Game.Components;
using Darkages.Network.Object;

namespace Darkages.Network.Game
{
    [Serializable]
    public partial class GameServer
    {
        int Frames;
        bool isRunning;

        DateTime lastServerUpdate = DateTime.UtcNow;
        DateTime lastClientUpdate = DateTime.UtcNow;
        TimeSpan ServerUpdateSpan, ClientUpdateSpan;

        GameServerTimer ServerTimer;


        public ObjectService ObjectFactory;
        public Dictionary<Type, GameServerComponent> Components;
        public ObjectComponent ObjectPulseController 
            => Components[typeof(ObjectComponent)] as ObjectComponent;

        public GameServer(int capacity)
            : base(capacity)
        {
            Frames = ServerContext.Config.FRAMES;

            ServerUpdateSpan = TimeSpan.FromSeconds(1.0 / Frames);
            ClientUpdateSpan = TimeSpan.FromSeconds(1.0 / Frames / 2);

            InitializeGameServer();
        }

        private void AutoSave(GameClient client)
        {
            if ((DateTime.UtcNow - client.LastSave)
                .TotalSeconds > ServerContext.Config.SaveRate)
                client.Save();
        }

        private void DoClientWork()
        {
            isRunning = true;
            lastClientUpdate = DateTime.UtcNow;

            while (isRunning)
            {
                var delta = DateTime.UtcNow - lastClientUpdate;
                {
                    ExecuteClientWork(delta);
                    lastClientUpdate = DateTime.UtcNow;
                }
                Thread.Sleep(ClientUpdateSpan);
            }
        }
    

        private void DoServerWork()
        {
            isRunning = true;
            lastServerUpdate = DateTime.UtcNow;

            while (isRunning)
            {
                var delta = DateTime.UtcNow - lastServerUpdate;
                {
                    ExecuteServerWork(delta);
                    lastServerUpdate = DateTime.UtcNow;
                }
                Thread.Sleep(ServerUpdateSpan);
            }
        }

        public void InitializeGameServer()
        {
            ServerTimer = new GameServerTimer(TimeSpan.FromMilliseconds(ServerContext.Config.MinimalLatency));
            ObjectFactory = new ObjectService();

            Components = new Dictionary<Type, GameServerComponent>();
            Components[typeof(MonolithComponent)] = new MonolithComponent(this);
            Components[typeof(DaytimeComponent)] = new DaytimeComponent(this);
            Components[typeof(MundaneComponent)] = new MundaneComponent(this);
            Components[typeof(MessageComponent)] = new MessageComponent(this);
            Components[typeof(ObjectComponent)] = new ObjectComponent(this);
            Components[typeof(PingComponent)] = new PingComponent(this);
            Components[typeof(ServerCacheComponent)] = new ServerCacheComponent(this);

            Console.WriteLine(Components.Count + " Server Components loaded.");
        }

        public void ExecuteClientWork(TimeSpan elapsedTime)
        {
            UpdateClients(elapsedTime);
        }

        public void ExecuteServerWork(TimeSpan elapsedTime)
        {
            ServerTimer.Update(elapsedTime);

            if (ServerTimer.Elapsed)
            {
                UpdateAreas(elapsedTime);
                UpdateComponents(elapsedTime);
            }
        }

        private void UpdateComponents(TimeSpan elapsedTime)
        {
            foreach (var component in Components.Values)
            {
                component.Update(elapsedTime);
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
                            obj.AbandonedDate = DateTime.UtcNow;
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


            new TaskFactory().StartNew(DoClientWork);
            new TaskFactory().StartNew(DoServerWork);

            isRunning = true;
        }
    }
}