using System.Collections.Generic;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.API.RequestObjects.Payments
{
    [System.Serializable]
    public class PaymentDataList : BaseRequest
    {
        public List<PaymentData> Payments { get; set; } = new List<PaymentData>();

        public PaymentDataList(string APIAddress) : base(APIAddress)
        {
            API_Path = "IFM/Payment/Processor";
        }

        public string GetJsonStringForPayload()
        {
            return GetJsonAsStringForRequestObject(this.Payments);
        }

        /// <summary>
        /// Will make the payment to our payment vendor (charging the policyholder). Then it will post the payment to our system. This will follow after hours logic as well.
        /// </summary>
        /// <returns></returns>
        public APIResponses.Common.ServiceResult<List<APIResponses.Payments.BulkPaymentResult>> MakeBulkPayment()
        {
            EncryptInfo();
            API_Endpoint = "MakeBulkPayment";
            return Post<APIResponses.Common.ServiceResult<List<APIResponses.Payments.BulkPaymentResult>>>(this.Payments);
        }

        /// <summary>
        /// Will only post the payment to our system. Does NOT attempt to make the payment to our payment vendor.
        /// </summary>
        /// <returns></returns>
        public APIResponses.Common.ServiceResult<List<APIResponses.Payments.BulkPaymentResult>> PostBulkPayment()
        {
            EncryptInfo();
            API_Endpoint = "PostBulkPayment";
            return Post<APIResponses.Common.ServiceResult<List<APIResponses.Payments.BulkPaymentResult>>>(this.Payments);
        }

        private void EncryptInfo()
        {
            if (Payments.IsLoaded())
            {
                foreach (var payment in Payments)
                {
                    payment.EncryptInfo();
                }
            }
        }
    }
}