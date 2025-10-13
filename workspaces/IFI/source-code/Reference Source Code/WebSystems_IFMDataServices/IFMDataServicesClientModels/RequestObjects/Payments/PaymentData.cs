using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.API.RequestObjects.Payments
{
    [System.Serializable]
    public class PaymentData : BaseRequest
    {
        public string PolicyNumber { get; set; }
        public string AccountBillNumber { get; set; }  //Account Bill Number
        public Int32 PolicyId { get; set; }
        public int PolicyImageNumber { get; set; }
        public Enums.PaymentInterface PaymentInterface { get; set; }
        public Enums.CashSource CashInSource { get; set; }
        public bool UserAgreedWithTerms { get; set; }
        public int UserId { get; set; }
        public int AgencyId { get; set; }
        public Enums.UserType UserType { get; set; }
        public double PaymentAmount { get; set; }
        public string Username { get; set; }
        public string UserLoginDomain { get; set; }
        public string UserPassword { get; set; }
        public string EmailAddress { get; set; }
        public string FeeAmount { get; set; } //added 9/25/2024; string instead of double so it can go to database as NULL when not used (property set to emptyString)

        //public bool WithApp { get; set; } //shouldn't be used anymore
        public FiservProperties FiservProperties { get; set; }
        public PaymentSettings PaymentSettings { get; set; }
        public EFTInformation EFTInformation { get; set;}
        public ECheckPaymentInformation ECheckPaymentInformation { get; set; }
        public CreditCardPaymentInformation CreditCardPaymentInformation { get; set; }
        public PostPaymentInfo PostPaymentInfo { get; set; }
        public WalletItem WalletItemInfo { get; set; } //to be populated when making a wallet payment; would need to populate FundingAccountToken property at minumum

        /// <summary>
        /// Use for API calls
        /// </summary>
        /// <param name="APIAddress"></param>
        public PaymentData(string APIAddress) : base(APIAddress) 
        {
            API_Path = "IFM/Payment/Processor";
        }


        /// <summary>
        /// Do not use this constructor if you want to use this class to send an API request. Only use this if you need to use the class for storing information in the object.
        /// </summary>
        public PaymentData() : base("")
        {
            API_Path = "IFM/Payment/Processor";
        }

        /// <summary>
        /// Will make the payment to our payment vendor (charging the policyholder). Then it will post the payment to our system. This will follow after hours logic as well.
        /// </summary>
        /// <returns></returns>
        public APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> MakePayment()
        {
            EncryptInfo();
            API_Endpoint = "MakePayment";
            return Post<APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult>>(this);
        }

        /// <summary>
        /// Will only post the payment to our system. Does NOT attempt to make the payment to our payment vendor.
        /// </summary>
        /// <returns></returns>
        public APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> PostPayment()
        {
            EncryptInfo();
            API_Endpoint = "PostPayment";
            return Post<APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult>>(this);
        }

        public APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> ApplyCash()
        {
            API_Endpoint = "ApplyCash";
            return Post<APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult>>(this);
        }

        internal void EncryptInfo()
        {
            //Not sure this will ever be used... Don't think so.
            if (UserPassword.IsNotNullEmptyOrWhitespace())
            {
                UserPassword = UserPassword.DoubleEncrypt();
            }
            if (ECheckPaymentInformation != null)
            {
                if (ECheckPaymentInformation.RoutingNumber.IsNumeric())
                {
                    ECheckPaymentInformation.RoutingNumber = ECheckPaymentInformation.RoutingNumber.DoubleEncrypt();
                }
                if (ECheckPaymentInformation.AccountNumber.IsNumeric())
                {
                    ECheckPaymentInformation.AccountNumber = ECheckPaymentInformation.AccountNumber.DoubleEncrypt();
                }
            }
            if (CreditCardPaymentInformation != null)
            {
                //Shouldn't be getting passed around.... but just in case
                if (CreditCardPaymentInformation.CardNumber.IsNumeric())
                {
                    CreditCardPaymentInformation.CardNumber = CreditCardPaymentInformation.CardNumber.DoubleEncrypt();
                }
                if (CreditCardPaymentInformation.SecurityCode.IsNumeric())
                {
                    CreditCardPaymentInformation.SecurityCode = CreditCardPaymentInformation.SecurityCode.DoubleEncrypt();
                }
                if (CreditCardPaymentInformation.ZIPCode.IsValidZipCode())
                {
                    CreditCardPaymentInformation.ZIPCode = CreditCardPaymentInformation.ZIPCode.DoubleEncrypt();
                }
            }
        }
    }

        //[System.Serializable]
        //public class RecurringData
        //{
        //    public string PolicyNumber { get; set; }
        //    public Int32 PolicyId { get; set; }
        //    public RecurringCreditCardInformation RecurringCreditCardInformation { get; set; }
        //    public RecurringEftInformation RecurringEftInformation { get; set; }
        //}

    [System.Serializable]
    public class ECheckPaymentInformation
    {
        public Int32 AccountType { get; set; }//BankAccount Type 1 = Checking, 2 = Savings ' Me.Quote.EFT_BankAccountTypeId
        public string AccountNumber { get; set; }//Me.Quote.EFT_BankAccountNumber
        public string RoutingNumber { get; set; }//Me.Quote.EFT_BankRoutingNumber
        public Enums.BankFileExclusion ExcludeFromBankFile { get; set; } = Enums.BankFileExclusion.Default;
        public bool StaffABABypass { get; set; }
    }

    [System.Serializable]
    public class EFTInformation
    {
        public int AgencyEFTAccountId { get; set; }
        public int LegacyUserId { get; set; }
        public int LegacyAgencyId { get; set; }
    }

    [System.Serializable]
    public class CreditCardPaymentInformation
    {
        public string CardNumber { get; set; }
        public Int32 CardExpireMonth { get; set; }
        public Int32 CardExpireYear { get; set; }
        public string NameOnCard { get; set; }
        public string ZIPCode { get; set; } //7/19/2020 note: required by Fiserv when not using their iframe
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SecurityCode { get; set; } //added 7/19/2020; required by Fiserv when not using their iframe
    }

    [System.Serializable]
    public class PostPaymentInfo
    {
        public int PaymentId { get; set; }
        public int UserId { get; set; }
        public int EcheckAfterHoursId { get; set; }
        public string ConfirmationNumber { get; set; }
        public int EcheckProcessedFlagId { get; set; }
    }

    [System.Serializable]
    public class PaymentSettings
    {
        /// <summary>
        /// Used to try to protect users from making duplicate payments back to back on accident.
        /// </summary>
        public bool CheckForImmediateDuplicatePayments { get; set; } = false;
        /// <summary>
        /// How much time do we require to pass before not assuming this may be an immediate duplicate payment. Defaults to 120 seconds.
        /// </summary>
        public int ImmediateDuplicatePaymentTimeframeInSeconds { get; set; } = 120;
        /// <summary>
        /// For a payment through to Diamond even if it is technically after hours.
        /// </summary>
        public bool IgnoreAfterHoursPostingRules { get; set; } = false;
        /// <summary>
        /// Can be used to force a payment in certain situations, like for staff.
        /// </summary>
        public bool BypassValidation { get; set; } = false;
        public bool Echeck_NSF_DoCheck { get; set; } = false;
        public int Echeck_NSF_ErrorQuantity { get; set; } = 2;
        public int Echeck_NSF_PeriodInDays { get; set; } = 90;
        public bool Echeck_DuplicatePayments_DoCheck { get; set; } = false;
        public int Echeck_DuplicatePayments_StartingErrorQuantityByPolicyNumber { get; set; } = 3;
        public int Echeck_DuplicatePayments_StartingErrorQuantityByPolicyID { get; set; } = 2;
        public int Echeck_DuplicatePayments_StartingErrorQuantityByPolicyIDWithSameBankInfo { get; set; } = 2;
        public int Echeck_DuplicatePayments_PeriodInHours { get; set; } = 24;
    }

    [System.Serializable]
    public class FiservProperties
    {
        //added 5/29/2020 for Fiserv
        public string AuthToken { get; set; } //obtained for use w/ iframe; not required if not using iframe; would only be used for Card payments (not Bank)
        public string SessionToken { get; set; } //obtained for use w/ iframe; not required if not using iframe; would only be used for Card payments (not Bank)
        public int SessionId { get; set; } //obtained while getting tokens used w/ iframe; not required if not using iframe; would only be used for Card payments (not Bank)
        public string FundingAccountToken { get; set; } //obtained from iframe; not required if not using iframe; would only be used for Card payments (not Bank)
        public bool AddToWallet { get; set; } //set to True to save payment information as new wallet item; will use EmailAddress property for walletItem Create email address, which will be used for Wallet email if it's the 1st walletItem
        public string WalletItemNickname { get; set; } //required when AddToWallet is True
        public string FundingMethod { get; set; }
        public string CCTypeText { get; set; }
        //added 6/29/2020 for Fiserv
        public string IframeResponse { get; set; } //obtained in response back from iframe (includes fundingMethod, which we will use); not required if not using iframe; would only be used for Card payments (not Bank)
    }

    //[System.Serializable]
    //public class RecurringCreditCardInformation
    //{
    //    public string CardNumber { get; set; }
    //    public Int32 CardExpireMonth { get; set; }
    //    public Int32 CardExpireYear { get; set; }
    //    public Int32 DeductionDay { get; set; }
    //    public string EmailAddress { get; set; }
    //    public string ZipCode { get; set; } //added 7/19/2020; required by Fiserv when not using their iframe
    //    public string SecurityCode { get; set; } //added 7/19/2020; required by Fiserv when not using their iframe

    //    //added 7/19/2020 for Fiserv
    //    public string Fiserv_AuthToken { get; set; } //obtained for use w/ iframe; not required if not using iframe
    //    public string Fiserv_SessionToken { get; set; } //obtained for use w/ iframe; not required if not using iframe
    //    public int Fiserv_SessionId { get; set; } //obtained while getting tokens used w/ iframe; not required if not using iframe
    //    public string Fiserv_FundingAccountToken { get; set; } //obtained from iframe; not required if not using iframe
    //    public string Fiserv_IframeResponse { get; set; } //obtained in response back from iframe (includes fundingMethod, which we will use); not required if not using iframe        
    //    public int Fiserv_WalletItemId { get; set; } //added 7/27/2020; generated by IFM; corresponds to FundingAccountToken; will need to set when switching to a different wallet item

    //    public RecurringCreditCardInformation()
    //    {
    //    }

    //    public RecurringCreditCardInformation(CreditCardPaymentInformation creditCardPaymentInfo)
    //    {
    //        this.CardExpireMonth = creditCardPaymentInfo.CardExpireMonth;
    //        this.CardExpireYear = creditCardPaymentInfo.CardExpireYear;
    //        this.CardNumber = creditCardPaymentInfo.CardNumber;
    //    }
    //}

    //[System.Serializable]
    //public class RecurringEftInformation : ECheckPaymentInformation
    //{
    //    public Int32 DeductionDay { get; set; }

    //    public string EmailAddress { get; set; }

    //    public RecurringEftInformation()
    //    {
    //    }

    //    public RecurringEftInformation(ECheckPaymentInformation eCheckPaymentInfo)
    //    {
    //        this.AccountNumber = eCheckPaymentInfo.AccountNumber;
    //        this.AccountType = eCheckPaymentInfo.AccountType;
    //        this.RoutingNumber = eCheckPaymentInfo.RoutingNumber;
    //    }
    //}

    [System.Serializable]
    public class WalletItem
    {
        public int WalletItemId { get; set; } //generated by IFM after Create
        public string Nickname { get; set; } //required; Create and Update
        public string FundingAccountToken { get; set; } //generated by Fiserv (their key)
        public string FundingMethod { get; set; } //ACH, ACCEL, STAR, VISA, DISCOVER, etc.; not set on Create or Update; should come back from Fiserv
        public string FundingCategory { get; set; } //Check, Debit, Credit; not set on Create or Update; should come back from Fiserv
        public string FundingAccountLastFourDigits { get; set; } //not set on Create or Update; should come back from Fiserv
        public string FundingAccountType { get; set; } //Personal, Business; for Bank info; Create and Update
        public string BankAccountType { get; set; } //Checking, Saving; for Bank info; Create and Update
        public string ExpirationMonth { get; set; } //won't ever come back but used for Update; for Card info; would also be used for Create if we're supporting wallet functionality but not using Fiserv's iframe
        public string ExpirationYear { get; set; } //won't ever come back but used for Update; for Card info; would also be used for Create if we're supporting wallet functionality but not using Fiserv's iframe
        public string ZipCode { get; set; } //won't ever come back but used for Update; for Card info
        public string CreditCardNumber { get; set; } //won't ever come back; would only be used if we're supporting wallet functionality but not using Fiserv's iframe; for Create only; cannot be updated
        public string SecurityCode { get; set; } //won't ever come back; would only be used if we're supporting wallet functionality but not using Fiserv's iframe; for Create only; cannot be updated
        public string FirstName { get; set; } //for Bank info; Create and Update
        public string LastName { get; set; } //for Bank info; Create and Update
        public string RoutingNumber { get; set; } //won't ever come back; for Bank info; for Create only; cannot be updated
        public string CheckAccountNumber { get; set; } //won't ever come back; for Bank info; for Create only; cannot be updated
        public string BankName { get; set; } //for Bank info; not set on Create or Update; may come back from Fiserv
        public string KeyIdentifier { get; set; } //Fiserv.Session.UserId
        public int WalletId { get; set; } //generated by IFM at Wallet Creation
        public string EmailAddress { get; set; } //would only be set at Create; will be used for Wallet EmailAddress if it's the 1st WalletItem in the Wallet; can only be updated from Wallet
        public List<string> ActiveRccPolicies { get; set; } //will contain the policy #s for any active RCC accounts tied to the walletItem; cannot delete the walletItem if any policies are tied to it
        public List<string> ActiveAndUnprocessedScheduledPaymentPolicies { get; set; } //will contain the policy #s for any active unprocessed scheduled payments tied to the walletItem; cannot delete the walletItem if any policies are tied to it
    }
}