using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using Dapper;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DapperExtensions.Test.IntegrationTests.SqlServer
{
    public class SqlServerBaseFixture
    {
        private readonly bool _logSql;

        protected SqlServerBaseFixture(bool logSql = true)
        {
            _logSql = logSql;
        }

        protected SqlConnection Connection { get; set; }

        [SetUp]
        public virtual void Setup()
        {
            Connection = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=dapperTest;Integrated security=True;");
            Connection.Open();

            var files = new List<string>
            {
                ReadScriptFile("CreateAnimalTable"),
                ReadScriptFile("CreateFooTable"),
                ReadScriptFile("CreateMultikeyTable"),
                ReadScriptFile("CreatePersonTable"),
                ReadScriptFile("CreateCarTable"),
                ReadScriptFile("CreateCatTable")
            };

            foreach (var setupFile in files)
            {
                Connection.Execute(setupFile);
            }

            if (_logSql)
            {
                DapperExtensions.SqlLogger = (sql, parameters) =>
                {
                    string parametersText;

                    var dynamicParameters = parameters as DynamicParameters;
                    if (dynamicParameters != null)
                    {
                        parametersText = dynamicParameters.ToJson();
                    }
                    else
                    {
                        parametersText = JsonConvert.SerializeObject(parameters, Formatting.Indented);
                    }

                    Console.WriteLine($"SQL: {sql}{Environment.NewLine}Parameters:{Environment.NewLine}{parametersText}{Environment.NewLine}");
                };
            }
            else
            {
                DapperExtensions.SqlLogger = null;
            }
        }

        [TearDown]
        public virtual void TearDown()
        {
            Connection?.Close();
            Connection = null;
        }
        
        private string ReadScriptFile(string name)
        {
            var fileName = GetType().Namespace + ".Sql." + name + ".sql";
            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
            using (var sr = new StreamReader(s))
            {
                return sr.ReadToEnd();
            }
        }
    }
}