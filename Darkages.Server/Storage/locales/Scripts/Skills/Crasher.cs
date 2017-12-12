using System;
using System.Linq;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Types;

namespace Darkages.Scripting.Scripts.Skills
{
    [Script("Crasher", "Huy")]
    public class Crasher : SkillScript
    {
        private Skill _skill;
        private Random _rand = new Random();
        public Sprite Target;

        public Crasher(Skill skill) : base(skill)
        {
            _skill = skill;
        }

        public override void OnFailed(Sprite sprite)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;

                client.SendMessage(0x02,
                    String.IsNullOrEmpty(Skill.Template.FailMessage) ? Skill.Template.FailMessage : "failed.");
            }

        }

        public override void OnSuccess(Sprite sprite)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;
                client.TrainSkill(Skill);

                var action = new ServerFormat1A
                {
                    Serial = client.Aisling.Serial,
                    Number = 0x82,
                    Speed = 20
                };

                var enemy = client.Aisling.GetInfront(1);

                if (enemy == null) return;
                foreach (var i in enemy)
                {
                    if (i == null)
                        continue;
                    if (client.Aisling.Serial == i.Serial)
                        continue;
                    if (i is Money)
                        continue;

                    Target = i;

                    var dmg = (client.Aisling.CurrentHp * 300) / 100 + (99 * Skill.Level);
                    i.Target = client.Aisling;
                    i.ApplyDamage(sprite, dmg, false, 44);

                    if (i is Monster)
                    {
                        (i as Monster).Target = client.Aisling;
                        (i as Monster).Attacked = true;
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
                client.Aisling.CurrentHp = 1;
                client.SendStats(StatusFlags.All);
                client.Aisling.Show(Scope.NearbyAislings, action);
            }
        }
        public Random rand = new Random();
        public override void OnUse(Sprite sprite)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;
                if (Skill.Ready)
                {
                    if (client.Aisling.Invisible && Skill.Template.PostQualifers == PostQualifer.BreakInvisible)
                    {
                        client.Aisling.Flags = AislingFlags.Normal;
                        client.Refresh();
                    }
                    client.Send(new ServerFormat3F(1, Skill.Slot, Skill.Template.Cooldown));

                    if (rand.Next(1, 101) < Skill.Level)
                        OnSuccess(sprite);
                    else
                    {
                        OnFailed(sprite);
                    }
                }
            }
        }
    }
}