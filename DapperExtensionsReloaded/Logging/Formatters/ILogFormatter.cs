using System;

namespace DapperExtensionsReloaded.Logging.Formatters
{
    internal interface ILogFormatter
    {
        string ToExecutionScript(DbCommandProxy command, DateTimeOffset timestamp, Exception ex);
    }
}