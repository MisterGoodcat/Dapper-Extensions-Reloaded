using System.Collections.Generic;

namespace DapperExtensions
{
    public interface IMultipleResultReader
    {
        IEnumerable<T> Read<T>();
    }
}