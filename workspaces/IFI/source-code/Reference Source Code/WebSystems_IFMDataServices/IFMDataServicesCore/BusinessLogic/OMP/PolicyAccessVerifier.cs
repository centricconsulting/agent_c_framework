using IFM.DataServicesCore.CommonObjects.OMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFM.PrimitiveExtensions;
using System.Diagnostics;
using IFM.DataServicesCore.CommonObjects.OMP.PPA;

namespace IFM.DataServicesCore.BusinessLogic.OMP
{
    public class PolicyAccessVerifier
    {
        public bool IsValidPolicyAccessVerification(PolicyAccessVerification policyVerificationInfo, ref string errorReason)
        {
            string reason = "";
            bool returnVar = false;
            if (policyVerificationInfo != null)
            {
                try
                {
                    AccountRegistedPolicy polInfo = null;
                    if (policyVerificationInfo.AccountNumber.IsAccountBillNumber())
                    {
                        polInfo = new AccountRegistedPolicy(policyVerificationInfo.AccountNumber.ToUpper().Trim(), "", false);
                    }
                    else
                    {
                        polInfo = new AccountRegistedPolicy(policyVerificationInfo.PolicyNumber.ToUpper().Trim(), "", false);
                    }

                    BasicPolicyInformation image = polInfo?.CurrentImage ?? polInfo.FutureImage;
                    if (image != null)
                    {

                        Func<string, Policyholder> findPolicyHolderByName = (string findName) =>
                        {
                            Policyholder ph = null;
                            if (image.Policyholders != null)
                                ph = (from p in image.Policyholders where p?.Name != null && p.Name.DoesEquateTo(findName) select p).FirstOrDefault();

                            if(ph == null)
                                reason = "Unable to find Policyholder.";

                            return ph;
                        };

                        Func<string, Applicant> findApplicantByName = (string findName) =>
                        {
                            Applicant app = null;
                            if (image?.Applicants != null)
                                app = (from p in image.Applicants where p?.Name != null && p.Name.DoesEquateTo(findName) select p).FirstOrDefault();

                            if(app == null)
                                reason = "Unable to find Applicant.";

                            return app;
                        };


                        Func<Name, bool> verifyName = (Name nm) =>
                        {
                            bool myReturn = false;
                            if (nm?.DisplayName != null)
                                myReturn = nm.DoesEquateTo(policyVerificationInfo.Name);

                            if(myReturn == false)
                                reason = "Unable to verify name.";

                            return myReturn;
                        };

                        Func<Address, bool> verifyZip = (Address ad) =>
                        {
                            bool myReturn = false;
                            if (ad?.Zip5 != null && policyVerificationInfo.Zip != null)
                                myReturn = ad.Zip5 == policyVerificationInfo.Zip.Trim();

                            if(myReturn == false)
                                reason = "Unable to verify zip code.";

                            return myReturn;
                        };

                        Func<Name, bool> verifyFEIN = (Name nm) =>
                        {
                            bool myReturn = false;
                            if (nm?.TaxNumber != null && policyVerificationInfo?.FEIN != null)
                            {
                                if (nm.TaxNumber.ToString().RemoveAny("-", " ").Length >= 4 && policyVerificationInfo.FEIN.ToString().RemoveAny(" ", "-").Length >= 4)
                                {
                                    if (policyVerificationInfo.FEIN.RemoveAny(" ", "-").Length == 4)
                                    {
                                        // compare last 4 only
                                        myReturn = nm.TaxNumber.RemoveAny("-", " ").LastNChars(4) == policyVerificationInfo.FEIN.Trim().RemoveAny("-", " ");
                                    }
                                    else
                                    {
                                        myReturn = nm.TaxNumber.RemoveAny("-", " ") == policyVerificationInfo.FEIN.Trim().RemoveAny("-", " ");
                                    }
                                }
                            }

                            if(myReturn == false)
                                reason = "Unable to verify FEIN.";

                            return myReturn;
                        };

                        Func<Name, bool> verifyDOB = (Name nm) =>
                        {
                            bool myReturn = false;
                            if (nm?.BirthDate != null && policyVerificationInfo?.DOB != null && policyVerificationInfo.DOB.IsDate())
                                myReturn = nm.BirthDate.ToShortDateString() == DateTime.Parse(policyVerificationInfo.DOB).ToShortDateString();

                            if (myReturn == false)
                                reason = "Unable to verify date of birth.";

                            return myReturn;
                        };

                        Func<Name, bool> verifyDLN = (Name nm) => {
                            bool myReturn = false;
                            if (nm?.DLN != null && policyVerificationInfo?.DLN != null)
                                myReturn = nm.DLN.Trim().Replace("-", "") == policyVerificationInfo.DLN.Trim().Replace("-", "");

                            if (myReturn == false)
                                reason = "Unable to verify driver's license number.";

                            return myReturn;
                        };

                        switch (policyVerificationInfo.VerificationTypeId)
                        {
                            case CommonObjects.Enums.Enums.PolicyInformationVerificationLevel.PolicyAndName:
                                {
                                    var ph = findPolicyHolderByName(policyVerificationInfo.Name);
                                    returnVar = verifyName(ph?.Name);
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyInformationVerificationLevel.PolicyFull:
                                switch (image.LobId)
                                {
                                    case 1:
                                    case 51:
                                        //PPA
                                        {
                                            var ph1 = findPolicyHolderByName(policyVerificationInfo.Name);
                                            var drivers = image.SupplementalInformation != null && ((PPASupplementalInfo)image.SupplementalInformation).Drivers != null ? ((PPASupplementalInfo)image.SupplementalInformation).Drivers : null;

                                            returnVar = (ph1 != null && drivers.IsLoaded()) ?
                                                (verifyName(ph1.Name) && verifyDLN(ph1.Name) && verifyZip(ph1.Address)) ||
                                                (verifyName(ph1.Name) &&  drivers.FindAll(c=> c.Name.DLN.Trim().ToLower() == policyVerificationInfo.DLN.Trim().ToLower()).Any() && verifyZip(ph1.Address))
                                                : false;
                                            break;
                                        }
                                    case 2: // HOM
                                        goto case 14;
                                    case 14: //PUP
                                    case 53:
                                        {
                                            if (policyVerificationInfo.PolicyNumber.Substring(0, 3) == "FUP")
                                            {
                                                var ph1 = findPolicyHolderByName(policyVerificationInfo.Name);
                                                returnVar = (ph1 != null) ? verifyName(ph1.Name) && verifyZip(ph1.Address) : false;
                                            }
                                            else
                                            {
                                                var ph1 = findPolicyHolderByName(policyVerificationInfo.Name);
                                                //var ap1 = findApplicantByName(policyVerificationInfo.Name);
                                                //returnVar = ((ph1 != null) ? verifyName(ph1.Name) && verifyDOB(ph1.Name) && verifyZip(ph1.Address) : false)|| ((ap1 != null) ? verifyName(ap1.Name) && verifyDOB(ap1.Name) : false);
                                                returnVar = (ph1 != null) ? verifyName(ph1.Name) && verifyDOB(ph1.Name) && verifyZip(ph1.Address) : false; //74383 :reverting changes for Applicant DOB 
                                            }
                                            break;
                                        }
                                    default:
                                        {
                                            var ph1 = findPolicyHolderByName(policyVerificationInfo.Name);
                                            returnVar = (ph1 != null) ? verifyName(ph1.Name) && verifyZip(ph1.Address) : false;

                                            //&& verifyFEIN(ph3.Name)

                                            //var ap3 = findApplicantByName(policyVerificationInfo.GetNameForCompare());
                                            //return (ph3 != null && ap3 != null) ? verifyName(ph3.Name) && verifyFEIN(ph3.Name) && verifyZip(ph3.Address) : false;
                                            break;
                                        }
                                }
                                break;
                            case CommonObjects.Enums.Enums.PolicyInformationVerificationLevel.OneTimePaymentByName:
                                {
                                    var ph1 = findPolicyHolderByName(policyVerificationInfo.Name);
                                    var ap1 = findApplicantByName(policyVerificationInfo.Name);
                                    returnVar = ((ph1 != null) ? (verifyName(ph1.Name) && verifyZip(ph1.Address)) : false) || ((ap1 != null) ? (verifyName(ap1.Name) && verifyZip(ap1.Address)) : false);
                                    break;
                                }
                            case CommonObjects.Enums.Enums.PolicyInformationVerificationLevel.OneTimePaymentByOnlinePaymentNumber:
                                if (polInfo.AllPolicyIds != null && polInfo.AllPolicyIds.Contains(policyVerificationInfo.OnlinePaymentNumber))
                                    returnVar = true;
                                break;
                            case CommonObjects.Enums.Enums.PolicyInformationVerificationLevel.OneTimePaymentByAccountNumber:
                                if (image.BillingInformation != null && image.BillingInformation.IsPartOfAccountBill && image.BillingInformation.AccountBillId == policyVerificationInfo.AccountNumber.GetBillingAccountIdFromAccountNumber())
                                {
                                    var ph1 = findPolicyHolderByName(policyVerificationInfo.Name);
                                    var ap1 = findApplicantByName(policyVerificationInfo.Name);
                                    returnVar = ((ph1 != null) ? (verifyName(ph1.Name) && verifyZip(ph1.Address)) : false) || ((ap1 != null) ? (verifyName(ap1.Name) && verifyZip(ap1.Address)) : false);
                                }
                                break;
                        }
                        errorReason = reason;
                        return returnVar;
                    }
                    else
                    {
                        //IFMErrorLogging.LogIssue($"Failed to find policy information for {policyVerificationInfo.PolicyNumber}.");
                        global::IFM.IFMErrorLogging.LogIssue($"Failed to find policy information for {policyVerificationInfo.PolicyNumber}.", "IFMDATASERVICES -> PolicyAccessVerifier -> Function IsValidPolicyAccessVerification");
                        errorReason = "Unable to find policy.";
                    }
                }
                catch (Exception ex)
                {
#if !DEBUG
                    global::IFM.IFMErrorLogging.LogException(ex, $"IFMDATASERVICES -> PolicyAccessVerifier -> Function IsValidPolicyAccessVerification - Attempting policy access verification. - Policy Number is " + (policyVerificationInfo?.PolicyNumber != null ? policyVerificationInfo.PolicyNumber :  ""), policyVerificationInfo);
#else
                    Debugger.Break();
#endif
                }
            }
            return false;
        }

    }
}
