using IFM.DataServicesCore.BusinessLogic.OnBase;
using IFM.DataServicesCore.CommonObjects.OnBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace IFM.DataServices.Controllers.OnBase.Provider
{
    [RoutePrefix("OnBase/Provider")]
    public class OnBase_ProviderController : BaseController
    {


        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("providername/{providerName}")]
        public JsonResult GetProviderInformation(string providerName)
        {
            ProviderLookup pl = new ProviderLookup();

            if (providerName != null && providerName != "")
            {
                var providerFein = pl.LoadProviderFein(providerName);
                if (providerFein != null)
                {
                    CodeOk();
                    return Json(providerFein);
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