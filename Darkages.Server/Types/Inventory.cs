using Darkages.Network.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Darkages.Types
{
    public class Inventory : ObjectManager
    {
        public static readonly int LENGTH = 59;

        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        public Inventory()
        {
            for (int i = 0; i < LENGTH; i++)
            {
                Items[i + 1] = null;
            }
        }

        public Item FindInSlot(int Slot)
        {
            return Items[Slot];
        }


        public int Length => Items.Count;
        public void Assign(Item Item) => Set(Item);
        public new Item[] Get(Predicate<Item> prediate) => this.Items.Values.Where(i => i != null && prediate(i)).ToArray();
        public void Swap(Item A, Item B) => A = Interlocked.Exchange<Item>(ref B, A);
        public void Set(Item s) => Items[s.Slot] = Clone<Item>(s);

        public byte FindEmpty()
        {
            byte idx = 1;

            foreach (var slot in Items)
            {
                if (slot.Value == null)
                    return idx;

                idx++;
            }

            return 0;
        }

        public void Set(Item s, bool clone = false) => Items[s.Slot] = (clone == true) ? Clone<Item>(s) : s;
        public void Clear(Item s) => Items[s.Slot] = null;

        public Item Remove(byte movingFrom)
        {
            var copy = Items[movingFrom];
            Items[movingFrom] = null;
            return copy;
        }
    }
}
