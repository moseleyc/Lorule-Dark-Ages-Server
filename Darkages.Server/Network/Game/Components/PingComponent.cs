using Darkages.Network.ServerFormats;
using System;

namespace Darkages.Network.Game.Components
{
    public class PingComponent : GameServerComponent
    {
        public GameServerTimer Timer { get; set; }

        public PingComponent(GameServer server)
            : base(server)
        {
            this.Timer = new GameServerTimer(
                TimeSpan.FromSeconds(ServerContext.Config.PingInterval));
        }

        public override void Update(TimeSpan elapsedTime)
        {
            this.Timer.Update(elapsedTime);

            if (this.Timer.Elapsed)
            {
                this.Timer.Reset();

                foreach (var client in base.Server.Clients)
                {
                    if (client != null &&
                        client.Aisling != null)
                    {
                        client.Send(new ServerFormat3B());
                        client.LastPing = DateTime.UtcNow;
                    }
                }
            }
        }
    }
}