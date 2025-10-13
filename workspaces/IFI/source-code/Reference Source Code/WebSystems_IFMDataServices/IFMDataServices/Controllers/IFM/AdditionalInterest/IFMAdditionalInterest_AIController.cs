using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IFM.PrimitiveExtensions;

namespace IFM.DataServices.Controllers.IFM.AdditionalInterest
{
    [RoutePrefix("IFM/AdditionalInterest")]
    public class IFMAdditionalInterest_AIController : BaseController
    {
        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("AILookupUniquesOnly")]
        public JsonResult AILookupUniquesOnly(DataServicesCore.CommonObjects.OMP.AdditionalInterest ai)
        {
            var sr = this.CreateServiceResult();
            sr.ResponseData = global::IFM.DataServicesCore.BusinessLogic.Diamond.AdditionalInterestHelper.AdditionalInterestLookup(ai, true);
            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("AILookupAll")]
        public JsonResult AILookupAll(DataServicesCore.CommonObjects.OMP.AdditionalInterest ai)
        {
            var sr = this.CreateServiceResult();
            sr.ResponseData = global::IFM.DataServicesCore.BusinessLogic.Diamond.AdditionalInterestHelper.AdditionalInterestLookup(ai, false);
            return Json(sr);
        }
    }
}