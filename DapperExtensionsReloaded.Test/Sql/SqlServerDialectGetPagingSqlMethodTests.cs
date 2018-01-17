using System;
using System.Collections.Generic;
using Xunit;

namespace DapperExtensionsReloaded.Test.Sql
{
    public class SqlServerDialectGetPagingSqlMethodTests : SqlServerDialectFixtureBase
    {
        [Fact]
        public void NullSql_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Dialect.GetPagingSql(null, 0, 10, new Dictionary<string, object>()));
            Assert.Equal("sql", ex.ParamName);
        }

        [Fact]
        public void EmptySql_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Dialect.GetPagingSql(string.Empty, 0, 10, new Dictionary<string, object>()));
            Assert.Equal("sql", ex.ParamName);
        }

        [Fact]
        public void NullParameters_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Dialect.GetPagingSql("SELECT [schema].[column] FROM [schema].[table]", 0, 10, null));
            Assert.Equal("parameters", ex.ParamName);
        }

        [Fact]
        public void Select_ReturnsSql()
        {
            var parameters = new Dictionary<string, object>();
            var sql = "SELECT TOP(10) [_proj].[column] FROM (SELECT ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) AS [_row_number], [column] FROM [schema].[table]) [_proj] WHERE [_proj].[_row_number] >= @_pageStartRow ORDER BY [_proj].[_row_number]";
            var result = Dialect.GetPagingSql("SELECT [column] FROM [schema].[table]", 0, 10, parameters);
            Assert.Equal(sql, result);
            Assert.Single(parameters);
            Assert.Equal(1, parameters["@_pageStartRow"]);
        }

        [Fact]
        public void SelectDistinct_ReturnsSql()
        {
            var parameters = new Dictionary<string, object>();
            var sql = "SELECT TOP(10) [_proj].[column] FROM (SELECT DISTINCT ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) AS [_row_number], [column] FROM [schema].[table]) [_proj] WHERE [_proj].[_row_number] >= @_pageStartRow ORDER BY [_proj].[_row_number]";
            var result = Dialect.GetPagingSql("SELECT DISTINCT [column] FROM [schema].[table]", 0, 10, parameters);
            Assert.Equal(sql, result);
            Assert.Single(parameters);
            Assert.Equal(1, parameters["@_pageStartRow"]);
        }

        [Fact]
        public void SelectOrderBy_ReturnsSql()
        {
            var parameters = new Dictionary<string, object>();
            var sql = "SELECT TOP(10) [_proj].[column] FROM (SELECT ROW_NUMBER() OVER(ORDER BY [column] DESC) AS [_row_number], [column] FROM [schema].[table]) [_proj] WHERE [_proj].[_row_number] >= @_pageStartRow ORDER BY [_proj].[_row_number]";
            var result = Dialect.GetPagingSql("SELECT [column] FROM [schema].[table] ORDER BY [column] DESC", 0, 10, parameters);
            Assert.Equal(sql, result);
            Assert.Single(parameters);
            Assert.Equal(1, parameters["@_pageStartRow"]);
        }
    }
}