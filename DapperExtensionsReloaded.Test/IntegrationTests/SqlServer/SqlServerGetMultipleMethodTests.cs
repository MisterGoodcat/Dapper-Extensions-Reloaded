using System;
using System.Linq;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerGetMultipleMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public void ReturnsItems()
        {
            DapperExtensions.Insert(Connection, new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow.AddDays(-10) });
            DapperExtensions.Insert(Connection, new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow.AddDays(-10) });
            DapperExtensions.Insert(Connection, new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow.AddDays(-3) });
            DapperExtensions.Insert(Connection, new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow.AddDays(-1) });

            DapperExtensions.Insert(Connection, new Animal { Name = "Foo" });
            DapperExtensions.Insert(Connection, new Animal { Name = "Bar" });
            DapperExtensions.Insert(Connection, new Animal { Name = "Baz" });

            var predicate = new GetMultiplePredicate();
            predicate.Add<Person>(null);
            predicate.Add<Animal>(Predicates.Predicates.Field<Animal>(a => a.Name, Operator.Like, "Ba%"));
            predicate.Add<Person>(Predicates.Predicates.Field<Person>(a => a.LastName, Operator.Eq, "c1"));

            var result = DapperExtensions.GetMultiple(Connection, predicate);
            var people = result.Read<Person>().ToList();
            var animals = result.Read<Animal>().ToList();
            var people2 = result.Read<Person>().ToList();

            Assert.Equal(4, people.Count);
            Assert.Equal(2, animals.Count);
            Assert.Single(people2);
        }
    }
}