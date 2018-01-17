using System;
using System.Threading.Tasks;
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
    }
}