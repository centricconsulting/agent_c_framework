using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace IFM.DataServices.Controllers.IFM.StaticData
{
    [RoutePrefix("IFM/StaticData/Diamond")]
    public class IFMStaticData_DiamondController : BaseController
    {

        [AcceptVerbs(HttpVerbs.Post)]
        [Route("properties")]
        public JsonResult GetMultipleProperties(List<string> propertyNames)
        {
            var sr = this.CreateServiceResult();
            if (propertyNames != null && propertyNames.Any())
            {
                sr.ResponseData = global::IFM.DataServicesCore.BusinessLogic.OMP.StaticDataQuery.QueryMultipleTerms(propertyNames);
                CodeOk();
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No property names provided.");
            }


            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Route("properties/{propertyName}")]
        public JsonResult GetProperties(string propertyName)
        {
            var sr = this.CreateServiceResult();
            if (string.IsNullOrWhiteSpace(propertyName) == false)
            {
                sr.ResponseData = new global::IFM.DataServicesCore.BusinessLogic.OMP.StaticDataQuery(propertyName);
            }
            else
            {
                sr.Messages.CreateErrorMessage("No property name null or empty.");
            }
            CodeOk();
            return Json(sr);
        }

    }
}