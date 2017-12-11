using System;

namespace Darkages.Types
{
    [Flags]
    public enum PreQualifer
    {
        RequiresInvisible = 0,
        RequiresWeapon = 1,
        RequresShield = 2,
        RequiresHands = 3,
        None = 4,
    }
}