namespace Darkages.Network.ClientFormats
{
    public class ClientFormat89 : NetworkFormat
    {
        public override bool Secured
        {
            get { return false; }
        }
        public override byte Command
        {
            get { return 0x00; }
        }

        public ushort DisplayMask { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            DisplayMask = reader.ReadUInt16();
        }

        public override void Serialize(NetworkPacketWriter writer)
        {

        }
    }
}
