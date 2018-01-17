using System;
using System.Collections.Generic;
using Xunit;

namespace DapperExtensionsReloaded.Test.IntegrationTests
{
    public class NonCrudGetNextGuidMethodTests
    {
        [Fact]
        public void GetMultiple_DoesNotDuplicate()
        {
            var list = new List<Guid>();
            for (var i = 0; i < 1000; i++)
            {
                var id = DapperExtensions.GetNextGuid();
                Assert.DoesNotContain(id, list);
                list.Add(id);
            }
        }
    }
}