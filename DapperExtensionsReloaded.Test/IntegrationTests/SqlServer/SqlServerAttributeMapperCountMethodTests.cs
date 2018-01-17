using System;
using System.Threading.Tasks;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerAttributeMapperCountMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public async Task UsingNullPredicate_Returns_Count()
        {
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow.AddDays(-10) });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow.AddDays(-10) });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow.AddDays(-3) });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow.AddDays(-1) });

            var count = await DapperExtensions.CountAsync<FourLeggedFurryAnimal>(Connection);
            Assert.Equal(4, count);
        }

        [Fact]
        public async Task UsingPredicate_Returns_Count()
        {
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow.AddDays(-10) });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow.AddDays(-10) });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow.AddDays(-3) });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow.AddDays(-1) });

            var predicate = Predicates.Predicates.Field<FourLeggedFurryAnimal>(f => f.DateCreated, Operator.Lt, DateTime.UtcNow.AddDays(-5));
            var count = await DapperExtensions.CountAsync<FourLeggedFurryAnimal>(Connection, predicate);
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task UsingEqualsPredicate_Returns_Count()
        {
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow.AddDays(-10) });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow.AddDays(-10) });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow.AddDays(-3) });
            await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow.AddDays(-1) });

            var predicate = Predicates.Predicates.Field<FourLeggedFurryAnimal>(x => x.HowItsCalled, Operator.Eq, new[] { "b", "d" });
            var count = await DapperExtensions.CountAsync<FourLeggedFurryAnimal>(Connection, predicate);
            Assert.Equal(2, count);
        }
    }
}