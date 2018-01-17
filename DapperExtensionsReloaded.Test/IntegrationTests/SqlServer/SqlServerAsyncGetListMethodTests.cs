using System;
using System.Linq;
using System.Threading.Tasks;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerAsyncGetListMethodTests : SqlServerBaseFixture
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
            var list = await DapperExtensions.GetListAsync<FourLeggedFurryAnimal>(Connection, predicate);
            Assert.Equal(2, list.Count());
            Assert.True(list.All(p => p.HowItsCalled == "a" || p.HowItsCalled == "c"));
        }

        [Fact]
        public async Task UsingEqualsPredicate_ReturnsMatching()
        {
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = null, DateCreated = DateTime.UtcNow });

            var predicate = Predicates.Predicates.Group(GroupOperator.And,
                                        Predicates.Predicates.Field<FourLeggedFurryAnimal>(x => x.Active, Operator.Eq, true),
                                        Predicates.Predicates.Field<FourLeggedFurryAnimal>(x => x.HowItsCalled, Operator.Eq, null));
            
            var list = await DapperExtensions.GetListAsync<FourLeggedFurryAnimal>(Connection, predicate);
            Assert.Single(list);
            Assert.True(list.All(p => p.HowItsCalled == null));
        }
    }
}