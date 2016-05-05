using System;
using System.Collections.Generic;

namespace SomeBasicRavenApp.Core.Entities
{
    public class Order : IIdentifiableByNumber
    {
        public Order()
        {
            Products = new List<Product>();
        }

        public virtual DateTime OrderDate { get; set; }

        public virtual int Id { get; set; }

        public virtual IList<Product> Products { get; set; }

        public virtual int Version { get; set; }

    }
}
