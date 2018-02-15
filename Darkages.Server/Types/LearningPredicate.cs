﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Darkages.Types
{
    public class ItemPredicate
    {
        public string Item        { get; set; }
        public int AmountRequired { get; set; }
    }

    public class LearningPredicate
    {
        public ClassStage Stage_Required  { get; set; }
        public Class Class_Required       { get; set; }

        public int ExpLevel_Required      { get; set; }
        public int Str_Required           { get; set; }
        public int Int_Required           { get; set; }
        public int Wis_Required           { get; set; }
        public int Dex_Required           { get; set; }
        public int Con_Required           { get; set; }
        public int Gold_Required          { get; set; }

        public string Skill_Required      { get; set; }
        public int Skill_Level_Required   { get; set; }
        public int Skill_Tier_Required    { get; set; }

        public string Spell_Required      { get; set; }
        public int Spell_Level_Required   { get; set; }
        public int Spell_Tier_Required    { get; set; }

        public List<string> Quests_Completed_Required = new List<string>();
        public List<ushort> Areas_Visited_Required    = new List<ushort>();
        public List<ItemPredicate> Items_Required     = new List<ItemPredicate>();

        public bool IsMet(Aisling player, Action<string, bool> callbackMsg = null)
        {
            var result = new Dictionary<int, Tuple<bool, object>>();
            var n = 0;

            n = CheckSpellandSkillPredicates(player, result, n);
            n = CHeckAttributePredicates(player, result, n);
            n = CheckItemPredicates(player, result, n);
            n = CheckQuestPredicates(player, result, n);

            return CheckPredicates(callbackMsg, result);
        }

        private int CheckQuestPredicates(Aisling player, Dictionary<int, Tuple<bool, object>> result, int n)
        {
            if (Quests_Completed_Required != null && Quests_Completed_Required.Count > 0)
            {
                foreach (var qr in Quests_Completed_Required)
                {
                    if (player.Quests.Where(i => i.Name.Equals(qr) && i.Completed) != null)
                    {
                        result[n] = new Tuple<bool, object>(true, "Thank you. Please proceed.");

                    }
                    else
                    {
                        result[n] = new Tuple<bool, object>(false, "Come back when you complete the quests required.");
                    }

                    n++;
                }
            }

            return n;
        }

        private int CheckSpellandSkillPredicates(Aisling player, Dictionary<int, Tuple<bool, object>> result, int n)
        {
            if (Skill_Required != null)
            {
                var skill = ServerContext.GlobalSkillTemplateCache[Skill_Required];
                var skill_retainer = player.SkillBook.Get(i => i.Template.Name.Equals(skill.Name)).FirstOrDefault();

                if (skill_retainer == null)
                {
                    result[n++] = new Tuple<bool, object>(false, 
                        string.Format("You don't have the skill required. ({0})", Skill_Required));
                }

                if (skill_retainer != null && skill_retainer.Level >= Skill_Level_Required)
                {
                    result[n++] = new Tuple<bool, object>(true,
                        "Skills Required.");

                }
                else
                {
                    result[n++] = new Tuple<bool, object>(false,
                        string.Format("{0} Must be level {1} - Go get {2} more levels.",
                        skill.Name, Skill_Level_Required, Math.Abs(skill_retainer.Level - Skill_Level_Required)));
                }
            }

            if (Spell_Required != null)
            {
                var spell = ServerContext.GlobalSpellTemplateCache[Spell_Required];
                var spell_retainer = player.SpellBook.Get(i => i.Template.Name.Equals(spell.Name)).FirstOrDefault();

                if (spell_retainer == null)
                {
                    result[n++] = new Tuple<bool, object>(false,
                        string.Format("You don't have the spell required. ({0})", Spell_Required));
                }

                if (spell_retainer != null & spell_retainer.Level >= Spell_Level_Required)
                {
                    result[n++] = new Tuple<bool, object>(true,
                        "Spells Required.");
                }
                else
                {
                    result[n++] = new Tuple<bool, object>(false,
                        string.Format("{0} Must be level {1} - Go get {2} more levels.",
                        spell.Name, Skill_Level_Required, Math.Abs(spell_retainer.Level - Spell_Level_Required)));
                }
            }

            return n;
        }

        private int CHeckAttributePredicates(Aisling player, Dictionary<int, Tuple<bool, object>> result, int n)
        {
            result[n++] = new Tuple<bool, object>(player.ExpLevel >= ExpLevel_Required, string.Format("Go level more. (Level {0} Required.)", Spell_Level_Required));
            result[n++] = new Tuple<bool, object>(player.Str >= Str_Required, string.Format("You are not strong enough. ({0} Str Required.).", Str_Required));
            result[n++] = new Tuple<bool, object>(player.Int >= Int_Required, string.Format("You are not smart enough.  ({0} Int Required.).", Int_Required));
            result[n++] = new Tuple<bool, object>(player.Wis >= Wis_Required, string.Format("You are not wise enough. ({0} Wis Required.).", Wis_Required));
            result[n++] = new Tuple<bool, object>(player.Con >= Con_Required, string.Format("You lack stamina. ({0} Con Required.).", Con_Required));
            result[n++] = new Tuple<bool, object>(player.Dex >= Dex_Required, string.Format("You are not nimble enough. ({0} Dex Required.).", Dex_Required));
            result[n++] = new Tuple<bool, object>(player.GoldPoints >= Gold_Required, string.Format("You best come back when you got the cash. ({0} Gold Required.).", Gold_Required));
            result[n++] = new Tuple<bool, object>(player.Stage == Stage_Required, "You must transcend further first");
            result[n++] = new Tuple<bool, object>(player.Path == Class_Required, "You should not be here, " + player.Path.ToString());

            return n;
        }

        private int CheckItemPredicates(Aisling player, Dictionary<int, Tuple<bool, object>> result, int n)
        {
            if (Items_Required != null && Items_Required.Count > 0)
            {
                foreach (var ir in Items_Required)
                {
                    var item = ServerContext.GlobalItemTemplateCache[ir.Item];

                    if (item == null)
                    {
                        result[n] = new Tuple<bool, object>(false,
                                string.Format("You don't have enough {0}'s. You have {1} of {2} required.",
                                ir.Item, "none of ", ir.AmountRequired));

                        break;
                    }

                    var item_obtained = player.Inventory.Get(i => i.Template.Name.Equals(item.Name));

                    if (item_obtained == null || item_obtained.Length == 0)
                    {
                        result[n] = new Tuple<bool, object>(false,
                            string.Format("You lack the items required. (One {0} Required)", ir.Item));
                        break;
                    }
                    else
                    {
                        if (ir.AmountRequired > 1)
                        {
                            if (item_obtained.Length + 1 >= ir.AmountRequired)
                            {
                                result[n] = new Tuple<bool, object>(true, "The right amount!. Thank you.");
                            }
                            else
                            {
                                result[n] = new Tuple<bool, object>(false,
                                    string.Format("You don't have enough {0}'s. You have {1} of {2} required.",
                                    ir.Item, item_obtained.Length + 1, ir.AmountRequired));
                            }
                        }
                        else
                        {
                            if (item_obtained.Length + 1 > 1 && ir.AmountRequired <= 1)
                            {
                                result[n] = new Tuple<bool, object>(true, "Thank you.");
                            }
                            else
                            {
                                result[n] = new Tuple<bool, object>(false, "You lack the items required.");
                            }
                        }
                    }
                    n++;
                }
            }

            return n;
        }

        private static bool CheckPredicates(Action<string, bool> callbackMsg, Dictionary<int, Tuple<bool, object>> result)
        {
            if (result == null || result.Count == 0)
                return false;

            var predicate_result = result.ToList().TrueForAll(i => i.Value.Item1);

            if (predicate_result)
            {
                callbackMsg?.Invoke("You have met all prerequisites, Do you wish to proceed?.", true);
                return true;
            }

            var sb = "";
            {
                sb += ("{=sYou are not worthy., \n{=u");
                foreach (var predicate in result.Select(i => i.Value))
                {
                    if (predicate != null && !predicate.Item1)
                    {
                        sb += ((string)predicate.Item2) + "\n";
                    }
                }
            }

            callbackMsg?.Invoke(sb.ToString(), false);
            return false;
        }
    }
}
 