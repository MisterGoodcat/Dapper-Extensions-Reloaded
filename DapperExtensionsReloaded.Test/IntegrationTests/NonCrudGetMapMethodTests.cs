using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
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
            Assert.Equal(typeof(AttributeClassMapper<EntityWithoutMapper>), mapper.GetType());
        }

        [Fact]
        public void ClassMapperDescendant_Returns_DefinedClass()
        {
            var mapper = DapperExtensions.GetMap<EntityWithMapper>();
            Assert.Equal(typeof(EntityWithMapperMapper), mapper.GetType());
        }

        [Fact]
        public void ClassMapperInterface_Returns_DefinedMapper()
        {
            var mapper = DapperExtensions.GetMap<EntityWithInterfaceMapper>();
            Assert.Equal(typeof(EntityWithInterfaceMapperMapper), mapper.GetType());
        }

        private class EntityWithoutMapper
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class EntityWithMapper
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        private class EntityWithMapperMapper : ClassMapper<EntityWithMapper>
        {
            public EntityWithMapperMapper()
            {
                Map(p => p.Key).Column("EntityKey").Key(KeyType.Assigned);
                AutoMap();
            }
        }

        private class EntityWithInterfaceMapper
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        private class EntityWithInterfaceMapperMapper : IClassMapper<EntityWithInterfaceMapper>
        {
            public string SchemaName { get; private set; }
            public string TableName { get; private set; }
            public IList<IPropertyMap> Properties { get; private set; }
            public Type EntityType { get; private set; }

            public PropertyMap Map(Expression<Func<EntityWithInterfaceMapper, object>> expression)
            {
                throw new NotImplementedException();
            }

            public PropertyMap Map(PropertyInfo propertyInfo)
            {
                throw new NotImplementedException();
            }
        }
    }
}