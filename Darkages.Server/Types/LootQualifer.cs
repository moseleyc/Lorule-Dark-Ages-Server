using System;

namespace Darkages.Types
{
    [Flags]
    public enum LootQualifer
    {
        Random = 1 << 1,
        Defined = 1 << 2,
        Table = 1 << 3,
        Event = 1 << 4,
        None = 1 << 5,
        Gold = 1 << 6
    }
}