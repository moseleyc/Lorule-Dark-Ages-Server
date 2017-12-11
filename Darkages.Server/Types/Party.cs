using System.Collections.Generic;
using System.Linq;

namespace Darkages.Types
{
    public class Party
    {
        public List<PartyMember> Members = new List<PartyMember>();

        public bool Has(Aisling _aisling)
        {
            return Members.Count(i => i.Aisling != null
                         && i.Aisling.Username.ToLower() == _aisling.Username.ToLower())
                         > 0;
        }

        public void Add(Aisling _aisling, bool isLeader = false)
        {
            if (_aisling == null)
                return;

            //make sure we don't add existing members, if so, remove old, add new.
            var removables = new List<PartyMember>();
            removables.AddRange(Members.Where(i => i.Aisling.Username.ToLower() == _aisling.Username.ToLower()));
            removables.ForEach(i => Members.Remove(i));

            Members.Add(new PartyMember(_aisling) { Leader = isLeader } );
        }

        public void Remove(Aisling _aisling)
        {
            var obj = Members.FirstOrDefault(i => i.Aisling != null && i.Aisling.Username.ToLower() == _aisling.Username.ToLower());

            if (obj != null)
                Members.Remove(obj);
        }
    }
}
