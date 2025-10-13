using System;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class QuickLookup
    {
        public bool IsArchived { get; set; }
        public string CurrentPolicy { get; set; }

        public DateTime EffectiveDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string LastTransaction { get; set; }

        public string OriginalTransaction { get; set; }

        public string PolicyDisplayName { get; set; }

        public string PolicyHolderName { get; set; }

        public Int32 PolicyId { get; set; }

        public Int32 PolicyImageNum { get; set; }

        public DateTime TransactionEffectiveDate { get; set; }

        public DateTime TransactionExpirationDate { get; set; }

        public bool IsViewable { get; set; }

        public  bool IsCurrentPolicyIdandImageNumber { get; set; }

        public QuickLookup() { }
        public QuickLookup(DCO.Policy.QuickLookup qlItem, bool IsCurrentPolicyIdandImageNumber = false)
        {
            this.IsCurrentPolicyIdandImageNumber = IsCurrentPolicyIdandImageNumber;
            this.IsArchived = qlItem.Archived;
            this.CurrentPolicy = qlItem.CurrentPolicy;
            this.EffectiveDate = qlItem.EffectiveDate;
            this.ExpirationDate = qlItem.ExpirationDate;
            this.LastTransaction = qlItem.LastTransaction;
            this.OriginalTransaction = qlItem.OriginalTransaction;
            this.PolicyDisplayName = qlItem.PolicyDisplayName;
            this.PolicyHolderName = qlItem.PolicyHolderName;
            this.PolicyId = qlItem.PolicyId;
            this.PolicyImageNum = qlItem.PolicyImageNum;
            this.TransactionEffectiveDate = qlItem.TransactionEffectiveDate;
            this.TransactionExpirationDate = qlItem.TransactionExpirationDate;
            this.IsViewable = qlItem.Viewable;
        }
    }
}