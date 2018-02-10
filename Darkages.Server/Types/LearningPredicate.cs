using System.Collections.Generic;
using System.Linq;

namespace Darkages.Types
{
    public class ItemPredicate
    {
        public ItemTemplate Item  { get; set; }
        public int AmountRequired { get; set; }
    }

    public class LearningPredicate
    {
        
        public int ExpLevel_Required      { get; set; }
        public int Str_Required           { get; set; }
        public int Int_Required           { get; set; }
        public int Wis_Required           { get; set; }
        public int Dex_Required           { get; set; }
        public int Con_Required           { get; set; }
        public int Gold_Required          { get; set; }

        public List<ItemPredicate> Item_Requirements = new List<ItemPredicate>();
        public SkillTemplate Skill_Required { get; set; }
        public int Skill_Level_Required { get; set; }
        public List<string> Quests_Completed_Required = new List<string>();


        public bool IsMet(Aisling player)
        {
            Dictionary<int, bool> result = new Dictionary<int, bool>();
            int n = 0;

            result[n++] = (player.SkillBook.Get(i => i.Template.Name.Equals(Skill_Required.Name)
                && i.Level >= Skill_Level_Required) != null);
            result[n++] = player.ExpLevel >= ExpLevel_Required;
            result[n++] = player.Str >= Str_Required;
            result[n++] = player.Int >= Int_Required;
            result[n++] = player.Wis >= Wis_Required;
            result[n++] = player.Con >= Con_Required;
            result[n++] = player.Dex >= Dex_Required;
            result[n++] = player.GoldPoints >= Gold_Required;

            foreach (var ir in Item_Requirements)
            {
                var item_obtained = player.Inventory.Get(i => i.Template.Name.Equals(ir.Item.Name));

                if (item_obtained == null || item_obtained.Length == 0)
                {
                    result[n] = false;
                    break;
                }
                else
                {
                    if (ir.AmountRequired > 0)
                    {
                        if (item_obtained.Length >= ir.AmountRequired)
                        {
                            result[n] = true;
                        }
                        else
                        {
                            result[n] = false;
                        }
                    } 
                    else
                    {
                        if (item_obtained.Length > 0 && ir.AmountRequired == 0)
                        {
                            result[n] = true;
                        }
                        else
                        {
                            result[n] = false;
                        }
                    }
                }
                n++;
            }

            foreach (var qr in Quests_Completed_Required)
            {
                if (player.Quests.Where(i => i.Name.Equals(qr)
                    && i.Completed) != null)
                {
                    result[n] = true;
                }
                else
                {
                    result[n] = false;
                }

                n++;
            }

            return result.ToList()
                .TrueForAll(i => i.Value);
        }
    }
}
 