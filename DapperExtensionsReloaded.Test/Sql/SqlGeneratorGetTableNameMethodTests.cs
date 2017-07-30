using DapperExtensionsReloaded.Mapper.Internal;
using Moq;
using Xunit;

namespace DapperExtensionsReloaded.Test.Sql
{
    public class SqlGeneratorGetTableNameMethodTests : SqlGeneratorFixtureBase
    {
        [Fact]
        public void CallsDialect()
        {
            ClassMap.SetupGet(c => c.SchemaName).Returns("SchemaName").Verifiable();
            ClassMap.SetupGet(c => c.TableName).Returns("TableName").Verifiable();
            Dialect.Setup(d => d.GetTableName("SchemaName", "TableName", null)).Returns("FullTableName").Verifiable();
            var result = Generator.Object.GetTableName(ClassMap.Object);
            Assert.Equal("FullTableName", result);
            Dialect.Verify();
            ClassMap.Verify();
        }

        [Fact]
        public void DoesNotIncludeAliasWhenParameterIsFalse()
        {
            var property = new Mock<IPropertyMap>();
            property.SetupGet(p => p.ColumnName).Returns("Column").Verifiable();
            property.SetupGet(p => p.Name).Returns("Name").Verifiable();

            Generator.Setup(g => g.GetTableName(ClassMap.Object)).Returns("TableName").Verifiable();
            Dialect.Setup(d => d.GetColumnName("TableName", "Column", null)).Returns("FullColumnName").Verifiable();
            var result = Generator.Object.GetColumnName(ClassMap.Object, property.Object, false);
            Assert.Equal("FullColumnName", result);
            property.Verify();
            Generator.Verify();
        }

        [Fact]
        public void DoesNotIncludeAliasWhenColumnAndNameAreSame()
        {
            var property = new Mock<IPropertyMap>();
            property.SetupGet(p => p.ColumnName).Returns("Column").Verifiable();
            property.SetupGet(p => p.Name).Returns("Column").Verifiable();

            Generator.Setup(g => g.GetTableName(ClassMap.Object)).Returns("TableName").Verifiable();
            Dialect.Setup(d => d.GetColumnName("TableName", "Column", null)).Returns("FullColumnName").Verifiable();
            var result = Generator.Object.GetColumnName(ClassMap.Object, property.Object, true);
            Assert.Equal("FullColumnName", result);
            property.Verify();
            Generator.Verify();
        }

        [Fact]
        public void IncludesAliasWhenColumnAndNameAreDifferent()
        {
            var property = new Mock<IPropertyMap>();
            property.SetupGet(p => p.ColumnName).Returns("Column").Verifiable();
            property.SetupGet(p => p.Name).Returns("Name").Verifiable();

            Generator.Setup(g => g.GetTableName(ClassMap.Object)).Returns("TableName").Verifiable();
            Dialect.Setup(d => d.GetColumnName("TableName", "Column", "Name")).Returns("FullColumnName").Verifiable();
            var result = Generator.Object.GetColumnName(ClassMap.Object, property.Object, true);
            Assert.Equal("FullColumnName", result);
            property.Verify();
            Generator.Verify();
        }
    }
}