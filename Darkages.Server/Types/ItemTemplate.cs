using static Darkages.Types.ElementManager;

namespace Darkages.Types
{
    public class ItemTemplate : Template
    {
        public int ID { get; set; }

        public bool CanStack { get; set; }

        public byte MaxStack { get; set; }

        public ushort Image { get; set; }

        public ushort DisplayImage { get; set; }

        public string ScriptName { get; set; }

        public Gender Gender { get; set; }

        public StatusOperator HealthModifer { get; set; }

        public StatusOperator ManaModifer { get; set; }

        public StatusOperator StrModifer { get; set; }

        public StatusOperator IntModifer { get; set; }

        public StatusOperator WisModifer { get; set; }

        public StatusOperator ConModifer { get; set; }

        public StatusOperator DexModifer { get; set; }

        public StatusOperator AcModifer { get; set; }

        public StatusOperator MrModifer { get; set; }

        public StatusOperator HitModifer { get; set; }

        public StatusOperator DmgModifer { get; set; }

        public SpellOperator SpellOperator { get; set; }

        public Element OffenseElement { get; set; }

        public Element DefenseElement { get; set; }

        public int Upgrades { get; set; }

        public byte Weight { get; set; }

        public ItemFlags Flags { get; set; }

        public uint MaxDurability { get; set; }

        public uint Value { get; set; }

        public int EquipmentSlot { get; set; }

        public string NpcKey { get; set; }

        public Class Class { get; set; }

        public byte LevelRequired { get; set; }

        public int DmgMin { get; set; }

        public int DmgMax { get; set; }

        public double DropRate { get; set; }

        public ClassStage StageRequired { get; set; }

        public bool HasPants { get; set; }
    }
}