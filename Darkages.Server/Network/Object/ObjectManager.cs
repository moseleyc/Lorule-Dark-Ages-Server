using System;
using System.Collections;
using System.Linq;
using Darkages.Storage;
using Darkages.Types;
using Newtonsoft.Json;

namespace Darkages.Network.Object
{
    public class ObjectManager
    {
        [Flags]
        public enum Get
        {
            Aislings = 1,
            Monsters = 2,
            Mundanes = 4,
            Items = 8,
            Money = 16,
            All = Aislings | Items | Money | Monsters | Mundanes
        }


        public void OnAdded(ObjectEvent<Sprite> p)
        {
            ObjectService.Context.ObjectAdded += p;
        }

        public void OnRemoved(ObjectEvent<Sprite> p)
        {
            ObjectService.Context.ObjectRemoved += p;
        }

        public void OnUpdated(ObjectEvent<Sprite> p)
        {
            ObjectService.Context.ObjectChanged += p;
        }

        public void DelObject<T>(T obj) where T : Sprite
        {
            ObjectService.Context.Remove(obj);
        }

        public void DelObjects<T>(T[] obj) where T : Sprite
        {
            ObjectService.Context.RemoveAll(obj);
        }

        public void SaveObject<T>(T obj) where T : Sprite, new()
        {
            ObjectService.Context.Save(obj, i => i.Serial == obj.Serial);
        }

        public T GetObject<T>(Predicate<T> p) where T : Sprite, new()
        {
            return ObjectService.Context.Query(p);
        }

        public T[] GetObjects<T>(Predicate<T> p) where T : Sprite, new()
        {
            return ObjectService.Context.QueryAll(p);
        }

        public void Flush()
        {
            ObjectService.Context?.Dispose();
        }

        public void Cache()
        {
            ObjectService.Context?.Cache();
        }

        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source, StorageManager.Settings);
            return JsonConvert.DeserializeObject<T>(serialized, StorageManager.Settings);
        }

        public void AddObject<T>(T obj, Predicate<T> p = null)
            where T : Sprite
        {
            if (p != null && p(obj))
                ObjectService.Context.Insert(obj);
            else
                ObjectService.Context.Insert(obj);
        }

        public Sprite[] GetObjects(Predicate<Sprite> p, Get selections)
        {
            var bucket = new ArrayList();

            if ((selections & Get.All) == Get.All)
                selections = Get.Items | Get.Money | Get.Monsters | Get.Mundanes | Get.Aislings;

            if ((selections & Get.Aislings) == Get.Aislings)
                bucket.AddRange(GetObjects<Aisling>(p));
            if ((selections & Get.Monsters) == Get.Monsters)
                bucket.AddRange(GetObjects<Monster>(p));
            if ((selections & Get.Mundanes) == Get.Mundanes)
                bucket.AddRange(GetObjects<Mundane>(p));
            if ((selections & Get.Money) == Get.Money)
                bucket.AddRange(GetObjects<Money>(p));
            if ((selections & Get.Items) == Get.Items)
                bucket.AddRange(GetObjects<Item>(p));

            return bucket.Cast<Sprite>().ToArray();
        }

        public Sprite GetObject(Predicate<Sprite> p, Get selections)
        {
            var bucket = new ArrayList();

            if ((selections & Get.All) == Get.All)
                selections = Get.Items | Get.Money | Get.Monsters | Get.Mundanes | Get.Aislings;

            if ((selections & Get.Aislings) == Get.Aislings)
                bucket.AddRange(GetObjects<Aisling>(p));

            if ((selections & Get.Monsters) == Get.Monsters)
                bucket.AddRange(GetObjects<Monster>(p));

            if ((selections & Get.Mundanes) == Get.Mundanes)
                bucket.AddRange(GetObjects<Mundane>(p));

            if ((selections & Get.Money) == Get.Money)
                bucket.AddRange(GetObjects<Money>(p));

            if ((selections & Get.Items) == Get.Items)
                bucket.AddRange(GetObjects<Item>(p));

            return bucket.Cast<Sprite>().FirstOrDefault();
        }
    }
}