namespace Darkages.Network.ClientFormats
{
    public class ClientFormat75 : NetworkFormat
    {
        public override bool Secured
        {
            get { return false; }
        }

        public override byte Command
        {
            get { return 0x75; }
        }

        public long Tick { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            this.Tick = (long)(reader.ReadByte() >> 4) - 0x15;
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}