using Raven.Client.UniqueConstraints;
using System.Collections.Generic;

namespace SomeBasicRavenApp.Core.Entities
{
    public class Customer : IIdentifiableByNumber
    {
        public Customer()
        {
            //Orders = new List<Order>();
        }
        public virtual CustomerId Id { get; set; }

        [UniqueConstraint]
        public virtual int Number { get; set; }

        public virtual string Firstname { get; set; }

        public virtual string Lastname { get; set; }

        [UniqueConstraint]
        public virtual string Email { get; set; }

        //public virtual IList<Order> Orders { get; set; }

        public virtual int Version { get; set; }

    }
}
