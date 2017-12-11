namespace Darkages.Network.ClientFormats
{
    public class ClientFormat03 : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x03; }
        }

        public string Username { get; set; }
        public string Password { get; set; }
        
        public override void Serialize(NetworkPacketReader reader)
        {
            this.Username = reader.ReadStringA();
            this.Password = reader.ReadStringA();
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}