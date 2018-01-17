using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Systems.Loot.Extensions;
using Darkages.Systems.Loot.Interfaces;

namespace Darkages.Systems.Loot
{
    public class LootDropper : ILootDropper
    {
        /// <summary>
        /// Event that fires before an item has been dropped
        /// </summary>
        public event EventHandler<EventArgs> OnDropStarted;
        /// <summary>
        /// Event that fires after an item has been dropped
        /// </summary>
        public event EventHandler<EventArgs> OnDropCompleted;
        
        public ILootDefinition Drop(ILootTable lootTable, string name)
        {
            var item = lootTable.Get(name);

            if (item is ILootTable childTable)
                return Drop(childTable);

            return item;
        }
        
        public IEnumerable<ILootDefinition> Drop(ILootTable lootTable, int amount)
        {
            if (amount <= 0)
                return new List<ILootDefinition>();

            OnDropStarted?.Invoke(this, EventArgs.Empty);

            var drops = new List<ILootDefinition>();

            for (var i = 0; i < amount; i++)
            {
                drops.Add(Drop(lootTable));
            }

            OnDropCompleted?.Invoke(this, EventArgs.Empty);

            return drops;
        }
        
        public IEnumerable<ILootDefinition> Drop(ILootTable lootTable, int amount, string name)
        {
            if (amount <= 0)
                return new List<ILootDefinition>();

            var drops = new List<ILootDefinition>();

            for (var i = 0; i < amount; i++)
            {
                drops.Add(Drop(lootTable, name));
            }

            return drops;
        }

        public ILootDefinition Drop(ILootTable lootTable)
        {
            var item = Pick(lootTable.Children);

            if (item is ILootTable childTable)
                return Drop(childTable);

            return item;
        }

        /// <summary>
        /// Pick an item based on weight
        /// </summary>
        public static T Pick<T>(IEnumerable<T> items) where T : class, IWeighable
        {
            var itemList = items as IList<T> ?? items.ToList();
            if (itemList == null || !itemList.Any())
                throw new ArgumentException("Items cannot be null or empty", nameof(items));

            var selectedItem = itemList.WeightedChoice(
                itemList.Sum(item => item.Weight));

            return selectedItem;
        }
    }
}
