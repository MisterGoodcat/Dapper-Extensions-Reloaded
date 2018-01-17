namespace DapperExtensionsReloaded.Logging
{
    public sealed class DatabaseCommandParameterInfo
    {
        public string ParameterType { get; }
        public string ParameterName { get; }
        public object ParameterValue { get; }

        internal DatabaseCommandParameterInfo(string parameterType, string parameterName, object parameterValue)
        {
            ParameterType = parameterType;
            ParameterName = parameterName;
            ParameterValue = parameterValue;
        }
    }
}