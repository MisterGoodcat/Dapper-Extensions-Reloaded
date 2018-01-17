using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperExtensionsReloaded.Internal.Sql;
using DapperExtensionsReloaded.Mapper.Internal;
using DapperExtensionsReloaded.Predicates;
using DapperExtensionsReloaded.Predicates.Internal;

namespace DapperExtensionsReloaded.Internal
{
    internal class DapperImplementor : IDapperImplementor
    {
        public DapperImplementor(ISqlGenerator sqlGenerator)
        {
            SqlGenerator = sqlGenerator;
        }

        public ISqlGenerator SqlGenerator { get; }
        
        public async Task InsertAsync<T>(IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            var classMap = SqlGenerator.Configuration.GetMap<T>();
            var properties = classMap.Properties.Where(p => p.KeyType != KeyType.NotAKey).ToList();

            foreach (var e in entities)
            {
                foreach (var column in properties)
                {
                    if (column.KeyType == KeyType.Guid)
                    {
                        var comb = SqlGenerator.Configuration.GetNextGuid();
                        column.PropertyInfo.SetValue(e, comb, null);
                    }
                }
            }

            var sql = SqlGenerator.Insert(classMap);

            await ExecuteAsync(connection, sql, entities, transaction, commandTimeout, CommandType.Text);
        }

        public async Task<dynamic> InsertAsync<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            var classMap = SqlGenerator.Configuration.GetMap<T>();
            var nonIdentityKeyProperties = classMap.Properties.Where(p => p.KeyType == KeyType.Guid || p.KeyType == KeyType.Assigned).ToList();
            var identityColumn = classMap.Properties.SingleOrDefault(p => p.KeyType == KeyType.Identity);
            foreach (var column in nonIdentityKeyProperties)
            {
                if (column.KeyType == KeyType.Guid)
                {
                    var comb = SqlGenerator.Configuration.GetNextGuid();
                    column.PropertyInfo.SetValue(entity, comb, null);
                }
            }

            IDictionary<string, object> keyValues = new ExpandoObject();
            var sql = SqlGenerator.Insert(classMap);
            if (identityColumn != null)
            {
                IEnumerable<long> result;
                if (SqlGenerator.SupportsMultipleStatements())
                {
                    sql += SqlGenerator.Configuration.Dialect.BatchSeperator + SqlGenerator.IdentitySql(classMap);
                    result = Query<long>(connection, sql, entity, transaction, false, commandTimeout, CommandType.Text);
                }
                else
                {
                    await ExecuteAsync(connection, sql, entity, transaction, commandTimeout, CommandType.Text);
                    sql = SqlGenerator.IdentitySql(classMap);
                    result = Query<long>(connection, sql, entity, transaction, false, commandTimeout, CommandType.Text);
                }

                var identityValue = result.First();
                var identityInt = Convert.ToInt32(identityValue);
                keyValues.Add(identityColumn.Name, identityInt);
                identityColumn.PropertyInfo.SetValue(entity, identityInt, null);
            }
            else
            {
                await ExecuteAsync(connection, sql, entity, transaction, commandTimeout, CommandType.Text);
            }

            foreach (var column in nonIdentityKeyProperties)
            {
                keyValues.Add(column.Name, column.PropertyInfo.GetValue(entity, null));
            }

            if (keyValues.Count == 1)
            {
                return keyValues.First().Value;
            }

            return keyValues;
        }

        public async Task<bool> UpdateAsync<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            var classMap = SqlGenerator.Configuration.GetMap<T>();
            var predicate = GetKeyPredicate(classMap, entity);
            var parameters = new Dictionary<string, object>();
            var sql = SqlGenerator.Update(classMap, predicate, parameters);
            var dynamicParameters = new DynamicParameters();

