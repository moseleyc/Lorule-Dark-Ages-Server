using Darkages.Types;
using System.Collections.Generic;

namespace Darkages.Network.ClientFormats
{
    public class ClientFormat0F : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x0F; }
        }

        public byte Index { get; set; }
        public Position Point { get; set; }
        public uint Serial { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            Index = reader.ReadByte();
            Serial = reader.ReadUInt32();
            Point = reader.ReadPosition();
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}