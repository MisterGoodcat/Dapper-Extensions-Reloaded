using System;
using System.Linq;
using DapperExtensions.Mapper;
using NUnit.Framework;

namespace DapperExtensions.Test.Mapper
{
    [TestFixture]
    public class AttributeClassMapperFixture
    {
        [TestFixture]
        public class AttributeClassMapperTableName
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
                var m = GetMapper<Foo2>();
                Assert.AreEqual("OverriddenFoo", m.TableName);
            }
        }

        [TestFixture]
        public class AttributeClassMapperColumns
        { 
            [Test]
            public void Sets_IdPropertyToKeyWhenFirstProperty()
            {
                var m = GetMapper<Foo>();
                var map = m.Properties.Single(p => p.KeyType == KeyType.Identity);
                Assert.IsTrue(map.ColumnName == "Id");
            }

            [Test]
            public void Sets_IdPropertyToKeyWhenFoundInAttributes()
            {
                var m = GetMapper<Foo2>();
                var map = m.Properties.Single(p => p.KeyType == KeyType.Identity);
                Assert.IsTrue(map.ColumnName == "MyTotallyAutoIncrementedProperty");
            }

            [Test]
            public void Sets_ColumnNameToOverridenValue()
            {
                var m = GetMapper<Foo2>();
                var map = m.Properties.SingleOrDefault(p => p.ColumnName == "Blubber");
                Assert.NotNull(map);
            }

            [Test]
            public void Sets_IsReadOnly()
            {
                var m = GetMapper<Foo2>();
                var map = m.Properties.SingleOrDefault(p => p.ColumnName == "Blubber");
                Assert.IsTrue(map.IsReadOnly);
            }

            [Test]
            public void Sets_Ignored()
            {
                var m = GetMapper<Foo>();
                var map = m.Properties.SingleOrDefault(p => p.Ignored);
                Assert.IsTrue(map.ColumnName == "Name");
            }
        }
        
        private static AttributeClassMapper<T> GetMapper<T>() where T : class
        {
            return new AttributeClassMapper<T>();
        }
        
        private class Foo
        {
            public int Id { get; set; }

            [DatabaseColumn(IsIgnored = true)]
            public string Name { get; set; }
        }

        [DatabaseEntity("OverriddenFoo")]
        private class Foo2
        {
            [DatabaseColumn(IsIdentity = true)]
            public int MyTotallyAutoIncrementedProperty { get; set; }

            [DatabaseColumn(ColumnName = "Blubber", IsReadOnly = true)]
            public string Name { get; set; }
        }
    }
}