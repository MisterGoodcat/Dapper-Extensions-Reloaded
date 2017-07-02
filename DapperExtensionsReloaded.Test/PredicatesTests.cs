using DapperExtensionsReloaded.Predicates;
using Moq;
using Xunit;

namespace DapperExtensionsReloaded.Test
{
    public class PredicatesTests : PredicatesFixtureBase
    {
        [Fact]
        public void Field_ReturnsSetupPredicate()
        {
            var predicate = Predicates.Predicates.Field<PredicateTestEntity>(f => f.Name, Operator.Like, "Lead", true);
            Assert.Equal("Name", predicate.PropertyName);
            Assert.Equal(Operator.Like, predicate.Operator);
            Assert.Equal("Lead", predicate.Value);
            Assert.True(predicate.Not);
        }

        [Fact]
        public void Property_ReturnsSetupPredicate()
        {
            var predicate = Predicates.Predicates.Property<PredicateTestEntity, PredicateTestEntity2>(f => f.Name, Operator.Le, f => f.Value, true);
            Assert.Equal("Name", predicate.PropertyName);
            Assert.Equal(Operator.Le, predicate.Operator);
            Assert.Equal("Value", predicate.PropertyName2);
            Assert.True(predicate.Not);
        }

        [Fact]
        public void Group_ReturnsSetupPredicate()
        {
            var subPredicate = new Mock<IPredicate>();
            var predicate = Predicates.Predicates.Group(GroupOperator.Or, subPredicate.Object);
            Assert.Equal(GroupOperator.Or, predicate.Operator);
            Assert.Equal(1, predicate.Predicates.Count);
            Assert.Equal(subPredicate.Object, predicate.Predicates[0]);
        }

        [Fact]
        public void Exists_ReturnsSetupPredicate()
        {
            var subPredicate = new Mock<IPredicate>();
            var predicate = Predicates.Predicates.Exists<PredicateTestEntity2>(subPredicate.Object, true);
            Assert.Equal(subPredicate.Object, predicate.Predicate);
            Assert.True(predicate.Not);
        }

        [Fact]
        public void Between_ReturnsSetupPredicate()
        {
            var values = new BetweenValues();
            var predicate = Predicates.Predicates.Between<PredicateTestEntity>(f => f.Name, values, true);
            Assert.Equal("Name", predicate.PropertyName);
            Assert.Equal(values, predicate.Value);
            Assert.True(predicate.Not);
        }

        [Fact]
        public void Sort__ReturnsSetupPredicate()
        {
            var predicate = Predicates.Predicates.Sort<PredicateTestEntity>(f => f.Name, false);
            Assert.Equal("Name", predicate.PropertyName);
            Assert.False(predicate.Ascending);
        }
    }
}