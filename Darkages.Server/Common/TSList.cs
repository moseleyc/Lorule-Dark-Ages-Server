using System;
using System.Collections.Generic;
using System.Linq;


namespace Darkages.Common
{
    public class TSList<T>
    {
        private List<T> _list = new List<T>();
        private object _sync = new object();

        public int Length => _list.Count;

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public void Add(T value)
        {
            lock (_sync)
            {
                _list.Add(value);
            }
        }

        public void Remove(T value)
        {
            lock (_sync)
            {
                _list.Remove(value);
            }
        }

        public void RemoveAt(int index)
        {
            var item = this[index];
            Remove(item);
        }

        public T Find(Predicate<T> predicate)
        {
            lock (_sync)
            {
                return _list.FirstOrDefault(i => predicate(i));
            }
        }
        public T FirstOrDefault()
        {
            lock (_sync)
            {
                return _list.FirstOrDefault();
            }
        }
    }
}
