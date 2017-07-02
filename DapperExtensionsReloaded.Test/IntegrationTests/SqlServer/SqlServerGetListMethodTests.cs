using System;
using System.Linq;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerGetListMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public void UsingNullPredicate_ReturnsAll()
        {
            DapperExtensions.Insert(Connection, new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow });

            var list = DapperExtensions.GetList<Person>(Connection);
            Assert.Equal(4, list.Count());
        }

        [Fact]
        public void UsingPredicate_ReturnsMatching()
        {
            DapperExtensions.Insert(Connection, new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow });

            var predicate = Predicates.Predicates.Field<Person>(f => f.Active, Operator.Eq, true);
            var list = DapperExtensions.GetList<Person>(Connection, predicate, null);
            Assert.Equal(2, list.Count());
            Assert.True(list.All(p => p.FirstName == "a" || p.FirstName == "c"));
        }

        [Fact]
        public void UsingObject_ReturnsMatching()
        {
            DapperExtensions.Insert(Connection, new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow });

            var predicate = new { Active = true, FirstName = "c" };
            var list = DapperExtensions.GetList<Person>(Connection, predicate, null);
            Assert.Equal(1, list.Count());
            Assert.True(list.All(p => p.FirstName == "c"));
        }
    }
}