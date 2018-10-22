using Raven.Client.Documents;

namespace SomeBasicRavenApp.Core
{
    public class Store
    {
        public static IDocumentStore CreateStore()
        {
            var store = new DocumentStore
            {
                
                Urls =new[]{"http://localhost:8080/"}, // server URL
                Database = "RavenStudies"   // default database
            };
            store.Initialize(); // initializes document store, by connecting to server and downloading various configurations

            return store;
        }
    }

    public interface IIdentifiableByNumber
    {
        string Id { get; }
    }
}
