using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Raven.Client.Indexes;
using SomeBasicRavenApp.Core.Entities;
using System.Linq;

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

            Analyzers.Add(x => x.Tags, typeof(SimpleAnalyzer).FullName);
            Analyzers.Add(x => x.Name, typeof(SimpleAnalyzer).FullName);
            Analyzers.Add(x => x.Description, typeof(StandardAnalyzer).FullName);
        }
    }
}
