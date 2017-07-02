using System.Collections.Generic;
using Dapper;

namespace DapperExtensionsReloaded.Internal
{
    internal class SequenceReaderResultReader : IMultipleResultReader
    {
        private readonly Queue<SqlMapper.GridReader> _items;

        public SequenceReaderResultReader(IEnumerable<SqlMapper.GridReader> items)
        {
            _items = new Queue<SqlMapper.GridReader>(items);
        }

        public IEnumerable<T> Read<T>()
        {
            var reader = _items.Dequeue();
            return reader.Read<T>();
        }
    }
}