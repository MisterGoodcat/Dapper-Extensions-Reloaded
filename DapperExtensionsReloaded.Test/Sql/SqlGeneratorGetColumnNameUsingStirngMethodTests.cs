using System;
using System.Collections.Generic;
using DapperExtensionsReloaded.Mapper.Internal;
using Moq;
using Xunit;

namespace DapperExtensionsReloaded.Test.Sql
{
    public class SqlGeneratorGetColumnNameUsingStirngMethodTests : SqlGeneratorFixtureBase
    {
        [Fact]
        public void ThrowsExceptionWhenDoesNotFindProperty()
        {
            ClassMap.SetupGet(c => c.Properties).Returns(new List<IPropertyMap>()).Verifiable();
            var ex = Assert.Throws<ArgumentException>(() => Generator.Object.GetColumnName(ClassMap.Object, "property", true));
            Assert.Contains("Could not find 'property'", ex.Message);
            ClassMap.Verify();
        }

        [Fact]
        public void CallsGetColumnNameWithProperty()
        {
            var property = new Mock<IPropertyMap>();
            property.Setup(p => p.Name).Returns("property").Verifiable();
            ClassMap.SetupGet(c => c.Properties).Returns(new List<IPropertyMap> { property.Object }).Verifiable();
            Generator.Setup(g => g.GetColumnName(ClassMap.Object, property.Object, true)).Returns("ColumnName").Verifiable();
            var result = Generator.Object.GetColumnName(ClassMap.Object, "property", true);
            Assert.Equal("ColumnName", result);
            ClassMap.Verify();
            property.Verify();
            Generator.Verify();
        }
    }
}