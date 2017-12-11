using Darkages.Types;
using System.Collections.Generic;

namespace Darkages.Network.ClientFormats
{
    public class ClientFormat13 : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x13; }
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}