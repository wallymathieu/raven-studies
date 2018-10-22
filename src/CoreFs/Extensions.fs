module SomeBasicRavenApp.CoreFs.Extensions
open Raven.Client.Documents
open Raven.Client.Documents.Session
open System.Linq.Expressions
open System
open System.Linq
open SomeBasicRavenApp.Core.Entities
type IDocumentSession with
    member session.GetCustomerOrders( predicate:Expression<Func<Order, bool>>)=
        session.Query<Order>()
               .Where(predicate)
               .Include("CustomerId")
               .Select(fun o-> (o, session.Load<Customer>(o.CustomerId)))
               .GroupBy(fun (o,c)->c.Id)
               .Select(fun grp-> (fst (Seq.head grp), grp.Select(snd).ToArray()))

    member session.GetOrderProducts( predicate:Expression<Func<Order, bool>>)=
        session.Query<Order>()
               .Where(predicate)
               .Include("Products")
               .Select(fun o-> (o, o.Products.Select(fun p-> session.Load<Product> p ).ToArray() ))
