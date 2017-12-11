using System.Collections.Generic;
using Darkages.Types;

namespace Darkages.Network.ClientFormats
{
    public class ClientFormat06 : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x06; }
        }

        public byte Direction { get; set; }
        public byte StepCount { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            this.Direction = reader.ReadByte();
            this.StepCount = reader.ReadByte();
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}