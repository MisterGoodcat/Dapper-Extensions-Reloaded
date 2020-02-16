using System.Collections.Generic;
using DapperExtensionsReloaded.Mapper.Internal;
using DapperExtensionsReloaded.Predicates;

namespace DapperExtensionsReloaded.Internal.Sql
{
    internal interface ISqlGenerator
    {
        IDapperExtensionsConfiguration Configuration { get; }

        string Select(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, IDictionary<string, object> parameters);
        string SelectPaged(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int page, int itemsPerPage, int resultsToReturn, IDictionary<string, object> parameters);
        string SelectSet(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int firstResult, int maxResults, IDictionary<string, object> parameters);
        string Count(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters);

        string Insert(IClassMapper classMap);
        string Update(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters);
        string UpdateSet(IClassMapper classMap, IList<IPropertyMap> propertiesToUpdate, IPredicate wherePredicate, IDictionary<string, object> parameters);
        string Delete(IClassMapper classMap, IPredicate wherePredicate, IDictionary<string, object> parameters);

        string IdentitySql(IClassMapper classMap);
        string GetTableName(IClassMapper map);
        string GetColumnName(IClassMapper map, IPropertyMap property, bool includeAlias);
        string GetColumnName(IClassMapper map, string propertyName, bool includeAlias);
        bool SupportsMultipleStatements();
    }
}