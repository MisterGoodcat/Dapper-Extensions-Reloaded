using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Dapper;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using NUnit.Framework;

namespace DapperExtensions.Test.IntegrationTests.SqlServer
{
    public class SqlServerBaseFixture
    {
        protected IDatabase Db;

        [SetUp]
        public virtual void Setup()
        {
            var connection = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=dapperTest;Integrated security=True;");
            var config = new DapperExtensionsConfiguration(typeof(AttributeClassMapper<>), new List<Assembly>(), new SqlServerDialect());
            var sqlGenerator = new SqlGeneratorImpl(config);
            Db = new Database(connection, sqlGenerator);
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
                connection.Execute(setupFile);
            }
        }

        public string ReadScriptFile(string name)
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