using System;
using System.Collections.Generic;
using DapperExtensionsReloaded.Mapper.Internal;
using DapperExtensionsReloaded.Predicates;
using Moq;
using Xunit;

namespace DapperExtensionsReloaded.Test.Sql
{
    public class SqlGeneratorUpdateMethodTests : SqlGeneratorFixtureBase
    {
        [Fact]
        public void WithNullPredicate_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Generator.Object.Update(ClassMap.Object, null, new Dictionary<string, object>()));
            Assert.Equal("predicate", ex.ParamName);
        }

        [Fact]
        public void WithNullParameters_ThrowsException()
        {
            var predicate = new Mock<IPredicate>();
            var ex = Assert.Throws<ArgumentNullException>(() => Generator.Object.Update(ClassMap.Object, predicate.Object, null));
            Assert.Equal("parameters", ex.ParamName);
        }

        [Fact]
        public void WithNoMappedColumns_Throws_Exception()
        {
            var property1 = new Mock<IPropertyMap>();
            property1.Setup(p => p.KeyType).Returns(KeyType.Identity).Verifiable();

            var property2 = new Mock<IPropertyMap>();
            property2.Setup(p => p.IsReadOnly).Returns(true).Verifiable();

            var properties = new List<IPropertyMap>
            {
                property1.Object,
                property2.Object
            };


            ClassMap.SetupGet(c => c.Properties).Returns(properties).Verifiable();
            var predicate = new Mock<IPredicate>();
            var parameters = new Dictionary<string, object>();

            var ex = Assert.Throws<ArgumentException>(() => Generator.Object.Update(ClassMap.Object, predicate.Object, parameters));

            Assert.True(ex.Message.Contains("columns were mapped"));
            ClassMap.Verify();
            property1.Verify();
            property2.Verify();
        }

        [Fact]
        public void DoesNotGenerateIdentityColumns()
        {
            var property1 = new Mock<IPropertyMap>();
            property1.Setup(p => p.KeyType).Returns(KeyType.Identity).Verifiable();

            var property2 = new Mock<IPropertyMap>();
            property2.Setup(p => p.KeyType).Returns(KeyType.NotAKey).Verifiable();
            property2.Setup(p => p.Name).Returns("Name").Verifiable();

            var properties = new List<IPropertyMap>
            {
                property1.Object,
                property2.Object
            };

            ClassMap.SetupGet(c => c.Properties).Returns(properties).Verifiable();

            Generator.Setup(g => g.GetColumnName(ClassMap.Object, property2.Object, false)).Returns("Column").Verifiable();
            Generator.Setup(g => g.GetTableName(ClassMap.Object)).Returns("TableName").Verifiable();

            Dialect.SetupGet(d => d.SupportsMultipleStatements).Returns(false).Verifiable();

            var predicate = new Mock<IPredicate>();
            var parameters = new Dictionary<string, object>();
            predicate.Setup(p => p.GetSql(parameters)).Returns("Predicate").Verifiable();

            var result = Generator.Object.Update(ClassMap.Object, predicate.Object, parameters);

            Assert.Equal((string)"UPDATE TableName SET Column = @Name WHERE Predicate", (string)result);

            predicate.Verify();
            ClassMap.Verify();
            property1.Verify();
            property1.VerifyGet(p => p.Name, Times.Never());
            property2.Verify();

            Generator.Verify();
            Generator.Verify(g => g.GetColumnName(ClassMap.Object, property1.Object, false), Times.Never());
        }

        [Fact]
        public void DoesNotGenerateIgnoredColumns()
        {
            var property1 = new Mock<IPropertyMap>();
            property1.Setup(p => p.Ignored).Returns(true).Verifiable();

            var property2 = new Mock<IPropertyMap>();
            property2.Setup(p => p.KeyType).Returns(KeyType.NotAKey).Verifiable();
            property2.Setup(p => p.Name).Returns("Name").Verifiable();

            var properties = new List<IPropertyMap>
            {
                property1.Object,
                property2.Object
            };

            ClassMap.SetupGet(c => c.Properties).Returns(properties).Verifiable();

            Generator.Setup(g => g.GetColumnName(ClassMap.Object, property2.Object, false)).Returns("Column").Verifiable();
            Generator.Setup(g => g.GetTableName(ClassMap.Object)).Returns("TableName").Verifiable();

            Dialect.SetupGet(d => d.SupportsMultipleStatements).Returns(false).Verifiable();

            var predicate = new Mock<IPredicate>();
            var parameters = new Dictionary<string, object>();
            predicate.Setup(p => p.GetSql(parameters)).Returns("Predicate").Verifiable();

            var result = Generator.Object.Update(ClassMap.Object, predicate.Object, parameters);

            Assert.Equal((string)"UPDATE TableName SET Column = @Name WHERE Predicate", (string)result);

            predicate.Verify();
            ClassMap.Verify();
            property1.Verify();
            property1.VerifyGet(p => p.Name, Times.Never());
            property2.Verify();

            Generator.Verify();
            Generator.Verify(g => g.GetColumnName(ClassMap.Object, property1.Object, false), Times.Never());
        }

        [Fact]
        public void DoesNotGenerateReadonlyColumns()
        {
            var property1 = new Mock<IPropertyMap>();
            property1.Setup(p => p.IsReadOnly).Returns(true).Verifiable();

            var property2 = new Mock<IPropertyMap>();
            property2.Setup(p => p.KeyType).Returns(KeyType.NotAKey).Verifiable();
            property2.Setup(p => p.Name).Returns("Name").Verifiable();

            var properties = new List<IPropertyMap>
            {
                property1.Object,
                property2.Object
            };

            ClassMap.SetupGet(c => c.Properties).Returns(properties).Verifiable();

            Generator.Setup(g => g.GetColumnName(ClassMap.Object, property2.Object, false)).Returns("Column").Verifiable();
            Generator.Setup(g => g.GetTableName(ClassMap.Object)).Returns("TableName").Verifiable();

            Dialect.SetupGet(d => d.SupportsMultipleStatements).Returns(false).Verifiable();

            var predicate = new Mock<IPredicate>();
            var parameters = new Dictionary<string, object>();
            predicate.Setup(p => p.GetSql(parameters)).Returns("Predicate").Verifiable();

            var result = Generator.Object.Update(ClassMap.Object, predicate.Object, parameters);

            Assert.Equal((string)"UPDATE TableName SET Column = @Name WHERE Predicate", (string)result);

            predicate.Verify();
            ClassMap.Verify();
            property1.Verify();
            property1.VerifyGet(p => p.Name, Times.Never());
            property2.Verify();

            Generator.Verify();
            Generator.Verify(g => g.GetColumnName(ClassMap.Object, property1.Object, false), Times.Never());
        }
    }
}