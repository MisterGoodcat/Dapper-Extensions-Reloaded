using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DapperExtensionsReloaded.Logging
{
    internal class DbCommandProxy : DbCommand
    {
        private DbConnection _connection;
        private DatabaseOperationMonitor _databaseOperationMonitor;
        private readonly DatabaseOperationMonitoringOptions _options;
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

        public DbCommandProxy(MonitoringDbConnection connection, DatabaseOperationMonitor databaseOperationMonitor, DatabaseOperationMonitoringOptions options, DbCommand command)
        {
            _connection = connection.MonitoredDbConnection;
            _databaseOperationMonitor = databaseOperationMonitor;
            _options = options;
            _command = command;
        }

        public override object InitializeLifetimeService()
        {
            return _command.InitializeLifetimeService();
        }
        
        public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            return DoExecuteCommandAsync(() => _command.ExecuteNonQueryAsync(cancellationToken));
        }

        public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            return DoExecuteCommandAsync(() => _command.ExecuteScalarAsync(cancellationToken));
        }

        public override void Cancel()
        {
            _command.Cancel();
        }

        public override int ExecuteNonQuery()
        {
            return DoExecuteCommand(_command.ExecuteNonQuery);
        }
        
        public override object ExecuteScalar()
        {
            return DoExecuteCommand(_command.ExecuteScalar);
        }

        public override void Prepare()
        {
            _command.Prepare();
        }

        protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            return DoExecuteCommandAsync(() => _command.ExecuteReaderAsync(behavior, cancellationToken));
        }
        
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return DoExecuteCommand(() => _command.ExecuteReader(behavior));
        }

        private T DoExecuteCommand<T>(Func<T> func)
        {
            var sw = _options.ProfileExecution ? Stopwatch.StartNew() : null;

            try
            {
                var result = func();
                var duration = sw != null ? Math.Max(1, sw.ElapsedMilliseconds) : 0;
                NotifyMonitoringOfExecutedCommand(duration);

                return result;
            }
            catch (Exception ex)
            {
                var duration = sw != null ? Math.Max(1, sw.ElapsedMilliseconds) : 0;
                NotifyMonitoringOfExecutedCommand(duration, ex);
                throw;
            }
        }

        private async Task<T> DoExecuteCommandAsync<T>(Func<Task<T>> func)
        {
            var sw = _options.ProfileExecution ? Stopwatch.StartNew() : null;

            try
            {
                var result = await func().ConfigureAwait(false);
                var duration = sw != null ? Math.Max(1, sw.ElapsedMilliseconds) : 0;
                NotifyMonitoringOfExecutedCommand(duration);

                return result;
            }
            catch (Exception ex)
            {
                var duration = sw != null ? Math.Max(1, sw.ElapsedMilliseconds) : 0;
                NotifyMonitoringOfExecutedCommand(duration, ex);
                throw;
            }
        }

        protected override DbParameter CreateDbParameter()
        {
            return _command.CreateParameter();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _command.Dispose();
            }

            base.Dispose(disposing);
        }

        private void NotifyMonitoringOfExecutedCommand(long duration, Exception ex = null)
        {
            if (_databaseOperationMonitor?.CommandExecuted == null)
            {
                return;
            }

            var timestamp = DateTimeOffset.Now;
            var executionScript = DapperExtensions.LogFormatter.ToExecutionScript(this, timestamp, ex);
            var info = DatabaseCommandInfo.From(this, timestamp, duration, executionScript, ex);
            _databaseOperationMonitor.CommandExecuted.Invoke(info);
        }
    }
}