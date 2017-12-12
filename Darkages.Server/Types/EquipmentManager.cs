using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Darkages.Types
{
    public class EquipmentManager
    {
        [JsonIgnore]
        public GameClient Client { get; set; }

        public Dictionary<int, EquipmentSlot> Equipment { get; set; }

        public EquipmentManager(GameClient _client)
        {
            Client = _client;
            Equipment = new Dictionary<int, EquipmentSlot>();

            for (byte i = 1; i < 18; i++)
                Equipment[i] = null;
        }

        public EquipmentSlot this[byte idx]
        {
            get
            {
                return Equipment.ContainsKey(idx) ? Equipment[idx] : null;
            }
        }

        public EquipmentSlot Weapon
        {
            get { return this[ItemSlots.Weapon]; }
        }

        public EquipmentSlot Armor
        {
            get { return this[ItemSlots.Armor]; }
        }

        public EquipmentSlot Shield
        {
            get { return this[ItemSlots.Shield]; }
        }

        public EquipmentSlot Helmet
        {
            get { return this[ItemSlots.Helmet]; }
        }

        public EquipmentSlot Earring
        {
            get { return this[ItemSlots.Earring]; }
        }

        public EquipmentSlot Necklace
        {
            get { return this[ItemSlots.Necklace]; }
        }

        public EquipmentSlot LRing
        {
            get { return this[ItemSlots.LHand]; }
        }

        public EquipmentSlot RRing
        {
            get { return this[ItemSlots.RHand]; }
        }

        public EquipmentSlot LGauntlet
        {
            get { return this[ItemSlots.LArm]; }
        }

        public EquipmentSlot RGauntlet
        {
            get { return this[ItemSlots.RArm]; }
        }

        public EquipmentSlot Belt
        {
            get { return this[ItemSlots.Waist]; }
        }

        public EquipmentSlot Greaves
        {
            get { return this[ItemSlots.Leg]; }
        }

        public EquipmentSlot Boots
        {
            get { return this[ItemSlots.Foot]; }
        }

        public EquipmentSlot FirstAcc
        {
            get { return this[ItemSlots.FirstAcc]; }
        }

        public EquipmentSlot Overcoat
        {
            get { return this[ItemSlots.Trousers]; }
        }

        public EquipmentSlot DisplayHelm
        {
            get { return this[ItemSlots.Coat]; }
        }

        public EquipmentSlot SecondAcc
        {
            get { return this[ItemSlots.SecondAcc]; }
        }

        public EquipmentSlot ThirdAcc
        {
            get { return this[ItemSlots.ThirdAcc]; }
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

            //give this item back to the inventory.
            if (displayslot == ItemSlots.Weapon)
            {
                success = itemObj.GiveTo(Client.Aisling, false, 1);
            }
            else
            {
                success = itemObj.GiveTo(Client.Aisling, false);
            }

            if (success)
            {
                //send remove equipment packet.
                Client.Aisling.Show(Scope.Self, new ServerFormat38((byte)displayslot));

                OnEquipmentRemoved((byte)displayslot);

                //make sure we remove it!
                Equipment[displayslot] = null;
            }

            return success;
        }


        public void AddEquipment(int displayslot, Item item)
        {
            Equipment[displayslot] = new EquipmentSlot(displayslot, item);

            //Remove it from inventory.
            RemoveFromInventory(item);

            DisplayToEquipment((byte)displayslot, item);

            OnEquipmentAdded((byte)displayslot, item);
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
                Client.Aisling.CurrentWeight -= item.Template.Weight;
                if (Client.Aisling.CurrentWeight < 0)
                    Client.Aisling.CurrentWeight = 0;
            }
        }
        #endregion

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
    }
}
