using System;
using System.Linq;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerAttributeMapperGetMultipleMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public void ReturnsItems()
        {
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow.AddDays(-10) });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow.AddDays(-10) });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow.AddDays(-3) });
            DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow.AddDays(-1) });

            DapperExtensions.Insert(Connection, new Animal { Name = "Foo" });
            DapperExtensions.Insert(Connection, new Animal { Name = "Bar" });
            DapperExtensions.Insert(Connection, new Animal { Name = "Baz" });

            var predicate = new GetMultiplePredicate();
            predicate.Add<FourLeggedFurryAnimal>(null);
            predicate.Add<Animal>(Predicates.Predicates.Field<Animal>(a => a.Name, Operator.Like, "Ba%"));
            predicate.Add<FourLeggedFurryAnimal>(Predicates.Predicates.Field<FourLeggedFurryAnimal>(a => a.HowItsCalled, Operator.Eq, "c"));

            var result = DapperExtensions.GetMultiple(Connection, predicate);
            var cats = result.Read<FourLeggedFurryAnimal>().ToList();
            var animals = result.Read<Animal>().ToList();
            var cats2 = result.Read<FourLeggedFurryAnimal>().ToList();

            Assert.Equal(4, cats.Count);
            Assert.Equal(2, animals.Count);
            Assert.Equal(1, cats2.Count);
        }
    }
}