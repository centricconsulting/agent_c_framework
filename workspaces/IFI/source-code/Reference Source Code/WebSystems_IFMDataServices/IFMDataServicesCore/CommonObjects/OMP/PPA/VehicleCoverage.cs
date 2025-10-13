using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.CommonObjects.OMP.PPA
{
    [System.Serializable]
    public class VehicleCoverage : CoverageBase
    {
        public VehicleCoverage() { }
        internal VehicleCoverage(DCO.Coverage dCov) : base(dCov)
        {
        }

        internal VehicleCoverage(QuickQuote.CommonObjects.QuickQuoteCoverage dCov) : base(dCov)
        {
        }

    }
}
