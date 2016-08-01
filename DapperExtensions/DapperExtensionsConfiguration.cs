using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;

namespace DapperExtensions
{
    public interface IDapperExtensionsConfiguration
    {
        Type DefaultMapper { get; }
        IList<Assembly> MappingAssemblies { get; }
        ISqlDialect Dialect { get; }
        IClassMapper GetMap(Type entityType);
        IClassMapper GetMap<T>() where T : class;
        void ClearCache();
        Guid GetNextGuid();
    }

    public class DapperExtensionsConfiguration : IDapperExtensionsConfiguration
    {
        private readonly ConcurrentDictionary<Type, IClassMapper> _classMaps = new ConcurrentDictionary<Type, IClassMapper>();

        public DapperExtensionsConfiguration()
            : this(typeof(AutoClassMapper<>), new List<Assembly>(), new SqlServerDialect())
        {
        }


        public DapperExtensionsConfiguration(Type defaultMapper, IList<Assembly> mappingAssemblies, ISqlDialect sqlDialect)
        {
            DefaultMapper = defaultMapper;
            MappingAssemblies = mappingAssemblies ?? new List<Assembly>();
            Dialect = sqlDialect;
        }

        public Type DefaultMapper { get; }
        public IList<Assembly> MappingAssemblies { get; }
        public ISqlDialect Dialect { get; }

        public IClassMapper GetMap(Type entityType)
        {
            IClassMapper map;
            if (!_classMaps.TryGetValue(entityType, out map))
            {
                var mapType = GetMapType(entityType);
                if (mapType == null)
                {
                    mapType = DefaultMapper.MakeGenericType(entityType);
                }

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

        public Guid GetNextGuid()
        {
            var b = Guid.NewGuid().ToByteArray();
            var dateTime = new DateTime(1900, 1, 1);
            var now = DateTime.Now;
            var timeSpan = new TimeSpan(now.Ticks - dateTime.Ticks);
            var timeOfDay = now.TimeOfDay;
            var bytes1 = BitConverter.GetBytes(timeSpan.Days);
            var bytes2 = BitConverter.GetBytes((long)(timeOfDay.TotalMilliseconds / 3.333333));
            Array.Reverse(bytes1);
            Array.Reverse(bytes2);
            Array.Copy(bytes1, bytes1.Length - 2, b, b.Length - 6, 2);
            Array.Copy(bytes2, bytes2.Length - 4, b, b.Length - 4, 4);
            return new Guid(b);
        }

        protected virtual Type GetMapType(Type entityType)
        {
            Func<Assembly, Type> getType = a =>
            {
                var types = a.GetTypes();
                return (from type in types
                    let interfaceType = type.GetInterface(typeof(IClassMapper<>).FullName)
                    where
                    interfaceType != null &&
                    interfaceType.GetGenericArguments()[0] == entityType
                    select type).SingleOrDefault();
            };

            var result = getType(entityType.Assembly);
            if (result != null)
            {
                return result;
            }

            foreach (var mappingAssembly in MappingAssemblies)
            {
                result = getType(mappingAssembly);
                if (result != null)
                {
                    return result;
                }
            }

            return getType(entityType.Assembly);
        }
    }
}