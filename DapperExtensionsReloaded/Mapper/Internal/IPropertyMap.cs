using System.Reflection;

namespace DapperExtensions.Mapper.Internal
{
    /// <summary>
    /// Maps an entity property to its corresponding column in the database.
    /// </summary>
    internal interface IPropertyMap
    {
        string Name { get; }
        string ColumnName { get; }
        bool Ignored { get; }
        bool IsReadOnly { get; }
        KeyType KeyType { get; }
        PropertyInfo PropertyInfo { get; }
    }
}