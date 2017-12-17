using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Mundanes
{
    [Script("Arena Master")]
    public class ArenaMaster : MundaneScript
    {
        public ArenaMaster(GameServer server, Mundane mundane)
            : base(server, mundane)
        {
        }

        public override void TargetAcquired(Sprite Target)
        {
        }

        public override void OnGossip(GameServer server, GameClient client, string message)
        {
        }

        public override void OnClick(GameServer server, GameClient client)
        {
            if (client.Aisling.Dead)
            {
                var options = new List<OptionsDataItem>();
                options.Add(new OptionsDataItem(0x0001, "Yes."));
                options.Add(new OptionsDataItem(0x0002, "No."));

                client.SendOptionsDialog(Mundane, "You seek redemption?", options.ToArray());
            }
        }

        public override void OnResponse(GameServer server, GameClient client, ushort responseID, string args)
        {
            if (responseID == 0x0001)
                client.SendOptionsDialog(Mundane, "Beg for Life.",
                    new OptionsDataItem(0x0005, "Please my lord."),
                    new OptionsDataItem(0x0001, "No, Fuck you."));

            if (responseID == 0x0005)
            {
                client.Aisling.CurrentHp = client.Aisling.MaximumHp;
                client.SendStats(StatusFlags.All);
            }
        }
    }
}