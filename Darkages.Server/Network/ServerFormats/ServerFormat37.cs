using Darkages.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkages.Network.ServerFormats
{
    public class ServerFormat37 : NetworkFormat
    {
        public override bool Secured => true;

        public override byte Command => 0x37;

        public Item Item { get; set; }
        public byte EquipmentSlot { get; set; }

        public ServerFormat37(Item item, byte slot)
        {
            this.Item = item;
            this.EquipmentSlot = slot;
        }

        public override void Serialize(NetworkPacketReader reader)
        {

        }

        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write((byte)EquipmentSlot);
            writer.Write((ushort)Item.DisplayImage);
            writer.Write((ushort)ushort.MinValue);
            writer.WriteStringA(Item.DisplayName);
            writer.WriteStringA(Item.Template.Name);
            writer.Write((uint)Item.Durability);
            writer.Write((uint)Item.Template.MaxDurability);
        }
    }
}
