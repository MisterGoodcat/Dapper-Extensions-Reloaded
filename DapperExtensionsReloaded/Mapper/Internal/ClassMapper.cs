using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DapperExtensionsReloaded.Mapper.Internal
{
    /// <summary>
    /// This class mapper looks for <see cref="DatabaseEntityAttribute"/> to determine the table and schema names as well as
    /// <see cref="DatabaseColumnAttribute"/> to determine column names, ignored and read-only properties and identity columns.
    /// For columns or other data that has not been mapped using attributes, a generic mapping is used.
    /// </summary>
    internal sealed class ClassMapper<T> : IClassMapper<T> where T : class
    {
        private readonly List<IPropertyMap> _properties = new List<IPropertyMap>();

        /// <summary>
        /// Gets or sets the schema to use when referring to the corresponding table name in the database.
        /// </summary>
        public string SchemaName { get; private set; }

        /// <summary>
        /// Gets or sets the table to use in the database.
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// A collection of properties that will map to columns in the database table.
        /// </summary>
        public IReadOnlyCollection<IPropertyMap> Properties => _properties;

        public Type EntityType => typeof(T);

        public ClassMapper()
        {
            Map();
        }

        private void Map()
        {
            var entityType = typeof(T);

            MapEntityInfo(entityType);
            MapPropertiesByAttribute(entityType);
            AutoMapRemainingProperties();
        }

        private void MapEntityInfo(Type entityType)
        {
            var entityAttribute = entityType.GetTypeInfo().GetCustomAttribute<DatabaseEntityAttribute>();
            if (entityAttribute != null)
            {
                TableName = entityAttribute.TableName;
                SchemaName = entityAttribute.SchemaName;
            }
            else
            {
                TableName = entityType.Name;
            }
        }

        private void MapPropertiesByAttribute(Type entityType)
        {
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttributes<DatabaseColumnAttribute>(false).FirstOrDefault();
                if (attribute != null)
                {
                    var propertyMap = CreatePropertyMap(property);

                    if (!string.IsNullOrEmpty(attribute.ColumnName))
                    {
                        propertyMap.Column(attribute.ColumnName);
                    }

                    if (attribute.IsIdentity)
                    {
                        propertyMap.Key(KeyType.Identity);
                    }
                    else
                    {
                        propertyMap.Key(KeyType.NotAKey);
                    }

                    if (attribute.IsIgnored)
                    {
                        propertyMap.Ignore();
                    }

                    if (attribute.IsReadOnly)
                    {
                        propertyMap.ReadOnly();
                    }
                }
            }
        }
        
        private void AutoMapRemainingProperties()
        {
            var type = typeof(T);
            var hasDefinedKey = Properties.Any(p => p.KeyType != KeyType.NotAKey);
            PropertyMap keyMap = null;
            foreach (var propertyInfo in type.GetProperties())
            {
                if (Properties.Any(p => p.Name.Equals(propertyInfo.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                var map = CreatePropertyMap(propertyInfo);
                if (!hasDefinedKey)
                {
                    if (string.Equals(map.PropertyInfo.Name, "id", StringComparison.OrdinalIgnoreCase))
                    {
                        keyMap = map;
                    }

                    if (keyMap == null && map.PropertyInfo.Name.EndsWith("id", StringComparison.OrdinalIgnoreCase))
                    {
                        keyMap = map;
                    }
                }
            }

            if (keyMap != null)
            {
                if (!PropertyTypeToKeyTypeMapping.Map.TryGetValue(keyMap.PropertyInfo.PropertyType, out var keyType))
                {
                    keyType = KeyType.Assigned;
                }

                keyMap.Key(keyType);
            }
        }
        
        private PropertyMap CreatePropertyMap(PropertyInfo propertyInfo)
        {
            var result = new PropertyMap(propertyInfo);
            GuardForDuplicatePropertyMap(result);
            _properties.Add(result);
            return result;
        }

        private void GuardForDuplicatePropertyMap(PropertyMap result)
        {
            if (_properties.Any(p => p.Name.Equals(result.Name)))
            {
                throw new ArgumentException($"Duplicate mapping for property {result.Name} detected.");
            }
        }
    }
}