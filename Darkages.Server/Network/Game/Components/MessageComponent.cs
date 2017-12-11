using System;

namespace Darkages.Network.Game.Components
{
    public class MessageComponent : GameServerComponent
    {
        public GameServerTimer Timer { get; set; }

        public MessageComponent(GameServer server)
            : base(server)
        {
            this.Timer = new GameServerTimer(
                TimeSpan.FromSeconds(ServerContext.Config.MessageClearInterval));
        }

        public override void Update(TimeSpan elapsedTime)
        {
            this.Timer.Update(elapsedTime);

            if (this.Timer.Elapsed)
            {
                this.Timer.Reset();

                foreach (GameClient client in base.Server.Clients)
                {
                    if (client != null &&
                        client.Aisling != null)
                    {
                        if ((DateTime.UtcNow - client.LastMessageSent).TotalSeconds > 5)
                        {
                            client.SendMessage(0x01, "\0");
                        }
                    }
                }
            }
        }
    }
}