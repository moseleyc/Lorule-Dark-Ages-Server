using Darkages.Scripting;
using Darkages.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkages.Storage.locales.Scripts.Spells
{
    [Script("Create", "Dean")]
    public class Create : SpellScript
    {
        public Create(Spell spell) : base(spell)
        {
        }

        public override void OnFailed(Sprite sprite, Sprite target)
        {

        }

        public override void OnSuccess(Sprite sprite, Sprite target)
        {

        }

        public override void OnUse(Sprite sprite, Sprite target)
        {
            var spellArgs = Arguments;

            if (!string.IsNullOrEmpty(spellArgs))
            {
                var exists = ServerContext.GlobalItemTemplateCache.Keys.FirstOrDefault(i 
                    => i.Equals(spellArgs, StringComparison.OrdinalIgnoreCase));

                if (exists != null)
                {
                    var template = ServerContext.GlobalItemTemplateCache[exists];
                    var offset = template.DisplayImage - 0x8000;
                    var item = Item.Create(sprite, template, false);

                    item.Release(sprite, sprite.Position);
                }
            }
        }
    }
}
