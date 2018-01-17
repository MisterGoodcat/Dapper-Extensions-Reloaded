using System;
using System.Linq;
using System.Threading.Tasks;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerAttributeMapperSqlServerAttributeMapperInsertMethodTestsTests : SqlServerBaseFixture
    {
        [Fact]
        public async Task AddsEntityToDatabase_ReturnsKey()
        {
            var p = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
            int id = await Connection.InsertAsync(p);
            Assert.Equal(1, id);
            Assert.Equal(1, p.Id);
        }

        [Fact]
        public async Task AddsEntityToDatabase_ReturnsPreGeneratedPrimaryKey()
        {
            var a1 = new Animal { Name = "Foo" };
            await Connection.InsertAsync(a1);

            var a2 = await Connection.GetAsync<Animal>(a1.Id);
            Assert.NotEqual(Guid.Empty, a2.Id);
            Assert.Equal(a1.Id, a2.Id);
        }

        [Fact]
        public async Task AddsMultipleEntitiesToDatabase()
        {
            var a1 = new Animal { Name = "Foo" };
            var a2 = new Animal { Name = "Bar" };
            var a3 = new Animal { Name = "Baz" };

            await Connection.InsertAsync<Animal>(new[] { a1, a2, a3 });

            var animals = await Connection.GetListAsync<Animal>();
            Assert.Equal(3, animals.Count());
        }
    }
}