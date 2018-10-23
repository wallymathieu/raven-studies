using System.Linq;
using SomeBasicRavenApp.Core.Entities;
using Raven.Client.Documents.Indexes;

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
            Index(a => a.Firstname, FieldIndexing.Search);
            Index(a => a.Lastname, FieldIndexing.Search);
        }
    }
}
