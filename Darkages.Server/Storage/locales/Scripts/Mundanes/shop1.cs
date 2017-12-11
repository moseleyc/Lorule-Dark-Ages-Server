using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;
using System.Collections.Generic;
using System.Linq;
using Darkages.Storage.locales.Scripts.Spells;

namespace Darkages.Storage.locales.Scripts.Mundanes
{
    [Script("shop1", "Dean")]
    public class shop1 : MundaneScript
    {
        public shop1(GameServer server, Mundane mundane) : base(server, mundane)
        {

        }

        public override void OnClick(GameServer server, GameClient client)
        {
            client.SendItemShopDialog(Mundane, "What you looking for?", 1, 
                ServerContext.GlobalItemTemplateCache.Values.Where(i => i.NpcKey == "shop1" ));
        }

        public override void OnResponse(GameServer server, GameClient client, short responseID, string args)
        {
            if (string.IsNullOrEmpty(args))
                return;

            if (!ServerContext.GlobalItemTemplateCache.ContainsKey(args))
                return;

            var template = ServerContext.GlobalItemTemplateCache[args];
            if (template != null)
            {
                if (client.Aisling.GoldPoints >= template.Value)
                {
                    //Create Item:
                    var item = Item.Create(client.Aisling, template);
                    item.GiveTo(client.Aisling);

                    client.Aisling.GoldPoints -= (int) template.Value;
                    if (client.Aisling.GoldPoints < 0)
                        client.Aisling.GoldPoints = 0;

                    client.SendStats(StatusFlags.All);
                }
                else
                {
                    var script = ScriptManager.Load<SpellScript>("beag cradh", Spell.Create(1, ServerContext.GlobalSpellTemplateCache["beag cradh"]));
                    script.OnUse(base.Mundane, client.Aisling);
                    client.SendOptionsDialog(base.Mundane, "You trying to rip me off?! go away.");
                }
            }
        }
    }
}
