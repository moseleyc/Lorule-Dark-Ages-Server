namespace Darkages.Network.ClientFormats
{
    public class ClientFormat4F : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x4F; }
        }

        public ushort Count { get; set; }
        public byte[] Image { get; set; }
        public string Words { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            this.Count = reader.ReadUInt16();
            this.Image = reader.ReadBytes(reader.ReadUInt16());
            this.Words = reader.ReadStringB();           
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}