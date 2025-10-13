using System;
using static IFM.DataServicesCore.CommonObjects.Enums.Enums;

namespace IFM.DataServicesCore.CommonObjects.Payments
{
    [System.Serializable]
    public class PayPlanData
    {
        public PayPlans PayPlanId { get; set; }
        public string PolicyNumber { get; set; }
        public Int32 PolicyId { get; set; }
        public string AccountBillNumber { get; set; }
        public Int32 ImageNumber { get; set; }
        public Int32 AccountId { get; set; }
        public Int32 ClientId { get; set; }
        public bool UserAgreedWithTerms { get; set; }
        public RecurringEftInformation RecurringEftInformation { get; set; }
        public RecurringCreditCardInformation RecurringCreditCardInformation { get; set; }

        public bool RequiresRecurringData()
        {
            return PayPlanId == PayPlans.accountBillEFTMonthly || PayPlanId == PayPlans.accountBillCCMontly || PayPlanId == PayPlans.rcc || PayPlanId == PayPlans.rEft;
        }

        public bool IsRccPlan()
        {
            return PayPlanId == PayPlans.accountBillCCMontly || PayPlanId == PayPlans.rcc;
        }

        public bool IsEftPlan()
        {
            return PayPlanId == PayPlans.accountBillEFTMonthly || PayPlanId == PayPlans.rEft;
        }
        public BillingTransactionType TransactionType { get; set; }
    }
}