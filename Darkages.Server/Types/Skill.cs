using Darkages.Common;
using Darkages.Scripting;
using Darkages.Storage.locales.debuffs;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Darkages.Types
{

    public class Skill
    {
        [JsonIgnoreAttribute]
        public SkillScript Script { get; set; }

        public SkillTemplate Template { get; set; }

        public byte Slot { get; set; }
        public byte Icon { get; set; }

        [JsonIgnore]
        [Browsable(false)]
        public string Name
        {
            get
            {
                return string.Format("{0} Lev {1}/{2}",
                    Template.Name,
                    Level, Template.MaxLevel);
            }
        }

        public int Level { get; set; }
        public int ID { get; set; }

        public DateTime NextAvailableUse { get; set; }

        public bool Ready => DateTime.UtcNow > NextAvailableUse;

        public bool InUse { get; internal set; }
        public int Uses { get; set; }

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

            AssignDebuffs(obj);

            return obj;
        }

        private static void AssignDebuffs(Skill obj)
        {
            if (obj.Template.Name == "Wolf Fang Fist")
            {
                obj.Template.Debuff = new debuff_frozen();
            }
        }
    }    
}