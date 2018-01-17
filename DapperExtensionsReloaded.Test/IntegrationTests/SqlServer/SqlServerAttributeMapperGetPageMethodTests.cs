using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerAttributeMapperGetPageMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public async Task UsingNullPredicate_ReturnsMatching()
        {
            var id1 = await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Sigma", DateCreated = DateTime.UtcNow });
            var id2 = await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Delta", DateCreated = DateTime.UtcNow });
            var id3 = await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Theta", DateCreated = DateTime.UtcNow });
            var id4 = await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Iota", DateCreated = DateTime.UtcNow });

            IList<ISort> sort = new List<ISort>
            {
                Predicates.Predicates.Sort<FourLeggedFurryAnimal>(p => p.HowItsCalled)
            };

            var list = await DapperExtensions.GetPageAsync<FourLeggedFurryAnimal>(Connection, null, sort, 0, 2);
            Assert.Equal(2, list.Count());
            Assert.Equal(id2, list.First().Id);
            Assert.Equal(id4, list.Skip(1).First().Id);
        }

        [Fact]
        public async Task UsingPredicate_ReturnsMatching()
        {
            var id1 = await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Sigma", DateCreated = DateTime.UtcNow });
            var id2 = await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Delta", DateCreated = DateTime.UtcNow });
            var id3 = await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Theta", DateCreated = DateTime.UtcNow });
            var id4 = await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Iota", DateCreated = DateTime.UtcNow });

            var predicate = Predicates.Predicates.Field<FourLeggedFurryAnimal>(f => f.Active, Operator.Eq, true);
            IList<ISort> sort = new List<ISort>
            {
                Predicates.Predicates.Sort<FourLeggedFurryAnimal>(p => p.HowItsCalled)
            };

            var list = await DapperExtensions.GetPageAsync<FourLeggedFurryAnimal>(Connection, predicate, sort, 0, 3);
            Assert.Equal(2, list.Count());
            Assert.True(list.All(p => p.HowItsCalled == "Sigma" || p.HowItsCalled == "Theta"));
        }

        [Fact]
        public async Task NotFirstPage_Returns_NextResults()
        {
            var id1 = await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Sigma", DateCreated = DateTime.UtcNow });
            var id2 = await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Delta", DateCreated = DateTime.UtcNow });
            var id3 = await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Theta", DateCreated = DateTime.UtcNow });
            var id4 = await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Iota", DateCreated = DateTime.UtcNow });

            IList<ISort> sort = new List<ISort>
            {
                Predicates.Predicates.Sort<FourLeggedFurryAnimal>(p => p.HowItsCalled)
            };

            var list = await DapperExtensions.GetPageAsync<FourLeggedFurryAnimal>(Connection, null, sort, 1, 2);
            Assert.Equal(2, list.Count());
            Assert.Equal(id1, list.First().Id);
            Assert.Equal(id3, list.Skip(1).First().Id);
        }

        [Fact]
        public async Task UsingEqualsPredicate_ReturnsMatching()
        {
            var id1 = await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Sigma", DateCreated = DateTime.UtcNow });
            var id2 = await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Delta", DateCreated = DateTime.UtcNow });
            var id3 = await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Theta", DateCreated = DateTime.UtcNow });
            var id4 = await DapperExtensions.InsertAsync(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Iota", DateCreated = DateTime.UtcNow });
            
            var predicate = Predicates.Predicates.Field<FourLeggedFurryAnimal>(x => x.Active, Operator.Eq, true);
            IList<ISort> sort = new List<ISort>
            {
                Predicates.Predicates.Sort<FourLeggedFurryAnimal>(p => p.HowItsCalled)
            };

            var list = await DapperExtensions.GetPageAsync<FourLeggedFurryAnimal>(Connection, predicate, sort, 0, 3);
            Assert.Equal(2, list.Count());
            Assert.True(list.All(p => p.HowItsCalled == "Sigma" || p.HowItsCalled == "Theta"));
        }
    }
}