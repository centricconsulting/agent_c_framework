using System;
using DCO = Diamond.Common.Objects;

#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class AccountLinkedPolicy : ModelBase
    {
        public Int32 AccountNum { get; set; }
        public Int32 AgencyId { get; set; }
        public Int32 BillingAccountId { get; set; }
        public double CurrentoutstandingBalance { get; set; }
        public Int32 PolicyCurrentStatusId { get; set; }
        public string PolicyCurrentStatus { get; set; }
        public Int32 PolicyId { get; set; }
        public Int32 PolicyImageNum { get; set; }
        public string PolicyNumber { get; set; }
        public bool IsPrimaryAccount { get; set; }
        public int AccountBillPriorityLevel { get; set; }
        public AccountLinkedPolicy() { }
        internal AccountLinkedPolicy(DCO.Billing.BillingAccountPolicyLink dLinkPolicy)
        {
            if (dLinkPolicy != null)
            {
                this.AccountNum = dLinkPolicy.AccountNum;
                this.AgencyId = dLinkPolicy.AgencyId;
                this.BillingAccountId = dLinkPolicy.BillingAccountId;
                this.CurrentoutstandingBalance = Convert.ToDouble(dLinkPolicy.CurrentOutstandingAmount);
                this.PolicyCurrentStatusId = dLinkPolicy.PolicyCurrentStatusId;
                //Me.PolicyCurrentStatus = qqhelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.sta, Me.StateId.ToString())
                this.PolicyId = dLinkPolicy.PolicyId;
                this.PolicyImageNum = dLinkPolicy.PolicyImageNum;
                this.PolicyNumber = dLinkPolicy.PolicyNumber != null ? dLinkPolicy.PolicyNumber.ToUpper().Trim() : String.Empty;
                //this.AccountBillPriorityLevel = BusinessLogic.OMP.Billing.GetAccountBillingPriorityLevel(LobId);
            }
#if DEBUG
            else
            {
                Debugger.Break();
            }
#endif
        }
    }
}