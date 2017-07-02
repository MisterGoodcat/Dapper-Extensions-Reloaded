using System;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerGetMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public void UsingKey_ReturnsEntity()
        {
            var p1 = new Person
            {
                Active = true,
                FirstName = "Foo",
                LastName = "Bar",
                DateCreated = DateTime.UtcNow
            };
            int id = DapperExtensions.Insert(Connection, p1);

            var p2 = DapperExtensions.Get<Person>(Connection, id);
            Assert.Equal(id, p2.Id);
            Assert.Equal("Foo", p2.FirstName);
            Assert.Equal("Bar", p2.LastName);
        }
    }
}