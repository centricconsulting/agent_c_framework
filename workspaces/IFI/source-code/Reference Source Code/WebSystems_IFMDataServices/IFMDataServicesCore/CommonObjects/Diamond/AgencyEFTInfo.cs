using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFM.DataServicesCore.CommonObjects.Diamond
{
    public class AgencyEFTInfo
    {
        public int AgencyId { get; set; }
        public int EFTAccountId { get; set; }
        public int LegacyAgencyId { get; set; }
    }
}
