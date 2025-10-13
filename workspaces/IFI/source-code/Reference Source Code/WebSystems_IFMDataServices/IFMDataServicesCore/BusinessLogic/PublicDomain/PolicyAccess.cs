using IFM.PrimitiveExtensions;
using System;

namespace IFM.DataServicesCore.BusinessLogic.PublicDomain
{
    public static class PolicyAccess
    {
        /// <summary>
        /// Determines basic policy ownership via policynumber and fullname comparison.
        /// </summary>
        /// <param name="PolicyNumber"></param>
        /// <param name="FullName"></param>
        /// <returns></returns>
        public static bool HasPaymentAccess(string PolicyNumber, string FullName)
        {
            var lookup = BusinessLogic.OMP.QuickPolicyLookup.LookupPolicyNumber(PolicyNumber);
            if (lookup != null)
            {
                if (string.IsNullOrWhiteSpace(FullName) == false)
                    return FullName.Replace_NullSafe(" ", "").ToLower() == lookup.PolicyDisplayName.Replace_NullSafe(" ", "").ToLower();
                else
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Determines basic policy ownership via policynumber and online payment number comparison.
        /// </summary>
        /// <param name="PolicyNumber"></param>
        /// <param name="OnlinePaymentNumber"></param>
        /// <returns></returns>
        public static bool HasPaymentAccess(string PolicyNumber, Int32 OnlinePaymentNumber)
        {
            var lookup = BusinessLogic.OMP.QuickPolicyLookup.LookupPolicyNumber(PolicyNumber);
            if (lookup != null)
            {
                return lookup.PolicyId == OnlinePaymentNumber;
            }
            return false;
        }
    }
}