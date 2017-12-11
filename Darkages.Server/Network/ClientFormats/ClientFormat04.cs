namespace Darkages.Network.ClientFormats
{
    public class ClientFormat04 : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x04; }
        }

        public byte Gender { get; set; }
        public short HairStyle { get; set; }
        public short HairColor { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            this.HairStyle = reader.ReadByte();
            this.Gender = reader.ReadByte();
            this.HairColor = reader.ReadInt16();
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}