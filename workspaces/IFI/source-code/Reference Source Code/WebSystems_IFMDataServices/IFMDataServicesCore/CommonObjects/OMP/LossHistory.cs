using System;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class LossHistory : ModelBase
    {
        public double Amount { get; set; }
        public string ClaimNumber { get; set; }
        public DateTime LossDate { get; set; }
        public string Description { get; set; }
        public Int32 SurChargeId { get; set; }
        public string SurCharge { get; set; }

        public LossHistory() { }
        internal LossHistory(DCO.Policy.LossHistory dLoss)
        {
            this.Amount = Convert.ToDouble(dLoss.Amount);
            this.ClaimNumber = dLoss.ClaimNumber;
            this.LossDate = dLoss.LossDate;
            this.Description = dLoss.LossDescription;
            this.SurChargeId = dLoss.LossHistorySurchargeId;
            this.SurCharge = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLossHistoryRecord, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.LossHistorySurchargeId, this.SurChargeId.ToString());
        }
    }
}