using System;
using System.Linq;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerAsyncInsertMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public void AddsEntityToDatabase_ReturnsKey()
        {
            var p = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
            int id = Connection.Insert(p);
            Assert.Equal(1, id);
            Assert.Equal(1, p.Id);
        }

        [Fact]
        public void AddsEntityToDatabase_ReturnsGeneratedPrimaryKey()
        {
            var a1 = new Animal { Name = "Foo" };
            Connection.Insert(a1);

            var a2 = Connection.GetAsync<Animal>(a1.Id).GetAwaiter().GetResult();
            Assert.NotEqual(Guid.Empty, a2.Id);
            Assert.Equal(a1.Id, a2.Id);
        }

        [Fact]
        public void AddsMultipleEntitiesToDatabase()
        {
            var a1 = new Animal { Name = "Foo" };
            var a2 = new Animal { Name = "Bar" };
            var a3 = new Animal { Name = "Baz" };

            Connection.Insert<Animal>(new[] { a1, a2, a3 });

            var animals = Connection.GetListAsync<Animal>().GetAwaiter().GetResult().ToList();
            Assert.Equal(3, animals.Count);
        }
    }
}