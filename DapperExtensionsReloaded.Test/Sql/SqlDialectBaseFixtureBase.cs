namespace DapperExtensionsReloaded.Test.Sql
{
    public abstract class SqlDialectBaseFixtureBase
    {
        internal TestDialect Dialect;

        protected SqlDialectBaseFixtureBase()
        {
            Dialect = new TestDialect();
        }
    }
}