using IFM.DataServicesCore.CommonObjects.OMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IFM.DataServicesCore.BusinessLogic.PublicDomain
{
    public static class PolicyData
    {
        public static List<MinimalPolicyInformation> GetPayableImages(string PolicyNumber, Int32 OnlinePaymentNumber)
        {
            if (BusinessLogic.PublicDomain.PolicyAccess.HasPaymentAccess(PolicyNumber, OnlinePaymentNumber))
            {
                List<MinimalPolicyInformation> results = new List<MinimalPolicyInformation>();
                CommonObjects.OMP.AccountRegistedPolicy pol = new CommonObjects.OMP.AccountRegistedPolicy(PolicyNumber, "", false);
                if (pol.HasFuture)
                {
                    results.Add(pol.MinimalPolicyInformation(pol.FuturePolicyHistory));
                }
                if (pol.HasInforceImage)
                {
                    results.Add(pol.MinimalPolicyInformation(pol.MostCurrentPolicyHistory));
                }
                return results.Any() ? results : null;
            }
            return null;
        }

        public static List<MinimalPolicyInformation> GetPayableImages(string PolicyNumber, string Fullname)
        {
            if (BusinessLogic.PublicDomain.PolicyAccess.HasPaymentAccess(PolicyNumber, Fullname))
            {
                List<MinimalPolicyInformation> results = new List<MinimalPolicyInformation>();
                CommonObjects.OMP.AccountRegistedPolicy pol = new CommonObjects.OMP.AccountRegistedPolicy(PolicyNumber, "", false);
                if (pol.HasFuture)
                {
                    results.Add(pol.MinimalPolicyInformation(pol.FuturePolicyHistory));
                }
                if (pol.HasInforceImage)
                {
                    results.Add(pol.MinimalPolicyInformation(pol.MostCurrentPolicyHistory));
                }
                return results.Any() ? results : null;
            }
            return null;
        }
    }
}