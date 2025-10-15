using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class Policyholder : ModelBase
    {
        public Name Name { get; set; }

        public Address Address { get; set; }

        public Policyholder() { }

        internal Policyholder(DCO.Name _name, DCO.Address _address)
        {
            this.Name = new Name(_name, false);
            this.Address = new Address(_address);
        }

        public override string ToString()
        {
            return this.Name != null ? Name.ToString() : "Name is NUll";
        }
    }
}