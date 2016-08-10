using System.Data;
using DapperExtensions.Internal.Sql;

namespace DapperExtensions.Test.Helpers
{
    internal class DatabaseInfo
    {
        public IDbConnection Connection { get; set; }
        public ISqlDialect Dialect { get; set; }
    }
}