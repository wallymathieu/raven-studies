using Raven.Client;
using Raven.Client.Document;
using Raven.Client.UniqueConstraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeBasicRavenApp.Core
{
    public class Store
    {
        public static IDocumentStore CreateStore()
        {
            var store = new DocumentStore
            {
                Url = "http://localhost:8080/", // server URL
                DefaultDatabase = "RavenStudies"   // default database
            };
            store.RegisterListener(new UniqueConstraintsStoreListener());
            store.Initialize(); // initializes document store, by connecting to server and downloading various configurations

            return store;
        }
    }
}
