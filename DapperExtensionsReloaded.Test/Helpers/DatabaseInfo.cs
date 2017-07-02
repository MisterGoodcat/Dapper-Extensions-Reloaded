using System.Data;
using DapperExtensionsReloaded.Internal.Sql;

namespace DapperExtensionsReloaded.Test.Helpers
{
    internal class DatabaseInfo
    {
        public IDbConnection Connection { get; set; }
        public ISqlDialect Dialect { get; set; }
    }
}