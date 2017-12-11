using Darkages.Network.Game;
using Darkages.Network.Object;
using Darkages.Types;

namespace Darkages.Scripting
{
    public abstract class SkillScript : ObjectManager
    {
        public Skill Skill { get; set; }

        public SkillScript(Skill skill) 
        {
            this.Skill = skill;
        }

        public abstract void OnUse(Sprite sprite);
        public abstract void OnFailed(Sprite sprite);
        public abstract void OnSuccess(Sprite sprite);
    }
}
