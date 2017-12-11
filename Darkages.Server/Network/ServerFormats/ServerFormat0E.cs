namespace Darkages.Network.ServerFormats
{
    public class ServerFormat0E : NetworkFormat
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
                return 0x0E;
            }
        }

        public int Serial { get; set; }

        public ServerFormat0E(int serial)
        {
            Serial = serial;
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(Serial);
        }
    }
}
