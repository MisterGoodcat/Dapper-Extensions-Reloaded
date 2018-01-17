using System;

namespace DapperExtensionsReloaded.Test.Data
{
    class Animal
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
    }
}