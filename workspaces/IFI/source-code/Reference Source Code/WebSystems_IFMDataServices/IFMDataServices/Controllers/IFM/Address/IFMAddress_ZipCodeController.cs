using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace IFM.DataServices.Controllers.IFM.Address
{
    [RoutePrefix("IFM/Address")]
    public class IFMAddress_ZipCodeController : BaseController
    {

        [AcceptVerbs(HttpVerbs.Get)]
        [Route("zipcodes/{zip?}")]
        public JsonResult GetZipInfo(string zip)
        {
            var sr = this.CreateServiceResult();

            if (string.IsNullOrWhiteSpace(zip) == false)
            {
                if (zip.IsValidZipCode())
                {
                    // can directly cast to global::IFM.DataServicesCore.CommonObjects.IFM.Address.ZipCodeLookupResult
                    sr.ResponseData = global::IFM.VR.Common.Helpers.GetCityCountyFromZipCode.GetCityCountyFromZipCode(zip);
                }
                else
                {
                    sr.Messages.CreateErrorMessage($"Invalid ZIP code format. Sent {zip}");
                }
                CodeOk();
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No ZIP code provided");
            }

            return Json(sr);
        }

    }

}