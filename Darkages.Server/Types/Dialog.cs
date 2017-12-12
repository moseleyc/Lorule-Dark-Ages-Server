using Darkages.Common;
using System.Collections.Generic;

namespace Darkages.Types
{
    public class Dialog
    {
        public List<DialogSequence> Sequences = new List<DialogSequence>();
        public DialogSequence Current => Sequences[SequenceIndex];

        public int Serial { get; set; }
        public int SequenceIndex { get; set; }
        public bool CanMoveNext => SequenceIndex + 1 < Sequences.Count;
        public bool CanMoveBack => SequenceIndex - 1 >= 0;
        public ushort DisplayImage { get; set; }

        public Dialog()
        {
            lock (Generator.Random)
            {
                Serial = Generator.GenerateNumber();
            }
        }
    }
}
