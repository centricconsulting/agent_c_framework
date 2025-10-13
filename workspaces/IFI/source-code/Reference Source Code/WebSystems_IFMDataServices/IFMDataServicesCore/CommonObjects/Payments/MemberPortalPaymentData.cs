using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFM.DataServicesCore.CommonObjects.Payments
{
    [System.Serializable]
    public class MemberPortalPaymentData
    {
        public IFM_CreditCardProcessing.Enums.UserType UserType { get; set; }
        public string PolicyNumber { get; set; }
        public string AccountNumber { get; set; }
        public Int32 PolicyId { get; set; }
        public Int32 ImageNumber { get; set; }
        public global::IFM.DataServices.API.Enums.PaymentInterface PaymentInterface { get; set; } = DataServices.API.Enums.PaymentInterface.None;
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public double PaymentAmount { get; set; }
        public string FeeAmount { get; set; } //added 9/25/2024; string instead of double so it can go to database as NULL when not used (property set to emptyString)
        public bool UserAgreedWithTerms { get; set; }
        //added 5/29/2020 for Fiserv
        public string Fiserv_AuthToken { get; set; } //obtained for use w/ iframe; not required if not using iframe; would only be used for Card payments (not Bank)
        public string Fiserv_SessionToken { get; set; } //obtained for use w/ iframe; not required if not using iframe; would only be used for Card payments (not Bank)
        public int Fiserv_SessionId { get; set; } //obtained while getting tokens used w/ iframe; not required if not using iframe; would only be used for Card payments (not Bank)
        public string Fiserv_FundingAccountToken { get; set; } //obtained from iframe; not required if not using iframe; would only be used for Card payments (not Bank)
        public bool Fiserv_AddToWallet { get; set; } //set to True to save payment information as new wallet item; will use EmailAddress property for walletItem Create email address, which will be used for Wallet email if it's the 1st walletItem
        public string Fiserv_WalletItemNickname { get; set; } //required when AddToWallet is True
        public bool IsFromOneView { get; set; } //should be set to True for Payments coming from OneView
                                                //added 6/29/2020 for Fiserv
        public string Fiserv_IframeResponse { get; set; } //obtained in response back from iframe (includes fundingMethod, which we will use); not required if not using iframe; would only be used for Card payments (not Bank)
        public global::IFM.DataServices.API.RequestObjects.Payments.WalletItem WalletItemInfo { get; set; } //to be populated when making a wallet payment; would need to populate FundingAccountToken property at minumum
        public global::IFM.DataServices.API.RequestObjects.Payments.ECheckPaymentInformation ECheckPaymentInformation { get; set; }
        public global::IFM.DataServices.API.RequestObjects.Payments.CreditCardPaymentInformation CreditCardPaymentInformation { get; set; }

        public global::IFM.DataServices.API.RequestObjects.Payments.PaymentData ConvertToNewPaymentDataObject()
        {
            var paymentData = new global::IFM.DataServices.API.RequestObjects.Payments.PaymentData()
            {
                Username = UserName,
                UserType = (global::IFM.DataServices.API.Enums.UserType)UserType,
                EmailAddress = EmailAddress,
                PolicyNumber = PolicyNumber,
                AccountBillNumber = AccountNumber,
                PolicyId = PolicyId,
                PolicyImageNumber = ImageNumber,
                UserAgreedWithTerms = UserAgreedWithTerms,
                PaymentAmount = PaymentAmount,
                FeeAmount = FeeAmount, //added 9/25/2024
                ECheckPaymentInformation = ECheckPaymentInformation,
                CreditCardPaymentInformation = CreditCardPaymentInformation,
                WalletItemInfo = WalletItemInfo,

            };

            paymentData.FiservProperties = new DataServices.API.RequestObjects.Payments.FiservProperties()
            {
                AuthToken = Fiserv_AuthToken,
                SessionToken = Fiserv_SessionToken,
                SessionId = Fiserv_SessionId,
                FundingAccountToken = Fiserv_FundingAccountToken,
                AddToWallet = Fiserv_AddToWallet,
                WalletItemNickname = Fiserv_WalletItemNickname,
                IframeResponse  = Fiserv_IframeResponse
            };

            if (PaymentInterface == DataServices.API.Enums.PaymentInterface.None)
            {
                paymentData.PaymentInterface = IsFromOneView == true ? DataServices.API.Enums.PaymentInterface.OneView : DataServices.API.Enums.PaymentInterface.MemberPortalSite;
            }
            else
            {
                paymentData.PaymentInterface = PaymentInterface;
            }

            //For MP and OneTime payments, the user's username is also their emailaddress. If the emailaddress property is not currently valid, lets try to set the emailaddress prop equal to the username, if the username is a valid email address.
            if (paymentData.EmailAddress.IsValidEmail() == false)
            {
                if (paymentData.Username.IsValidEmail())
                {
                    paymentData.EmailAddress = paymentData.Username;
                }
                else
                {
                    paymentData.EmailAddress = "";
                }
            }

            return paymentData;
        }
    }
}
