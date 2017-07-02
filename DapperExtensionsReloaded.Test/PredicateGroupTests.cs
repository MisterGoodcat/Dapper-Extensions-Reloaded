using System.Collections.Generic;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Predicates.Internal;
using Moq;
using Xunit;

namespace DapperExtensionsReloaded.Test
{
    public class PredicateGroupTests : PredicatesFixtureBase
    {
        [Fact]
        public void EmptyPredicate__CreatesNoOp_And_ReturnsProperSql()
        {
            var subPredicate1 = new Mock<IPredicate>();
            SqlDialect.SetupGet(s => s.EmptyExpression).Returns("1=1").Verifiable();

            var subPredicates = new List<IPredicate> { subPredicate1.Object, subPredicate1.Object };
            var predicate = Setup(GroupOperator.And, subPredicates);
            var parameters = new Dictionary<string, object>();

            subPredicate1.Setup(s => s.GetSql(parameters)).Returns("").Verifiable();
            var sql = predicate.GetSql(parameters);

            SqlDialect.Verify();
            subPredicate1.Verify(s => s.GetSql(parameters), Times.AtMost(2));

            Assert.Equal(0, parameters.Count);
            Assert.Equal((string)"(1=1)", (string)sql);
        }

        [Fact]
        public void GetSql_And_ReturnsProperSql()
        {
            var subPredicate1 = new Mock<IPredicate>();
            var subPredicates = new List<IPredicate> { subPredicate1.Object, subPredicate1.Object };
            var predicate = Setup(GroupOperator.And, subPredicates);
            var parameters = new Dictionary<string, object>();

            subPredicate1.Setup(s => s.GetSql(parameters)).Returns("subSql").Verifiable();
            var sql = predicate.GetSql(parameters);

            subPredicate1.Verify(s => s.GetSql(parameters), Times.AtMost(2));

            Assert.Equal(0, parameters.Count);
            Assert.Equal((string)"(subSql AND subSql)", (string)sql);
        }

        [Fact]
        public void GetSql_Or_ReturnsProperSql()
        {
            var subPredicate1 = new Mock<IPredicate>();
            var subPredicates = new List<IPredicate> { subPredicate1.Object, subPredicate1.Object };
            var predicate = Setup(GroupOperator.Or, subPredicates);
            var parameters = new Dictionary<string, object>();

            subPredicate1.Setup(s => s.GetSql(parameters)).Returns("subSql").Verifiable();
            var sql = predicate.GetSql(parameters);

            subPredicate1.Verify(s => s.GetSql(parameters), Times.AtMost(2));

            Assert.Equal(0, parameters.Count);
            Assert.Equal((string)"(subSql OR subSql)", (string)sql);
        }

        internal PredicateGroup Setup(GroupOperator op, IList<IPredicate> predicates)
        {
            var predicate = new PredicateGroup(() => Generator.Object)
            {
                Operator = op,
                Predicates = predicates
            };
            return predicate;
        }
    }
}