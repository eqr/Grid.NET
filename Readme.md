*Grid.NET* is a caching solution for .NET Standard application that solves the problem of effective search over a collection by different class properties in memory-efficient way.

Example:
-------
```C#
List<Person> persons = GetAllPersons();
var cache = CacheFactory.CreateCache(persons);
cache.AddIndexer(new Indexer<Person, string>(p => p.Name));
// Returns all persons with name Jane
var janes = cache.Get(p => p.Name, "Jane").ToList(); 
cache.AddIndexer(new Indexer<Person, string>(p => p.LastName));
// Returns all persons with last name Smith
var smiths = cache.Get(p => p.LastName, "Smith").ToList(); 
```

User can not query by non-indexed field, ArgumentException is thrown in that case.
```C#
List<Person> persons = GetAllPersons();
var cache = CacheFactory.CreateCache(persons);
// not allowed, the field is not indexed
var bornInThatDay = cache.Get(p => p.BirthDate, new DateTime(1970,1,1)); 
```

Other usecases are expressed in tests.

Plans:
------
* Expose the cache as a web service with WebAPI
* Add more flexibility in indexing
* Find a better options to store the original values, probably off-heap solutions
* Distributed cache
* Document things better, add examples
* Publish a NuGet package
* More complicated queries (e.g. range queries)

Improvements for indexer:
-------------------------
* add option to keep only unique records in the index. This option chooses between List and HashSet in index dictionary
* add option to make indexing parallel or sequential
* for string indexers: case insensitive or not. Different Indexer class?
* option to protect cache entities, they are deep copied on return if it is possible. If no, returned as is. Deep copy can be made e.g. by serializing and deserializing if there are no better approaches.
