using System;
using System.Collections.Generic;
using DapperExtensionsReloaded.Test.Data;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests.SqlServer
{
    public class SqlServerInsertTimesTests : SqlServerBaseFixture
    {
        private static readonly int cnt = 1000;
        
        [Fact]
        public void IdentityKey_UsingEntity()
        {
            var p = new Person
            {
                FirstName = "FirstName",
                LastName = "LastName",
                DateCreated = DateTime.Now,
                Active = true
            };
            Connection.Insert(p);
            var start = DateTime.Now;
            var ids = new List<int>();
            for (var i = 0; i < cnt; i++)
            {
                var p2 = new Person
                {
                    FirstName = "FirstName" + i,
                    LastName = "LastName" + i,
                    DateCreated = DateTime.Now,
                    Active = true
                };
                Connection.Insert(p2);
                ids.Add(p2.Id);
            }

            var total = DateTime.Now.Subtract(start).TotalMilliseconds;
            Console.WriteLine("Total Time:" + total);
            Console.WriteLine("Average Time:" + total / cnt);
        }

        [Fact]
        public void IdentityKey_UsingReturnValue()
        {
            var p = new Person
            {
                FirstName = "FirstName",
                LastName = "LastName",
                DateCreated = DateTime.Now,
                Active = true
            };
            Connection.Insert(p);
            var start = DateTime.Now;
            var ids = new List<int>();
            for (var i = 0; i < cnt; i++)
            {
                var p2 = new Person
                {
                    FirstName = "FirstName" + i,
                    LastName = "LastName" + i,
                    DateCreated = DateTime.Now,
                    Active = true
                };
                var id = Connection.Insert(p2);
                ids.Add(id);
            }

            var total = DateTime.Now.Subtract(start).TotalMilliseconds;
            Console.WriteLine("Total Time:" + total);
            Console.WriteLine("Average Time:" + total / cnt);
        }

        [Fact]
        public void GuidKey_UsingEntity()
        {
            var a = new Animal { Name = "Name" };
            Connection.Insert(a);
            var start = DateTime.Now;
            var ids = new List<Guid>();
            for (var i = 0; i < cnt; i++)
            {
                var a2 = new Animal { Name = "Name" + i };
                Connection.Insert(a2);
                ids.Add(a2.Id);
            }

            var total = DateTime.Now.Subtract(start).TotalMilliseconds;
            Console.WriteLine("Total Time:" + total);
            Console.WriteLine("Average Time:" + total / cnt);
        }

        [Fact]
        public void GuidKey_UsingReturnValue()
        {
            var a = new Animal { Name = "Name" };
            Connection.Insert(a);
            var start = DateTime.Now;
            var ids = new List<Guid>();
            for (var i = 0; i < cnt; i++)
            {
                var a2 = new Animal { Name = "Name" + i };
                var id = Connection.Insert(a2);
                ids.Add(id);
            }

            var total = DateTime.Now.Subtract(start).TotalMilliseconds;
            Console.WriteLine("Total Time:" + total);
            Console.WriteLine("Average Time:" + total / cnt);
        }

        [Fact]
        public void AssignKey_UsingEntity()
        {
            var ca = new Car { Id = string.Empty.PadLeft(15, '0'), Name = "Name" };
            Connection.Insert(ca);
            var start = DateTime.Now;
            var ids = new List<string>();
            for (var i = 0; i < cnt; i++)
            {
                var key = (i + 1).ToString().PadLeft(15, '0');
                var ca2 = new Car { Id = key, Name = "Name" + i };
                Connection.Insert(ca2);
                ids.Add(ca2.Id);
            }

            var total = DateTime.Now.Subtract(start).TotalMilliseconds;
            Console.WriteLine("Total Time:" + total);
            Console.WriteLine("Average Time:" + total / cnt);
        }

        [Fact]
        public void AssignKey_UsingReturnValue()
        {
            var ca = new Car { Id = string.Empty.PadLeft(15, '0'), Name = "Name" };
            Connection.Insert(ca);
            var start = DateTime.Now;
            var ids = new List<string>();
            for (var i = 0; i < cnt; i++)
            {
                var key = (i + 1).ToString().PadLeft(15, '0');
                var ca2 = new Car { Id = key, Name = "Name" + i };
                var id = Connection.Insert(ca2);
                ids.Add(id);
            }

            var total = DateTime.Now.Subtract(start).TotalMilliseconds;
            Console.WriteLine("Total Time:" + total);
            Console.WriteLine("Average Time:" + total / cnt);
        }
    }
}