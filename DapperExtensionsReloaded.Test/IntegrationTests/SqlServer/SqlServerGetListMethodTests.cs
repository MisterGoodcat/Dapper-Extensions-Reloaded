using System;
using System.Linq;
using System.Threading.Tasks;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerGetListMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public async Task UsingNullPredicate_ReturnsAll()
        {
            await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow });

            var list = await DapperExtensions.GetListAsync<Person>(Connection);
            Assert.Equal(4, list.Count());
        }

        [Fact]
        public async Task UsingPredicate_ReturnsMatching()
        {
            await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow });

            var predicate = Predicates.Predicates.Field<Person>(f => f.Active, Operator.Eq, true);
            var list = await DapperExtensions.GetListAsync<Person>(Connection, predicate, null);
            Assert.Equal(2, list.Count());
            Assert.True(list.All(p => p.FirstName == "a" || p.FirstName == "c"));
        }

        [Fact]
        public async Task UsingEqualsPredicate_ReturnsMatching()
        {
            await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow });

            var predicate = Predicates.Predicates.Group(
                GroupOperator.And,
                Predicates.Predicates.Field<Person>(x => x.Active, Operator.Eq, true),
                Predicates.Predicates.Field<Person>(x => x.FirstName, Operator.Eq, "c"));
            var list = await DapperExtensions.GetListAsync<Person>(Connection, predicate);
            Assert.Single(list);
            Assert.True(list.All(p => p.FirstName == "c"));
        }
    }
}