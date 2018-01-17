using System;
using System.Collections.Generic;

namespace DapperExtensionsReloaded.Mapper.Internal
{
    internal interface IClassMapper
    {
        string SchemaName { get; }
        string TableName { get; }
        IReadOnlyCollection<IPropertyMap> Properties { get; }
        Type EntityType { get; }
    }

    // ReSharper disable once UnusedTypeParameter => Marker interface
    internal interface IClassMapper<T> : IClassMapper where T : class
    {
    }
}