using System;
using System.Linq;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerAttributeMapperDeleteMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public void UsingKey_DeletesFromDatabase()
        {
            var p1 = new FourLeggedFurryAnimal
            {
                Active = true,
                HowItsCalled = "Foo",
                DateCreated = DateTime.UtcNow
            };
            int id = DapperExtensions.Insert(Connection, p1);

            var p2 = DapperExtensions.Get<FourLeggedFurryAnimal>(Connection, id);
            DapperExtensions.Delete(Connection, p2);
            Assert.Null(DapperExtensions.Get<FourLeggedFurryAnimal>(Connection, id));
        }

        [Fact]
        public void UsingPredicate_DeletesRows()
        {
            var p1 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
            var p2 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
            var p3 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo2", DateCreated = DateTime.UtcNow };
            DapperExtensions.Insert(Connection, p1);
            DapperExtensions.Insert(Connection, p2);
            DapperExtensions.Insert(Connection, p3);

            var list = DapperExtensions.GetList<FourLeggedFurryAnimal>(Connection);
            Assert.Equal(3, list.Count());

            IPredicate pred = Predicates.Predicates.Field<FourLeggedFurryAnimal>(p => p.HowItsCalled, Operator.Eq, "Foo2");
            var result = DapperExtensions.Delete<FourLeggedFurryAnimal>(Connection, pred);
            Assert.True(result);

            list = DapperExtensions.GetList<FourLeggedFurryAnimal>(Connection);
            Assert.Equal(2, list.Count());
        }

        [Fact]
        public void UsingObject_DeletesRows()
        {
            var p1 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
            var p2 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
            var p3 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo2", DateCreated = DateTime.UtcNow };
            DapperExtensions.Insert(Connection, p1);
            DapperExtensions.Insert(Connection, p2);
            DapperExtensions.Insert(Connection, p3);

            var list = DapperExtensions.GetList<FourLeggedFurryAnimal>(Connection);
            Assert.Equal(3, list.Count());

            var result = DapperExtensions.Delete<FourLeggedFurryAnimal>(Connection, new { HowItsCalled = "Foo2" });
            Assert.True(result);

            list = DapperExtensions.GetList<FourLeggedFurryAnimal>(Connection);
            Assert.Equal(2, list.Count());
        }
    }
}