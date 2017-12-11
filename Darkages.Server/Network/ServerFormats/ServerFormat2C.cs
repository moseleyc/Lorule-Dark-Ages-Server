namespace Darkages.Network.ServerFormats
{
    public class ServerFormat2C : NetworkFormat
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
                return 0x2C;
            }
        }

        public byte Slot { get; set; }
        public short Icon { get; set; }
        public string Text { get; set; }
        
        public ServerFormat2C(byte slot, short icon, string text)
        {
            Slot = slot;
            Icon = icon;
            Text = text;
        }
        
        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(Slot);
            writer.Write(Icon);
            writer.WriteStringA(Text);
        }
    }
}
