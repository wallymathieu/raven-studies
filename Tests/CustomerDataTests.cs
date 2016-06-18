using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using Order = SomeBasicRavenApp.Core.Entities.Order;
using System.Linq;
using System;
using SomeBasicRavenApp.Core.Entities;
using Raven.Client;
using Raven.Client.Linq;
using Raven.Client.UniqueConstraints;
using Raven.Client.Exceptions;
using SomeBasicRavenApp.Core.Transformations;
using Raven.Client.Indexes;
using SomeBasicRavenApp.Core.Extensions;
using SomeBasicRavenApp.Core.Indexes;

namespace SomeBasicRavenApp.Tests
{
    [TestFixture]
    public class CustomerDataTests : DbTestsBase
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
            var customerOrder = _session.GetCustomerOrders(order => order.Number == 1)
                .First();

            Assert.True(customerOrder.Item2.Any());
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
            Assert.AreEqual(51, customer.Number);
        }

        [Test]
        public void CanSearchForCustomerByName()
        {
            var customers = _session.Query<Customer, Customer_ByFirstAndLastName>()
                .Where(c => c.Firstname == "Steve")
                .ToList();
            Assert.AreEqual(2, customers.Count);
        }

        [Test]
        public void CanFindAllThingsSugar()
        {
            var products = _session.SearchForProducts("sugar")
                .ToList();
            Assert.That(products.Count, Is.AtLeast(2));
        }

        [Test]
        public void CanFindAllThingsSugarWhenFuzzy()
        {
            var products = _session.FuzzySearchForProducts("sugar")
                .ToList();
            Assert.That(products.Count, Is.AtLeast(2));
        }

        [Test]
        public void CanFindSugDrink()
        {
            var products = _session.FuzzySearchForProducts("sug drink")
                .ToList();
            Assert.That(products.Count, Is.AtLeast(1));
            Assert.AreEqual("Soda", products.First().Name);
        }

        [Test]
        public void CanSearchForSugarDrinkWhenFuzzy()
        {
            var products = _session.FuzzySearchForProducts("sugar drink")
                .ToList();
            Assert.That(products.Count, Is.AtLeast(1));
            Assert.AreEqual("Soda", products.First().Name);
        }

        [Test]
        public void CanSearchForSugarDrink()
        {
            var products = _session.SearchForProducts("sugar drink")
                .ToList();
            Assert.That(products.Count, Is.AtLeast(1));
            Assert.AreEqual("Soda", products.First().Name);
        }

        [Test]
        public void CanCheckIfConstraintIsValid()
        {
            var customer = new Customer
            {
                Number = 61,
                Firstname = "Peter John",
                Lastname = "Sylvester",
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
                Number = 61,
                Firstname = "Peter John",
                Lastname = "Sylvester",
                Email = "peter@sylvester.com"
            };
            _session.Store(customer);
            WaitForIndexing(_store);

            var first = _session.LoadByUniqueConstraint<Customer>(c => c.Number, 51);
            Assert.IsNotNull(first, "first");
            var second = _session.Load<Customer>(customer.Id);
            Assert.IsNotNull(second, "second");
            var customerWhenLoadByConstrain = _session.LoadByUniqueConstraint<Customer>(x => x.Email,
              "peter@sylvester.com");
            Assert.AreEqual(51, customerWhenLoadByConstrain.Number);
        }


        [Test]
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

        [Test]
        public void CanGetProductById()
        {
            var product = _session.Load<Product>(1);

            Assert.IsNotNull(product);
        }
        [Test]
        public void OrderContainsProduct()
        {
            var orderProducts = _session.GetOrderProducts(order => order.Number == 1);
            Assert.True(orderProducts.First().Item2.Any(p => p.Number == 1));
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

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            _store.Dispose();
        }
    }
}
