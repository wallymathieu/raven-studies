using NUnit.Framework;
using Raven.Client;
using Raven.Client.Embedded;

namespace SomeBasicRavenApp.Tests
{
    [SetUpFixture]
    class DbContextSetUpFixture
    {
        public static IDocumentStore store;
        [SetUp]
        public void RunBeforeAnyTests()
        {
            store = new EmbeddableDocumentStore()
            {
                Configuration = {
                    RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true,
                    RunInMemory = true,
                    Storage = {
                        Voron= {
                            AllowOn32Bits = true
                        }
                    }
                }
            };
            store.Initialize();
        }

        [TearDown]
        public void RunAfterAnyTests()
        {
            store.Dispose();
        }
    }
}
