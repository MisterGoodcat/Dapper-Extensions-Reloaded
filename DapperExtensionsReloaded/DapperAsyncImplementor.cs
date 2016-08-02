using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;

namespace DapperExtensions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDapperAsyncImplementor : IDapperImplementor
    {
        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Get{T}"/>.
        /// </summary>
        Task<T> GetAsync<T>(IDbConnection connection, object id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.GetList{T}"/>.
        /// </summary>
        Task<IEnumerable<T>> GetListAsync<T>(IDbConnection connection, object predicate = null, IList<ISort> sort = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.GetPage{T}"/>.
        /// </summary>
        Task<IEnumerable<T>> GetPageAsync<T>(IDbConnection connection, object predicate = null, IList<ISort> sort = null, int page = 0, int resultsPerPage = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.GetSet{T}"/>.
        /// </summary>
        Task<IEnumerable<T>> GetSetAsync<T>(IDbConnection connection, object predicate = null, IList<ISort> sort = null, int firstResult = 1, int maxResults = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Count{T}"/>.
        /// </summary>
        Task<int> CountAsync<T>(IDbConnection connection, object predicate = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
    }

    public class DapperAsyncImplementor : DapperImplementor, IDapperAsyncImplementor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DapperAsyncImplementor"/> class.
        /// </summary>
        /// <param name="sqlGenerator">The SQL generator.</param>
        public DapperAsyncImplementor(ISqlGenerator sqlGenerator)
            : base(sqlGenerator)
        {
        }

        #region Implementation of IDapperAsyncImplementor

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Get{T}"/>.
        /// </summary>
        public async Task<T> GetAsync<T>(IDbConnection connection, object id, IDbTransaction transaction = null,
            int? commandTimeout = null) where T : class
        {
            var classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate predicate = GetIdPredicate(classMap, id);
            return (await GetListAsync<T>(connection, classMap, predicate, null, transaction, commandTimeout)).SingleOrDefault();
        }

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.GetList{T}"/>.
        /// </summary>
        public async Task<IEnumerable<T>> GetListAsync<T>(IDbConnection connection, object predicate = null, IList<ISort> sort = null,
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var classMap = SqlGenerator.Configuration.GetMap<T>();
            var wherePredicate = GetPredicate(classMap, predicate);
            return await GetListAsync<T>(connection, classMap, wherePredicate, sort, transaction, commandTimeout);
        }

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.GetPage{T}"/>.
        /// </summary>
        public async Task<IEnumerable<T>> GetPageAsync<T>(IDbConnection connection, object predicate = null, IList<ISort> sort = null, int page = 0, int resultsPerPage = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var classMap = SqlGenerator.Configuration.GetMap<T>();
            var wherePredicate = GetPredicate(classMap, predicate);
            return await GetPageAsync<T>(connection, classMap, wherePredicate, sort, page, resultsPerPage, transaction, commandTimeout);
        }

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.GetSet{T}"/>.
        /// </summary>
        public async Task<IEnumerable<T>> GetSetAsync<T>(IDbConnection connection, object predicate = null, IList<ISort> sort = null, int firstResult = 1,
            int maxResults = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var classMap = SqlGenerator.Configuration.GetMap<T>();
            var wherePredicate = GetPredicate(classMap, predicate);
            return await GetSetAsync<T>(connection, classMap, wherePredicate, sort, firstResult, maxResults, transaction, commandTimeout);
        }

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.Count{T}"/>.
        /// </summary>
        public async Task<int> CountAsync<T>(IDbConnection connection, object predicate = null, IDbTransaction transaction = null,
            int? commandTimeout = null) where T : class
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

            return (int)(await connection.QueryAsync(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text)).Single().Total;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.GetList{T}"/>.
        /// </summary>
        protected async Task<IEnumerable<T>> GetListAsync<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            var parameters = new Dictionary<string, object>();
            var sql = SqlGenerator.Select(classMap, predicate, sort, parameters);
            var dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            return await connection.QueryAsync<T>(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.GetPage{T}"/>.
        /// </summary>
        protected async Task<IEnumerable<T>> GetPageAsync<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            var parameters = new Dictionary<string, object>();
            var sql = SqlGenerator.SelectPaged(classMap, predicate, sort, page, resultsPerPage, parameters);
            var dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            return await connection.QueryAsync<T>(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// The asynchronous counterpart to <see cref="IDapperImplementor.GetSet{T}"/>.
        /// </summary>
        protected async Task<IEnumerable<T>> GetSetAsync<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int firstResult, int maxResults, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            var parameters = new Dictionary<string, object>();
            var sql = SqlGenerator.SelectSet(classMap, predicate, sort, firstResult, maxResults, parameters);
            var dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            return await connection.QueryAsync<T>(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text);
        }

        #endregion
    }
}