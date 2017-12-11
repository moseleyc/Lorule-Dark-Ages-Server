namespace Darkages.Types
{
    public class WarpTemplate : Template
    {
        public Area Destination { get; set; }
        public byte LevelRequired { get; set; }
        public Position Location { get; set; }
        public Position DestinationPosition { get; set; }
        public int WarpRadius { get; set; }
        public int AreaID { get; set; }
    }
}
