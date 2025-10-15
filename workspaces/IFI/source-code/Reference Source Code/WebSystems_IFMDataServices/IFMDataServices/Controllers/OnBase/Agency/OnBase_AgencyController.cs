using IFM.DataServicesCore.BusinessLogic.OnBase;
using IFM.DataServicesCore.CommonObjects.OnBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace IFM.DataServices.Controllers.OnBase.Agency
{
    [RoutePrefix("OnBase/Agency")]
    public class OnBase_AgencyController : BaseController
    {

        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("agencyid/{agencyId}")]
        public JsonResult GetAgencyInformationById(int agencyId)
        {
            AgencyLookup al = new AgencyLookup();

            if (agencyId != 0)
            {
                var agencyInfo = al.LoadAgency(agencyId);
                if (agencyInfo != null)
                {
                    CodeOk();
                    return Json(agencyInfo);
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
        [Route("agencycode/{agencyCode}")]
        public JsonResult GetAgencyInformationByCode(string agencyCode)
        {
            AgencyLookup al = new AgencyLookup();

            if (agencyCode != null && agencyCode != "")
            {
                var agencyInfo = al.LoadAgency(agencyCode);
                if (agencyInfo != null)
                {
                    CodeOk();
                    return Json(agencyInfo);
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