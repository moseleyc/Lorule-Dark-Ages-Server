using Darkages.Network.Object;
using Darkages.Types;

namespace Darkages.Scripting
{
    public abstract class ItemScript : ObjectManager
    {
        public Item Item { get; set; }

        public ItemScript(Item item)
        {
            this.Item = item;
        }

        public abstract void OnUse(Sprite sprite, byte slot);
        public abstract void Equipped(Sprite sprite, byte displayslot);
        public abstract void UnEquipped(Sprite sprite, byte displayslot);

    }
}
