using System;
using System.Linq;
using Dapper;

namespace DapperExtensions
{
    public static class DynamicParametersExtensions
    {
        private static readonly Type[] NumericTypes =
        {
            typeof(sbyte), typeof(short), typeof(int), typeof(long), 
            typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double), typeof(decimal)
        };

        public static string ToJson(this DynamicParameters dynamicParameters)
        {
            if (dynamicParameters == null)
            {
                return null;
            }

            if (!dynamicParameters.ParameterNames?.Any() ?? false)
            {
                return "{}";
            }

            var values = string.Join($",{Environment.NewLine}  ", dynamicParameters.ParameterNames.Select(x => $"\"{x}\": {GetDynamicParameterValueText(dynamicParameters, x)}"));
            var parametersText = $"{{{Environment.NewLine}  {values}{Environment.NewLine}}}";
            return parametersText;
        }
        
        private static string GetDynamicParameterValueText(DynamicParameters dynamicParameters, string parameterName)
        {
            var value = dynamicParameters.Get<dynamic>(parameterName);
            if (value == null)
            {
                return "NULL";
            }

            var valueType = (Type)value.GetType();
            if (NumericTypes.Contains(valueType) || valueType == typeof(bool))
            {
                return value.ToString();
            }
            
            if (value is DateTime || value is DateTimeOffset )
            {
                return $"\"{value.ToString("O")}\"";
            }

            return $"\"{value.ToString()}\"";
        }
    }
}
