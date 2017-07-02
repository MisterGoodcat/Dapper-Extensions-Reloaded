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
            Assert.Equal((string)"=", (string)Setup(Operator.Eq, false).GetOperatorString());
            Assert.Equal((string)"<>", (string)Setup(Operator.Eq, true).GetOperatorString());
            Assert.Equal((string)">", (string)Setup(Operator.Gt, false).GetOperatorString());
            Assert.Equal((string)"<=", (string)Setup(Operator.Gt, true).GetOperatorString());
            Assert.Equal((string)">=", (string)Setup(Operator.Ge, false).GetOperatorString());
            Assert.Equal((string)"<", (string)Setup(Operator.Ge, true).GetOperatorString());
            Assert.Equal((string)"<", (string)Setup(Operator.Lt, false).GetOperatorString());
            Assert.Equal((string)">=", (string)Setup(Operator.Lt, true).GetOperatorString());
            Assert.Equal((string)"<=", (string)Setup(Operator.Le, false).GetOperatorString());
            Assert.Equal((string)">", (string)Setup(Operator.Le, true).GetOperatorString());
            Assert.Equal((string)"LIKE", (string)Setup(Operator.Like, false).GetOperatorString());
            Assert.Equal((string)"NOT LIKE", (string)Setup(Operator.Like, true).GetOperatorString());
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