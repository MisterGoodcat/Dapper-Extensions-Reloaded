using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DapperExtensionsReloaded.Logging
{
    internal class DbCommandProxy : DbCommand
    {
        private DbConnection _connection;
        private DatabaseOperationMonitor _databaseOperationMonitor;
        private readonly DbCommand _command;
        
        protected override DbConnection DbConnection { 
            get => _connection;
            set
            {
                if (value is MonitoringDbConnection monitoringDbConnection)
                {
                    _databaseOperationMonitor = monitoringDbConnection.DatabaseOperationMonitor;
                    _connection = monitoringDbConnection.MonitoredDbConnection;
                }
                else
                {
                    _connection = value;
                }
            }
        }

        protected override DbParameterCollection DbParameterCollection => _command.Parameters;

        protected override DbTransaction DbTransaction 
        { 
            get => _command.Transaction;
            set => _command.Transaction = value;
        }

        protected override bool CanRaiseEvents => true;

        public override string CommandText
        {
            get => _command.CommandText;
            set => _command.CommandText = value;
        }

        public override int CommandTimeout 
        { 
            get => _command.CommandTimeout;
            set => _command.CommandTimeout = value;
        }

        public override CommandType CommandType 
        { 
            get => _command.CommandType;
            set => _command.CommandType = value;
        }
        
        public override bool DesignTimeVisible 
        { 
            get => _command.DesignTimeVisible;
            set => _command.DesignTimeVisible = value;
        }

        public override UpdateRowSource UpdatedRowSource
        { 
            get => _command.UpdatedRowSource;
            set => _command.UpdatedRowSource = value;
        }

        public override ISite Site
        {
            get => _command.Site;
            set => _command.Site = value;
        }

        public DbCommandProxy(MonitoringDbConnection connection, DatabaseOperationMonitor databaseOperationMonitor, DbCommand command)
        {
            _connection = connection.MonitoredDbConnection;
            _databaseOperationMonitor = databaseOperationMonitor;
            _command = command;
        }

        public override object InitializeLifetimeService()
        {
            return _command.InitializeLifetimeService();
        }
        
        public override async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _command.ExecuteNonQueryAsync(cancellationToken);

                NotifyMonitoringOfExecutedCommand();

                return result;
            }
            catch (Exception ex)
            {
                NotifyMonitoringOfExecutedCommand(ex);
                throw;
            }
        }

        public override async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result =  await _command.ExecuteScalarAsync(cancellationToken);
                
                NotifyMonitoringOfExecutedCommand();

                return result;
            }
            catch (Exception ex)
            {
                NotifyMonitoringOfExecutedCommand(ex);
                throw;
            }
        }

        public override void Cancel()
        {
            _command.Cancel();
        }

        public override int ExecuteNonQuery()
        {
            try
            {
                var result = _command.ExecuteNonQuery();
                
                NotifyMonitoringOfExecutedCommand();

                return result;
            }
            catch (Exception ex)
            {
                NotifyMonitoringOfExecutedCommand(ex);
                throw;
            }
        }
        
        public override object ExecuteScalar()
        {
            try
            {
                var result = _command.ExecuteScalar();
                
                NotifyMonitoringOfExecutedCommand();

                return result;
            }
            catch (Exception ex)
            {
                NotifyMonitoringOfExecutedCommand(ex);
                throw;
            }
        }

        public override void Prepare()
        {
            _command.Prepare();
        }

        protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _command.ExecuteReaderAsync(behavior, cancellationToken);
                
                NotifyMonitoringOfExecutedCommand();

                return result;
            }
            catch (Exception ex)
            {
                NotifyMonitoringOfExecutedCommand(ex);
                throw;
            }
        }
        
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            try
            {
                var result =  _command.ExecuteReader(behavior);
                
                NotifyMonitoringOfExecutedCommand();

                return result;
            }
            catch (Exception ex)
            {
                NotifyMonitoringOfExecutedCommand(ex);
                throw;
            }
        }

        protected override DbParameter CreateDbParameter()
        {
            return _command.CreateParameter();
        }

        protected override void Dispose(bool disposing)
        {
            _command.Dispose();
            base.Dispose(disposing);
        }

        private void NotifyMonitoringOfExecutedCommand(Exception ex = null)
        {
            if (_databaseOperationMonitor?.CommandExecuted == null)
            {
                return;
            }

            var timestamp = DateTimeOffset.Now;
            var executionScript = DapperExtensions.LogFormatter.ToExecutionScript(this, timestamp, ex);
            var info = DatabaseCommandInfo.From(this, timestamp, executionScript, ex);
            _databaseOperationMonitor.CommandExecuted.Invoke(info);
        }
    }
}