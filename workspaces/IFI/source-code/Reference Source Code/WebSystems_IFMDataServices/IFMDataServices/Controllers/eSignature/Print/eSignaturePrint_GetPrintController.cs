using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using IFM.DataServicesCore;

namespace IFM.DataServices.Controllers.eSignature.Print
{
    [RoutePrefix("eSignature/Print")]
    public class eSignaturePrint_GetPrintController : BaseController
    {
        [AcceptVerbs(HttpVerbs.Get)]
        [Route("GetPrintForEsignature/{policyNumber?}/{printFormCategories?}")]
        public JsonResult GetPrintForEsignature(string policyNumber, string printFormCategories)
        {
            var sr = this.CreateServiceResult();

            if (String.IsNullOrWhiteSpace(policyNumber) == false)
            {
                policyNumber = policyNumber.Trim(); //Had a scenario where a valid policy number came through but with a space afterwards "PPA1234567 "... could other things go wrong if this happens?
                if (policyNumber.IsPolicyNumber())
                {
                    printFormCategories = printFormCategories.Replace(" ", "");
                    string[] formCategoriesSplit = printFormCategories.Split(',');

                    if (formCategoriesSplit.Contains("0") == false)
                    {
                        printFormCategories += ",0";
                    }

                    var printFormBytes = DataServicesCore.BusinessLogic.Diamond.Print.GetSpecificPrintFormBytes(policyNumber, printFormCategories);
                    
                    if (printFormBytes?.Length > 0)
                    {
                        sr.ResponseData = printFormBytes;
                    }
                    else
                    {
                        sr.Messages.CreateErrorMessage($"No print documents found for {policyNumber}");
                    }
                }
                else
                {
                    sr.Messages.CreateErrorMessage($"Invalid policy number format. Sent {policyNumber}");
                }
                CodeOk();
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage($"No policy number provided.");
            }

            return Json(sr);
        }
    }
}