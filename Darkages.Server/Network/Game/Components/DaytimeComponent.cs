using Darkages.Network.ServerFormats;
using System;

namespace Darkages.Network.Game.Components
{
    public class DaytimeComponent : GameServerComponent
    {
        private GameServerTimer timer;
        private byte shade = 0;

        public DaytimeComponent(GameServer server)
            : base(server)
        {
            this.timer = new GameServerTimer(
                TimeSpan.FromSeconds(ServerContext.Config.DayTimeInterval));
        }

        public override void Update(TimeSpan elapsedTime)
        {
            this.timer.Update(elapsedTime);

            if (this.timer.Elapsed)
            {
                this.timer.Reset();

                var format20 = new ServerFormat20 { Shade = this.shade };

                lock (this.Server.Clients)
                {
                    foreach (var client in this.Server.Clients)
                    {
                        if (client != null)
                        {
                            client.Send(format20);
                        }
                    }
                }

                this.shade += 1;
                this.shade %= 18;
            }
        }
    }
}