using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using IFM.DataServicesCore;

namespace IFM.DataServices.Controllers.eSignature.Agency
{
    [RoutePrefix("eSignature/Agency")]
    public class eSignatureAgency_AgencyInformationController : BaseController
    {
        [AcceptVerbs(HttpVerbs.Get)]
        [Route("AgencyInfoByAgencyCode/{agencyCode?}")]
        public JsonResult GetAgencyInformationByAgencyCode(string agencyCode)
        {
            var sr = this.CreateServiceResult();
            //var agencyInfo = new DataServicesCore.CommonObjects.BasicAgencyInfo();

            if (string.IsNullOrWhiteSpace(agencyCode) == false)
            {
                if (agencyCode.IsAgencyCode_LongOrShort() == true)
                {
                    var agencyInfo = DataServicesCore.BusinessLogic.Diamond.AgencyInformation.GetAgencyInformationByAgencyCode(agencyCode);
                    if (agencyInfo?.AgencyId > 0)
                    {
                        sr.ResponseData = agencyInfo;
                        CodeOk();
                    }
                    else
                    {
                        CodeNotFound();
                        sr.Messages.CreateErrorMessage($"No agency info found. Sent {agencyCode}");
                    }
                }
                else
                {
                    CodeBadRequest();
                    //sr.Messages.CreateErrorMessage($"Invalid ZIP code format. Sent {agencyCode}");
                    //updated 5/30/2020
                    sr.Messages.CreateErrorMessage($"Invalid agency code format. Sent {agencyCode}");
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No agency code provided");
            }

            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Route("AgencyInfoByPolicyNumber/{policynumber?}")]
        public JsonResult GetAgencyInformationByPolicyNumber(string policyNumber)
        {
            var sr = this.CreateServiceResult();
            //var agencyInfo = new DataServicesCore.CommonObjects.BasicAgencyInfo();

            if (string.IsNullOrWhiteSpace(policyNumber) == false)
            {

                if(policyNumber.IsPolicyNumber())
                {
                    var agencyInfo = DataServicesCore.BusinessLogic.Diamond.AgencyInformation.GetAgencyInformationByPolicyNumber(policyNumber);
                    if (agencyInfo?.AgencyId > 0)
                    {
                        sr.ResponseData = agencyInfo;
                        CodeOk();
                    }
                    else
                    {
                        CodeNotFound();
                        sr.Messages.CreateErrorMessage($"No agency info found. Sent {policyNumber}");
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
                //sr.Messages.CreateErrorMessage("No agency code provided");
                //updated 5/30/2020
                sr.Messages.CreateErrorMessage("No policy number provided");
            }

            return Json(sr);
        }
    }
}