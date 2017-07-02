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
    public class BetweenPredicateTests : PredicatesFixtureBase
    {
        [Fact]
        public void GetSql_ReturnsProperSql()
        {
            SetupClassMapper();

            var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, 12, 20, false);
            var parameters = new Dictionary<string, object>();

            var sql = predicate.GetSql(parameters);

            Assert.Equal(2, parameters.Count);
            Assert.Equal(12, parameters["@Name_0"]);
            Assert.Equal(20, parameters["@Name_1"]);
            Assert.Equal((string)"(fooCol BETWEEN @Name_0 AND @Name_1)", (string)sql);
        }

        [Fact]
        public void GetSql_Not_ReturnsProperSql()
        {
            SetupClassMapper();

            var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, 12, 20, true);
            var parameters = new Dictionary<string, object>();

            var sql = predicate.GetSql(parameters);

            Assert.Equal(2, parameters.Count);
            Assert.Equal(12, parameters["@Name_0"]);
            Assert.Equal(20, parameters["@Name_1"]);
            Assert.Equal((string)"(fooCol NOT BETWEEN @Name_0 AND @Name_1)", (string)sql);
        }

        internal BetweenPredicate<T> Setup<T>(string propertyName, Operator op, object value1, object value2, bool not) where T : class
        {
            var predicate = new BetweenPredicate<T>((() => Generator.Object))
            {
                PropertyName = propertyName,
                Value = new BetweenValues { Value1 = value1, Value2 = value2 },
                Not = not
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