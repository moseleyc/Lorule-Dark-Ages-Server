namespace Darkages.Network.ServerFormats
{
    public class ServerFormat67 : NetworkFormat
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
                return 0x67;
            }
        }


        public byte Type
        {
            get
            {
                return 0x03;
            }
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(Type);
            writer.Write(uint.MinValue);
        }
    }
}
