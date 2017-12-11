using Darkages.Compression;
using Darkages.IO;
using Darkages.Network;
using System.Collections.ObjectModel;
using System.IO;

namespace Darkages.Types
{
    public class Metafile : CompressableObject, IFormattable
    {
        public Collection<MetafileNode> Nodes { get; private set; }
        public uint Hash { get; private set; }
        public string Name { get; private set; }

        public Metafile()
        {
            this.Nodes = new Collection<MetafileNode>();
        }

        public override void Load(MemoryStream stream)
        {
            using (var reader = new BufferReader(stream))
            {
                int length = reader.ReadUInt16();

                for (int i = 0; i < length; i++)
                {
                    var node = new MetafileNode(reader.ReadStringA());
                    var atomSize = reader.ReadUInt16();

                    for (int j = 0; j < atomSize; j++)
                    {
                        node.Atoms.Add(
                            reader.ReadStringB());
                    }

                    this.Nodes.Add(node);
                }
            }

            this.Hash = Crc32Provider.ComputeChecksum(base.InflatedData);
            this.Name = Path.GetFileName(base.Filename);
        }
        public override void Save(MemoryStream stream)
        {
            using (var writer = new BufferWriter(stream))
            {
                writer.Write(
                    (ushort)this.Nodes.Count);

                foreach (var node in this.Nodes)
                {
                    writer.WriteStringA(node.Name);
                    writer.Write(
                        (ushort)node.Atoms.Count);

                    foreach (var atom in node.Atoms)
                    {
                        writer.WriteStringB(atom);
                    }
                }
            }
        }

        public void Serialize(NetworkPacketReader reader)
        {
        }
        public void Serialize(NetworkPacketWriter writer)
        {
            writer.WriteStringA(this.Name);
            writer.Write(this.Hash);
            writer.Write(
                (ushort)base.DeflatedData.Length);
            writer.Write(base.DeflatedData);
        }
    }
}