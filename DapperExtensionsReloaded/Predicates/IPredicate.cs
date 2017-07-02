using System.Collections.Generic;

namespace DapperExtensionsReloaded.Predicates
{
    public interface IPredicate
    {
        string GetSql(IDictionary<string, object> parameters);
    }
}