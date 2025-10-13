using IFM.PrimitiveExtensions;
using System;
using System.Linq;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.BusinessLogic.OMP
{
    public static class QuickPolicyLookup
    {
        /// <summary>
        /// Do not expose this to the web.
        /// </summary>
        /// <param name="PolicyNumber"></param>
        /// <returns></returns>
        static internal CommonObjects.OMP.QuickLookup LookupPolicyNumber(string PolicyNumber)
        {
            if (DiamondLogin.OMPLogin())
            {
                using (var DS = Insuresoft.DiamondServices.PolicyService.QueryForPolicyNumber())
                {
                    DS.RequestData.PolicyNumber = PolicyNumber;
                    DS.RequestData.OnlyReturnViewableItems = true;
                    DS.RequestData.IsLegacyPolicyNumber = PolicyNumber[0].ToString().IsNumeric();
                    using (var lookup = DS.Invoke()?.DiamondResponse?.ResponseData?.DataItems)
                    {
                        if (lookup != null && lookup.Any())
                        {
                            var latestImage = (from l in lookup orderby l.TransactionEffectiveDate descending select l).First();
                            return new CommonObjects.OMP.QuickLookup(latestImage);
                        }
                    }
                }
            }
            return null;
        }
    }
}