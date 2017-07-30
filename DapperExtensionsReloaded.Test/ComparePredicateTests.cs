using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Predicates.Internal;
using Xunit;

namespace DapperExtensionsReloaded.Test
{
    public class ComparePredicateTests : PredicatesFixtureBase
    {
        [Fact]
        public void GetOperatorString_ReturnsOperatorStrings()
        {
            Assert.Equal("=", Setup(Operator.Eq, false).GetOperatorString());
            Assert.Equal("<>", Setup(Operator.Eq, true).GetOperatorString());
            Assert.Equal(">", Setup(Operator.Gt, false).GetOperatorString());
            Assert.Equal("<=", Setup(Operator.Gt, true).GetOperatorString());
            Assert.Equal(">=", Setup(Operator.Ge, false).GetOperatorString());
            Assert.Equal("<", Setup(Operator.Ge, true).GetOperatorString());
            Assert.Equal("<", Setup(Operator.Lt, false).GetOperatorString());
            Assert.Equal(">=", Setup(Operator.Lt, true).GetOperatorString());
            Assert.Equal("<=", Setup(Operator.Le, false).GetOperatorString());
            Assert.Equal(">", Setup(Operator.Le, true).GetOperatorString());
            Assert.Equal("LIKE", Setup(Operator.Like, false).GetOperatorString());
            Assert.Equal("NOT LIKE", Setup(Operator.Like, true).GetOperatorString());
        }

        internal ComparePredicate Setup(Operator op, bool not)
        {
            var predicate = new FieldPredicate<string>(() => Generator.Object)
            {
                Operator = op,
                Not = not
            };
            return predicate;
        }
    }
}