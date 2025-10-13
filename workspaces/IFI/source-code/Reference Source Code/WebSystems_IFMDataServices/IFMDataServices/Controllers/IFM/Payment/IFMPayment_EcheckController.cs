using System.Web.Mvc;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Controllers.IFM.Payment
{
    [RoutePrefix("IFM/Payment/Echeck")]
    public class IfmPaymentEcheckController : BaseController
    {

        [AcceptVerbs(HttpVerbs.Get)]
        [Route("routingnumbers/{routingNumber?}")]
        public JsonResult GetBankName(string routingNumber)
        {
            APIResponses.Common.ServiceResult sr = new APIResponses.Common.ServiceResult();

            if (routingNumber.IsNullEmptyOrWhitespace() == false && routingNumber.Length == 9)
            {
                sr.ResponseData = global::IFM.DataServicesCore.BusinessLogic.Payments.eCheck.EcheckProcessor.GetBankNameFromAbaLookUp(routingNumber);
                CodeOk();
                if (string.IsNullOrWhiteSpace(sr.ResponseData.ToString()))
                {
                    sr.Messages.CreateErrorMessage("Routing number not found.");
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage($"Invalid routing number must be 9 numeric digits. Sent {routingNumber}");
            }

            return Json(sr);
        }
    }
}