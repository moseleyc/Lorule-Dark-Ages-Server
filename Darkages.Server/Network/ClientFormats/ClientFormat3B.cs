namespace Darkages.Network.ClientFormats
{
    public class ClientFormat3B : NetworkFormat
    {
        public override bool Secured => true;

        public override byte Command => 0x3B;

        public override void Serialize(NetworkPacketReader reader)
        {

        }

        public override void Serialize(NetworkPacketWriter writer)
        {

        }
    }
}
