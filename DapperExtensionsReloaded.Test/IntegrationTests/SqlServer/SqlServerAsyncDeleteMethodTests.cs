using System;
using System.Linq;
using System.Threading.Tasks;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerAsyncDeleteMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public async Task UsingKey_DeletesFromDatabase()
        {
            var p1 = new FourLeggedFurryAnimal
            {
                Active = true,
                HowItsCalled = "Foo",
                DateCreated = DateTime.UtcNow
            };
            int id = await DapperExtensions.InsertAsync(Connection, p1);

            var p2 = await DapperExtensions.GetAsync<FourLeggedFurryAnimal>(Connection, id);
            await DapperExtensions.DeleteAsync(Connection, p2);
            Assert.Null(await DapperExtensions.GetAsync<FourLeggedFurryAnimal>(Connection, id));
        }

        [Fact]
        public async Task UsingPredicate_DeletesRows()
        {
            var p1 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
            var p2 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
            var p3 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo2", DateCreated = DateTime.UtcNow };
            await DapperExtensions.InsertAsync(Connection, p1);
            await DapperExtensions.InsertAsync(Connection, p2);
            await DapperExtensions.InsertAsync(Connection, p3);

            var list = await DapperExtensions.GetListAsync<FourLeggedFurryAnimal>(Connection);
            Assert.Equal(3, list.Count());

            IPredicate pred = Predicates.Predicates.Field<FourLeggedFurryAnimal>(p => p.HowItsCalled, Operator.Eq, "Foo2");
            var result = await DapperExtensions.DeleteAsync<FourLeggedFurryAnimal>(Connection, pred);
            Assert.True(result);

            list = await DapperExtensions.GetListAsync<FourLeggedFurryAnimal>(Connection);
            Assert.Equal(2, list.Count());
        }

        [Fact]
        public async Task UsingEqualsPredicate_DeletesRows()
        {
            var p1 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
            var p2 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
            var p3 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo2", DateCreated = DateTime.UtcNow };
            await DapperExtensions.InsertAsync(Connection, p1);
            await DapperExtensions.InsertAsync(Connection, p2);
            await DapperExtensions.InsertAsync(Connection, p3);

            var list = await DapperExtensions.GetListAsync<FourLeggedFurryAnimal>(Connection);
            Assert.Equal(3, list.Count());

            var predicate = Predicates.Predicates.Field<FourLeggedFurryAnimal>(x => x.HowItsCalled, Operator.Eq, new[] { "Foo2" });
            var result = await DapperExtensions.DeleteAsync<FourLeggedFurryAnimal>(Connection, predicate);
            Assert.True(result);

            list = await DapperExtensions.GetListAsync<FourLeggedFurryAnimal>(Connection);
            Assert.Equal(2, list.Count());
        }
    }
}