using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IFM.DataServicesCore.BusinessLogic;
using static IFM.DataServicesCore.BusinessLogic.AppConfig;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Controllers.IFM.Payment
{
    [RoutePrefix("IFM/Payment/BillingInformation")]
    public class IFMPayment_BillingInformationController : BaseController
    {

        [AcceptVerbs(HttpVerbs.Get)]
        [Route("policyByOnlinePaymentNumber/{policyNumber}/{onlinePaymentNumber:int}")]
        public JsonResult GetBillingInfo(string policyNumber, Int32 onlinePaymentNumber)
        {
            APIResponses.Common.ServiceResult sr = new APIResponses.Common.ServiceResult();

            if (global::IFM.DataServicesCore.BusinessLogic.OMP.DiamondLogin.OMPLogin())
                sr.ResponseData = global::IFM.DataServicesCore.BusinessLogic.PublicDomain.PolicyData.GetPayableImages(policyNumber, onlinePaymentNumber);

            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Route("policyByFullName/{policyNumber}/{fullname}")]
        public JsonResult GetBillingInfo(string policyNumber, string fullname)
        {
            APIResponses.Common.ServiceResult sr = new APIResponses.Common.ServiceResult();
            if (global::IFM.DataServicesCore.BusinessLogic.OMP.DiamondLogin.OMPLogin())
                sr.ResponseData = global::IFM.DataServicesCore.BusinessLogic.PublicDomain.PolicyData.GetPayableImages(policyNumber, fullname);

            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Route("AccountByFullName/{accountnumber}/{fullname}")]
        public JsonResult GetAccountBillingInfo(string accountnumber, string fullname)
        {
            APIResponses.Common.ServiceResult sr = new APIResponses.Common.ServiceResult();
            string policyNumber = "";
            if (accountnumber.IsAccountBillNumber())
            {
                var policyAPI = new global::IFM.PolicyAPIModels.Request.AccountBillInquiry(AppConfig.PolicyInquiryAPIEndpoint);
                var response = policyAPI.GetPreferredAccountBillPolicyByAccountNumber(accountnumber);
                policyNumber = response.ResponseData;
            }
            if (global::IFM.DataServicesCore.BusinessLogic.OMP.DiamondLogin.OMPLogin())
                sr.ResponseData = global::IFM.DataServicesCore.BusinessLogic.PublicDomain.PolicyData.GetPayableImages(policyNumber, fullname);

            return Json(sr);
        }
    }
}