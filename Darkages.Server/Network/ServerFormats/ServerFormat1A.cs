namespace Darkages.Network.ServerFormats
{
    public class ServerFormat1A : NetworkFormat
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
                return 0x1A;
            }
        }

        public int Serial { get; set; }
        public byte Number { get; set; }
        public short Speed { get; set; }

        public ServerFormat1A()
        {
            
        }
        
        public ServerFormat1A(int serial, byte number, short speed)
        {
            Serial = serial;
            Number = number;
            Speed = speed;
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(Serial);
            writer.Write(Number);
            writer.Write(Speed);
            writer.Write(byte.MaxValue);
        }
    }
}
