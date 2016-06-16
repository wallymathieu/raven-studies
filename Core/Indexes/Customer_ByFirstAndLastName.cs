using Raven.Client.Indexes;
using Raven.Client.Linq;
using System.Linq;
using SomeBasicRavenApp.Core.Entities;
using Raven.Abstractions.Indexing;

namespace SomeBasicRavenApp.Core.Indexes
{
    public class Customer_ByFirstAndLastName : AbstractIndexCreationTask<Customer>
    {
        public Customer_ByFirstAndLastName()
        {
            Map = customers => from customer in customers
                               select new
                               {
                                   Firstname = customer.Firstname,
                                   Lastname = customer.Lastname
                               };
            Index(a => a.Firstname, FieldIndexing.Analyzed);
            Index(a => a.Lastname, FieldIndexing.Analyzed);
        }
    }
}
