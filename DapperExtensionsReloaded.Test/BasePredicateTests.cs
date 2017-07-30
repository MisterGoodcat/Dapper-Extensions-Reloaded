using System;
using System.Collections.Generic;
using DapperExtensionsReloaded.Internal.Sql;
using DapperExtensionsReloaded.Mapper.Internal;
using DapperExtensionsReloaded.Predicates.Internal;
using DapperExtensionsReloaded.Test.Helpers;
using Moq;
using Xunit;

namespace DapperExtensionsReloaded.Test
{
    public class BasePredicateTests : PredicatesFixtureBase
    {
        [Fact]
        public void GetColumnName_WhenMapNotFound_ThrowsException()
        {
            var predicate = new Mock<BasePredicate>(new Func<ISqlGenerator>(() => Generator.Object)) { CallBase = true };
            Configuration.Setup(c => c.GetMap(typeof(PredicateTestEntity))).Returns(() => null).Verifiable();

            var ex = Assert.Throws<NullReferenceException>(() => predicate.Object.TestProtected().RunMethod<string>("GetColumnName", typeof(PredicateTestEntity), "Name"));

            Configuration.Verify();

            Assert.StartsWith("No map found for", ex.Message);
        }

        [Fact]
        public void GetColumnName_WhenPropertyNotFound_ThrowsException()
        {
            var classMapper = new Mock<IClassMapper>();
            var predicate = new Mock<BasePredicate>(new Func<ISqlGenerator>(() => Generator.Object));
            var propertyMaps = new List<IPropertyMap>();
            predicate.CallBase = true;

            Configuration.Setup(c => c.GetMap(typeof(PredicateTestEntity))).Returns(classMapper.Object).Verifiable();
            classMapper.SetupGet(c => c.Properties).Returns(propertyMaps).Verifiable();

            var ex = Assert.Throws<NullReferenceException>(() => predicate.Object.TestProtected().RunMethod<string>("GetColumnName", typeof(PredicateTestEntity), "Name"));

            Configuration.Verify();
            classMapper.Verify();

            Assert.StartsWith("Property Name was not found for type", ex.Message);
        }

        [Fact]
        public void GetColumnName_GetsColumnName()
        {
            var classMapper = new Mock<IClassMapper>();
            var predicate = new Mock<BasePredicate>(new Func<ISqlGenerator>(() => Generator.Object));
            var propertyMap = new Mock<IPropertyMap>();
            var propertyMaps = new List<IPropertyMap> { propertyMap.Object };
            predicate.CallBase = true;

            Configuration.Setup(c => c.GetMap(typeof(PredicateTestEntity))).Returns(classMapper.Object).Verifiable();
            classMapper.SetupGet(c => c.Properties).Returns(propertyMaps).Verifiable();
            propertyMap.SetupGet(p => p.Name).Returns("Name").Verifiable();
            Generator.Setup(g => g.GetColumnName(classMapper.Object, propertyMap.Object, false)).Returns("foo").Verifiable();

            var result = predicate.Object.TestProtected().RunMethod<string>("GetColumnName", typeof(PredicateTestEntity), "Name");

            Configuration.Verify();
            classMapper.Verify();
            propertyMap.Verify();
            Generator.Verify();

            Assert.StartsWith("foo", result);
        }
    }
}