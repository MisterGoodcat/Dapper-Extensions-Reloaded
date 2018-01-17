using System;
using System.Linq;
using System.Threading.Tasks;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerAsyncUpdateMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public async Task UsingKey_UpdatesEntity()
        {
            var p1 = new FourLeggedFurryAnimal
            {
                Active = true,
                HowItsCalled = "Foo",
                DateCreated = DateTime.UtcNow
            };
            int id = await DapperExtensions.InsertAsync(Connection, p1);

            var p2 = await DapperExtensions.GetAsync<FourLeggedFurryAnimal>(Connection, id);
            p2.HowItsCalled = "Baz";
            p2.Active = false;

            await DapperExtensions.UpdateAsync(Connection, p2);

            var p3 = await DapperExtensions.GetAsync<FourLeggedFurryAnimal>(Connection, id);
            Assert.Equal("Baz", p3.HowItsCalled);
            Assert.False(p3.Active);
        }
        
        [Fact]
        public async Task UpdateMultipleUsingUpdateSet_AllUpdatesPerformedCorrectly()
        {
            var a1 = new FourLeggedFurryAnimal { HowItsCalled = "abc", Active = true, DateCreated = DateTime.Now };
            var a2 = new FourLeggedFurryAnimal { HowItsCalled = "bcd", Active = true, DateCreated = DateTime.Now };
            var a3 = new FourLeggedFurryAnimal { HowItsCalled = "cde", Active = true, DateCreated = DateTime.Now };
            var a4 = new FourLeggedFurryAnimal { HowItsCalled = "def", Active = true, DateCreated = DateTime.Now };

            await Connection.InsertAsync<FourLeggedFurryAnimal>(new[] { a1, a2, a3, a4 });

            await Connection.UpdateSetAsync<FourLeggedFurryAnimal>(
                new { HowItsCalled = "Bla", Active = false },
                Predicates.Predicates.Field<FourLeggedFurryAnimal>(x => x.HowItsCalled, Operator.Like, "%b%")
            );

            var animals = (await Connection.GetListAsync<FourLeggedFurryAnimal>()).ToList();
            
            Assert.Equal(2, animals.Count(x => x.HowItsCalled.Equals("Bla")));
            Assert.Equal(1, animals.Count(x => x.HowItsCalled.Equals("cde")));
            Assert.Equal(1, animals.Count(x => x.HowItsCalled.Equals("def")));
            Assert.All(animals.Where(x => x.HowItsCalled.Equals("Bla")), x => Assert.False(x.Active));
            Assert.All(animals.Where(x => !x.HowItsCalled.Equals("Bla")), x => Assert.True(x.Active));
        }
    }
}