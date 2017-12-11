namespace Darkages.Network.ServerFormats
{
    public class ServerFormat19 : NetworkFormat
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
                return 0x19;
            }
        }

        public short Number { get; set; }


        public ServerFormat19(int number)
        {
            Number = (short)number;
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write((byte)0x00);
            writer.Write((ushort)22);
        }
    }
}
