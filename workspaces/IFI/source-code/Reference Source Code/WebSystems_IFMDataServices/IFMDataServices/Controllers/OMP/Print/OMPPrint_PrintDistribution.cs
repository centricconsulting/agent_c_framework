

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace IFM.DataServices.Controllers.OMP.Print
{
    [RoutePrefix("OMP/Print/Distribution")]
    public class OMPPrint_PrintDistributionController : BaseController
    {
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("PolicyPrintDistribution/{policyId}")]
        public JsonResult LoadPolicyPrintDistribution(int policyId)
        {
            var sr = this.CreateServiceResult();
            sr.ResponseData = global::IFM.DataServicesCore.BusinessLogic.OMP.PrintDistribution.LoadPolicyPrintDistribution(policyId);
            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("PolicyPrintDistribution")]
        public JsonResult SavePolicyPrintDistribution(DataServicesCore.CommonObjects.OMP.PrintDistribution newSettings)
        {
            return Json(DataServicesCore.BusinessLogic.OMP.PrintDistribution.SavePolicyPrintDistributionSR(newSettings));            
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("BulkLoadPolicyPrintDistribution")]
        public JsonResult BulkLoadPolicyPrintDistribution(List<int> policyId)
        {
            var sr = this.CreateServiceResult();
            sr.ResponseData = global::IFM.DataServicesCore.BusinessLogic.OMP.PrintDistribution.BulkLoadPolicyPrintDistribution(policyId);
            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("BulkSavePolicyPrintDistribution")]
        public JsonResult BulkSavePolicyPrintDistribution(List<DataServicesCore.CommonObjects.OMP.PrintDistribution> newSettings)
        {
            return Json(DataServicesCore.BusinessLogic.OMP.PrintDistribution.BulkSavePolicyPrintDistributionSR(newSettings));
        }
    }

}