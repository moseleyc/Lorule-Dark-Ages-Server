namespace Darkages.Network.ServerFormats
{
    public class ServerFormat13 : NetworkFormat
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
                return 0x13;
            }
        }
        public int Serial { get; set; }
        public ushort Health { get; set; }
        public byte Sound { get; set; }

        public ServerFormat13()
        {
            
        }

        public ServerFormat13(int serial, byte health)
        {
            Serial = serial;
            Health = health;
            Sound = 0xFF;
        }

        public ServerFormat13(int serial, byte health, byte sound)
        {
            Serial = serial;
            Health = health;
            Sound = sound;
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(Serial);
            writer.Write(Health);
            writer.Write(Sound);
        }
    }
}
