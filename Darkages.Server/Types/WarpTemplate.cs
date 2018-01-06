using Newtonsoft.Json;

namespace Darkages.Types
{
    public class WarpTemplate : Template
    {
        public byte LevelRequired { get; set; }
        public int WarpRadius { get; set; }
        public Warp From { get; set; }
        public Warp To { get; set; }
        public WarpType WarpType { get; set; }
    }

    public enum WarpType
    {
        Map,
        World
    }
}
