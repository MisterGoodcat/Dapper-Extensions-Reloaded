using DapperExtensionsReloaded.Internal.Sql;

namespace DapperExtensionsReloaded.Test.Sql
{
    public abstract class SqlServerDialectFixtureBase
    {
        internal SqlServerDialect Dialect;

        protected SqlServerDialectFixtureBase()
        {
            Dialect = new SqlServerDialect();
        }
    }
}