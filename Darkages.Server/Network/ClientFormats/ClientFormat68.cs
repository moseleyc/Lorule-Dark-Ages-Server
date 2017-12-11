namespace Darkages.Network.ClientFormats
{
    public class ClientFormat68 : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x68; }
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