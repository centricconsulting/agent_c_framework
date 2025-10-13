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
    public class QuoteHistoryLookup : PolicyHistoryLookup
    {
        readonly string policyNumber;
        public QuoteHistoryLookup(string policyNumber) : base(policyNumber)
        {
            this.policyNumber = policyNumber;
            GetQuoteHistories();
        }

        public void GetQuoteHistories()
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
                            using (var DSQuoteHistory = Insuresoft.DiamondServices.PolicyService.GetPolicyHistory())
                            {
                                DSQuoteHistory.RequestData.GetPreviousPolicyHistory = true;
                                DSQuoteHistory.RequestData.PolicyId = latestqkLookup.PolicyId;
                                DSQuoteHistory.RequestData.SortResultsInReverseChronologicalOrder = true;
                                var quoteHistory = DSQuoteHistory.Invoke()?.DiamondResponse?.ResponseData?.Data;
                                //list in reverse chronological order
                                if (quoteHistory != null)
                                {
                                    // includes out of order and renewal histories and quotes
                                    //TODO:DJG - NEW PARAMETER ADDED.... WHAT SHOULD WE SET IT TO????
                                    foreach (var ph in quoteHistory.get_Items(true, true, true, false, true, true))
                                    {
                                        PolicyHistories.Add(ph);
                                    }
                                }
                                else
                                {
#if !DEBUG
                global::IFM.IFMErrorLogging.LogIssue($"QuoteHistory was null. - Policy #{(this.policyNumber ?? String.Empty)}", "IFMDATASERVICES -> QuoteHistoryLookup.cs -> Function GetQuoteHistories");
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
                //Doesn't seem log worthy
                //global::IFM.IFMErrorLogging.LogIssue($"QuickLookup was null. - Policy #{(this.policyNumber ?? String.Empty)}", "IFMDATASERVICES -> QuoteHistoryLookup.cs -> Function GetQuoteHistories");
#else
                        Debugger.Break();
#endif
                    }
                }
            }
        }


    }
}
