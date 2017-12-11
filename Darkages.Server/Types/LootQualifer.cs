using System;

namespace Darkages.Types
{
    [Flags]
    public enum LootQualifer
    {
        Random  = 1,
        Defined = 2,
        Table   = 3,
        Event   = 4,
        None    = 5,
        Gold    = 6
    }
}