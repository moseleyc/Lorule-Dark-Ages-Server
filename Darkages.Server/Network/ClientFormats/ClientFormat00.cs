using System;
using Darkages.Types;

namespace Darkages.Network.ClientFormats
{
    public class ClientFormat00 : NetworkFormat
    {
        public override bool Secured
        {
            get { return false; }
        }
        public override byte Command
        {
            get { return 0x00; }
        }

        public int Version { get; set; }
        public byte UnknownA { get; set; }
        public byte UnknownB { get; set; }


        public override void Serialize(NetworkPacketReader reader)
        {
            this.Version = reader.ReadUInt16();
            this.UnknownA = reader.ReadByte();
            this.UnknownB = reader.ReadByte();
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}