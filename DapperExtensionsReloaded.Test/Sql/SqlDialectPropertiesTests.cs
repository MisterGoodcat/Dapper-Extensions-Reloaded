using System;
using Xunit;

namespace DapperExtensionsReloaded.Test.Sql
{
    public class SqlDialectPropertiesTests : SqlDialectBaseFixtureBase
    {
        [Fact]
        public void CheckSettings()
        {
            Assert.Equal('"', Dialect.OpenQuote);
            Assert.Equal('"', Dialect.CloseQuote);
            Assert.Equal(";" + Environment.NewLine, (string)Dialect.BatchSeperator);
            Assert.Equal('@', Dialect.ParameterPrefix);
            Assert.True((bool)Dialect.SupportsMultipleStatements);
        }
    }
}