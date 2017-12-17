using System.IO;
using Darkages.Network.Game;
using Darkages.Network.Object;
using Darkages.Types;
using Newtonsoft.Json;

namespace Darkages.Storage
{
    public class StorageManager
    {
        public static JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Auto,
            Formatting = Formatting.Indented,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
        };

        public static AislingStorage AislingBucket = new AislingStorage();
        public static AreaStorage AreaBucket = new AreaStorage();

        public static TemplateStorage<SkillTemplate> SKillBucket = new TemplateStorage<SkillTemplate>();
        public static TemplateStorage<SpellTemplate> SpellBucket = new TemplateStorage<SpellTemplate>();
        public static TemplateStorage<ItemTemplate> ItemBucket = new TemplateStorage<ItemTemplate>();
        public static TemplateStorage<MonsterTemplate> MonsterBucket = new TemplateStorage<MonsterTemplate>();
        public static TemplateStorage<MundaneTemplate> MundaneBucket = new TemplateStorage<MundaneTemplate>();
        public static TemplateStorage<WarpTemplate> WarpBucket = new TemplateStorage<WarpTemplate>();

        public static T LoadFrom<T>(string path) where T : Template
        {
            if (!File.Exists(path))
                return null;

            using (var s = File.OpenRead(path))
            using (var f = new StreamReader(s))
            {
                return JsonConvert.DeserializeObject<T>(f.ReadToEnd(), Settings);
            }
        }

        public static void SaveTo<T>(T obj, string path) where T : Template
        {
            var objString = JsonConvert.SerializeObject(obj, Settings);
            File.WriteAllText(path, objString);
        }

        public static T Load<T>() where T : class, new()
        {
            try
            {
                var obj = new T();

                if (obj is ServerConstants)
                {
                    var StoragePath = $@"{ServerContext.STORAGE_PATH}\darkages_config";
                    var path = Path.Combine(StoragePath, string.Format("{0}.json", "global"));

                    if (!File.Exists(path))
                        return null;

                    using (var s = File.OpenRead(path))
                    using (var f = new StreamReader(s))
                    {
                        return JsonConvert.DeserializeObject<T>(f.ReadToEnd(), Settings);
                    }
                }

                if (obj is ObjectService)
                {
                    var StoragePath = $@"{ServerContext.STORAGE_PATH}\states";
                    var path = Path.Combine(StoragePath, string.Format("{0}.json", "state_objcache"));

                    if (!File.Exists(path))
                        return null;

                    using (var s = File.OpenRead(path))
                    using (var f = new StreamReader(s))
                    {
                        return JsonConvert.DeserializeObject<T>(f.ReadToEnd(), Settings);
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static string Save<T>(T obj)
        {
            try
            {
                if (obj is ServerConstants)
                {
                    var StoragePath = $@"{ServerContext.STORAGE_PATH}\darkages_config";

                    if (!Directory.Exists(StoragePath))
                        Directory.CreateDirectory(StoragePath);

                    var path = Path.Combine(StoragePath, string.Format("{0}.json", "global"));
                    var objString = JsonConvert.SerializeObject(obj, Settings);

                    File.WriteAllText(path, objString);
                    return objString;
                }

                if (obj is ObjectService)
                {
                    var StoragePath = $@"{ServerContext.STORAGE_PATH}\states";

                    if (!Directory.Exists(StoragePath))
                        Directory.CreateDirectory(StoragePath);

                    var path = Path.Combine(StoragePath, string.Format("{0}.json", "state_objcache"));
                    var objString = JsonConvert.SerializeObject(obj, Settings);

                    File.WriteAllText(path, objString);
                    return objString;
                }


                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}