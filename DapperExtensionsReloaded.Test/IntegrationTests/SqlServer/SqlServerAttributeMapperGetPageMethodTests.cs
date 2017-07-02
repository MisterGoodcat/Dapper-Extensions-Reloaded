using System;
using System.Collections.Generic;
using System.Linq;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerAttributeMapperGetPageMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public void UsingNullPredicate_ReturnsMatching()
        {
            var id1 = DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Sigma", DateCreated = DateTime.UtcNow });
            var id2 = DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Delta", DateCreated = DateTime.UtcNow });
            var id3 = DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Theta", DateCreated = DateTime.UtcNow });
            var id4 = DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Iota", DateCreated = DateTime.UtcNow });

            IList<ISort> sort = new List<ISort>
            {
                Predicates.Predicates.Sort<FourLeggedFurryAnimal>(p => p.HowItsCalled)
            };

            var list = DapperExtensions.GetPage<FourLeggedFurryAnimal>(Connection, null, sort, 0, 2);
            Assert.Equal(2, list.Count());
            Assert.Equal(id2, list.First().Id);
            Assert.Equal(id4, list.Skip(1).First().Id);
        }

        [Fact]
        public void UsingPredicate_ReturnsMatching()
        {
            var id1 = DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Sigma", DateCreated = DateTime.UtcNow });
            var id2 = DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Delta", DateCreated = DateTime.UtcNow });
            var id3 = DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Theta", DateCreated = DateTime.UtcNow });
            var id4 = DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Iota", DateCreated = DateTime.UtcNow });

            var predicate = Predicates.Predicates.Field<FourLeggedFurryAnimal>(f => f.Active, Operator.Eq, true);
            IList<ISort> sort = new List<ISort>
            {
                Predicates.Predicates.Sort<FourLeggedFurryAnimal>(p => p.HowItsCalled)
            };

            var list = DapperExtensions.GetPage<FourLeggedFurryAnimal>(Connection, predicate, sort, 0, 3);
            Assert.Equal(2, list.Count());
            Assert.True(list.All(p => p.HowItsCalled == "Sigma" || p.HowItsCalled == "Theta"));
        }

        [Fact]
        public void NotFirstPage_Returns_NextResults()
        {
            var id1 = DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Sigma", DateCreated = DateTime.UtcNow });
            var id2 = DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Delta", DateCreated = DateTime.UtcNow });
            var id3 = DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Theta", DateCreated = DateTime.UtcNow });
            var id4 = DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Iota", DateCreated = DateTime.UtcNow });

            IList<ISort> sort = new List<ISort>
            {
                Predicates.Predicates.Sort<FourLeggedFurryAnimal>(p => p.HowItsCalled)
            };

            var list = DapperExtensions.GetPage<FourLeggedFurryAnimal>(Connection, null, sort, 1, 2);
            Assert.Equal(2, list.Count());
            Assert.Equal(id1, list.First().Id);
            Assert.Equal(id3, list.Skip(1).First().Id);
        }

        [Fact]
        public void UsingObject_ReturnsMatching()
        {
            var id1 = DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Sigma", DateCreated = DateTime.UtcNow });
            var id2 = DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Delta", DateCreated = DateTime.UtcNow });
            var id3 = DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Theta", DateCreated = DateTime.UtcNow });
            var id4 = DapperExtensions.Insert(Connection, new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Iota", DateCreated = DateTime.UtcNow });

            var predicate = new { Active = true };
            IList<ISort> sort = new List<ISort>
            {
                Predicates.Predicates.Sort<FourLeggedFurryAnimal>(p => p.HowItsCalled)
            };

            var list = DapperExtensions.GetPage<FourLeggedFurryAnimal>(Connection, predicate, sort, 0, 3);
            Assert.Equal(2, list.Count());
            Assert.True(list.All(p => p.HowItsCalled == "Sigma" || p.HowItsCalled == "Theta"));
        }
    }
}