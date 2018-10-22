using Raven.Client;
using SomeBasicRavenApp.Core.Entities;
using SomeBasicRavenApp.Core.Indexes;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace SomeBasicRavenApp.Core.Extensions
{
    public static class ProductExtensions
    {
        public static IEnumerable<Product> SearchForProducts(this IDocumentSession session, string search)
        {
            return session.Query<Product, Product_ByNameAndTagAndDescription>()
                .Search(searchTerms: search, fieldSelector: p => p.Name, options: SearchOptions.Guess, boost: 10)
                .Search(searchTerms: search, fieldSelector: p => p.Tags, options: SearchOptions.Guess, boost: 5)
                .Search(searchTerms: search, fieldSelector: p => p.Description, options: SearchOptions.Guess, boost: 1)
                ;
        }
    }
}
