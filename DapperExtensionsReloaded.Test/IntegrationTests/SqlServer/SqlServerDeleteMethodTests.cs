using System;
using System.Linq;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerDeleteMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public void UsingKey_DeletesFromDatabase()
        {
            var p1 = new Person
            {
                Active = true,
                FirstName = "Foo",
                LastName = "Bar",
                DateCreated = DateTime.UtcNow
            };
            int id = DapperExtensions.Insert(Connection, p1);

            var p2 = DapperExtensions.Get<Person>(Connection, id);
            DapperExtensions.Delete(Connection, p2);
            Assert.Null(DapperExtensions.Get<Person>(Connection, id));
        }

        [Fact]
        public void UsingPredicate_DeletesRows()
        {
            var p1 = new Person { Active = true, FirstName = "Foo", LastName = "Bar", DateCreated = DateTime.UtcNow };
            var p2 = new Person { Active = true, FirstName = "Foo", LastName = "Bar", DateCreated = DateTime.UtcNow };
            var p3 = new Person { Active = true, FirstName = "Foo", LastName = "Barz", DateCreated = DateTime.UtcNow };
            DapperExtensions.Insert(Connection, p1);
            DapperExtensions.Insert(Connection, p2);
            DapperExtensions.Insert(Connection, p3);

            var list = DapperExtensions.GetList<Person>(Connection);
            Assert.Equal(3, list.Count());

            IPredicate pred = Predicates.Predicates.Field<Person>(p => p.LastName, Operator.Eq, "Bar");
            var result = DapperExtensions.Delete<Person>(Connection, pred);
            Assert.True(result);

            list = DapperExtensions.GetList<Person>(Connection);
            Assert.Single(list);
        }

        [Fact]
        public void UsingObject_DeletesRows()
        {
            var p1 = new Person { Active = true, FirstName = "Foo", LastName = "Bar", DateCreated = DateTime.UtcNow };
            var p2 = new Person { Active = true, FirstName = "Foo", LastName = "Bar", DateCreated = DateTime.UtcNow };
            var p3 = new Person { Active = true, FirstName = "Foo", LastName = "Barz", DateCreated = DateTime.UtcNow };
            DapperExtensions.Insert(Connection, p1);
            DapperExtensions.Insert(Connection, p2);
            DapperExtensions.Insert(Connection, p3);

            var list = DapperExtensions.GetList<Person>(Connection);
            Assert.Equal(3, list.Count());

            var result = DapperExtensions.Delete<Person>(Connection, new { LastName = "Bar" });
            Assert.True(result);

            list = DapperExtensions.GetList<Person>(Connection);
            Assert.Single(list);
        }
    }
}