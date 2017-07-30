using System;
using System.Collections.Generic;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Predicates.Internal;
using Moq;
using Xunit;

namespace DapperExtensionsReloaded.Test.Sql
{
    public class SqlGeneratorSelectPagedMethodTests : SqlGeneratorFixtureBase
    {
        [Fact]
        public void WithNoSort_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => Generator.Object.SelectPaged(ClassMap.Object, null, null, 0, 1, new Dictionary<string, object>()));
            Assert.Equal("sort", ex.ParamName);
        }

        [Fact]
        public void WithEmptySort_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => Generator.Object.SelectPaged(ClassMap.Object, null, new List<ISort>(), 0, 1, new Dictionary<string, object>()));
            Assert.Equal("sort", ex.ParamName);
        }

        [Fact]
        public void WithNullParameters_ThrowsException()
        {
            var sort = new Sort();
            var ex = Assert.Throws<ArgumentNullException>(
                () => Generator.Object.SelectPaged(ClassMap.Object, null, new List<ISort> { sort }, 0, 1, null));
            Assert.Equal("parameters", ex.ParamName);
        }

        [Fact]
        public void WithSort_GeneratesSql()
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            var sortField = new Mock<ISort>();
            sortField.SetupGet(s => s.PropertyName).Returns("SortProperty").Verifiable();
            sortField.SetupGet(s => s.Ascending).Returns(true).Verifiable();
            var sort = new List<ISort>
            {
                sortField.Object
            };

            Generator.Setup(g => g.GetTableName(ClassMap.Object)).Returns("TableName").Verifiable();
            Generator.Setup(g => g.BuildSelectColumns(ClassMap.Object)).Returns("Columns").Verifiable();
            Generator.Setup(g => g.GetColumnName(ClassMap.Object, "SortProperty", false)).Returns("SortColumn").Verifiable();

            Dialect.Setup(d => d.GetPagingSql("SELECT Columns FROM TableName ORDER BY SortColumn ASC", 2, 10, parameters)).Returns("PagedSQL").Verifiable();

            var result = Generator.Object.SelectPaged(ClassMap.Object, null, sort, 2, 10, parameters);
            Assert.Equal("PagedSQL", result);
            ClassMap.Verify();
            sortField.Verify();
            Generator.Verify();
            Dialect.Verify();
        }

        [Fact]
        public void WithPredicateAndSort_GeneratesSql()
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            var sortField = new Mock<ISort>();
            sortField.SetupGet(s => s.PropertyName).Returns("SortProperty").Verifiable();
            sortField.SetupGet(s => s.Ascending).Returns(true).Verifiable();
            var sort = new List<ISort>
            {
                sortField.Object
            };

            var predicate = new Mock<IPredicate>();
            predicate.Setup(p => p.GetSql(parameters)).Returns("PredicateWhere");

            Generator.Setup(g => g.GetTableName(ClassMap.Object)).Returns("TableName").Verifiable();
            Generator.Setup(g => g.BuildSelectColumns(ClassMap.Object)).Returns("Columns").Verifiable();
            Generator.Setup(g => g.GetColumnName(ClassMap.Object, "SortProperty", false)).Returns("SortColumn").Verifiable();

            Dialect.Setup(d => d.GetPagingSql("SELECT Columns FROM TableName WHERE PredicateWhere ORDER BY SortColumn ASC", 2, 10, parameters)).Returns("PagedSQL").Verifiable();

            var result = Generator.Object.SelectPaged(ClassMap.Object, predicate.Object, sort, 2, 10, parameters);
            Assert.Equal("PagedSQL", result);
            ClassMap.Verify();
            sortField.Verify();
            predicate.Verify();
            Generator.Verify();
        }
    }
}