using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IFM.DataServices.Controllers.VR
{
    

    [RoutePrefix("VR/QuoteTransfer")]
    public class TransferQuoteController : BaseController
    {

        [AcceptVerbs(HttpVerbs.Get)]
        [Route("AquireByUserId/{policyId}/{userId}")]
        public JsonResult AquireByUserId(int policyId, int userId)
        {
            var sr = this.CreateServiceResult();

            if (policyId > 0 && userId > 0)
            {
                var response = global::IFM.DataServicesCore.BusinessLogic.Diamond.Policy.QuoteUserTransfer.TransferQuoteToUser(policyId,1,userId);
                if (response != null)
                {
                    if (!response.DiamondValidation.HasAnyItems())
                    {
                        CodeOk();
                    }
                    else
                    {
                        sr.Messages.CreateErrorMessage("Service call had validation items.");
                        CodeBadRequest();
                    }
                    
                }
                else
                {
                    sr.Messages.CreateErrorMessage("No response from diamond service.");
                    CodeBadRequest();
                }
                
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No policyId or userId provided");
            }

            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Route("TransferQuoteToAgencyUser/{quoteNumber}/{agencyCode}")]
        public JsonResult TransferQuoteToAgencyUser(string quoteNumber, string agencyCode)
        {
            var sr = this.CreateServiceResult();

            if (string.IsNullOrWhiteSpace(quoteNumber) == false && string.IsNullOrWhiteSpace(agencyCode) == false)
            {
                var response = global::IFM.DataServicesCore.BusinessLogic.Diamond.Policy.QuoteUserTransfer.TransferQuoteToAgency(quoteNumber,agencyCode);
                if (response != null)
                {
                    if (!response.DiamondValidation.HasAnyItems())
                    {
                        CodeOk();
                    }
                    else
                    {
                        sr.Messages.CreateErrorMessage("Service call had validation items.");
                        CodeBadRequest();
                    }

                }
                else
                {
                    sr.Messages.CreateErrorMessage("No response from diamond service.");
                    CodeBadRequest();
                }

            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No quotenumber or agencycode provided");
            }

            return Json(sr);
        }

    }

}