using System;
using DapperExtensionsReloaded.Internal.Sql;
using DapperExtensionsReloaded.Mapper.Internal;

namespace DapperExtensionsReloaded.Internal
{
    internal interface IDapperExtensionsConfiguration
    {
        ISqlDialect Dialect { get; }
        IClassMapper GetMap(Type entityType);
        IClassMapper GetMap<T>() where T : class;
        void ClearCache();
        Guid GetNextGuid();
    }
}