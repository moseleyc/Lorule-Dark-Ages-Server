using Darkages.Common;
using Darkages.Scripting;
using Darkages.Storage.locales.debuffs;
using Newtonsoft.Json;
using System;

namespace Darkages.Types
{

    public class Skill
    {
        [JsonIgnoreAttribute]
        public SkillScript Script { get; set; }

        public SkillTemplate Template { get; set; }

        public byte Slot { get; set; }
        public byte Icon { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int ID { get; set; }

        public DateTime NextAvailableUse { get; set; }

        public bool Ready => DateTime.UtcNow > NextAvailableUse;

        public bool InUse { get; internal set; }

        public bool CanUse() => Ready;

        public static Skill Create(int slot, SkillTemplate skillTemplate)
        {
            var obj = new Skill();
            obj.Template = skillTemplate;
            obj.Level = 0;
            lock (Generator.Random)
            {
              obj.ID = Generator.GenerateNumber();
            }
            obj.Slot = (byte)slot;
            obj.Icon = skillTemplate.Icon;            
            obj.Name = string.Format("{0} Lev {1}/{2}",
                obj.Template.Name,
                obj.Level,
                obj.Template.MaxLevel);

            if (obj.Template.Name == "Wolf Fang Fist")
            {
                obj.Template.Debuff = new debuff_frozen();
            }

            return obj;
        }
    }    
}