using Raven.Client.Indexes;
using SomeBasicRavenApp.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SomeBasicRavenApp.Core.Transformations
{
    public class Order_WithProducts : AbstractTransformerCreationTask<Order>
    {
        public class Result
        {
            public IEnumerable<Product> Products { get; internal set; }
            public Order Order { get; internal set; }
        }

        public Order_WithProducts()
        {
            TransformResults =
                orders =>
                from order in orders
                let products = this.Recurse<Order,Product>(order,
                                    o=>o.Products.Select(p=>LoadDocument<Product>(p)))
                select new Result
                {
                    Products = products,
                    Order = order
                };
        }
    }
}
