namespace Darkages.Network.ServerFormats
{
    public class ServerFormat18 : NetworkFormat
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
                return 0x18;
            }
        }
        public byte Slot { get; set; }

        public ServerFormat18(byte slot)
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
