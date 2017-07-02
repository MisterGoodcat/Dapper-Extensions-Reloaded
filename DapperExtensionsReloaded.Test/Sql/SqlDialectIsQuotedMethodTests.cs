using Xunit;

namespace DapperExtensionsReloaded.Test.Sql
{
    public class SqlDialectIsQuotedMethodTests : SqlDialectBaseFixtureBase
    {
        [Fact]
        public void WithQuotes_ReturnsTrue()
        {
            Assert.True((bool)Dialect.IsQuoted("\"foo\""));
        }

        [Fact]
        public void WithOnlyStartQuotes_ReturnsFalse()
        {
            Assert.False((bool)Dialect.IsQuoted("\"foo"));
        }

        [Fact]
        public void WithOnlyCloseQuotes_ReturnsFalse()
        {
            Assert.False((bool)Dialect.IsQuoted("foo\""));
        }
    }
}