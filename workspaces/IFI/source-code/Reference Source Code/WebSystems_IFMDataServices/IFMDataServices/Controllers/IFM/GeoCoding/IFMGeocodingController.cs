using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace IFM.DataServices.Controllers.IFM.GeoCoding
{
    [RoutePrefix("IFM/GeoCoding")]
    public class IFMGeocodingController : BaseController
    {

        [AcceptVerbs(HttpVerbs.Get)]
        [Route("AddressInformation")]
        public JsonResult AddressInformation(string address)
        {
            var sr = this.CreateServiceResult();

            if (string.IsNullOrWhiteSpace(address.Trim()) == false)
            {
                CodeOk();
                sr.ResponseData = IfmGeoCoder.GeoCoder.LookupAddress(address); // can be cast to IFM.DataServicesCore.CommonObjects.IFM.GeoCoding.GeoCodeLookupResult to avoid geocoding dll reference
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage($"Invalid Address format. Sent {address}");
            }


            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Route("StraightLineDistance")]
        public JsonResult StraigthLineDistance(string address1, string address2)
        {
            var sr = this.CreateServiceResult();

            if (string.IsNullOrWhiteSpace(address1.Trim()) == false && string.IsNullOrWhiteSpace(address2.Trim()) == false)
            {
                try
                {
                    var ad1 = IfmGeoCoder.GeoCoder.LookupAddress(address1);
                    var ad2 = IfmGeoCoder.GeoCoder.LookupAddress(address2);
                    CodeOk();
                    sr.ResponseData = IfmGeoCoder.GeoCoder.FindDistanceBetweenTwoPoints(ad1.Coordinates, ad2.Coordinates);
                }
                catch (Exception ex)
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    sr.Messages.CreateErrorMessage($"Geocoding failed. Sent '{address1}' and '{address2}'");
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage($"Invalid Address(es). Sent '{address1}' and '{address2}'");
            }


            return Json(sr);
        }

    }
}