            var columns = classMap.Properties.Where(p => !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity));
            foreach (var property in ReflectionHelper.GetObjectValues(entity).Where(property => columns.Any(c => c.Name == property.Key)))
            {
                dynamicParameters.Add(property.Key, property.Value);
            }

            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            return await ExecuteAsync(connection, sql, dynamicParameters, transaction, commandTimeout, CommandType.Text) > 0;
        }

        public Task<bool> DeleteAsync<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            var classMap = SqlGenerator.Configuration.GetMap<T>();
            var predicate = GetKeyPredicate(classMap, entity);
            return DeleteAsync(connection, classMap, predicate, transaction, commandTimeout);
        }

        public Task<bool> DeleteAsync<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            var classMap = SqlGenerator.Configuration.GetMap<T>();
            var wherePredicate = GetPredicate(classMap, predicate);
            return DeleteAsync(connection, classMap, wherePredicate, transaction, commandTimeout);
        }

        public Task<IMultipleResultReader> GetMultipleAsync(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout)
        {
            if (SqlGenerator.SupportsMultipleStatements())
            {
                return GetMultipleByBatchAsync(connection, predicate, transaction, commandTimeout);
            }

            return GetMultipleBySequenceAsync(connection, predicate, transaction, commandTimeout);
        }

        #region Async implementation
        
        public async Task<T> GetAsync<T>(IDbConnection connection, object id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var classMap = SqlGenerator.Configuration.GetMap<T>();
            var predicate = GetIdPredicate(classMap, id);
            return (await GetListAsync<T>(connection, classMap, predicate, null, transaction, commandTimeout)).SingleOrDefault();
        }
        
        public async Task<IEnumerable<T>> GetListAsync<T>(IDbConnection connection, object predicate = null, IList<ISort> sort = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var classMap = SqlGenerator.Configuration.GetMap<T>();
            var wherePredicate = GetPredicate(classMap, predicate);
            return await GetListAsync<T>(connection, classMap, wherePredicate, sort, transaction, commandTimeout);
        }
        
        public async Task<IEnumerable<T>> GetPageAsync<T>(IDbConnection connection, object predicate = null, IList<ISort> sort = null, int page = 0, int resultsPerPage = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var classMap = SqlGenerator.Configuration.GetMap<T>();
            var wherePredicate = GetPredicate(classMap, predicate);
            return await GetPageAsync<T>(connection, classMap, wherePredicate, sort, page, resultsPerPage, transaction, commandTimeout);
        }
        
        public async Task<IEnumerable<T>> GetSetAsync<T>(IDbConnection connection, object predicate = null, IList<ISort> sort = null, int firstResult = 1, int maxResults = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var classMap = SqlGenerator.Configuration.GetMap<T>();
            var wherePredicate = GetPredicate(classMap, predicate);
            return await GetSetAsync<T>(connection, classMap, wherePredicate, sort, firstResult, maxResults, transaction, commandTimeout);
        }
        
        public async Task<int> CountAsync<T>(IDbConnection connection, object predicate = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var classMap = SqlGenerator.Configuration.GetMap<T>();
            var wherePredicate = GetPredicate(classMap, predicate);
            var parameters = new Dictionary<string, object>();
            var sql = SqlGenerator.Count(classMap, wherePredicate, parameters);
            var dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            var result = await QueryAsync(connection, sql, dynamicParameters, transaction, commandTimeout, CommandType.Text);
            return (int)result.Single().Total;
        }

        #region Helpers
        
        protected async Task<IEnumerable<T>> GetListAsync<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            var parameters = new Dictionary<string, object>();
            var sql = SqlGenerator.Select(classMap, predicate, sort, parameters);
            var dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            return await QueryAsync<T>(connection, sql, dynamicParameters, transaction, commandTimeout, CommandType.Text);
        }

        protected async Task<IEnumerable<T>> GetPageAsync<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            var parameters = new Dictionary<string, object>();
            var sql = SqlGenerator.SelectPaged(classMap, predicate, sort, page, resultsPerPage, parameters);
            var dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            return await QueryAsync<T>(connection, sql, dynamicParameters, transaction, commandTimeout, CommandType.Text);
        }
        
        protected async Task<IEnumerable<T>> GetSetAsync<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int firstResult, int maxResults, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            var parameters = new Dictionary<string, object>();
            var sql = SqlGenerator.SelectSet(classMap, predicate, sort, firstResult, maxResults, parameters);
            var dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            return await QueryAsync<T>(connection, sql, dynamicParameters, transaction, commandTimeout, CommandType.Text);
        }

        #endregion

        #endregion
        
        protected async Task<bool> DeleteAsync(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IDbTransaction transaction, int? commandTimeout)
        {
            var parameters = new Dictionary<string, object>();
            var sql = SqlGenerator.Delete(classMap, predicate, parameters);
            var dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            return await ExecuteAsync(connection, sql, dynamicParameters, transaction, commandTimeout, CommandType.Text) > 0;
        }

        protected IPredicate GetPredicate(IClassMapper classMap, object predicate)
        {
            var wherePredicate = predicate as IPredicate;
            if (wherePredicate == null && predicate != null)
            {
                wherePredicate = GetEntityPredicate(classMap, predicate);
            }

            return wherePredicate;
        }

        protected IPredicate GetIdPredicate(IClassMapper classMap, object id)
        {
            var isSimpleType = ReflectionHelper.IsSimpleType(id.GetType());
            var keys = classMap.Properties.Where(p => p.KeyType != KeyType.NotAKey);
            IDictionary<string, object> paramValues = null;
            IList<IPredicate> predicates = new List<IPredicate>();
            if (!isSimpleType)
            {
                paramValues = ReflectionHelper.GetObjectValues(id);
            }

            foreach (var key in keys)
            {
                var value = id;
                if (!isSimpleType)
                {
                    value = paramValues[key.Name];
                }

                var predicateType = typeof(FieldPredicate<>).MakeGenericType(classMap.EntityType);

                var fieldPredicate = (IFieldPredicate)Activator.CreateInstance(predicateType, new Func<ISqlGenerator>(() => SqlGenerator));
                fieldPredicate.Not = false;
                fieldPredicate.Operator = Operator.Eq;
                fieldPredicate.PropertyName = key.Name;
                fieldPredicate.Value = value;
                predicates.Add(fieldPredicate);
            }

            return predicates.Count == 1
                ? predicates[0]
                : new PredicateGroup(() => SqlGenerator)
                {
                    Operator = GroupOperator.And,
                    Predicates = predicates
                };
        }

        protected IPredicate GetKeyPredicate<T>(IClassMapper classMap, T entity) where T : class
        {
            var whereFields = classMap.Properties.Where(p => p.KeyType != KeyType.NotAKey).ToList();
            if (!whereFields.Any())
            {
                throw new ArgumentException("At least one Key column must be defined.");
            }

            IList<IPredicate> predicates = (from field in whereFields
                                            select new FieldPredicate<T>(() => SqlGenerator)
                                            {
                                                Not = false,
                                                Operator = Operator.Eq,
                                                PropertyName = field.Name,
                                                Value = field.PropertyInfo.GetValue(entity, null)
                                            }).Cast<IPredicate>().ToList();

            return predicates.Count == 1
                ? predicates[0]
                : new PredicateGroup(() => SqlGenerator)
                {
                    Operator = GroupOperator.And,
                    Predicates = predicates
                };
        }

        protected IPredicate GetEntityPredicate(IClassMapper classMap, object entity)
        {
            var predicateType = typeof(FieldPredicate<>).MakeGenericType(classMap.EntityType);
            IList<IPredicate> predicates = new List<IPredicate>();
            foreach (var kvp in ReflectionHelper.GetObjectValues(entity))
            {
                var fieldPredicate = (IFieldPredicate)Activator.CreateInstance(predicateType, new Func<ISqlGenerator>(() => SqlGenerator));
                fieldPredicate.Not = false;
                fieldPredicate.Operator = Operator.Eq;
                fieldPredicate.PropertyName = kvp.Key;
                fieldPredicate.Value = kvp.Value;
                predicates.Add(fieldPredicate);
            }

            return predicates.Count == 1
                ? predicates[0]
                : new PredicateGroup(() => SqlGenerator)
                {
                    Operator = GroupOperator.And,
                    Predicates = predicates
                };
        }

        protected async Task<IMultipleResultReader> GetMultipleByBatchAsync(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout)
        {
            var parameters = new Dictionary<string, object>();
            var sql = new StringBuilder();
            foreach (var item in predicate.Items)
            {
                var classMap = SqlGenerator.Configuration.GetMap(item.Type);
                var itemPredicate = item.Value as IPredicate;
                if (itemPredicate == null && item.Value != null)
                {
                    itemPredicate = GetPredicate(classMap, item.Value);
                }

                sql.AppendLine(SqlGenerator.Select(classMap, itemPredicate, item.Sort, parameters) + SqlGenerator.Configuration.Dialect.BatchSeperator);
            }

            var dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            var grid = await QueryMultipleAsync(connection, sql.ToString(), dynamicParameters, transaction, commandTimeout, CommandType.Text);
            return new GridReaderResultReader(grid);
        }

        protected async Task<IMultipleResultReader> GetMultipleBySequenceAsync(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout)
        {
            IList<SqlMapper.GridReader> items = new List<SqlMapper.GridReader>();
            foreach (var item in predicate.Items)
            {
                var parameters = new Dictionary<string, object>();
                var classMap = SqlGenerator.Configuration.GetMap(item.Type);
                var itemPredicate = item.Value as IPredicate;
                if (itemPredicate == null && item.Value != null)
                {
                    itemPredicate = GetPredicate(classMap, item.Value);
                }

                var sql = SqlGenerator.Select(classMap, itemPredicate, item.Sort, parameters);
                var dynamicParameters = new DynamicParameters();
                foreach (var parameter in parameters)
                {
                    dynamicParameters.Add(parameter.Key, parameter.Value);
                }

                var queryResult = await QueryMultipleAsync(connection, sql, dynamicParameters, transaction, commandTimeout, CommandType.Text);
                items.Add(queryResult);
            }

            return new SequenceReaderResultReader(items);
        }

        private IEnumerable<dynamic> Query(IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return connection.Query(sql, param, transaction, buffered, commandTimeout, commandType);
        }

        private IEnumerable<T> Query<T>(IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return connection.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);
        }

        private Task<IEnumerable<dynamic>> QueryAsync(IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return connection.QueryAsync(sql, param, transaction, commandTimeout, commandType);
        }

        private Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        private Task<SqlMapper.GridReader> QueryMultipleAsync(IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return connection.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType);
        }

        private Task<int> ExecuteAsync(IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
        }
    }
}