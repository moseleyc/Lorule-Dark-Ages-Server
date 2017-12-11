namespace Darkages.Network.ClientFormats
{
    public class ClientFormat02 : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x02; }
        }

        public string AislingUsername { get; set; }
        public string AislingPassword { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            this.AislingUsername = reader.ReadStringA();
            this.AislingPassword = reader.ReadStringA();
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}