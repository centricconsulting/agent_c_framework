using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using global::IFM.DataServicesCore.CommonObjects;
using System.Net;
using Diamond.Common.Objects.Billing;
using APIResponses = IFM.DataServices.API.ResponseObjects;
using APIRequests = IFM.DataServices.API.RequestObjects;
using IFM.DataServicesCore.CommonObjects.OMP;

namespace IFM.DataServices.Controllers.IFM.Payment
{
    [RoutePrefix("IFM/Payment/Processor")]
    public class IFMPayment_ProcessorController : BaseController
    {
        private const string location = "IFM.DataServices.Controllers.IFM.Payment.IFMPayment_ProcessorController";

        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("paymentdata")]
        public JsonResult MemberPortalOrOneViewPayment(global::IFM.DataServicesCore.CommonObjects.Payments.MemberPortalPaymentData mpPayData)
        {
            APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr = new APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult>();

            DataServices.API.RequestObjects.Payments.PaymentData paymentInfo = null;

            if (mpPayData != null)
            {
                paymentInfo = mpPayData.ConvertToNewPaymentDataObject();
            }

            if (paymentInfo != null)
            {
                DecryptIfNecessary(paymentInfo);

                CheckForAccountBillInfo(paymentInfo);

                Validation.PaymentDataValidator val = new Validation.PaymentDataValidator(Validation.PaymentDataValidator.ValidationInterface.MemberPortalOrOneView);
                var valResult = val.Validate(paymentInfo);
                if (valResult.IsValid)
                {
                    sr = global::IFM.DataServicesCore.BusinessLogic.Payments.ProcessPayment.MakePayment(paymentInfo);
                }
                else
                {
                    this.AddFluentErrorsToServiceResult(sr, valResult.Errors);
                }

                CodeOk();
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No payment or policy data provided");
            }

            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("MakePayment")]
        public JsonResult MakePayment(APIRequests.Payments.PaymentData paymentInfo)
        {
            APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr = new APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult>();

            if (paymentInfo != null)
            {
                DecryptIfNecessary(paymentInfo);

                CheckForAccountBillInfo(paymentInfo);

                Validation.PaymentDataValidator val = new Validation.PaymentDataValidator();
                var valResult = val.Validate(paymentInfo);
                if (valResult.IsValid)
                {
                    sr = global::IFM.DataServicesCore.BusinessLogic.Payments.ProcessPayment.MakePayment(paymentInfo);
                }
                else
                {
                    this.AddFluentErrorsToServiceResult(sr, valResult.Errors);
                }

                CodeOk();
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No payment or policy data provided");
            }

            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("PostPayment")]
        public JsonResult PostPayment(APIRequests.Payments.PaymentData paymentInfo)
        {
            APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr = new APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult>();

            if (paymentInfo != null)
            {
                DecryptIfNecessary(paymentInfo);

                CheckForAccountBillInfo(paymentInfo);

                Validation.PaymentDataValidator val = new Validation.PaymentDataValidator();
                var valResult = val.Validate(paymentInfo);
                if (valResult.IsValid)
                {
                    sr = global::IFM.DataServicesCore.BusinessLogic.Payments.ProcessPayment.PostPayment(paymentInfo);
                }
                else
                {
                    this.AddFluentErrorsToServiceResult(sr, valResult.Errors);
                }

                CodeOk();
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No payment or policy data provided");
            }

            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("MakeBulkPayment")]
        public JsonResult MakeBulkPayment(List<APIRequests.Payments.PaymentData> paymentInfos)
        {
            APIResponses.Common.ServiceResult srMain = new APIResponses.Common.ServiceResult();

            if (paymentInfos != null)
            {
                bool oneHasInfo = false;
                int paymentsAttemptedCount = 0;
                List<APIResponses.Payments.BulkPaymentResult> payments = new List<APIResponses.Payments.BulkPaymentResult>();
                foreach (var paymentInfo in paymentInfos)
                {
                    APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr = new APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult>();
                    if (paymentInfo != null)
                    {
                        oneHasInfo = true;

                        DecryptIfNecessary(paymentInfo);

                        CheckForAccountBillInfo(paymentInfo);

                        Validation.PaymentDataValidator val = new Validation.PaymentDataValidator();
                        var valResult = val.Validate(paymentInfo);
                        if (valResult.IsValid)
                        {
                            paymentsAttemptedCount++;
                            sr = global::IFM.DataServicesCore.BusinessLogic.Payments.ProcessPayment.MakePayment(paymentInfo);
                        }
                        else
                        {
                            this.AddFluentErrorsToServiceResult(sr, valResult.Errors);
                        }

                        APIResponses.Payments.BulkPaymentResult bulkPaymentResult = new APIResponses.Payments.BulkPaymentResult
                        {
                            PaymentConfirmationNumber = sr.ResponseData.PaymentConfirmationNumber,
                            PaymentCompleted = sr.ResponseData.PaymentCompleted,
                            PolicyNumber = paymentInfo.PolicyNumber,
                            PolicyId = paymentInfo.PolicyId,
                            PolicyImageNum = paymentInfo.PolicyImageNumber,
                            AccountBillNumber = paymentInfo.AccountBillNumber
                        };

                        bulkPaymentResult.SetMessages(sr.Messages);
                        bulkPaymentResult.SetDetailedErrorMessages(sr.DetailedErrorMessages);

                        payments.Add(bulkPaymentResult);
                    }
                }

                if (payments != null && payments.Count > 0)
                {
                    srMain.ResponseData = payments;
                }

                if (oneHasInfo)
                {
                    CodeOk();
                    if (paymentsAttemptedCount > 0)
                    {
                        srMain.Messages.CreateGeneralMessage(paymentsAttemptedCount + " payments attempted");
                    }
                    else
                    {
                        srMain.Messages.CreateGeneralMessage("No payments attempted");
                    }
                }
                else
                {
                    CodeBadRequest();
                    srMain.Messages.CreateErrorMessage("No payment or policy data provided");
                }
            }
            else
            {
                CodeBadRequest();
                srMain.Messages.CreateErrorMessage("No payment or policy data provided");
            }

            return Json(srMain);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("PostBulkPayment")]
        public JsonResult PostBulkPayment(List<APIRequests.Payments.PaymentData> paymentInfos)
        {
            APIResponses.Common.ServiceResult srMain = new APIResponses.Common.ServiceResult();

            if (paymentInfos != null)
            {
                bool oneHasInfo = false;
                int paymentsAttemptedCount = 0;
                List<APIResponses.Payments.BulkPaymentResult> payments = new List<APIResponses.Payments.BulkPaymentResult>();
                foreach (var paymentInfo in paymentInfos)
                {
                    APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr = new APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult>();
                    if (paymentInfo != null)
                    {
                        oneHasInfo = true;

                        DecryptIfNecessary(paymentInfo);

                        CheckForAccountBillInfo(paymentInfo);

                        Validation.PaymentDataValidator val = null;

                        if (paymentInfo.PostPaymentInfo != null && paymentInfo.PostPaymentInfo.EcheckAfterHoursId > 0)
                        {
                            val = new Validation.PaymentDataValidator(Validation.PaymentDataValidator.ValidationInterface.eCheckAfterHours);
                        }
                        else
                        {
                            val = new Validation.PaymentDataValidator();
                        }
                        
                        var valResult = val.Validate(paymentInfo);
                        if (valResult.IsValid)
                        {
                            paymentsAttemptedCount++;
                            sr = global::IFM.DataServicesCore.BusinessLogic.Payments.ProcessPayment.PostPayment(paymentInfo);
                        }
                        else
                        {
                            this.AddFluentErrorsToServiceResult(sr, valResult.Errors) ;
                        }

                        APIResponses.Payments.BulkPaymentResult bulkPaymentResult = new APIResponses.Payments.BulkPaymentResult{
                            PaymentConfirmationNumber = sr.ResponseData.PaymentConfirmationNumber,
                            PaymentCompleted = sr.ResponseData.PaymentCompleted,
                            PolicyNumber = paymentInfo.PolicyNumber,
                            PolicyId = paymentInfo.PolicyId,
                            PolicyImageNum = paymentInfo.PolicyImageNumber,
                            AccountBillNumber = paymentInfo.AccountBillNumber
                        };

                        bulkPaymentResult.SetMessages(sr.Messages);
                        bulkPaymentResult.SetDetailedErrorMessages(sr.DetailedErrorMessages);

                        payments.Add(bulkPaymentResult);
                    }
                }

                if (payments != null && payments.Count > 0)
                {
                    srMain.ResponseData = payments;
                }

                if (oneHasInfo)
                {
                    CodeOk();
                    if (paymentsAttemptedCount > 0)
                    {
                        srMain.Messages.CreateGeneralMessage(paymentsAttemptedCount + " payments attempted");
                    }
                    else
                    {
                        srMain.Messages.CreateGeneralMessage("No payments attempted");
                    }
                }
                else
                {
                    CodeBadRequest();
                    srMain.Messages.CreateErrorMessage("No payment or policy data provided");
                }
            }
            else
            {
                CodeBadRequest();
                srMain.Messages.CreateErrorMessage("No payment or policy data provided");
            }
            
            return Json(srMain);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("ApplyCash")]
        public JsonResult ApplyCash(APIRequests.Payments.PaymentData paymentInfo)
        {
            APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr = new APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult>();

            if (paymentInfo != null)
            {
                //DecryptIfNecessary(paymentInfo);

                CheckForAccountBillInfo(paymentInfo);

                FluentValidation.Results.ValidationResult valResult = new FluentValidation.Results.ValidationResult();

                if (paymentInfo.PaymentSettings == null || paymentInfo.PaymentSettings.BypassValidation == false)
                {
                    Validation.PaymentDataValidator val = new Validation.PaymentDataValidator(Validation.PaymentDataValidator.ValidationInterface.ApplyCash);
                    valResult = val.Validate(paymentInfo);
                }
                if (valResult.IsValid)
                {
                    global::IFM.DataServicesCore.BusinessLogic.Payments.ApplyCash ac = new DataServicesCore.BusinessLogic.Payments.ApplyCash(paymentInfo);
                    sr = ac.DoApplyCashServiceResult();
                }
                else
                {
                    this.AddFluentErrorsToServiceResult(sr, valResult.Errors);
                }

                CodeOk();
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No payment or policy data provided");
            }

            return Json(sr);
        }


