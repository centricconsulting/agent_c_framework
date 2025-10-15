using System;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class PolicyHistory
    {
        public string Description { get; set; }
        public DateTime LastModified { get; set; }
        public Int32 PolicyId { get; set; }
        public Int32 PolicyImageNumber { get; set; }
        public Int32 PolicyStatusCodeId { get; set; }
        public double PremiumFullTerm { get; set; }
        public double PremiumWritten { get; set; }
        public double PremiumChangeWritten { get; set; }
        public double PremiumDifferentChangeWritten { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Reason { get; set; }

        public string Remark { get; set; }

        public PolicyHistory() { }
        public PolicyHistory(DCO.Policy.History dPolicyHistory)
        {
            this.Description = dPolicyHistory.Description;
            this.LastModified = dPolicyHistory.LastModifiedDate ?? DateTime.MinValue;
            this.PolicyId = dPolicyHistory.PolicyId;
            this.PolicyImageNumber = dPolicyHistory.PolicyImageNum;
            this.PolicyStatusCodeId = dPolicyHistory.PolicyStatusCodeId;
            this.PremiumFullTerm = Convert.ToDouble(dPolicyHistory.PremiumFullterm);
            this.PremiumWritten = Convert.ToDouble(dPolicyHistory.PremiumWritten);
            this.PremiumChangeWritten = Convert.ToDouble(dPolicyHistory.PremiumChangeWritten);
            this.PremiumDifferentChangeWritten = Convert.ToDouble(dPolicyHistory.PremiumDifferentChangeWritten);

            this.EffectiveDate = dPolicyHistory.TEffDate;
            this.ExpirationDate = dPolicyHistory.TExpDate;
            this.Reason = dPolicyHistory.TransReason;
            this.Remark = dPolicyHistory.TransRemark;
        }

        public override string ToString()
        {
            return $"{Description} - PolicyId: {PolicyId}/{PolicyImageNumber}";
        }

    }
}