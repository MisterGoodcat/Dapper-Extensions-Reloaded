using System.Collections.Generic;
using DapperExtensionsReloaded.Mapper.Internal;
using Moq;
using Xunit;

namespace DapperExtensionsReloaded.Test.Sql
{
    public class SqlGeneratorBuildSelectColumnsMethodTests : SqlGeneratorFixtureBase
    {
        [Fact]
        public void GeneratesSql()
        {
            var property1 = new Mock<IPropertyMap>();
            var property2 = new Mock<IPropertyMap>();
            var properties = new List<IPropertyMap>
            {
                property1.Object,
                property2.Object
            };

            Generator.Setup(g => g.GetColumnName(ClassMap.Object, property1.Object, true)).Returns("Column1").Verifiable();
            Generator.Setup(g => g.GetColumnName(ClassMap.Object, property2.Object, true)).Returns("Column2").Verifiable();
            ClassMap.SetupGet(c => c.Properties).Returns(properties).Verifiable();

            var result = Generator.Object.BuildSelectColumns(ClassMap.Object);
            Assert.Equal((string)"Column1, Column2", (string)result);
            ClassMap.Verify();
            Generator.Verify();
        }

        [Fact]
        public void DoesNotIncludeIgnoredColumns()
        {
            var property1 = new Mock<IPropertyMap>();
            property1.SetupGet(p => p.Ignored).Returns(true).Verifiable();
            var property2 = new Mock<IPropertyMap>();
            var properties = new List<IPropertyMap>
            {
                property1.Object,
                property2.Object
            };

            Generator.Setup(g => g.GetColumnName(ClassMap.Object, property2.Object, true)).Returns("Column2").Verifiable();
            ClassMap.SetupGet(c => c.Properties).Returns(properties).Verifiable();

            var result = Generator.Object.BuildSelectColumns(ClassMap.Object);
            Assert.Equal((string)"Column2", (string)result);
            ClassMap.Verify();
            Generator.Verify();
            Generator.Verify(g => g.GetColumnName(ClassMap.Object, property1.Object, true), Times.Never());
            property1.Verify();
        }
    }
}