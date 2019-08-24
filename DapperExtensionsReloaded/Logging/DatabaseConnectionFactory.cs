using System.Data.Common;

namespace DapperExtensionsReloaded.Logging
{
    public sealed class DatabaseConnectionFactory
    {
        public MonitoringDbConnection CreateMonitoringConnection(DbConnection connection, DatabaseOperationMonitor monitor)
        {
            return new MonitoringDbConnection(connection, monitor);
        }

        public MonitoringDbConnection CreateMonitoringConnection(DbConnection connection, DatabaseOperationMonitor monitor, DatabaseOperationMonitoringOptions options)
        {
            return new MonitoringDbConnection(connection, monitor, options);
        }
    }
}
