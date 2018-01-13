using System;

namespace Darkages.Types
{
    public class CastInfo
    {
        public byte Slot;
        public byte SpellLines;
        public DateTime Started;

        public uint Target { get; internal set; }
        public Position Position { get; internal set; }
    }
}