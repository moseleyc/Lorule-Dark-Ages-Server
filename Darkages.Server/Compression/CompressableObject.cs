using System.IO;
using System.Xml.Serialization;

namespace Darkages.Compression
{
    public class CompressableObject
    {
        [XmlIgnore]
        public byte[] DeflatedData { get; set; }
        [XmlIgnore]
        public byte[] InflatedData { get; set; }
        [XmlIgnore]
        public string Filename { get; set; }

        public static T Load<T>(string filename, bool deflated = true)
            where T : CompressableObject, new()
        {
            T result = new T();

            if (deflated)
            {
                result.DeflatedData = File.ReadAllBytes(filename);
                result.Inflate();
            }
            else
            {
                result.InflatedData = File.ReadAllBytes(filename);
                result.Deflate();
            }

            result.Filename = filename;

            using (var stream = new MemoryStream(result.InflatedData))
            {
                result.Load(stream);
            }

            return result;
        }
        public static void Save(string filename, CompressableObject obj)
        {
            using (var stream = new MemoryStream())
            {
                obj.Save(stream);
                obj.InflatedData = stream.ToArray();
            }

            obj.Deflate();

            File.WriteAllBytes(filename + ".deflated", obj.DeflatedData);
            File.WriteAllBytes(filename + ".inflated", obj.InflatedData);
        }

        public void Deflate()
        {
            this.DeflatedData = CompressionProvider.Deflate(this.InflatedData);
        }
        public void Inflate()
        {
            this.InflatedData = CompressionProvider.Inflate(this.DeflatedData);
        }

        public virtual void Load(MemoryStream stream) { }
        public virtual void Save(MemoryStream stream) { }
    }
}