using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DapperExtensionsReloaded.Logging;
using DapperExtensionsReloaded.Mapper;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerDatabaseOperationMonitoringTests : SqlServerBaseFixture
    {
        [Fact]
        public async Task UsingNullPredicate_Returns_Count()
        {
            var results = new List<DatabaseCommandInfo>();
            var factory = new DatabaseConnectionFactory();
            var monitor = new DatabaseOperationMonitor
            {
                CommandExecuted = info => results.Add(info)
            };

            using (var monitoringConnection = factory.CreateMonitoringConnection(Connection, monitor))
            {
                await DapperExtensions.InsertAsync(monitoringConnection, new ManyDataTypes
                {
                    C1 = "Test",
                    C2 = "123", // char(3)
                    C3 = DateTime.Now,
                    C4 = DateTimeOffset.Now,
                    C5 = 4200000,
                    C6 = 4200,
                    C7 = 42,
                    C8 = new byte[] { 1, 2, 3 },
                    C9 = Guid.NewGuid(),
                    C10 = DateTime.Now,
                    C11 = DateTime.Now,
                    C12 = DateTime.Now,
                    C13 = 1.23456,
                    C14 = 1.234m,
                    C15 = 2.345m,
                    C16 = 1.23456f,
                    C17 = true
                });
                await DapperExtensions.CountAsync<ManyDataTypes>(monitoringConnection);
            }
            
            Assert.Equal(2, results.Count);
        }

        public class ManyDataTypes
        {
            [DatabaseColumn(IsIdentity = true)]
            public int Id { get; set; }
            public string C1 { get; set; }
            public string C2 { get; set; }
            public DateTime C3 { get; set; }
            public DateTimeOffset C4 { get; set; }
            public int C5 { get; set; }
            public short C6 { get; set; }
            public byte C7 { get; set; }
            public byte[] C8 { get; set; }
            public Guid C9 { get; set; }
            public DateTime C10 { get; set; }
            public DateTime C11 { get; set; }
            public DateTime C12 { get; set; }
            public double C13 { get; set; }
            public decimal C14 { get; set; }
            public decimal C15 { get; set; }
            public float C16 { get; set; }
            public bool C17 { get; set; }
        }
    }
}
