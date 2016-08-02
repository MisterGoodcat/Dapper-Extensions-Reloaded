using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Dapper;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using NUnit.Framework;

namespace DapperExtensions.Test.IntegrationTests.SqlCe
{
    public class SqlCeBaseFixture
    {
        protected IDatabase Db;

        [SetUp]
        public virtual void Setup()
        {
            var connectionString = string.Format("Data Source=.\\dapperTest_{0}.sdf", Guid.NewGuid());
            var connectionParts = connectionString.Split(';');
            var file = connectionParts
                .ToDictionary(k => k.Split('=')[0], v => v.Split('=')[1])
                .Where(d => d.Key.Equals("Data Source", StringComparison.OrdinalIgnoreCase))
                .Select(k => k.Value).Single();

            if (File.Exists(file))
            {
                File.Delete(file);
            }

            using (var ce = new SqlCeEngine(connectionString))
            {
                ce.CreateDatabase();
            }

            var connection = new SqlCeConnection(connectionString);
            var config = new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), new SqlCeDialect());
            var sqlGenerator = new SqlGeneratorImpl(config);
            Db = new Database(connection, sqlGenerator);

            var files = new List<string>
            {
                ReadScriptFile("CreateAnimalTable"),
                ReadScriptFile("CreateFooTable"),
                ReadScriptFile("CreateMultikeyTable"),
                ReadScriptFile("CreatePersonTable"),
                ReadScriptFile("CreateCarTable")
            };

            foreach (var setupFile in files)
            {
                connection.Execute(setupFile);
            }
        }

        [TearDown]
        public void TearDown()
        {
            var databaseName = Db.Connection.Database;
            if (!File.Exists(databaseName))
            {
                return;
            }

            var i = 10;
            while (IsDatabaseInUse(databaseName) && i > 0)
            {
                i--;
                Thread.Sleep(1000);
            }

            if (i > 0)
            {
                File.Delete(databaseName);
            }
        }

        public static bool IsDatabaseInUse(string databaseName)
        {
            FileStream fs = null;
            try
            {
                var fi = new FileInfo(databaseName);
                fs = fi.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                return false;
            }
            catch (Exception)
            {
                return true;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
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