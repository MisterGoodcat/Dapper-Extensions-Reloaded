using System;
using System.Collections.Generic;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Predicates.Internal;
using Moq;
using Xunit;

namespace DapperExtensionsReloaded.Test.Sql
{
    public class SqlGeneratorSelectMethodTests : SqlGeneratorFixtureBase
    {
        [Fact]
        public void WithNullParameters_ThrowsException()
        {
            var sort = new Sort();
            var ex = Assert.Throws<ArgumentNullException>(
                () => Generator.Object.Select(ClassMap.Object, null, null, null));
            Assert.Equal("parameters", ex.ParamName);
        }

        [Fact]
        public void WithoutPredicateAndSort_GeneratesSql()
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();

            Generator.Setup(g => g.GetTableName(ClassMap.Object)).Returns("TableName").Verifiable();
            Generator.Setup(g => g.BuildSelectColumns(ClassMap.Object)).Returns("Columns").Verifiable();

            var result = Generator.Object.Select(ClassMap.Object, null, null, parameters);
            Assert.Equal((string)"SELECT Columns FROM TableName", (string)result);
            ClassMap.Verify();
            Generator.Verify();
        }

        [Fact]
        public void WithPredicate_GeneratesSql()
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            var predicate = new Mock<IPredicate>();
            predicate.Setup(p => p.GetSql(parameters)).Returns("PredicateWhere");

            Generator.Setup(g => g.GetTableName(ClassMap.Object)).Returns("TableName").Verifiable();
            Generator.Setup(g => g.BuildSelectColumns(ClassMap.Object)).Returns("Columns").Verifiable();

            var result = Generator.Object.Select(ClassMap.Object, predicate.Object, null, parameters);
            Assert.Equal((string)"SELECT Columns FROM TableName WHERE PredicateWhere", (string)result);
            ClassMap.Verify();
            predicate.Verify();
            Generator.Verify();
            predicate.Verify();
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

            var result = Generator.Object.Select(ClassMap.Object, null, sort, parameters);
            Assert.Equal((string)"SELECT Columns FROM TableName ORDER BY SortColumn ASC", (string)result);
            ClassMap.Verify();
            sortField.Verify();
            Generator.Verify();
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

            var result = Generator.Object.Select(ClassMap.Object, predicate.Object, sort, parameters);
            Assert.Equal((string)"SELECT Columns FROM TableName WHERE PredicateWhere ORDER BY SortColumn ASC", (string)result);
            ClassMap.Verify();
            sortField.Verify();
            predicate.Verify();
            Generator.Verify();
        }
    }
}