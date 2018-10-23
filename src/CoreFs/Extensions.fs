module SomeBasicRavenApp.CoreFs.Extensions
open Raven.Client.Documents
open Raven.Client.Documents.Session
open System.Linq.Expressions
open System
open System.Linq
open SomeBasicRavenApp.Core.Entities
module Number=
    let toOrderId v = sprintf "order/%d" v
    let toCustomerId v = sprintf "customer/%d" v
    let toProductId v = sprintf "product/%d" v
let private nullToOption res = if Object.ReferenceEquals (res,null) then None else Some res

type IDocumentSession with
    member session.GetOrder id=
        session.Load<Order> (Number.toOrderId id) |> nullToOption

    member session.GetProduct id=
        session.Load<Product> (Number.toProductId id) |> nullToOption

    member session.GetCustomer id=
        session.Load<Customer>(Number.toCustomerId id) |> nullToOption

    member session.GetCustomerOrders id= failwith "Not implemented"
        //Number.toOrderId id
        (*
        let order = session.Query<Order> (, fun i->i.IncludeDocuments "Products" |>ignore )
        (order, order.Products.Select(fun p-> session.Load<Product> p ).ToArray())
        session.Query<Order>()
               .Where(predicate)
               .Include("CustomerId")
               .Select(fun o-> (o, session.Load<Customer>(o.CustomerId)))
               .GroupBy(fun (o,c)->c.Id)
               .Select(fun grp-> (fst (Seq.head grp), grp.Select(snd).ToArray()))
               *)

    member session.GetOrderProducts id=
        let order = session.Load<Order> (Number.toOrderId id, fun i->i.IncludeDocuments "Products" |>ignore )
        (order, order.Products.Select(fun p-> session.Load<Product> p ).ToArray())
