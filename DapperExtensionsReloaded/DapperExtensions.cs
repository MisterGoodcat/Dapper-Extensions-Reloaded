using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DapperExtensionsReloaded.Internal;
using DapperExtensionsReloaded.Internal.Sql;
using DapperExtensionsReloaded.Logging.Formatters;
using DapperExtensionsReloaded.Mapper.Internal;
using DapperExtensionsReloaded.Predicates;

namespace DapperExtensionsReloaded
{
    public static class DapperExtensions
    {
        private static readonly object Lock = new object();

        private static Func<IDapperExtensionsConfiguration, IDapperImplementor> s_instanceFactory;
        private static IDapperImplementor s_instance;
        private static IDapperExtensionsConfiguration s_configuration;
        
        internal static ISqlDialect SqlDialect
        {
            get => s_configuration.Dialect;
            set => Configure(value, s_configuration.LogFormatter);
        }

        internal static ILogFormatter LogFormatter
        {
            get => s_configuration.LogFormatter;
            set => Configure(s_configuration.Dialect, value);
        }
        
        internal static Func<IDapperExtensionsConfiguration, IDapperImplementor> InstanceFactory
        {
            get
            {
                return s_instanceFactory ?? (s_instanceFactory = config => new DapperImplementor(new SqlGeneratorImpl(config)));
            }
            set
            {
                s_instanceFactory = value;
                Configure(s_configuration.Dialect, s_configuration.LogFormatter);
            }
        }
        
        internal static void Configure(IDapperExtensionsConfiguration configuration)
        {
            s_instance = null;
            s_configuration = configuration;
        }
        
        internal static void Configure(ISqlDialect sqlDialect, ILogFormatter logFormatter)
        {
            Configure(new DapperExtensionsConfiguration(sqlDialect, logFormatter));
        }
        
        internal static IDapperImplementor Instance
        {
            get
            {
                if (s_instance == null)
                {
                    lock (Lock)
                    {
                        if (s_instance == null)
                        {
                            s_instance = InstanceFactory(s_configuration);
                        }
                    }
                }

                return s_instance;
            }
        }

        static DapperExtensions()
        {
            Configure(new SqlServerDialect(), new SqlServerLogFormatter());
        }
        
        /// <summary>
        /// Executes an insert query for the specified entity.
        /// </summary>
        public static Task InsertAsync<T>(this IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return Instance.InsertAsync(connection, entities, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes an insert query for the specified entity, returning the primary key.  
        /// If the entity has a single key, just the value is returned.  
        /// If the entity has a composite key, an IDictionary&lt;string, object&gt; is returned with the key values.
        /// The key value for the entity will also be updated if the KeyType is a Guid or Identity.
        /// </summary>
        public static Task<dynamic> InsertAsync<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return Instance.InsertAsync(connection, entity, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes an update query for the specified entity.
        /// </summary>
        public static Task<bool> UpdateAsync<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return Instance.UpdateAsync(connection, entity, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes an update query for the specified predicate.
        /// </summary>
        public static Task<bool> UpdateSetAsync<T>(this IDbConnection connection, object values, IPredicate wherePredicate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return Instance.UpdateSetAsync<T>(connection, values, wherePredicate, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes a delete query for the specified entity.
        /// </summary>
        public static Task<bool> DeleteAsync<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return Instance.DeleteAsync(connection, entity, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes a delete query using the specified predicate.
        /// </summary>
        public static Task<bool> DeleteSetAsync<T>(this IDbConnection connection, IPredicate wherePredicate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return Instance.DeleteSetAsync<T>(connection, wherePredicate, transaction, commandTimeout);
        }
        
        /// <summary>
        /// Executes a select query for multiple objects, returning IMultipleResultReader for each predicate.
        /// </summary>
        public static Task<IMultipleResultReader> GetMultipleAsync(this IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return Instance.GetMultipleAsync(connection, predicate, transaction, commandTimeout);
        }

        /// <summary>
        /// Gets the appropriate mapper for the specified type T. 
        /// If the mapper for the type is not yet created, a new mapper is generated from the mapper type specifed by DefaultMapper.
        /// </summary>
        internal static IClassMapper GetMap<T>() where T : class
        {
            return Instance.SqlGenerator.Configuration.GetMap<T>();
        }

        /// <summary>
        /// Clears the ClassMappers for each type.
        /// </summary>
        public static void ClearCache()
        {
            Instance.SqlGenerator.Configuration.ClearCache();
        }

        /// <summary>
        /// Executes a query using the specified predicate, returning an integer that represents the number of rows that match the query.
        /// </summary>
        public static async Task<int> CountAsync<T>(this IDbConnection connection, IPredicate predicate = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await Instance.CountAsync<T>(connection, predicate, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes a query for the specified id, returning the data typed as per T.
        /// </summary>
        public static async Task<T> GetAsync<T>(this IDbConnection connection, object id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await Instance.GetAsync<T>(connection, id, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes a query for the specified predicate, returning the data typed as per T.
        /// </summary>
        public static async Task<T> GetAsync<T>(this IDbConnection connection, IPredicate predicate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await Instance.GetAsync<T>(connection, predicate, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// </summary>
        public static async Task<IEnumerable<T>> GetListAsync<T>(this IDbConnection connection, IPredicate predicate = null, IList<ISort> sort = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await Instance.GetListAsync<T>(connection, predicate, sort, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// Data returned is dependent upon the specified page and resultsPerPage.
        /// </summary>
        public static async Task<IEnumerable<T>> GetPageAsync<T>(this IDbConnection connection, IPredicate predicate = null, IList<ISort> sort = null, int page = 0, int resultsPerPage = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await Instance.GetPageAsync<T>(connection, predicate, sort, page, resultsPerPage, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// Data returned is dependent upon the specified firstResult and maxResults.
        /// </summary>
        public static async Task<IEnumerable<T>> GetSetAsync<T>(this IDbConnection connection, IPredicate predicate = null, IList<ISort> sort = null, int firstResult = 1, int maxResults = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await Instance.GetSetAsync<T>(connection, predicate, sort, firstResult, maxResults, transaction, commandTimeout);
        }
    }
}