namespace Darkages.Network.ServerFormats
{
    public class ServerFormat2D : NetworkFormat
    {
        public override bool Secured
        {
            get
            {
                return true;
            }
        }
        public override byte Command
        {
            get
            {
                return 0x2D;
            }
        }
        public byte Slot { get; set; }

        public ServerFormat2D(byte slot)
        {
            Slot = slot;
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(Slot);
        }
    }
}
