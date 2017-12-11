using Darkages.Common;
using Darkages.Compression;
using Darkages.IO;
using System.IO;

namespace Darkages.Types
{
    public class Notification : CompressableObject
    {
        public byte[] Data
        {
            get { return base.DeflatedData; }
        }
        public ushort Size
        {
            get { return (ushort)base.DeflatedData.Length; }
        }
        public uint Hash { get; private set; }

        public static Notification FromFile(string filename)
        {
            var result = new Notification();

            result.InflatedData = File.ReadAllText(filename).ToByteArray();
            result.Hash = Crc32Provider.ComputeChecksum(result.InflatedData);
            result.Deflate();

            return result;
        }
    }
}