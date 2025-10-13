using System;
using DCO = Diamond.Common.Objects;

#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class FutureBillingActivityItem : ModelBase
    {
        public string PolicyNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime DueDate { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }

        public FutureBillingActivityItem() { }

        public FutureBillingActivityItem(DCO.Billing.FutureActivity dActivity)
        {
            if (dActivity != null)
            {
                this.PolicyNumber = dActivity.CurrentPolicy;
                this.TransactionDate = dActivity.TranDate;
                this.DueDate = dActivity.DueDate;
                this.Amount = Convert.ToDouble(dActivity.Amount);
                this.Description = dActivity.Dscr;
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