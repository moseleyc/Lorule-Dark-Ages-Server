namespace Darkages.Network.ServerFormats
{
    public class ServerFormat05 : NetworkFormat
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
                return 0x05;
            }
        }

        public Aisling Aisling { get; set; }

        public ServerFormat05(Aisling aisling)
        {
            this.Aisling = aisling;
        }

        //05 A2 [03 95 C1 2D] [02 00 01 00] [00 00]
        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(this.Aisling.Serial);
            writer.Write(new byte[]
            {
                0x02,
                0x00,
                0x01,
                0x00,
                0x00,
                0x00,
                               });
        }
    }
}
