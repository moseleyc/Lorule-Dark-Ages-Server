using System.Collections.Generic;
using System.Linq;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Mundanes
{
    [Script("shop1", "Dean")]
    public class shop1 : MundaneScript
    {
        public shop1(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        public override void OnGossip(GameServer server, GameClient client, string message)
        {
        }

        public override void TargetAcquired(Sprite Target)
        {
        }


        public override void OnClick(GameServer server, GameClient client)
        {
            var opts = new List<OptionsDataItem>();
            opts.Add(new OptionsDataItem(0x0001, "Buy"));
            opts.Add(new OptionsDataItem(0x0002, "Sell"));
            opts.Add(new OptionsDataItem(0x0003, "Repair Items"));

            client.SendOptionsDialog(Mundane, "What you looking for?", opts.ToArray());
        }

        public override void OnResponse(GameServer server, GameClient client, ushort responseID, string args)
        {
            switch (responseID)
            {
                case 0x0001:
                    client.SendItemShopDialog(Mundane, "Have a browse!", 0x0004,
                        ServerContext.GlobalItemTemplateCache.Values.ToList());
                    break;
                case 0x0002:
                    client.SendItemSellDialog(Mundane, "What do you want to pawn?", 0x0005,
                        client.Aisling.Inventory.Items.Values.Where(i => i != null && i.Template != null)
                            .Select(i => i.Slot).ToList());

                    break;

                #region Buy

                case 0x0004:
                {
                    if (string.IsNullOrEmpty(args))
                        return;

                    if (!ServerContext.GlobalItemTemplateCache.ContainsKey(args))
                        return;

                    var template = ServerContext.GlobalItemTemplateCache[args];
                    if (template != null)
                        if (client.Aisling.GoldPoints >= template.Value)
                        {
                            //Create Item:
                            var item = Item.Create(client.Aisling, template);
                            item.GiveTo(client.Aisling);

                            client.Aisling.GoldPoints -= (int) template.Value;
                            if (client.Aisling.GoldPoints < 0)
                                client.Aisling.GoldPoints = 0;

                            client.SendStats(StatusFlags.All);
                            client.SendOptionsDialog(Mundane, string.Format("You have a brand new {0}", args));
                        }
                        else
                        {
                            var script = ScriptManager.Load<SpellScript>("beag cradh",
                                Spell.Create(1, ServerContext.GlobalSpellTemplateCache["beag cradh"]));
                            script.OnUse(Mundane, client.Aisling);
                            client.SendOptionsDialog(Mundane, "You trying to rip me off?! go away.");
                        }
                }
                    break;

                #endregion
            }
        }
    }
}