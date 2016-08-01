﻿using System;
using System.Linq;
using DapperExtensions.Mapper;
using NUnit.Framework;

namespace DapperExtensions.Test.Mapper
{
    [TestFixture]
    public class AutoClassMapperFixture
    {
        [TestFixture]
        public class AutoClassMapperTableName
        {
            [Test]
            public void Constructor_ReturnsProperName()
            {
                var m = GetMapper<Foo>();
                Assert.AreEqual("Foo", m.TableName);
            }

            [Test]
            public void SettingTableName_ReturnsProperName()
            {
                var m = GetMapper<Foo>();
                m.Table("Barz");
                Assert.AreEqual("Barz", m.TableName);
            }

            [Test]
            public void Sets_IdPropertyToKeyWhenFirstProperty()
            {
                var m = GetMapper<IdIsFirst>();
                var map = m.Properties.Single(p => p.KeyType == KeyType.Guid);
                Assert.IsTrue(map.ColumnName == "Id");
            }

            [Test]
            public void Sets_IdPropertyToKeyWhenFoundInClass()
            {
                var m = GetMapper<IdIsSecond>();
                var map = m.Properties.Single(p => p.KeyType == KeyType.Guid);
                Assert.IsTrue(map.ColumnName == "Id");
            }

            [Test]
            public void Sets_IdFirstPropertyEndingInIdWhenNoIdPropertyFound()
            {
                var m = GetMapper<IdDoesNotExist>();
                var map = m.Properties.Single(p => p.KeyType == KeyType.Guid);
                Assert.IsTrue(map.ColumnName == "SomeId");
            }

            private AutoClassMapper<T> GetMapper<T>() where T : class
            {
                return new AutoClassMapper<T>();
            }
        }

        [TestFixture]
        public class CustomAutoMapperTableName
        {
            [Test]
            public void ReturnsProperPluralization()
            {
                var m = GetMapper<Foo>();
                Assert.AreEqual("Foo", m.TableName);
            }

            [Test]
            public void ReturnsProperResultsForExceptions()
            {
                var m = GetMapper<Foo2>();
                Assert.AreEqual("TheFoo", m.TableName);
            }

            private CustomAutoMapper<T> GetMapper<T>() where T : class
            {
                return new CustomAutoMapper<T>();
            }

            public class CustomAutoMapper<T> : AutoClassMapper<T> where T : class
            {
                public override void Table(string tableName)
                {
                    if (tableName.Equals("Foo2", StringComparison.CurrentCultureIgnoreCase))
                    {
                        TableName = "TheFoo";
                    }
                    else
                    {
                        base.Table(tableName);
                    }
                }
            }
        }

        private class Foo
        {
            public Guid Id { get; set; }
            public Guid ParentId { get; set; }
        }

        private class Foo2
        {
            public Guid ParentId { get; set; }
            public Guid Id { get; set; }
        }


        private class IdIsFirst
        {
            public Guid Id { get; set; }
            public Guid ParentId { get; set; }
        }

        private class IdIsSecond
        {
            public Guid ParentId { get; set; }
            public Guid Id { get; set; }
        }

        private class IdDoesNotExist
        {
            public Guid SomeId { get; set; }
            public Guid ParentId { get; set; }
        }
    }
}