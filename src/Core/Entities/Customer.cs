namespace SomeBasicRavenApp.Core.Entities
{
    public class Customer
    {
        public Customer()
        {
            //Orders = new List<Order>();
        }
        public string Id { get; set; }

        public int Number { get; set; }

        //public virtual string FirstNameAndLastName { get { return string.Concat(Firstname, Lastname); } }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        //public virtual IList<Order> Orders { get; set; }

        public int Version { get; set; }

    }
}
