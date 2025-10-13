using IFM.VR.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IFM.PrimitiveExtensions;
using IFM.DataServicesCore.CommonObjects.Payments;
using IFM.DataServicesCore.BusinessLogic.Payments;
using APIResponses = IFM.DataServices.API.ResponseObjects;
using static IFM.DataServicesCore.CommonObjects.Enums.Enums;

namespace IFM.DataServices.Controllers.IFM.Payment
{
    [RoutePrefix("IFM/Payment/PayPlan")]
    public class IFMPayment_PayPlanController : BaseController
    {
        
        [AcceptVerbs(HttpVerbs.Get)]
        [Route("GetCurrentPayPlanIdById/{PayPlanId?}")]
        public JsonResult GetCurrentPayPlanIdById(int PayPlanId)
        {
            APIResponses.Common.ServiceResult sr = new APIResponses.Common.ServiceResult();

            if (PayPlanId.HasValue())
            {
                CodeOk();
                GetCurrentPayPlanOptions ppOptions = new GetCurrentPayPlanOptions 
                { 
                    PayPlanId = PayPlanId
                };
                var payplan = PayPlanHelper.GetCurrentPayPlans(ppOptions).FirstOrDefault();

                if (payplan == null || payplan.BillingPayPlanId == 0)
                {
                    sr.Messages.CreateErrorMessage("Could not find a current version of the payplan.");
                }
                else
                {
                    sr.ResponseData = payplan;
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage($"PayPlanId is required.");
            }

            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Route("GetCurrentPayPlanIdByName/{PayPlanName?}")]
        public JsonResult GetCurrentPayPlanIdByName(string PayPlanName)
        {
            APIResponses.Common.ServiceResult sr = new APIResponses.Common.ServiceResult();

            if (PayPlanName.HasValue())
            {
                CodeOk();
                GetCurrentPayPlanOptions ppOptions = new GetCurrentPayPlanOptions
                {
                    PayPlanName = PayPlanName
                };
                var payplans = PayPlanHelper.GetCurrentPayPlans(ppOptions);

                if (payplans.IsLoaded() == false)
                {
                    sr.Messages.CreateErrorMessage("Could not find a current version of the payplan by name.");
                }
                else
                {
                    sr.ResponseData = payplans;
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage($"PayPlanId is required.");
            }

            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Route("GetAllCurrentPayPlans")]
        public JsonResult GetAllCurrentPayPlans()
        {
            APIResponses.Common.ServiceResult sr = new APIResponses.Common.ServiceResult();

            CodeOk();
            var payplans = PayPlanHelper.GetCurrentPayPlans(new GetCurrentPayPlanOptions());

            if (payplans.IsLoaded() == false)
            {
                sr.Messages.CreateErrorMessage("Could not retrieve any payplans.");
            }
            else
            {
                sr.ResponseData = payplans;
            }

            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Route("GetAllCurrentDirectBillPayPlans")]
        public JsonResult GetAllCurrentDirectBillPayPlans()
        {
            APIResponses.Common.ServiceResult sr = new APIResponses.Common.ServiceResult();

            CodeOk();

            GetCurrentPayPlanOptions ppOptions = new GetCurrentPayPlanOptions { 
                BillMethodId = 2
            };
            var payplans = PayPlanHelper.GetCurrentPayPlans(ppOptions);

            if (payplans.IsLoaded() == false)
            {
                sr.Messages.CreateErrorMessage("Could not retrieve any payplans.");
            }
            else
            {
                sr.ResponseData = payplans;
            }

            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Route("GetAllCurrentAgencyBillPayPlans")]
        public JsonResult GetAllCurrentAgencyBillPayPlans()
        {
            APIResponses.Common.ServiceResult sr = new APIResponses.Common.ServiceResult();

            CodeOk();

            GetCurrentPayPlanOptions ppOptions = new GetCurrentPayPlanOptions
            {
                BillMethodId = 1
            };
            var payplans = PayPlanHelper.GetCurrentPayPlans(ppOptions);

            if (payplans.IsLoaded() == false)
            {
                sr.Messages.CreateErrorMessage("Could not retrieve any payplans.");
            }
            else
            {
                sr.ResponseData = payplans;
            }

            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Route("GetAllPayPlans")]
        public JsonResult GetAllPayPlans()
        {
            APIResponses.Common.ServiceResult sr = new APIResponses.Common.ServiceResult();

            CodeOk();

            GetCurrentPayPlanOptions ppOptions = new GetCurrentPayPlanOptions
            {
                GetExpiredPayPlans = true
            };
            var payplans = PayPlanHelper.GetCurrentPayPlans(ppOptions);

            if (payplans.IsLoaded() == false)
            {
                sr.Messages.CreateErrorMessage("Could not retrieve any payplans.");
            }
            else
            {
                sr.ResponseData = payplans;
            }

            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Route("GetCurrentPayPlansByOptions")]
        public JsonResult GetCurrentPayPlansByOptions(GetCurrentPayPlanOptions PayPlanOptions)
        {
            APIResponses.Common.ServiceResult sr = new APIResponses.Common.ServiceResult();

            if (PayPlanOptions != null)
            {
                CodeOk();
                var payplans = PayPlanHelper.GetCurrentPayPlans(PayPlanOptions);

                if (payplans.IsLoaded() == false)
                {
                    sr.Messages.CreateErrorMessage("Could not find most current payplan(s).");
                }
                else
                {
                    sr.ResponseData = payplans;
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage($"PayPlanId is required.");
            }

            return Json(sr);
        }

    }
}