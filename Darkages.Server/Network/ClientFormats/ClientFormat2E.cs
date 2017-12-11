namespace Darkages.Network.ClientFormats
{
    public class ClientFormat2E : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x2E; }
        }

        public byte Type { get; set; }
        public string Name { get; set; }
        public bool ShowOnMap { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            this.Type = reader.ReadByte();

            if (Type == 0x02)
            {
                this.Name = reader.ReadStringA();
            }

            if (Type == 0x08)
            {
                this.Name = reader.ReadStringA();
                this.ShowOnMap = reader.ReadBool();
            }
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}