using System;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;
using System.Linq;

namespace Darkages.Storage.locales.Scripts.Skills
{
    [Script("wff", "Dean")]
    public class Wff : SkillScript
    {
        public Skill _skill;

        public Wff(Skill skill) : base(skill)
        {
            _skill = skill;
        }

        public override void OnFailed(Sprite sprite)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;

                client.SendMessage(0x02,
                    !String.IsNullOrEmpty(Skill.Template.FailMessage) ? Skill.Template.FailMessage : "failed.");
            }

        }

        public override void OnSuccess(Sprite sprite)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;

                var debuff = Clone<Debuff>(Skill.Template.Debuff);

                var i = sprite.GetInfront(sprite);

                if (i == null || i.Count == 0)
                {
                    client.SendMessage(0x02, "you have embarrassed yourself.");
                    return;
                }


                foreach (var target in i)
                {
                    if (target is Money)
                        continue;
                    if (target is Item)
                        continue;

                    target.RemoveDebuff("sleep");
                    target.RemoveDebuff("frozen");

                    if (target.Debuffs.FirstOrDefault(n => n.Name == debuff.Name) == null)
                    {
                        Apply(client, debuff, target);
                        return;
                    }
                }
            }
        }

        private void Apply(Network.Game.GameClient client, Debuff debuff, Sprite target)
        {
            var action = new ServerFormat1A
            {
                Serial = client.Aisling.Serial,
                Number = 0x02,
                Speed = 40
            };
            client.Aisling.Show(Scope.NearbyAislings, action);

            target.ApplyDamage(client.Aisling, 0, false, Skill.Template.Sound);
            debuff.OnApplied(target, debuff);
            return;
        }

        private Random rand = new Random();
        public override void OnUse(Sprite sprite)
        {
            if (sprite is Aisling && Skill.Ready)
            {
                var client = (sprite as Aisling).Client;
                client.TrainSkill(Skill);

                if (client.Aisling != null && !client.Aisling.Dead)
                {
                    client.Send(new ServerFormat3F(1, Skill.Slot, Skill.Template.Cooldown));

                    if (rand.Next(1, 101) < Skill.Level)
                        OnSuccess(sprite);
                    else
                    {
                        OnFailed(sprite);
                    }
                }
            }
            else
            {
                var target = sprite.Target;
                if (target == null)
                    return;
                if (target is Aisling)
                {
                    var debuff = Clone<Debuff>(Skill.Template.Debuff);

                    if (target.Debuffs.FirstOrDefault(n => n.Name == debuff.Name) == null)
                    {
                        Apply((target as Aisling)?.Client, debuff, target);
                    }
                }
            }
        }
    }
}
