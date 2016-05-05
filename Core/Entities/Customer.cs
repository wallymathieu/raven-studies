﻿using System.Collections.Generic;

namespace SomeBasicRavenApp.Core.Entities
{
    public class Customer : IIdentifiableByNumber
    {
        public Customer()
        {
            Orders = new List<Order>();
        }
        public virtual int Id { get; set; }

        public virtual string Firstname { get; set; }

        public virtual string Lastname { get; set; }

        public virtual IList<Order> Orders { get; set; }

        public virtual int Version { get; set; }

    }
}
