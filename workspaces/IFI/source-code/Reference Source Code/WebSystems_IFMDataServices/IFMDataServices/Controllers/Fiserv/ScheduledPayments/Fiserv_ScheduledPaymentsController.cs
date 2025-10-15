using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using APIResponse = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Controllers.Fiserv.ScheduledPayments
{
    [RoutePrefix("Fiserv/ScheduledPayments")]
    public class Fiserv_ScheduledPaymentsController : BaseController
    {
        //[AcceptVerbs(HttpVerbs.Get]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        //[Route("ScheduledPaymentsForPolicyNumbers/{policyNumbers}")]
        //public JsonResult ScheduledPaymentsForPolicyNumbers(List<string> policyNumbers)
        //{
        //    var sr = this.CreateServiceResult();

        //    if (policyNumbers != null & policyNumbers.Count > 0)
        //    {
        //        //emailAddress = IFM_CreditCardProcessing.Common.UrlDecodedValue(emailAddress); //may not need this

        //        //if (CommonValidations.IsPolicyNumber(policyNumber1) == true)
        //        //{

        //        DataServicesCore.BusinessLogic.Fiserv.ScheduledPaymentHelper helper = new DataServicesCore.BusinessLogic.Fiserv.ScheduledPaymentHelper();
        //        List<DataServicesCore.CommonObjects.Fiserv.ScheduledPayment> scheduledPayments = helper.ScheduledPaymentsForPolicyNumbers(policyNumbers);
        //        if (scheduledPayments != null && scheduledPayments.Count > 0)
        //        {
        //            sr.ResponseData = scheduledPayments;
        //            CodeOk();
        //        }
        //        else
        //        {
        //            CodeNotFound();
        //            sr.Messages.CreateErrorMessage($"Problem retrieving scheduled payments.");
        //        }
        //        //}
        //        //else
        //        //{
        //        //    CodeBadRequest();
        //        //    sr.Messages.CreateErrorMessage($"Invalid policy number format. Sent {policyNumber1}");
        //        //}
        //    }
        //    else
        //    {
        //        CodeBadRequest();
        //        sr.Messages.CreateErrorMessage("No policyNumbers provided");
        //    }

        //    return Json(sr);
        //}


        //we could just add the POST verb to above, but to make it explicit I'll add another call
        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("ScheduledPaymentsForPolicyNumbers")]
        public JsonResult ScheduledPaymentsForPolicyNumbersList(List<string> policyNumbers)
        {
            var sr = this.CreateServiceResult();

            if (policyNumbers?.Any() ?? false)
            {

                DataServicesCore.BusinessLogic.Fiserv.ScheduledPaymentHelper helper = new DataServicesCore.BusinessLogic.Fiserv.ScheduledPaymentHelper();
                List<DataServicesCore.CommonObjects.Fiserv.ScheduledPayment> scheduledPayments = helper.ScheduledPaymentsForPolicyNumbers(policyNumbers);

                if (scheduledPayments != null && scheduledPayments.Count > 0)
                {
                    sr.ResponseData = scheduledPayments;
                    CodeOk();
                }
                else
                {
                    CodeNotFound();
                    sr.Messages.CreateErrorMessage($"Problem retrieving scheduled payments.");
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No policyNumbers provided");
            }

            return Json(sr);
        }

        //[AcceptVerbs(HttpVerbs.Get)]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        //[Route("ScheduledPaymentsForPolicyNumbers_FromUri/{policyNumbers}")]
        //public JsonResult ScheduledPaymentsForPolicyNumbers_FromUri([System.Web.Http.FromUri] List<string> policyNumbers)
        //{
        //    var sr = this.CreateServiceResult();

        //    if (policyNumbers != null & policyNumbers.Count > 0)
        //    {
        //        //emailAddress = IFM_CreditCardProcessing.Common.UrlDecodedValue(emailAddress); //may not need this

        //        //if (CommonValidations.IsPolicyNumber(policyNumber1) == true)
        //        //{

        //        DataServicesCore.BusinessLogic.Fiserv.ScheduledPaymentHelper helper = new DataServicesCore.BusinessLogic.Fiserv.ScheduledPaymentHelper();
        //        List<DataServicesCore.CommonObjects.Fiserv.ScheduledPayment> scheduledPayments = helper.ScheduledPaymentsForPolicyNumbers(policyNumbers);
        //        if (scheduledPayments != null && scheduledPayments.Count > 0)
        //        {
        //            sr.ResponseData = scheduledPayments;
        //            CodeOk();
        //        }
        //        else
        //        {
        //            CodeNotFound();
        //            sr.Messages.CreateErrorMessage($"Problem retrieving scheduled payments.");
        //        }
        //        //}
        //        //else
        //        //{
        //        //    CodeBadRequest();
        //        //    sr.Messages.CreateErrorMessage($"Invalid policy number format. Sent {policyNumber1}");
        //        //}
        //    }
        //    else
        //    {
        //        CodeBadRequest();
        //        sr.Messages.CreateErrorMessage("No policyNumbers provided");
        //    }

        //    return Json(sr);
        //}


        //[AcceptVerbs(HttpVerbs.Get)]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        //[Route("ScheduledPaymentsForEachPolicyNumber/{policyNumber1}/{policyNumber2?}/{policyNumber3?}/{policyNumber4?}/{policyNumber5?}/{policyNumber6?}/{policyNumber7?}/{policyNumber8?}/{policyNumber9?}/{policyNumber10?}")]
        //public JsonResult ScheduledPaymentsForEachPolicyNumber(string policyNumber1, string policyNumber2 = null, string policyNumber3 = null, string policyNumber4 = null, string policyNumber5 = null, string policyNumber6 = null, string policyNumber7 = null, string policyNumber8 = null, string policyNumber9 = null, string policyNumber10 = null)
        //{
        //    var sr = this.CreateServiceResult();

        //    if (string.IsNullOrWhiteSpace(policyNumber1) == false)
        //    {
        //        //emailAddress = IFM_CreditCardProcessing.Common.UrlDecodedValue(emailAddress); //may not need this

        //        //if (CommonValidations.IsPolicyNumber(policyNumber1) == true)
        //        //{
        //        List<string> policyNumbers = null;
        //        IFM_CreditCardProcessing.Common.AddStringToList(policyNumber1, ref policyNumbers);
        //        IFM_CreditCardProcessing.Common.AddStringToList(policyNumber2, ref policyNumbers);
        //        IFM_CreditCardProcessing.Common.AddStringToList(policyNumber3, ref policyNumbers);
        //        IFM_CreditCardProcessing.Common.AddStringToList(policyNumber4, ref policyNumbers);
        //        IFM_CreditCardProcessing.Common.AddStringToList(policyNumber5, ref policyNumbers);
        //        IFM_CreditCardProcessing.Common.AddStringToList(policyNumber6, ref policyNumbers);
        //        IFM_CreditCardProcessing.Common.AddStringToList(policyNumber7, ref policyNumbers);
        //        IFM_CreditCardProcessing.Common.AddStringToList(policyNumber8, ref policyNumbers);
        //        IFM_CreditCardProcessing.Common.AddStringToList(policyNumber9, ref policyNumbers);
        //        IFM_CreditCardProcessing.Common.AddStringToList(policyNumber10, ref policyNumbers);

        //        DataServicesCore.BusinessLogic.Fiserv.ScheduledPaymentHelper helper = new DataServicesCore.BusinessLogic.Fiserv.ScheduledPaymentHelper();
        //        List<DataServicesCore.CommonObjects.Fiserv.ScheduledPayment> scheduledPayments = helper.ScheduledPaymentsForPolicyNumbers(policyNumbers);
        //        if (scheduledPayments != null && scheduledPayments.Count > 0)
        //        {
        //            sr.ResponseData = scheduledPayments;
        //            CodeOk();
        //        }
        //        else
        //        {
        //            CodeNotFound();
        //            sr.Messages.CreateErrorMessage($"Problem retrieving scheduled payments. Sent {policyNumber1} along with other policy numbers");
        //        }
        //        //}
        //        //else
        //        //{
        //        //    CodeBadRequest();
        //        //    sr.Messages.CreateErrorMessage($"Invalid policy number format. Sent {policyNumber1}");
        //        //}
        //    }
        //    else
        //    {
        //        CodeBadRequest();
        //        sr.Messages.CreateErrorMessage("No policyNumbers provided");
        //    }

        //    return Json(sr);
        //}



        //added 6/30/2020
        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("AddScheduledPayment")]
        public JsonResult AddScheduledPayment(global::IFM.DataServicesCore.CommonObjects.Fiserv.AddScheduledPaymentBody addBody)
        {
            var sr = new APIResponse.Common.ServiceResult<APIResponse.Fiserv.FiservAddScheduledPaymentResult>();

            if (addBody != null)
            {
                Validation.Fiserv.AddScheduledPaymentBodyValidator val = new Validation.Fiserv.AddScheduledPaymentBodyValidator();
                var valResult = val.Validate(addBody);
                if (valResult.IsValid)
                {
                    DataServicesCore.BusinessLogic.Fiserv.ScheduledPaymentHelper helper = new DataServicesCore.BusinessLogic.Fiserv.ScheduledPaymentHelper();
                    sr = helper.AddScheduledPayment(addBody);
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
                sr.Messages.CreateErrorMessage("No scheduled payment data provided");
            }
            //sr.ResponseData = new { Success = sr.Success, FiservScheduledPaymentId = sr.FiservScheduledPaymentId, ReferenceId = sr.ReferenceId };

            return Json(sr);
        }


        [AcceptVerbs(HttpVerbs.Put)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("UpdateScheduledPayment")]
        public JsonResult UpdateScheduledPayment(global::IFM.DataServicesCore.CommonObjects.Fiserv.UpdateScheduledPaymentBody updateBody)
        {
            var sr = new APIResponse.Common.ServiceResult<APIResponse.Fiserv.FiservUpdateScheduledPaymentResult>();

            if (updateBody != null)
            {
                Validation.Fiserv.UpdateScheduledPaymentBodyValidator val = new Validation.Fiserv.UpdateScheduledPaymentBodyValidator();
                var valResult = val.Validate(updateBody);
                if (valResult.IsValid)
                {
                    DataServicesCore.BusinessLogic.Fiserv.ScheduledPaymentHelper helper = new DataServicesCore.BusinessLogic.Fiserv.ScheduledPaymentHelper();
                    sr = helper.UpdateScheduledPayment(updateBody);
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
                sr.Messages.CreateErrorMessage("No scheduled payment data provided");
            }
            //sr.ResponseData = new { Success = sr.Success };

            return Json(sr);
        }


        [AcceptVerbs(HttpVerbs.Delete)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("DeleteScheduledPayment")]
        public JsonResult DeleteScheduledPayment(global::IFM.DataServicesCore.CommonObjects.Fiserv.DeleteScheduledPaymentBody deleteBody)
        {
            var sr = new APIResponse.Common.ServiceResult<APIResponse.Fiserv.FiservDeleteScheduledPaymentResult>();

            if (deleteBody != null)
            {
                Validation.Fiserv.DeleteScheduledPaymentBodyValidator val = new Validation.Fiserv.DeleteScheduledPaymentBodyValidator();
                var valResult = val.Validate(deleteBody);
                if (valResult.IsValid)
                {
                    DataServicesCore.BusinessLogic.Fiserv.ScheduledPaymentHelper helper = new DataServicesCore.BusinessLogic.Fiserv.ScheduledPaymentHelper();
                    sr = helper.DeleteScheduledPayment(deleteBody);
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
                sr.Messages.CreateErrorMessage("No scheduled payment data provided");
            }
            //sr.ResponseData = new { Success = sr.Success };

            return Json(sr);
        }



    }
}