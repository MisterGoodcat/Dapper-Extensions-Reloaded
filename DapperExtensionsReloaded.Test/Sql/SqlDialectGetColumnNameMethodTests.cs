using System;
using Xunit;

namespace DapperExtensionsReloaded.Test.Sql
{
    public class SqlDialectGetColumnNameMethodTests : SqlDialectBaseFixtureBase
    {
        [Fact]
        public void NullColumnName_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Dialect.GetColumnName(null, null, null));
            Assert.Equal("columnName", ex.ParamName);
        }

        [Fact]
        public void EmptyColumnName_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Dialect.GetColumnName(null, string.Empty, null));
            Assert.Equal("columnName", ex.ParamName);
        }

        [Fact]
        public void ColumnNameOnly_ReturnsProperlyQuoted()
        {
            var result = Dialect.GetColumnName(null, "foo", null);
            Assert.Equal("\"foo\"", result);
        }

        [Fact]
        public void PrefixColumnName_ReturnsProperlyQuoted()
        {
            var result = Dialect.GetColumnName("bar", "foo", null);
            Assert.Equal("\"bar\".\"foo\"", result);
        }

        [Fact]
        public void AllParams_ReturnsProperlyQuoted()
        {
            var result = Dialect.GetColumnName("bar", "foo", "al");
            Assert.Equal("\"bar\".\"foo\" AS \"al\"", result);
        }

        [Fact]
        public void ContainsQuotes_DoesNotAddExtraQuotes()
        {
            var result = Dialect.GetColumnName("\"bar\"", "\"foo\"", "\"al\"");
            Assert.Equal("\"bar\".\"foo\" AS \"al\"", result);
        }
    }
}