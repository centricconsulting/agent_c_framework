using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using IFM.PrimitiveExtensions;
using System.Text;
using System.Threading.Tasks;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.BusinessLogic.Diamond.Policy
{
    public class PolicyHistoryLookup
    {
        readonly string policyNumber;
        public DCO.Policy.History MostCurrentPolicyHistory
        {
            //1 = InForce, 2 = Future, 3 = History, 4 = Pending, 5 = Renewal Offer, 12 = Quote, 13 = Archived Quote
            get
            {

                return PolicyHistories != null && PolicyHistories.Any()
                    ? (from p in PolicyHistories where p.PolicyStatusCodeId != 2 orderby p.PolicyStatusCodeId ascending, p.TEffDate descending select p).FirstOrDefault()
                    : null;
            }
            set { }
        }

        public DCO.Policy.History FuturePolicyHistory
        {
            //1 = InForce, 2 = Future, 3 = History, 4 = Pending, 5 = Renewal Offer, 12 = Quote
            get { return PolicyHistories != null && PolicyHistories.Any() ? (from p in PolicyHistories where p.PolicyStatusCodeId == 2 orderby p.TEffDate descending select p).FirstOrDefault() : null; }
            set { }
        }

        public List<Int32> AllPolicyIds
        {
            get { return PolicyHistories != null && PolicyHistories.Any() ? (from p in PolicyHistories select p.PolicyId).Distinct().ToList() : null; }
            set { }
        }

        public List<DCO.Policy.History> PolicyHistories { get; set; }
        public PolicyHistoryLookup(string policyNumber)
        {
            this.policyNumber = policyNumber;
            GetPolicyHistories();
        }

        public void GetPolicyHistories()
        {
            PolicyHistories = new List<DCO.Policy.History>();

            if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {
                using (var DSLookup = Insuresoft.DiamondServices.PolicyService.QueryForPolicyNumber())
                {
                    DSLookup.RequestData.PolicyNumber = this.policyNumber;
                    DSLookup.RequestData.IsLegacyPolicyNumber = !string.IsNullOrWhiteSpace(this.policyNumber) && this.policyNumber[0].ToString().IsNumeric();
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
                                        PolicyHistories.Add(ph);
                                    }
                                }
                                else
                                {
#if !DEBUG
                                    global::IFM.IFMErrorLogging.LogIssue($"PolicyHistory was null. - Policy #{(this.policyNumber ?? String.Empty)}", "IFMDATASERVICES -> PolicyHistoryLookup.cs -> Function GetPolicyHistoies");
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
                        //Do we want this logged???? Seems doubtful.
                        //global::IFM.IFMErrorLogging.LogIssue($"QuickLookup was null. Policy #{(this.policyNumber ?? String.Empty)}", "IFMDATASERVICES -> PolicyHistoryLookup.cs -> Function GetPolicyHistoies");
#else
                        Debugger.Break();
#endif
                    }
                }
            }
        }


    }
}
