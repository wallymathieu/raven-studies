﻿using System.Collections.Generic;

namespace SomeBasicRavenApp.Core.Entities
{
    public class Product : IIdentifiableByNumber
    {
        public virtual float Cost { get; set; }

        public virtual string Name { get; set; }

        public virtual int Id { get; set; }

        public virtual int Version { get; set; }
    }
}
