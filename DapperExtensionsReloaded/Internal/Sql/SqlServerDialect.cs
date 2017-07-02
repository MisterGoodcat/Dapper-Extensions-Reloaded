using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperExtensionsReloaded.Internal.Sql
{
    internal sealed class SqlServerDialect : SqlDialectBase
    {
        public override char OpenQuote => '[';

        public override char CloseQuote => ']';

        public override string GetIdentitySql(string tableName)
        {
            return "SELECT CAST(SCOPE_IDENTITY()  AS BIGINT) AS [Id]";
        }

        public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
        {
            var startValue = (page * resultsPerPage) + 1;
            return GetSetSql(sql, startValue, resultsPerPage, parameters);
        }

        public override string GetSetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException(nameof(sql));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var selectIndex = GetSelectEnd(sql) + 1;
            var orderByClause = GetOrderByClause(sql) ?? "ORDER BY CURRENT_TIMESTAMP";

            var projectedColumns = GetColumnNames(sql).Aggregate(new StringBuilder(), (sb, s) => (sb.Length == 0 ? sb : sb.Append(", ")).Append(GetColumnName("_proj", s, null)), sb => sb.ToString());
            var newSql = sql
                .Replace(" " + orderByClause, string.Empty)
                .Insert(selectIndex, $"ROW_NUMBER() OVER(ORDER BY {orderByClause.Substring(9)}) AS {GetColumnName(null, "_row_number", null)}, ");

            var result = string.Format("SELECT TOP({0}) {1} FROM ({2}) [_proj] WHERE {3} >= @_pageStartRow ORDER BY {3}",
                maxResults, projectedColumns.Trim(), newSql, GetColumnName("_proj", "_row_number", null));

            parameters.Add("@_pageStartRow", firstResult);
            return result;
        }

        private string GetOrderByClause(string sql)
        {
            var orderByIndex = sql.LastIndexOf(" ORDER BY ", StringComparison.OrdinalIgnoreCase);
            if (orderByIndex == -1)
            {
                return null;
            }

            var result = sql.Substring(orderByIndex).Trim();

            var whereIndex = result.IndexOf(" WHERE ", StringComparison.OrdinalIgnoreCase);
            if (whereIndex == -1)
            {
                return result;
            }

            return result.Substring(0, whereIndex).Trim();
        }

        private int GetFromStart(string sql)
        {
            var selectCount = 0;
            var words = sql.Split(' ');
            var fromIndex = 0;
            foreach (var word in words)
            {
                if (word.Equals("SELECT", StringComparison.OrdinalIgnoreCase))
                {
                    selectCount++;
                }

                if (word.Equals("FROM", StringComparison.OrdinalIgnoreCase))
                {
                    selectCount--;
                    if (selectCount == 0)
                    {
                        break;
                    }
                }

                fromIndex += word.Length + 1;
            }

            return fromIndex;
        }

        private int GetSelectEnd(string sql)
        {
            if (sql.StartsWith("SELECT DISTINCT", StringComparison.OrdinalIgnoreCase))
            {
                return 15;
            }

            if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                return 6;
            }

            throw new ArgumentException("SQL must be a SELECT statement.", nameof(sql));
        }

        private IList<string> GetColumnNames(string sql)
        {
            var start = GetSelectEnd(sql);
            var stop = GetFromStart(sql);
            var columnSql = sql.Substring(start, stop - start).Split(',');
            var result = new List<string>();
            foreach (var c in columnSql)
            {
                var index = c.IndexOf(" AS ", StringComparison.OrdinalIgnoreCase);
                if (index > 0)
                {
                    result.Add(c.Substring(index + 4).Trim());
                    continue;
                }

                var colParts = c.Split('.');
                result.Add(colParts[colParts.Length - 1].Trim());
            }

            return result;
        }
    }
}