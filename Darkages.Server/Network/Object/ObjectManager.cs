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

        public void DelObject<T>(T obj) where T : Sprite => ServerContext.Game.ObjectFactory.RemoveGameObject(obj);
        public void DelObjects<T>(T[] obj) where T : Sprite => ServerContext.Game.ObjectFactory.RemoveAllGameObjects(obj);
        public T GetObject<T>(Predicate<T> p) where T : Sprite => ServerContext.Game.ObjectFactory.Query(p);
        public T[] GetObjects<T>(Predicate<T> p) where T : Sprite => ServerContext.Game.ObjectFactory.QueryAll(p);

        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source, StorageManager.Settings);
            return JsonConvert.DeserializeObject<T>(serialized, StorageManager.Settings);
        }

        public void AddObject<T>(T obj, Predicate<T> p = null)
            where T : Sprite
        {
            if (p != null && p(obj))
            {
                ServerContext.Game.ObjectFactory.AddGameObject(obj);
                return;
            }

            ServerContext.Game.ObjectFactory.AddGameObject(obj);
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