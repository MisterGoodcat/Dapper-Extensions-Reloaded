using System;
using System.Linq;
using System.Threading.Tasks;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerAttributeMapperGetMultipleMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public async Task ReturnsItems()
        {
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow.AddDays(-10) });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow.AddDays(-10) });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow.AddDays(-3) });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow.AddDays(-1) });

            await DapperExtensions.InsertAsync(Connection, new Animal { Name = "Foo" });
            await DapperExtensions.InsertAsync(Connection, new Animal { Name = "Bar" });
            await DapperExtensions.InsertAsync(Connection, new Animal { Name = "Baz" });

            var predicate = new GetMultiplePredicate();
            predicate.Add<FourLeggedFurryAnimal>(null);
            predicate.Add<Animal>(Predicates.Predicates.Field<Animal>(a => a.Name, Operator.Like, "Ba%"));
            predicate.Add<FourLeggedFurryAnimal>(Predicates.Predicates.Field<FourLeggedFurryAnimal>(a => a.HowItsCalled, Operator.Eq, "c"));

            var result = await DapperExtensions.GetMultipleAsync(Connection, predicate);
            var cats = result.Read<FourLeggedFurryAnimal>().ToList();
            var animals = result.Read<Animal>().ToList();
            var cats2 = result.Read<FourLeggedFurryAnimal>().ToList();

            Assert.Equal(4, cats.Count);
            Assert.Equal(2, animals.Count);
            Assert.Single(cats2);
        }
    }
}