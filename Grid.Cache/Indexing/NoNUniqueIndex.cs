using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grid.Cache.StoringItems;

namespace Grid.Cache.Indexing
{
    internal class NonUniqueIndex<T, P> : IIndex<T, P>
    {
        private IEnumerable<T> holder;
        private Indexer<T, P> indexer;
        private ConcurrentDictionary<P, ConcurrentBag<T>> index;

        public NonUniqueIndex(IItemsHolder<T> holder, Indexer<T, P> indexer)
        {
            this.holder = holder;
            this.indexer = indexer;
        }

        public void Build()
        {
            var index = new ConcurrentDictionary<P, ConcurrentBag<T>>();
            Parallel.ForEach(this.holder, e =>
            {
                try
                {
                    P value = this.indexer.Index(e);
                    this.AddToIndex(value, e, index);
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $"Error while indexing element {e.ToString()} with indexer {this.indexer.ToString()}", ex);
                }
            });
            
            this.index = index;
        }
    
        private void AddToIndex(P value, T e, ConcurrentDictionary<P, ConcurrentBag<T>> index)
        {
            index.AddOrUpdate(value, new ConcurrentBag<T>() {e}, (v, a) =>
            {
                index[v].Add(e);
                return index[v];
            });
        }

        public void AddToIndex(object indexedValue, object item)
        {
            this.AddToIndex((P) indexedValue, (T) item, this.index);
        }

        public void RemoveFromIndex(object t)
        {
            T item = (T)t;
            List<P> keysToRemove = new List<P>();
            foreach (var keyValuePair in this.index)
            {
                if (keyValuePair.Value.Contains(item))
                {
                    var newItems = keyValuePair.Value.ToList();
                    newItems.Remove(item);

                    if (newItems.Any())
                    {
                        this.index[keyValuePair.Key] = new ConcurrentBag<T>(newItems);
                    }
                    else
                    {
                        keysToRemove.Add(keyValuePair.Key);
                        this.index[keyValuePair.Key] = new ConcurrentBag<T>();
                    }
                }
            }
            
            foreach (var p in keysToRemove)
            {
                ConcurrentBag<T> a;
                this.index.TryRemove(p, out a);
            }
        }

        public IEnumerable<T> Lookup(P value)
        {
            ConcurrentBag<T> results;
            if(this.index.TryGetValue(value, out results))
            {
                return results;
            }

            return new List<T>(0);
        }
    }
}