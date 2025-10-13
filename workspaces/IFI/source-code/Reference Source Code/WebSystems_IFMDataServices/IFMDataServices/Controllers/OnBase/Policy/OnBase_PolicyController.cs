using IFM.DataServicesCore.BusinessLogic.OnBase;
using IFM.DataServicesCore.CommonObjects.OnBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace IFM.DataServices.Controllers.OnBase.Policy
{
    [RoutePrefix("OnBase/Policy")]
    public class OnBase_PolicyController : BaseController
    {
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("policynumberV1/{policyNumber}")]
        public JsonResult GetPolicyInformation(string policyNumber)
        {            
            PolicyLookup pl = new PolicyLookup();

            var sr = this.CreateServiceResult();

            if (policyNumber != null && policyNumber != "")
            {
                var policyInfo = pl.LoadPolicy(policyNumber);
                if (policyInfo != null)
                {
                    CodeOk();
                    return Json(policyInfo);
                }
                else
                {
                    CodeNotFound();
                }
            }
            else
            {                
                CodeBadRequest();
            }
            return Json(null);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("policynumber/{policyNumber}")]
        public JsonResult GetPolicyInformationV2(string policyNumber)
        {
            PolicyLookup pl = new PolicyLookup();

            var sr = this.CreateServiceResult();

            if (policyNumber != null && policyNumber != "")
            {
                var policyInfo = pl.LoadPolicyV2(policyNumber);
                if (policyInfo != null)
                {
                    CodeOk();
                    return Json(policyInfo);
                }
                else
                {
                    CodeNotFound();
                }
            }
            else
            {
                CodeBadRequest();
            }
            return Json(null);
        }
    }
}