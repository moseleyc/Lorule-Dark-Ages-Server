using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Newtonsoft.Json;

namespace Darkages.Types
{
    public class EquipmentManager
    {
        public EquipmentManager(GameClient _client)
        {
            Client = _client;
            Equipment = new Dictionary<int, EquipmentSlot>();

            for (byte i = 1; i < 18; i++)
                Equipment[i] = null;
        }

        [JsonIgnore] public GameClient Client { get; set; }

        public Dictionary<int, EquipmentSlot> Equipment { get; set; }

        public EquipmentSlot this[byte idx] => Equipment.ContainsKey(idx) ? Equipment[idx] : null;

        public EquipmentSlot Weapon => this[ItemSlots.Weapon];

        public EquipmentSlot Armor => this[ItemSlots.Armor];

        public EquipmentSlot Shield => this[ItemSlots.Shield];

        public EquipmentSlot Helmet => this[ItemSlots.Helmet];

        public EquipmentSlot Earring => this[ItemSlots.Earring];

        public EquipmentSlot Necklace => this[ItemSlots.Necklace];

        public EquipmentSlot LRing => this[ItemSlots.LHand];

        public EquipmentSlot RRing => this[ItemSlots.RHand];

        public EquipmentSlot LGauntlet => this[ItemSlots.LArm];

        public EquipmentSlot RGauntlet => this[ItemSlots.RArm];

        public EquipmentSlot Belt => this[ItemSlots.Waist];

        public EquipmentSlot Greaves => this[ItemSlots.Leg];

        public EquipmentSlot Boots => this[ItemSlots.Foot];

        public EquipmentSlot FirstAcc => this[ItemSlots.FirstAcc];

        public EquipmentSlot Overcoat => this[ItemSlots.Trousers];

        public EquipmentSlot DisplayHelm => this[ItemSlots.Coat];

        public EquipmentSlot SecondAcc => this[ItemSlots.SecondAcc];

        private void OnEquipmentRemoved(byte displayslot)
        {
            if (Equipment[displayslot] == null)
                return;

            Equipment[displayslot].Item?.Script?.UnEquipped(Client.Aisling, displayslot);
            Client.SendStats(StatusFlags.All);
            Client.UpdateDisplay();
        }

        private void OnEquipmentAdded(byte displayslot, Item item)
        {
            Equipment[displayslot].Item?.Script?.Equipped(Client.Aisling, displayslot);
            Client.SendStats(StatusFlags.All);
            Client.UpdateDisplay();
        }

        public void DecreaseDurability()
        {
            var broken = new List<Item>();
            foreach (var equipment in Equipment)
            {
                var item = equipment.Value?.Item;

                if (item?.Template == null)
                    continue;

                item.Durability--;

                if (item.Durability <= 0 || item.Durability > item.Template.MaxDurability)
                    broken.Add(item);
            }


            foreach (var item in broken)
            {
                if (item?.Template == null)
                    continue;

                RemoveFromExisting(item.Template.EquipmentSlot);
            }
        }

        #region Core Methods

        public void Add(int displayslot, Item item)
        {
            if (Client == null)
                return;

            if (displayslot <= 0 || displayslot > 17)
                return;

            if (item == null)
                return;

            if (item.Template == null)
                return;

            if (!item.Template.Flags.HasFlag(ItemFlags.Equipable))
                return;

            if (Equipment == null)
                Equipment = new Dictionary<int, EquipmentSlot>();


            if (RemoveFromExisting(displayslot))
                AddEquipment(displayslot, item);
        }

        public bool RemoveFromExisting(int displayslot)
        {
            if (Equipment[displayslot] == null)
                return true;

            //get current equipped item occupying the requested slot.
            var itemObj = Equipment[displayslot].Item;

            //sanity check
            if (itemObj == null)
                return false;

            var success = false;
            var returntouser = true;
            if (itemObj.Template.Flags.HasFlag(ItemFlags.Repairable))
                if (itemObj.Durability <= 0 || itemObj.Durability > itemObj.Template.MaxDurability)
                    returntouser = false;

            RemoveFromSlot(displayslot);

            //give this item back to the inventory.
            if (displayslot == ItemSlots.Weapon)
                success = itemObj.GiveTo(Client.Aisling, false, 1);
            else
                success = itemObj.GiveTo(Client.Aisling, false);

            if (!returntouser)
            {
                RemoveFromInventory(itemObj, true);
                success = true;
            }

            return success;
        }

        private void RemoveFromSlot(int displayslot)
        {
            //send remove equipment packet.
            Client.Aisling.Show(Scope.Self, new ServerFormat38((byte) displayslot));

            OnEquipmentRemoved((byte) displayslot);

            //make sure we remove it!
            Equipment[displayslot] = null;
        }

        public void AddEquipment(int displayslot, Item item)
        {
            Equipment[displayslot] = new EquipmentSlot(displayslot, item);

            //Remove it from inventory.
            RemoveFromInventory(item);

            DisplayToEquipment((byte) displayslot, item);

            OnEquipmentAdded((byte) displayslot, item);
        }

        public void DisplayToEquipment(byte displayslot, Item item)
        {
            //Send Equipment packet.
            Client.Send(new ServerFormat37(item, displayslot));
        }

        public void RemoveFromInventory(Item item, bool handleWeight = false)
        {
            Client.Aisling.Inventory.Remove(item.Slot);
            Client.Send(new ServerFormat10(item.Slot));

            if (handleWeight)
            {
                Client.Aisling.CurrentWeight -= item.Template.CarryWeight;
                if (Client.Aisling.CurrentWeight < 0)
                    Client.Aisling.CurrentWeight = 0;
            }
        }

        #endregion
    }
}