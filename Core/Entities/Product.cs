using Raven.Client.UniqueConstraints;
using System.Collections.Generic;

namespace SomeBasicRavenApp.Core.Entities
{
    public class Product : IIdentifiableByNumber
    {
        public virtual string Id { get; set; }

        public virtual float Cost { get; set; }

        public virtual string Name { get; set; }
        [UniqueConstraint]
        public virtual int Number { get; set; }

        public virtual int Version { get; set; }
    }
}
