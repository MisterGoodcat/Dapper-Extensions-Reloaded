using System;
using Xunit;

namespace DapperExtensionsReloaded.Test.Sql
{
    public class SqlDialectGetTableNameMethodTests : SqlDialectBaseFixtureBase
    {
        [Fact]
        public void NullTableName_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Dialect.GetTableName(null, null, null));
            Assert.Equal("tableName", ex.ParamName);
        }

        [Fact]
        public void EmptyTableName_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Dialect.GetTableName(null, string.Empty, null));
            Assert.Equal("tableName", ex.ParamName);
        }

        [Fact]
        public void TableNameOnly_ReturnsProperlyQuoted()
        {
            var result = Dialect.GetTableName(null, "foo", null);
            Assert.Equal("\"foo\"", result);
        }

        [Fact]
        public void SchemaAndTable_ReturnsProperlyQuoted()
        {
            var result = Dialect.GetTableName("bar", "foo", null);
            Assert.Equal("\"bar\".\"foo\"", result);
        }

        [Fact]
        public void AllParams_ReturnsProperlyQuoted()
        {
            var result = Dialect.GetTableName("bar", "foo", "al");
            Assert.Equal("\"bar\".\"foo\" AS \"al\"", result);
        }

        [Fact]
        public void ContainsQuotes_DoesNotAddExtraQuotes()
        {
            var result = Dialect.GetTableName("\"bar\"", "\"foo\"", "\"al\"");
            Assert.Equal("\"bar\".\"foo\" AS \"al\"", result);
        }
    }
}