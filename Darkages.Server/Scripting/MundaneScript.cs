using Darkages.Network.Game;
using Darkages.Network.Object;
using Darkages.Types;

namespace Darkages.Scripting
{
    public abstract class MundaneScript : ObjectManager
    {
        public GameServer Server { get; set; }
        public Mundane Mundane { get; set; }

        public MundaneScript(GameServer server, Mundane mundane)
        {
            this.Server = server;
            this.Mundane = mundane;
        }

        public abstract void OnClick(GameServer server, GameClient client);
        public abstract void OnResponse(GameServer server, GameClient client, short responseID, string args);
    }
}
