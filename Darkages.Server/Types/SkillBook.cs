using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Darkages.Network.Object;

namespace Darkages.Types
{
    public class SkillBook : ObjectManager
    {
        public static readonly int SKILLLENGTH = 35;

        public Dictionary<int, Skill> Skills = new Dictionary<int, Skill>();

        public SkillBook()
        {
            for (var i = 0; i < SKILLLENGTH; i++) Skills[i + 1] = null;
        }


        public int Length => Skills.Count;

        public Skill FindInSlot(int Slot)
        {
            return Skills[Slot];
        }

        public void Assign(Skill skill)
        {
            Set(skill);
        }

        public new Skill[] Get(Predicate<Skill> prediate)
        {
            return Skills.Values.Where(i => i != null && prediate(i)).ToArray();
        }

        public void Swap(Skill A, Skill B)
        {
            A = Interlocked.Exchange(ref B, A);
        }

        public void Set(Skill s)
        {
            Skills[s.Slot] = Clone(s);
        }

        public void Set(Skill s, bool clone = false)
        {
            Skills[s.Slot] = clone ? Clone(s) : s;
        }

        public void Clear(Skill s)
        {
            Skills[s.Slot] = null;
        }

        public Skill Remove(byte movingFrom)
        {
            var copy = Skills[movingFrom];
            Skills[movingFrom] = null;
            return copy;
        }

        public int FindEmpty()
        {
            for (var i = 0; i < Length; i++)
                if (Skills[i + 1] == null)
                    return i + 1;
            return -1;
        }
    }
}