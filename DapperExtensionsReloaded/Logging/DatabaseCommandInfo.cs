using System;
using System.Collections.Generic;
using System.Data.Common;

namespace DapperExtensionsReloaded.Logging
{
    public sealed class DatabaseCommandInfo
    {
        private readonly List<DatabaseCommandParameterInfo> _parameters = new List<DatabaseCommandParameterInfo>();
        
        public DateTimeOffset Timestamp { get; private set; }
        public string DatabaseName { get; private set; }
        public string CommandType { get; private set; }
        public string CommandText { get; private set; }
        public Exception ExecutionError { get; private set; }
        public string ExecutionScriptText { get; set; }

        public IReadOnlyCollection<DatabaseCommandParameterInfo> Parameters => _parameters;

        internal static DatabaseCommandInfo From(DbCommandProxy command, DateTimeOffset timestamp, string executionScriptText, Exception executionError = null)
        {
            var result = new DatabaseCommandInfo
            {
                Timestamp = timestamp,
                DatabaseName = command.Connection.Database,
                CommandType = command.CommandType.ToString(),
                CommandText = command.CommandText,
                ExecutionError = executionError,
                ExecutionScriptText = executionScriptText
            };

            foreach (DbParameter parameter in command.Parameters)
            {
                result._parameters.Add(new DatabaseCommandParameterInfo(parameter.DbType.ToString(), parameter.ParameterName, parameter.Value));
            }

            return result;
        }

        private DatabaseCommandInfo()
        {
        }
    }
}