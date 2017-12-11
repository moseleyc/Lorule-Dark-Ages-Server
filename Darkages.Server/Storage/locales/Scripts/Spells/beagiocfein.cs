using System.Linq;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Storage.locales.debuffs;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Spells
{
    [Script("beag ioc fein", "Dean")]
    public class beagiocfein : SpellScript
    {
        public beagiocfein(Spell spell) : base(spell)
        {

        }

        public override void OnFailed(Sprite sprite, Sprite target)
        {

        }

        public override void OnSuccess(Sprite sprite, Sprite target)
        {

        }

        public override void OnUse(Sprite sprite, Sprite target)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;
                if (client.Aisling.CurrentMp >= Spell.Template.ManaCost)
                {
                    client.TrainSpell(Spell);

                    var action = new ServerFormat1A
                    {
                        Serial = client.Aisling.Serial,
                        Number = 0x80,
                        Speed = 30
                    };


                    client.Aisling.CurrentHp += (client.Aisling.MaximumHp / 10);

                    if (client.Aisling.CurrentHp > client.Aisling.MaximumHp)
                        client.Aisling.CurrentHp = client.Aisling.MaximumHp;

                    client.Aisling.CurrentMp -= Spell.Template.ManaCost;

                    if (client.Aisling.CurrentMp < 0)
                        client.Aisling.CurrentMp = 0;

                    if (client.Aisling.CurrentHp > 0)
                    {
                        var hpbar = new ServerFormat13
                        {
                            Serial = client.Serial,
                            Health = (ushort) (100 * client.Aisling.CurrentHp / client.Aisling.MaximumHp),
                            Sound = 8
                        };
                        client.Aisling.Show(Scope.NearbyAislings, hpbar);
                    }

                    client.SendAnimation(0x04, client.Aisling, client.Aisling);
                    client.Aisling.Show(Scope.NearbyAislings, action);
                    client.SendMessage(0x02, "you cast " + Spell.Template.Name + ".");
                    client.SendStats(StatusFlags.All);
                }
            }
            else
            {
                if (!(target is Aisling))
                    return;

                var client = (target as Aisling).Client;
                sprite.CurrentHp = sprite.MaximumHp;

                var hpbar = new ServerFormat13
                {
                    Serial = sprite.Serial,
                    Health = (ushort) (100 * sprite.CurrentHp / sprite.MaximumHp),
                    Sound = 8
                };

                client.SendAnimation(0x04, sprite, sprite);
                client.Aisling.Show(Scope.NearbyAislings, hpbar);
            }
        }
    }
}
