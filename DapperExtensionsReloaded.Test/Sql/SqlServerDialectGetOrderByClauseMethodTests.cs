using DapperExtensionsReloaded.Test.Helpers;
using Xunit;

namespace DapperExtensionsReloaded.Test.Sql
{
    public class SqlServerDialectGetOrderByClauseMethodTests : SqlServerDialectFixtureBase
    {
        [Fact]
        public void NoOrderBy_Returns()
        {
            var result = TestHelpers.TestProtected(Dialect).RunMethod<string>("GetOrderByClause", "SELECT * FROM Table");
            Assert.Null(result);
        }

        [Fact]
        public void OrderBy_ReturnsItemsAfterClause()
        {
            var result = TestHelpers.TestProtected(Dialect).RunMethod<string>("GetOrderByClause", "SELECT * FROM Table ORDER BY Column1 ASC, Column2 DESC");
            Assert.Equal("ORDER BY Column1 ASC, Column2 DESC", result);
        }

        [Fact]
        public void OrderByWithWhere_ReturnsOnlyOrderBy()
        {
            var result = TestHelpers.TestProtected(Dialect).RunMethod<string>("GetOrderByClause", "SELECT * FROM Table ORDER BY Column1 ASC, Column2 DESC WHERE Column1 = 'value'");
            Assert.Equal("ORDER BY Column1 ASC, Column2 DESC", result);
        }
    }
}