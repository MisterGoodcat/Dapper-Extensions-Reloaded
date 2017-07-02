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
            Assert.Equal((string)"\"foo\"", (string)result);
        }

        [Fact]
        public void PrefixColumnName_ReturnsProperlyQuoted()
        {
            var result = Dialect.GetColumnName("bar", "foo", null);
            Assert.Equal((string)"\"bar\".\"foo\"", (string)result);
        }

        [Fact]
        public void AllParams_ReturnsProperlyQuoted()
        {
            var result = Dialect.GetColumnName("bar", "foo", "al");
            Assert.Equal((string)"\"bar\".\"foo\" AS \"al\"", (string)result);
        }

        [Fact]
        public void ContainsQuotes_DoesNotAddExtraQuotes()
        {
            var result = Dialect.GetColumnName("\"bar\"", "\"foo\"", "\"al\"");
            Assert.Equal((string)"\"bar\".\"foo\" AS \"al\"", (string)result);
        }
    }
}