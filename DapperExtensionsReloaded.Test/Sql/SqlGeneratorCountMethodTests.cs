using System;
using System.Collections.Generic;
using DapperExtensionsReloaded.Predicates;
using Moq;
using Xunit;

namespace DapperExtensionsReloaded.Test.Sql
{
    public class SqlGeneratorCountMethodTests : SqlGeneratorFixtureBase
    {
        [Fact]
        public void WithNullParameters_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Generator.Object.Count(ClassMap.Object, null, null));
            Assert.Equal("parameters", ex.ParamName);
        }

        [Fact]
        public void WithoutPredicate_ThrowsException()
        {
            Generator.Setup(g => g.GetTableName(ClassMap.Object)).Returns("TableName").Verifiable();
            Dialect.SetupGet(d => d.OpenQuote).Returns('!').Verifiable();
            Dialect.SetupGet(d => d.CloseQuote).Returns('^').Verifiable();

            var result = Generator.Object.Count(ClassMap.Object, null, new Dictionary<string, object>());
            Assert.Equal("SELECT COUNT(*) AS !Total^ FROM TableName", result);
            Generator.Verify();
            Dialect.Verify();
        }

        [Fact]
        public void WithPredicate_ThrowsException()
        {
            var parameters = new Dictionary<string, object>();
            var predicate = new Mock<IPredicate>();
            predicate.Setup(p => p.GetSql(parameters)).Returns("PredicateWhere").Verifiable();
            Dialect.SetupGet(d => d.OpenQuote).Returns('!').Verifiable();
            Dialect.SetupGet(d => d.CloseQuote).Returns('^').Verifiable();

            Generator.Setup(g => g.GetTableName(ClassMap.Object)).Returns("TableName").Verifiable();

            var result = Generator.Object.Count(ClassMap.Object, predicate.Object, parameters);
            Assert.Equal("SELECT COUNT(*) AS !Total^ FROM TableName WHERE PredicateWhere", result);
            Generator.Verify();
            predicate.Verify();
            Dialect.Verify();
        }
    }
}