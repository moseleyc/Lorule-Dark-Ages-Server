using Newtonsoft.Json;
using System;

namespace Darkages.Types
{
    public class SpellTemplate : Template
    {
        [Flags]
        public enum SpellTargetScope
        {
            Self = 1,
            SingleTarget = 2,
            Area = 3,
            InFront = 4,
            Group = 5,
            GroupExcludingSelf = 6,
            NearbyAislings = 7,
            NearbySprites = 8,
            AllSpritesOnMap = 9,
            EveryAisling = 10,
            EverySprite = 11,
            All = EveryAisling | EverySprite,
        }

        public enum IconStatus : byte
        {
            Active = 0x00,
            Available = 0x01,
            Unavailable = 0x02
        }

        public enum SpellUseType : byte
        {
            Unusable = 0,
            Prompt = 1,
            ChooseTarget = 2,
            FourDigit = 3,
            ThreeDigit = 4,
            NoTarget = 5,
            TwoDigit = 6,
            OneDigit = 7
        }


        public byte Icon { get; set; }
        public byte MaxLevel { get; set; }
        public string ScriptKey { get; set; }
        public int MinLines { get; set; }
        public int MaxLines { get; set; }
        public int ManaCost { get; set; }
        public string Text { get; set; }
        public Debuff Debuff { get; set; }
        public Buff Buff { get; set; }
        public SpellUseType TargetType { get; set; }
        public int BaseLines { get; set; }
        public double LevelRate { get; set; }

        public SpellTemplate()
        {
            Text = string.Empty + "\0";
        }
    }
}