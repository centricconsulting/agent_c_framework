using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IFM.DataServices.Controllers.VR.PayplanFators
{
    [RoutePrefix("VR/PayPlan")]
    public class VR_PayplanFactorsController : BaseController
    {
        [AcceptVerbs(HttpVerbs.Get)]
        [Route("Factors/{lobid}")]
        public JsonResult GetFactors(int lobId)
        {
            var factors = new List<global::IFM.DataServicesCore.CommonObjects.VR.Payplan.PayplanFactor>();
            // can cast to global::IFM.DataServicesCore.CommonObjects.VR.Payplan.PayplanFactor
            switch (lobId)
            {
                case 1: // ppa
                    CodeOk();
                    foreach (var pair in global::IFM.VR.Common.Helpers.PPA.PPA_Payplans.GetPayplanFactors(true))
                    {
                        factors.Add(new global::IFM.DataServicesCore.CommonObjects.VR.Payplan.PayplanFactor()
                        {
                            PayPlanId = pair.Key,
                            Factor = pair.Value
                        });
                    }                    
                    break;
                default:
                    CodeBadRequest();
                    break;

            }
            return Json(factors);
            
        }
    }
}