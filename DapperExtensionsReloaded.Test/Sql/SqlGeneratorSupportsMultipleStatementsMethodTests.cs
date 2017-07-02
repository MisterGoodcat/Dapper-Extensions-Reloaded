using Xunit;

namespace DapperExtensionsReloaded.Test.Sql
{
    public class SqlGeneratorSupportsMultipleStatementsMethodTests : SqlGeneratorFixtureBase
    {
        [Fact]
        public void CallsDialect()
        {
            Dialect.SetupGet(d => d.SupportsMultipleStatements).Returns(true).Verifiable();
            var result = Generator.Object.SupportsMultipleStatements();
            Assert.True((bool)result);
            Dialect.Verify();
        }
    }
}