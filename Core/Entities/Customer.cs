namespace SomeBasicRavenApp.Core.Entities
{
    public class Customer:IIdentifiableByNumber
    {
        public Customer()
        {
            //Orders = new List<Order>();
        }
        public virtual string Id { get; set; }

        public virtual int Number { get; set; }

        //public virtual string FirstNameAndLastName { get { return string.Concat(Firstname, Lastname); } }

        public virtual string Firstname { get; set; }

        public virtual string Lastname { get; set; }

        public virtual string Email { get; set; }

        //public virtual IList<Order> Orders { get; set; }

        public virtual int Version { get; set; }

    }
}
