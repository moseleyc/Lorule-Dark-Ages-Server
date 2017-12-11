using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Types;
using System;
using System.Linq;

namespace Darkages.Scripting.Scripts.Skills
{
    [Script("DoublePunch", "Dean")]
    public class DoublePunch : SkillScript
    {
        public Skill _skill;

        Random rand = new Random();

        public Sprite Target;

        public DoublePunch(Skill skill) : base(skill)
        {
            _skill = skill;
        }

        public override void OnFailed(Sprite sprite)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;
                if (Target != null)
                {
                    client.Aisling.Show(Scope.NearbyAislings, (new ServerFormat29(Skill.Template.MissAnimation, (ushort)Target.X, (ushort)Target.Y)));
                }
            }
        }

        public override void OnSuccess(Sprite sprite)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;
                var action = new ServerFormat1A
                {
                    Serial = client.Aisling.Serial,
                    Number = 0x84,
                    Speed = 30
                };

                //test
                var enemy = client.Aisling.GetInfront();

                if (enemy != null)
                {
                    foreach (var i in enemy.Cast<Sprite>())
                    {
                        if (i == null)
                            continue;


                        if (client.Aisling.Serial == i.Serial)
                            continue;

                        if (i is Money)
                            continue;

                        Target = i;

                        var dmg = (int)(Skill.Level + 1 * 100 / 60) * (client.Aisling.Str + client.Aisling.Dex) * 2;
                        i.ApplyDamage(sprite, dmg);

                        var hpbar = new ServerFormat13
                        {
                            Serial = i.Serial,
                            Health = (ushort)((double)100 * i.CurrentHp / (double)i.MaximumHp),
                            Sound = Skill.Template.Sound
                        };

                        //send hpbar to client
                        client.Aisling.Show(Scope.NearbyAislings, hpbar);

                        if (i is Monster)
                        {
                            (i as Monster).Target = client.Aisling;
                            (i as Monster).Attacked = true;

                            //Monster Dead!
                            if (i.CurrentHp == 0)
                            {
                                var obj = GetObject<Monster>(o => o.Serial == i.Serial);
                                if (obj != null && obj is Monster)
                                    obj.Remove<Monster>();
                            }
                        }

                        if (i is Aisling)
                        {
                            (i as Aisling).Client.Aisling.Show(Scope.NearbyAislings, (new ServerFormat29((uint)client.Aisling.Serial, (uint)i.Serial, byte.MinValue, Skill.Template.TargetAnimation, 100)));
                            (i as Aisling).Client.Send(new ServerFormat08(i as Aisling, StatusFlags.All));
                        }
                        if (i is Monster || i is Mundane || i is Aisling)
                        {
                            client.Aisling.Show(Scope.NearbyAislings, (new ServerFormat29((uint)client.Aisling.Serial, (uint)i.Serial, Skill.Template.TargetAnimation, 0, 100)));
                        }

                    }
                }
                client.Aisling.Show(Scope.NearbyAislings, action);
            }
        }

        public override void OnUse(Sprite sprite)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;
                if (Skill.Ready)
                {
                    if (client.Aisling.Invisible)
                    {
                        client.Aisling.Flags = AislingFlags.Normal;
                        client.Refresh();
                    }

                    client.Send(new ServerFormat3F(1, Skill.Slot, Skill.Template.Cooldown));

                    OnSuccess(sprite);
                }
            }
        }
    }
}