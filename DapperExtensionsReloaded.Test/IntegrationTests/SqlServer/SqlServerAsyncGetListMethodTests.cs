using System;
using System.Linq;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerAsyncGetListMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public void UsingNullPredicate_ReturnsAll()
        {
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow });

            var list = DapperExtensions.GetListAsync<FourLeggedFurryAnimal>(Connection).GetAwaiter().GetResult();
            Assert.Equal(4, list.Count());
        }

        [Fact]
        public void UsingPredicate_ReturnsMatching()
        {
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow });

            var predicate = Predicates.Predicates.Field<FourLeggedFurryAnimal>(f => f.Active, Operator.Eq, true);
            var list = DapperExtensions.GetListAsync<FourLeggedFurryAnimal>(Connection, predicate, null).GetAwaiter().GetResult();
            Assert.Equal(2, list.Count());
            Assert.True(list.All(p => p.HowItsCalled == "a" || p.HowItsCalled == "c"));
        }

        [Fact]
        public void UsingObject_ReturnsMatching()
        {
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = null, DateCreated = DateTime.UtcNow });

            var predicate = new { Active = true, HowItsCalled = (string)null };
            var list = DapperExtensions.GetListAsync<FourLeggedFurryAnimal>(Connection, predicate, null).GetAwaiter().GetResult();
            Assert.Equal(1, list.Count());
            Assert.True(list.All(p => p.HowItsCalled == null));
        }
    }
}