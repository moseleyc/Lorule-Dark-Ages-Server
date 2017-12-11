using System;

namespace Darkages.Types
{
    public class CastInfo
    {
        public DateTime Started;
        public byte SpellLines;
        public byte Slot;

        public uint Target { get; internal set; }
        public Position Position { get; internal set; }
    }
}
