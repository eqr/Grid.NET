using System;
using System.Collections.Generic;
using System.Diagnostics;
using Grid.Cache;
using Grid.Cache.Facade;
using Grid.Cache.Indexing;

namespace Grid.Playground
{
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            var sw = Stopwatch.StartNew();
            sw.Start();
            int dataSize = 2 * 1000 * 1000;
            var testData = PrepareTestData(dataSize);
            int sampleSize = 2000;
            var sampleQueries = new List<Item>(sampleSize);
            var random = new Random();
            for (int i = 0; i < sampleSize; i++)
            {
                sampleQueries.Add(testData[random.Next(dataSize)]);
            }
            
            sw.Stop();
            Console.WriteLine($"Preparing data took {sw.Elapsed}");
            sw.Reset();
            
            sw.Start();
            foreach (var sampleQuery in sampleQueries)
            {
                var r = testData.First(i => i.Equals(sampleQuery));
            }
            
            sw.Stop();
            Console.WriteLine($"Plain old search {sampleSize} from {dataSize}: {sw.Elapsed}");
            sw.Reset();

            Dictionary<string, Item> dict =
                testData.ToDictionary(d => d.Value, d => d, StringComparer.OrdinalIgnoreCase);
            sw.Start();
            foreach (var sampleQuery in sampleQueries)
            {
                var r = dict[sampleQuery.Value];
            }

            sw.Stop();
            Console.WriteLine($"Searching in dictionary took {sw.Elapsed}");
            sw.Reset();

            sw.Start();
            var cache = CacheFactory.CreateCache(testData);
            sw.Stop();
            Console.WriteLine($"Creating cache took {sw.Elapsed}");
            sw.Reset();
            
            sw.Start();
            cache.AddIndexer(new Indexer<Item, string>(i => i.Value));
            sw.Stop();
            Console.WriteLine($"Indexing took {sw.Elapsed}");
            sw.Reset();
            
            sw.Start();
            foreach (var sampleQuery in sampleQueries)
            {
                var r = cache.Get(i => i.Value, sampleQuery.Value);
            }
            sw.Stop();
            Console.WriteLine($"Lookup in cache took {sw.Elapsed}");
        }

        private static List<Item> PrepareTestData(int dataSize)
        {
            int count = dataSize;
            HashSet<Item> result = new HashSet<Item>(count);
            while(result.Count < dataSize)
            {
                result.Add(new Item(Guid.NewGuid().ToString()));
            }

            return result.ToList();
        }
    }

    class Item
    {
        public Item(string value)
        {
            this.Value = value;
        }
        
        public string Value { get; }

        protected bool Equals(Item other)
        {
            return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Item) obj);
        }

        public override int GetHashCode()
        {
            return (this.Value != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(this.Value) : 0);
        }
    }
}
