using System.IO;
using System.Xml.Linq;
using Order = SomeBasicRavenApp.Core.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Queries.Facets;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions.Documents.Session;
using Raven.TestDriver;
using SomeBasicRavenApp.Core.Entities;
using SomeBasicRavenApp.Core.Extensions;
using SomeBasicRavenApp.Core.Indexes;
using SomeBasicRavenApp.Core.Transformations;
using Xunit;

namespace SomeBasicRavenApp.Tests
{
    public class CustomerDataTests :RavenTestDriver
    {
        private IDocumentStore _store;
        private IDocumentSession session;
        private Lazy<IDocumentSession> _session;

        [Fact]
        public void CanGetCustomerById()
        {
            var customer = session.Load<Customer>("customer/1");

            Assert.NotNull(customer);
        }

        [Fact]
        public void CustomerHasOrders()
        {
            var customerOrder = session.GetCustomerOrders(order => order.Number == 1)
                .First();

            Assert.True(customerOrder.Item2.Any());
        }

        [Fact]
        public void CanGetCustomerByFirstname()
        {
            var customers = session.Query<Customer>()
                .Where(c => c.Firstname == "Steve")
                .ToList();
            Assert.Equal(2, customers.Count);
        }

        [Fact]
        public void CanGetFirstNames()
        {
            var facets = session.Query<Customer, Customer_ByFirstAndLastName>()
                .AggregateBy(f=>f.ByField("Firstname").WithOptions(new FacetOptions()
                {
                    TermSortMode = FacetTermSortMode.ValueAsc,
                }))
                .Execute();

            Assert.Equal(new[] {"joe", "mike", "peter", "steve", "yuliana"},
                facets["Firstname"].Values.Select(v=>v.Range).ToArray());
        }

        [Fact]
        public void CanGetCustomerByEmail()
        {
            var customer = session.Query<Customer>().Single(x => x.Email=="peter@sylvester.com");
            Assert.Equal(51, customer.Number);
        }

        [Fact]
        public void CanSearchForCustomerByName()
        {
            var customers = session.Query<Customer, Customer_ByFirstAndLastName>()
                .Where(c => c.Firstname == "Steve")
                .ToList();
            Assert.Equal(2, customers.Count);
        }

        [Fact]
        public void CanFindAllThingsSugar()
        {
            var products = session.SearchForProducts("sugar")
                .ToList();
            Assert.Equal(4, products.Count);
        }

        [Fact]
        public void CanFindSugDrink()
        {
            var products = session.SearchForProducts("sug drink")
                .ToList();
            Assert.True(products.Count>=1);
            
            Assert.Equal("Soda", products.First().Name);
        }

        [Fact]
        public void CanSearchForSugarDrink()
        {
            var products = session.SearchForProducts("sugar drink")
                .ToList();
            Assert.True(products.Count>=1);
            Assert.Equal("Soda", products.First().Name);
        }

        [Fact]
        public void CantInsertARecordWithSameId()
        {
            var customer = new Customer
            {
                Number = 61,
                Firstname = "Peter John",
                Lastname = "Sylvester1",
                Email = "peter1@sylvester.com"
            };
            session.Store(customer);
            var customer_2 = new Customer
            {
                Id = customer.Id,
                Number = 61,
                Firstname = "Peter John",
                Lastname = "Sylvester2",
                Email = "peter2@sylvester.com"
            };
            Assert.Throws<NonUniqueObjectException>(() => session.Store(customer_2));
            WaitForIndexing(_store);
            var load = session.Load<Customer>(customer.Id);
            Assert.NotNull(load);
            Assert.Equal(customer.Lastname, load.Lastname);
            Assert.Equal(customer.Email, load.Email);
        }

        [Fact]
        public void CanGetProductById()
        {
            var product = session.Load<Product>("product/1");

            Assert.NotNull(product);
        }
        [Fact]
        public void OrderContainsProduct()
        {
            var orderProducts = session.GetOrderProducts(order => order.Number == 1);
            Assert.True(orderProducts.First().Item2.Any(p => p.Number == 1));
        }

        public CustomerDataTests()
        {
            _store = GetDocumentStore();
            _store.Initialize();
            //_store.index
            IndexCreation.CreateIndexes(typeof(Order_WithCustomer).Assembly, _store);
            var doc = XDocument.Load(Path.Combine("TestData", "TestData.xml"));
            var import = new XmlImport(doc, "http://tempuri.org/Database.xsd");
            using (var session = _store.OpenSession())
            {
                import.Parse<Customer>(new[] { typeof(Customer) },
                                (type, obj) =>
                                {
                                    obj.Id = $"customer/{obj.Number}";
                                    session.Store(obj, obj.Id);
                                }, onIgnore: (type, property) =>
                                {
                                    Console.WriteLine("ignoring property {1} on {0}", type.Name, property.PropertyType.Name);
                                });
                import.Parse<Product>(new[] { typeof(Product) },
                    (type, obj) =>
                    {
                        obj.Id = $"product/{obj.Number}";
                        session.Store(obj, obj.Id);
                    }, onIgnore: (type, property) =>
                    {
                        Console.WriteLine("ignoring property {1} on {0}", type.Name, property.PropertyType.Name);
                    });
                import.Parse<Order>(new[] { typeof(Order) },
                                (type, obj) =>
                                {
                                    obj.Id = $"order/{obj.Number}";
                                    session.Store(obj, obj.Id);
                                }, onIgnore: (type, property) =>
                                {
                                    Console.WriteLine("ignoring property {1} on {0}", type.Name, property.PropertyType.Name);
                                });
                session.SaveChanges();
                import.ParseConnections("OrderProduct", "Product", "Order", (productId, orderId) =>
                {
                    var product = session.Load<Product>($"product/{productId}");
                    var order = session.Load<Order>($"order/{orderId}");
                    
                    if (order.Products==null) order.Products=new List<string>();
                    order.Products.Add(product.Id);
                });

                import.ParseIntProperty("Order", "Customer", (orderId, customerId) =>
                {
                    var order = session.Load<Order>($"order/{orderId}");
                    var customer = session.Load<Customer>($"customer/{customerId}");
                    order.CustomerId = customer.Id;
                });
                session.SaveChanges();
            }

            WaitForIndexing(_store);
            session = _store.OpenSession();
        }
    }
}
