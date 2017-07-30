using System;
using System.Collections.Generic;
using DapperExtensionsReloaded.Mapper.Internal;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Predicates.Internal;
using DapperExtensionsReloaded.Test.Helpers;
using Moq;
using Xunit;

namespace DapperExtensionsReloaded.Test
{
    public class ExistsPredicateTests : PredicatesFixtureBase
    {
        [Fact]
        public void GetSql_WithoutNot_ReturnsProperSql()
        {
            var subPredicate = new Mock<IPredicate>();
            var subMap = new Mock<IClassMapper>();
            Configuration.Setup(c => c.GetMap(typeof(PredicateTestEntity2))).Returns(() => subMap.Object).Verifiable();

            var predicate = Setup<PredicateTestEntity2>(subPredicate.Object, false);
            Generator.Setup(g => g.GetTableName(subMap.Object)).Returns("subTable").Verifiable();

            var parameters = new Dictionary<string, object>();

            subPredicate.Setup(s => s.GetSql(parameters)).Returns("subSql").Verifiable();
            var sql = predicate.GetSql(parameters);

            subPredicate.Verify();
            Generator.Verify();

            Assert.Equal(0, parameters.Count);
            Assert.Equal("(EXISTS (SELECT 1 FROM subTable WHERE subSql))", sql);
        }

        [Fact]
        public void GetSql_WithNot_ReturnsProperSql()
        {
            var subPredicate = new Mock<IPredicate>();
            var subMap = new Mock<IClassMapper>();
            Configuration.Setup(c => c.GetMap(typeof(PredicateTestEntity2))).Returns(() => subMap.Object).Verifiable();

            var predicate = Setup<PredicateTestEntity2>(subPredicate.Object, true);
            Generator.Setup(g => g.GetTableName(subMap.Object)).Returns("subTable").Verifiable();

            var parameters = new Dictionary<string, object>();

            subPredicate.Setup(s => s.GetSql(parameters)).Returns("subSql").Verifiable();
            var sql = predicate.GetSql(parameters);

            subPredicate.Verify();
            Generator.Verify();

            Assert.Equal(0, parameters.Count);
            Assert.Equal("(NOT EXISTS (SELECT 1 FROM subTable WHERE subSql))", sql);
        }

        [Fact]
        public void GetClassMapper_NoMapFound_ThrowsException()
        {
            var predicate = new ExistsPredicate<PredicateTestEntity>(() => Generator.Object);

            Configuration.Setup(c => c.GetMap(typeof(PredicateTestEntity2))).Returns(() => null).Verifiable();

            var ex = Assert.Throws<NullReferenceException>(() => predicate.TestProtected().RunMethod<IClassMapper>("GetClassMapper", typeof(PredicateTestEntity2), Configuration.Object));

            Configuration.Verify();

            Assert.StartsWith("Map was not found", ex.Message);
        }

        [Fact]
        public void GetClassMapper_ReturnsMap()
        {
            var classMap = new Mock<IClassMapper>();
            var predicate = new ExistsPredicate<PredicateTestEntity>(() => Generator.Object);

            Configuration.Setup(c => c.GetMap(typeof(PredicateTestEntity2))).Returns(classMap.Object).Verifiable();

            var result = predicate.TestProtected().RunMethod<IClassMapper>("GetClassMapper", typeof(PredicateTestEntity2), Configuration.Object);

            Configuration.Verify();

            Assert.Equal(classMap.Object, result);
        }

        internal ExistsPredicate<T> Setup<T>(IPredicate predicate, bool not) where T : class
        {
            var result = new ExistsPredicate<T>(() => Generator.Object)
            {
                Predicate = predicate,
                Not = not
            };
            return result;
        }
    }
}