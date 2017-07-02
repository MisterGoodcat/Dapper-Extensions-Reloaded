using System;
using System.Linq;
using System.Reflection;

namespace DapperExtensionsReloaded.Mapper.Internal
{
    /// <summary>
    /// This class mapper looks for <see cref="DatabaseEntityAttribute"/> to determine the table and schema names as well as
    /// <see cref="DatabaseColumnAttribute"/> to determine column names, ignored and read-only properties and identity columns.
    /// For columns or other data that has not been mapped using attributes, the auto-map feature is used.
    /// </summary>
    internal sealed class AttributeClassMapper<T> : ClassMapper<T> where T : class
    {
        public AttributeClassMapper()
        {
            Map();
        }

        private void Map()
        {
            var entityType = typeof(T);

            MapEntityInfo(entityType);
            MapPropertyInfo(entityType);
            
            AutoMap();
        }

        private void MapEntityInfo(Type entityType)
        {
            var entityAttribute = entityType.GetTypeInfo().GetCustomAttribute<DatabaseEntityAttribute>();
            if (entityAttribute != null)
            {
                Table(entityAttribute.TableName);
                Schema(entityAttribute.SchemaName);
            }
            else
            {
                Table(entityType.Name);
            }
        }

        private void MapPropertyInfo(Type entityType)
        {
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttributes<DatabaseColumnAttribute>(false).FirstOrDefault();
                if (attribute != null)
                {
                    var propertyMap = Map(property);

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
    }
}
