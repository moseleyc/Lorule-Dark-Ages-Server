namespace Darkages.Network.ServerFormats
{
    public class ServerFormat2E : NetworkFormat
    {
        public override bool Secured => true;
        public override byte Command => 0x2E;

        public string ImageName { get; set; }
//        public MapPoint MapPoint { get; set; }
        public byte Points { get; set; }
        
//        public ushort 
        
        public ServerFormat2E( )
        {
            
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
        
    }
}