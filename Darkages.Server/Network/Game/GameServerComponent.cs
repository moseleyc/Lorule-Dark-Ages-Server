using Darkages.Network.Object;
using Newtonsoft.Json;
using System;

namespace Darkages.Network.Game
{
    public abstract class GameServerComponent : ObjectManager
    {
        [JsonIgnore]
        public GameServer Server { get; private set; }

        public GameServerComponent(GameServer server)
        {
            this.Server = server;
        }

        public abstract void Update(TimeSpan elapsedTime);
    }
}