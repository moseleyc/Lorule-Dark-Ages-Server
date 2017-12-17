using Darkages.Types;

namespace Darkages.Network.ClientFormats
{
    public class ClientFormat0F : NetworkFormat
    {
        public override bool Secured => true;

        public override byte Command => 0x0F;

        public byte Index { get; set; }
        public Position Point { get; set; }
        public uint Serial { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            Index = reader.ReadByte();

            if (reader.CanRead)
                Serial = reader.ReadUInt32();

            if (reader.CanRead)
                Point = reader.ReadPosition();
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}