﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Darkages.Network.Object;

namespace Darkages.Types
{
    public class SpellBook : ObjectManager
    {
        public static readonly int SPELLLENGTH = 35 * 3;

        public Dictionary<int, Spell> Spells = new Dictionary<int, Spell>();

        public SpellBook()
        {
            for (var i = 0; i < SPELLLENGTH; i++) Spells[i + 1] = null;
        }


        public int Length => Spells.Count;

        public Spell FindInSlot(int Slot)
        {
            if (Spells.ContainsKey(Slot))
                return Spells[Slot];

            return null;
        }

        public void Assign(Spell spell)
        {
            Set(spell);
        }

        public new Spell[] Get(Predicate<Spell> prediate)
        {
            return Spells.Values.Where(i => i != null && prediate(i)).ToArray();
        }

        public void Swap(Spell A, Spell B)
        {
            A = Interlocked.Exchange(ref B, A);
        }

        public void Set(Spell s)
        {
            Spells[s.Slot] = Clone(s);
        }

        public void Set(Spell s, bool clone = false)
        {
            Spells[s.Slot] = clone ? Clone(s) : s;
        }

        public void Clear(Spell s)
        {
            Spells[s.Slot] = null;
        }

        public Spell Remove(byte movingFrom)
        {
            var copy = Spells[movingFrom];
            Spells[movingFrom] = null;
            return copy;
        }

        public int FindEmpty()
        {
            for (var i = 0; i < Length; i++)
                if (Spells[i + 1] == null)
                    return i + 1;

            return -1;
        }
    }
}