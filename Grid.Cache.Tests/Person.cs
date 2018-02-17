using System;
using System.Collections.Generic;

namespace Grid.Cache.Tests
{
    internal class Person
    {
        public string Name { get; }
        
        public string LastName { get; }
        
        public DateTime BirthDate { get; }

        public Person(string name, string lastName, DateTime birthDate)
        {
            Name = name;
            LastName = lastName;
            BirthDate = birthDate;
        }

        public static List<Person> CreateTestData()
        {
            return new List<Person>()
            {
                new Person("John", "Smith", new DateTime(1970, 1, 1)),
                new Person("Jane", "Smith", new DateTime(1980, 2, 2)),
                new Person("Katie", "Smith", new DateTime(1990, 3, 3)),
                new Person("Jane", "Johnes", new DateTime(2000, 4, 4))
            };
        }
        
        public override string ToString()
        {
            return
                $"{nameof(this.Name)}: {this.Name}, {nameof(this.LastName)}: {this.LastName}, {nameof(this.BirthDate)}: {this.BirthDate}";
        }
    }
}