using System;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerCountMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public void UsingNullPredicate_Returns_Count()
        {
            DapperExtensions.Insert(Connection, new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow.AddDays(-10) });
            DapperExtensions.Insert(Connection, new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow.AddDays(-10) });
            DapperExtensions.Insert(Connection, new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow.AddDays(-3) });
            DapperExtensions.Insert(Connection, new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow.AddDays(-1) });

            var count = DapperExtensions.Count<Person>(Connection, null);
            Assert.Equal(4, count);
        }

        [Fact]
        public void UsingPredicate_Returns_Count()
        {
            DapperExtensions.Insert(Connection, new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow.AddDays(-10) });
            DapperExtensions.Insert(Connection, new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow.AddDays(-10) });
            DapperExtensions.Insert(Connection, new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow.AddDays(-3) });
            DapperExtensions.Insert(Connection, new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow.AddDays(-1) });

            var predicate = Predicates.Predicates.Field<Person>(f => f.DateCreated, Operator.Lt, DateTime.UtcNow.AddDays(-5));
            var count = DapperExtensions.Count<Person>(Connection, predicate);
            Assert.Equal(2, count);
        }

        [Fact]
        public void UsingObject_Returns_Count()
        {
            DapperExtensions.Insert(Connection, new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow.AddDays(-10) });
            DapperExtensions.Insert(Connection, new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow.AddDays(-10) });
            DapperExtensions.Insert(Connection, new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow.AddDays(-3) });
            DapperExtensions.Insert(Connection, new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow.AddDays(-1) });

            var predicate = new { FirstName = new[] { "b", "d" } };
            var count = DapperExtensions.Count<Person>(Connection, predicate);
            Assert.Equal(2, count);
        }
    }
}