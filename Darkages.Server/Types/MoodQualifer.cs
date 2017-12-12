using System;

namespace Darkages.Types
{
    [Flags]
    public enum MoodQualifer
    {
        Aggressive   = 1 << 1,
        Friendly     = 1 << 1,
        Defensive    = 1 << 3,
        Unpredicable = 1 << 4,
    }
}