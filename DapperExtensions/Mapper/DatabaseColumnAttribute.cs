﻿using System;

namespace DapperExtensions.Mapper
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DatabaseColumnAttribute : Attribute
    {
        /// <summary>
        /// The database column name for this property.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Whether this property is completely ignored, i.e. not read nor written to.
        /// </summary>
        public bool IsIgnored { get; set; }

        /// <summary>
        /// Whether this property is read-only, i.e. generated by the database as computed column etc., meaning it's read but not written to.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Whether this property is the identity, i.e. it's read after insert but never written to.
        /// </summary>
        public bool IsIdentity { get; set; }
    }
}
