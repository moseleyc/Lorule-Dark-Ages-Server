namespace Darkages.Network.ServerFormats
{
    public class ServerFormat32 : NetworkFormat
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
                return 0x32;
            }
        }

        public ServerFormat32()
        {
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write((byte)0x00);
        }
    }
}
