using System;
using System.Linq;
using System.Numerics;
using DapperExtensionsReloaded.Mapper.Internal;
using DapperExtensionsReloaded.Test.Helpers;
using Xunit;

namespace DapperExtensionsReloaded.Test.Mapper
{
    public class ClassMapperAutoMapIdTests
    {
        [Fact]
        public void AutoMapSetsFirstIdToIdentityWhenByte()
        {
            var mapper = GetMapper<Test1<byte>>();
            mapper.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Identity, mapper.Properties.ElementAt(0).KeyType);

            var mapper2 = GetMapper<Test1<byte?>>();
            mapper2.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Identity, mapper2.Properties.ElementAt(0).KeyType);
        }

        [Fact]
        public void AutoMapSetsFirstIdToIdentityWhenSByte()
        {
            var mapper = GetMapper<Test1<sbyte>>();
            mapper.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Identity, mapper.Properties.ElementAt(0).KeyType);

            var mapper2 = GetMapper<Test1<sbyte?>>();
            mapper2.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Identity, mapper2.Properties.ElementAt(0).KeyType);
        }

        [Fact]
        public void AutoMapSetsFirstIdToIdentityWhenInt16()
        {
            var mapper = GetMapper<Test1<short>>();
            mapper.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Identity, mapper.Properties.ElementAt(0).KeyType);

            var mapper2 = GetMapper<Test1<short?>>();
            mapper2.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Identity, mapper2.Properties.ElementAt(0).KeyType);
        }

        [Fact]
        public void AutoMapSetsFirstIdToIdentityWhenUnsignedInt16()
        {
            var mapper = GetMapper<Test1<ushort>>();
            mapper.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Identity, mapper.Properties.ElementAt(0).KeyType);

            var mapper2 = GetMapper<Test1<ushort?>>();
            mapper2.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Identity, mapper2.Properties.ElementAt(0).KeyType);
        }

        [Fact]
        public void AutoMapSetsFirstIdToIdentityWhenInt32()
        {
            var mapper = GetMapper<Test1<int>>();
            mapper.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Identity, mapper.Properties.ElementAt(0).KeyType);

            var mapper2 = GetMapper<Test1<int?>>();
            mapper2.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Identity, mapper2.Properties.ElementAt(0).KeyType);
        }

        [Fact]
        public void AutoMapSetsFirstIdToIdentityWhenUnsignedInt32()
        {
            var mapper = GetMapper<Test1<uint>>();
            mapper.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Identity, mapper.Properties.ElementAt(0).KeyType);

            var mapper2 = GetMapper<Test1<uint?>>();
            mapper2.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Identity, mapper2.Properties.ElementAt(0).KeyType);
        }

        [Fact]
        public void AutoMapSetsFirstIdToIdentityWhenInt64()
        {
            var mapper = GetMapper<Test1<long>>();
            mapper.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Identity, mapper.Properties.ElementAt(0).KeyType);

            var mapper2 = GetMapper<Test1<long?>>();
            mapper2.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Identity, mapper2.Properties.ElementAt(0).KeyType);
        }

        [Fact]
        public void AutoMapSetsFirstIdToIdentityWhenUnsignedInt64()
        {
            var mapper = GetMapper<Test1<ulong>>();
            mapper.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Identity, mapper.Properties.ElementAt(0).KeyType);

            var mapper2 = GetMapper<Test1<ulong?>>();
            mapper2.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Identity, mapper2.Properties.ElementAt(0).KeyType);
        }

        [Fact]
        public void AutoMapSetsFirstIdToIdentityWhenBigInteger()
        {
            var mapper = GetMapper<Test1<BigInteger>>();
            mapper.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Identity, mapper.Properties.ElementAt(0).KeyType);

            var mapper2 = GetMapper<Test1<BigInteger?>>();
            mapper2.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Identity, mapper2.Properties.ElementAt(0).KeyType);
        }

        [Fact]
        public void AutoMapSetsFirstIdToGuidWhenGuid()
        {
            var mapper = GetMapper<Test1<Guid>>();
            mapper.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Assigned, mapper.Properties.ElementAt(0).KeyType);

            var mapper2 = GetMapper<Test1<Guid?>>();
            mapper2.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Assigned, mapper2.Properties.ElementAt(0).KeyType);
        }

        [Fact]
        public void AutoMapSetsFirstIdToAssignedWhenNotKeyType()
        {
            var mapper = GetMapper<Test1<string>>();
            mapper.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Assigned, mapper.Properties.ElementAt(0).KeyType);

            var mapper2 = GetMapper<Test1<bool>>();
            mapper2.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Assigned, mapper2.Properties.ElementAt(0).KeyType);

            var mapper3 = GetMapper<Test1<bool?>>();
            mapper3.TestProtected().RunMethod("AutoMapRemainingProperties");
            Assert.Equal(KeyType.Assigned, mapper3.Properties.ElementAt(0).KeyType);
        }

        private ClassMapper<T> GetMapper<T>() where T : class
        {
            return new ClassMapper<T>();
        }

        private class Test1<T>
        {
            public T SomeId { get; set; }
        }
    }
}