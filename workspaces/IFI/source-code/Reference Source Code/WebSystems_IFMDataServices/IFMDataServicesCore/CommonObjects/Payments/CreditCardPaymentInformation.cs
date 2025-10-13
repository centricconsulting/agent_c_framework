using IFM.PrimitiveExtensions;
using System;

namespace IFM.DataServicesCore.CommonObjects.Payments
{
    [System.Serializable]
    public class CreditCardPaymentInformation
    {
        public string CardNumber { get; set; }
        public Int32 CardExpireMonth { get; set; }
        public Int32 CardExpireYear { get; set; }
        public string NameOnCard { get; set; }
        public string ZIPCode { get; set; } //7/19/2020 note: required by Fiserv when not using their iframe
        public string SecurityCode { get; set; } //added 7/19/2020; required by Fiserv when not using their iframe

        public bool PassesBasicValidation()
        {
            DateTime.TryParse($"{CardExpireMonth}/1/{CardExpireYear}", out DateTime expireDate);
            if (expireDate != DateTime.MinValue)
            {
                expireDate = expireDate.AddMonths(1);
            }
            else
            {
                return false;
            }

            return CardNumber.IsNullEmptyOrWhitespace() == false && DateTime.Now < expireDate && NameOnCard.IsNullEmptyOrWhitespace() == false;
        }
    }
}