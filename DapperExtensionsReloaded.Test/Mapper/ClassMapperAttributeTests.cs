using System.Linq;
using DapperExtensionsReloaded.Mapper;
using DapperExtensionsReloaded.Mapper.Internal;
using Xunit;

namespace DapperExtensionsReloaded.Test.Mapper
{
    public class ClassMapperAttributeTests
    {
        [Fact]
        public void Constructor_ReturnsProperName()
        {
            var m = GetMapper<Foo>();
            Assert.Equal("Foo", m.TableName);
        }

        [Fact]
        public void SettingTableName_ReturnsProperName()
        {
            var m = GetMapper<Foo2>();
            Assert.Equal("OverriddenFoo", m.TableName);
        }

        [Fact]
        public void Sets_IdPropertyToKey()
        {
            var m = GetMapper<Foo>();
            var map = m.Properties.Single(p => p.KeyType == KeyType.Identity);
            Assert.True(map.ColumnName == "Id");
        }

        [Fact]
        public void Sets_IdPropertyToKeyWhenFoundInAttributes()
        {
            var m = GetMapper<Foo2>();
            var map = m.Properties.Single(p => p.KeyType == KeyType.Identity);
            Assert.True(map.ColumnName == "MyTotallyAutoIncrementedProperty");
        }

        [Fact]
        public void Sets_ColumnNameToOverridenValue()
        {
            var m = GetMapper<Foo2>();
            var map = m.Properties.SingleOrDefault(p => p.ColumnName == "Blubber");
            Assert.NotNull(map);
        }

        [Fact]
        public void Sets_IsReadOnly()
        {
            var m = GetMapper<Foo2>();
            var map = m.Properties.SingleOrDefault(p => p.ColumnName == "Blubber");
            Assert.True(map.IsReadOnly);
        }

        [Fact]
        public void Sets_Ignored()
        {
            var m = GetMapper<Foo>();
            var map = m.Properties.SingleOrDefault(p => p.Ignored);
            Assert.True(map.ColumnName == "Name");
        }
        
        private static ClassMapper<T> GetMapper<T>() where T : class

        {
            return new ClassMapper<T>();
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