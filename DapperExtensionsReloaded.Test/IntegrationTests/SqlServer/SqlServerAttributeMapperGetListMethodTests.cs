using System;
using System.Linq;
using System.Threading.Tasks;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerAttributeMapperGetListMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public async Task UsingNullPredicate_ReturnsAll()
        {
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow });

            var list = await DapperExtensions.GetListAsync<FourLeggedFurryAnimal>(Connection);
            Assert.Equal(4, list.Count());
        }

        [Fact]
        public async Task UsingPredicate_ReturnsMatching()
        {
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow });

            var predicate = Predicates.Predicates.Field<FourLeggedFurryAnimal>(f => f.Active, Operator.Eq, true);
            var list = await DapperExtensions.GetListAsync<FourLeggedFurryAnimal>(Connection, predicate, null);
            Assert.Equal(2, list.Count());
            Assert.True(list.All(p => p.HowItsCalled == "a" || p.HowItsCalled == "c"));
        }

        [Fact]
        public async Task UsingObject_ReturnsMatching()
        {
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow });

            var predicate = new { Active = true, HowItsCalled = "c" };
            var list = await DapperExtensions.GetListAsync<FourLeggedFurryAnimal>(Connection, predicate, null);
            Assert.Single(list);
            Assert.True(list.All(p => p.HowItsCalled == "c"));
        }
    }
}