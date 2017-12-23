using System;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Types;

namespace Darkages.Scripting.Scripts
{
    [Script("Lorule_Tour1", "Dean")]
    public class Tour : MapScript
    {
        public static Random Rand = new Random();

        public Tour(Area area)
            : base(area)
        {
            Timer = new GameServerTimer(TimeSpan.FromMilliseconds(ServerContext.Config.MapUpdateInterval));
        }

        public GameServerTimer Timer { get; set; }

        private ushort animation => 57;

        public override void OnClick(GameClient client, int x, int y)
        {
        }

        public override void OnEnter(GameClient client)
        {
        }

        public override void OnLeave(GameClient client)
        {
        }

        public override void OnStep(GameClient client)
        {
            var position = new Position(client.Aisling.X, client.Aisling.Y);

            if (!ServerContext.GlobalWarpTemplateCache.ContainsKey(Area.ID))
                return;

            foreach (var warps in ServerContext.GlobalWarpTemplateCache[Area.ID])
                if (warps.Location.DistanceFrom(position) <= warps.WarpRadius)
                    client.WarpTo(warps);
        }

        public override void Update(TimeSpan elapsedTime)
        {
            Timer.Update(elapsedTime);

            if (Timer.Elapsed)
            {
                Timer.Reset();
            }
        }
    }
}