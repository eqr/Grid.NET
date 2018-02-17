using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Grid.Cache.Indexing;

namespace Grid.Cache
{
    using Grid.Cache.StoringItems;

    public interface ICache<T>
    {
        IEnumerable<T> Get<P>(Expression<Func<T, P>> accessor, P value);
        void AddIndexer<P>(Indexer<T,P> indexer);
        void RemoveIndexer<P>(Indexer<T,P> indexer);

        IItemsHolder<T> GetItemsHolder();
        void RemoveItems(params T[] items);
        void AddItems(params T[] items);

        List<T> ToList();
    }
}