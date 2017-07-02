using System;
using System.Collections.Generic;

namespace DapperExtensionsReloaded.Test.Data
{
    public class Foo
    {
        public int Id { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        public List<Bar> BarList { get; set; }
    }
}