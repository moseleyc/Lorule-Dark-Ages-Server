namespace Darkages.Network.ClientFormats
{
    public class ClientFormat79 : NetworkFormat
    {
        public override bool Secured => true;
        public override byte Command => 0x79;

        public override void Serialize(NetworkPacketReader reader)
        {
            var totalLength = reader.ReadUInt16();
            var portraitLength = reader.ReadUInt16();
            var portraitData = reader.ReadBytes(portraitLength);
            var profileMsg = reader.ReadStringA();
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}