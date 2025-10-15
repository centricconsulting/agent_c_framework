using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static IFM.DataServices.API.Enums;
using static IFM_CreditCardProcessing.Enums;
using APIResponse = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Controllers.Fiserv
{
    [RoutePrefix("Fiserv")]
    public class Fiserv_GeneralController : BaseController
    {
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("DefaultPaymentSettings")]
        public JsonResult DefaultPaymentSettings()
        {
            return DefaultPaymentSettings(false);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("DefaultPaymentSettings/{isOneView}")]
        public JsonResult DefaultPaymentSettings(bool isOneView)
        {
            var sr = this.CreateServiceResult();

            IFM_CreditCardProcessing.Enums.PaymentInterface pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.MemberPortalSite;
            if (isOneView == true)
            {
                pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.OneView;
            }

            DataServicesCore.CommonObjects.Fiserv.PaymentSettings settings = DataServicesCore.BusinessLogic.Fiserv.GeneralHelper.DefaultPaymentSettings(pmtInterface);

            if (settings != null)
            {
                sr.ResponseData = settings;
                CodeOk();
            }
            else
            {
                CodeNotFound();
                sr.Messages.CreateErrorMessage($"Problem retrieving default payment settings.");
            }

            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("AuthAndSessionTokensForPolicyNumber/{policyNumber}/{isOneView?}")]
        public JsonResult AuthAndSessionTokensForPolicyNumber(string policyNumber, bool isOneView = false)
        {
            IFM_CreditCardProcessing.Enums.PaymentInterface pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.MemberPortalSite;
            if (isOneView == true)
            {
                pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.OneView;
            }

            return AuthAndSessionTokensForPolicyNumber_IFM(policyNumber, (API.Enums.PaymentInterface)pmtInterface);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("AuthAndSessionTokensForPolicyNumber_IFM/{policyNumber}/{paymentInterface}")]
        public JsonResult AuthAndSessionTokensForPolicyNumber_IFM(string policyNumber, API.Enums.PaymentInterface paymentInterface)
        {
            var sr = this.CreateServiceResult();

            if (string.IsNullOrWhiteSpace(policyNumber) == false)
            {
                if (policyNumber.IsPolicyOrAccountBillNumber())
                {
                    IFM_CreditCardProcessing.Enums.PaymentInterface pmtInterface = (IFM_CreditCardProcessing.Enums.PaymentInterface)paymentInterface;

                    DataServicesCore.BusinessLogic.Fiserv.GeneralHelper helper = new DataServicesCore.BusinessLogic.Fiserv.GeneralHelper();
                    global::IFM.DataServices.API.ResponseObjects.Payments.AuthAndSessionTokens tokens = helper.TokensForPolicyNumber(policyNumber, pmtInterface: pmtInterface);
                    if (tokens != null)
                    {
                        sr.ResponseData = tokens;
                        CodeOk();
                    }
                    else
                    {
                        CodeNotFound();
                        sr.Messages.CreateErrorMessage($"Problem retrieving tokens. Sent {policyNumber}");
                    }
                }
                else
                {
                    CodeBadRequest();
                    sr.Messages.CreateErrorMessage($"Invalid policy number format. Sent {policyNumber}");
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No policy number provided");
            }

            return Json(sr);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("AuthAndSessionTokensForEmailAddress/{emailAddress}/{isOneView?}")]
        public JsonResult AuthAndSessionTokensForEmailAddress(string emailAddress, bool isOneView = false)
        {
            IFM_CreditCardProcessing.Enums.PaymentInterface pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.MemberPortalSite;
            if (isOneView == true)
            {
                pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.OneView;
            }
            return AuthAndSessionTokensForEmailAddress_IFM(emailAddress, (API.Enums.PaymentInterface)pmtInterface);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("AuthAndSessionTokensForEmailAddress_IFM/{emailAddress}/{paymentInterface}")]
        public JsonResult AuthAndSessionTokensForEmailAddress_IFM(string emailAddress, API.Enums.PaymentInterface paymentInterface)
        {
            var sr = this.CreateServiceResult();

            if (string.IsNullOrWhiteSpace(emailAddress) == false)
            {
                //emailAddress = IFM_CreditCardProcessing.Common.UrlDecodedValue(emailAddress); //may not need this

                if (emailAddress.IsValidEmail() == true)
                {
                    IFM_CreditCardProcessing.Enums.PaymentInterface pmtInterface = (IFM_CreditCardProcessing.Enums.PaymentInterface) paymentInterface;

                    DataServicesCore.BusinessLogic.Fiserv.GeneralHelper helper = new DataServicesCore.BusinessLogic.Fiserv.GeneralHelper();
                    global::IFM.DataServices.API.ResponseObjects.Payments.AuthAndSessionTokens tokens = helper.TokensForEmailAddress(emailAddress, pmtInterface: pmtInterface);
                    if (tokens != null)
                    {
                        sr.ResponseData = tokens;
                        CodeOk();
                    }
                    else
                    {
                        CodeNotFound();
                        sr.Messages.CreateErrorMessage($"Problem retrieving tokens. Sent {emailAddress}");
                    }
                }
                else
                {
                    CodeBadRequest();
                    sr.Messages.CreateErrorMessage($"Invalid email address format. Sent {emailAddress}");
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No email address provided");
            }

            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("AuthAndSessionTokensForPolicyNumberAndEmailAddress/{policyNumber}/{emailAddress}/{isOneView?}")]
        public JsonResult AuthAndSessionTokensForPolicyNumberAndEmailAddress(string policyNumber, string emailAddress, bool isOneView = false)
        {
            IFM_CreditCardProcessing.Enums.PaymentInterface pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.MemberPortalSite;
            if (isOneView == true)
            {
                pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.OneView;
            }
            return AuthAndSessionTokensForPolicyNumberAndEmailAddress_IFM(policyNumber, emailAddress, (API.Enums.PaymentInterface)pmtInterface);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("AuthAndSessionTokensForPolicyNumberAndEmailAddress_IFM/{policyNumber}/{emailAddress}/{paymentInterface}")]
        public JsonResult AuthAndSessionTokensForPolicyNumberAndEmailAddress_IFM(string policyNumber, string emailAddress, API.Enums.PaymentInterface paymentInterface)
        {
            var sr = this.CreateServiceResult();

            if (string.IsNullOrWhiteSpace(policyNumber) == false && string.IsNullOrWhiteSpace(emailAddress) == false)
            {
                //emailAddress = IFM_CreditCardProcessing.Common.UrlDecodedValue(emailAddress); //may not need this

                if (policyNumber.IsPolicyOrAccountBillNumber() == true && emailAddress.IsValidEmail() == true)
                {
                    IFM_CreditCardProcessing.Enums.PaymentInterface pmtInterface = (IFM_CreditCardProcessing.Enums.PaymentInterface)paymentInterface;

                    DataServicesCore.BusinessLogic.Fiserv.GeneralHelper helper = new DataServicesCore.BusinessLogic.Fiserv.GeneralHelper();
                    global::IFM.DataServices.API.ResponseObjects.Payments.AuthAndSessionTokens tokens = helper.TokensForPolicyNumberAndEmailAddress(policyNumber, emailAddress, pmtInterface: pmtInterface);
                    if (tokens != null)
                    {
                        sr.ResponseData = tokens;
                        CodeOk();
                    }
                    else
                    {
                        CodeNotFound();
                        sr.Messages.CreateErrorMessage($"Problem retrieving tokens. Sent {policyNumber} and {emailAddress}");
                    }
                }
                else
                {
                    CodeBadRequest();
                    sr.Messages.CreateErrorMessage($"Invalid format for policy number and/or email address. Sent {policyNumber} and {emailAddress}");
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("Missing policy number and/or email address");
            }

            return Json(sr);
        }


        //added 8/19/2020
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("AuthAndSessionTokensForUsername/{username}/{isOneView?}")]
        public JsonResult AuthAndSessionTokensForUsername(string username, bool isOneView = false)
        {
            IFM_CreditCardProcessing.Enums.PaymentInterface pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.MemberPortalSite;
            if (isOneView == true)
            {
                pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.OneView;
            }
            return AuthAndSessionTokensForUsername_IFM(username, (API.Enums.PaymentInterface)pmtInterface);
        }

        //added 8/19/2020
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("AuthAndSessionTokensForUsername_IFM/{username}/{paymentInterface}")]
        public JsonResult AuthAndSessionTokensForUsername_IFM(string username, API.Enums.PaymentInterface paymentInterface)
        {
            var sr = this.CreateServiceResult();

            if (string.IsNullOrWhiteSpace(username) == false)
            {
                IFM_CreditCardProcessing.Enums.PaymentInterface pmtInterface = (IFM_CreditCardProcessing.Enums.PaymentInterface)paymentInterface;

                DataServicesCore.BusinessLogic.Fiserv.GeneralHelper helper = new DataServicesCore.BusinessLogic.Fiserv.GeneralHelper();
                global::IFM.DataServices.API.ResponseObjects.Payments.AuthAndSessionTokens tokens = helper.TokensForEmailAddress(username, pmtInterface: pmtInterface);
                if (tokens != null)
                {
                    sr.ResponseData = tokens;
                    CodeOk();
                }
                else
                {
                    CodeNotFound();
                    sr.Messages.CreateErrorMessage($"Problem retrieving tokens. Sent {username}");
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No username provided");
            }

            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("AuthAndSessionTokensForPolicyNumberAndUsername/{policyNumber}/{username}/{isOneView?}")]
        public JsonResult AuthAndSessionTokensForPolicyNumberAndUsername(string policyNumber, string username, bool isOneView = false)
        {
            IFM_CreditCardProcessing.Enums.PaymentInterface pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.MemberPortalSite;
            if (isOneView == true)
            {
                pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.OneView;
            }
            return AuthAndSessionTokensForPolicyNumberAndUsername_IFM(policyNumber, username, (API.Enums.PaymentInterface)pmtInterface);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("AuthAndSessionTokensForPolicyNumberAndUsername_IFM/{policyNumber}/{username}/{paymentInterface}")]
        public JsonResult AuthAndSessionTokensForPolicyNumberAndUsername_IFM(string policyNumber, string username, API.Enums.PaymentInterface paymentInterface)
        {
            var sr = this.CreateServiceResult();

            if (string.IsNullOrWhiteSpace(policyNumber) == false && string.IsNullOrWhiteSpace(username) == false)
            {
                if (policyNumber.IsPolicyOrAccountBillNumber() == true)
{
                    IFM_CreditCardProcessing.Enums.PaymentInterface pmtInterface = (IFM_CreditCardProcessing.Enums.PaymentInterface)paymentInterface;

                    DataServicesCore.BusinessLogic.Fiserv.GeneralHelper helper = new DataServicesCore.BusinessLogic.Fiserv.GeneralHelper();
                    global::IFM.DataServices.API.ResponseObjects.Payments.AuthAndSessionTokens tokens = helper.TokensForPolicyNumberAndEmailAddress(policyNumber, username, pmtInterface: pmtInterface);
                    if (tokens != null)
                    {
                        sr.ResponseData = tokens;
                        CodeOk();
                    }
                    else
                    {
                        CodeNotFound();
                        sr.Messages.CreateErrorMessage($"Problem retrieving tokens. Sent {policyNumber} and {username}");
                    }
                }
                else
                {
                    CodeBadRequest();
                    sr.Messages.CreateErrorMessage($"Invalid format for policy number. Sent {policyNumber}");
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("Missing policy number and/or username");
            }


            return Json(sr);
        }

        //added 9/26/2024
        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("CalculateFee")]
        public JsonResult CalculateFee(global::IFM.DataServicesCore.CommonObjects.Fiserv.CalculateFeeBody calcFeeBody)
        {
            var sr = new APIResponse.Common.ServiceResult<APIResponse.Fiserv.FiservCalculateFeeResult>();

            if (calcFeeBody != null)
            {
                Validation.Fiserv.CalculateFeeValidator val = new Validation.Fiserv.CalculateFeeValidator();
                var valResult = val.Validate(calcFeeBody);
                if (valResult.IsValid)
                {
                    DataServicesCore.BusinessLogic.Fiserv.GeneralHelper helper = new DataServicesCore.BusinessLogic.Fiserv.GeneralHelper();
                    sr = helper.CalculateFee(calcFeeBody);
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
                sr.Messages.CreateErrorMessage("No fee data data provided");
            }

            return Json(sr);
        }

        //added 9/27/2024
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("ShouldCalculateFee")]
        public JsonResult ShouldCalculateFee()
        {
            return ShouldCalculateFee("");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("ShouldCalculateFee/{dateToUse}")]
        public JsonResult ShouldCalculateFee(string dateToUse)
        {
            var sr = this.CreateServiceResult();

            bool calcFee = IFM_CreditCardProcessing.Common.CalculateFeeAmountOnCreditCardPayments();
            string dt = IFM_CreditCardProcessing.Common.CalculateFeeAmountOnCreditCardPayments_Date();

            //if (IFM_CreditCardProcessing.Common.IsValidDateString(dt, mustBeGreaterThanDefaultDate: true))
            //{
            //    bool isOkay = false;
            //    if (calcFee)
            //    {
            //        if (IFM_CreditCardProcessing.Common.IsValidDateString(dateToUse, mustBeGreaterThanDefaultDate: true) == false)
            //        {
            //            dateToUse = DateTime.Today.ToShortDateString();
            //        }
            //        if (Convert.ToDateTime(dateToUse) >= Convert.ToDateTime(dt))
            //        {
            //            isOkay = true;
            //        }
            //    }
            //    sr.ResponseData = isOkay;
            //    CodeOk();
            //}
            //else
            //{
            //    CodeNotFound();
            //    sr.Messages.CreateErrorMessage($"Problem retrieving CalculateFee settings.");
            //}
            //decided to not require date setting; will just default to current date when missing
            bool isOkay = false;
            if (calcFee)
            {
                if (IFM_CreditCardProcessing.Common.IsValidDateString(dt, mustBeGreaterThanDefaultDate: true) == false)
                {
                    dt = DateTime.Today.ToShortDateString();
                }
                if (IFM_CreditCardProcessing.Common.IsValidDateString(dateToUse, mustBeGreaterThanDefaultDate: true) == false)
                {
                    dateToUse = DateTime.Today.ToShortDateString();
                }
                if (Convert.ToDateTime(dateToUse) >= Convert.ToDateTime(dt))
                {
                    isOkay = true;
                }
            }
            sr.ResponseData = isOkay;
            CodeOk();

            return Json(sr);
        }
    }
}