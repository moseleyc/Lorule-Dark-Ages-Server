using Darkages.Network.Game;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Darkages.Types
{
    public class MundaneTemplate : Template
    {
        public MundaneTemplate()
        {
            Speech = new Collection<string>();
        }

        public short Image { get; set; }
        public byte Level { get; set; }
        public string ScriptKey { get; set; }

        public bool EnableWalking { get; set; }
        public bool EnableTurning { get; set; }
        public bool EnableAttacking { get; set; }
        public bool EnableSpeech { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public GameServerTimer TurnTimer { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public GameServerTimer ChatTimer { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public GameServerTimer AttackTimer { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public GameServerTimer WalkTimer { get; set; }

        public Collection<string> Speech { get; set; }

        public ushort X { get; set; }
        public ushort Y { get; set; }
        public int AreaID { get;  set; }
        public byte Direction { get; set; }

        [Browsable(false)]
        public int MaximumHp { get; set; }

        [Browsable(false)]
        public int MaximumMp { get; set; }
    }
}
