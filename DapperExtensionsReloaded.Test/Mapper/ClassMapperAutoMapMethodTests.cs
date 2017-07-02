using System;
using System.Reflection;
using DapperExtensionsReloaded.Internal;
using DapperExtensionsReloaded.Mapper.Internal;
using DapperExtensionsReloaded.Test.Helpers;
using Moq;
using Xunit;

namespace DapperExtensionsReloaded.Test.Mapper
{
    public class ClassMapperAutoMapMethodTests
    {
        [Fact]
        public void MapsAllProperties()
        {
            var mapper = GetMapper<FooWithIntId>();
            TestHelpers.TestProtected(mapper).RunMethod("AutoMap");
            Assert.Equal(3, mapper.Properties.Count);
            Assert.Equal((string)"FooId", (string)mapper.Properties[0].ColumnName);
            Assert.Equal((string)"FooId", (string)mapper.Properties[0].Name);
            Assert.Equal((string)"Value", (string)mapper.Properties[1].ColumnName);
            Assert.Equal((string)"Value", (string)mapper.Properties[1].Name);
            Assert.Equal((string)"BarId", (string)mapper.Properties[2].ColumnName);
            Assert.Equal((string)"BarId", (string)mapper.Properties[2].Name);
        }

        [Fact]
        public void MakesFirstIntId_AIdentityKey()
        {
            var mapper = GetMapper<FooWithIntId>();
            TestHelpers.TestProtected(mapper).RunMethod("AutoMap");
            Assert.Equal(KeyType.Identity, mapper.Properties[0].KeyType);
            Assert.Equal(KeyType.NotAKey, mapper.Properties[2].KeyType);
        }

        [Fact]
        public void MakesFirstGuidId_AGuidKey()
        {
            var mapper = GetMapper<FooWithGuidId>();
            TestHelpers.TestProtected(mapper).RunMethod("AutoMap");
            Assert.Equal(KeyType.Guid, mapper.Properties[0].KeyType);
            Assert.Equal(KeyType.NotAKey, mapper.Properties[2].KeyType);
        }

        [Fact]
        public void MakesFirstStringId_AAssignedKey()
        {
            var mapper = GetMapper<FooWithStringId>();
            TestHelpers.TestProtected(mapper).RunMethod("AutoMap");
            Assert.Equal(KeyType.Assigned, mapper.Properties[0].KeyType);
            Assert.Equal(KeyType.NotAKey, mapper.Properties[2].KeyType);
        }

        [Fact]
        public void DoesNotMapAlreadyMappedProperties()
        {
            var property = new Mock<IPropertyMap>();
            property.SetupGet(p => p.Name).Returns("FooId");
            property.SetupGet(p => p.KeyType).Returns(KeyType.Assigned);

            var mapper = GetMapper<FooWithIntId>();
            mapper.Properties.Add(property.Object);
            TestHelpers.TestProtected(mapper).RunMethod("AutoMap");
            Assert.Equal(3, mapper.Properties.Count);
            Assert.Equal(mapper.Properties[0], property.Object);
            Assert.Equal(KeyType.NotAKey, mapper.Properties[1].KeyType);
            Assert.Equal(KeyType.NotAKey, mapper.Properties[2].KeyType);
        }

        [Fact]
        public void EnumerableDoesNotThrowException()
        {
            var mapper = GetMapper<Foo>();
            TestHelpers.TestProtected(mapper).RunMethod("AutoMap");
            Assert.Equal(2, mapper.Properties.Count);
        }

        [Fact]
        public void IgnoringAnEnumerableDoesNotCauseError()
        {
            var mapper = new TestMapper<Foo>();
            mapper.Map(m => m.List).Ignore();
            TestHelpers.TestProtected(mapper).RunMethod("AutoMap");
            Assert.Equal(2, mapper.Properties.Count);
        }

        [Fact]
        public void DoesNotMapPropertyWhenCanMapIsFalse()
        {
            var mapper = new TestMapper<Foo>();
            Func<Type, PropertyInfo, bool> canMap = (t, p) => ReflectionHelper.IsSimpleType(p.PropertyType);
            TestHelpers.TestProtected(mapper).RunMethod("AutoMap", canMap);
            Assert.Equal(1, mapper.Properties.Count);
        }

        private ClassMapper<T> GetMapper<T>() where T : class
        {
            return new ClassMapper<T>();
        }
    }
}