namespace Darkages.Types
{
    public class EquipmentSlot
    {
        public int Slot { get; set; }
        public Item Item { get; set; }

        public EquipmentSlot(int _slot, Item _item)
        {
            Slot = _slot;
            Item = _item;
        }
    }
}