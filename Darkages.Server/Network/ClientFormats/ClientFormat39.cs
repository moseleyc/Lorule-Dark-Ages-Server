namespace Darkages.Network.ClientFormats
{
    public class ClientFormat39 : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x39; }
        }

        public byte Type { get; set; }
        public int Serial { get; set; }
        public ushort Step { get; set; }
        public string Args { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            this.Type = reader.ReadByte();
            this.Serial = reader.ReadInt32();
            reader.ReadByte();
            this.Step = reader.ReadByte();

            try
            {
                this.Args = reader.ReadStringA();
            }
            catch
            {

            }
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}