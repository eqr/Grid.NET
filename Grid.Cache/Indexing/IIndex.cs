using System.Collections.Generic;

namespace Grid.Cache.Indexing
{
    internal interface IIndex<T, P>
    {
        void Build();
        void AddToIndex(object p, object t);
        void RemoveFromIndex(object t);
        IEnumerable<T> Lookup(P value);
    }
}