using Darkages.Network.Game;
using Darkages.Network.Object;
using System;

namespace Darkages.Scripting
{
    public abstract class MapScript : ObjectManager
    {
        public Area Area { get; set; }

        public MapScript(Area area)
        {
            Area = area;
        }

        public abstract void OnClick(GameClient client, int x, int y);
        public abstract void OnEnter(GameClient client);
        public abstract void OnLeave(GameClient client);
        public abstract void OnStep(GameClient client);
        public abstract void Update(TimeSpan elapsedTime);
    }
}
