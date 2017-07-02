using System;

namespace DapperExtensionsReloaded.Mapper
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DatabaseEntityAttribute : Attribute
    {
        public string TableName { get; }
        public string SchemaName { get; set; }

        public DatabaseEntityAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
