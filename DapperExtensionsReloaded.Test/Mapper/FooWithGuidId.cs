using System;

namespace DapperExtensionsReloaded.Test.Mapper
{
    public class FooWithGuidId
    {
        public Guid FooId { get; set; }
        public string Value { get; set; }
        public Guid BarId { get; set; }
    }
}