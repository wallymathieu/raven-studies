using Raven.Client.UniqueConstraints;

namespace SomeBasicRavenApp.Core.Entities
{
    public class Product : IIdentifiableByNumber
    {
        public virtual ProductId Id { get; set; }

        public virtual float Cost { get; set; }

        public virtual string Name { get; set; }

        public virtual string Tags { get; set; }
        public virtual string Description { get; set; }
        [UniqueConstraint]
        public virtual int Number { get; set; }

        public virtual int Version { get; set; }
    }
}
