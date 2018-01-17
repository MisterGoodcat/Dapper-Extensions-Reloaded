using System;
using System.Collections.Generic;
using DapperExtensionsReloaded.Internal.Sql;
using DapperExtensionsReloaded.Mapper.Internal;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Predicates.Internal;
using Moq;
using Xunit;

namespace DapperExtensionsReloaded.Test
{
    public class FieldPredicateTests : PredicatesFixtureBase
    {
        [Fact]
        public void GetSql_NullValue_ReturnsProperSql()
        {
            SetupClassMapper();

            var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, null, false);
            var parameters = new Dictionary<string, object>();

            var sql = predicate.GetSql(parameters);

            Assert.Empty(parameters);
            Assert.Equal("(fooCol IS NULL)", sql);
        }

        [Fact]
        public void GetSql_NullValue_Not_ReturnsProperSql()
        {
            SetupClassMapper();

            var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, null, true);
            var parameters = new Dictionary<string, object>();

            var sql = predicate.GetSql(parameters);

            Assert.Empty(parameters);
            Assert.Equal("(fooCol IS NOT NULL)", sql);
        }

        [Fact]
        public void GetSql_Enumerable_NotEqOperator_ReturnsProperSql()
        {
            SetupClassMapper();

            var predicate = Setup<PredicateTestEntity>("Name", Operator.Le, new[] { "foo", "bar" }, false);
            var parameters = new Dictionary<string, object>();

            var ex = Assert.Throws<ArgumentException>(() => predicate.GetSql(parameters));

            Assert.StartsWith("Operator must be set to Eq for Enumerable types", ex.Message);
        }

        [Fact]
        public void GetSql_Enumerable_ReturnsProperSql()
        {
            SetupClassMapper();

            var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, new[] { "foo", "bar" }, false);
            var parameters = new Dictionary<string, object>();

            var sql = predicate.GetSql(parameters);

            Assert.Equal(2, parameters.Count);
            Assert.Equal("foo", parameters["@Name_0"]);
            Assert.Equal("bar", parameters["@Name_1"]);
            Assert.Equal("(fooCol IN (@Name_0, @Name_1))", sql);
        }

        [Fact]
        public void GetSql_Enumerable_Not_ReturnsProperSql()
        {
            SetupClassMapper();

            var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, new[] { "foo", "bar" }, true);
            var parameters = new Dictionary<string, object>();

            var sql = predicate.GetSql(parameters);

            Assert.Equal(2, parameters.Count);
            Assert.Equal("foo", parameters["@Name_0"]);
            Assert.Equal("bar", parameters["@Name_1"]);
            Assert.Equal("(fooCol NOT IN (@Name_0, @Name_1))", sql);
        }

        [Fact]
        public void GetSql_ReturnsProperSql()
        {
            SetupClassMapper();

            var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, 12, true);
            var parameters = new Dictionary<string, object>();

            var sql = predicate.GetSql(parameters);

            Assert.Single(parameters);
            Assert.Equal(12, parameters["@Name_0"]);
            Assert.Equal("(fooCol <> @Name_0)", sql);
        }

        internal FieldPredicate<T> Setup<T>(string propertyName, Operator op, object value, bool not) where T : class
        {
            var predicate = new FieldPredicate<T>(() => Generator.Object)
            {
                PropertyName = propertyName,
                Operator = op,
                Not = not,
                Value = value
            };
            return predicate;
        }

        private void SetupClassMapper()
        {
            var classMapper = new Mock<IClassMapper>();
            var predicate = new Mock<BasePredicate>(new Func<ISqlGenerator>(() => Generator.Object));
            var propertyMap = new Mock<IPropertyMap>();
            var propertyMaps = new List<IPropertyMap> { propertyMap.Object };
            predicate.CallBase = true;

            Configuration.Setup(c => c.GetMap(typeof(PredicateTestEntity))).Returns(classMapper.Object).Verifiable();
            classMapper.SetupGet(c => c.Properties).Returns(propertyMaps).Verifiable();
            propertyMap.SetupGet(p => p.Name).Returns("Name").Verifiable();
            Generator.Setup(g => g.GetColumnName(classMapper.Object, propertyMap.Object, false)).Returns("fooCol").Verifiable();
        }
    }
}