using IFM.DataServicesCore.CommonObjects.OMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace IFM.DataServices.Controllers.OMP.Claims
{
    [RoutePrefix("OMP/Claim")]
    public class OMP_ClaimsController : BaseController
    {
        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("ClaimReport")]
        public JsonResult GetRegisteredAccountPolicies(ClaimReport report)
        {
            var sr = this.CreateServiceResult();

#if DEBUG
            if (report == null || string.IsNullOrWhiteSpace(report.Name))
            {
                var systemDate = new global::IFM.DataServicesCore.BusinessLogic.Diamond.SystemDate().GetSystemDate();
                report = new ClaimReport
                {
                    EmailAddress = "fake@site.com",
                    InjuriesExist = false,
                    LossDateTime = systemDate,
                    IpAddress = "no ip",
                    LossDescription = "Loss description",
                    LossType = global::IFM.DataServicesCore.CommonObjects.Enums.Enums.ClaimLossType.Auto_Personal,
                    Name = "Fake Guy",
                    PhoneNumber = "(317) 555-1234",
                    PolicyNumber = "PPA1234567"
                };
            }
#endif

            if (report != null)
            {
                Validation.ClaimReportValidator val = new Validation.ClaimReportValidator();
                var valResult = val.Validate(report);

                if (valResult.IsValid)
                {
                    global::IFM.DataServicesCore.BusinessLogic.OMP.ClaimsManager cm = new global::IFM.DataServicesCore.BusinessLogic.OMP.ClaimsManager();
                    sr.ResponseData = cm.SubmitClaim(report);

                    if (Convert.ToBoolean(sr.ResponseData))
                    {
                    }
                    else
                    {
                        sr.Messages.CreateErrorMessage("Email failed to send.");
                    }
                }
                else
                {
                    this.AddFluentErrorsToServiceResult(sr, valResult.Errors);
                }

                CodeOk();
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("ClaimReport is empty.");
            }


            return Json(sr);
        }

    }
}