using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Dapper;
using DapperExtensions.Mapper;
using NUnit.Framework;

namespace DapperExtensions.Test.IntegrationTests.SqlServer
{
    public class SqlServerBaseFixture
    {
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