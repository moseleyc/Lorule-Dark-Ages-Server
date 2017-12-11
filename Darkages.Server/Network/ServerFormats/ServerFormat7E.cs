using Darkages.Network.Game;

namespace Darkages.Network.ServerFormats
{
    public class ServerFormat7E : NetworkFormat
    {
        public override bool Secured
        {
            get
            {
                return false;
            }
        }
        public override byte Command
        {
            get
            {
                return 0x7E;
            }
        }

        public byte Type
        {
            get
            {
                return 0x1B;
            }
        }
        public string Text
        {
            get
            {
                return ServerContext.Config.HandShakeMessage;
            }
        }


        public override void Serialize(NetworkPacketReader reader)
        {

        }
        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(Type);
            writer.WriteString(Text);
        }
    }
}
