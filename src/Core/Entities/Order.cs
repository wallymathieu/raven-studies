using System;
using System.Collections.Generic;

namespace SomeBasicRavenApp.Core.Entities
{
    public class Order
    {
        public Order()
        {
            Products = new List<string>();
        }
        public string Id { get; set; }

        public DateTime OrderDate { get; set; }

        public int Number { get; set; }

        public string CustomerId { get; set; }

        public IList<string> Products { get; set; }

        public int Version { get; set; }

    }
}
