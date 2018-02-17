using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Grid.Cache.Utils;

namespace Grid.Cache.Indexing
{
    internal class IndexStorage<T>
    {
        private Dictionary<Type, Dictionary<PropertyInfo, List<IndexDescriptor>>> appliedIndexes =
            new Dictionary<Type, Dictionary<PropertyInfo, List<IndexDescriptor>>>();

        public void AddIndexer<P>(Indexer<T, P> indexer, IIndex<T, P> index)
        {
            Expression<Func<T, P>> expression = indexer.GetAccessorExpression();
            var info = ReflectionUtils.GetPropertyFromExpression(expression);
            var indexDescriptor = new IndexDescriptor(indexer, index, typeof(T), typeof(P));

            if (this.appliedIndexes.ContainsKey(typeof(P)) == false)
            {
                this.appliedIndexes[typeof(P)] = new Dictionary<PropertyInfo, List<IndexDescriptor>>();
            }

            if (this.appliedIndexes[typeof(P)].ContainsKey(info) == false)
            {
                this.appliedIndexes[typeof(P)][info] = new List<IndexDescriptor>(1);
            }

            this.appliedIndexes[typeof(P)][info].Add(indexDescriptor);

        }

        public IEnumerable<IIndex<T, P>> GetAppropriateIndex<T, P>(Expression<Func<T, P>> accessor)
        {
            var info = ReflectionUtils.GetPropertyFromExpression(accessor);

            List<IndexDescriptor> indexers;
            Dictionary<PropertyInfo, List<IndexDescriptor>> propertyIndexers;
            if (this.appliedIndexes.TryGetValue(typeof(P), out propertyIndexers))
            {
                if (propertyIndexers.TryGetValue(info, out indexers))
                {
                    return indexers.Select(i => i.Index as IIndex<T, P>);
                }
            }

            throw new ArgumentException($"Missed index: {accessor}", nameof(accessor));
        }

        public void RemoveIndexer<P>(Indexer<T, P> indexer)
        {
            var info = ReflectionUtils.GetPropertyFromExpression(indexer.GetAccessorExpression());

            this.appliedIndexes[typeof(P)].Remove(info);

        }

        public void IndexNewItems(params T[] items)
        {
            this.DoForAllIndexers(indexDescriptor => indexDescriptor.IndexNewItems(items));
        }

        public void RemoveItems(params T[] items)
        {
            this.DoForAllIndexers(indexDescriptor => indexDescriptor.RemoveItems(items));
        }

        private void DoForAllIndexers(Action<IndexDescriptor> action)
        {
            if (action == null) return;

            foreach (var keyValuePair in this.appliedIndexes)
            {
                foreach (KeyValuePair<PropertyInfo, List<IndexDescriptor>> valuePair in keyValuePair.Value)
                {
                    foreach (var indexDescriptor in valuePair.Value)
                    {
                        action(indexDescriptor);
                    }
                }
            }
        }

        private class IndexDescriptor
        {
            public object Index { get; }
            private object Indexer { get; }
            private Type T { get; }
            private Type P { get; }
            private readonly Lazy<Tuple<object, MethodInfo>> indexFunctionInfo;
            private readonly Lazy<MethodInfo> addToIndexFunctionInfo;
            private readonly Lazy<MethodInfo> removeFromIndexFunctionInfo;

            public IndexDescriptor(object indexer, object index, Type t, Type p)
            {
                this.Indexer = indexer;
                this.Index = index;
                this.T = t;
                this.P = p;

                this.indexFunctionInfo = new Lazy<Tuple<object, MethodInfo>>(() =>
                {
                    Type indexerType = typeof(Indexer<,>);
                    Type constructed = indexerType.MakeGenericType(this.T, this.P);
                    string methodName =
                        ReflectionUtils.GetMethodName<Indexer<object, object>>(i => i.GetAccessorFunction());
                    object func = constructed.GetMethod(methodName).Invoke(this.Indexer, new object[] { });
                    MethodInfo invoke = func.GetType().GetMethod("Invoke");
                    return Tuple.Create(func, invoke);
                });

                this.addToIndexFunctionInfo = new Lazy<MethodInfo>(() =>
                {
                    var methodName =
                        ReflectionUtils.GetMethodName<IIndex<object, object>>(i => i.AddToIndex(null, null));
                    return this.GetIndexFunctionInfo(methodName);
                });

                this.removeFromIndexFunctionInfo = new Lazy<MethodInfo>(() =>
                {
                    var methodName =
                        ReflectionUtils.GetMethodName<IIndex<object, object>>(i => i.RemoveFromIndex(null));
                    return this.GetIndexFunctionInfo(methodName);
                });
            }

            public void IndexNewItems<T>(T[] items)
            {
                foreach (var item in items)
                {
                    object result = this.IndexNewItem(item);
                    this.AddToIndex(result, item);

                }
            }

            public void RemoveItems<T>(T[] items)
            {
                foreach (var item in items)
                {
                    this.RemoveFromIndex(item);
                }
            }

            private void RemoveFromIndex(object item)
            {
                this.removeFromIndexFunctionInfo.Value.Invoke(this.Index, new object[] {item});
            }

            private void AddToIndex(object result, object item)
            {
                this.addToIndexFunctionInfo.Value.Invoke(this.Index,
                    new object[] {result, item});
            }

            private object IndexNewItem<T>(T item)
            {
                return this.indexFunctionInfo.Value.Item2.Invoke(this.indexFunctionInfo.Value.Item1,
                    new object[] {item});
            }

            private MethodInfo GetIndexFunctionInfo(string methodName)
            {
                Type indexType = typeof(IIndex<,>);
                Type constructed = indexType.MakeGenericType(this.T, this.P);
                MethodInfo removeFromIndex = constructed.GetMethod(methodName);
                return removeFromIndex;
            }
        }
    }
}