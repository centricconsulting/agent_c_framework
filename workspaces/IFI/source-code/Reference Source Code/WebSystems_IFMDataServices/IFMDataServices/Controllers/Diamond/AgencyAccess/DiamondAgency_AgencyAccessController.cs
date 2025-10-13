using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IDSC = IFM.DataServicesCore;

namespace IFM.DataServices.Controllers.Diamond.AgencyAccess
{
    [RoutePrefix("Diamond/AgencyAccess")]
    public class DiamondAgency_AgencyAccessController : BaseController
    {
        [AcceptVerbs(HttpVerbs.Get)]
        [Route("AssignMissingAgenciesToUserAsSecondaryAgencies/{DiamondUserID?}")]
        public JsonResult AssignMissingAgenciesToUserAsSecondaryAgencies(string DiamondUserID)
        {
            var sr = this.CreateServiceResult();
            int myUserID = 0;
            int.TryParse(DiamondUserID, out myUserID);
            if (myUserID > 0)
            {
                sr.ResponseData = IDSC.BusinessLogic.Diamond.AgencyAccess.AssignMissingAgenciesToUserAsSecondaryAgencies(myUserID);
            }
            else
            {
                sr.Messages.CreateErrorMessage($"DiamondUserID must be greater than 0. Sent {DiamondUserID}");
            }
            CodeOk();

            return Json(sr);
        }
    }
}