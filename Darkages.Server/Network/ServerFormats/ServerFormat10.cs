namespace Darkages.Network.ServerFormats
{
    public class ServerFormat10 : NetworkFormat
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
                return 0x10;
            }
        }
        public byte Slot { get; set; }

        public ServerFormat10(byte slot)
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
