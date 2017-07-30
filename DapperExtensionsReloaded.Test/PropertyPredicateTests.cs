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
    public class PropertyPredicateTests : PredicatesFixtureBase
    {
        [Fact]
        public void GetSql_ReturnsProperSql()
        {
            SetupClassMapper();

            var predicate = Setup<PredicateTestEntity, PredicateTestEntity2>("Name", Operator.Eq, "Value", false);
            var parameters = new Dictionary<string, object>();

            var sql = predicate.GetSql(parameters);

            Assert.Equal(0, parameters.Count);
            Assert.Equal("(fooCol = fooCol2)", sql);
        }

        internal PropertyPredicate<T, T2> Setup<T, T2>(string propertyName, Operator op, string propertyName2, bool not) where T : class where T2 : class
        {
            var predicate = new PropertyPredicate<T, T2>(() => Generator.Object)
            {
                PropertyName = propertyName,
                PropertyName2 = propertyName2,
                Operator = op,
                Not = not
            };
            return predicate;
        }

        private void SetupClassMapper()
        {
            var classMapper = new Mock<IClassMapper>();
            var classMapper2 = new Mock<IClassMapper>();
            var predicate = new Mock<BasePredicate>(new Func<ISqlGenerator>(() => Generator.Object));
            var propertyMap = new Mock<IPropertyMap>();
            var propertyMap2 = new Mock<IPropertyMap>();
            var propertyMaps = new List<IPropertyMap> { propertyMap.Object };
            var propertyMaps2 = new List<IPropertyMap> { propertyMap2.Object };
            predicate.CallBase = true;

            Configuration.Setup(c => c.GetMap(typeof(PredicateTestEntity))).Returns(classMapper.Object).Verifiable();
            Configuration.Setup(c => c.GetMap(typeof(PredicateTestEntity2))).Returns(classMapper2.Object).Verifiable();
            classMapper.SetupGet(c => c.Properties).Returns(propertyMaps).Verifiable();
            classMapper2.SetupGet(c => c.Properties).Returns(propertyMaps2).Verifiable();
            propertyMap.SetupGet(p => p.Name).Returns("Name").Verifiable();
            propertyMap2.SetupGet(p => p.Name).Returns("Value").Verifiable();
            Generator.Setup(g => g.GetColumnName(classMapper.Object, propertyMap.Object, false)).Returns("fooCol").Verifiable();
            Generator.Setup(g => g.GetColumnName(classMapper2.Object, propertyMap2.Object, false)).Returns("fooCol2").Verifiable();
        }
    }
}