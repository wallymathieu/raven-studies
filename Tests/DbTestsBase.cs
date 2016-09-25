using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Database.Config;
using System.IO;
using Raven.Client.Document;
using Raven.Client.UniqueConstraints;
using Raven.Client.Embedded;
using SomeBasicRavenApp.Core;

namespace SomeBasicRavenApp.Tests
{
    public class DbTestsBase: Raven.Tests.Helpers.RavenTestBase
    {
        protected override void ModifyConfiguration(InMemoryRavenConfiguration configuration)
        {
            var plugindirectory = Directory.EnumerateDirectories(
                Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "packages")
            ).FirstOrDefault(dir => Path.GetFileName(dir).StartsWith("RavenDB.Bundles.UniqueConstraints"));
            if (plugindirectory == null) throw new NullReferenceException("plugindirectory");
            configuration.PluginsDirectory = Path.Combine(plugindirectory, "lib", "net45");
            //configuration.Storage.Voron.AllowOn32Bits = true;
            configuration.DefaultStorageTypeName = InMemoryRavenConfiguration.EsentTypeName;
            base.ModifyConfiguration(configuration);
        }
        protected override void ModifyStore(DocumentStore documentStore)
        {
            documentStore.RegisterListener(new UniqueConstraintsStoreListener());
            documentStore.Conventions.IdentityTypeConvertors.Add(new CustomerIdentityConverter());
            documentStore.Conventions.IdentityTypeConvertors.Add(new ProductIdentityConverter());
            base.ModifyStore(documentStore);
        }
        protected override void ModifyStore(EmbeddableDocumentStore documentStore)
        {
            documentStore.RegisterListener(new UniqueConstraintsStoreListener());
            documentStore.Conventions.IdentityTypeConvertors.Add(new CustomerIdentityConverter());
            documentStore.Conventions.IdentityTypeConvertors.Add(new ProductIdentityConverter());
            base.ModifyStore(documentStore);
        }
    }
}
