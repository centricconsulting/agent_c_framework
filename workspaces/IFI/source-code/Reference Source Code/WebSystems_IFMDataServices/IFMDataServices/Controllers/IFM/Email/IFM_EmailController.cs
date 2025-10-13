using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IFM.DataServices.Controllers.IFM.Email
{
    [RoutePrefix("IFM/Email")]
    public class IFM_EmailController : BaseController
    {

        [AcceptVerbs(HttpVerbs.Post)]
        [Route("document")]
        public JsonResult SendEmail(global::IFM.DataServicesCore.CommonObjects.EmailDocument email)
        {            
            var sr = this.CreateServiceResult();

            Validation.EmailDocumentValidator val = new Validation.EmailDocumentValidator();
            var valResult = val.Validate(email);

            if (valResult.IsValid)
            {
                sr.ResponseData = global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(email.ToAddress, string.IsNullOrWhiteSpace(email.FromAddress) ? "noreply@indianafarmers.com" : email.FromAddress, email.Subject, email.Body);

                if (!(bool)sr.ResponseData)
                {
                    sr.Messages.CreateErrorMessage("Failed to send email.");                    
                }
                
            }
            else
            {
                this.AddFluentErrorsToServiceResult(sr, valResult.Errors);                
                sr.Messages.CreateErrorMessage("Failed to send email.");
                sr.ResponseData = false;
            }



            return Json(sr);
        }

    }
}