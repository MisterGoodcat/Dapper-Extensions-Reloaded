using System;
using System.Collections.Generic;
using DapperExtensionsReloaded.Mapper;

namespace DapperExtensionsReloaded.Test.Data
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Active { get; set; }

        [DatabaseColumn(IsIgnored = true)]
        public IEnumerable<Phone> Phones { get; private set; }
    }
}