        private void CheckForAccountBillInfo(APIRequests.Payments.PaymentData paymentInfo)
        {
            string NumToUse = "";
            if (paymentInfo.AccountBillNumber.IsAccountBillNumber())
            {
                NumToUse = paymentInfo.AccountBillNumber;
            }
            else
            {
                NumToUse = paymentInfo.PolicyNumber;
            }

            var policyAPI = new global::IFM.PolicyAPIModels.Request.AccountBillInquiry(global::IFM.DataServicesCore.BusinessLogic.AppConfig.PolicyInquiryAPIEndpoint);
            var apiResponse = policyAPI.GetAccountBillInfoByAccountNumberOrPolicyNumber(NumToUse);
            if(apiResponse?.ResponseData?.PoliciesInAccount?.Count > 0)
            {
                var accountBillInfo = apiResponse.ResponseData;
                var preferredPolicy = accountBillInfo.PoliciesInAccount.MinBy(x => x.PreferredAccountBillPolicyOrder);
                paymentInfo.AccountBillNumber = accountBillInfo.AccountNumber;
                paymentInfo.PolicyId = preferredPolicy.PolicyId;
                paymentInfo.PolicyNumber = preferredPolicy.PolicyNumber;
                paymentInfo.PolicyImageNumber = preferredPolicy.PolicyImageNum;
            }
            else
            {
                paymentInfo.AccountBillNumber = "";
                if (apiResponse == null)
                {
                    global::IFM.IFMErrorLogging.LogIssue("", $"{location}.CheckForAccountBillInfo; {policyAPI.GetDebugInfo()}");
                }
            }
        }

