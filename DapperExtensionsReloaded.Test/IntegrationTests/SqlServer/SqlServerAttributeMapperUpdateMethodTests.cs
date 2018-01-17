using System;
using System.Threading.Tasks;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerAttributeMapperUpdateMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public async Task UsingKey_UpdatesEntity()
        {
            var p1 = new FourLeggedFurryAnimal
            {
                Active = true,
                HowItsCalled = "Foo",
                DateCreated = DateTime.UtcNow
            };
            int id = await DapperExtensions.InsertAsync(Connection, p1);

            var p2 = await DapperExtensions.GetAsync<FourLeggedFurryAnimal>(Connection, id);
            p2.HowItsCalled = "Baz";
            p2.Active = false;

            await DapperExtensions.UpdateAsync(Connection, p2);

            var p3 = await DapperExtensions.GetAsync<FourLeggedFurryAnimal>(Connection, id);
            Assert.Equal("Baz", p3.HowItsCalled);
            Assert.False(p3.Active);
        }
    }
}