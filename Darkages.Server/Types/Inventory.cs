using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Network.Game;
using Darkages.Network.Object;
using Darkages.Network.ServerFormats;

namespace Darkages.Types
{
    public class Inventory : ObjectManager
    {
        public static readonly int LENGTH = 59;

        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        public Inventory()
        {
            for (var i = 0; i < LENGTH; i++) Items[i + 1] = null;
        }


        public int Length => Items.Count;

        public Item FindInSlot(int Slot)
        {
            return Items[Slot];
        }

        public void Assign(Item Item)
        {
            Set(Item);
        }

        public new Item[] Get(Predicate<Item> prediate)
        {
            return Items.Values.Where(i => i != null && prediate(i)).ToArray();
        }

        public void Set(Item s)
        {
            Items[s.Slot] = Clone(s);
        }

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

        public void Set(Item s, bool clone = false)
        {
            Items[s.Slot] = clone ? Clone(s) : s;
        }

        public Item Remove(byte movingFrom)
        {
            var copy = Items[movingFrom];
            Items[movingFrom] = null;
            return copy;
        }

        public int Has(Template templateContext)
        {
            var items = Items.Where(i => i.Value != null && i.Value.Template.Name == templateContext.Name)
                .Select(i => i.Value).ToList();

            return items.Sum(i => i.Stacks);
        }

        public void RemoveRange(GameClient client, Item item, int range)
        {
            var remaining = item.Stacks - range;

            if (remaining <= 0)
            {
                Remove(item.Slot);
                client.Send(new ServerFormat10(item.Slot));

                client.Aisling.CurrentWeight -= item.Template.Weight;

                if (client.Aisling.CurrentWeight < 0)
                    client.Aisling.CurrentWeight = 0;

                client.SendStats(StatusFlags.StructA);
            }
            else
            {
                item.Stacks = (byte) remaining;
                client.Aisling.Inventory.Set(item, false);

                client.Send(new ServerFormat0F(item));
            }
        }
    }
}