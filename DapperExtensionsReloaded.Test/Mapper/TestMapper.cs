using System;
using System.Linq.Expressions;
using DapperExtensionsReloaded.Mapper.Internal;

namespace DapperExtensionsReloaded.Test.Mapper
{
    internal class TestMapper<T> : ClassMapper<T> where T : class
    {
        public new PropertyMap Map(Expression<Func<T, object>> expression)
        {
            return base.Map(expression);
        }
    }
}