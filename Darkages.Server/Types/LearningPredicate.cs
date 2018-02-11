using System;
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
                result[n++] = new Tuple<bool, object>(skill == null ? false 
                    : (player.SkillBook.Get(i => i.Template.Name.Equals(skill.Name)
                    && i.Level >= Skill_Level_Required
                    && (int)i.Template.TierLevel >= Skill_Tier_Required) != null),
                    "You don't have the skills required.");
            }

            if (Spell_Required != null)
            {
                var spell = ServerContext.GlobalSkillTemplateCache[Spell_Required];
                result[n++] = new Tuple<bool, object>(spell == null ? false 
                    : (player.SpellBook.Get(i => i.Template.Name.Equals(spell.Name)
                    && i.Level >= Spell_Level_Required
                    && (int)i.Template.TierLevel >= Spell_Tier_Required) != null),
                    "You lack the spells required to proceed.");
            }

            return n;
        }

        private int CHeckAttributePredicates(Aisling player, Dictionary<int, Tuple<bool, object>> result, int n)
        {
            result[n++] = new Tuple<bool, object>(player.ExpLevel >= ExpLevel_Required, "You can't learn this yet. Go level more.");
            result[n++] = new Tuple<bool, object>(player.Str >= Str_Required, "You are not strong enough.");
            result[n++] = new Tuple<bool, object>(player.Int >= Int_Required, "You are not smart enough.");
            result[n++] = new Tuple<bool, object>(player.Wis >= Wis_Required, "You are not wise enough.");
            result[n++] = new Tuple<bool, object>(player.Con >= Con_Required, "You lack stamina.");
            result[n++] = new Tuple<bool, object>(player.Dex >= Dex_Required, "You are not nimble enough.");
            result[n++] = new Tuple<bool, object>(player.GoldPoints >= Gold_Required, "You best come back when you got the cash.");
            result[n++] = new Tuple<bool, object>(player.Stage == Stage_Required, "You must transcend further first.");
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
                        result[n] = new Tuple<bool, object>(false, "You lack the items required.");
                        break;
                    }

                    var item_obtained = player.Inventory.Get(i => i.Template.Name.Equals(item.Name));

                    if (item_obtained == null || item_obtained.Length == 0)
                    {
                        result[n] = new Tuple<bool, object>(false, "You lack the items required.");
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
                                result[n] = new Tuple<bool, object>(false, "You don't have the correct amount i need.");
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
 