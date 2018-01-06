using Newtonsoft.Json;
using System.Collections.Generic;

namespace Darkages.Types
{
    public class WorldMapTemplate : Template
    {
        [JsonProperty]
        public int FieldNumber { get; set; }

        [JsonProperty]
        public Warp Transition { get; set; }

        [JsonProperty]
        public List<WorldPortal> Portals = new List<WorldPortal>();
    }

    public class WorldPortal
    {
        [JsonProperty]
        public string DisplayName { get; set; }

        [JsonProperty]
        public short PointX { get; set; }

        [JsonProperty]
        public short PointY { get; set; }

        [JsonProperty]
        public Warp Destination { get; set; }
    }
}
