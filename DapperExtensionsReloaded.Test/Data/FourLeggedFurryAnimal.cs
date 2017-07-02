using System;
using DapperExtensionsReloaded.Mapper;

namespace DapperExtensionsReloaded.Test.Data
{
    [DatabaseEntity("Cat")]
    public class FourLeggedFurryAnimal
    {
        [DatabaseColumn(IsIdentity = true)]
        public int Id { get; set; }

        [DatabaseColumn(ColumnName = "Name")]
        public string HowItsCalled { get; set; }
        
        public DateTime DateCreated { get; set; }
        public bool Active { get; set; }
    }
}
