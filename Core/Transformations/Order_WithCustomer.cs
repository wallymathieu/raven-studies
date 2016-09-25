using Raven.Client.Indexes;
using SomeBasicRavenApp.Core.Entities;
using System.Linq;

namespace SomeBasicRavenApp.Core.Transformations
{
    public class Order_WithCustomer: AbstractTransformerCreationTask<Order>
    {
        public class Result
        {
            public Customer Customer { get; internal set; }
            public Order Order { get; internal set; }
        }

        public Order_WithCustomer()
        {
            TransformResults =
                orders =>
                from order in orders
                let customer = LoadDocument<Customer>((string)order.CustomerId)
                select new Result
                {
                    Customer = customer,
                    Order = order
                };
        }
    }
}
