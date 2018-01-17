using System;
using System.Collections.Generic;

namespace DapperExtensionsReloaded.Predicates
{
    public sealed class GetMultiplePredicate
    {
        private readonly List<GetMultiplePredicateItem> _items;

        public GetMultiplePredicate()
        {
            _items = new List<GetMultiplePredicateItem>();
        }

        public IEnumerable<GetMultiplePredicateItem> Items => _items.AsReadOnly();

        public void Add<T>(IPredicate predicate, IList<ISort> sort = null) where T : class
        {
            _items.Add(new GetMultiplePredicateItem
            {
                Predicate = predicate,
                Type = typeof(T),
                Sort = sort
            });
        }

        public class GetMultiplePredicateItem
        {
            public IPredicate Predicate { get; set; }
            public Type Type { get; set; }
            public IList<ISort> Sort { get; set; }
        }
    }
}