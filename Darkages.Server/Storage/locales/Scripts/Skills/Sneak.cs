using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    [Script("Sneak", "Dean")]
    public class Sneak : SkillScript
    {
        public Skill _skill;

        public Sneak(Skill skill) : base(skill)
        {
            _skill = skill;
        }

        public override void OnFailed(Sprite sprite)
        {

        }

        public override void OnSuccess(Sprite sprite)
        {

        }

        public override void OnUse(Sprite sprite)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;

                if (client.Aisling != null && !client.Aisling.Dead)
                {
                    client.Aisling.Flags = client.Aisling.Flags == AislingFlags.Invisible ? AislingFlags.Normal : AislingFlags.Invisible;


                    client.Send(new ServerFormat3F(1, Skill.Slot, Skill.Template.Cooldown));
                    client.Refresh();
                }
            }
        }
    }
}
