using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using Microsoft.Data.SqlClient;

namespace DapperExtensionsReloaded.Logging.Formatters
{
    internal sealed class SqlServerLogFormatter : LogFormatterBase
    {
        protected override string CreateUseDatabaseStatement(DbCommandProxy command)
        {
            return $"use [{command.Connection?.Database ?? string.Empty}]";
        }

        protected override string CreateCommandParameterNameText(DbParameter parameter)
        {
            return $"@{parameter.ParameterName}";
        }

        protected override string CreateCommandParameterTypeText(DbParameter parameter)
        {
            var sqlParameter = (SqlParameter)parameter;
            switch (sqlParameter.SqlDbType)
            {
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                    return "nvarchar(max)";
                case SqlDbType.BigInt:
                case SqlDbType.Int:
                case SqlDbType.SmallInt:
                case SqlDbType.TinyInt:
                    return "bigint";
                case SqlDbType.Decimal:
                    return "decimal(18,6)";
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return "money(6,4)";
            }
            
            return sqlParameter.SqlDbType.ToString();
        }

        protected override string CreateCommandParameterValueText(DbParameter parameter)
        {
            if (parameter.Value == null || parameter.Value is DBNull)
            {
                return "NULL";
            }

            var sqlParameter = (SqlParameter)parameter;
            switch (sqlParameter.SqlDbType)
            {
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                    return $"'{parameter.Value}'";
                case SqlDbType.BigInt:
                case SqlDbType.Int:
                case SqlDbType.SmallInt:
                case SqlDbType.TinyInt:
                    return parameter.Value.ToString();
                case SqlDbType.Bit:
                    return (bool)parameter.Value ? "1" : "0";
                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.VarBinary:
                    return $"##### NO VALUE ##### -- [binary data, raw type: {sqlParameter.SqlDbType}]";
                case SqlDbType.UniqueIdentifier:
                    return $"'{((Guid)parameter.Value):D}'";
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Time:
                    return ConvertToDateTimeBasedString(sqlParameter.Value, sqlParameter.SqlDbType, "yyyy-MM-dd HH:mm:ss.fff", "121");
                case SqlDbType.DateTimeOffset:
                    return ConvertToDateTimeBasedString(sqlParameter.Value, sqlParameter.SqlDbType, "O", "127");
                case SqlDbType.Decimal:
                    return $"{Convert.ToDecimal(sqlParameter.Value).ToString("0.######", CultureInfo.InvariantCulture)} -- raw .NET value: {sqlParameter.Value}";
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return $"{Convert.ToDecimal(sqlParameter.Value).ToString("0.####", CultureInfo.InvariantCulture)} -- raw .NET value: {sqlParameter.Value}"; // 4 decimals in SQL Server
                case SqlDbType.Float:
                case SqlDbType.Real:
                    return $"{Convert.ToDouble(sqlParameter.Value).ToString("0.######", CultureInfo.InvariantCulture)} -- raw .NET value: {sqlParameter.Value}";
                default:
                    return $"##### NO VALUE ##### -- [unsupported or unknown, raw type: {sqlParameter.SqlDbType}]";
            }
        }

        private string ConvertToDateTimeBasedString(object value, SqlDbType sqlDbType, string netFormat, string sqlFormat)
        {
            string valueText;
            if (value is DateTime dt)
            {
                valueText = dt.ToString(netFormat);
            }
            else if (value is DateTimeOffset dto)
            {
                valueText = dto.ToString(netFormat);
            }
            else
            {
                valueText = value.ToString();
            }
            
            return $"CONVERT({sqlDbType}, '{valueText}', {sqlFormat}) -- raw .NET value: {value}";
        }
    }
}