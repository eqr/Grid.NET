using System.Collections.Generic;

namespace Grid.Cache.StoringItems
{
    public interface IItemsHolder<T> : IEnumerable<T>
    {
        void Add(params T[] items);

        void Remove(params T[] item);
    }
}
