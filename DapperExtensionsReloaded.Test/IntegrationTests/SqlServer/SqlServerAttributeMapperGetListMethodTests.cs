using System;
using System.Linq;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerAttributeMapperGetListMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public void UsingNullPredicate_ReturnsAll()
        {
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow });

            var list = DapperExtensions.GetList<FourLeggedFurryAnimal>(Connection);
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
            var list = DapperExtensions.GetList<FourLeggedFurryAnimal>(Connection, predicate, null);
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

            var predicate = new { Active = true, HowItsCalled = "c" };
            var list = DapperExtensions.GetList<FourLeggedFurryAnimal>(Connection, predicate, null);
            Assert.Equal(1, list.Count());
            Assert.True(list.All(p => p.HowItsCalled == "c"));
        }
    }
}