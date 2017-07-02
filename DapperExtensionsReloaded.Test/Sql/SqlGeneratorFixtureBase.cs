using DapperExtensionsReloaded.Internal;
using DapperExtensionsReloaded.Internal.Sql;
using DapperExtensionsReloaded.Mapper.Internal;
using Moq;

namespace DapperExtensionsReloaded.Test.Sql
{
    public abstract class SqlGeneratorFixtureBase
    {
        internal Mock<IDapperExtensionsConfiguration> Configuration;

        internal Mock<SqlGeneratorImpl> Generator;
        internal Mock<ISqlDialect> Dialect;
        internal Mock<IClassMapper> ClassMap;

        protected SqlGeneratorFixtureBase()
        {
            Configuration = new Mock<IDapperExtensionsConfiguration>();
            Dialect = new Mock<ISqlDialect>();
            ClassMap = new Mock<IClassMapper>();

            Dialect.SetupGet(c => c.ParameterPrefix).Returns('@');
            Configuration.SetupGet(c => c.Dialect).Returns(Dialect.Object).Verifiable();

            Generator = new Mock<SqlGeneratorImpl>(Configuration.Object) { CallBase = true };
        }
    }
}