using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DapperExtensionsReloaded.Internal.Sql;
using DapperExtensionsReloaded.Predicates;

namespace DapperExtensionsReloaded.Internal
{
    internal interface IDapperImplementor
    {
        ISqlGenerator SqlGenerator { get; }
        Task InsertAsync<T>(IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction, int? commandTimeout) where T : class;
        Task<dynamic> InsertAsync<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class;
        Task<bool> UpdateAsync<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class;
        Task<bool> DeleteAsync<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class;
        Task<bool> DeleteAsync<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class;
        Task<IMultipleResultReader> GetMultipleAsync(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout);

        Task<T> GetAsync<T>(IDbConnection connection, object id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        Task<IEnumerable<T>> GetListAsync<T>(IDbConnection connection, object predicate = null, IList<ISort> sort = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        Task<IEnumerable<T>> GetPageAsync<T>(IDbConnection connection, object predicate = null, IList<ISort> sort = null, int page = 0, int resultsPerPage = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        Task<IEnumerable<T>> GetSetAsync<T>(IDbConnection connection, object predicate = null, IList<ISort> sort = null, int firstResult = 1, int maxResults = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        Task<int> CountAsync<T>(IDbConnection connection, object predicate = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
    }
}