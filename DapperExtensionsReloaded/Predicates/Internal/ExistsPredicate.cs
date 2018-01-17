using System;
using System.Collections.Generic;
using DapperExtensionsReloaded.Internal;
using DapperExtensionsReloaded.Internal.Sql;
using DapperExtensionsReloaded.Mapper.Internal;

namespace DapperExtensionsReloaded.Predicates.Internal
{
    internal sealed class ExistsPredicate<TSub> : IExistsPredicate where TSub : class
    {
        private readonly Func<ISqlGenerator> _getGenerator;
        public IPredicate Predicate { get; set; }
        public bool Not { get; set; }

        public ExistsPredicate(Func<ISqlGenerator> getGenerator)
        {
            _getGenerator = getGenerator;
        }

        public string GetSql(IDictionary<string, object> parameters)
        {
            var sqlGenerator = _getGenerator();
            var mapSub = GetClassMapper(typeof(TSub), sqlGenerator.Configuration);
            var sql = $"({(Not ? "NOT " : string.Empty)}EXISTS (SELECT 1 FROM {sqlGenerator.GetTableName(mapSub)} WHERE {Predicate.GetSql(parameters)}))";
            return sql;
        }

        private IClassMapper GetClassMapper(Type type, IDapperExtensionsConfiguration configuration)
        {
            var map = configuration.GetMap(type);
            if (map == null)
            {
                throw new NullReferenceException($"CreatePropertyMap was not found for {type}");
            }

            return map;
        }
    }
}