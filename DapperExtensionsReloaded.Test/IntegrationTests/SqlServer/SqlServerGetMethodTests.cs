using System;
using System.Threading.Tasks;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerGetMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public async Task UsingKey_ReturnsEntity()
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
            Assert.Equal(id, p2.Id);
            Assert.Equal("Foo", p2.FirstName);
            Assert.Equal("Bar", p2.LastName);
        }

        [Fact]
        public async Task UsingPredicate_ReturnsEntity()
        {
            var p1 = new Person
            {
                Active = true,
                FirstName = "Foo",
                LastName = "Bar",
                DateCreated = DateTime.UtcNow
            };
            int id = await DapperExtensions.InsertAsync(Connection, p1);

            var predicate = Predicates.Predicates.Field<Person>(x => x.Id, Operator.Eq, id);
            var p2 = await DapperExtensions.GetAsync<Person>(Connection, predicate);
            Assert.Equal(id, p2.Id);
            Assert.Equal("Foo", p2.FirstName);
            Assert.Equal("Bar", p2.LastName);
        }
    }
}