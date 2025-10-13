using System.Collections.Generic;
using DCO = Diamond.Common.Objects;

#if DEBUG

using System.Diagnostics;

#endif

using System.Linq;

namespace IFM.DataServicesCore.CommonObjects.OMP.FAR
{
    [System.Serializable]
    public class FARSupplementalInfo : ModelBase
    {
        public List<FAR.FarLocation> Locations { get; set; }

        public FARSupplementalInfo() { }
        internal FARSupplementalInfo(DCO.Policy.Image image, bool Ismultistate)
        {
            if (!Ismultistate)
            {
                if (image?.LOB?.RiskLevel?.Locations != null && image.LOB.RiskLevel.Locations.Any())
                {
                    this.Locations = new List<FAR.FarLocation>();
                    foreach (var l in image.LOB.RiskLevel.Locations)
                    {
                        this.Locations.Add(new FarLocation(l));
                    }
                }
                else
                {
#if !DEBUG
                global::IFM.IFMErrorLogging.LogIssue($"Locations was null. Policy #{((image != null) ? image.PolicyNumber : string.Empty)}", "IFMDATASERVICES -> FARSupplementalInfo -> FARSupplementalInfo");
#else
                    Debugger.Break();
#endif
                }
            }
            else
            {
                this.Locations = new List<FAR.FarLocation>();
                foreach (var p in image.PackageParts.Skip(1))
                {
                    if (p?.LOB?.RiskLevel?.Locations != null && image.LOB.RiskLevel.Locations.Any())
                    {

                        foreach (var l in image.LOB.RiskLevel.Locations)
                        {
                            this.Locations.Add(new FarLocation(l));
                        }
                    }
                    else
                    {
#if !DEBUG
                global::IFM.IFMErrorLogging.LogIssue($"Locations was null. Policy #{((image != null) ? image.PolicyNumber : string.Empty)}", "IFMDATASERVICES -> FARSupplementalInfo -> FARSupplementalInfo");
#else
                        Debugger.Break();
#endif
                    }
                }
            }
        }

        internal FARSupplementalInfo(DCO.Policy.Image image, DCO.Policy.UnderlyingPolicy uPol)
        {
            if (uPol?.PolicyInfos?[0] != null && uPol.PolicyInfos[0].Locations.Any())
            {
                this.Locations = new List<FAR.FarLocation>();
                foreach (var l in image.LOB.RiskLevel.Locations)
                {
                    this.Locations.Add(new FarLocation(l));
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