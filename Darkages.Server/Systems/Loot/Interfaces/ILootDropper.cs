using System.Collections.Generic;

namespace Darkages.Systems.Loot.Interfaces
{
    /// <summary>
    /// Interface for implementing the class that will pick and drop loot.
    /// </summary>
    public interface ILootDropper
    {
        /// <summary>
        /// Drops a single item from the loot table.
        /// </summary>
        ILootDefinition Drop(ILootTable lootTable);

        /// <summary>
        /// Drops a specific <see cref="ILootDefinition"/> from the <param name="lootTable"></param>, or selects a drop from the specified child table.
        /// </summary>
        /// <param name="lootTable">The loot table to select the drop from.</param>
        /// <param name="name">The name of the item or loot table</param>
        ILootDefinition Drop(ILootTable lootTable, string name);

        /// <summary>
        /// Drop one or more items
        /// </summary>
        /// <param name="lootTable">The table to pick from.</param>
        /// <param name="amount">The amount of drops to get.</param>
        IEnumerable<ILootDefinition> Drop(ILootTable lootTable, int amount);

        /// <summary>
        /// Drops items from a specific child table, or x <param name="amount"></param> of a specific item.
        /// </summary>
        /// <param name="lootTable">The loot table to select drops from.</param>
        /// <param name="amount">The amount of drops to get.</param>
        /// <param name="name">The name of the item or loot table</param>
        IEnumerable<ILootDefinition> Drop(ILootTable lootTable, int amount, string name);
    }
}
