using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using IDSC = IFM.DataServicesCore;
using IFM.PrimitiveExtensions;

namespace IFM.DataServices.Controllers.Diamond.Claim
{
    [RoutePrefix("Diamond/Claim")]
    public class ClaimController : BaseController
    {
        [AcceptVerbs(HttpVerbs.Post)]
        [Route("FNOLClaim")]
        public JsonResult FNOLClaim(DataServicesCore.CommonObjects.OMP.FNOL FNOL) 
        {
            var sr = this.CreateServiceResult();
            if (FNOL != null && FNOL.PolicyNumber.HasValue())
            {
                sr.ResponseData = IDSC.BusinessLogic.Diamond.Claim.SubmitClaim(FNOL);
                   if (sr.ResponseData == null)
                {
                    sr.Messages.CreateErrorMessage("Claim submission was not succesful");
                }
                CodeOk();
            }
            else
                CodeBadRequest();
            return Json(sr);
        }
    }
}
