using IFM.DataServicesCore.BusinessLogic.OnBase;
using IFM.DataServicesCore.CommonObjects.OnBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace IFM.DataServices.Controllers.OnBase.ClaimantList
{
    [RoutePrefix("OnBase/ClaimantList")]
    public class OnBase_ClaimantListController : BaseController
    {


        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("claimnumber/{claimNumber}")]
        public JsonResult GetClaimantList(string claimNumber)
        {

            if (!string.IsNullOrWhiteSpace(claimNumber))
            {
                ClaimantListLookup cl = new ClaimantListLookup();
                var claimantList = cl.LoadClaimantList(claimNumber);
                if (claimantList != null)
                {
                    CodeOk();
                    return Json(claimantList);
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