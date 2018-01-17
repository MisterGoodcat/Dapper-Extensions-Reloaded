using System;
using System.Collections.Concurrent;
using DapperExtensionsReloaded.Internal.Sql;
using DapperExtensionsReloaded.Mapper.Internal;

namespace DapperExtensionsReloaded.Internal
{
    internal class DapperExtensionsConfiguration : IDapperExtensionsConfiguration
    {
        private readonly ConcurrentDictionary<Type, IClassMapper> _classMaps = new ConcurrentDictionary<Type, IClassMapper>();

        public DapperExtensionsConfiguration() : this(new SqlServerDialect())
        {
        }

        public DapperExtensionsConfiguration(ISqlDialect sqlDialect)
        {
            Dialect = sqlDialect;
        }
        
        public ISqlDialect Dialect { get; }

        public IClassMapper GetMap(Type entityType)
        {
            if (!_classMaps.TryGetValue(entityType, out var map))
            {
                var mapType = typeof(ClassMapper<>).MakeGenericType(entityType);
                map = Activator.CreateInstance(mapType) as IClassMapper;
                _classMaps[entityType] = map;
            }

            return map;
        }

        public IClassMapper GetMap<T>() where T : class
        {
            return GetMap(typeof(T));
        }

        public void ClearCache()
        {
            _classMaps.Clear();
        }
    }
}