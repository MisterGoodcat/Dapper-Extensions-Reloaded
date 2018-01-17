using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DapperExtensionsReloaded.Logging
{
    internal class DbConnectionProxy : DbConnection
    {
        private readonly DbConnection _connection;
        private readonly DatabaseOperationMonitor _databaseOperationMonitor;

        protected override bool CanRaiseEvents => true;

        internal DbConnection InnerConnection => _connection;
        internal DatabaseOperationMonitor DatabaseOperationMonitor => _databaseOperationMonitor;

        public override string ConnectionString
        {
            get => _connection.ConnectionString;
            set => _connection.ConnectionString = value;
        }

        public override int ConnectionTimeout => _connection.ConnectionTimeout;
        public override string Database => _connection.Database;
        public override string DataSource => _connection.DataSource;
        public override string ServerVersion => _connection.ServerVersion;

        public override ConnectionState State => _connection.State;

        public DbConnectionProxy(DbConnection connection, DatabaseOperationMonitor databaseOperationMonitor)
        {
            _connection = connection;
            _databaseOperationMonitor = databaseOperationMonitor;
        }
        
        public override void ChangeDatabase(string databaseName)
        {
            _connection.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            _connection.Close();
        }

        public override void Open()
        {
            _connection.Open();
        }

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return _connection.OpenAsync(cancellationToken);
        }

        public override void EnlistTransaction(System.Transactions.Transaction transaction)
        {
            _connection.EnlistTransaction(transaction);
        }

        public override DataTable GetSchema()
        {
            return _connection.GetSchema();
        }

        public override DataTable GetSchema(string collectionName)
        {
            return _connection.GetSchema(collectionName);
        }

        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return _connection.GetSchema(collectionName, restrictionValues);
        }

        public override object InitializeLifetimeService()
        {
            return _connection.InitializeLifetimeService();
        }
        
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return _connection.BeginTransaction(isolationLevel);
        }
        
        protected override DbCommand CreateDbCommand()
        {
            return new DbCommandProxy(this, _databaseOperationMonitor, _connection.CreateCommand());
        }

        protected override void Dispose(bool disposing)
        {
            _connection.Dispose();
            base.Dispose(disposing);
        }
    }
}