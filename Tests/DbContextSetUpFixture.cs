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
            store = Allow32Bit(new EmbeddableDocumentStore()
            {
                Configuration = {
                    RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true,
                    RunInMemory = true,
                }
            });
            store.Initialize();
        }

        private IDocumentStore Allow32Bit(EmbeddableDocumentStore store)
        {
            store.Configuration.Storage.Voron.AllowOn32Bits = true;
            return store;
        }

        [TearDown]
        public void RunAfterAnyTests()
        {
            store.Dispose();
        }
    }
}
