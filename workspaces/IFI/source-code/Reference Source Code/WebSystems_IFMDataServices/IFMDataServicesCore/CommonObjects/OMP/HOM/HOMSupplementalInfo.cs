using System.Collections.Generic;
using DCO = Diamond.Common.Objects;

#if DEBUG

using System.Diagnostics;

#endif

using System.Linq;

namespace IFM.DataServicesCore.CommonObjects.OMP.HOM
{
    [System.Serializable]
    public class HOMSupplementalInfo : ModelBase
    {
        public List<HOM.HomLocation> Locations { get; set; }

        public HOMSupplementalInfo(){ }
        internal HOMSupplementalInfo(DCO.Policy.Image image)
        {
            if (image?.LOB?.RiskLevel?.Locations != null && image.LOB.RiskLevel.Locations.Any())
            {
                this.Locations = new List<HomLocation>();
                foreach (var l in image.LOB.RiskLevel.Locations)
                {
                    this.Locations.Add(new HomLocation(l));
                }
            }
            else
            {
#if !DEBUG
                global::IFM.IFMErrorLogging.LogIssue($"IFMDATASERVICES -> HOMSupplementalInfo -> HOMSupplementalInfo - Locations was null. Policy #{((image != null) ? image.PolicyNumber : string.Empty)}");
#else
                Debugger.Break();
#endif
            }
        }

        internal HOMSupplementalInfo(DCO.Policy.Image image, DCO.Policy.UnderlyingPolicy uPolicy)
        {
            if(uPolicy?.PolicyInfos?[0] != null && uPolicy.PolicyInfos[0].Locations.Any())
            {
                this.Locations = new List<HomLocation>();
                foreach (var l in uPolicy.PolicyInfos[0].Locations)
                {
                    this.Locations.Add(new HomLocation(l));
                }
            }
            else
            {
#if !DEBUG
                global::IFM.IFMErrorLogging.LogIssue($"Locations was null. Policy #{((image != null) ? image.PolicyNumber : string.Empty)}");
#else
                Debugger.Break();
#endif
            }
        }
    }
}