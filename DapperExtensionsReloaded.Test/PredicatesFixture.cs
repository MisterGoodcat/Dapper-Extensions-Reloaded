using System;
using System.Collections.Generic;
using DapperExtensions.Internal;
using DapperExtensions.Internal.Sql;
using DapperExtensions.Mapper;
using DapperExtensions.Mapper.Internal;
using DapperExtensions.Predicates;
using DapperExtensions.Predicates.Internal;
using DapperExtensions.Test.Helpers;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace DapperExtensions.Test
{
    [TestFixture]
    internal class PredicatesFixture
    {
        public abstract class PredicatesFixtureBase
        {
            protected Mock<ISqlDialect> SqlDialect;
            protected Mock<ISqlGenerator> Generator;
            protected Mock<IDapperExtensionsConfiguration> Configuration;

            [SetUp]
            public void Setup()
            {
                SqlDialect = new Mock<ISqlDialect>();
                Generator = new Mock<ISqlGenerator>();
                Configuration = new Mock<IDapperExtensionsConfiguration>();

                SqlDialect.SetupGet(c => c.ParameterPrefix).Returns('@');
                Configuration.SetupGet(c => c.Dialect).Returns(SqlDialect.Object);
                Generator.SetupGet(c => c.Configuration).Returns(Configuration.Object);
            }
        }

        [TestFixture]
        public class PredicatesTests : PredicatesFixtureBase
        {
            [Test]
            public void Field_ReturnsSetupPredicate()
            {
                var predicate = Predicates.Predicates.Field<PredicateTestEntity>(f => f.Name, Operator.Like, "Lead", true);
                Assert.AreEqual("Name", predicate.PropertyName);
                Assert.AreEqual(Operator.Like, predicate.Operator);
                Assert.AreEqual("Lead", predicate.Value);
                Assert.AreEqual(true, predicate.Not);
            }

            [Test]
            public void Property_ReturnsSetupPredicate()
            {
                var predicate = Predicates.Predicates.Property<PredicateTestEntity, PredicateTestEntity2>(f => f.Name, Operator.Le, f => f.Value, true);
                Assert.AreEqual("Name", predicate.PropertyName);
                Assert.AreEqual(Operator.Le, predicate.Operator);
                Assert.AreEqual("Value", predicate.PropertyName2);
                Assert.AreEqual(true, predicate.Not);
            }

            [Test]
            public void Group_ReturnsSetupPredicate()
            {
                var subPredicate = new Mock<IPredicate>();
                var predicate = Predicates.Predicates.Group(GroupOperator.Or, subPredicate.Object);
                Assert.AreEqual(GroupOperator.Or, predicate.Operator);
                Assert.AreEqual(1, predicate.Predicates.Count);
                Assert.AreEqual(subPredicate.Object, predicate.Predicates[0]);
            }

            [Test]
            public void Exists_ReturnsSetupPredicate()
            {
                var subPredicate = new Mock<IPredicate>();
                var predicate = Predicates.Predicates.Exists<PredicateTestEntity2>(subPredicate.Object, true);
                Assert.AreEqual(subPredicate.Object, predicate.Predicate);
                Assert.AreEqual(true, predicate.Not);
            }

            [Test]
            public void Between_ReturnsSetupPredicate()
            {
                var values = new BetweenValues();
                var predicate = Predicates.Predicates.Between<PredicateTestEntity>(f => f.Name, values, true);
                Assert.AreEqual("Name", predicate.PropertyName);
                Assert.AreEqual(values, predicate.Value);
                Assert.AreEqual(true, predicate.Not);
            }

            [Test]
            public void Sort__ReturnsSetupPredicate()
            {
                var predicate = Predicates.Predicates.Sort<PredicateTestEntity>(f => f.Name, false);
                Assert.AreEqual("Name", predicate.PropertyName);
                Assert.AreEqual(false, predicate.Ascending);
            }
        }

        [TestFixture]
        public class BasePredicateTests : PredicatesFixtureBase
        {
            [Test]
            public void GetColumnName_WhenMapNotFound_ThrowsException()
            {
                var predicate = new Mock<BasePredicate>(new Func<ISqlGenerator>(() => Generator.Object));
                predicate.CallBase = true;
                Configuration.Setup(c => c.GetMap(typeof(PredicateTestEntity))).Returns(() => null).Verifiable();

                var ex = Assert.Throws<NullReferenceException>(() => predicate.Object.TestProtected().RunMethod<string>("GetColumnName", typeof(PredicateTestEntity), "Name"));

                Configuration.Verify();

                StringAssert.StartsWith("No map found for", ex.Message);
            }

            [Test]
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

                StringAssert.StartsWith("Property Name was not found for type", ex.Message);
            }

            [Test]
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

                StringAssert.StartsWith("foo", result);
            }
        }

        [TestFixture]
        public class ComparePredicateTests : PredicatesFixtureBase
        {
            [Test]
            public void GetOperatorString_ReturnsOperatorStrings()
            {
                Assert.AreEqual("=", Setup(Operator.Eq, false).GetOperatorString());
                Assert.AreEqual("<>", Setup(Operator.Eq, true).GetOperatorString());
                Assert.AreEqual(">", Setup(Operator.Gt, false).GetOperatorString());
                Assert.AreEqual("<=", Setup(Operator.Gt, true).GetOperatorString());
                Assert.AreEqual(">=", Setup(Operator.Ge, false).GetOperatorString());
                Assert.AreEqual("<", Setup(Operator.Ge, true).GetOperatorString());
                Assert.AreEqual("<", Setup(Operator.Lt, false).GetOperatorString());
                Assert.AreEqual(">=", Setup(Operator.Lt, true).GetOperatorString());
                Assert.AreEqual("<=", Setup(Operator.Le, false).GetOperatorString());
                Assert.AreEqual(">", Setup(Operator.Le, true).GetOperatorString());
                Assert.AreEqual("LIKE", Setup(Operator.Like, false).GetOperatorString());
                Assert.AreEqual("NOT LIKE", Setup(Operator.Like, true).GetOperatorString());
            }

            protected ComparePredicate Setup(Operator op, bool not)
            {
                var predicate = new FieldPredicate<string>(() => Generator.Object);
                predicate.Operator = op;
                predicate.Not = not;
                return predicate;
            }
        }

        [TestFixture]
        public class FieldPredicateTests : PredicatesFixtureBase
        {
            [Test]
            public void GetSql_NullValue_ReturnsProperSql()
            {
                SetupClassMapper();

                var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, null, false);
                var parameters = new Dictionary<string, object>();

                var sql = predicate.GetSql(parameters);
                
                Assert.AreEqual(0, parameters.Count);
                Assert.AreEqual("(fooCol IS NULL)", sql);
            }

            [Test]
            public void GetSql_NullValue_Not_ReturnsProperSql()
            {
                SetupClassMapper();

                var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, null, true);
                var parameters = new Dictionary<string, object>();

                var sql = predicate.GetSql(parameters);
                
                Assert.AreEqual(0, parameters.Count);
                Assert.AreEqual("(fooCol IS NOT NULL)", sql);
            }

            [Test]
            public void GetSql_Enumerable_NotEqOperator_ReturnsProperSql()
            {
                SetupClassMapper();

                var predicate = Setup<PredicateTestEntity>("Name", Operator.Le, new[] { "foo", "bar" }, false);
                var parameters = new Dictionary<string, object>();

                var ex = Assert.Throws<ArgumentException>(() => predicate.GetSql(parameters));
                
                StringAssert.StartsWith("Operator must be set to Eq for Enumerable types", ex.Message);
            }

            [Test]
            public void GetSql_Enumerable_ReturnsProperSql()
            {
                SetupClassMapper();

                var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, new[] { "foo", "bar" }, false);
                var parameters = new Dictionary<string, object>();

                var sql = predicate.GetSql(parameters);
                
                Assert.AreEqual(2, parameters.Count);
                Assert.AreEqual("foo", parameters["@Name_0"]);
                Assert.AreEqual("bar", parameters["@Name_1"]);
                Assert.AreEqual("(fooCol IN (@Name_0, @Name_1))", sql);
            }

            [Test]
            public void GetSql_Enumerable_Not_ReturnsProperSql()
            {
                SetupClassMapper();

                var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, new[] { "foo", "bar" }, true);
                var parameters = new Dictionary<string, object>();

                var sql = predicate.GetSql(parameters);
                
                Assert.AreEqual(2, parameters.Count);
                Assert.AreEqual("foo", parameters["@Name_0"]);
                Assert.AreEqual("bar", parameters["@Name_1"]);
                Assert.AreEqual("(fooCol NOT IN (@Name_0, @Name_1))", sql);
            }

            [Test]
            public void GetSql_ReturnsProperSql()
            {
                SetupClassMapper();

                var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, 12, true);
                var parameters = new Dictionary<string, object>();

                var sql = predicate.GetSql(parameters);
                
                Assert.AreEqual(1, parameters.Count);
                Assert.AreEqual(12, parameters["@Name_0"]);
                Assert.AreEqual("(fooCol <> @Name_0)", sql);
            }

            protected FieldPredicate<T> Setup<T>(string propertyName, Operator op, object value, bool not) where T : class
            {
                var predicate = new FieldPredicate<T>(() => Generator.Object);
                predicate.PropertyName = propertyName;
                predicate.Operator = op;
                predicate.Not = not;
                predicate.Value = value;
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

        [TestFixture]
        public class PropertyPredicateTests : PredicatesFixtureBase
        {
            [Test]
            public void GetSql_ReturnsProperSql()
            {
                SetupClassMapper();

                var predicate = Setup<PredicateTestEntity, PredicateTestEntity2>("Name", Operator.Eq, "Value", false);
                var parameters = new Dictionary<string, object>();

                var sql = predicate.GetSql(parameters);
                
                Assert.AreEqual(0, parameters.Count);
                Assert.AreEqual("(fooCol = fooCol2)", sql);
            }

            protected PropertyPredicate<T, T2> Setup<T, T2>(string propertyName, Operator op, string propertyName2, bool not) where T : class where T2 : class
            {
                var predicate = new PropertyPredicate<T, T2>(() => Generator.Object);
                predicate.PropertyName = propertyName;
                predicate.PropertyName2 = propertyName2;
                predicate.Operator = op;
                predicate.Not = not;
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

        [TestFixture]
        public class BetweenPredicateTests : PredicatesFixtureBase
        {
            [Test]
            public void GetSql_ReturnsProperSql()
            {
                SetupClassMapper();
                
                var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, 12, 20, false);
                var parameters = new Dictionary<string, object>();

                var sql = predicate.GetSql(parameters);

                Assert.AreEqual(2, parameters.Count);
                Assert.AreEqual(12, parameters["@Name_0"]);
                Assert.AreEqual(20, parameters["@Name_1"]);
                Assert.AreEqual("(fooCol BETWEEN @Name_0 AND @Name_1)", sql);
            }

            [Test]
            public void GetSql_Not_ReturnsProperSql()
            {
                SetupClassMapper();

                var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, 12, 20, true);
                var parameters = new Dictionary<string, object>();

                var sql = predicate.GetSql(parameters);
                
                Assert.AreEqual(2, parameters.Count);
                Assert.AreEqual(12, parameters["@Name_0"]);
                Assert.AreEqual(20, parameters["@Name_1"]);
                Assert.AreEqual("(fooCol NOT BETWEEN @Name_0 AND @Name_1)", sql);
            }

            protected BetweenPredicate<T> Setup<T>(string propertyName, Operator op, object value1, object value2, bool not) where T : class
            {
                var predicate = new BetweenPredicate<T>((() => Generator.Object));
                predicate.PropertyName = propertyName;
                predicate.Value = new BetweenValues { Value1 = value1, Value2 = value2 };
                predicate.Not = not;
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

        [TestFixture]
        public class PredicateGroupTests : PredicatesFixtureBase
        {
            [Test]
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

                Assert.AreEqual(0, parameters.Count);
                Assert.AreEqual("(1=1)", sql);
            }

            [Test]
            public void GetSql_And_ReturnsProperSql()
            {
                var subPredicate1 = new Mock<IPredicate>();
                var subPredicates = new List<IPredicate> { subPredicate1.Object, subPredicate1.Object };
                var predicate = Setup(GroupOperator.And, subPredicates);
                var parameters = new Dictionary<string, object>();

                subPredicate1.Setup(s => s.GetSql(parameters)).Returns("subSql").Verifiable();
                var sql = predicate.GetSql(parameters);
                
                subPredicate1.Verify(s => s.GetSql(parameters), Times.AtMost(2));

                Assert.AreEqual(0, parameters.Count);
                Assert.AreEqual("(subSql AND subSql)", sql);
            }

            [Test]
            public void GetSql_Or_ReturnsProperSql()
            {
                var subPredicate1 = new Mock<IPredicate>();
                var subPredicates = new List<IPredicate> { subPredicate1.Object, subPredicate1.Object };
                var predicate = Setup(GroupOperator.Or, subPredicates);
                var parameters = new Dictionary<string, object>();

                subPredicate1.Setup(s => s.GetSql(parameters)).Returns("subSql").Verifiable();
                var sql = predicate.GetSql(parameters);
                
                subPredicate1.Verify(s => s.GetSql(parameters), Times.AtMost(2));

                Assert.AreEqual(0, parameters.Count);
                Assert.AreEqual("(subSql OR subSql)", sql);
            }

            protected PredicateGroup Setup(GroupOperator op, IList<IPredicate> predicates)
            {
                var predicate = new PredicateGroup(() => Generator.Object);
                predicate.Operator = op;
                predicate.Predicates = predicates;
                return predicate;
            }
        }

        [TestFixture]
        public class ExistsPredicateTests : PredicatesFixtureBase
        {
            [Test]
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

                Assert.AreEqual(0, parameters.Count);
                Assert.AreEqual("(EXISTS (SELECT 1 FROM subTable WHERE subSql))", sql);
            }

            [Test]
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

                Assert.AreEqual(0, parameters.Count);
                Assert.AreEqual("(NOT EXISTS (SELECT 1 FROM subTable WHERE subSql))", sql);
            }

            [Test]
            public void GetClassMapper_NoMapFound_ThrowsException()
            {
                var predicate = new ExistsPredicate<PredicateTestEntity>(() => Generator.Object);

                Configuration.Setup(c => c.GetMap(typeof(PredicateTestEntity2))).Returns(() => null).Verifiable();

                var ex = Assert.Throws<NullReferenceException>(() => predicate.TestProtected().RunMethod<IClassMapper>("GetClassMapper", typeof(PredicateTestEntity2), Configuration.Object));

                Configuration.Verify();

                StringAssert.StartsWith("Map was not found", ex.Message);
            }

            [Test]
            public void GetClassMapper_ReturnsMap()
            {
                var classMap = new Mock<IClassMapper>();
                var predicate = new ExistsPredicate<PredicateTestEntity>(() => Generator.Object);

                Configuration.Setup(c => c.GetMap(typeof(PredicateTestEntity2))).Returns(classMap.Object).Verifiable();

                var result = predicate.TestProtected().RunMethod<IClassMapper>("GetClassMapper", typeof(PredicateTestEntity2), Configuration.Object);

                Configuration.Verify();

                Assert.AreEqual(classMap.Object, result);
            }

            protected ExistsPredicate<T> Setup<T>(IPredicate predicate, bool not) where T : class
            {
                var result = new ExistsPredicate<T>(() => Generator.Object);
                result.Predicate = predicate;
                result.Not = not;
                return result;
            }
        }

        public class PredicateTestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class PredicateTestEntity2
        {
            public int Key { get; set; }
            public string Value { get; set; }
        }
    }
}