namespace SomeBasicRavenApp.Core.Entities
{
    public class Product
    {
        public string Id { get; set; }

        public float Cost { get; set; }

        public string Name { get; set; }

        public string Tags { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }

        public int Version { get; set; }
    }
}
