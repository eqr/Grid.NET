using System;
using System.Linq;
using Grid.Cache.Facade;
using Grid.Cache.Indexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Grid.Cache.Tests
{
    [TestClass]
    public class NonUniqueIndexingTest
    {
        [TestMethod]
        public void ItemIsReturnedIfIndexed()
        {
            var cache = CacheFactory.CreateCache(Person.CreateTestData());
            cache.AddIndexer(new Indexer<Person, string>(p => p.Name));
            var janes = cache.Get(p => p.Name, "Jane").ToList();

            Assert.AreEqual(2, janes.Count);
            Assert.AreEqual(1, janes.Count(j => j.Name == "Jane" && j.LastName == "Smith"));
            Assert.AreEqual(1, janes.Count(j => j.Name == "Jane" && j.LastName == "Johnes"));
        }

        [TestMethod]
        public void ExceptionIsThrownIfPropertyIsNotIndexed()
        {
            var cache = CacheFactory.CreateCache(Person.CreateTestData());
            Assert.ThrowsException<ArgumentException>(() => { cache.Get(p => p.Name, "Jane"); });
        }

        [TestMethod]
        public void ExceptionIsThrownIfIndexerIsRemovedFromProperty()
        {
            var cache = CacheFactory.CreateCache(Person.CreateTestData());
            cache.AddIndexer(new Indexer<Person, string>(p => p.Name));
            cache.RemoveIndexer(new Indexer<Person, string>(p => p.Name));
            Assert.ThrowsException<ArgumentException>(() => { cache.Get(p => p.Name, "Jane"); });
        }

        [TestMethod]
        public void ItemAddedToHolderIsIndexed()
        {
            var cache = CacheFactory.CreateCache(Person.CreateTestData());
            cache.AddIndexer(new Indexer<Person, string>(p => p.Name));
            cache.AddItems(new Person("Jane", "Dow", new DateTime()), new Person("Jill", "Jungle", new DateTime()));
            
            var janes = cache.Get(p => p.Name, "Jane").ToList();
            Assert.AreEqual(3, janes.Count);
            Assert.AreEqual(1, janes.Count(j => j.Name == "Jane" && j.LastName == "Smith"));
            Assert.AreEqual(1, janes.Count(j => j.Name == "Jane" && j.LastName == "Johnes"));
            Assert.AreEqual(1, janes.Count(j => j.Name == "Jane" && j.LastName == "Dow"));
        }

        [TestMethod]
        public void ItemsRemovedFromHolderAreIndexed()
        {
            var cache = CacheFactory.CreateCache(Person.CreateTestData());
            cache.AddIndexer(new Indexer<Person, string>(p => p.Name));
            Person janeJohnes = Person.CreateTestData().Single(p => p.Name == "Jane" && p.LastName == "Johnes");
            
            cache.RemoveItems(janeJohnes);
            var janes = cache.Get(p => p.Name, "Jane").ToList();
            Assert.AreEqual(1, janes.Count(j => j.Name == "Jane" && j.LastName == "Smith"));
        }
    }
}