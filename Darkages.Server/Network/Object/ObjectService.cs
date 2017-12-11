using Darkages.Storage;
using Darkages.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Common;

namespace Darkages.Network.Object
{
    public sealed class ObjectService : IDisposable
    {
        public event ObjectEvent<Sprite> ObjectAdded = null;
        public event ObjectEvent<Sprite> ObjectChanged = null;
        public event ObjectEvent<Sprite> ObjectRemoved = null;



        [JsonIgnore]
        private static object syncLock = new object();
        [JsonIgnore]
        private static ObjectService context = null;

        [JsonIgnore]
        private static bool CacheLoaded { get; set; }


        [JsonIgnore]
        private HashSet<Sprite> _aislings = new HashSet<Sprite>();
        [JsonIgnore]
        private HashSet<Sprite> Aislings
        {
            get
            {
                lock (syncLock)
                {
                    return new HashSet<Sprite>(_aislings);
                }
            }
        }

        [JsonIgnore]
        private HashSet<Sprite> _monsters = new HashSet<Sprite>();
        [JsonIgnore]
        private HashSet<Sprite> Monsters
        {
            get
            {
                lock (syncLock)
                {
                    return new HashSet<Sprite>(_monsters);
                }
            }
        }

        [JsonIgnore]
        private HashSet<Sprite> _mundanes = new HashSet<Sprite>();
        [JsonIgnore]
        private HashSet<Sprite> Mundanes
        {
            get
            {
                lock (syncLock)
                {
                    return new HashSet<Sprite>(_mundanes);
                }
            }
        }

        [JsonProperty]
        private HashSet<Sprite> _money = new HashSet<Sprite>();
        [JsonIgnore]
        private HashSet<Sprite> Money
        {
            get
            {
                lock (syncLock)
                {
                    return new HashSet<Sprite>(_money);
                }
            }
        }

        [JsonProperty]
        private HashSet<Sprite> _items = new HashSet<Sprite>();
        [JsonIgnore]
        private HashSet<Sprite> Items
        {
            get
            {
                lock (syncLock)
                {
                    return new HashSet<Sprite>(_items);
                }
            }
        }


        public T Query<T>(Predicate<T> predicate) where T : Sprite, new()
        {
            T obj = new T();


            if (obj is Aisling)
            {
                if (Aislings == null)
                    return null;

                return Aislings.Cast<T>().FirstOrDefault(i => predicate(i));
            }

            if (obj is Monster)
            {
                if (Monsters == null)
                    return null;

                return Monsters.Cast<T>().FirstOrDefault(i => predicate(i));
            }

            if (obj is Mundane)
            {
                if (Mundanes == null)
                    return null;

                return Mundanes.Cast<T>().FirstOrDefault(i => predicate(i));
            }

            if (obj is Money)
            {
                if (Money == null)
                    return null;

                return Money.Cast<T>().FirstOrDefault(i => predicate(i));
            }

            if (obj is Item)
            {
                if (Items == null)
                    return null;

                return Items.Cast<T>().FirstOrDefault(i => predicate(i));
            }


            return null;
        }

        public T[] QueryAll<T>(Predicate<T> predicate) where T : Sprite, new()
        {
            T obj = new T();

            if (obj is Aisling)
            {
                if (Aislings == null)
                    return null;

                return Aislings.Cast<T>().Where(i => predicate(i)).ToArray();
            }

            if (obj is Monster)
            {
                if (Monsters == null)
                    return null;

                return Monsters.Cast<T>().Where(i => predicate(i)).ToArray();
            }

            if (obj is Mundane)
            {
                if (Mundanes == null)
                    return null;

                return Mundanes.Cast<T>().Where(i => predicate(i)).ToArray();
            }

            if (obj is Money)
            {
                if (Money == null)
                    return null;

                return Money.Cast<T>().Where(i => predicate(i)).ToArray();
            }

            if (obj is Item)
            {
                if (Items == null)
                    return null;

                return Items.Cast<T>().Where(i => predicate(i)).ToArray();
            }


            return null;
        }

        public void Save<T>(T reference, Predicate<T> predicate) where T : Sprite, new()
        {
            if (reference == null)
                return;

            var obj = Query<T>(predicate);
            obj = (T)reference;

            ObjectChanged?.Invoke(obj);
        }

        public void Insert<T>(T obj) where T : Sprite
        {
            if (obj == null)
                return;

            lock (Generator.Random)
            {
                obj.Serial = Generator.GenerateNumber();
            }

            lock (syncLock)
            {
                if (obj is Aisling)
                {
                    _aislings.Add(obj);
                }

                if (obj is Monster)
                {
                    _monsters.Add(obj);
                }

                if (obj is Mundane)
                {
                    _mundanes.Add(obj);
                }

                if (obj is Money)
                {
                    _money.Add(obj);
                }

                if (obj is Item)
                {
                    _items.Add(obj);
                }

                ObjectAdded?.Invoke(obj);
            }
        }

        public void RemoveAll<T>(T[] objects) where T : Sprite
        {
            if (objects == null)
                return;

            for (uint i = 0; i < objects.Length; i++)
                Remove<T>(objects[i]);
        }


        public void Remove<T>(T obj) where T : Sprite
        {
            if (obj == null)
                return;

            lock (syncLock)
            {
                if (obj is Aisling)
                {
                    _aislings.Remove(obj);
                }

                if (obj is Monster)
                {
                    _monsters.Remove(obj);
                }

                if (obj is Mundane)
                {
                    _mundanes.Remove(obj);
                }

                if (obj is Money)
                {
                    _money.Remove(obj);
                }

                if (obj is Item)
                {
                    _items.Remove(obj);
                }

                ObjectRemoved?.Invoke(obj);
            }
        }

        [JsonIgnore]
        public static ObjectService Context
        {
            get
            {
                if (ObjectService.context == null)
                    ObjectService.context = new ObjectService();

                return ObjectService.context;
            }
        }

        private bool disposedValue = false; // To detect redundant calls

        public void Cache()
        {
            StorageManager.Save<ObjectService>(this);
        }

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    var removables = Aislings.Concat(Money).Concat(Mundanes).Concat(Monsters).Concat(Items)
                        .Reverse();

                    foreach (var obj in removables)
                        obj.Remove();

                    _items?.Clear();
                    _money?.Clear();
                    _monsters?.Clear();
                    _aislings?.Clear();
                    _mundanes?.Clear();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            disposedValue = !disposedValue;
            Dispose(true);
        }

        internal static void Set(ObjectService cache_)
        {
            if (CacheLoaded)
                return;

            foreach (var obj in cache_.Items)
                Context.Insert(obj);

            foreach (var obj in cache_.Money)
                Context.Insert(obj);

            CacheLoaded = true;
        }
    }
}