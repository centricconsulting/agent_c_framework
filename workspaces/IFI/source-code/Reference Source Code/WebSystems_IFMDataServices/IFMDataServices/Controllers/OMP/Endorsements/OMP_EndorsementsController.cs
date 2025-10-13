using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IFM.DataServicesCore;
using IFM.DataServicesCore.BusinessLogic;
using IFM.PrimitiveExtensions;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Controllers.OMP.Endorsements
{
    [RoutePrefix("OMP/Endorsements")]
    public class OMP_EndorsementsController : BaseController
    {
        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("Endorsement")]
        public JsonResult Endorsement(DataServicesCore.CommonObjects.OMP.Endorsement endorsement)
        {
            var sr = this.CreateServiceResult();
            if (DataServicesCore.BusinessLogic.OMP.EndorsementsValidator.ValidateEndorsementInfo(endorsement))
            {
                int myTransactionHistoryId = SQL_StoreRequest(endorsement);
                if (myTransactionHistoryId.HasValue())
                {
                    System.Web.HttpContext.Current.Session.Add("endorsementTransHistoryId", myTransactionHistoryId);
                }

                DataServicesCore.CommonObjects.OMP.Endorsement processedEndorsement = DataServicesCore.BusinessLogic.OMP.Endorsements.ProcessEndorsement(endorsement);
                CodeOk();
                sr.ResponseData = processedEndorsement;

                //Could have combine with if statement below but it is much easier to read what is happening like this...
                bool rateOrFinalizeError = false;
                if (endorsement.TransactionType == DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementTransactionType.Rate && processedEndorsement.EndorsementStatus.RateSuccessful == false)
                {
                    rateOrFinalizeError = true;
                }
                else if (endorsement.TransactionType == DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementTransactionType.Finalize && (processedEndorsement.EndorsementStatus.FinalizeSuccessful == false || processedEndorsement.EndorsementStatus.PromoteSuccessful == false))
                {
                    rateOrFinalizeError = true;
                }

                if(String.IsNullOrWhiteSpace(processedEndorsement.ErrorMessage) == false)
                {
                    sr.HasErrors = true;
                    sr.Messages.CreateErrorMessage(processedEndorsement.ErrorMessage);
                }
                else if(rateOrFinalizeError == true)
                {
                    sr.HasErrors = true;
                    if (processedEndorsement.ValidationItems.IsLoaded())
                    {
                        foreach(var vi in processedEndorsement.ValidationItems)
                        {
                            if(vi.ValidationSeverityTypeId == 1)
                            {
                                sr.Messages.CreateErrorMessage(vi.Message);
                            }
                        }
                    }
                }
                SQL_StoreResponse(processedEndorsement, myTransactionHistoryId, sr.Messages);
            }
            else
            {
                CodeOk();
                sr.Messages.CreateErrorMessage(endorsement.ErrorMessage);
            }

            return Json(sr);
        }

        private static int SQL_StoreRequest(DataServicesCore.CommonObjects.OMP.Endorsement endo)
        {
            CommonHelperClass chc = new CommonHelperClass();
            bool doStoreRequest = chc.GetApplicationXMLSettingForBoolean("API_StoreRequestAndReponseJSON", "APISettings.xml");
            if (doStoreRequest)
            {
                string requestJSON = "";
                int apiJsonRequestId = 0;

                //Finalize and Delete send very little information through JSON. We will be capturing all the main info below.
                if (endo.TransactionType == DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementTransactionType.Rate)
                    requestJSON = Newtonsoft.Json.JsonConvert.SerializeObject(endo);

                if (System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session["ApiJsonRequestID"] != null && System.Web.HttpContext.Current.Session["ApiJsonRequestID"].ToString().HasValue() && System.Web.HttpContext.Current.Session["ApiJsonRequestID"].ToString().IsNumeric())
                    apiJsonRequestId = System.Web.HttpContext.Current.Session["ApiJsonRequestID"].ToString().TryToGetInt32();

                System.Collections.ArrayList myParams = new System.Collections.ArrayList();
                if(endo.Username.HasValue())
                    myParams.Add(new System.Data.SqlClient.SqlParameter("@mpUser", endo.Username));
                myParams.Add(new System.Data.SqlClient.SqlParameter("@policyNumber", endo.PolicyNumber));
                myParams.Add(new System.Data.SqlClient.SqlParameter("@policyID", endo.PolicyId));
                myParams.Add(new System.Data.SqlClient.SqlParameter("@policyImageNum", endo.PolicyImageNum));
                if(endo.TransactionDate.HasValue())
                    myParams.Add(new System.Data.SqlClient.SqlParameter("@endorsementTransDate", endo.TransactionDate));
                myParams.Add(new System.Data.SqlClient.SqlParameter("@transTypeID", endo.TransactionType));
                if(endo.ObjectType != DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementObjectType.NA)
                    myParams.Add(new System.Data.SqlClient.SqlParameter("@objectTypeID", endo.ObjectType));
                if(endo.ActionType != DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementActionType.NA)
                    myParams.Add(new System.Data.SqlClient.SqlParameter("@actionTypeID", endo.ActionType));
                if(requestJSON.HasValue())
                    myParams.Add(new System.Data.SqlClient.SqlParameter("@requestJSON", requestJSON));
                if (apiJsonRequestId.HasValue())
                    myParams.Add(new System.Data.SqlClient.SqlParameter("@IFMDataServicesRequestID", apiJsonRequestId));

                System.Data.SqlClient.SqlParameter myOutputParam = new System.Data.SqlClient.SqlParameter("@transHistoryID", System.Data.SqlDbType.Int);
                SQLexecuteObject sqle = new SQLexecuteObject(AppConfig.ConnDiamondReports, "dbo.usp_MemberPortalEndorsements_StoreRequest", myParams, myOutputParam);
                sqle.ExecuteStatement();
                if (myOutputParam != null && myOutputParam.Value != null && myOutputParam.Value.ToString().HasValue())
                {
                    return myOutputParam.Value.ToString().TryToGetInt32();
                }
            }
            return 0;
        }

        private static void SQL_StoreResponse(DataServicesCore.CommonObjects.OMP.Endorsement endo, int historyId, APIResponses.Common.MessagesList errorMessages)
        {
            CommonHelperClass chc = new CommonHelperClass();
            bool doStoreResponse = chc.GetApplicationXMLSettingForBoolean("API_StoreRequestAndReponseJSON", "APISettings.xml");
            if (doStoreResponse)
            {
                string errorMsg = "";
                if (errorMessages.IsLoaded())
                {
                    bool first = true;
                    int counter = 0;
                    foreach(var msg in errorMessages)
                    {
                        counter++;
                        if (msg.MessageText.HasValue())
                        {
                            string myError = $"{counter}) {msg.MessageText}";
                            if (first)
                            {
                                errorMsg = myError;
                                first = false;
                            }
                            else
                                errorMsg += $"; {myError}";
                        }
                    }
                }

                string responseJSON = "";
                if(endo.TransactionType == DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementTransactionType.Rate)
                {
                    responseJSON = Newtonsoft.Json.JsonConvert.SerializeObject(endo);
                }

                System.Collections.ArrayList myParams = new System.Collections.ArrayList();
                myParams.Add(new System.Data.SqlClient.SqlParameter("@transHistoryID", historyId));
                if(responseJSON.HasValue())
                    myParams.Add(new System.Data.SqlClient.SqlParameter("@responseJSON", responseJSON));
                if (errorMsg.HasValue())
                    myParams.Add(new System.Data.SqlClient.SqlParameter("@responseError", errorMsg));
                else
                    myParams.Add(new System.Data.SqlClient.SqlParameter("@isSuccess", "1"));

                if(endo.PolicyImageNum.HasValue())
                    myParams.Add(new System.Data.SqlClient.SqlParameter("@policyImageNum", endo.PolicyImageNum));

                SQLexecuteObject sqle = new SQLexecuteObject(AppConfig.ConnDiamondReports, "usp_MemberPortalEndorsements_StoreResponse", myParams);
                sqle.ExecuteStatement();
            }
        }

        //JUST FOR TESTING PROMOTING POLICIES SEPERATELY - NOT REALLY MEANT TO BE A STANDALONE SERVICE YET.... BUT IT COULD BE!
        //[AcceptVerbs(HttpVerbs.Post)]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        //[Route("EndorsementPromote")]
        //public JsonResult EndorsementPromote(DataServicesCore.CommonObjects.OMP.Endorsement endorsement)
        //{
        //    var sr = this.CreateServiceResult();
        //    if (DataServicesCore.BusinessLogic.OMP.EndorsementsValidator.ValidateEndorsementInfo(endorsement))
        //    {
        //        DataServicesCore.CommonObjects.OMP.Endorsement processedEndorsement = DataServicesCore.BusinessLogic.OMP.Endorsements.PromoteEndorsement(endorsement);
        //        CodeOk();
        //        sr.ResponseData = processedEndorsement;
        //        if (String.IsNullOrWhiteSpace(processedEndorsement.ErrorMessage) == false)
        //        {
        //            sr.HasErrors = true;
        //            sr.Messages.CreateErrorMessage(processedEndorsement.ErrorMessage);
        //        }
        //        else if (processedEndorsement.EndorsementStatus.RateSuccessful == false)
        //        {
        //            sr.HasErrors = true;
        //            if (processedEndorsement.ValidationItems.IsLoaded())
        //            {
        //                foreach (var vi in processedEndorsement.ValidationItems)
        //                {
        //                    if (vi.ValidationSeverityTypeId == 1)
        //                    {
        //                        sr.Messages.CreateErrorMessage(vi.Message);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        CodeBadRequest();
        //        sr.Messages.CreateErrorMessage(endorsement.ErrorMessage);
        //    }

        //    return Json(sr);
        //}
    }
}