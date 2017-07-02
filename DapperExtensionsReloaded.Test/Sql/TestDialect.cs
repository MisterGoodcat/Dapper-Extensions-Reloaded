using System;
using System.Collections.Generic;
using DapperExtensionsReloaded.Internal.Sql;

namespace DapperExtensionsReloaded.Test.Sql
{
    internal class TestDialect : SqlDialectBase
    {
        public override string GetIdentitySql(string tableName)
        {
            throw new NotImplementedException();
        }

        public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public override string GetSetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }
    }
}