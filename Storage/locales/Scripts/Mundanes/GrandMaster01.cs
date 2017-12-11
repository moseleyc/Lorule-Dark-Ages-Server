using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Storage.locales.Buffs;
using Darkages.Storage.locales.debuffs;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Mundanes
{
    [Script("The Grand Master")]
    public class GrandMaster01 : MundaneScript
    {
        public GrandMaster01(GameServer server, Mundane mundane)
            : base(server, mundane)
        {

        }

        public override void OnClick(GameServer server, GameClient client)
        {
            client.SendOptionsDialog(base.Mundane, "What the fuck do you want?",
                new OptionsDataItem(0x0001, "I seek wisdom, Grand Master."));
        }
        public override void OnResponse(GameServer server, GameClient client, short responseID, string args)
        {
            switch (responseID)
            {
                case 0x0001:
                    var spells = ServerContext.GlobalSpellTemplateCache.Values;
                    client.SendSpellLearnDialog(base.Mundane, "You do believe you are worthy?", 0x0003, spells);
                    break;
                case 0x0003:
                    client.SendOptionsDialog(base.Mundane, "Are you sure, " + args + " is a powerful spell.", args,
                        new OptionsDataItem(0x0005, "Yes, Grand Master"),
                        new OptionsDataItem(0x0001, "No, I'm not worthy."));
                    break;
                case 0x0005:
                    var template = ServerContext.GlobalSpellTemplateCache[args];
                    var slot = 0;

                    for (int i = 0; i < client.Aisling.SpellBook.Length; i++)
                    {
                        if (client.Aisling.SpellBook.Spells[i + 1] == null)
                        {
                            slot = (i + 1);
                            break;
                        }
                    }

                    if (template.Name == "dion")
                        template.Buff = new buff_dion();

                    if (template.Name == "pramh")
                        template.Debuff = new debuff_sleep();


                    var spell = Spell.Create(slot, template);
                    spell.Script = ScriptManager.Load<SpellScript>(spell.Template.ScriptKey, spell);
                    client.Aisling.SpellBook.Assign(spell);
                    client.Aisling.SpellBook.Set(spell, false);
                    client.Aisling.Show(Scope.NearbyAislings, new ServerFormat29((uint)client.Aisling.Serial, (uint)Mundane.Serial, 0, 124, 100));
                    client.Send(new ServerFormat17(spell));
                    client.SendOptionsDialog(base.Mundane, "You are worthy. Now be gone, " + client.Aisling.Path.ToString() + ".");
                    break;
            }
        }
    }
}
