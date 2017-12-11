using Darkages.Security;

namespace Darkages.Network.ClientFormats
{
    public class ClientFormat10 : NetworkFormat
    {
        public override byte Command
        {
            get { return 0x10; }
        }
        public override bool Secured
        {
            get { return false; }
        }

        public SecurityParameters Parameters { get; set; }
        public string Name { get; set; }
        public int ID { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            this.Parameters = reader.ReadObject<SecurityParameters>();
            this.Name = reader.ReadStringA();
            this.ID = reader.ReadInt32();
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}