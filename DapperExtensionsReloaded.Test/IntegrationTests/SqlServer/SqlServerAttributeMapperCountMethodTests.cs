using System;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerAttributeMapperCountMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public void UsingNullPredicate_Returns_Count()
        {
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow.AddDays(-10) });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow.AddDays(-10) });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow.AddDays(-3) });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow.AddDays(-1) });

            var count = DapperExtensions.Count<FourLeggedFurryAnimal>(Connection, null);
            Assert.Equal(4, count);
        }

        [Fact]
        public void UsingPredicate_Returns_Count()
        {
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow.AddDays(-10) });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow.AddDays(-10) });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow.AddDays(-3) });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow.AddDays(-1) });

            var predicate = Predicates.Predicates.Field<FourLeggedFurryAnimal>(f => f.DateCreated, Operator.Lt, DateTime.UtcNow.AddDays(-5));
            var count = DapperExtensions.Count<FourLeggedFurryAnimal>(Connection, predicate);
            Assert.Equal(2, count);
        }

        [Fact]
        public void UsingObject_Returns_Count()
        {
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow.AddDays(-10) });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow.AddDays(-10) });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow.AddDays(-3) });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow.AddDays(-1) });

            var predicate = new { HowItsCalled = new[] { "b", "d" } };
            var count = DapperExtensions.Count<FourLeggedFurryAnimal>(Connection, predicate);
            Assert.Equal(2, count);
        }
    }
}