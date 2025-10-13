using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace IFM.DataServices.Controllers.IFM.Auto
{
    [RoutePrefix("IFM/Auto")]
    public class IFMAuto_VinLookupsController : BaseController
    {
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(Duration = 30, VaryByParam = "*")]
        [Route("ModelInfoByVin")]
        public JsonResult ModelsByVin(string vin, int versionId = 128)
        {
            var sr = this.CreateServiceResult();
            CodeOk();
            if (string.IsNullOrWhiteSpace(vin) == false)
            {
                var systemDate = new global::IFM.DataServicesCore.BusinessLogic.Diamond.SystemDate().GetSystemDate();
                sr.ResponseData = DataServicesCore.CommonObjects.IFM.Auto.VinLookup.GetMakeModelYearOrVinVehicleInfo(HttpContext.Server.UrlDecode(vin), "", "", 0, systemDate,versionId);
            }
            return Json(sr);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(Duration = 30, VaryByParam = "*")]
        [Route("ModelInfoByYearMakeModel")]
        public JsonResult ModelsByMakeYear(Int32 year, string make, string model, int versionId = 128)
        {
            var sr = this.CreateServiceResult();
            CodeOk();
            if (year > 0)
            {
                // can cast to List<global::IFM.DataServicesCore.CommonObjects.IFM.Auto.VinLookupResult>
                var systemDate = new global::IFM.DataServicesCore.BusinessLogic.Diamond.SystemDate().GetSystemDate();
                sr.ResponseData = DataServicesCore.CommonObjects.IFM.Auto.VinLookup.GetMakeModelYearOrVinVehicleInfo("", HttpContext.Server.UrlDecode(make), HttpContext.Server.UrlDecode(model), year, systemDate,versionId);
            }
            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(Duration = 30, VaryByParam = "*")]
        [Route("GetMakes")]
        public JsonResult GetMakes(int year, int versionId = 128)
        {
            var sr = this.CreateServiceResult();
            CodeOk();
            if (versionId > 0)
            {
                var ymmLookup = new DataServicesCore.CommonObjects.IFM.Auto.YearMakeModelLookup(year, versionId);
                sr.ResponseData = ymmLookup.GetMakes().ToList(); //switch to list to avoid Dictionary/Json bug
            }
            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(Duration = 30, VaryByParam = "*")]
        [Route("GetModels")]
        public JsonResult GetModels(int year, string make, int versionId = 128)
        {
            var sr = this.CreateServiceResult();
            CodeOk();
            if (versionId > 0)
            {
                var ymmLookup = new DataServicesCore.CommonObjects.IFM.Auto.YearMakeModelLookup(year, make, versionId);
                sr.ResponseData = ymmLookup.GetModels();
            }
            return Json(sr);
        }
    }

}