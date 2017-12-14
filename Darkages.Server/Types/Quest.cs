using Darkages.Network.Game;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Darkages.Types
{
    public enum QuestType
    {
        ItemHandIn = 0,
        KillCount  = 1,
        Gossip     = 2,
        Boss       = 3,
        Legend     = 4,
        Accept     = 255
    }

    public class Quest
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
        public List<QuestStep<Template>> QuestStages = new List<QuestStep<Template>>();

        public bool Rewarded { get; set; }

        public List<Item> ItemsToHandIn { get; set; }

        public Quest()
        {
            ItemsToHandIn = new List<Item>();
        }

        public void OnCompleted(Aisling user)
        {
            foreach (var item in ItemsToHandIn)
            {
                if (item == null)
                    continue;
                user.EquipmentManager.RemoveFromInventory(item, true);
            }

            Completed = true;
            TimeCompleted = DateTime.Now;

            foreach (var items in SkillRewards)
            {
                if (!Skill.GiveTo(user.Client, items.Name))
                {
                    Completed = false;
                    return;
                }
            }

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

            Rewarded = true;
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

        public bool MeetsCritera(Aisling user, List<Item> items, Quest quest)
        {
            var complete = 0;
            if (quest != null)
                foreach (var reqs in quest.QuestStages)
                {
                    var req = reqs.RequirementsToProgress;

                    foreach (var r in req)
                    {
                        var obj = items.Find(i => r(i.Template));

                        if (obj != null)
                        {
                            if (quest.ItemsToHandIn.Find(i => i != null && i.Template.Name == obj.Template.Name) ==
                                null)
                                quest.ItemsToHandIn.Add(obj);

                            complete++;
                        }
                    }
                }

            return complete > 0;
        }

        public void HandleQuest(Dialog menu, Mundane mundane, GameClient client)
        {
            var items = client.Aisling.Inventory.Items.Where(i => i.Value != null).Select(i => i.Value).ToList();
            var quest = client.Aisling.Quests.FirstOrDefault(i => i.Name == mundane.Template.QuestKey);
            var valid = false;

            if (quest != null)
                valid = quest.MeetsCritera(client.Aisling, items, quest);

            if (valid)
            {
                if (menu.CanMoveNext)
                {
                    menu.MoveNext(client);
                    menu.Invoke(client);
                }
            }
        }
    }

    public class QuestStep<T>
    {
        public QuestType Type { get; set; }

        [JsonIgnore]
        public List<Predicate<T>> RequirementsToProgress
            = new List<Predicate<T>>();
    }
}
