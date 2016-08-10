using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DapperExtensions.Internal.Sql;

namespace DapperExtensions.Predicates.Internal
{
    /// <summary>
    ///     Groups IPredicates together using the specified group operator.
    /// </summary>
    internal sealed class PredicateGroup : IPredicateGroup
    {
        private readonly Func<ISqlGenerator> _getGenerator;
        public GroupOperator Operator { get; set; }
        public IList<IPredicate> Predicates { get; set; }

        public PredicateGroup(Func<ISqlGenerator> getGenerator)
        {
            _getGenerator = getGenerator;
        }

        public string GetSql(IDictionary<string, object> parameters)
        {
            var sqlGenerator = _getGenerator();
            var seperator = Operator == GroupOperator.And ? " AND " : " OR ";
            return "(" + Predicates.Aggregate(
                       new StringBuilder(),
                       (sb, p) => (sb.Length == 0 ? sb : sb.Append(seperator)).Append(p.GetSql(parameters)),
                       sb =>
                       {
                           var s = sb.ToString();
                           if (s.Length == 0)
                               return sqlGenerator.Configuration.Dialect.EmptyExpression;
                           return s;
                       }
                   ) + ")";
        }
    }
}