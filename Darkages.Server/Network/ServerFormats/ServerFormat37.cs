using Darkages.Types;

namespace Darkages.Network.ServerFormats
{
    public class ServerFormat37 : NetworkFormat
    {
        public ServerFormat37(Item item, byte slot)
        {
            Item = item;
            EquipmentSlot = slot;
        }

        public override bool Secured => true;

        public override byte Command => 0x37;

        public Item Item { get; set; }
        public byte EquipmentSlot { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(EquipmentSlot);
            writer.Write((ushort) Item.DisplayImage);
            writer.Write(ushort.MinValue);
            writer.WriteStringA(Item.DisplayName);
            writer.WriteStringA(Item.Template.Name);
            writer.Write(Item.Durability);
            writer.Write(Item.Template.MaxDurability);
        }
    }
}