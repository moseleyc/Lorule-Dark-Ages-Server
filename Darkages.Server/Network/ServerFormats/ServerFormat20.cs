namespace Darkages.Network.ServerFormats
{
    public class ServerFormat20 : NetworkFormat
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
                return 0x20;
            }
        }

        public byte Shade { get; set; }
        public byte Unknown
        {
            get
            {
                return 0x01;
            }
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(Shade);
            writer.Write(Unknown);
        }
    }
}
