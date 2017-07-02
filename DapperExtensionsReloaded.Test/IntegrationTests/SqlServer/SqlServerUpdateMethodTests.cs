using System;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerUpdateMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public void UsingKey_UpdatesEntity()
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
            p2.FirstName = "Baz";
            p2.Active = false;

            DapperExtensions.Update(Connection, p2);

            var p3 = DapperExtensions.Get<Person>(Connection, id);
            Assert.Equal("Baz", p3.FirstName);
            Assert.Equal("Bar", p3.LastName);
            Assert.False(p3.Active);
        }
    }
}