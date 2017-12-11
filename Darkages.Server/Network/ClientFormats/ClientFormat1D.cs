namespace Darkages.Network.ClientFormats
{
    public class ClientFormat1D : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x1D; }
        }

        public byte Number { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            this.Number = reader.ReadByte();
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}