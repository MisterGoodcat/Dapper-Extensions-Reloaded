using System;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerAsyncUpdateMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public void UsingKey_UpdatesEntity()
        {
            var p1 = new FourLeggedFurryAnimal
            {
                Active = true,
                HowItsCalled = "Foo",
                DateCreated = DateTime.UtcNow
            };
            int id = DapperExtensions.Insert(Connection, p1);

            var p2 = DapperExtensions.GetAsync<FourLeggedFurryAnimal>(Connection, id).GetAwaiter().GetResult();
            p2.HowItsCalled = "Baz";
            p2.Active = false;

            DapperExtensions.Update(Connection, p2);

            var p3 = DapperExtensions.GetAsync<FourLeggedFurryAnimal>(Connection, id).GetAwaiter().GetResult();
            Assert.Equal("Baz", p3.HowItsCalled);
            Assert.False(p3.Active);
        }
    }
}