using System.Linq;
using DapperExtensionsReloaded.Mapper.Internal;
using DapperExtensionsReloaded.Test.Helpers;
using Xunit;

namespace DapperExtensionsReloaded.Test.Mapper
{
    public class ClassMapperAutoMapMethodTests
    {
        [Fact]
        public void MapsAllProperties()
        {
            var mapper = GetMapper<FooWithIntId>();
            TestHelpers.TestProtected(mapper).RunMethod("AutoMapRemainingProperties");
            Assert.Equal(3, mapper.Properties.Count);
            Assert.Equal("FooId", mapper.Properties.ElementAt(0).ColumnName);
            Assert.Equal("FooId", mapper.Properties.ElementAt(0).Name);
            Assert.Equal("Value", mapper.Properties.ElementAt(1).ColumnName);
            Assert.Equal("Value", mapper.Properties.ElementAt(1).Name);
            Assert.Equal("BarId", mapper.Properties.ElementAt(2).ColumnName);
            Assert.Equal("BarId", mapper.Properties.ElementAt(2).Name);
        }

        [Fact]
        public void MakesFirstIntId_AIdentityKey()
        {
            var mapper = GetMapper<FooWithIntId>();
            TestHelpers.TestProtected(mapper).RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Identity, mapper.Properties.ElementAt(0).KeyType);
            Assert.Equal(KeyType.NotAKey, mapper.Properties.ElementAt(2).KeyType);
        }

        [Fact]
        public void MakesFirstGuidId_AGuidKey()
        {
            var mapper = GetMapper<FooWithGuidId>();
            TestHelpers.TestProtected(mapper).RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Assigned, mapper.Properties.ElementAt(0).KeyType);
            Assert.Equal(KeyType.NotAKey, mapper.Properties.ElementAt(2).KeyType);
        }

        [Fact]
        public void MakesFirstStringId_AAssignedKey()
        {
            var mapper = GetMapper<FooWithStringId>();
            TestHelpers.TestProtected(mapper).RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Assigned, mapper.Properties.ElementAt(0).KeyType);
            Assert.Equal(KeyType.NotAKey, mapper.Properties.ElementAt(2).KeyType);
        }

        [Fact]
        public void EnumerableDoesNotThrowException()
        {
            var mapper = GetMapper<Foo>();
            TestHelpers.TestProtected(mapper).RunMethod("AutoMapRemainingProperties");
            Assert.Equal(2, mapper.Properties.Count);
        }

        private ClassMapper<T> GetMapper<T>() where T : class
        {
            return new ClassMapper<T>();
        }
    }
}