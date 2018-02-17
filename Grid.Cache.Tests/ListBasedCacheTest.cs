using System.Collections.Generic;
using Grid.Cache.Facade;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Grid.Cache.Tests
{
    [TestClass]
    public class ListBasedCacheTest
    {
        [TestMethod]
        public void EmptyCacheCanBeCreated()
        {
            var cache = CacheFactory.CreateCache(new List<string>());
            Assert.IsNotNull(cache);
            CollectionAssert.AreEquivalent(cache.ToList(), new List<string>());
        }

        [TestMethod]
        public void NonEmptyCacheCanBeCreated()
        {
            var cache = CacheFactory.CreateCache(new List<string>
            {
                "first",
                "second",
                "third"
            });
            
            Assert.IsNotNull(cache);
            var expected = new List<string>
            {
                "first",
                "second",
                "third"
            };
            
            CollectionAssert.AreEquivalent(cache.ToList(), expected);
        }

        [TestMethod]
        public void ItemsCanBeAddedToCache()
        {
            var cache = CacheFactory.CreateCache(new List<string>
            {
                "first",
                "second"
            });
            
            cache.AddItems("third", "fourth");
            var expected = new List<string>
            {
                "first",
                "second",
                "third",
                "fourth"
            };
            
            CollectionAssert.AreEquivalent(cache.ToList(), expected);
        }

        [TestMethod]
        public void ItemsCanBeRemovedFromCache()
        {
            var cache = CacheFactory.CreateCache(new List<string>
            {
                "first",
                "second",
                "third",
                "fourth"
            });
            
            cache.RemoveItems("first", "fourth");
            var expected = new List<string>
            {
                "second",
                "third"
            };
            
            CollectionAssert.AreEquivalent(cache.ToList(), expected);
        }
    }
}
