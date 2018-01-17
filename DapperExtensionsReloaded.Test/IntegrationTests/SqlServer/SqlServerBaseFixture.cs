using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Dapper;
using Newtonsoft.Json;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    [Collection("SqlServerIntegrationTests")]
    public class SqlServerBaseFixture : IDisposable
    {
        protected SqlConnection Connection { get; set; }

        public SqlServerBaseFixture()
        {
            Connection = new SqlConnection("Data Source=.;Initial Catalog=DapperTest;Integrated security=True;");
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
        }
        
        public void Dispose()
        {
            Connection?.Close();
            Connection = null;
        }
        
        private string ReadScriptFile(string name)
        {
            var fileName = GetType().Namespace + ".Sql." + name + ".sql";
            using (var s = GetType().GetTypeInfo().Assembly.GetManifestResourceStream(fileName))
            using (var sr = new StreamReader(s))
            {
                return sr.ReadToEnd();
            }
        }
    }
}