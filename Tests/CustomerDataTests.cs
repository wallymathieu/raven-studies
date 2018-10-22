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
using SomeBasicRavenApp.Core.Entities;
using SomeBasicRavenApp.Core.Extensions;
using SomeBasicRavenApp.Core.Indexes;
using SomeBasicRavenApp.Core.Transformations;
using Xunit;

namespace SomeBasicRavenApp.Tests
{
    public class CustomerDataTests : IDisposable
    {
        private IDocumentStore _store;
        private IDocumentSession _session;


        [Fact]
        public void CanGetCustomerById()
        {
            var customer = _session.Load<Customer>(1);

            Assert.NotNull(customer);
        }

        [Fact]
        public void CustomerHasOrders()
        {
            var customerOrder = _session.GetCustomerOrders(order => order.Number == 1)
                .First();

            Assert.True(customerOrder.Item2.Any());
        }

        [Fact]
        public void CanGetCustomerByFirstname()
        {
            var customers = _session.Query<Customer>()
                .Where(c => c.Firstname == "Steve")
                .ToList();
            Assert.Equal(2, customers.Count);
        }

        [Fact]
        public void CanGetFirstNames()
        {
            var facets = _session.Query<Customer, Customer_ByFirstAndLastName>()
                .ToFacets(new List<Facet>() {
                    new Facet() {
                        Name="Firstname",
                        AggregationField="Firstname",
                        TermSortMode=FacetTermSortMode.ValueDesc,
                        Mode=FacetMode.Default
                    }
                })
                ;
            Assert.That(facets.Results["Firstname"].Values.Select(v=>v.Range).ToArray(),
                Is.EquivalentTo(new[] { "steve", "joe", "mike", "peter", "yuliana" }));
        }

        [Fact]
        public void CanGetCustomerByEmail()
        {
            var customer = _session.Query<Customer>().Single(x => x.Email=="peter@sylvester.com");
            Assert.Equal(51, customer.Number);
        }

        [Fact]
        public void CanSearchForCustomerByName()
        {
            var customers = _session.Query<Customer, Customer_ByFirstAndLastName>()
                .Where(c => c.Firstname == "Steve")
                .ToList();
            Assert.Equal(2, customers.Count);
        }

        [Fact]
        public void CanFindAllThingsSugar()
        {
            var products = _session.SearchForProducts("sugar")
                .ToList();
            Assert.Equal(2,products.Count);
        }

        [Fact]
        public void CanFindAllThingsSugarWhenFuzzy()
        {
            var products = _session.FuzzySearchForProducts("sugar")
                .ToList();
            Assert.Equal(2,products.Count);
        }

        [Fact]
        public void CanFindSugDrink()
        {
            var products = _session.FuzzySearchForProducts("sug drink")
                .ToList();
            Assert.True(products.Count>=1);
            
            Assert.Equal("Soda", products.First().Name);
        }

        [Fact]
        public void CanSearchForSugarDrinkWhenFuzzy()
        {
            var products = _session.FuzzySearchForProducts("sugar drink")
                .ToList();
            Assert.True(products.Count>=1);
            Assert.Equal("Soda", products.First().Name);
        }

        [Fact]
        public void CanSearchForSugarDrink()
        {
            var products = _session.SearchForProducts("sugar drink")
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
            _session.Store(customer);
            var customer_2 = new Customer
            {
                Id = customer.Id,
                Number = 61,
                Firstname = "Peter John",
                Lastname = "Sylvester2",
                Email = "peter2@sylvester.com"
            };
            Assert.Throws<NonUniqueObjectException>(() => _session.Store(customer_2));
            WaitForIndexing(_store);
            var load = _session.Load<Customer>(customer.Id);
            Assert.IsNotNull(load);
            Assert.AreEqual(customer.Lastname, load.Lastname);
            Assert.AreEqual(customer.Email, load.Email);
        }

        [Fact]
        public void CanGetProductById()
        {
            var product = _session.Load<Product>(1);

            Assert.NotNull(product);
        }
        [Fact]
        public void OrderContainsProduct()
        {
            var orderProducts = _session.GetOrderProducts(order => order.Number == 1);
            Assert.True(orderProducts.First().Item2.Any(p => p.Number == 1));
        }

        public CustomerDataTests()
        {
            _session = _store.OpenSession();
        }


        public void Dispose()
        {
            _session.Dispose();
        }

        static CustomerDataTests()
        {
            _store = this.NewDocumentStore(runInMemory: true);
            _store.Initialize();
            //_store.index
            IndexCreation.CreateIndexes(typeof(Order_WithCustomer).Assembly, _store);
            var doc = XDocument.Load(Path.Combine("TestData", "TestData.xml"));
            var import = new XmlImport(doc, "http://tempuri.org/Database.xsd");
            using (var session = _store.OpenSession())
            {
                import.Parse<Core.IIdentifiableByNumber>(new[] { typeof(Customer), typeof(Product) },
                                (type, obj) => session.Store(obj), onIgnore: (type, property) =>
                                {
                                    Console.WriteLine("ignoring property {1} on {0}", type.Name, property.PropertyType.Name);
                                });
                import.Parse<Order>(new[] { typeof(Order) },
                                (type, obj) => session.Store(obj), onIgnore: (type, property) =>
                                {
                                    Console.WriteLine("ignoring property {1} on {0}", type.Name, property.PropertyType.Name);
                                });
                session.SaveChanges();
            }
            WaitForIndexing(_store);
            using (var session = _store.OpenSession())
            {
                import.ParseConnections("OrderProduct", "Product", "Order", (productId, orderId) =>
                {
                    var product = session.LoadByUniqueConstraint<Product>(p => p.Number, productId);
                    var order = session.LoadByUniqueConstraint<Order>(p => p.Number, orderId);
                    order.Products.Add(product.Id);
                });

                import.ParseIntProperty("Order", "Customer", (orderId, customerId) =>
                {
                    var order = session.LoadByUniqueConstraint<Order>(p => p.Number, orderId);
                    var customer = session.LoadByUniqueConstraint<Customer>(p => p.Number, orderId);
                    order.CustomerId = customer.Id;
                    //session.Load<Customer>(customerId).Orders.Add(orders[orderId]);
                });
                session.SaveChanges();
            }

            _store.ExecuteTransformer(new Order_WithCustomer());

            WaitForIndexing(_store);
        }

    }
}
