using System;
using Darkages.Common;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;
using System.Linq;

namespace Darkages.Storage.locales.Scripts.Skills
{
    [Script("Ambush", "Dean")]
    public class Ambush : SkillScript
    {
        public Skill _skill;

        public Ambush(Skill skill) : base(skill)
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

                var targets = client.Aisling.GetInfront(3).ToList();
                Position prev = client.Aisling.Position;
                Position targetPosition = null;

                if (targets != null && targets.Count > 0)
                {
                    foreach (var target in targets)
                    {
                        if (target == null)
                            continue;

                        if (target is Money)
                            continue;

                        if (target is Item)
                            continue;

                        if (target.Serial == client.Aisling.Serial)
                            continue;

                        var blocks = target.Position.SurroundingContent(client.Aisling.Map);



                        if (blocks.Length > 0)
                        {
                            var selections = blocks.Where(i => i.Content == 
                            TileContent.Item 
                            || i.Content == TileContent.Money 
                            || i.Content == TileContent.None).
                            ToArray();
                            var selection = selections.OrderByDescending(i => i.Position.DistanceFrom(client.Aisling.Position)).FirstOrDefault();
                            if (selections.Length == 0 || selection == null)
                            {
                                client.SendMessageBox(0x02, "you can't do that.");
                                return;
                            }
                            targetPosition = selection.Position;
                        }


                        if (targetPosition != null)
                        {
                            client.Aisling.X = targetPosition.X;
                            client.Aisling.Y = targetPosition.Y;
                            client.Aisling.Map.Tile[prev.X, prev.Y] = TileContent.None;


                            int direction;
                            if (!client.Aisling.Facing(target.X, target.Y, out direction))
                            {
                                client.Aisling.Direction = (byte)direction;

                                if (client.Aisling.Position.IsNextTo(target.Position))
                                    client.Aisling.Turn();
                            }

                            client.Refresh();
                            return;
                        }
                    }
                }
            }
        }

        public Random rand = new Random();
        public override void OnUse(Sprite sprite)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;
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
        }
    }
}
