using System.Collections.Generic;

namespace DapperExtensionsReloaded.Test.Mapper
{
    public class Foo
    {
        public int FooId { get; set; }
        public IEnumerable<string> List { get; set; }
    }
}