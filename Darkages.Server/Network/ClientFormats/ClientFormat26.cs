namespace Darkages.Network.ClientFormats
{
    public class ClientFormat26 : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x26; }
        }

        public string Username { get; set;}
        public string Password { get; set; }
        public string NewPassword { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            this.Username = reader.ReadStringA();
            this.Password = reader.ReadStringA();
            this.NewPassword = reader.ReadStringA();
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}