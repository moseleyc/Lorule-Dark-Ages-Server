using Darkages.Common;
using Darkages.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Darkages.Network.Object
{
    public class SpriteCollection<T> : IEnumerable<T>, INotifyCollectionChanged
    {
        private readonly ConcurrentList<T> _values;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public SpriteCollection(IEnumerable<T> values) => _values = new ConcurrentList<T>(values);

        public T Query(Predicate<T> predicate)
        {
            return _values.FirstOrDefault(i => predicate(i));
        }

        public T[] QueryAll(Predicate<T> predicate)
        {
            return _values.Where(i => predicate(i)).ToArray();
        }

        public void Add(T obj)
        {
            _values.Add(obj);
            CollectionChanged?
                .Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
        }

        public void Delete(T obj)
        {
            if (_values.Remove(obj))
            {
                CollectionChanged?
                    .Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
            }
        }

        public IEnumerator<T> GetEnumerator() => _values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public sealed class ObjectService
    {
        private readonly IDictionary<Type, object> _spriteCollections = new Dictionary<Type, object>
        {
            [typeof(Aisling)] = new SpriteCollection<Aisling>(Enumerable.Empty<Aisling>()),
            [typeof(Monster)] = new SpriteCollection<Monster>(Enumerable.Empty<Monster>()),
            [typeof(Mundane)] = new SpriteCollection<Mundane>(Enumerable.Empty<Mundane>()),
            [typeof(Money)]   = new SpriteCollection<Money>(Enumerable.Empty<Money>()),
            [typeof(Item)]    = new SpriteCollection<Item>(Enumerable.Empty<Item>()),
        };


        public T Query<T>(Predicate<T> predicate) 
        {
            var obj = (SpriteCollection<T>)_spriteCollections[typeof(T)];
            return obj.Query(predicate);
        }

        public T[] QueryAll<T>(Predicate<T> predicate) 
        {
            var obj = (SpriteCollection<T>)_spriteCollections[typeof(T)];
            return obj.QueryAll(predicate);
        }
      
        public void RemoveAllGameObjects<T>(T[] objects) where T : Sprite
        {
            if (objects == null)
                return;

            for (uint i = 0; i < objects.Length; i++)
                RemoveGameObject(objects[i]);
        }

        public void AddGameObject<T>(T obj) where T : Sprite
        {
            var objCollection = (SpriteCollection<T>)_spriteCollections[typeof(T)];
            objCollection.Add(obj);
        }

        public void RemoveGameObject<T>(T obj) where T : Sprite
        {
            var objCollection = (SpriteCollection<T>)_spriteCollections[typeof(T)];
            objCollection.Delete(obj);
        }             
    }
}