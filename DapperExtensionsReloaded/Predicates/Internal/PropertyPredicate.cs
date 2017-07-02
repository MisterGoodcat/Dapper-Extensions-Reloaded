using System;
using System.Collections.Generic;
using DapperExtensionsReloaded.Internal.Sql;

namespace DapperExtensionsReloaded.Predicates.Internal
{
    internal sealed class PropertyPredicate<T, T2> : ComparePredicate, IPropertyPredicate where T : class where T2 : class
    {
        public PropertyPredicate(Func<ISqlGenerator> getGenerator) : base(getGenerator)
        {
        }

        public string PropertyName2 { get; set; }

        public override string GetSql(IDictionary<string, object> parameters)
        {
            var columnName = GetColumnName(typeof(T), PropertyName);
            var columnName2 = GetColumnName(typeof(T2), PropertyName2);
            return $"({columnName} {GetOperatorString()} {columnName2})";
        }
    }
}