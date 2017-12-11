using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;
using System;
using System.Linq;
using Darkages.Storage.locales.debuffs;

namespace Darkages.Storage.locales.Scripts.Spells
{
    [Script("pramh", "Dean")]
    public class pramh : SpellScript
    {
        Random rand = new Random();

        public pramh(Spell spell) : base(spell)
        {
        }

        public override void OnFailed(Sprite sprite, Sprite target)
        {
            if (sprite is Aisling)
            {
                (sprite as Aisling)
                    .Client
                    .SendMessage(0x02, string.Format("Your spell has been deflected."));
                (sprite as Aisling)
                    .Client
                    .SendAnimation(33, target, sprite);
            }
            else
            {
                if (sprite is Monster)
                {
                    (sprite.Target as Aisling)?
                        .Client
                        .SendAnimation(33, sprite, target);
                }
            }
        }

        public override void OnSuccess(Sprite sprite, Sprite target)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;

                client.TrainSpell(Spell);

                var debuff = Clone<Debuff>(Spell.Template.Debuff);

                target.RemoveDebuff("frozen");

                if (target.Debuffs.FirstOrDefault(i => i.Name == debuff.Name) == null)
                {
                    debuff.OnApplied(target, debuff);

                    if (target is Aisling)
                    {
                        (target as Aisling).Client
                            .SendMessage(0x02,
                                string.Format("{0} Attacks you with {1}.", client.Aisling.Username,
                                    Spell.Template.Name));
                    }

                    client.SendMessage(0x02, string.Format("you cast {0}", Spell.Template.Name));
                    client.SendAnimation(32, target, sprite);

                    var action = new ServerFormat1A
                    {
                        Serial = sprite.Serial,
                        Number = 0x80,
                        Speed = 30
                    };

                    var hpbar = new ServerFormat13
                    {
                        Serial = client.Aisling.Serial,
                        Health = 255,
                        Sound = 8
                    };

                    client.Aisling.Show(Scope.NearbyAislings, action);
                    client.Aisling.Show(Scope.NearbyAislings, hpbar);
                }
                else
                {
                    client.SendMessage(0x02, string.Format("They are sleeping already."));
                }
            }
            else
            {
                if (!(target is Aisling))
                    return;

                var client = (target as Aisling).Client;
                var debuff = Clone<Debuff>(Spell.Template.Debuff);
                var curses = target.Debuffs.OfType<debuff_sleep>().ToList();

                if (curses.Count == 0)
                {
                    if (target.Debuffs.FirstOrDefault(i => i.Name == debuff.Name) == null)
                    {
                        debuff.OnApplied(target, debuff);

                        (target as Aisling).Client
                            .SendMessage(0x02,
                                string.Format("{0} Attacks you with {1}.",
                                    (sprite is Monster ? (sprite as Monster).Template.Name : (sprite as Mundane).Template.Name) ?? "Monster",
                                    Spell.Template.Name));

                        client.SendAnimation(32, target, sprite);

                        var action = new ServerFormat1A
                        {
                            Serial = sprite.Serial,
                            Number = 0x80,
                            Speed = 30
                        };

                        var hpbar = new ServerFormat13
                        {
                            Serial = target.Serial,
                            Health = 255,
                            Sound = 27
                        };

                        client.Aisling.Show(Scope.NearbyAislings, action);
                        client.Aisling.Show(Scope.NearbyAislings, hpbar);
                    }
                }
            }
        }

        public override void OnUse(Sprite sprite, Sprite target)
        {

            if (sprite.CurrentMp - Spell.Template.ManaCost > 0)
                sprite.CurrentMp -= Spell.Template.ManaCost;

            if (sprite.CurrentMp < 0)
                sprite.CurrentMp = 0;

            if (rand.Next(0, 100) > target.Mr)
            {
                OnSuccess(sprite, target);
            }
            else
            {
                OnFailed(sprite, target);
            }

            if (sprite is Aisling)
                (sprite as Aisling)
                    .Client
                    .SendStats(StatusFlags.StructB);

        }
    }
}
