namespace Darkages
{
    public class PartyMember
    {
        public Aisling Aisling { get; set; }
        public bool Leader { get; set; }

        public PartyMember(Aisling _aisling)
        {
            Aisling = _aisling;
        }
    }
}