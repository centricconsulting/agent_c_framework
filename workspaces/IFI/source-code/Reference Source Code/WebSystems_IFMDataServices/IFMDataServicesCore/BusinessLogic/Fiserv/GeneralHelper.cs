using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFM.DataServicesCore.CommonObjects.Fiserv;
using cc = IFM_CreditCardProcessing;
using APIResponse = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServicesCore.BusinessLogic.Fiserv
{
    public class GeneralHelper : ModelBase
    {
        //public static cc.PaymentSettingsObject DefaultPaymentSettings()
        //{
        //    return cc.Common.DefaultPaymentSettings();
        //}
        public static PaymentSettings DefaultPaymentSettings(IFM_CreditCardProcessing.Enums.PaymentInterface pmtInterface) //updated 8/19/2020 for PaymentInterface
        {
            PaymentSettings s = null;

            cc.PaymentSettingsObject settings = cc.Common.DefaultPaymentSettings_OptionalPaymentInterface(pmtInterface); //updated 8/19/2020 for PaymentInterface from original method (DefaultPaymentSettings)
            if (settings != null)
            {
                s = new PaymentSettings();
                s.AllowScheduledPayments = settings.allowScheduledPayments;
                s.AllowSplitPayments = settings.allowSplitPayments; //added 6/30/2020; defaulting for now until the CC library is rebuilt; okay as-of 7/1/2020
                s.AllowUseOfWalletItems = settings.allowUseOfWalletItems;
                s.FiservApiKey = settings.fiservApiKey;
                s.FiservBankIframeUrl = settings.fiservBankIframeUrl;
                s.FiservCardIframeUrl = settings.fiservCardIframeUrl;
                s.FiservUseIframeForCreditCardInfo = settings.fiservUseIframeForCreditCardInfo;
                s.FiservUseIframeForRecurringCreditCardInfo = settings.fiservUseIframeForRecurringCreditCardInfo;
                s.NumberOfPaymentMethodsToAllowOnSplitPayments = settings.numberOfPaymentMethodsToAllowOnSplitPayments; //added 6/30/2020; defaulting for now until the CC library is rebuilt; okay as-of 7/1/2020
                s.CollectSecurityCodeForCreditCardPayments = settings.collectSecurityCodeForCreditCardPayments; //added 7/19/2020
                s.CollectSecurityCodeForRecurringCreditCardPayments = settings.collectSecurityCodeForRecurringCreditCardPayments; //added 7/19/2020
                s.CollectZipCodeForCreditCardPayments = settings.collectZipCodeForCreditCardPayments; //added 7/19/2020
                s.CollectZipCodeForRecurringCreditCardPayments = settings.collectZipCodeForRecurringCreditCardPayments; //added 7/19/2020
                s.SendEcheckPaymentConfirmationEmails = settings.sendEcheckPaymentConfirmationEmails; //added 8/18/2020
                s.SendCreditCardPaymentConfirmationEmails = settings.sendCreditCardPaymentConfirmationEmails; //added 8/19/2020
                s.SendWalletPaymentConfirmationEmails = settings.sendCreditCardPaymentConfirmationEmails; //added 8/19/2020
                s.NumberOfPoliciesToAllowOnSplitPayments = settings.numberOfPoliciesToAllowOnSplitPayments; //added 8/19/2020
                s.UseTemporaryWalletItemToPayMultiplePoliciesWithSameCard = settings.useTemporaryWalletItemToPayMultiplePoliciesWithSameCard; //added 9/17/2020
                s.EmailAddressToUseForTemporaryCardWalletItemUsedToPayMultiplePolicies = settings.emailAddressToUseForTemporaryCardWalletItemUsedToPayMultiplePolicies; //added 9/17/2020
                s.MaximumVendorPaymentAmount = settings.maximumVendorPaymentAmount;
                s.CalculateFeeAmountOnCreditCardPayments = settings.calculateFeeAmountOnCreditCardPayments; //added 9/25/2024
                s.CalculateFeeAmountOnCreditCardPayments_Date = settings.calculateFeeAmountOnCreditCardPayments_Date; //added 9/25/2024
                s.RestrictCreditCardsForRecurringPayments_Date = global::IFM.DataServicesCore.BusinessLogic.AppConfig.RestrictCreditCardsForRecurringPayments_Date; //added 11/05/2024
            }

            return s;
        }
        public global::IFM.DataServices.API.ResponseObjects.Payments.AuthAndSessionTokens TokensForPolicyNumber(string policyNumber, cc.Enums.PaymentInterface pmtInterface = cc.Enums.PaymentInterface.MemberPortalSite)
        {
            //if (System.Enum.IsDefined(GetType(cc.Enums.PaymentInterface), pmtInterface) == false)
            if (System.Enum.IsDefined(typeof(cc.Enums.PaymentInterface), pmtInterface) == false || pmtInterface == cc.Enums.PaymentInterface.None)
            {
                pmtInterface = cc.Enums.PaymentInterface.MemberPortalSite;
            }

            IFM_FiservTransactionObjects.FiservTransactionInfo tranInfo = new IFM_FiservTransactionObjects.FiservTransactionInfo();
            tranInfo.pmtInterface = pmtInterface;
            tranInfo.policyNumber = policyNumber;

            return TokensForTranInfo(tranInfo);
        }
        public global::IFM.DataServices.API.ResponseObjects.Payments.AuthAndSessionTokens TokensForEmailAddress(string emailAddress, cc.Enums.PaymentInterface pmtInterface = cc.Enums.PaymentInterface.MemberPortalSite)
        {
            if (System.Enum.IsDefined(typeof(cc.Enums.PaymentInterface), pmtInterface) == false || pmtInterface == cc.Enums.PaymentInterface.None)
            {
                pmtInterface = cc.Enums.PaymentInterface.MemberPortalSite;
            }

            IFM_FiservTransactionObjects.FiservTransactionInfo tranInfo = new IFM_FiservTransactionObjects.FiservTransactionInfo();
            tranInfo.pmtInterface = pmtInterface;
            //tranInfo.memberIdentifier = emailAddress;
            tranInfo.memberIdentifier = cc.Common.AdjustedEmailAddressForFiservApiCall(emailAddress);

            return TokensForTranInfo(tranInfo);
        }
        public global::IFM.DataServices.API.ResponseObjects.Payments.AuthAndSessionTokens TokensForPolicyNumberAndEmailAddress(string policyNumber, string emailAddress, cc.Enums.PaymentInterface pmtInterface = cc.Enums.PaymentInterface.MemberPortalSite)
        {
            if (System.Enum.IsDefined(typeof(cc.Enums.PaymentInterface), pmtInterface) == false || pmtInterface == cc.Enums.PaymentInterface.None)
            {
                pmtInterface = cc.Enums.PaymentInterface.MemberPortalSite;
            }

            IFM_FiservTransactionObjects.FiservTransactionInfo tranInfo = new IFM_FiservTransactionObjects.FiservTransactionInfo();
            tranInfo.pmtInterface = pmtInterface;
            tranInfo.policyNumber = policyNumber;
            //tranInfo.memberIdentifier = emailAddress;
            tranInfo.memberIdentifier = cc.Common.AdjustedEmailAddressForFiservApiCall(emailAddress);

            return TokensForTranInfo(tranInfo);
        }
        private global::IFM.DataServices.API.ResponseObjects.Payments.AuthAndSessionTokens TokensForTranInfo(IFM_FiservTransactionObjects.FiservTransactionInfo tranInfo)
        {
            global::IFM.DataServices.API.ResponseObjects.Payments.AuthAndSessionTokens t = null;

            IFM_FiservTransactionObjects.FiservAuthAndSessionTokens tokens = creditCardMethods.Fiserv_AuthAndSessionTokensForTransactionInfo(tranInfo);
            if (tokens != null)
            {
                t = new global::IFM.DataServices.API.ResponseObjects.Payments.AuthAndSessionTokens();
                t.AuthToken = tokens.authToken;
                if (tokens.sessionObject != null)
                {
                    t.SessionId = tokens.sessionObject.sessionId;
                    t.SessionToken = tokens.sessionObject.sessionToken;
                }
            }

            return t;
        }

        public static List<string> ListForEmailAndPolicyNumbers(string emailAddress, List<string> policyNumbers)
        {
            List<string> newList = null;

            if (string.IsNullOrWhiteSpace(emailAddress) == false)
            {
                cc.Common.AddStringToList(cc.Common.AdjustedEmailAddressForFiservApiCall(emailAddress), ref newList);
            }

            cc.Common.AddStringsToList(policyNumbers, ref newList);

            return newList;
        }

        //added 9/26/2024
        public APIResponse.Common.ServiceResult<APIResponse.Fiserv.FiservCalculateFeeResult> CalculateFee(CommonObjects.Fiserv.CalculateFeeBody calcFeeBody)
        {
            //walletItemInfo
            var sr = new APIResponse.Common.ServiceResult<APIResponse.Fiserv.FiservCalculateFeeResult>();
            if (calcFeeBody != null)
            {
                string fundingAccountToken = calcFeeBody.Fiserv_FundingAccountToken;
                bool hasFundingAcctToken = !string.IsNullOrWhiteSpace(calcFeeBody.Fiserv_FundingAccountToken);
                bool hasCardInfo = !(string.IsNullOrWhiteSpace(calcFeeBody.CreditCardNumber) || string.IsNullOrWhiteSpace(calcFeeBody.ExpirationMonth) || string.IsNullOrWhiteSpace(calcFeeBody.ExpirationYear));
                bool hasBankInfo = !(string.IsNullOrWhiteSpace(calcFeeBody.RoutingNumber) || string.IsNullOrWhiteSpace(calcFeeBody.CheckAccountNumber));
                if (hasFundingAcctToken || hasCardInfo || hasBankInfo)
                {
                    cc.Fiserv_CalculateFeeBodyRequest calcFeeBodyRequest = new cc.Fiserv_CalculateFeeBodyRequest();
                    cc.Fiserv_FundingAccountRequestBody fundingAccountRequestBody = null;
                    //insertWalletItemBodyRequest.nickName = addBody.Item.Nickname;
                    //updated 8/18/2020 to handle for max length
                    //insertWalletItemBodyRequest.nickName = IFM_CreditCardProcessing.Common.Fiserv_Formatted_WalletItem_Nickname(addBody.Item.Nickname);
                    //insertWalletItemBodyRequest.emailAddress = addBody.Item.EmailAddress; //added 11/12/2020
                    calcFeeBodyRequest.fundingAccountToken = calcFeeBody.Fiserv_FundingAccountToken;
                    calcFeeBodyRequest.paymentAmount = Convert.ToDouble(calcFeeBody.PaymentAmount);
                    if (IFM_CreditCardProcessing.Common.Fiserv_ImmediatePaymentRequestBody_AddPolicyNumberToPaymentCustomData())
                    {
                        if (string.IsNullOrWhiteSpace(calcFeeBody.PolicyNumber) == false)
                        {
                            calcFeeBodyRequest.paymentCustomList = new cc.Fiserv_PaymentCustomList();
                            IFM_CreditCardProcessing.Fiserv_PaymentCustomData customData = new cc.Fiserv_PaymentCustomData();
                            customData.customDataName = "PolicyNumber";
                            customData.customDataValue = calcFeeBody.PolicyNumber;
                            calcFeeBodyRequest.paymentCustomList.Add(customData);
                        }
                    }
                    calcFeeBodyRequest.paymentModel = "Immediate";
                    if (hasCardInfo || hasBankInfo)
                    {
                        fundingAccountRequestBody = new cc.Fiserv_FundingAccountRequestBody();
                        if (hasCardInfo)
                        {
                            fundingAccountRequestBody.cardDetail = new cc.Fiserv_CardDetail();
                            fundingAccountRequestBody.cardDetail.cardNumber = calcFeeBody.CreditCardNumber;
                            CommonHelperClass chc = new CommonHelperClass();
                            fundingAccountRequestBody.cardDetail.expirationDate = cc.Common.FiservExpirationDateString(chc.IntegerForString(calcFeeBody.ExpirationMonth), chc.IntegerForString(calcFeeBody.ExpirationYear));
                            fundingAccountRequestBody.cardDetail.firstName = calcFeeBody.FirstName;
                            fundingAccountRequestBody.cardDetail.lastName = calcFeeBody.LastName;
                            fundingAccountRequestBody.cardDetail.nameOnCard = cc.Common.Append(calcFeeBody.FirstName, calcFeeBody.LastName, appendText: " ");
                            fundingAccountRequestBody.cardDetail.securityCode = calcFeeBody.SecurityCode;
                            fundingAccountRequestBody.cardDetail.zipCode = calcFeeBody.ZipCode;
                        }
                        else if (hasBankInfo)
                        {
                            fundingAccountRequestBody.checkDetail = new cc.Fiserv_CheckDetail();
                            fundingAccountRequestBody.checkDetail.checkAccountNumber = calcFeeBody.CheckAccountNumber;
                            if (string.IsNullOrWhiteSpace(calcFeeBody.FundingAccountType) == false && calcFeeBody.FundingAccountType == "Business")
                            {
                                fundingAccountRequestBody.checkDetail.checkAccountType_Enum = cc.Enums.Fiserv_CheckAccountType.Business;
                            }
                            else
                            {
                                fundingAccountRequestBody.checkDetail.checkAccountType_Enum = cc.Enums.Fiserv_CheckAccountType.Personal;
                            }
                            fundingAccountRequestBody.checkDetail.firstName = calcFeeBody.FirstName;
                            if (string.IsNullOrWhiteSpace(calcFeeBody.BankAccountType) == false && calcFeeBody.BankAccountType == "Saving")
                            {
                                fundingAccountRequestBody.checkDetail.fundingAccountType_Enum = cc.Enums.Fiserv_FundingAccountType.Saving;
                            }
                            else
                            {
                                fundingAccountRequestBody.checkDetail.fundingAccountType_Enum = cc.Enums.Fiserv_FundingAccountType.Checking;
                            }
                            fundingAccountRequestBody.checkDetail.lastName = calcFeeBody.LastName;
                            fundingAccountRequestBody.checkDetail.routingNumber = calcFeeBody.RoutingNumber;
                        }
                    }

                    string authToken = calcFeeBody.Fiserv_AuthToken;
                    string sessionToken = calcFeeBody.Fiserv_SessionToken;
                    int sessionId = calcFeeBody.Fiserv_SessionId;
                    cc.Enums.PaymentInterface pmtInterface = (IFM_CreditCardProcessing.Enums.PaymentInterface)(int)calcFeeBody.PaymentInterface;
                    if (Enum.IsDefined(typeof(IFM_CreditCardProcessing.Enums.PaymentInterface), pmtInterface) == false || pmtInterface == cc.Enums.PaymentInterface.None)
                    {
                        if (calcFeeBody.IsFromOneView == true)
                        {
                            pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.OneView;
                        }
                        else
                        {
                            pmtInterface = cc.Enums.PaymentInterface.MemberPortalSite;
                        }
                    }
                    cc.UserInfo user = new cc.UserInfo();
                    user.Username = calcFeeBody.UserName;
                    user.UserType = calcFeeBody.UserType;
                    IFM_FiservTransactionObjects.FiservTransactionInfo tranInfo = new IFM_FiservTransactionObjects.FiservTransactionInfo();
                    tranInfo.pmtInterface = pmtInterface;
                    tranInfo.policyNumber = calcFeeBody.PolicyNumber;
                    tranInfo.accountBillNumber = calcFeeBody.AccountBillNumber;
                    tranInfo.memberIdentifier = IFM_CreditCardProcessing.Common.AdjustedEmailAddressForFiservApiCall(calcFeeBody.MemberIdentifier);
                    tranInfo.user = user;
                    cc.Fiserv_SessionBodyRequest sessionBodyRequest = IFM_CreditCardProcessing.Common.Fiserv_SessionBodyRequest_ForTransactionInfo(tranInfo);

                    string fiservResponse = "";
                    IFM_FiservResponseObjects.FiservCalculateFeeResults calcFeeResults = null;
                    IFM_CreditCardProcessing.Enums.Fiserv_JsonTransaction_ErrorType errorType = cc.Enums.Fiserv_JsonTransaction_ErrorType.None;
                    string errorMsg = "";
                    string exceptionErrorMsg = "";
                    cc.FiservErrorObject errorObject = null;

                    //sr.ResponseData.FeeAmount = creditCardMethods.Fiserv_FeeAmount(authToken: ref authToken, sessionBodyRequest: sessionBodyRequest, sessionToken: ref sessionToken, fiservSessionId: ref sessionId, fundingAcctBodyRequest: fundingAccountRequestBody, fundingAccountToken: ref fundingAccountToken, calcFeeBodyRequest: calcFeeBodyRequest, calcFeeResponse: ref fiservResponse, returnCalcFeeResultsObject: true, calcFeeResults: ref calcFeeResults, fiservErrorType: ref errorType, errorMsg: ref errorMsg, exceptionErrorMsg: ref exceptionErrorMsg, populateErrorObject: true, errorObject: ref errorObject);

                    //if (string.IsNullOrWhiteSpace(sr.ResponseData.FeeAmount))
                    //{
                    //    sr.ResponseData.Success = false;
                    //}
                    //else
                    //{
                    //    sr.ResponseData.Success = true;
                    //}
                    //updated 10/17/2024 to use new method
                    IFM_FiservTransactionObjects.FiservCalculateFeeTransaction cfTran = new IFM_FiservTransactionObjects.FiservCalculateFeeTransaction();
                    cfTran.authToken = authToken;
                    cfTran.calcFeeBodyRequest = calcFeeBodyRequest;
                    cfTran.fundingAccountToken = fundingAccountToken;
                    cfTran.fundingAcctBodyRequest = fundingAccountRequestBody;
                    cfTran.returnCalcFeeResultsObject = true;
                    cfTran.sessionBodyRequest = sessionBodyRequest;
                    cfTran.sessionObject = new IFM_FiservTransactionObjects.FiservSessionObject();
                    cfTran.sessionObject.sessionToken = sessionToken;
                    cfTran.sessionObject.sessionId = sessionId;
                    cfTran.pmtInterface = pmtInterface;

                    sr.ResponseData.Success = creditCardMethods.SuccessfullyProcessedFiservCalculateFeeTransaction(ref cfTran);

                    //now get return values                    
                    authToken = cfTran.authToken;
                    calcFeeBodyRequest = cfTran.calcFeeBodyRequest;
                    fiservResponse = cfTran.calcFeeResponse;
                    calcFeeResults = cfTran.calcFeeResults;
                    errorObject = cfTran.errorObject;
                    if (errorObject != null)
                    {
                        errorMsg = errorObject.ErrorMessage;
                        exceptionErrorMsg = errorObject.ExceptionToString;
                        errorType = errorObject.FiservErrorType;
                    }
                    fundingAccountToken = cfTran.fundingAccountToken;
                    sessionId = cfTran.sessionObject.sessionId;
                    sessionToken = cfTran.sessionObject.sessionToken;

                    sr.ResponseData.FeeAmount = cfTran.feeAmount;
                    sr.ResponseData.BackupLogicUsedToCalculateFee = cfTran.backupLogicUsedToCalculateFee;
                    sr.ResponseData.OriginalFeeAmountFromFiserv = cfTran.originalFeeAmountFromFiserv;

                    //added 10/16/2024
                    sr.ResponseData.Fiserv_AuthToken = authToken;
                    sr.ResponseData.Fiserv_FundingAccountToken = fundingAccountToken;
                    sr.ResponseData.Fiserv_SessionId = sessionId;
                    sr.ResponseData.Fiserv_SessionToken = sessionToken;


                if (sr.ResponseData.Success == false)
                    {
                        string responseMessage = IFM_CreditCardProcessing.Common.FiservApiResponseStatusMessagesAsString(calcFeeResults);
                        string errorMessage = "problem calculating fee";
                        //use something specific if possible
                        if (string.IsNullOrWhiteSpace(responseMessage) == false)
                        {
                            errorMessage = errorMessage + ": " + responseMessage;
                        }
                        else if (errorObject != null && string.IsNullOrWhiteSpace(errorObject.ErrorMessage) == false)
                        {
                            errorMessage = errorMessage + ": " + errorObject.ErrorMessage;
                        }
                        else if (string.IsNullOrWhiteSpace(errorMsg) == false)
                        {
                            errorMessage = errorMessage + ": " + errorMsg;
                        }
                        sr.Messages.CreateErrorMessage(errorMessage);
                    }
                
                else //added 10/16/2024
                {
                    string responseMessage = IFM_CreditCardProcessing.Common.FiservApiResponseStatusMessagesAsString(calcFeeResults);
                    if (string.IsNullOrWhiteSpace(responseMessage) == true)
                    {
                        if (errorObject != null && string.IsNullOrWhiteSpace(errorObject.ErrorMessage) == false)
                        {
                            responseMessage = errorObject.ErrorMessage;
                        }
                        else if (string.IsNullOrWhiteSpace(errorMsg) == false)
                        {
                            responseMessage = errorMsg;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(responseMessage) == false)
                    {
                        sr.Messages.CreateGeneralMessage(responseMessage);
                    }
                }
            }

            else
                {
                    sr.Messages.CreateErrorMessage("Funding Account Token or Complete Card/Bank Information required.");
                }
            }

            return sr;
        }

    }
}
