using Darkages.Common;
using Darkages.Scripting;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using Darkages.Storage.locales.Buffs;
using Darkages.Storage.locales.debuffs;

namespace Darkages.Types
{
    public class Spell
    {
        public int ID { get; set; }

        [JsonIgnore]
        public SpellScript Script { get; set; }

        public SpellTemplate Template { get; set; }

        public byte Level { get; set; }

        public byte Slot { get; set; }

        public DateTime NextAvailableUse { get; set; }

        public bool Ready => DateTime.UtcNow > NextAvailableUse;

        public bool InUse { get; internal set; }

        public bool CanUse() => Ready;

        public int Casts = 0;

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

        [JsonIgnore]
        [Browsable(false)]
        public int Lines { get; set; }

        public static Spell Create(int slot, SpellTemplate spellTemplate)
        {
            var obj = new Spell();
            lock (Generator.Random)
            {
                obj.ID = Generator.GenerateNumber();
            }
            obj.Template = spellTemplate;
            obj.Level = 0;
            obj.Slot = (byte)slot;
            obj.Lines = obj.Template.BaseLines;



            if (obj.Template.Name == "dion")
                obj.Template.Buff = new buff_dion();

            if (obj.Template.Name == "pramh")
                obj.Template.Debuff = new debuff_sleep();

            if (obj.Template.Name == "beag cradh")
                obj.Template.Debuff = new debuff_beagcradh();

            if (obj.Template.Name == "cradh")
                obj.Template.Debuff = new debuff_cradh();

            if (obj.Template.Name == "mor cradh")
                obj.Template.Debuff = new debuff_morcradh();

            if (obj.Template.Name == "ard cradh")
                obj.Template.Debuff = new debuff_ardcradh();

            return obj;
        }
    }
}
