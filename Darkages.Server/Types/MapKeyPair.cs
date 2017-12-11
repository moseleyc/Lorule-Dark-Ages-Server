namespace Darkages.Types
{
    public class MapKeyPair
    {
        public int Number
        {
            get; set;
        }

        public ushort Key
        {
            get; set;
        }

        public MapKeyPair(int _number, ushort _key)
        {
            this.Number = _number;
            this.Key    = _key;
        }
    }
}
