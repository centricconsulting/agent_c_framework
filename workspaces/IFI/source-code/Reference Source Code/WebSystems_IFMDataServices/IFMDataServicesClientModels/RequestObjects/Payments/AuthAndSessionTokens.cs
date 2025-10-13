using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IFM.DataServices.API;

namespace IFM.DataServices.API.RequestObjects.Payments
{
    [System.Serializable]
    public class BankNameLookup : BaseRequest
    {
        public string PolicyNumber { get; set; }
        public Enums.PaymentInterface PaymentInterface { get; set; }
        public string EmailAddress { get; set; }
        public string Username { get; set; }

        public BankNameLookup(string APIAddress) : base(APIAddress)
        {
            API_Path = "Fiserv";
        }

        public ResponseObjects.Common.ServiceResult<ResponseObjects.Payments.AuthAndSessionTokens> GetAuthAndSessionTokensForPolicyNumber()
        {
            TestRequiredVariable(PolicyNumber, nameof(PolicyNumber));
            TestRequiredVariable(PaymentInterface, (int)API.Enums.PaymentInterface.None, nameof(PaymentInterface));

            API_Endpoint = $"AuthAndSessionTokensForPolicyNumber_IFM/{this.PolicyNumber}/{this.PaymentInterface}";
            return Get<ResponseObjects.Common.ServiceResult<ResponseObjects.Payments.AuthAndSessionTokens>>();
        }

        public ResponseObjects.Common.ServiceResult<ResponseObjects.Payments.AuthAndSessionTokens> GetAuthAndSessionTokensForPolicyNumber(string policyNumber, API.Enums.PaymentInterface paymentInterface)
        {
            this.PolicyNumber = policyNumber;
            this.PaymentInterface = paymentInterface;
            return GetAuthAndSessionTokensForPolicyNumber();
        }

        public ResponseObjects.Common.ServiceResult<ResponseObjects.Payments.AuthAndSessionTokens> AuthAndSessionTokensForEmailAddress()
        {
            TestRequiredVariable(EmailAddress, nameof(EmailAddress));
            TestRequiredVariable(PaymentInterface, (int)API.Enums.PaymentInterface.None, nameof(PaymentInterface));

            API_Endpoint = $"AuthAndSessionTokensForEmailAddress_IFM/{this.EmailAddress}/{this.PaymentInterface}";
            return Get<ResponseObjects.Common.ServiceResult<ResponseObjects.Payments.AuthAndSessionTokens>>();
        }

        public ResponseObjects.Common.ServiceResult<ResponseObjects.Payments.AuthAndSessionTokens> AuthAndSessionTokensForEmailAddress(string emailAddress, API.Enums.PaymentInterface paymentInterface)
        {
            this.EmailAddress = emailAddress;
            this.PaymentInterface = paymentInterface;
            return AuthAndSessionTokensForEmailAddress();
        }

        public ResponseObjects.Common.ServiceResult<ResponseObjects.Payments.AuthAndSessionTokens> AuthAndSessionTokensForPolicyNumberAndEmailAddress()
        {
            TestRequiredVariable(PolicyNumber, nameof(PolicyNumber));
            TestRequiredVariable(EmailAddress, nameof(EmailAddress));
            TestRequiredVariable(PaymentInterface, (int)API.Enums.PaymentInterface.None, nameof(PaymentInterface));

            API_Endpoint = $"AuthAndSessionTokensForPolicyNumberAndEmailAddress_IFM/{this.PolicyNumber}/{this.EmailAddress}/{this.PaymentInterface}";
            return Get<ResponseObjects.Common.ServiceResult<ResponseObjects.Payments.AuthAndSessionTokens>>();
        }

        public ResponseObjects.Common.ServiceResult<ResponseObjects.Payments.AuthAndSessionTokens> AuthAndSessionTokensForPolicyNumberAndEmailAddress(string policyNumber, string emailAddress, API.Enums.PaymentInterface paymentInterface)
        {
            this.PolicyNumber = policyNumber;
            this.EmailAddress = emailAddress;
            this.PaymentInterface = paymentInterface;

            return AuthAndSessionTokensForPolicyNumberAndEmailAddress();
        }

        public ResponseObjects.Common.ServiceResult<ResponseObjects.Payments.AuthAndSessionTokens> AuthAndSessionTokensForUsername()
        {
            TestRequiredVariable(Username, nameof(Username));
            TestRequiredVariable(PaymentInterface, (int)API.Enums.PaymentInterface.None, nameof(PaymentInterface));

            API_Endpoint = $"AuthAndSessionTokensForUsername_IFM/{this.Username}/{this.PaymentInterface}";
            return Get<ResponseObjects.Common.ServiceResult<ResponseObjects.Payments.AuthAndSessionTokens>>();
        }

        public ResponseObjects.Common.ServiceResult<ResponseObjects.Payments.AuthAndSessionTokens> AuthAndSessionTokensForUsername(string username, API.Enums.PaymentInterface paymentInterface)
        {
            this.Username = username;
            this.PaymentInterface = this.PaymentInterface;

            return AuthAndSessionTokensForUsername();
        }

        public ResponseObjects.Common.ServiceResult<ResponseObjects.Payments.AuthAndSessionTokens> AuthAndSessionTokensForPolicyNumberAndUsername()
        {
            TestRequiredVariable(PolicyNumber, nameof(PolicyNumber));
            TestRequiredVariable(Username, nameof(Username));
            TestRequiredVariable(PaymentInterface, (int)API.Enums.PaymentInterface.None, nameof(PaymentInterface));

            API_Endpoint = $"AuthAndSessionTokensForPolicyNumberAndUsername_IFM/{this.EmailAddress}/{this.PaymentInterface}";
            return Get<ResponseObjects.Common.ServiceResult<ResponseObjects.Payments.AuthAndSessionTokens>>();
        }

        public ResponseObjects.Common.ServiceResult<ResponseObjects.Payments.AuthAndSessionTokens> AuthAndSessionTokensForPolicyNumberAndUsername(string policyNumber, string username, API.Enums.PaymentInterface paymentInterface)
        {
            this.PolicyNumber = policyNumber;
            this.Username = username;
            this.PaymentInterface = paymentInterface;

            return AuthAndSessionTokensForPolicyNumberAndUsername();
        }
    }
}