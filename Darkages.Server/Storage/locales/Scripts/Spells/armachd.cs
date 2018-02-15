﻿using System;
using System.Linq;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Storage.locales.buffs;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Spells
{
    [Script("armachd", "Dean")]
    public class armachd : SpellScript
    {
        private readonly Random rand = new Random();

        public armachd(Spell spell) : base(spell)
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
                 
                client.TrainSpell(Spell);

                var buff = Clone(Spell.Template.Buff);

                if (target.Buffs.FirstOrDefault(i => i.Name.Equals(buff.Name, StringComparison.OrdinalIgnoreCase)) == null)
                {
                    buff.OnApplied(target, buff);

                    if (target is Aisling)
                        (target as Aisling).Client
                            .SendMessage(0x02,
                                string.Format("{0} casts {1} on you.", client.Aisling.Username,
                                    Spell.Template.Name));

                    client.SendMessage(0x02, string.Format("you cast {0}", Spell.Template.Name));
                    client.SendAnimation(Spell.Template.Animation, target, sprite);

                    var action = new ServerFormat1A
                    {
                        Serial = sprite.Serial,
                        Number = 0x80,
                        Speed = 30
                    };

                    var hpbar = new ServerFormat13
                    {
                        Serial = sprite.Serial,
                        Health = 255,
                        Sound = 1
                    };

                    client.Aisling.Show(Scope.NearbyAislings, action);
                    client.Aisling.Show(Scope.NearbyAislings, hpbar);
                }
                else
                {
                    client.SendMessage(0x02, "You already cast this.");
                }
            }
            else
            {
                if (!(target is Aisling))
                    return;

                var client = (target as Aisling).Client;
                var buff = Clone(Spell.Template.Debuff);

                if (target.Buffs.FirstOrDefault(i => i.Name.Equals(buff.Name, StringComparison.OrdinalIgnoreCase)) == null)
                {
                    buff.OnApplied(target, buff);

                    (target as Aisling).Client
                        .SendMessage(0x02,
                            string.Format("{0} Attacks you with {1}.",
                                (sprite is Monster
                                    ? (sprite as Monster).Template.Name
                                    : (sprite as Mundane).Template.Name) ?? "Monster",
                                Spell.Template.Name));

                    client.SendAnimation(Spell.Template.Animation, target, sprite);

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
                        Sound = 1
                    };

                    client.Aisling.Show(Scope.NearbyAislings, action);
                    client.Aisling.Show(Scope.NearbyAislings, hpbar);

                }
            }
        }
    

        public override void OnUse(Sprite sprite, Sprite target)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;
                if (client.Aisling.CurrentMp >= Spell.Template.ManaCost)
                {
                    client.Aisling.CurrentMp -= Spell.Template.ManaCost;
                    if (client.Aisling.CurrentMp < 0)
                        client.Aisling.CurrentMp = 0;

                    OnSuccess(sprite, target);
                }
                else
                {
                    client.SendMessage(0x02, "your will is to weak.");
                }

                client.SendStats(StatusFlags.StructB);
            }
            else
            {
                if (!(target is Aisling))
                    return;

                var client = (target as Aisling).Client;
                var buff = Clone(Spell.Template.Buff);

                if (sprite.Buffs.FirstOrDefault(i => i.Name == buff.Name) == null)
                {
                    buff.OnApplied(sprite, buff);
                    client.SendAnimation(244, sprite, sprite);
                }
            }
        }
    }
}