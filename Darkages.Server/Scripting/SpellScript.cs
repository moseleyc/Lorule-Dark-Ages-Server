using Darkages.Network.Object;
using Darkages.Types;

namespace Darkages.Scripting
{
    public abstract class SpellScript : ObjectManager
    {
        public SpellScript(Spell spell)
        {
            Spell = spell;
        }

        public Spell Spell { get; set; }

        public abstract void OnUse(Sprite sprite, Sprite target);
        public abstract void OnFailed(Sprite sprite, Sprite target);
        public abstract void OnSuccess(Sprite sprite, Sprite target);
    }
}