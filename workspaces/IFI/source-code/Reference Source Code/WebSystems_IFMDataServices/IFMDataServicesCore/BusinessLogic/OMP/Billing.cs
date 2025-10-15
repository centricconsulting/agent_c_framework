using Dapper;
using IFM.DataServicesCore.BusinessLogic.Diamond;
using IFM.DataServicesCore.CommonObjects.OMP;
using System;
using System.Collections.Generic;
using System.Linq;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.BusinessLogic.OMP
{
    public class Billing : BusinessLogicBase
    {
        public static List<BillingItem> BillingStatements(DCO.Billing.Data data, bool IsAccountBill,int diamondImageNumber)
        {
            if (IsAccountBill)
                return BillingStatementsNoOrder(data,diamondImageNumber);

            if (BusinessLogic.OMP.DiamondLogin.OMPLogin() && data != null) //Do we need the login call??? Needs some testing... Don't think we do
            //if (data != null)
            {
                List<BillingItem> results = new List<BillingItem>();
                // Need to group by BillingActivityOrder and sum up their PaidAmount to get the true payment in some cases (usually when a late fee has been applied)
                // take last in group to reflect the correct balance
                var GroupedBillingItems = from i in data.Statements where i.PolicyImageNum <= diamondImageNumber group i by i.BillingActivityOrder into g select g;

                foreach (var grp in GroupedBillingItems)
                {
                    if (grp != null && grp.Any())
                    {
                        if (grp.Count() == 1)
                        {
                            results.Add(new BillingItem(grp.First()));
                        }
                        else
                        {
                            grp.Last().PaidAmount = grp.Sum(x => { return x.PaidAmount.TryToGetDouble(); }).ToString();
                            results.Add(new BillingItem(grp.Last()));
                        }
                    }
                }

                results.Reverse();
                // newest to oldest
                return results;
            }

            return null;
        }

        private static List<BillingItem> BillingStatementsNoOrder(DCO.Billing.Data data,int diamondImageNumber)
        {
            if (BusinessLogic.OMP.DiamondLogin.OMPLogin() && data != null) //Do we need the login call??? Needs some testing... Don't think we do
            //if (data != null)
            {
                List<BillingItem> results = new List<BillingItem>();

                foreach (var i in data.Statements)
                {
                    if (i.PolicyImageNum <= diamondImageNumber)
                        results.Add(new BillingItem(i));
                }
                results.Reverse();
                // newest to oldest
                return results;
            }

            return null;
        }


        static internal DCO.Billing.Data BillingData(Int32 policyid, bool LoadAsAccount)
        {
            if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {
                //using (var DS = Insuresoft.DiamondServices.BillingService.GetBillingSummary())
                //{
                //    DS.RequestData.PolicyId = policyid;
                //    var summary = DS.Invoke()?.DiamondResponse?.ResponseData?.BillingSummary;
                //    var current = summary.Summary.CurrentItem;
                //}

                using (var DS = Insuresoft.DiamondServices.BillingService.Load())
                {
                    DS.RequestData.PolicyId = policyid;
                    DS.RequestData.LoadAccountDetail = LoadAsAccount;
                    return DS.Invoke()?.DiamondResponse?.ResponseData?.BillingData;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the priority of the policy in the account. Higher value indicated greater preference toward being the primary policy for the account.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <returns></returns>
        public static int GetAccountBillingPriorityLevel(int lobid)
        {
            int priorityLevel = int.MaxValue;

            // each time the lob doesn't match lower the priority level
            foreach (int l in AccountLobIdOrder())
            {
                if (lobid != l)
                    priorityLevel--;
                else
                    break;
            }
            return priorityLevel;
        }

        /// <summary>
        /// Returns most preferred to least preferred list of LOBID for account bill policies.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<int> AccountLobIdOrder()
        {
            using (var conn = OpenConnection(AppConfig.ConnDiamondReports))
            {
                return conn.Query<int>("dbo.usp_GetBillingAccountLobId", commandType: System.Data.CommandType.StoredProcedure);
            }
        }


    }
}