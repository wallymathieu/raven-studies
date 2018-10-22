using SomeBasicRavenApp.Core.Entities;
using SomeBasicRavenApp.Core.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace SomeBasicRavenApp.Core.Extensions
{
    public static class OrderExtensions
    {
        public static IEnumerable<(Customer, Order[])> GetCustomerOrders(this IDocumentSession session, Expression<Func<Order, bool>> predicate)
        {
            return session.Query<Order>()
                .Where(predicate)
                .Include(o=>o.CustomerId)
                .Select(o=>new Order_WithCustomer
                {
                    Order = o,
                    Customer = session.Load<Customer>(o.CustomerId) 
                })
                .ToList()
                .GroupBy(c => c.Customer?.Number ?? 0)
                .Select(c => (c.First().Customer, c.Select(o => o.Order).ToArray()));
        }

        public static IEnumerable<(Order, Product[])> GetOrderProducts(this IDocumentSession session, Expression<Func<Order, bool>> predicate)
        {
            /*
let products = this.Recurse<Order,Product>(order,
    o=>o.Products.Select(p=>LoadDocument<Product>((string)p))) */

            return session.Query<Order>()
                .Where(predicate)
                .Include(o=>o.Products)
                .Select(o=>new Order_WithProducts
                {
                    Order = o,
                    Products = o.Products.Select(session.Load<Product>)
                })
                .ToList()
                .Select(c => (c.Order, c.Products.ToArray()));
        }
    }
}
