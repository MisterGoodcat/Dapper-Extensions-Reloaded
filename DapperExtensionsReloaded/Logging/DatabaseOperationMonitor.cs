using System;

namespace DapperExtensionsReloaded.Logging
{
    public sealed class DatabaseOperationMonitor
    {
        public Action<DatabaseCommandInfo> CommandExecuted { get; set; }
    }
}