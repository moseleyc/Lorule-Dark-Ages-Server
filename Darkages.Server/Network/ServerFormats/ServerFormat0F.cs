using Darkages.Types;

namespace Darkages.Network.ServerFormats
{
    public class ServerFormat0F : NetworkFormat
    {
        public override bool Secured => true;

        public override byte Command => 0x0F;

        public Item Item
        {
            get; set;
        }

        public ServerFormat0F(Item item)
        {
            this.Item = item;
        }


        public override void Serialize(NetworkPacketReader reader)
        {

        }

        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write((byte)Item.Slot);
            writer.Write((ushort)Item.DisplayImage);
            writer.Write(byte.MinValue);
            writer.WriteStringA(Item.Template.Name);
            writer.Write((uint)Item.Stacks);
            writer.Write((byte)Item.Stacks > 1);
            writer.Write((uint)Item.Template.MaxDurability);
            writer.Write((uint)Item.Durability);
            writer.Write((uint)0x00);
        }
    }
}
