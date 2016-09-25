using Raven.Client.UniqueConstraints;
using System;
using System.Collections.Generic;

namespace SomeBasicRavenApp.Core.Entities
{
    public class Order : IIdentifiableByNumber
    {
        public Order()
        {
            Products = new List<ProductId>();
        }
        public virtual string Id { get; set; }

        public virtual DateTime OrderDate { get; set; }
        [UniqueConstraint]
        public virtual int Number { get; set; }

        public virtual CustomerId CustomerId { get; set; }

        public virtual IList<ProductId> Products { get; set; }

        public virtual int Version { get; set; }

    }
}
