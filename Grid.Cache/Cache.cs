using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Grid.Cache.Indexing;
using Grid.Cache.StoringItems;

namespace Grid.Cache
{
    public class Cache<T> : ICache<T>
    {
        private IItemsHolder<T> holder;
        
        private IndexStorage<T> indexStorage = new IndexStorage<T>();

        private ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
        
        internal Cache(IItemsHolder<T> holder)
        {
            this.holder = holder;
        }

        public void AddItems(params T[] items)
        {
            this.rwLock.EnterWriteLock();
            try
            {
                this.holder.Add(items);
                this.indexStorage.IndexNewItems(items);
            }
            finally
            {
                this.rwLock.ExitWriteLock();
            }
        }

        public void RemoveItems(params T[] items)
        {
            this.rwLock.EnterWriteLock();
            try
            {
                this.holder.Remove(items);
                this.indexStorage.RemoveItems(items);
            }
            finally
            {
                this.rwLock.ExitWriteLock();
            }
        }

        public List<T> ToList()
        {
            this.rwLock.EnterReadLock();
            try
            {
                return new List<T>(this.holder);
            }
            finally
            {
                this.rwLock.ExitReadLock();
            }
        }

        public IEnumerable<T> Get<P>(Expression<Func<T, P>> accessor, P value)
        {
            this.rwLock.EnterReadLock();
            try
            {
                var index = this.indexStorage.GetAppropriateIndex<T, P>(accessor);
                return index.Single().Lookup(value);
            }
            finally
            {
                this.rwLock.ExitReadLock();
            }
        }

        public void AddIndexer<P>(Indexer<T,P> indexer)
        {
            var index = new NonUniqueIndex<T,P>(this.holder, indexer);
            index.Build();
            
            this.rwLock.EnterWriteLock();
            try
            {
                this.indexStorage.AddIndexer(indexer, index);
            }
            finally
            {
                this.rwLock.ExitWriteLock();
            }
        }

        public void RemoveIndexer<P>(Indexer<T,P> indexer)
        {
            this.rwLock.EnterWriteLock();
            try
            {
                this.indexStorage.RemoveIndexer(indexer);
            }
            finally
            {
                this.rwLock.ExitWriteLock();
            }
        }
        
        // NB! Not guaranteed to be thread-safe, for testing only
        public IItemsHolder<T> GetItemsHolder()
        {
            return this.holder;
        }
    }
}
