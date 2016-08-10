﻿using System;
using System.Collections.Generic;
using System.Reflection;
using DapperExtensions.Internal.Sql;
using DapperExtensions.Mapper.Internal;

namespace DapperExtensions.Internal
{
    internal interface IDapperExtensionsConfiguration
    {
        Type DefaultMapper { get; }
        IList<Assembly> MappingAssemblies { get; }
        ISqlDialect Dialect { get; }
        IClassMapper GetMap(Type entityType);
        IClassMapper GetMap<T>() where T : class;
        void ClearCache();
        Guid GetNextGuid();
    }
}