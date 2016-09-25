using Raven.Client;
using Raven.Client.Linq;
using SomeBasicRavenApp.Core.Entities;
using SomeBasicRavenApp.Core.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SomeBasicRavenApp.Core.Extensions
{
    public static class OrderExtensions
    {
        public static IEnumerable<Tuple<Customer, Order[]>> GetCustomerOrders(this IDocumentSession session, Expression<Func<Order, bool>> predicate)
        {
            return session.Query<Order>()
                .Where(predicate)
                .TransformWith<Order_WithCustomer, Order_WithCustomer.Result>()
                .ToList()
                .GroupBy(c => c.Customer != null ? c.Customer.Number : 0)
                .Select(c => Tuple.Create(
                    c.First().Customer,
                    c.Select(o => o.Order).ToArray()));
        }

        public static IEnumerable<Tuple<Order, Product[]>> GetOrderProducts(this IDocumentSession session, Expression<Func<Order, bool>> predicate)
        {
            return session.Query<Order>()
                .Where(predicate)
                .TransformWith<Order_WithProducts, Order_WithProducts.Result>()
                .ToList()
                .Select(c => Tuple.Create(
                    c.Order,
                    c.Products.ToArray()));
        }
    }
}
