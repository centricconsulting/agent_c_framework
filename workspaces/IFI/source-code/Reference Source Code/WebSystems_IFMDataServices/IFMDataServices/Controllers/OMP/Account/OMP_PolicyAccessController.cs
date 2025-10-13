using IFM.DataServicesCore.BusinessLogic.OMP;
using IFM.DataServicesCore.CommonObjects.OMP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace IFM.DataServices.Controllers.OMP.Account
{
    [RoutePrefix("OMP/Account")]
    public class OMP_PolicyAccessController : BaseController
    {

        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("accountpolicies")]
        public JsonResult GetRegisteredAccountPolicies(List<MemberAccountPolicy> policyList)
        {
            var sr = this.CreateServiceResult();

            if (policyList != null)
            {
                Validation.MemberAccountPolicyValidator val = new Validation.MemberAccountPolicyValidator();
                FluentValidation.Results.ValidationResult valResult = new FluentValidation.Results.ValidationResult();
                foreach (var p in policyList)
                {
                    var v = val.Validate(p);
                    foreach (var Er in v.Errors)
                    {
                        valResult.Errors.Add(Er);
                    }
                }
                if (valResult.IsValid)
                {
                    sr.ResponseData = PolicyAccess.GetRegisteredAccountPoliciesAsync(policyList);
                }
                //Me.AddFluentErrorsToServiceResult(sr, valResult?.Errors)
                CodeOk();
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("Account list empty");

            }

            return Json(sr);
        }




        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("policies/{policyId}/{imageNumber}")]
        public JsonResult GetPolicyInformation(Int32 policyId, Int32 imageNumber)
        {
            var sr = this.CreateServiceResult();

            if (policyId > 0 && imageNumber > 0)
            {
                var policyInfo = PolicyAccess.GetPolicyInformation(policyId, imageNumber);
                if (policyInfo != null)
                {
                    sr.ResponseData = policyInfo;
                }
                else
                {
                    sr.Messages.CreateErrorMessage($"No data found for policyId '{policyId}' and imageNumber '{imageNumber}'.");
                }
                CodeOk();
            }
            else
            {
                sr.Messages.CreateErrorMessage($"Invalid policyId '{policyId}' or imageNumber '{imageNumber}'.");
                CodeBadRequest();
            }

            return Json(sr);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("policyverification")]
        public JsonResult PolicyVerification(PolicyAccessVerification verificationInfo)
        {
            var sr = this.CreateServiceResult();

            if (verificationInfo != null)
            {
                Validation.PolicyAccessVerificationValidator val = new Validation.PolicyAccessVerificationValidator();
                var valResult = val.Validate(verificationInfo);
                string errorReason = "";
                if (valResult.IsValid)
                {
                    PolicyAccessVerifier verifier = new PolicyAccessVerifier();
                    sr.ResponseData = verifier.IsValidPolicyAccessVerification(verificationInfo, ref errorReason);

                    if((bool)sr.ResponseData == false)
                        sr.Messages.CreateErrorMessage(errorReason);
                }
                else
                {
                    sr.ResponseData = false;
                    this.AddFluentErrorsToServiceResult(sr, valResult.Errors);
                }

                CodeOk();
            }
            else
            {
                sr.ResponseData = false;
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No verification data provided.");
            }


            return Json(sr);
        }


    }

}