﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Darkages.Types
{
    public class CursedSachel 
    {
        private ISet<Item> Items;

        public Aisling Owner { get; set; }
        public DateTime DateReleased { get; set; }
        public Position Location { get; set; }
        public int MapId { get; set; }

        public CursedSachel(Aisling parent)
        {
            Owner = parent;
            Items = new HashSet<Item>();
        }

        public void RecoverItems(Aisling Owner)
        {
            if (Owner.Position.IsNearby(Location) && Owner.LoggedIn && Owner.CurrentHp > 0)
            {
                foreach (var item in Items)
                {
                    var nitem = Item.Clone(item);
                    if (nitem.GiveTo(Owner, true))
                    {
                        item.Remove();
                    }
                }
            }
        }

        public void ReepItems()
        {
            Items    = new HashSet<Item>();
            Location = new Position(Owner.X, Owner.Y);
            MapId    = Owner.CurrentMapId;

            Owner.Client.SendMessage(0x02, "Everyone is a bad ass, til they meet one.");

            foreach (Item item in Owner.Inventory.Items.Select(i => i.Value))
            {
                var obj = item;

                if (obj == null) {
                    continue;
                }

                if (obj.Template == null)
                    continue;

                if (obj.Template.Flags.HasFlag(ItemFlags.Perishable))
                {
                    var copy = Item.Clone(obj);
                    Add(copy);

                    obj.Durability -= (obj.Durability * 100 / 10);

                    //delete the item from inventory.
                    Owner.Inventory.Remove(Owner.Client, item);
                    Owner.Inventory.UpdateWeight(Owner, item);

                    if (obj.Durability > 0)
                    {                     
                        //drop it back to the world at current position.
                        var nitem = Item.Clone<Item>(obj);
                        {
                            nitem.Cursed = true;
                            nitem.Type = typeof(CursedSachel);
                            nitem.Release(Owner, Owner.Position);
                        }
                    }
                }
            }

            Owner.Client.SendMessage(0x02, "Everyone dances with the grim reaper.");
        }

        private void Add(Item obj)
        {
            if (obj.Template.Flags.HasFlag(ItemFlags.Perishable))
            {
                lock (Items)
                {
                    Items.Add(obj);
                }
            }
        }
    }
}
