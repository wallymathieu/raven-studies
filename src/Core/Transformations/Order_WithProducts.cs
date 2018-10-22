using SomeBasicRavenApp.Core.Entities;
using System.Collections.Generic;

namespace SomeBasicRavenApp.Core.Transformations
{
    public class Order_WithProducts
    {
        public IEnumerable<Product> Products { get; internal set; }
        public Order Order { get; internal set; }
    }
}