namespace Darkages.Network.ClientFormats
{
    public class ClientFormat3A : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x3A; }
        }

        public ushort ScriptId { get; set; }
        public byte Step { get; set; }
        public uint Serial { get; set; }


        public override void Serialize(NetworkPacketReader reader)
        {
            var type = reader.ReadByte();
            var id = reader.ReadUInt32();
            var scriptid = reader.ReadUInt16();
            var step = reader.ReadByte();

            ScriptId = scriptid;
            Step = step;
            Serial = id;
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}