using System.Collections.Generic;

namespace DapperExtensions.Predicates
{
    public interface IPredicate
    {
        string GetSql(IDictionary<string, object> parameters);
    }
}