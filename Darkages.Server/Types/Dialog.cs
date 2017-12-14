using Darkages.Common;
using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using System;

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

        public DialogSequence Invoke(GameClient client)
        {
            client.Send(new ServerFormat30(client, this));
            {
                Current?.Callback?.Invoke(client.Aisling, Current);
                return Current;
            }
        }

        public void MoveNext(GameClient client)
        {
            if (CanMoveNext)
                SequenceIndex++;

            client.DlgSession.Sequence = (ushort)SequenceIndex;
        }
    }
}
