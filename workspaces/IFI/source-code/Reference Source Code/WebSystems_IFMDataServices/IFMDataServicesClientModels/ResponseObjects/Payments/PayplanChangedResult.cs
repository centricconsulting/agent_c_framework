using IFM.DataServices.API.ResponseObjects.Common;

namespace IFM.DataServices.API.ResponseObjects.Payments
{
    [System.Serializable]
    public class PayplanChangedResult : ServiceResult
    {
        public bool PayPlanChanged { get; set; }
        public bool RecurringPaymentDataFailed { get; set; }
    }
}