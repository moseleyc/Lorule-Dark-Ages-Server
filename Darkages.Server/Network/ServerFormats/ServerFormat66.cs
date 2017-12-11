namespace Darkages.Network.ServerFormats
{
    public class ServerFormat66 : NetworkFormat
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
                return 0x66;
            }
        }

        public byte Type
        {
            get
            {
                return 0x03;
            }
        }
        public string Text
        {
            get
            {
                return "https://classicrpgcharacter.nexon.com/service/ConfirmGameUser.aspx?id=%s&pw=%s&mainCode=2&subCode=0";
            }
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(Type);
            writer.WriteStringA(Text);
        }
    }
}
