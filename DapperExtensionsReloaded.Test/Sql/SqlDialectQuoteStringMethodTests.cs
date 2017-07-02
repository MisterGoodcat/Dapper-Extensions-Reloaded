using Xunit;

namespace DapperExtensionsReloaded.Test.Sql
{
    public class SqlDialectQuoteStringMethodTests : SqlDialectBaseFixtureBase
    {
        [Fact]
        public void WithNoQuotes_AddsQuotes()
        {
            Assert.Equal((string)"\"foo\"", (string)Dialect.QuoteString("foo"));
        }

        [Fact]
        public void WithStartQuote_AddsQuotes()
        {
            Assert.Equal((string)"\"\"foo\"", (string)Dialect.QuoteString("\"foo"));
        }

        [Fact]
        public void WithCloseQuote_AddsQuotes()
        {
            Assert.Equal((string)"\"foo\"\"", (string)Dialect.QuoteString("foo\""));
        }

        [Fact]
        public void WithBothQuote_DoesNotAddQuotes()
        {
            Assert.Equal((string)"\"foo\"", (string)Dialect.QuoteString("\"foo\""));
        }
    }
}