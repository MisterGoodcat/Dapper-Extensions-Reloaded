using Xunit;

namespace DapperExtensionsReloaded.Test.Sql
{
    public class SqlGeneratorIdentitySqlMethodTests : SqlGeneratorFixtureBase
    {
        [Fact]
        public void CallsDialect()
        {
            Dialect.Setup(d => d.GetIdentitySql("TableName")).Returns("IdentitySql").Verifiable();
            Generator.Setup(g => g.GetTableName(ClassMap.Object)).Returns("TableName").Verifiable();
            var result = Generator.Object.IdentitySql(ClassMap.Object);
            Assert.Equal((string)"IdentitySql", (string)result);
            Generator.Verify();
            Dialect.Verify();
        }
    }
}