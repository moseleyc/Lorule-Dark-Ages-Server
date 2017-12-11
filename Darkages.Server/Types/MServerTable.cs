using Darkages.Compression;
using Darkages.IO;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace Darkages.Types
{
    public class MServerTable : CompressableObject
    {
        public Collection<MServer> Servers { get; set; }

        [XmlIgnore]
        public ushort Size
        {
            get { return (ushort)this.DeflatedData.Length; }
        }
        [XmlIgnore]
        public byte[] Data
        {
            get { return this.DeflatedData; }
        }
        [XmlIgnore]
        public uint Hash { get; set; }

        public MServerTable()
        {
            this.Servers = new Collection<MServer>();
        }

        public static MServerTable FromFile(string filename)
        {
            MServerTable result;

            using (var stream = File.OpenRead(filename))
            {
                result = new XmlSerializer(typeof(MServerTable)).Deserialize(stream) as MServerTable;
            }

            using (var stream = new MemoryStream())
            {
                result.Save(stream);
                result.InflatedData = stream.ToArray();
            }

            result.Hash = Crc32Provider.ComputeChecksum(result.InflatedData);
            result.Deflate();

            return result;
        }

        public override void Load(MemoryStream stream)
        {
            using (var reader = new BufferReader(stream))
            {
                var count = reader.ReadByte();

                for (int i = 0; i < count; i++)
                {
                    var server = new MServer();

                    server.Guid = reader.ReadByte();
                    server.Address = reader.ReadIPAddress();
                    server.Port = reader.ReadUInt16();

                    var text = reader.ReadString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    server.Name = text[0];
                    server.Description = text[1];

                    reader.ReadByte();

                    this.Servers.Add(server);
                }
            }
        }
        public override void Save(MemoryStream stream)
        {
            using (var writer = new BufferWriter(stream))
            {
                writer.Write(
                    (byte)this.Servers.Count);

                foreach (var server in this.Servers)
                {
                    writer.Write(server.Guid);
                    writer.Write(server.Address);
                    writer.Write(server.Port);
                    writer.Write(server.Name + ";" + server.Description);
                    writer.Write(byte.MinValue);
                }
            }
        }
    }
}