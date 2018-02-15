﻿using Darkages.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Darkages.Types
{
    public class CursedSachel 
    {
        private ISet<Item> Items { get; set; }
        public Aisling Owner { get; set; }
        public DateTime DateReleased { get; set; }
        public Position Location { get; set; }
        public int MapId { get; set; }
        public Item ReaperBag { get; set; }

        public void GenerateReeper()
        {
            var itemTemplate = new ItemTemplate()
            {
                Name = string.Format("{0}'s Shit.", Owner?.Username),
                ScriptName = "Cursed Sachel",
                Image = 135,
                DisplayImage = 0x8000 + 135,
                Flags = ItemFlags.Tradeable | ItemFlags.Consumable | ItemFlags.Stackable | ItemFlags.Dropable,
                Value = 10000000,
                Class = Class.Peasant,
                LevelRequired = 11,
                MaxStack = 255,
                CanStack = true,
                CarryWeight = 1,
            };


            ReaperBag = Item.Create(Owner, itemTemplate, true);
            ReaperBag?.Release(Owner, Owner.Position);

        }

        public CursedSachel(Aisling parent)
        {
            Owner = parent;
            Items = new HashSet<Item>();
        }

        public void RecoverItems(Aisling Owner)
        {
            foreach (var item in Items)
            {
                var nitem = Item.Clone(item);

                if (nitem.GiveTo(Owner, true))
                {
                    Owner.Client.SendMessage(0x02, string.Format("You have recovered {0}.", item.Template.Name));
                }
            }


            Items = new HashSet<Item>();
            {
                Owner.EquipmentManager.RemoveFromInventory(ReaperBag, true);
                Owner.Client.SendStats(StatusFlags.All);
            }

            ReaperBag?.Remove();
            ReaperBag = null;

        }

        public void ReepItems()
        {
            Items = new HashSet<Item>();
            Location = new Position(Owner.X, Owner.Y);
            MapId = Owner.CurrentMapId;

            ReepInventory();
            ReepEquipment();
            ReepGold();
            GenerateReeper();

            Owner.Client.SendMessage(0x02, "Everyone is a bad ass, til they meet one.");
            Owner.Client.SendStats(StatusFlags.All);
        }

        private void ReepGold()
        {
            var gold = Owner.GoldPoints;
            {
                Money.Create(Owner, gold, Owner.Position);
                Owner.GoldPoints = 0;
            }
        }

        private void ReepEquipment()
        {
            List<EquipmentSlot> inv;

            lock (Owner.EquipmentManager.Equipment)
            {
                var batch = Owner.EquipmentManager.Equipment.Where(i => i.Value != null)
                    .Select(i => i.Value);

                inv = new List<EquipmentSlot>(batch);
            }

            foreach (EquipmentSlot es in inv)
            {
                var obj = es.Item;

                if (obj == null)
                {
                    continue;
                }

                if (obj.Template == null)
                    continue;

                if (Owner.EquipmentManager.RemoveFromExisting(es.Slot, false))
                {
                    //were not processing weight changes in the above function. so we must do it here.
                    obj.Durability -= (obj.Durability * 10 / 100);
                    Owner.Inventory.UpdateWeight(Owner, obj);

                    if (obj.Durability > 0)
                    {
                        var copy = Item.Clone(obj);
                        Add(copy);
                    }
                }
            }
        }
    

        private void ReepInventory()
        {
            List<Item> inv;

            lock (Owner.Inventory.Items)
            {
                var batch = Owner.Inventory.Items.Select(i => i.Value);
                inv = new List<Item>(batch);
            }

            foreach (Item item in inv)
            {
                var obj = item;

                if (obj == null)
                {
                    continue;
                }

                if (obj.Template == null)
                    continue;

                obj.Durability -= (obj.Durability * 10 / 100);

                //delete the item from inventory.
                Owner.Inventory.Remove(Owner.Client, item);
                Owner.Inventory.UpdateWeight(Owner, item);

                if (obj.Durability > 0)
                {
                    var copy = Item.Clone(obj);
                    Add(copy);
                }
            }
        }

        private void Add(Item obj)
        {
            lock (Items)
            {
                Items.Add(obj);
            }
        }
    }
}
