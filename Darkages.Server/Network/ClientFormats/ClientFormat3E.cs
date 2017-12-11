namespace Darkages.Network.ClientFormats
{
    public class ClientFormat3E : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x3E; }
        }

        public byte Index { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            this.Index = reader.ReadByte();
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}