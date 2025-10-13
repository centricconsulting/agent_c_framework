using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class BillingAddress : ModelBase
    {
        public Name Name { get; set; }
        public Address Address { get; set; }

        public BillingAddress() { }
        public BillingAddress(DCO.Policy.BillingAddressee dBillingAddress)
        {
            this.Name = new Name(dBillingAddress?.Name, false);
            this.Address = new Address(dBillingAddress?.Address);
        }

        public override string ToString()
        {
            return $"{Address} {Name}";
        }

    }
}