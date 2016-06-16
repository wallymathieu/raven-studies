using Raven.Client.UniqueConstraints;
using System;
using System.Collections.Generic;

namespace SomeBasicRavenApp.Core.Entities
{
    public class Order : IIdentifiableByNumber
    {
        public Order()
        {
            Products = new List<string>();
        }
        public virtual string Id { get; set; }

        public virtual DateTime OrderDate { get; set; }
        [UniqueConstraint]
        public virtual int Number { get; set; }

        public virtual string CustomerId { get; set; }

        public virtual IList<string> Products { get; set; }

        public virtual int Version { get; set; }

    }
}
