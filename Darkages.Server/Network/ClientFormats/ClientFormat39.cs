namespace Darkages.Network.ClientFormats
{
    public class ClientFormat39 : NetworkFormat
    {
        public override bool Secured => true;

        public override byte Command => 0x39;

        public byte Type { get; set; }
        public int Serial { get; set; }
        public ushort Step { get; set; }
        public string Args { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            Type = reader.ReadByte();
            Serial = reader.ReadInt32();
            Step = reader.ReadUInt16();

            if (reader.CanRead)
                Args = reader.ReadStringA();
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}