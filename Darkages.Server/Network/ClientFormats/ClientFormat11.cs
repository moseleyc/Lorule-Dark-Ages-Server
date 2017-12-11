using Darkages.Types;
using System.Collections.Generic;

namespace Darkages.Network.ClientFormats
{
    public class ClientFormat11 : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x11; }
        }

        public byte Direction { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            this.Direction = reader.ReadByte();
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}