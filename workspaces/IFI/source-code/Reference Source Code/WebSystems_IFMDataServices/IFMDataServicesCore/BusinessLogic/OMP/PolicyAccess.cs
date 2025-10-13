using IFM.DataServicesCore.CommonObjects.OMP;
using IFM.PrimitiveExtensions;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Web;
using IFM.DataServicesCore.BusinessLogic.OMP;
using System.Threading.Tasks;

namespace IFM.DataServicesCore.BusinessLogic.OMP
{
    public static class PolicyAccess
    {
        private const string location = "IFM.DataServicesCore.BusinessLogic.OMP.PolicyAccess";

        public static List<AccountRegistedPolicy> GetRegisteredAccountPoliciesAsync(List<MemberAccountPolicy> accountList)
        {

            // list to keep multiple threads from trying to load the same policy information
            var startedLoadingPolicyNumberList = new System.Collections.Concurrent.ConcurrentBag<string>();
            var registeredPoliciesBag = new System.Collections.Concurrent.ConcurrentBag<AccountRegistedPolicy>();

            HttpContext ctx = HttpContext.Current;

            if (accountList != null && DiamondLogin.OMPLogin())
            {
#if DEBUG
                ParallelOptions options = new ParallelOptions() { MaxDegreeOfParallelism = 1 };
#else
                ParallelOptions options = new ParallelOptions() { MaxDegreeOfParallelism = 10 };
#endif

                var policyExceptions = new List<string>();
                var exceptions = new ConcurrentQueue<Exception>();

                Parallel.ForEach(accountList, options, (ap) =>
                {
                    try
                    {
                        HttpContext.Current = ctx;
                        if (string.IsNullOrWhiteSpace(ap.PolicyNumber) == false)
                        {
                            string policyNumber = ap.PolicyNumber.Trim().ToUpper();
                            string PolicyNickname = ap.NickName.Trim();

                            if (policyNumber.Length > 1)
                            {
                                if (policyNumber[0].ToString().IsNumeric() == false)
                                {
                                    if ((from p in registeredPoliciesBag where p.PolicyNumber == policyNumber select p).Any() == false && startedLoadingPolicyNumberList.Contains(policyNumber) == false)
                                    {
                                        startedLoadingPolicyNumberList.Add(policyNumber.ToUpper());
                                        registeredPoliciesBag.Add(new AccountRegistedPolicy(policyNumber, PolicyNickname, true));
                                    }
                                }
                                else
                                {
                                    // is a legacy policy number
                                    using (PolicyNumberObject pno = new PolicyNumberObject(policyNumber))
                                    {
                                        if (pno.hasError == false && pno.IsInLegacyFormat)
                                        {
                                            var q = BusinessLogic.OMP.QuickPolicyLookup.LookupPolicyNumber(pno.TwelveDigitPolicy);
                                            if (q != null && (from p in registeredPoliciesBag where p.PolicyNumber == q.CurrentPolicy select p).Any() == false && startedLoadingPolicyNumberList.Contains(q.CurrentPolicy.ToUpper().Trim()) == false)
                                            {
                                                startedLoadingPolicyNumberList.Add(q.CurrentPolicy.ToUpper().Trim());
                                                registeredPoliciesBag.Add(new AccountRegistedPolicy(q.CurrentPolicy.ToUpper().Trim(), PolicyNickname, true));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
#if DEBUG
                            Debugger.Break();
#else
                        global::IFM.IFMErrorLogging.LogIssue($"Invalid registered policy entry. - PolicyNumber '{ap.PolicyNumber??"null"} Nickname'{ap.NickName??"null"}''", $"{location}.GetRegisteredAccountPoliciesAsync");
#endif
                        }
                    }
                    catch (Exception ex)
                    {
                        policyExceptions.Add(ap.PolicyNumber);
                        exceptions.Enqueue(ex);
                    }
                });
                if (exceptions.Count > 0)
                {
                    int count = 0;
                    foreach (var ex in exceptions)
                    {
                        string thisPolicy = "";
                        if (policyExceptions.IsLoaded() && policyExceptions.Count > count)
                        {
                            thisPolicy = $"; Policy: {policyExceptions[count]}";
                        }
                        global::IFM.IFMErrorLogging.LogException(ex, $"{location}.GetRegisteredAccountPoliciesAsync{thisPolicy}");
                        count++;
                    }
                }
            }

            if (registeredPoliciesBag.Count > 0)
            {
                //pull in Account Bill Linked Policies
                AddAssociatedAccountBillPoliciesAsync(registeredPoliciesBag);

                var registeredPolicies = registeredPoliciesBag.ToList();

                foreach (var registerdPolicy in registeredPolicies)
                {
                    if (registerdPolicy.CurrentImage != null  && registerdPolicy.FutureImage != null && !string.IsNullOrEmpty(registerdPolicy.CurrentImage.ReWrittenTo) && registerdPolicy.PolicyNumber == registerdPolicy.CurrentImage.PolicyNumber
                        && (registerdPolicy.CurrentImage.EffectiveDate == registerdPolicy.FutureImage.EffectiveDate   
                        || registeredPolicies.Exists(rp => rp != registerdPolicy && rp.PolicyNumber == registerdPolicy.FutureImage.PolicyNumber)))
                    {
                        registerdPolicy.FutureImage = null;
                    }
                    else if (registerdPolicy.CurrentImage != null && registerdPolicy.FutureImage != null && !string.IsNullOrEmpty(registerdPolicy.CurrentImage.ReWrittenTo) && registerdPolicy.PolicyNumber != registerdPolicy.CurrentImage.PolicyNumber)
                    {
                        registerdPolicy.CurrentImage = registerdPolicy.FutureImage;
                        registerdPolicy.FutureImage = null;
                    }
                }

                // if they have a policy registred but it is also pulled in via a rewrittenTo or the like this will make sure there are no duplicates
                registeredPolicies = registeredPolicies.DistinctBy(p => p?.CurrentImage?.PolicyNumber).ToList();

                FilterRegisteredPolicyList(ref registeredPolicies);

                // used to garantee that if a single policy number is requested it is always first in the returned list - onetime payment relies on this to be true
                registeredPolicies = (from rp in registeredPolicies orderby rp.IsRegisteredPolicy descending select rp).ToList();

                // need to find primary account bill policy
                SetPrimaryPolicyForAnyAccountBillPolicies(registeredPolicies);

                return registeredPolicies;
            }
            else
            {
                return null;
            }
        }




        private static void FilterRegisteredPolicyList(ref List<AccountRegistedPolicy> registeredPolicies)
        {
            registeredPolicies = (from rp in registeredPolicies where rp.PolicyHistories != null && rp.PolicyHistories.Any() select rp).ToList();
            registeredPolicies = (from rp in registeredPolicies where rp.CurrentImage != null select rp).ToList();
            registeredPolicies = (from rp in registeredPolicies where rp.CurrentImage.PolicyStatusId.NotEqualsAny(AccountRegistedPolicy.UndesireablePolicyStatusIds()) select rp).ToList(); // removed pending,quote,archived quote
        }



        private static void AddAssociatedAccountBillPoliciesAsync(System.Collections.Concurrent.ConcurrentBag<AccountRegistedPolicy> registeredPolicies)
        {
            HttpContext ctx = HttpContext.Current;
            // list to keep multiple threads from trying to load the same policy information
            var startedLoadingPolicyNumberList = new System.Collections.Concurrent.ConcurrentBag<string>();

            if (registeredPolicies != null)
            {
                Parallel.ForEach(new List<AccountRegistedPolicy>(registeredPolicies), (p) => {
                    if (p.CurrentImage?.LinkedAccountPolicies != null)// && p.CurrentImage.BillingInformation.IsPartOfAccountBill)
                    {
                        Parallel.ForEach(p.CurrentImage.LinkedAccountPolicies, (lp) => {
                            if ((from a in registeredPolicies where a.PolicyNumber == lp.PolicyNumber select a).Any() == false && startedLoadingPolicyNumberList.Contains(lp.PolicyNumber.ToUpper()) == false)
                            {
                                HttpContext.Current = ctx;
                                startedLoadingPolicyNumberList.Add(lp.PolicyNumber.ToUpper());
                                registeredPolicies.Add(new AccountRegistedPolicy(lp.PolicyNumber.ToUpper().Trim(), "", false));
                            }
                        });
                    }
                });
            }
        }

        private static void SetPrimaryPolicyForAnyAccountBillPolicies(IEnumerable<AccountRegistedPolicy> registeredPolicies)
        {
            if (registeredPolicies != null)
            {
                var accountBillPolicies = from p in registeredPolicies where p?.CurrentImage?.BillingInformation != null && p.CurrentImage.BillingInformation.IsPartOfAccountBill select p;

                // check if there are any account bill policies
                if (accountBillPolicies.Any())
                {
                    // group policies by accountid
                    var groupedAccounts = accountBillPolicies.GroupBy(p => p.CurrentImage.BillingInformation.AccountBillId).Select(p => p);
                    foreach (var g in groupedAccounts)
                    {
                        // find the one with highest priority level that is direct pay and is inforce
                        // We don't care about inforce vs cancelled anymore... Just get the primary policy despite the  policy status
                        //var inForceDirectBillPolicies = g.Where(p => p.CurrentImage.PolicyStatusId == 1 && p.CurrentImage.BillingInformation.IsDirectBill);
                        var DirectBillPolicies = g.Where(p => p.CurrentImage.BillingInformation.IsDirectBill);
                        if (DirectBillPolicies != null && DirectBillPolicies.Any())
                        {
                            var primaryPolicy = (DirectBillPolicies.OrderBy(p => p.CurrentImage.BillingInformation.AccountBillPriorityLevel)).Last();
                            primaryPolicy.CurrentImage.BillingInformation.IsPrimaryAccountPolicy = true;
                        }
                    }
                }

                var futureAccountBillPolicies = from p in registeredPolicies where p?.FutureImage?.BillingInformation != null && p.FutureImage.BillingInformation.IsPartOfAccountBill select p;

                if (futureAccountBillPolicies.Any())
                {
                    var groupedFutureAccounts = futureAccountBillPolicies.GroupBy(p => p.FutureImage.BillingInformation.AccountBillId).Select(p => p);
                    foreach (var g in groupedFutureAccounts)
                    {
                        // find the one with highest priority level that is direct pay and is Future
                        //var futureImageDirectBillPolicies = g.Where(p => p.FutureImage.PolicyStatusId == 2 && p.FutureImage.BillingInformation.IsDirectBill);
                        var DirectBillPolicies = g.Where(p => p.FutureImage.BillingInformation.IsDirectBill);
                        if (DirectBillPolicies != null && DirectBillPolicies.Any())
                        {
                            var futurePrimaryPolicy = (DirectBillPolicies.OrderBy(p => p.FutureImage.BillingInformation.AccountBillPriorityLevel)).Last();
                            futurePrimaryPolicy.FutureImage.BillingInformation.IsPrimaryAccountPolicy = true;
                        }
                    }
                }
            }
        }


        public static BasicPolicyInformation GetPolicyInformation(Int32 policyId, Int32 policyImageNumber)
        {
            if (policyId > 0 && policyImageNumber > 0 && DiamondLogin.OMPLogin())
            {
                using (var DS = Insuresoft.DiamondServices.PolicyService.LoadImage())
                {
                    DS.RequestData.PolicyId = policyId;
                    DS.RequestData.ImageNumber = policyImageNumber;

                    using (var image = DS.Invoke()?.DiamondResponse?.ResponseData?.Image)
                    {
                        if (image != null)
                        {
                            return new CommonObjects.OMP.BasicPolicyInformation(image, null);
                        }
                        else
                        {
#if !DEBUG
                global::IFM.IFMErrorLogging.LogIssue($"Failed to load image. - PolicyId'{policyId}' ImageNum'{policyImageNumber}'.", $"{location}.GetPolicyInformation");
#else
                            Debugger.Break();
#endif
                        }
                    }
                }
            }

            return null;
        }

        public static List<PolicyHistory> GetPolicyHistories(string PolicyNumber)
        {
            var PolicyHistories = new List<PolicyHistory>();
            if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {
                using (var DSLookup = Insuresoft.DiamondServices.PolicyService.QueryForPolicyNumber())
                {
                    DSLookup.RequestData.PolicyNumber = PolicyNumber;
                    DSLookup.RequestData.IsLegacyPolicyNumber = String.IsNullOrWhiteSpace(PolicyNumber) == false ? PolicyNumber[0].ToString().IsNumeric() : false;
                    DSLookup.RequestData.OnlyReturnViewableItems = true;
                    var response = DSLookup.Invoke();
                    var lookup = response?.DiamondResponse?.ResponseData?.DataItems;
                    if (lookup != null && lookup.Any())
                    {
                        var latestqkLookup = (from l in lookup orderby l.TransactionEffectiveDate descending select l).FirstOrDefault();
                        if (latestqkLookup != null)
                        {
                            using (var DSPolicyHistory = Insuresoft.DiamondServices.PolicyService.GetPolicyHistory())
                            {
                                DSPolicyHistory.RequestData.GetPreviousPolicyHistory = true;
                                DSPolicyHistory.RequestData.PolicyId = latestqkLookup.PolicyId;
                                DSPolicyHistory.RequestData.SortResultsInReverseChronologicalOrder = true;
                                var policyHistory = DSPolicyHistory.Invoke()?.DiamondResponse?.ResponseData?.Data;
                                //list in reverse chronological order
                                if (policyHistory != null)
                                {
                                    // includes out of order and renewal histories but not quotes or audits
                                    foreach (var ph in policyHistory.get_Items(false, false, true, false, true, true))
                                    {
                                        PolicyHistories.Add(new PolicyHistory(ph));
                                    }
                                }
                                else
                                {
#if !DEBUG
                                    global::IFM.IFMErrorLogging.LogIssue($"{location}.GetPolicyHistories - PolicyHistory was null. - Policy #{(PolicyNumber ?? String.Empty)}");
#else
                                    Debugger.Break();
#endif
                                }
                            }
                        }

                    }
                    else
                    {
#if !DEBUG
                        global::IFM.IFMErrorLogging.LogIssue($"{location}.GetPolicyHistories - QuickLookup was null. - Policy #{(PolicyNumber ?? String.Empty)}");
#else
                        Debugger.Break();
#endif
                    }
                }
            }
            return PolicyHistories;
        }
    }
}