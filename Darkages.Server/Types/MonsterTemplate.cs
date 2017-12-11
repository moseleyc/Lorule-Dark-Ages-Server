using Darkages.Network.Game;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Darkages.Types
{
    public class MonsterTemplate : Template
    {
        [Description("Template ID. Must be Unique.")]
        public int ID { get; set; }

        [Description("What sprite ID? range from 0x4000 - 0x8000 ")]
        public ushort Image { get; set; }

        [Description("What script will this monster run?")]
        public string ScriptName { get; set; }

        [Description("Leave Empty unless SpawnQualifer = Reactor.")]
        public string ReactorScriptName { get; set; }

        [Description("Leave empty unless SpawnQualifer = Defined.")]
        public ushort DefinedX { get; set; }

        [Description("Leave empty unless SpawnQualifer = Defined.")]
        public ushort DefinedY { get; set; }

        [Description("Scaling Exponent")]
        public double Exponent { get; set; }

        [Description("Scaling Proponent Overflow")]
        public double Coponent { get; set; }

        [Description("Damage Scale Factor. ")]
        public double Factor { get; set; }

        public LootQualifer LootType { get; set; }

        public MoodQualifer MoodTyle { get; set; }

        public SpawnQualifer SpawnType { get; set; }

        public ElementQualifer ElementType { get; set; }

        public PathQualifer PathQualifer { get; set; }

        public byte Level { get; set; }

        public int MaximumHP { get; set; }

        public int MaximumMP { get; set; }

        public int SpawnerID { get; set; }

        public int AreaID { get; set; }

        [Description("Regen time in seconds, each tick = 10% health + mana.")]
        public int RegenSpeed { get; set; }

        public int MovementSpeed { get; set; }

        public int CastSpeed { get; set; }

        [Description("Base Damage will be +- % mod.")]
        public int ArmorClass { get; set; }

        [Description("Spells casted on this monster will miss by 30% if i give the value 70.")]
        public byte MagicResist { get; set; }

        [Description("Monsters spawned will not exceed this.")]
        public int SpawnMax { get; set; }

        [Description("In seconds, what is the respawn rate?")]
        public int SpawnRate { get; set; }

        [Description("How many monsters will i spawn at any single time?")]
        public int SpawnSize { get; set; }

        public int AttackSpeed { get; set; }

        [JsonIgnore]
        [Browsable(false)]
        public GameServerTimer Timer { get; set; }

        [Description("Does this monster have various other sprites? use 0 if not.")]
        public int ImageVarience { get; set; }

        [Description("Does this aisling spawn if no aislings are on this map? default = false")]
        public bool SpawnOnlyOnActiveMaps { get; set; }

        [Description("Does this monster script run when no aislings are nearby? default = false")]
        public bool UpdateMapWide { get; set; }

        [Description("Does this monster grow stonger over time? default = false")]
        public bool Grow { get; set; }

        [JsonProperty]
        [Description("What Spells will this monster cast?")]
        public Collection<string> SpellScripts { get; set; }
    }
}