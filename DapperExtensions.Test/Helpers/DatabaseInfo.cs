using System.Data;
using DapperExtensions.Sql;

namespace DapperExtensions.Test.Helpers
{
    public class DatabaseInfo
    {
        public IDbConnection Connection { get; set; }
        public ISqlDialect Dialect { get; set; }
    }
}