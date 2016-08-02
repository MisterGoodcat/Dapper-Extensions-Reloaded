using System;
using System.Collections.Generic;
using System.Text;

namespace DapperExtensions.Sql
{
    public class SqlCeDialect : SqlDialectBase
    {
        public override char OpenQuote => '[';

        public override char CloseQuote => ']';

        public override bool SupportsMultipleStatements => false;

        public override string GetTableName(string schemaName, string tableName, string alias)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            var result = new StringBuilder();
            result.Append(OpenQuote);
            if (!string.IsNullOrWhiteSpace(schemaName))
            {
                result.AppendFormat("{0}_", schemaName);
            }

            result.AppendFormat("{0}{1}", tableName, CloseQuote);


            if (!string.IsNullOrWhiteSpace(alias))
            {
                result.AppendFormat(" AS {0}{1}{2}", OpenQuote, alias, CloseQuote);
            }

            return result.ToString();
        }

        public override string GetIdentitySql(string tableName)
        {
            return "SELECT CAST(@@IDENTITY AS BIGINT) AS [Id]";
        }

        public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
        {
            var startValue = (page * resultsPerPage);
            return GetSetSql(sql, startValue, resultsPerPage, parameters);
        }

        public override string GetSetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters)
        {
            var result = $"{sql} OFFSET @firstResult ROWS FETCH NEXT @maxResults ROWS ONLY";
            parameters.Add("@firstResult", firstResult);
            parameters.Add("@maxResults", maxResults);
            return result;
        }
    }
}