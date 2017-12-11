namespace Darkages.Network.ServerFormats
{
    public class ServerFormat04 : NetworkFormat
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
                return 0x04;
            }
        }

        public short X { get; set; }
        public short Y { get; set; }

        public ServerFormat04(Aisling aisling)
        {
            X = (short)aisling.X;
            Y = (short)aisling.Y;
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(
                (short)0x000B);
            writer.Write(
                (short)0x000B);
        }
    }
}
