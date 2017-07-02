using System.Collections.Generic;

namespace DapperExtensionsReloaded
{
    public interface IMultipleResultReader
    {
        IEnumerable<T> Read<T>();
    }
}