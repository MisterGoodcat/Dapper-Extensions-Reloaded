using DapperExtensionsReloaded.Mapper.Internal;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests
{
    public class NonCrudGetMapMethodTests
    {
        [Fact]
        public void NoMappingClass_ReturnsDefaultMapper()
        {
            var mapper = DapperExtensions.GetMap<EntityWithoutMapper>();
            Assert.Equal(typeof(ClassMapper<EntityWithoutMapper>), mapper.GetType());
        }

        private class EntityWithoutMapper
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}