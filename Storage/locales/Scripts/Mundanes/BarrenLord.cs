using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;
using System.Collections.Generic;

namespace Darkages.Storage.locales.Scripts.Mundanes
{
    [Script("Barren Lord")]
    public class BarrenLord : MundaneScript
    {
        public BarrenLord(GameServer server, Mundane mundane)
            : base(server, mundane)
        {
        }

        public override void OnClick(GameServer server, GameClient client)
        {
            var options = new List<OptionsDataItem>();
            options.Add(new OptionsDataItem(0x0001, "Yes, Lord Barren"));
            options.Add(new OptionsDataItem(0x0002, "No."));

            client.SendOptionsDialog(base.Mundane, "You seek redemption?", options.ToArray());

        }
        public override void OnResponse(GameServer server, GameClient client, short responseID, string args)
        {
            if (responseID == 0x0001)
            {
                client.SendOptionsDialog(base.Mundane, "You dare pay the costs?",
                                        new OptionsDataItem(0x0005, "Yes"),
                                        new OptionsDataItem(0x0001, "No"));
            }

            if (responseID == 0x0005)
            {
                client.Aisling.MaximumHp -= ServerContext.Config.DeathHPPenalty;

                if (client.Aisling.MaximumHp <= 0)
                    client.Aisling.MaximumHp = ServerContext.Config.MinimumHp;

                client.Aisling.CurrentHp = client.Aisling.MaximumHp;

                client.SendStats(StatusFlags.All);
                client.WarpTo(ServerContext.GlobalWarpTemplateCache[509][0]);
            }
        }
    }
}
