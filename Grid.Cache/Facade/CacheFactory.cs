using System.Collections.Generic;
using Grid.Cache.StoringItems;

namespace Grid.Cache.Facade
{
    public class CacheFactory
    {
        public static ICache<T> CreateCache<T>(IEnumerable<T> items)
        {
            var holder = new ListBasedItemsHolder<T>(items);
            return new Cache<T>(holder);
        }
    }
}