using System.Collections.Specialized;

namespace Darkages.Types
{
    public class MetafileNode
    {
        public string Name { get; private set; }
        public StringCollection Atoms { get; private set; }

        public MetafileNode(string name, params string[] atoms)
        {
            this.Name = name;
            this.Atoms = new StringCollection();
            this.Atoms.AddRange(atoms);
        }
    }
}