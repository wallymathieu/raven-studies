module Tests

open System
open Xunit
open FSharp.Data
open Raven.TestDriver
open SomeBasicRavenApp.CoreFs.Extensions
open SomeBasicRavenApp.Core.Entities

module TestData=
    open System.IO
    open System.Collections.Generic
    open Raven.Client.Documents

    type TestData = XmlProvider<"../Tests/TestData/TestData.xml", Global=false>

    let fillDb (sessionFactory:IDocumentStore)=
        use session = sessionFactory.OpenSession()

        let toCustomer (o : TestData.Customer) :Customer=
            {Id=Number.toCustomerId o.Number;Version=o.Version;Firstname=o.Firstname;Lastname=o.Lastname}

        let toOrder (o : TestData.Order) :Order=
            {Id=Number.toOrderId o.Number;Version=o.Version;CustomerId=Number.toCustomerId o.Customer; OrderDate=o.OrderDate.DateTime; Products=List<_>()}

        let toProduct (o : TestData.Product) :Product=
            {Id=Number.toProductId o.Number;Version=o.Version;Name=o.Name;Cost=float o.Cost; Products=List<_>()}

        let toOrderProduct(o : TestData.OrderProduct)=
            let order=session.Load<Order>(Number.toOrderId o.Order)
            (order, Number.toProductId o.Product)

        use f = File.Open("TestData/TestData.xml", FileMode.Open, FileAccess.Read, FileShare.Read)
        let db = TestData.Load(f)

        for customer in db.Customers |> Array.map toCustomer do
            session.Store (customer, customer.Id)

        for order in db.Orders |> Array.map toOrder do
            session.Store (order, order.Id)
        for product in db.Products |> Array.map toProduct do
            session.Store (product, product.Id)
        for (order,product) in db.OrderProducts |> Array.map toOrderProduct do
            order.Products.Add product
        session.SaveChanges()

type CustomerDataTests()=
    inherit RavenTestDriver()

    let mutable sessionFactory =null
    let mutable session=null
    do
        sessionFactory <- base.GetDocumentStore()
        TestData.fillDb sessionFactory
        session <-sessionFactory.OpenSession()

    [<Fact>]
    member this.CanGetCustomerById()=
        Assert.True(session.GetCustomer 1 |> Option.isSome)

    [<Fact>]
    member this.CanGetProductById()=
        Assert.True(session.GetProduct 1 |> Option.isSome)

    [<Fact>]
    member this.AProductThatDoesNotExist()=
        Assert.True(session.GetProduct 1000 |> Option.isNone)

    [<Fact>]
    member this.OrderContainsProduct()=
        let (order,products) = session.GetOrderProducts 1
        let productId = Number.toProductId 1
        Assert.True(order.Products |> Seq.tryFind( (=) productId) |> Option.isSome)
        Assert.True(products |> Seq.tryFind( Product.id >> (=) (productId)) |> Option.isSome)


