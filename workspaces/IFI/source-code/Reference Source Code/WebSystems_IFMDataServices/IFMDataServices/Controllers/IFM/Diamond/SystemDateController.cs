using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IFM.DataServices.Controllers.IFM.Diamond
{
    [RoutePrefix("IFM/Diamond")]
    public class SystemDateController : BaseController
    {
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(Duration = 60, VaryByParam = "none")]
        [Route("systemDate")]
        public JsonResult GetSystemDate()
        {
            var sr = this.CreateServiceResult();

            sr.ResponseData = global::IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate().ToShortDateString();

            return Json(sr);
        }

    }
}