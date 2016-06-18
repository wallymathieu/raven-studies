using Raven.Client;
using SomeBasicRavenApp.Core.Entities;
using SomeBasicRavenApp.Core.Indexes;
using System.Collections.Generic;
using System.Linq;

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

        public static IEnumerable<Product> FuzzySearchForProducts(this IDocumentSession session, string search)
        {
            var searchQuery = string.Join(" ", search.Replace("*", "").Split(' ').Select(t => t + "*"));
            return session.Query<Product, Product_ByNameAndTagAndDescription>()
                .Search(searchTerms: searchQuery, fieldSelector: p => p.Name, options: SearchOptions.Guess, boost: 10,
                    escapeQueryOptions:EscapeQueryOptions.AllowPostfixWildcard)
                .Search(searchTerms: searchQuery, fieldSelector: p => p.Tags, options: SearchOptions.Guess, boost: 5,
                    escapeQueryOptions: EscapeQueryOptions.AllowPostfixWildcard)
                .Search(searchTerms: searchQuery, fieldSelector: p => p.Description, options: SearchOptions.Guess, boost: 1,
                    escapeQueryOptions: EscapeQueryOptions.AllowPostfixWildcard)
                ;
        }
    }
}
