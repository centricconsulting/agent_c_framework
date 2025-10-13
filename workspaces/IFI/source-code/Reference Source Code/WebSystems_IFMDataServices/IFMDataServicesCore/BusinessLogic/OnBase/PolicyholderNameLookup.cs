using IFM.PrimitiveExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using DCO = Diamond.Common.Objects;
using DCSM = Diamond.Common.Services.Messages;

namespace IFM.DataServicesCore.BusinessLogic.OnBase
{
    public static class PolicyholderNameLookup
    {
        /// <summary>
        /// Do not expose this to the web.
        /// Finds policyholder names tied to a specified policy
        /// </summary>
        /// <param name="PolicyNumber"></param>
        /// <returns>list of policyholder names</returns>
        static internal List<string> LookupPolicyholderName(string policyNumber)
        {
            var lookup = new Diamond.Policy.PolicyImage(policyNumber); // this is expensive call so cache this if you find yourself pulling the same image more than once per request
            return GatherPolicyholderNames(lookup?.Image);            
        }

        internal static List<string> GatherPolicyholderNames(DCO.Policy.Image image)
        {            
            List<string> policyholderNames = new List<string>();
            
            if (image != null)
            {
                string policyholderName = "";
                // policyholder 1
                policyholderName = (image.PolicyHolder?.Name.FirstName + " " + image.PolicyHolder?.Name.LastName).Trim();
                if (!string.IsNullOrWhiteSpace(policyholderName))
                {
                    policyholderNames.Add(policyholderName);
                }
                policyholderName = image.PolicyHolder?.Name.CommercialName1;
                if (!string.IsNullOrWhiteSpace(policyholderName))
                {
                    policyholderNames.Add(policyholderName);
                }
                policyholderName = image.PolicyHolder?.Name.CommercialName2;
                if (!string.IsNullOrWhiteSpace(policyholderName))
                {
                    policyholderNames.Add(policyholderName);
                }

                // policyholder 2
                policyholderName = (image.PolicyHolder2?.Name.FirstName + " " + image.PolicyHolder2?.Name.LastName).Trim();
                if (!string.IsNullOrWhiteSpace(policyholderName))
                {
                    policyholderNames.Add(policyholderName);
                }
                policyholderName = image.PolicyHolder2?.Name.CommercialName1;
                if (!string.IsNullOrWhiteSpace(policyholderName))
                {
                    policyholderNames.Add(policyholderName);
                }
                policyholderName = image.PolicyHolder2?.Name.CommercialName2;
                if (!string.IsNullOrWhiteSpace(policyholderName))
                {
                    policyholderNames.Add(policyholderName);
                }

                // additional names
                if (image.AdditionalPolicyHolders != null)
                {
                    foreach (var additionalPH in image.AdditionalPolicyHolders)
                    {
                        if(additionalPH.Name != null)
                        {
                            //TODO: DJG - object has changed. Should we use the Name object to find all of these things????
                            policyholderName = $"{additionalPH.Name.FirstName} {additionalPH.Name.LastName}".Trim();
                            if (!string.IsNullOrWhiteSpace(policyholderName))
                            {
                                policyholderNames.Add(policyholderName);
                            }
                            if (!string.IsNullOrWhiteSpace(additionalPH.Name.CommercialName1))
                            {
                                policyholderNames.Add(additionalPH.Name.CommercialName1);
                            }
                            if (!string.IsNullOrWhiteSpace(additionalPH.Name.CommercialName2))
                            {
                                policyholderNames.Add(additionalPH.Name.CommercialName2);
                            }
                        }
                    }
                }

            }
            
            //remove duplicates before returning
            return policyholderNames.Distinct().ToList();
        }


        /// <summary>
        /// Do not expose this to the web.
        /// Finds policyholder dba names tied to a specified policy
        /// </summary>
        /// <param name="PolicyNumber"></param>
        /// <returns>List of policyholderDBANames</returns>
        static internal List<string> LookupPolicyholderDBA(string policyNumber)
        {
            var lookup = new Diamond.Policy.PolicyImage(policyNumber); // this is expensive call so cache this if you find yourself pulling the same image more than once per request
            return GatherPolicyDBANames(lookup?.Image);
        }

        internal static List<string> GatherPolicyDBANames(DCO.Policy.Image image)
        {
            List<string> policyholderDBAs = new List<string>();
            if (image != null){
                string policyholderDBA = "";
                policyholderDBA = image.PolicyHolder?.Name.DoingBusinessAs;
                if (!string.IsNullOrWhiteSpace(policyholderDBA))
                {
                    policyholderDBAs.Add(policyholderDBA);
                }
                policyholderDBA = image.PolicyHolder2?.Name.DoingBusinessAs;
                if (!string.IsNullOrWhiteSpace(policyholderDBA))
                {
                    policyholderDBAs.Add(policyholderDBA);
                }
            }
            //remove duplicates before returning
            return policyholderDBAs.Distinct().ToList();
        }


    }
}