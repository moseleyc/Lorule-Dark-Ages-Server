using Darkages.Common;
using Darkages.Network.Game;
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
            }
        }

        public override void OnSuccess(Sprite sprite)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;
                var target = client.Aisling.GetInfront(5).FirstOrDefault(i =>
                    i != null && ((i is Aisling) || (i is Monster)));

                if (target != null)
                {
                    var blocks = target.Position.SurroundingContent(client.Aisling.Map);

                    Position prev = client.Aisling.Position;
                    Position targetPosition = null;


                    if (blocks.Length > 0)
                    {
                        var selections = blocks.Where(i => i.Content == TileContent.None).ToArray();
                        var selection = (int)Generator.Random.Next(selections.Count());

                        if (selections.Length == 0)
                        {
                            client.SendMessageBox(0x02, "You can't do that.");
                            return;
                        }
                        targetPosition = selections[selection]?.Position;
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
                            client.Aisling.Turn();
                        }


                        client.Refresh();
                        return;
                    }
                }
            }
        }

        public override void OnUse(Sprite sprite)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;
                if (client.Aisling != null && !client.Aisling.Dead)
                {
                    client.Send(new ServerFormat3F(1, Skill.Slot, Skill.Template.Cooldown));
                    OnSuccess(sprite);
                }
            }
        }
    }
}
