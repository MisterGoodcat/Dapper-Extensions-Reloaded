using System;
using System.Collections.Generic;
using DapperExtensionsReloaded.Mapper.Internal;
using Moq;
using Xunit;

namespace DapperExtensionsReloaded.Test.Sql
{
    public class SqlGeneratorInsertMethodTests : SqlGeneratorFixtureBase
    {
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

            var ex = Assert.Throws<ArgumentException>(() => Generator.Object.Insert(ClassMap.Object));

            Assert.Contains("columns were mapped", ex.Message);
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

            var result = Generator.Object.Insert(ClassMap.Object);
            Assert.Equal("INSERT INTO TableName (Column) VALUES (@Name)", result);

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

            var result = Generator.Object.Insert(ClassMap.Object);
            Assert.Equal("INSERT INTO TableName (Column) VALUES (@Name)", result);

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

            var result = Generator.Object.Insert(ClassMap.Object);
            Assert.Equal("INSERT INTO TableName (Column) VALUES (@Name)", result);

            ClassMap.Verify();
            property1.Verify();
            property1.VerifyGet(p => p.Name, Times.Never());
            property2.Verify();

            Generator.Verify();
            Generator.Verify(g => g.GetColumnName(ClassMap.Object, property1.Object, false), Times.Never());
        }
    }
}