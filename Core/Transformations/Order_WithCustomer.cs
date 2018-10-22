using SomeBasicRavenApp.Core.Entities;

namespace SomeBasicRavenApp.Core.Transformations
{
    public class Order_WithCustomer
    {
        public Customer Customer { get; internal set; }
        public Order Order { get; internal set; }
    }
}
