using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCO = Diamond.Common.Objects;

#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class AgencyProducer : ModelBase
    {
        public string DisplayName { get; set; }
        public Name Name { get; set; }
        public List<string> Emails { get; set; }
        public List<Phone> Phones { get; set; }
        public string ProducerCode { get; set; }
        public AgencyProducer() { }
        internal AgencyProducer(DCO.Policy.Agency.AgencyProducer DiamondProducer)
        {
            this.Name = new Name(DiamondProducer.Name, true);// MUST NEVER SEND TAX INFO
            this.DisplayName = DiamondProducer.DisplayName;
            this.Phones = new List<Phone>();
            this.ProducerCode = DiamondProducer.ProducerCode;
            if (DiamondProducer.Emails != null && DiamondProducer.Emails.Any())
                this.Emails = (from e in DiamondProducer.Emails select e.Address).ToList();
        }
    }
}
