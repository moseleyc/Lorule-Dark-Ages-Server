using Darkages.Compression;
using System;
using System.IO;

namespace Darkages.Types
{
    public class MetafileManager
    {
        private static MetafileCollection metafiles;

        static MetafileManager()
        {
            var files = Directory.GetFiles($"{ServerContext.STORAGE_PATH}/metafile/");
            metafiles = new MetafileCollection(files.Length);

            foreach (var file in files)
            {
                metafiles.Add(
                    CompressableObject.Load<Metafile>(file, true));
            }
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