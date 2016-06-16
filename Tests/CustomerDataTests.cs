using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using Order = SomeBasicRavenApp.Core.Entities.Order;
using System.Linq;
using System;
using SomeBasicRavenApp.Core.Entities;
using Raven.Client;
using System.Collections.Generic;
using Raven.Client.UniqueConstraints;
using Raven.Client.Exceptions;

namespace SomeBasicRavenApp.Tests
{
    [TestFixture]
    public class CustomerDataTests: DbTestsBase
    {
        private IDocumentStore _store;
        private IDocumentSession _session;


        [Test]
        public void CanGetCustomerById()
        {
            var customer = _session.Load<Customer>(1);

            Assert.IsNotNull(customer);
        }

        [Test]
        public void CustomerHasOrders()
        {
            var customer = _session.Load<Customer>(1);

            Assert.True(customer.Orders.Any());
        }

        [Test]
        public void CanGetCustomerByFirstname()
        {
            var customers = _session.Query<Customer>()
                .Where(c => c.Firstname == "Steve")
                .ToList();
            Assert.AreEqual(2, customers.Count);
        }

        [Test]
        public void CanGetCustomerByEmail()
        {
            var customer = _session.LoadByUniqueConstraint<Customer>(x => x.Email,
                "peter@sylvester.com");
            Assert.AreEqual(51, customer.Id);
        }

        [Test]
        public void CanCheckIfConstraintIsValid()
        {
            var customer = new Customer {
                Id=61,
                Firstname = "Peter John",
                Lastname ="Sylvester",
                Email = "peter@sylvester.com"
            };
            var checkResult = _session.CheckForUniqueConstraints(customer);
            Assert.IsFalse(checkResult.ConstraintsAreFree());
        }

        [Test]
        public void CanInsertDuplicate()
        {
            var customer = new Customer
            {
                Id = 61,
                Firstname = "Peter John",
                Lastname = "Sylvester",
                Email = "peter@sylvester.com"
            };
            _session.Store(customer);
            WaitForIndexing(_store);

            var first = _session.Load<Customer>(51);
            Assert.IsNotNull(first,"first");
            var second= _session.Load<Customer>(61);
            Assert.IsNotNull(second, "second");
            var customerWhenLoadByConstrain = _session.LoadByUniqueConstraint<Customer>(x => x.Email,
              "peter@sylvester.com");
            Assert.AreEqual(51, customerWhenLoadByConstrain.Id);
        }


        [Test]
        public void CantInsertARecordWithSameId()
        {
            var customer = new Customer
            {
                Id = 61,
                Firstname = "Peter John",
                Lastname = "Sylvester1",
                Email = "peter1@sylvester.com"
            };
            _session.Store(customer);
            var customer_2 = new Customer
            {
                Id = 61,
                Firstname = "Peter John",
                Lastname = "Sylvester2",
                Email = "peter2@sylvester.com"
            };
            Assert.Throws<NonUniqueObjectException>(()=> _session.Store(customer_2));
            WaitForIndexing(_store);
            var load = _session.Load<Customer>(61);
            Assert.IsNotNull(load);
            Assert.AreEqual(customer.Lastname, load.Lastname);
            Assert.AreEqual(customer.Email, load.Email);
        }

        [Test]
        public void CanGetProductById()
        {
            var product = _session.Load<Product>(1);

            Assert.IsNotNull(product);
        }
        [Test]
        public void OrderContainsProduct()
        {
            Assert.True(_session.Load<Customer>(1).Orders.First().Products.Any(p => p.Id == 1));
        }

        [SetUp]
        public void Setup()
        {
            _session = _store.OpenSession();
        }


        [TearDown]
        public void TearDown()
        {
            _session.Dispose();
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _store = this.NewDocumentStore(runInMemory: true);
            _store.Initialize();
            var doc = XDocument.Load(Path.Combine("TestData", "TestData.xml"));
            var import = new XmlImport(doc, "http://tempuri.org/Database.xsd");
            using (var session = _store.OpenSession())
            {
                import.Parse<Core.IIdentifiableByNumber>(new[] { typeof(Customer), typeof(Product) },
                                (type, obj) => session.Store(obj), onIgnore: (type, property) =>
                                {
                                    Console.WriteLine("ignoring property {1} on {0}", type.Name, property.PropertyType.Name);
                                });
                var orders = new Dictionary<int, Order>();
                import.Parse<Order>(new[] { typeof(Order) },
                                (type, obj) => orders.Add(obj.Id,obj), onIgnore: (type, property) =>
                                {
                                    Console.WriteLine("ignoring property {1} on {0}", type.Name, property.PropertyType.Name);
                                });

                import.ParseConnections("OrderProduct", "Product", "Order", (productId, orderId) =>
                {
                    var product = session.Load<Product>(productId);
                    var order = orders[orderId];
                    order.Products.Add(product);
                });

                import.ParseIntProperty("Order", "Customer", (orderId, customerId) =>
                {
                    session.Load<Customer>(customerId).Orders.Add(orders[orderId]);
                });
                session.SaveChanges();
            }
            WaitForIndexing(_store);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            _store.Dispose();
        }
    }
}
