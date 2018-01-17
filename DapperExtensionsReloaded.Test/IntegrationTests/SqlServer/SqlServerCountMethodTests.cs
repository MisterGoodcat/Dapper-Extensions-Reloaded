using System;
using System.Threading.Tasks;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerCountMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public async Task UsingNullPredicate_Returns_Count()
        {
            await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow.AddDays(-10) });
            await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow.AddDays(-10) });
            await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow.AddDays(-3) });
            await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow.AddDays(-1) });

            var count = await DapperExtensions.CountAsync<Person>(Connection, null);
            Assert.Equal(4, count);
        }

        [Fact]
        public async Task UsingPredicate_Returns_Count()
        {
            await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow.AddDays(-10) });
            await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow.AddDays(-10) });
            await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow.AddDays(-3) });
            await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow.AddDays(-1) });

            var predicate = Predicates.Predicates.Field<Person>(f => f.DateCreated, Operator.Lt, DateTime.UtcNow.AddDays(-5));
            var count = await DapperExtensions.CountAsync<Person>(Connection, predicate);
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task UsingEqualsPredicate_Returns_Count()
        {
            await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow.AddDays(-10) });
            await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow.AddDays(-10) });
            await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow.AddDays(-3) });
            await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow.AddDays(-1) });

            var predicate = Predicates.Predicates.Field<Person>(x => x.FirstName, Operator.Eq, new[] { "b", "d" });
            var count = await DapperExtensions.CountAsync<Person>(Connection, predicate);
            Assert.Equal(2, count);
        }
    }
}