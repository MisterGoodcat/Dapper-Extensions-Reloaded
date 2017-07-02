using System;
using System.Collections.Generic;
using DapperExtensionsReloaded.Internal;
using DapperExtensionsReloaded.Internal.Sql;

namespace DapperExtensionsReloaded.Predicates.Internal
{
    internal sealed class BetweenPredicate<T> : BasePredicate, IBetweenPredicate where T : class
    {
        public BetweenPredicate(Func<ISqlGenerator> getGenerator) : base(getGenerator)
        {
        }

        public override string GetSql(IDictionary<string, object> parameters)
        {
            var sqlGenerator = GetGenerator();
            var columnName = GetColumnName(typeof(T), PropertyName);
            var propertyName1 = parameters.SetParameterName(PropertyName, Value.Value1, sqlGenerator.Configuration.Dialect.ParameterPrefix);
            var propertyName2 = parameters.SetParameterName(PropertyName, Value.Value2, sqlGenerator.Configuration.Dialect.ParameterPrefix);
            return $"({columnName} {(Not ? "NOT " : string.Empty)}BETWEEN {propertyName1} AND {propertyName2})";
        }

        public BetweenValues Value { get; set; }

        public bool Not { get; set; }
    }
}