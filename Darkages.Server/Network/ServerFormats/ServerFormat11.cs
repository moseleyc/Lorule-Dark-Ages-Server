using Darkages.Types;
using System.Collections.Generic;

namespace Darkages.Network.ServerFormats
{
    public class ServerFormat11 : NetworkFormat
    {
        public override bool Secured
        {
            get
            {
                return true;
            }
        }
        public override byte Command
        {
            get
            {
                return 0x11;
            }
        }

        public byte Direction { get; set; }
        public int Serial { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(Serial);
            writer.Write(Direction);
        }
    }
}
