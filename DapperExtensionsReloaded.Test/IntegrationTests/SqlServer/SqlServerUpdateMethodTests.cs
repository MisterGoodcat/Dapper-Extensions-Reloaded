using System;
using System.Threading.Tasks;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerUpdateMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public async Task UsingKey_UpdatesEntity()
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
            p2.FirstName = "Baz";
            p2.Active = false;

            await DapperExtensions.UpdateAsync(Connection, p2);

            var p3 = await DapperExtensions.GetAsync<Person>(Connection, id);
            Assert.Equal("Baz", p3.FirstName);
            Assert.Equal("Bar", p3.LastName);
            Assert.False(p3.Active);
        }
    }
}