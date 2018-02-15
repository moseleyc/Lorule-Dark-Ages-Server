﻿using System;
using System.ComponentModel;
using Darkages.Common;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Storage.locales.debuffs;
using Newtonsoft.Json;

namespace Darkages.Types
{
    public class Skill
    {
        [JsonIgnore] public SkillScript Script { get; set; }

        public SkillTemplate Template { get; set; }

        public byte Slot { get; set; }
        public byte Icon { get; set; }

        [JsonIgnore]
        [Browsable(false)] public string Name => string.Format("{0} (Lev:{1}/{2})",
            Template.Name,
            Level, Template.MaxLevel);

        public int Level { get; set; }
        public int ID { get; set; }

        public DateTime NextAvailableUse { get; set; }

        public bool Ready => DateTime.UtcNow > NextAvailableUse;

        public bool InUse { get; internal set; }
        public int Uses { get; set; }

        public bool CanUse()
        {
            return Ready;
        }

        public bool RollDice(Random rand)
        {
            if (Level < 50)
                return rand.Next(1, 101) < 50;

            return rand.Next(1, 101) < Level;
        }

        public static Skill Create(int slot, SkillTemplate skillTemplate)
        {
            var obj = new Skill();
            obj.Template = skillTemplate;
            obj.Level = 0;
            lock (Generator.Random)
            {
                obj.ID = Generator.GenerateNumber();
            }

            obj.Slot = (byte) slot;
            obj.Icon = skillTemplate.Icon;

            AssignDebuffs(obj);

            return obj;
        }

        private static void AssignDebuffs(Skill obj)
        {
            if (obj.Template.Name == "Wolf Fang Fist") obj.Template.Debuff = new debuff_frozen();
        }

        public static bool GiveTo(GameClient client, string args)
        {
            var skillTemplate = ServerContext.GlobalSkillTemplateCache[args];
            var slot = client.Aisling.SkillBook.FindEmpty();

            if (slot <= 0)
                return false;

            var skill = Create(slot, skillTemplate);
            skill.Script = ScriptManager.Load<SkillScript>(skill.Template.ScriptName, skill);
            client.Aisling.SkillBook.Assign(skill);
            client.Aisling.SkillBook.Set(skill, false);
            client.Send(new ServerFormat2C(skill.Slot, skill.Icon, skill.Name));

            return true;
        }

        public static bool GiveTo(Aisling aisling, string args, byte slot)
        {
            var skillTemplate = ServerContext.GlobalSkillTemplateCache[args];
            var skill = Create(slot, skillTemplate);
            skill.Script = ScriptManager.Load<SkillScript>(skill.Template.ScriptName, skill);
            aisling.SkillBook.Assign(skill);

            return true;
        }

        public static bool GiveTo(Aisling aisling, string args)
        {
            var skillTemplate = ServerContext.GlobalSkillTemplateCache[args];
            var slot = aisling.SkillBook.FindEmpty();

            if (slot <= 0)
                return false;

            var skill = Create(slot, skillTemplate);
            skill.Level = 1;
            skill.Script = ScriptManager.Load<SkillScript>(skill.Template.ScriptName, skill);
            aisling.SkillBook.Assign(skill);

            return true;
        }
    }
}