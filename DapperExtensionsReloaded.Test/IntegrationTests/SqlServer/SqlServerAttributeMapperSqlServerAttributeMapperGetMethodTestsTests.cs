using System;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerAttributeMapperSqlServerAttributeMapperGetMethodTestsTests : SqlServerBaseFixture
    {
        [Fact]
        public void UsingKey_ReturnsEntity()
        {
            var p1 = new FourLeggedFurryAnimal
            {
                Active = true,
                HowItsCalled = "Foo",
                DateCreated = DateTime.UtcNow
            };
            int id = DapperExtensions.Insert(Connection, p1);

            var p2 = DapperExtensions.Get<FourLeggedFurryAnimal>(Connection, id);
            Assert.Equal(id, p2.Id);
            Assert.Equal("Foo", p2.HowItsCalled);
        }
    }
}