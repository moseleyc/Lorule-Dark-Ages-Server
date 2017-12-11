namespace Darkages.Network.ServerFormats
{
    public class ServerFormat02 : NetworkFormat
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
                return 0x2;
            }
        }

        public byte Code { get; set; }
        public string Text { get; set; }

        public ServerFormat02(byte code, string text)
        {
            Code = code;
            Text = text;
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(Code);
            writer.WriteStringA(Text);
        }
    }
}
