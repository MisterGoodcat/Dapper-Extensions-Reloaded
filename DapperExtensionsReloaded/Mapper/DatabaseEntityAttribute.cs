using System;

namespace DapperExtensionsReloaded.Mapper
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DatabaseEntityAttribute : Attribute
    {
        /// <summary>
        /// The corresponding table name for the attributed entity.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// The schema name the corresponding table belongs to.
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="DatabaseEntityAttribute"/> type with the given table name.
        /// </summary>
        /// <param name="tableName">The table name of the attributed entity.</param>
        public DatabaseEntityAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
