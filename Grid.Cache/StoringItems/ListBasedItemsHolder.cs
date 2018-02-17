using System;
using System.Collections;
using System.Collections.Generic;

namespace Grid.Cache.StoringItems
{
    internal class ListBasedItemsHolder<T> : IItemsHolder<T>
    {
        private List<T> records;

        public ListBasedItemsHolder()
            : this(0)
        {
        }

        public ListBasedItemsHolder(IEnumerable<T> items)
        {
            this.records = new List<T>(items);
        }

        public ListBasedItemsHolder(int count)
        {
            this.records = new List<T>(count);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.records.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(params T[] items)
        {
            this.records.AddRange(items);
        }

        public void Remove(params T[] items)
        {
            foreach (var item in items)
            {
                this.records.Remove(item);
            }
        }
    }
}