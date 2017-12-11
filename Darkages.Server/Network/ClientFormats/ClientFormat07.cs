using Darkages.Types;

namespace Darkages.Network.ClientFormats
{
    public class ClientFormat07 : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x07; }
        }


        public Position Position { get; set; }
        public byte PickupType { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            PickupType = reader.ReadByte();
            Position = reader.ReadPosition();
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}
