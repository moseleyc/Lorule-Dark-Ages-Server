using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Darkages.Types
{
    public class Quest<T>
    {
        public string Name { get; set; }
        public bool Started { get; set; }
        public bool Completed { get; set; }

        public DateTime TimeStarted { get; set; }
        public DateTime TimeCompleted { get; set; }

        public uint ExpReward { get; set; }
        public uint GoldReward { get; set; }
        public int StageIndex { get; set; }

        public List<ItemTemplate> ItemRewards = new List<ItemTemplate>();
        public List<SpellTemplate> SpellRewards = new List<SpellTemplate>();
        public List<SkillTemplate> SkillRewards = new List<SkillTemplate>();
        public List<Legend.LegendItem> LegendRewards = new List<Legend.LegendItem>();

        [JsonIgnore]
        public List<QuestStep<T>> QuestStages = new List<QuestStep<T>>();

        public void OnCompleted(Aisling user)
        {
            Completed = true;
            TimeCompleted = DateTime.Now;

            foreach (var legends in LegendRewards)
            {
                user.LegendBook.AddLegend(new Legend.LegendItem()
                {
                    Category = "Quest Reward",
                    Color = (byte)legends.Color,
                    Icon = (byte)legends.Icon,
                    Value = legends.Value
                });
            }
        }

        public void UpdateQuest(Aisling user)
        {
            if (StageIndex + 1 < QuestStages.Count)
            {
                StageIndex++;
            }
            else
            {
                OnCompleted(user);
            }
        }
    }

    public class QuestStep<T>
    {
        public string Name { get; set; }

        [JsonIgnore]
        public List<Predicate<T>> RequirementsToProgress
            = new List<Predicate<T>>();
    }
}
