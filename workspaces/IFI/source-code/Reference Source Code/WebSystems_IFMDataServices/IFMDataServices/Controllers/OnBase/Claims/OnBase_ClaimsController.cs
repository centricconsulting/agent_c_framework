using IFM.DataServicesCore.BusinessLogic.OnBase;
using IFM.DataServicesCore.CommonObjects.OnBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace IFM.DataServices.Controllers.OnBase.Claims
{
    [RoutePrefix("OnBase/Claim")]
    public class OnBase_ClaimsController : BaseController
    {
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("claimnumberV1/{claimNumber}")]
        public JsonResult GetClaimInformation(string claimNumber)
        {
            ClaimLookup cl = new ClaimLookup();

            if (claimNumber != null && claimNumber != "")
            {
                var claimInfo = cl.LoadClaim(claimNumber);
                if (claimInfo != null)
                {
                    CodeOk();
                    return Json(claimInfo);
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
        [Route("claimnumber/{claimNumber}")]
        public JsonResult GetClaimInformationV2(string claimNumber)
        {
            ClaimLookup cl = new ClaimLookup();

            if (claimNumber != null && claimNumber != "")
            {
                var claimInfo = cl.LoadClaimV2(claimNumber);
                if (claimInfo != null)
                {
                    CodeOk();
                    return Json(claimInfo);
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