namespace Darkages.Network.ClientFormats
{
    public class ClientFormat57 : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x57; }
        }

        public byte Type { get; set; }
        public byte Slot { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            this.Type = reader.ReadByte();

            if (this.Type == 0x00)
            {
                this.Slot = reader.ReadByte();
            }
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}