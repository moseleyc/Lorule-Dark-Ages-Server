namespace Darkages.Network.ClientFormats
{
    public class ClientFormat47 : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x47; }
        }

        public byte Stat { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            this.Stat = reader.ReadByte();
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}