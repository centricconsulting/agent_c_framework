using System;

namespace IFM.DataServicesCore.CommonObjects.Payments
{
    [System.Serializable]
    public class RecurringData
    {
        public string PolicyNumber { get; set; }
        public Int32 PolicyId { get; set; }
        public RecurringCreditCardInformation RecurringCreditCardInformation { get; set; }
        public RecurringEftInformation RecurringEftInformation { get; set; }
    }
}