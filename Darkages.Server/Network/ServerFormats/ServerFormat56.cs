namespace Darkages.Network.ServerFormats
{
    public class ServerFormat56 : NetworkFormat
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
                return 0x56;
            }
        }

        public ushort Size { get; set; }
        public byte[] Data { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(Size);
            writer.Write(Data);
        }
    }
}
