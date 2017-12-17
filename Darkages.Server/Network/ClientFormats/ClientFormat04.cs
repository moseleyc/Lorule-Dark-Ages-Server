namespace Darkages.Network.ClientFormats
{
    public class ClientFormat04 : NetworkFormat
    {
        public override bool Secured => true;

        public override byte Command => 0x04;

        public byte Gender { get; set; }
        public short HairStyle { get; set; }
        public short HairColor { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            HairStyle = reader.ReadByte();
            Gender = reader.ReadByte();
            HairColor = reader.ReadInt16();
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}