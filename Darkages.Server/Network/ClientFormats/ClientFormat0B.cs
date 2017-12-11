namespace Darkages.Network.ClientFormats
{
    public class ClientFormat0B : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x0B; }
        }

        public byte Type { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            this.Type = reader.ReadByte();
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}