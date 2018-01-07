using System;
using System.Collections.Generic;
using System.IO;
using Darkages.Types;
using Newtonsoft.Json;

namespace Darkages.Storage
{
    public class TemplateStorage<T> where T : Template, new()
    {
        public static string StoragePath;

        static TemplateStorage()
        {
            if (ServerContext.STORAGE_PATH == null)
                ServerContext.LoadConstants();

            StoragePath = $@"{ServerContext.STORAGE_PATH}\templates";

            var tmp = new T();

            StoragePath = Path.Combine(StoragePath, "%");

            if (tmp is SkillTemplate)
                StoragePath = StoragePath.Replace("%", "Skills");

            if (tmp is SpellTemplate)
                StoragePath = StoragePath.Replace("%", "Spells");

            if (tmp is MonsterTemplate)
                StoragePath = StoragePath.Replace("%", "Monsters");

            if (tmp is ItemTemplate)
                StoragePath = StoragePath.Replace("%", "Items");

            if (tmp is MundaneTemplate)
                StoragePath = StoragePath.Replace("%", "Mundanes");

            if (tmp is WorldMapTemplate)
                StoragePath = StoragePath.Replace("%", "WorldMaps");


            if (!Directory.Exists(StoragePath))
                Directory.CreateDirectory(StoragePath);
        }

        public void CacheFromStorage()
        {
            var results = new List<T>();
            var asset_names = Directory.GetFiles(
                StoragePath,
                "*.json",
                SearchOption.TopDirectoryOnly);

            if (asset_names.Length == 0)
                return;

            foreach (var asset in asset_names)
            {
                var tmp = new T();

                if (tmp is SkillTemplate)
                {
                    var template =
                        StorageManager.SKillBucket.Load<SkillTemplate>(Path.GetFileNameWithoutExtension(asset));
                    ServerContext.GlobalSkillTemplateCache[template.Name] = template;

                    Console.WriteLine(" -> {0} Loaded From {1}", template.Name, Path.GetFileName(asset));
                }
                else if (tmp is SpellTemplate)
                {
                    var template =
                        StorageManager.SpellBucket.Load<SpellTemplate>(Path.GetFileNameWithoutExtension(asset));
                    ServerContext.GlobalSpellTemplateCache[template.Name] = template;

                    Console.WriteLine(" -> {0} Loaded From {1}", template.Name, Path.GetFileName(asset));
                }
                else if (tmp is MonsterTemplate)
                {
                    var template =
                        StorageManager.MonsterBucket.Load<MonsterTemplate>(Path.GetFileNameWithoutExtension(asset));
                    ServerContext.GlobalMonsterTemplateCache[template.Name] = template;

                    Console.WriteLine(" -> {0} Loaded From {1}", template.Name, Path.GetFileName(asset));
                }
                else if (tmp is MundaneTemplate)
                {
                    var template =
                        StorageManager.MundaneBucket.Load<MundaneTemplate>(Path.GetFileNameWithoutExtension(asset));
                    ServerContext.GlobalMundaneTemplateCache[template.Name] = template;

                    Console.WriteLine(" -> {0} Loaded From {1}", template.Name, Path.GetFileName(asset));
                }
                else if (tmp is ItemTemplate)
                {
                    var template =
                        StorageManager.ItemBucket.Load<ItemTemplate>(Path.GetFileNameWithoutExtension(asset));
                    ServerContext.GlobalItemTemplateCache[template.Name] = template;

                    Console.WriteLine(" -> {0} Loaded From {1}", template.Name, Path.GetFileName(asset));
                }
                else if (tmp is WorldMapTemplate)
                {
                    var template =
                        StorageManager.WorldMapBucket.Load<WorldMapTemplate>(Path.GetFileNameWithoutExtension(asset));
                    ServerContext.GlobalWorldMapTemplateCache[template.FieldNumber] = template;

                    Console.WriteLine(" -> {0} Loaded From {1}", template.Name, Path.GetFileName(asset));
                }

            }
        }

#pragma warning disable CS0693 
        public T Load<T>(string Name) where T : Template
#pragma warning restore CS0693 
        {
            var path = Path.Combine(StoragePath, string.Format("{0}.json", Name.ToLower()));

            if (!File.Exists(path))
                return null;

            using (var s = File.OpenRead(path))
            using (var f = new StreamReader(s))
            {
                return JsonConvert.DeserializeObject<T>(f.ReadToEnd(), StorageManager.Settings);
            }
        }

        public void Save(T obj, bool replace = true)
        {
            var path = MakeUnique(Path.Combine(StoragePath, string.Format("{0}.json", obj.Name.ToLower())))
                .FullName;


            var objString = JsonConvert.SerializeObject(obj, StorageManager.Settings);
            File.WriteAllText(path, objString);
        }

        public FileInfo MakeUnique(string path)
        {
            string dir = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string fileExt = Path.GetExtension(path);

            for (int i = 1; ; ++i)
            {
                if (!File.Exists(path))
                    return new FileInfo(path);

                path = Path.Combine(dir, fileName + " " + i + fileExt);
            }
        }
    }
}