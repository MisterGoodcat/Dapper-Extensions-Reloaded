using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DapperExtensions.Internal.Sql;
using DapperExtensions.Predicates;

namespace DapperExtensions.Internal
{
    internal interface IDapperImplementor
    {
        ISqlGenerator SqlGenerator { get; }
        T Get<T>(IDbConnection connection, dynamic id, IDbTransaction transaction, int? commandTimeout) where T : class;
        void Insert<T>(IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction, int? commandTimeout) where T : class;
        dynamic Insert<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class;
        bool Update<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class;
        bool Delete<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class;
        bool Delete<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class;
        IEnumerable<T> GetList<T>(IDbConnection connection, object predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class;
        IEnumerable<T> GetPage<T>(IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class;
        IEnumerable<T> GetSet<T>(IDbConnection connection, object predicate, IList<ISort> sort, int firstResult, int maxResults, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class;
        int Count<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class;
        IMultipleResultReader GetMultiple(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout);

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
}