using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DapperExtensionsReloaded.Internal;
using DapperExtensionsReloaded.Internal.Sql;

namespace DapperExtensionsReloaded.Predicates.Internal
{
    internal sealed class FieldPredicate<T> : ComparePredicate, IFieldPredicate where T : class
    {
        public FieldPredicate(Func<ISqlGenerator> getGenerator) : base(getGenerator)
        {
        }

        public object Value { get; set; }

        public override string GetSql(IDictionary<string, object> parameters)
        {
            var sqlGenerator = GetGenerator();
            var columnName = GetColumnName(typeof(T), PropertyName);
            if (Value == null)
            {
                return $"({columnName} IS {(Not ? "NOT " : string.Empty)}NULL)";
            }

            if (Value is IEnumerable && !(Value is string))
            {
                if (Operator != Operator.Eq)
                {
                    throw new ArgumentException("Operator must be set to Eq for Enumerable types");
                }

                var @params = new List<string>();
                foreach (var value in (IEnumerable)Value)
                {
                    var valueParameterName = parameters.SetParameterName(PropertyName, value, sqlGenerator.Configuration.Dialect.ParameterPrefix);
                    @params.Add(valueParameterName);
                }

                var paramStrings = @params.Aggregate(new StringBuilder(), (sb, s) => sb.Append((sb.Length != 0 ? ", " : string.Empty) + s), sb => sb.ToString());
                return $"({columnName} {(Not ? "NOT " : string.Empty)}IN ({paramStrings}))";
            }

            var parameterName = parameters.SetParameterName(PropertyName, Value, sqlGenerator.Configuration.Dialect.ParameterPrefix);
            return $"({columnName} {GetOperatorString()} {parameterName})";
        }
    }
}