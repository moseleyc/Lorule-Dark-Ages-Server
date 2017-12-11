using Darkages.Network.Game;
using Darkages.Network.Object;
using Darkages.Types;
using System;

namespace Darkages.Scripting
{
    public abstract class MonsterScript : ObjectManager
    {
        public Monster Monster { get; set; }
        public Area Map { get; set; }

        public MonsterScript(Monster monster, Area map)
        {
            this.Monster = monster;
            this.Map = map;
        }

        public abstract void OnApproach(GameClient client);
        public abstract void OnAttacked(GameClient client);
        public abstract void OnCast(GameClient client);
        public abstract void OnClick(GameClient client);
        public abstract void OnDeath(GameClient client);
        public abstract void OnLeave(GameClient client);
        public abstract void Update(TimeSpan elapsedTime);
    }
}
