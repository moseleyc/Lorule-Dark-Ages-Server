using System;

namespace Darkages.Types
{
    [Flags]
    public enum SpawnQualifer
    {
        Random = 1,
        Defined = 2,
        NearPlayers = 3,
    }
}