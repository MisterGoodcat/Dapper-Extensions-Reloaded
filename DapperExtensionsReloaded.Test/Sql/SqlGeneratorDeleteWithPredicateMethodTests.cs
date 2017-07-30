using System;
using System.Collections.Generic;
using DapperExtensionsReloaded.Predicates;
using Moq;
using Xunit;

namespace DapperExtensionsReloaded.Test.Sql
{
    public class SqlGeneratorDeleteWithPredicateMethodTests : SqlGeneratorFixtureBase
    {
        [Fact]
        public void WithNullPredicate_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Generator.Object.Delete(ClassMap.Object, null, new Dictionary<string, object>()));
            Assert.Equal("predicate", ex.ParamName);
        }

        [Fact]
        public void WithNullParameters_ThrowsException()
        {
            var predicate = new Mock<IPredicate>();
            var ex = Assert.Throws<ArgumentNullException>(() => Generator.Object.Delete(ClassMap.Object, predicate.Object, null));
            Assert.Equal("parameters", ex.ParamName);
        }

        [Fact]
        public void GeneratesSql()
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            var predicate = new Mock<IPredicate>();
            predicate.Setup(p => p.GetSql(parameters)).Returns("PredicateWhere");

            Generator.Setup(g => g.GetTableName(ClassMap.Object)).Returns("TableName").Verifiable();

            var result = Generator.Object.Delete(ClassMap.Object, predicate.Object, parameters);
            Assert.Equal("DELETE FROM TableName WHERE PredicateWhere", result);
            ClassMap.Verify();
            predicate.Verify();
            Generator.Verify();
        }
    }
}