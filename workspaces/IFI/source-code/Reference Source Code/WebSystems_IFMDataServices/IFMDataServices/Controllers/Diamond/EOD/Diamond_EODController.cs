using DevExpress.Utils;
using IFM.DataServices.Controllers;
using IFM.DataServicesCore.BusinessLogic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IFM.DataServices.Controllers.Diamond.EOD
{
    [RoutePrefix("Diamond/EOD")]
    public class Diamond_EODController : BaseController
    {
        private const string location = "IFMDataServices.Diamond_EODController";

        [AcceptVerbs(HttpVerbs.Post)]
        [Route("EODFinished")]
        public async Task<JsonResult> EODFinishedAsync(global::IFM.DataServicesCore.CommonObjects.Diamond.EODFinishedNotification eodNotification)
        {
            var sr = this.CreateServiceResult();
            
            var webRequest = (HttpWebRequest)System.Net.WebRequest.Create(AppConfig.SnapLogicEodUrl);
            webRequest.ContentType = "application/json";
            webRequest.Method = "POST";
            
            using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(eodNotification);
                streamWriter.Write(json);
            }

            try
            {
                var returnVar = await webRequest.GetResponseAsync();
            }
            catch(Exception ex)
            {
                IFMErrorLogging.LogException(ex, "Error sending request or receiving response." ,location + ".EODFinishedAsync");
            }

            sr.ResponseData = true;

            CodeOk();

            return Json(sr);
        }
    }
}