using DCO = Diamond.Common.Objects;
#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class Applicant : ModelBase
    {
        public Name Name { get; set; }
        public Address Address { get; set; }

        public Applicant() { }
        internal Applicant(DCO.Policy.Applicant dApp)
        {
            if (dApp != null)
            {
                this.Name = new Name(dApp.Name, false);
                this.Address = new Address(dApp.Address);
            }
#if DEBUG
            else
            {
                Debugger.Break();
            }
#endif
        }

        public override string ToString()
        {
            return this.Name != null ? Name.ToString() : "Name is NUll";
        }
    }
}