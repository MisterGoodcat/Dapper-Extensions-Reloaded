using System.Data.Common;

namespace DapperExtensionsReloaded.Logging
{
    public sealed class DatabaseConnectionFactory
    {
        public DbConnection CreateMonitoringConnection(DbConnection connection, DatabaseOperationMonitor monitor)
        {
            return new DbConnectionProxy(connection, monitor);
        }
    }
}
