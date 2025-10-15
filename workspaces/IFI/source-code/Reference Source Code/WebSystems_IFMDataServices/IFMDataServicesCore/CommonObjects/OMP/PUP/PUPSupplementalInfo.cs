using System;
using System.Collections.Generic;
using DCO = Diamond.Common.Objects;

#if DEBUG

using System.Diagnostics;

#endif

using System.Linq;
using System.Web.UI.WebControls;

namespace IFM.DataServicesCore.CommonObjects.OMP.PUP
{
    [System.Serializable]
    class PUPSupplementalInfo : ModelBase
    {
        public List<PPA.PPASupplementalInfo> PPAPolicies = new List<PPA.PPASupplementalInfo>();
        public List<HOM.HOMSupplementalInfo> HOMPolicies = new List<HOM.HOMSupplementalInfo>();
        //public List<FAR.FARSupplementalInfo> FARPolicies;
        public PUPSupplementalInfo() { }
        internal PUPSupplementalInfo(BasicPolicyInformation pol, DCO.Policy.Image image)
        {
            if (image?.LOB != null && image?.PackageParts.Count == 0)
            {
                foreach (var uPol in image.LOB.UnderlyingPolicies)
                {
                    if (uPol?.PolicyInfos?[0] != null)
                    {
                        switch (uPol.PolicyInfos[0].PrimaryPolicyNumber.Substring(0, 3))
                        {
                            case "PPA":
                                PPAPolicies.Add(new PPA.PPASupplementalInfo(pol, image, uPol));
                                break;
                            case "HOM":
                                HOMPolicies.Add(new HOM.HOMSupplementalInfo(image, uPol));
                                break;
                                //case "FAR":
                                //    FARPolicies.Add(new FAR.FARSupplementalInfo(image, uPol));
                                //    break;
                        }
                    }
                }
            }
            else if (image?.LOB != null && image?.PackageParts.Count > 0)
            {
                foreach (var p in image.PackageParts)
                {
                    foreach (var uPol in p.LOB.UnderlyingPolicies)
                    {
                        if (uPol?.PolicyInfos?[0] != null)
                        {
                            switch (uPol.PolicyInfos[0].PrimaryPolicyNumber.Substring(0, 3))
                            {
                                case "PPA":
                                    PPAPolicies.Add(new PPA.PPASupplementalInfo(pol, image, uPol));
                                    break;
                                case "HOM":
                                    HOMPolicies.Add(new HOM.HOMSupplementalInfo(image, uPol));
                                    break;
                                    //case "FAR":
                                    //    FARPolicies.Add(new FAR.FARSupplementalInfo(image, uPol));
                                    //    break;
                            }
                        }
                    }
                }
            }
        }
    }
}
