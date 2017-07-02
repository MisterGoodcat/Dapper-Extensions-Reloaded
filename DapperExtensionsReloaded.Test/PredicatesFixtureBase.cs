using DapperExtensionsReloaded.Internal;
using DapperExtensionsReloaded.Internal.Sql;
using Moq;

namespace DapperExtensionsReloaded.Test
{
    public abstract class PredicatesFixtureBase
    {
        internal Mock<ISqlDialect> SqlDialect;
        internal Mock<ISqlGenerator> Generator;
        internal Mock<IDapperExtensionsConfiguration> Configuration;

        protected PredicatesFixtureBase()
        {
            SqlDialect = new Mock<ISqlDialect>();
            Generator = new Mock<ISqlGenerator>();
            Configuration = new Mock<IDapperExtensionsConfiguration>();

            SqlDialect.SetupGet(c => c.ParameterPrefix).Returns('@');
            Configuration.SetupGet(c => c.Dialect).Returns(SqlDialect.Object);
            Generator.SetupGet(c => c.Configuration).Returns(Configuration.Object);
        }
    }
}