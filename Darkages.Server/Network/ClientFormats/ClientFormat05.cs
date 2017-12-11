namespace Darkages.Network.ClientFormats
{
    public class ClientFormat05 : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x05; }
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}