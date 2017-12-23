using System;
using System.Linq;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    [Script("Kelberoth Strike", "Dean")]
    public class KelberothStrike : SkillScript
    {
        public Skill _skill;
        public Random rand = new Random();
        public Sprite Target;

        public KelberothStrike(Skill skill) : base(skill)
        {
            _skill = skill;
        }

        public override void OnFailed(Sprite sprite)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;

                client.SendMessage(0x02,
                    !string.IsNullOrEmpty(Skill.Template.FailMessage) ? Skill.Template.FailMessage : "failed.");
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
                    Number = 0x82,
                    Speed = 30
                };

                var enemy = client.Aisling.GetInfront(1);

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

                        var dmg = Convert.ToInt32(client.Aisling.CurrentHp / 3);
                        i.ApplyDamage(sprite, dmg, true);

                        sprite.CurrentHp -= dmg * 2;
                        ((Aisling) sprite).Client.SendStats(StatusFlags.StructB);


                        //probably should calculate the percent after we do some damage. not before.
                        //response to send hpbar to client.
                        var hpbar = new ServerFormat13
                        {
                            Serial = i.Serial,
                            Health = 255,
                            Sound = Skill.Template.Sound
                        };

                        //send hpbar to client
                        client.Aisling.Show(Scope.NearbyAislings, hpbar);


                        if (i is Monster)
                        {
                            (i as Monster).Target = client.Aisling;
                        }

                        if (i is Aisling)
                        {
                            (i as Aisling).Client.Aisling.Show(Scope.NearbyAislings,
                                new ServerFormat29((uint) client.Aisling.Serial, (uint) i.Serial, byte.MinValue,
                                    Skill.Template.TargetAnimation, 100));
                            (i as Aisling).Client.Send(new ServerFormat08(i as Aisling, StatusFlags.All));
                        }
                        if (i is Monster || i is Mundane || i is Aisling)
                            client.Aisling.Show(Scope.NearbyAislings,
                                new ServerFormat29((uint) client.Aisling.Serial, (uint) i.Serial,
                                    Skill.Template.TargetAnimation, 0, 100));
                    }
                    client.Aisling.Show(Scope.NearbyAislings, action);
                }
            }
        }

        public override void OnUse(Sprite sprite)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;

                if (Skill.Ready)
                {
                    client.TrainSkill(Skill);
                    if (client.Aisling.Invisible)
                    {
                        client.Aisling.Flags = AislingFlags.Normal;
                        client.Refresh();
                    }

                    client.Send(new ServerFormat3F(1, Skill.Slot, Skill.Template.Cooldown));

                    if (rand.Next(1, 101) < Skill.Level)
                        OnSuccess(sprite);
                    else
                        OnFailed(sprite);
                }
            }
        }
    }
}