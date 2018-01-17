using System;
using System.Linq;
using System.Threading.Tasks;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerDeleteMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public async Task UsingKey_DeletesFromDatabase()
        {
            var p1 = new Person
            {
                Active = true,
                FirstName = "Foo",
                LastName = "Bar",
                DateCreated = DateTime.UtcNow
            };
            int id = await DapperExtensions.InsertAsync(Connection, p1);

            var p2 = await DapperExtensions.GetAsync<Person>(Connection, id);
            await DapperExtensions.DeleteAsync(Connection, p2);
            Assert.Null(await DapperExtensions.GetAsync<Person>(Connection, id));
        }

        [Fact]
        public async Task UsingPredicate_DeletesRows()
        {
            var p1 = new Person { Active = true, FirstName = "Foo", LastName = "Bar", DateCreated = DateTime.UtcNow };
            var p2 = new Person { Active = true, FirstName = "Foo", LastName = "Bar", DateCreated = DateTime.UtcNow };
            var p3 = new Person { Active = true, FirstName = "Foo", LastName = "Barz", DateCreated = DateTime.UtcNow };
            await DapperExtensions.InsertAsync(Connection, p1);
            await DapperExtensions.InsertAsync(Connection, p2);
            await DapperExtensions.InsertAsync(Connection, p3);

            var list = await DapperExtensions.GetListAsync<Person>(Connection);
            Assert.Equal(3, list.Count());

            IPredicate pred = Predicates.Predicates.Field<Person>(p => p.LastName, Operator.Eq, "Bar");
            var result = await DapperExtensions.DeleteAsync<Person>(Connection, pred);
            Assert.True(result);

            list = await DapperExtensions.GetListAsync<Person>(Connection);
            Assert.Single(list);
        }

        [Fact]
        public async Task UsingEqualsPredicate_DeletesRows()
        {
            var p1 = new Person { Active = true, FirstName = "Foo", LastName = "Bar", DateCreated = DateTime.UtcNow };
            var p2 = new Person { Active = true, FirstName = "Foo", LastName = "Bar", DateCreated = DateTime.UtcNow };
            var p3 = new Person { Active = true, FirstName = "Foo", LastName = "Barz", DateCreated = DateTime.UtcNow };
            await DapperExtensions.InsertAsync(Connection, p1);
            await DapperExtensions.InsertAsync(Connection, p2);
            await DapperExtensions.InsertAsync(Connection, p3);

            var list = await DapperExtensions.GetListAsync<Person>(Connection);
            Assert.Equal(3, list.Count());

            var predicate = Predicates.Predicates.Field<Person>(x => x.LastName, Operator.Eq, "Bar");
            var result = await DapperExtensions.DeleteAsync<Person>(Connection, predicate);
            Assert.True(result);

            list = await DapperExtensions.GetListAsync<Person>(Connection);
            Assert.Single(list);
        }
    }
}