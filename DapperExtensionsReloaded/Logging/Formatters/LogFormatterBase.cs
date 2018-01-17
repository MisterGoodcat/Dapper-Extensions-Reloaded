using System;
using System.Data.Common;
using System.Text;

namespace DapperExtensionsReloaded.Logging.Formatters
{
    internal abstract class LogFormatterBase : ILogFormatter
    {
        public string ToExecutionScript(DbCommandProxy command, DateTimeOffset timestamp, Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"-- Command type: {command.CommandType}, Timestamp: {timestamp:O}");
            if (ex != null)
            {
                sb.AppendLine($"-- Execution error: {ex.Message}");
            }

            sb.AppendLine(CreateUseDatabaseStatement(command));

            foreach (DbParameter parameter in command.Parameters)
            {
                try
                {
                    var parameterName = CreateCommandParameterNameText(parameter);
                    var parameterType = CreateCommandParameterTypeText(parameter);
                    var parameterValue = CreateCommandParameterValueText(parameter);
                    sb.AppendLine($"declare {parameterName} {parameterType} = {parameterValue}");
                }
                catch (Exception e)
                {
                    // safety net if something goes wrong during conversion in derived class
                    sb.AppendLine($"-- Error converting parameter {parameter.ParameterName}: {e.Message}");
                }
            }

            sb.AppendLine(command.CommandText);

            return sb.ToString();
        }
        
        protected abstract string CreateUseDatabaseStatement(DbCommandProxy command);
        protected abstract string CreateCommandParameterNameText(DbParameter parameter);
        protected abstract string CreateCommandParameterTypeText(DbParameter parameter);
        protected abstract string CreateCommandParameterValueText(DbParameter parameter);
    }
}
