using System;
using System.IO;
using Darkages.Compression;

namespace Darkages.Types
{
    public class MetafileManager
    {
        private static readonly MetafileCollection metafiles;

        static MetafileManager()
        {
            var files = Directory.GetFiles($"{ServerContext.StoragePath}/metafile/");
            metafiles = new MetafileCollection(files.Length);

            foreach (var file in files)
                metafiles.Add(
                    CompressableObject.Load<Metafile>(file, true));
        }

        public static Metafile GetMetafile(string name)
        {
            return metafiles.Find(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static MetafileCollection GetMetafiles()
        {
            return metafiles;
        }
    }
}