        private void DecryptIfNecessary(APIRequests.Payments.PaymentData payment)
        {
            if (payment != null)
            {
                if (payment.UserPassword.IsNotNullEmptyOrWhitespace())
                {
                    payment.UserPassword = payment.UserPassword.DoubleDecrypt();
                }
                if (payment.ECheckPaymentInformation != null)
                {
                    if (payment.ECheckPaymentInformation.RoutingNumber.IsNotNullEmptyOrWhitespace() && payment.ECheckPaymentInformation.RoutingNumber.IsNumeric() == false)
                    {
                        payment.ECheckPaymentInformation.RoutingNumber = payment.ECheckPaymentInformation.RoutingNumber.DoubleDecrypt();
                    }
                    if (payment.ECheckPaymentInformation.AccountNumber.IsNotNullEmptyOrWhitespace() && payment.ECheckPaymentInformation.AccountNumber.IsNumeric() == false)
                    {
                        payment.ECheckPaymentInformation.AccountNumber = payment.ECheckPaymentInformation.AccountNumber.DoubleDecrypt();
                    }
                }
                if (payment.CreditCardPaymentInformation != null)
                {
                    if (payment.CreditCardPaymentInformation.CardNumber.IsNotNullEmptyOrWhitespace() && payment.CreditCardPaymentInformation.CardNumber.IsNumeric() == false)
                    {
                        payment.CreditCardPaymentInformation.CardNumber = payment.CreditCardPaymentInformation.CardNumber.DoubleDecrypt();
                    }
                    if (payment.CreditCardPaymentInformation.SecurityCode.IsNotNullEmptyOrWhitespace() && payment.CreditCardPaymentInformation.SecurityCode.IsNumeric() == false)
                    {
                        payment.CreditCardPaymentInformation.SecurityCode = payment.CreditCardPaymentInformation.SecurityCode.DoubleDecrypt();
                    }
                    if (payment.CreditCardPaymentInformation.ZIPCode.IsNotNullEmptyOrWhitespace() && payment.CreditCardPaymentInformation.ZIPCode.IsValidZipCode() == false)
                    {
                        payment.CreditCardPaymentInformation.ZIPCode = payment.CreditCardPaymentInformation.ZIPCode.DoubleDecrypt();
                    }
                }
            }
        }
    }
}