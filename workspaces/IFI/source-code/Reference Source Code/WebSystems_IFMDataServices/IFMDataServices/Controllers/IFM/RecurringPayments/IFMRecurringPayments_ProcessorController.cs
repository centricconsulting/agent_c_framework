using IFM.DataServicesCore.CommonObjects.Fiserv;
using IFM_FiservDatabaseObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Controllers.IFM.RecurringPayments
{
    [RoutePrefix("IFM/RecurringPayments/Processor")]
    public class IFMRecurringPayments_ProcessorController : BaseController
    {


        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("payplan")]
        public JsonResult SetRecurringPaymentData(global::IFM.DataServicesCore.CommonObjects.Payments.PayPlanData PayPlanData)
        {
            APIResponses.Payments.PayplanChangedResult sr = new APIResponses.Payments.PayplanChangedResult();
            
            if (PayPlanData != null)
            {
                Validation.PayPlanDataValidator val = new Validation.PayPlanDataValidator();
                var valResult = val.Validate(PayPlanData);
                if (valResult.IsValid)
                {
                    //added 3/22/2018
                    int prevPayPlanId = DataServicesCore.BusinessLogic.Payments.PayPlanHelper.CurrentPayPlanId(PayPlanData.PolicyId, PayPlanData.ImageNumber);

                    sr = global::IFM.DataServicesCore.BusinessLogic.Payments.PayPlanHelper.SetPayPlanNow(PayPlanData, prevPayPlanId); //updated 7/30/2020 for prevPayPlanId
                    if (sr.HasErrors == false)
                    {
                        var doc = global::IFM.DataServicesCore.BusinessLogic.Payments.PayPlanHelper.GeneratePayPlanChangeEmail(PayPlanData, prevPayPlanId); //updated 3/26/2018 to include prevPayPlanId
                        if (doc != null)
                            global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(doc);
                    }

                }
                else
                {
                    this.AddFluentErrorsToServiceResult(sr, valResult.Errors);
                }
                //troubleshooting 9/5/2020
                //string exMsg = "";
                //int prevPayPlanId = 0;
                //string strPayPlanData = "";
                ////bool passedValidation = false; //added 9/8/2020
                //try
                //{
                //    Validation.PayPlanDataValidator val = new Validation.PayPlanDataValidator();
                //    var valResult = val.Validate(PayPlanData);
                //    if (valResult.IsValid)
                //    {
                //        //passedValidation = true; //added 9/8/2020
                //        prevPayPlanId = DataServicesCore.BusinessLogic.Payments.PayPlanHelper.CurrentPayPlanId(PayPlanData.PolicyId, PayPlanData.ImageNumber);

                //        sr = global::IFM.DataServicesCore.BusinessLogic.Payments.PayPlanHelper.SetPayPlanNow(PayPlanData, prevPayPlanId);
                //        if (sr.HasErrors == false)
                //        {
                //            var doc = global::IFM.DataServicesCore.BusinessLogic.Payments.PayPlanHelper.GeneratePayPlanChangeEmail(PayPlanData, prevPayPlanId);
                //            if (doc != null)
                //                global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(doc);
                //        }

                //    }
                //    else
                //    {
                //        this.AddFluentErrorsToServiceResult(sr, valResult.Errors);
                //    }
                //}
                //catch (Exception ex)
                //{
                //    exMsg = ex.ToString();
                //}

                //if (string.IsNullOrWhiteSpace(exMsg) == false)
                ////updated 9/8/2020
                ////if (string.IsNullOrWhiteSpace(exMsg) == false || passedValidation == false)
                //{
                //    string emailBody = "";

                //    if (PayPlanData != null)
                //    {
                //        if (PayPlanData.RecurringCreditCardInformation != null)
                //        {
                //            strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "PayPlanData.RecurringCreditCardInformation is something", appendText: "<br />");
                //            if (string.IsNullOrWhiteSpace(PayPlanData.RecurringCreditCardInformation.CardNumber) == false)
                //            {
                //                strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "CardNumber: " + PayPlanData.RecurringCreditCardInformation.CardNumber, appendText: "<br />");
                //                //added 9/8/2020
                //                //int cardNumLength = PayPlanData.RecurringCreditCardInformation.CardNumber.Length;
                //                //bool cardNumLengthIsOkay = false;
                //                //if (PayPlanData.RecurringCreditCardInformation.CardNumber.Length >= 15)
                //                //{
                //                //    cardNumLengthIsOkay = true;
                //                //}
                //                //bool containsMaskChars = false;
                //                //if (PayPlanData.RecurringCreditCardInformation.CardNumber.Contains("*") == true || PayPlanData.RecurringCreditCardInformation.CardNumber.ToUpper().Contains("X") == true)
                //                //{
                //                //    containsMaskChars = true;
                //                //}
                //                //bool hasMaskedCardNumber = false;
                //                //if (PayPlanData.RecurringCreditCardInformation != null)
                //                //{
                //                //    if (string.IsNullOrWhiteSpace(PayPlanData.RecurringCreditCardInformation.CardNumber) == false)
                //                //    {
                //                //        if (PayPlanData.RecurringCreditCardInformation.CardNumber.Length >= 15)
                //                //        {
                //                //            if (PayPlanData.RecurringCreditCardInformation.CardNumber.Contains("*") == true || PayPlanData.RecurringCreditCardInformation.CardNumber.ToUpper().Contains("X") == true)
                //                //            {
                //                //                hasMaskedCardNumber = true;
                //                //            }
                //                //        }
                //                //    }
                //                //}
                //                //strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "CardNumber Length: " + cardNumLength.ToString(), appendText: "<br />");
                //                //strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "CardNumber Length is Okay: " + cardNumLengthIsOkay.ToString(), appendText: "<br />");
                //                //strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "CardNumber contains Masked Chars: " + containsMaskChars.ToString(), appendText: "<br />");
                //                //strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "HasMaskedCardNumber: " + hasMaskedCardNumber.ToString(), appendText: "<br />");
                //            }
                //            strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "CardExpireMonth: " + PayPlanData.RecurringCreditCardInformation.CardExpireMonth.ToString(), appendText: "<br />");
                //            strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "CardExpireYear: " + PayPlanData.RecurringCreditCardInformation.CardExpireYear.ToString(), appendText: "<br />");
                //            strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "DeductionDay: " + PayPlanData.RecurringCreditCardInformation.DeductionDay.ToString(), appendText: "<br />");
                //            if (string.IsNullOrWhiteSpace(PayPlanData.RecurringCreditCardInformation.EmailAddress) == false)
                //            {
                //                strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "EmailAddress: " + PayPlanData.RecurringCreditCardInformation.EmailAddress, appendText: "<br />");
                //            }
                //            if (string.IsNullOrWhiteSpace(PayPlanData.RecurringCreditCardInformation.SecurityCode) == false)
                //            {
                //                strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "SecurityCode: " + PayPlanData.RecurringCreditCardInformation.SecurityCode, appendText: "<br />");
                //            }
                //            if (string.IsNullOrWhiteSpace(PayPlanData.RecurringCreditCardInformation.ZipCode) == false)
                //            {
                //                strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "ZipCode: " + PayPlanData.RecurringCreditCardInformation.ZipCode, appendText: "<br />");
                //            }
                //            if (string.IsNullOrWhiteSpace(PayPlanData.RecurringCreditCardInformation.Fiserv_AuthToken) == false)
                //            {
                //                strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "Fiserv_AuthToken: " + PayPlanData.RecurringCreditCardInformation.Fiserv_AuthToken, appendText: "<br />");
                //            }
                //            if (string.IsNullOrWhiteSpace(PayPlanData.RecurringCreditCardInformation.Fiserv_FundingAccountToken) == false)
                //            {
                //                strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "Fiserv_FundingAccountToken: " + PayPlanData.RecurringCreditCardInformation.Fiserv_FundingAccountToken, appendText: "<br />");
                //            }
                //            if (string.IsNullOrWhiteSpace(PayPlanData.RecurringCreditCardInformation.Fiserv_IframeResponse) == false)
                //            {
                //                strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "Fiserv_IframeResponse: " + PayPlanData.RecurringCreditCardInformation.Fiserv_IframeResponse, appendText: "<br />");
                //            }
                //            strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "Fiserv_SessionId: " + PayPlanData.RecurringCreditCardInformation.Fiserv_SessionId.ToString(), appendText: "<br />");
                //            if (string.IsNullOrWhiteSpace(PayPlanData.RecurringCreditCardInformation.Fiserv_SessionToken) == false)
                //            {
                //                strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "Fiserv_SessionToken: " + PayPlanData.RecurringCreditCardInformation.Fiserv_SessionToken, appendText: "<br />");
                //            }
                //            strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "Fiserv_WalletItemId: " + PayPlanData.RecurringCreditCardInformation.Fiserv_WalletItemId.ToString(), appendText: "<br />");
                //        }
                //        if (PayPlanData.RecurringEftInformation != null)
                //        {
                //            strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "PayPlanData.RecurringEftInformation is something", appendText: "<br />");
                //            if (string.IsNullOrWhiteSpace(PayPlanData.RecurringEftInformation.RoutingNumber) == false)
                //            {
                //                strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "RoutingNumber: " + PayPlanData.RecurringEftInformation.RoutingNumber, appendText: "<br />");
                //            }
                //            if (string.IsNullOrWhiteSpace(PayPlanData.RecurringEftInformation.AccountNumber) == false)
                //            {
                //                strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "AccountNumber: " + PayPlanData.RecurringEftInformation.AccountNumber, appendText: "<br />");
                //            }
                //            strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "AccountType: " + PayPlanData.RecurringEftInformation.AccountType.ToString(), appendText: "<br />");
                //            strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "DeductionDay: " + PayPlanData.RecurringEftInformation.DeductionDay.ToString(), appendText: "<br />");
                //            if (string.IsNullOrWhiteSpace(PayPlanData.RecurringEftInformation.EmailAddress) == false)
                //            {
                //                strPayPlanData = IFM_CreditCardProcessing.Common.Append(strPayPlanData, "EmailAddress: " + PayPlanData.RecurringEftInformation.EmailAddress, appendText: "<br />");
                //            }
                //        }
                //    }

                //    emailBody = IFM_CreditCardProcessing.Common.Append(emailBody, strPayPlanData, appendText: "<br /><br />");
                //    emailBody = IFM_CreditCardProcessing.Common.Append(emailBody, "prevPayPlanId: " + prevPayPlanId.ToString(), appendText: "<br /><br />");
                //    emailBody = IFM_CreditCardProcessing.Common.Append(emailBody, "Unhandled Exception: " + exMsg, appendText: "<br /><br />");

                //    global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail("dmink@indianafarmers.com", DataServicesCore.BusinessLogic.AppConfig.NoReplyEmailAddress, "MemberPortalApi PayPlan Change Attempt", emailBody);
                //}

                CodeOk();
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No payplan data provided");
            }
            sr.ResponseData = new { PayPlanChanged = sr.PayPlanChanged, RecurringPaymentDataFailed = sr.RecurringPaymentDataFailed };
            return Json(sr);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("payplan/{policyNumber}/{policyId}")]
        public JsonResult GetRecurringPaymentData(string policyNumber, string policyId)
        {
            APIResponses.Common.ServiceResult sr = new APIResponses.Common.ServiceResult
            {
                ResponseData = global::IFM.DataServicesCore.BusinessLogic.Payments.PayPlanHelper.GetCurrentRecurringData(policyNumber, Convert.ToInt32(policyId))
            };
            CodeOk();
            return Json(sr);
        }
        //added 5/4/2023
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("payplan_getAndUseWallets/{policyNumber}/{policyId}/{walletUserIdentifier?}/{walletPolicyNumbers?}")]
        public JsonResult GetRecurringPaymentData_GetAndUseWallets(string policyNumber, string policyId, string walletUserIdentifier = null, List<string> walletPolicyNumbers = null)
        {
            List<string> userIdentifierAndPolicyNumbers = DataServicesCore.BusinessLogic.Fiserv.GeneralHelper.ListForEmailAndPolicyNumbers(walletUserIdentifier, walletPolicyNumbers);

            APIResponses.Common.ServiceResult sr = new APIResponses.Common.ServiceResult
            {
                ResponseData = global::IFM.DataServicesCore.BusinessLogic.Payments.PayPlanHelper.GetCurrentRecurringData_UseWalletsOrWalletIdentifiers(policyNumber, Convert.ToInt32(policyId), null, null, userIdentifierAndPolicyNumbers)
            };
            CodeOk();
            return Json(sr);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("payplan_getAndUseWallets/{policyNumber}/{policyId}/{walletUserIdentifier?}")]
        public JsonResult GetRecurringPaymentData_GetAndUseWallets_PostWalletPolicyNumbers(string policyNumber, string policyId, string walletUserIdentifier = null, List<string> walletPolicyNumbers = null)
        {
            List<string> userIdentifierAndPolicyNumbers = DataServicesCore.BusinessLogic.Fiserv.GeneralHelper.ListForEmailAndPolicyNumbers(walletUserIdentifier, walletPolicyNumbers);

            APIResponses.Common.ServiceResult sr = new APIResponses.Common.ServiceResult
            {
                ResponseData = global::IFM.DataServicesCore.BusinessLogic.Payments.PayPlanHelper.GetCurrentRecurringData_UseWalletsOrWalletIdentifiers(policyNumber, Convert.ToInt32(policyId), null, null, userIdentifierAndPolicyNumbers)
            };
            CodeOk();
            return Json(sr);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("payplan_useWallets/{policyNumber}/{policyId}")]
        public JsonResult GetRecurringPaymentData_UseWallets(string policyNumber, string policyId, string walletUserIdentifier = null, List<Wallet> wallets = null)
        {
            APIResponses.Common.ServiceResult sr = new APIResponses.Common.ServiceResult
            {
                ResponseData = global::IFM.DataServicesCore.BusinessLogic.Payments.PayPlanHelper.GetCurrentRecurringData_UseWalletsOrWalletIdentifiers(policyNumber, Convert.ToInt32(policyId), null, wallets, null)
            };
            CodeOk();
            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("RemoveRecurringPayPlan")]
        public JsonResult RemoveRecurringPayPlan(global::IFM.DataServicesCore.CommonObjects.Payments.PayPlanData PayPlanData)
        {
            APIResponses.Common.ServiceResult sr = new APIResponses.Common.ServiceResult
            {
                ResponseData = global::IFM.DataServicesCore.BusinessLogic.Payments.PayPlanHelper.RemoveRecurringPayPlan(PayPlanData)
            };
            CodeOk();
            return Json(sr);
        }
    }
}