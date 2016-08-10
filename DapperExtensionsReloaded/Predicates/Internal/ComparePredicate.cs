using System;
using DapperExtensions.Internal.Sql;

namespace DapperExtensions.Predicates.Internal
{
    internal abstract class ComparePredicate : BasePredicate
    {
        protected ComparePredicate(Func<ISqlGenerator> getGenerator) : base(getGenerator)
        {
        }

        public Operator Operator { get; set; }
        public bool Not { get; set; }

        public string GetOperatorString()
        {
            switch (Operator)
            {
                case Operator.Gt:
                    return Not ? "<=" : ">";
                case Operator.Ge:
                    return Not ? "<" : ">=";
                case Operator.Lt:
                    return Not ? ">=" : "<";
                case Operator.Le:
                    return Not ? ">" : "<=";
                case Operator.Like:
                    return Not ? "NOT LIKE" : "LIKE";
                default:
                    return Not ? "<>" : "=";
            }
        }
    }
}