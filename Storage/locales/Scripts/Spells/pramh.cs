using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;
using System.Linq;

namespace Darkages.Storage.locales.Scripts.Spells
{
    [Script("pramh", "Dean")]
    public class pramh : SpellScript
    {
        public pramh(Spell spell) : base(spell)
        {
        }

        public override void OnFailed(Sprite sprite, Sprite target)
        {

        }

        public override void OnSuccess(Sprite sprite, Sprite target)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;

                var debuff = Clone<Debuff>(Spell.Template.Debuff);

                if (target.Debuffs.FirstOrDefault(i => i.Name == debuff.Name) == null)
                {
                    debuff.OnApplied(target, debuff);

                    if (target is Aisling)
                    {
                        (target as Aisling).Client.SendMessage(0x02,
                            string.Format("{0} Attacks you with {1}.", client.Aisling.Username, Spell.Template.Name));
                    }

                    client.SendMessage(0x02, string.Format("you cast {0}", Spell.Template.Name));

                    var action = new ServerFormat1A
                    {
                        Serial = client.Aisling.Serial,
                        Number = 0x80,
                        Speed = 30
                    };

                    client.SendAnimation(32, target, sprite);
                    client.Aisling.Show(Scope.NearbyAislings, action);
                }
            }
        }

        public override void OnUse(Sprite sprite, Sprite target)
        {
            OnSuccess(sprite, target);
        }
    }
}
