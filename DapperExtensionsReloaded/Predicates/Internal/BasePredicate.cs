using System;
using System.Collections.Generic;
using System.Linq;
using DapperExtensionsReloaded.Internal.Sql;

namespace DapperExtensionsReloaded.Predicates.Internal
{
    internal abstract class BasePredicate : IBasePredicate
    {
        protected Func<ISqlGenerator> GetGenerator { get; }
        public abstract string GetSql(IDictionary<string, object> parameters);
        public string PropertyName { get; set; }

        protected BasePredicate(Func<ISqlGenerator> getGenerator)
        {
            if (getGenerator == null)
            {
                throw new ArgumentNullException(nameof(getGenerator));
            }

            GetGenerator = getGenerator;
        }

        protected virtual string GetColumnName(Type entityType, string propertyName)
        {
            var sqlGenerator = GetGenerator();
            var map = sqlGenerator.Configuration.GetMap(entityType);
            if (map == null)
            {
                throw new NullReferenceException($"No map found for {entityType}");
            }

            var propertyMap = map.Properties.SingleOrDefault(p => p.Name == propertyName);
            if (propertyMap == null)
            {
                throw new NullReferenceException($"Property {propertyName} was not found for type {entityType}");
            }

            return sqlGenerator.GetColumnName(map, propertyMap, false);
        }
    }
}