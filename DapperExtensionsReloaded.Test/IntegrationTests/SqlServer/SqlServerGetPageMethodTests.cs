using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerGetPageMethodTests : SqlServerBaseFixture
    {
        [Fact]
        public async Task UsingNullPredicate_ReturnsMatching()
        {
            var id1 = await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "Sigma", LastName = "Alpha", DateCreated = DateTime.UtcNow });
            var id2 = await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "Delta", LastName = "Alpha", DateCreated = DateTime.UtcNow });
            var id3 = await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "Theta", LastName = "Gamma", DateCreated = DateTime.UtcNow });
            var id4 = await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "Iota", LastName = "Beta", DateCreated = DateTime.UtcNow });

            IList<ISort> sort = new List<ISort>
            {
                Predicates.Predicates.Sort<Person>(p => p.LastName),
                Predicates.Predicates.Sort<Person>(p => p.FirstName)
            };

            var list = await DapperExtensions.GetPageAsync<Person>(Connection, null, sort, 0, 2);
            Assert.Equal(2, list.Count());
            Assert.Equal(id2, list.First().Id);
            Assert.Equal(id1, list.Skip(1).First().Id);
        }

        [Fact]
        public async Task UsingPredicate_ReturnsMatching()
        {
            var id1 = await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "Sigma", LastName = "Alpha", DateCreated = DateTime.UtcNow });
            var id2 = await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "Delta", LastName = "Alpha", DateCreated = DateTime.UtcNow });
            var id3 = await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "Theta", LastName = "Gamma", DateCreated = DateTime.UtcNow });
            var id4 = await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "Iota", LastName = "Beta", DateCreated = DateTime.UtcNow });

            var predicate = Predicates.Predicates.Field<Person>(f => f.Active, Operator.Eq, true);
            IList<ISort> sort = new List<ISort>
            {
                Predicates.Predicates.Sort<Person>(p => p.LastName),
                Predicates.Predicates.Sort<Person>(p => p.FirstName)
            };

            var list = await DapperExtensions.GetPageAsync<Person>(Connection, predicate, sort, 0, 3);
            Assert.Equal(2, list.Count());
            Assert.True(list.All(p => p.FirstName == "Sigma" || p.FirstName == "Theta"));
        }

        [Fact]
        public async Task NotFirstPage_Returns_NextResults()
        {
            var id1 = await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "Sigma", LastName = "Alpha", DateCreated = DateTime.UtcNow });
            var id2 = await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "Delta", LastName = "Alpha", DateCreated = DateTime.UtcNow });
            var id3 = await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "Theta", LastName = "Gamma", DateCreated = DateTime.UtcNow });
            var id4 = await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "Iota", LastName = "Beta", DateCreated = DateTime.UtcNow });

            IList<ISort> sort = new List<ISort>
            {
                Predicates.Predicates.Sort<Person>(p => p.LastName),
                Predicates.Predicates.Sort<Person>(p => p.FirstName)
            };

            var list = await DapperExtensions.GetPageAsync<Person>(Connection, null, sort, 1, 2);
            Assert.Equal(2, list.Count());
            Assert.Equal(id4, list.First().Id);
            Assert.Equal(id3, list.Skip(1).First().Id);
        }

        [Fact]
        public async Task UsingObject_ReturnsMatching()
        {
            var id1 = await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "Sigma", LastName = "Alpha", DateCreated = DateTime.UtcNow });
            var id2 = await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "Delta", LastName = "Alpha", DateCreated = DateTime.UtcNow });
            var id3 = await DapperExtensions.InsertAsync(Connection, new Person { Active = true, FirstName = "Theta", LastName = "Gamma", DateCreated = DateTime.UtcNow });
            var id4 = await DapperExtensions.InsertAsync(Connection, new Person { Active = false, FirstName = "Iota", LastName = "Beta", DateCreated = DateTime.UtcNow });

            var predicate = new { Active = true };
            IList<ISort> sort = new List<ISort>
            {
                Predicates.Predicates.Sort<Person>(p => p.LastName),
                Predicates.Predicates.Sort<Person>(p => p.FirstName)
            };

            var list = await DapperExtensions.GetPageAsync<Person>(Connection, predicate, sort, 0, 3);
            Assert.Equal(2, list.Count());
            Assert.True(list.All(p => p.FirstName == "Sigma" || p.FirstName == "Theta"));
        }
    }
}