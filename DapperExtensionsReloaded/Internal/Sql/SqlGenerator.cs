using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DapperExtensionsReloaded.Mapper.Internal;
using DapperExtensionsReloaded.Predicates;

namespace DapperExtensionsReloaded.Internal.Sql
{
    internal class SqlGeneratorImpl : ISqlGenerator
    {
        public SqlGeneratorImpl(IDapperExtensionsConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IDapperExtensionsConfiguration Configuration { get; }

        public virtual string Select(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, IDictionary<string, object> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var sql = new StringBuilder($"SELECT {BuildSelectColumns(classMap)} FROM {GetTableName(classMap)}");
            if (predicate != null)
            {
                sql.Append(" WHERE ")
                   .Append(predicate.GetSql(parameters));
            }

            if (sort != null && sort.Any())
            {
                sql.Append(" ORDER BY ")
                   .Append(sort.Select(s => GetColumnName(classMap, s.PropertyName, false) + (s.Ascending ? " ASC" : " DESC")).AppendStrings());
            }

            return sql.ToString();
        }

        public virtual string SelectPaged(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int page, int resultsPerPage, IDictionary<string, object> parameters)
        {
            if (sort == null || !sort.Any())
            {
                throw new ArgumentNullException(nameof(sort));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var innerSql = new StringBuilder($"SELECT {BuildSelectColumns(classMap)} FROM {GetTableName(classMap)}");
            if (predicate != null)
            {
                innerSql.Append(" WHERE ")
                        .Append(predicate.GetSql(parameters));
            }

            var orderBy = sort.Select(s => GetColumnName(classMap, s.PropertyName, false) + (s.Ascending ? " ASC" : " DESC")).AppendStrings();
            innerSql.Append(" ORDER BY " + orderBy);

            var sql = Configuration.Dialect.GetPagingSql(innerSql.ToString(), page, resultsPerPage, parameters);
            return sql;
        }

        public virtual string SelectSet(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int firstResult, int maxResults, IDictionary<string, object> parameters)
        {
            if (sort == null || !sort.Any())
            {
                throw new ArgumentNullException(nameof(sort));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var innerSql = new StringBuilder($"SELECT {BuildSelectColumns(classMap)} FROM {GetTableName(classMap)}");
            if (predicate != null)
            {
                innerSql.Append(" WHERE ")
                        .Append(predicate.GetSql(parameters));
            }

            var orderBy = sort.Select(s => GetColumnName(classMap, s.PropertyName, false) + (s.Ascending ? " ASC" : " DESC")).AppendStrings();
            innerSql.Append(" ORDER BY " + orderBy);

            var sql = Configuration.Dialect.GetSetSql(innerSql.ToString(), firstResult, maxResults, parameters);
            return sql;
        }

        public virtual string Count(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var sql = new StringBuilder($"SELECT COUNT(*) AS {Configuration.Dialect.OpenQuote}Total{Configuration.Dialect.CloseQuote} FROM {GetTableName(classMap)}");
            if (predicate != null)
            {
                sql.Append(" WHERE ")
                   .Append(predicate.GetSql(parameters));
            }

            return sql.ToString();
        }

        public virtual string Insert(IClassMapper classMap)
        {
            var columns = classMap.Properties.Where(p => !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity)).ToList();
            if (!columns.Any())
            {
                throw new ArgumentException("No columns were mapped.");
            }

            var columnNames = columns.Select(p => GetColumnName(classMap, p, false));
            var parameters = columns.Select(p => Configuration.Dialect.ParameterPrefix + p.Name);

            var sql = $"INSERT INTO {GetTableName(classMap)} ({columnNames.AppendStrings()}) VALUES ({parameters.AppendStrings()})";

            return sql;
        }

        public virtual string Update(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var columns = classMap.Properties.Where(p => !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity)).ToList();
            if (!columns.Any())
            {
                throw new ArgumentException("No columns were mapped.");
            }

            var setSql = columns.Select(p => $"{GetColumnName(classMap, p, false)} = {Configuration.Dialect.ParameterPrefix}{p.Name}");

            return $"UPDATE {GetTableName(classMap)} SET {setSql.AppendStrings()} WHERE {predicate.GetSql(parameters)}";
        }

        public virtual string UpdateSet(IClassMapper classMap, IList<IPropertyMap> propertiesToUpdate, IPredicate wherePredicate, IDictionary<string, object> parameters)
        {
            if (propertiesToUpdate == null || !propertiesToUpdate.Any())
            {
                throw new ArgumentException(nameof(propertiesToUpdate));
            }

            if (wherePredicate == null)
            {
                throw new ArgumentNullException(nameof(wherePredicate));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (propertiesToUpdate.Any(p => p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity))
            {
                throw new ArgumentException("Ignored, read-only and identity properties cannot be updated.");
            }

            var setSql = propertiesToUpdate.Select(p => $"{GetColumnName(classMap, p, false)} = {Configuration.Dialect.ParameterPrefix}{p.Name}");

            return $"UPDATE {GetTableName(classMap)} SET {setSql.AppendStrings()} WHERE {wherePredicate.GetSql(parameters)}";
        }

        public virtual string Delete(IClassMapper classMap, IPredicate wherePredicate, IDictionary<string, object> parameters)
        {
            if (wherePredicate == null)
            {
                throw new ArgumentNullException(nameof(wherePredicate));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var sql = new StringBuilder($"DELETE FROM {GetTableName(classMap)}");
            sql.Append(" WHERE ").Append(wherePredicate.GetSql(parameters));
            return sql.ToString();
        }

        public virtual string IdentitySql(IClassMapper classMap)
        {
            return Configuration.Dialect.GetIdentitySql(GetTableName(classMap));
        }

        public virtual string GetTableName(IClassMapper map)
        {
            return Configuration.Dialect.GetTableName(map.SchemaName, map.TableName, null);
        }

        public virtual string GetColumnName(IClassMapper map, IPropertyMap property, bool includeAlias)
        {
            string alias = null;
            if (property.ColumnName != property.Name && includeAlias)
            {
                alias = property.Name;
            }

            return Configuration.Dialect.GetColumnName(GetTableName(map), property.ColumnName, alias);
        }

        public virtual string GetColumnName(IClassMapper map, string propertyName, bool includeAlias)
        {
            var propertyMap = map.Properties.SingleOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
            if (propertyMap == null)
            {
                throw new ArgumentException($"Could not find '{propertyName}' in Mapping.");
            }

            return GetColumnName(map, propertyMap, includeAlias);
        }

        public virtual bool SupportsMultipleStatements()
        {
            return Configuration.Dialect.SupportsMultipleStatements;
        }

        public virtual string BuildSelectColumns(IClassMapper classMap)
        {
            var columns = classMap.Properties
                                  .Where(p => !p.Ignored)
                                  .Select(p => GetColumnName(classMap, p, true));
            return columns.AppendStrings();
        }
    }
}