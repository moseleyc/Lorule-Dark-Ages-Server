using Newtonsoft.Json;
using System.Collections.Generic;

namespace Darkages.Types
{
    public class WorldMapTemplate : Template
    {
        public int FieldNumber { get; set; }

        public Warp Transition { get; set; }

        public List<WorldPortal> Portals = new List<WorldPortal>();
    }

    public class WorldPortal
    {
        public string DisplayName { get; set; }

        public short PointX { get; set; }

        public short PointY { get; set; }

        public Warp Destination { get; set; }
    }
}
