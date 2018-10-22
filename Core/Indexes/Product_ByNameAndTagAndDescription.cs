using SomeBasicRavenApp.Core.Entities;
using System.Linq;
using Raven.Client.Documents.Indexes;

namespace SomeBasicRavenApp.Core.Indexes
{
    public class Product_ByNameAndTagAndDescription: AbstractIndexCreationTask<Product>
    {
        public Product_ByNameAndTagAndDescription()
        {
            Map = products => from product in products
                           select new
                           {
                               product.Name,
                               product.Tags,
                               product.Description
                           };

            Analyzers.Add(x => x.Tags, "SimpleAnalyzer");
            Analyzers.Add(x => x.Name, "SimpleAnalyzer");
            Analyzers.Add(x => x.Description, "StandardAnalyzer");
        }
    }
}
