using DC = Diamond.Common;
using IFM.DataServicesCore.CommonObjects.OMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFM.PrimitiveExtensions;
using IFM.DataServicesCore.BusinessLogic.Diamond.Policy;
//using Diamond.Web.BaseControls;

namespace IFM.DataServicesCore.BusinessLogic.Diamond
{
    class PolicyQuoteProcessing : ModelBase
    {
        public static DC.Objects.Policy.Image GetDiamondImage(int policyID, int policyImageNum, List<String> errorMsgs)
        {
            if (policyID > 0 && policyImageNum > 0)
            {
                try
                {
                    using (var DS = Insuresoft.DiamondServices.PolicyService.LoadImage())
                    {
                        DS.RequestData.ImageNumber = policyImageNum;
                        DS.RequestData.PolicyId = policyID;
                        var response = DS.Invoke()?.DiamondResponse;
                        if (response?.ResponseData?.Image != null)
                        {
                            return response?.ResponseData?.Image;
                        }
                        else
                        {
                            if (response == null)
                                errorMsgs.Add("Response object came back null when attempting to get Diamond image.");
                            else if (response.ResponseData == null)
                                errorMsgs.Add("Response data in response object came back null when attempting to get Diamond image.");
                            else
                                errorMsgs.Add("Diamond image came back null.");
                        }
                    }
                }
                catch(Exception ex)
                {
                    errorMsgs.Add("Exception occurred while attempting to get Diamond image.");
                    errorMsgs.Add(ex.Message);
                }

            }
            else
            {
                errorMsgs.Add("Must provide policyID and policyImageNum.");
            }
            return null;
        }
        public static DC.Services.Messages.PolicyService.Issue.Response AcquireAndIssueQuote(int policyID, int policyImageNum, CommonObjects.Enums.Enums.PolicyQuoteProcessingType processingType, int userID, List<String> errorMsgs)
        {
            DC.Objects.Policy.Image img = null;
            if (policyID > 0 && policyImageNum > 0)
            {
                var rsp = DataServicesCore.BusinessLogic.Diamond.Policy.QuoteUserTransfer.TransferQuoteToUser(policyID, policyImageNum, userID);
                if (rsp?.ResponseData?.PolicyImage != null)
                {
                    img = rsp.ResponseData.PolicyImage;
                }
            }
            return IssueQuote(img, processingType, errorMsgs);
        }

        public static DC.Services.Messages.PolicyService.Issue.Response IssueQuote(int policyID, int policyImageNum, CommonObjects.Enums.Enums.PolicyQuoteProcessingType processingType, int userID, List<String> errorMsgs)
        {
            DC.Objects.Policy.Image img = GetDiamondImage(policyID, policyImageNum, errorMsgs);
            return IssueQuote(img, processingType, errorMsgs);
        }

        public static DC.Services.Messages.PolicyService.Issue.Response IssueQuote (DC.Objects.Policy.Image img, CommonObjects.Enums.Enums.PolicyQuoteProcessingType processingType, List<String> errorMsgs)
        {
            if(img != null)
            {
                try
                {
                    using (var DS = Insuresoft.DiamondServices.PolicyService.Issue())
                    {
                        DS.RequestData.PolicyImage = img;
                        DS.RequestData.Rate = true;
                        DS.RequestData.AlreadyValidated = true;
                        var response = DS.Invoke()?.DiamondResponse;
                        //if (response.DiamondValidation?.ValidationItems?.Count > 0)
                        //{
                        //    foreach (var vi in response.DiamondValidation.ValidationItems)
                        //    {
                        //        ValidationItems.Add(vi.Message.ScrubHTML()); //Some rules errors contain HTML in the actual message. We will use the ScrubHTML extension to attempt to get rid of that.
                        //    }
                        //}
                        if (response?.ResponseData != null)
                        {
                            return response;
                        }
                        else
                        {
                            if (errorMsgs.Count == 0)
                            {
                                if (response == null)
                                    errorMsgs.Add("An error occurred while attempting to issue the quote. No response object came back null from Diamond service.");
                                else if (response.ResponseData == null)
                                    errorMsgs.Add("An error occurred while attempting to issue the quote. No ResponseData came back in response object.");
                                else
                                    errorMsgs.Add("An error occurred while attempting to issue the quote. The operation did not complete succesfully.");

                                errorMsgs.Add("No Diamond validations were returned.");
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    errorMsgs.Add("Exception occurred while attempting to issue quote.");
                    errorMsgs.Add(ex.Message);
                    return null;
                }
            }
            return null;
        }

        public static int InsertStraightThruProcessingRecord(DataServicesCore.CommonObjects.Enums.Enums.PolicyQuoteProcessingType processingType, DataServicesCore.CommonObjects.Enums.Enums.PolicyQuotingApplication application, string policyNumber, int policyID, int policyImageNum, DateTime effectiveDate, int agencyID, string agencyCode, int userID, string username)
        {
            int loggedInID = Login.GetUserId();
            using (var sql = new SQLexecuteObject(AppConfig.ConnDiamondReports))
            {
                string STPType = processingType == CommonObjects.Enums.Enums.PolicyQuoteProcessingType.Change ? "Endorsement" : "New Business";
                switch(application)
                {
                    case CommonObjects.Enums.Enums.PolicyQuotingApplication.VR:
                        //basically just do nothing; This will keep the current pattern correct in the STP table.
                        break;
                    case CommonObjects.Enums.Enums.PolicyQuotingApplication.MemberPortal:
                        STPType = "MP " + STPType;
                        break;
                }
                if (agencyCode.IsAgencyCode_Long() == false)
                {
                    agencyCode = AgencyInformation.GetAgencyCodeByAgencyID(agencyID);
                }
                sql.queryOrStoredProc = "usp_Insert_StraightThroughProcessingRecord";
                sql.InputParameters_TypeSafe = sql.InputParameters_TypeSafe.NewIfNull();
                sql.OutputParameters_TypeSafe = sql.OutputParameters_TypeSafe.NewIfNull();

                sql.InputParameters_TypeSafe.Add(new System.Data.SqlClient.SqlParameter("@stp_type", STPType));
                sql.InputParameters_TypeSafe.Add(new System.Data.SqlClient.SqlParameter("@policy_number", policyNumber));
                sql.InputParameters_TypeSafe.Add(new System.Data.SqlClient.SqlParameter("@policy_id", policyID));
                sql.InputParameters_TypeSafe.Add(new System.Data.SqlClient.SqlParameter("@policyimage_num", policyImageNum));
                if(effectiveDate.HasValue())
                    sql.InputParameters_TypeSafe.Add(new System.Data.SqlClient.SqlParameter("@effective_date", effectiveDate));
                if(agencyID.HasValue())
                    sql.InputParameters_TypeSafe.Add(new System.Data.SqlClient.SqlParameter("@agency_id", agencyID));
                if(agencyCode.IsAgencyCode_Long())
                    sql.InputParameters_TypeSafe.Add(new System.Data.SqlClient.SqlParameter("@agency_code", agencyCode));
                if (userID.HasValue() == false)
                    userID = loggedInID;
                sql.InputParameters_TypeSafe.Add(new System.Data.SqlClient.SqlParameter("@users_id", userID));
                if (username.HasValue())
                {
                    if (userID == loggedInID)
                    {
                        sql.InputParameters_TypeSafe.Add(new System.Data.SqlClient.SqlParameter("@username", Login.GetUserName() + " - " + username));
                    }
                    else
                    {
                        sql.InputParameters_TypeSafe.Add(new System.Data.SqlClient.SqlParameter("@username", username));
                    }
                }
                sql.InputParameters_TypeSafe.Add(new System.Data.SqlClient.SqlParameter("@comment", "Issuance not attempted"));
                sql.OutputParameters_TypeSafe.Add(new System.Data.SqlClient.SqlParameter("@stp_id", System.Data.SqlDbType.Int));
                sql.ExecuteStatement();
                if (sql.rowsAffected.IsNotZero() && sql.hasError == false)
                {
                    if (sql.OutputParameters_TypeSafe[0].Value != null)
                        return sql.OutputParameters_TypeSafe[0].Value.ToString().TryToGetInt32();
                }
                return 0;
            }
        }

        public static int InsertStraightThruProcessingRecord(DataServicesCore.CommonObjects.Enums.Enums.PolicyQuoteProcessingType processingType, DataServicesCore.CommonObjects.Enums.Enums.PolicyQuotingApplication application, string policyNumber, int policyID, int policyImageNum, DateTime effectiveDate, int agencyID, int userID, string username)
        {
            var agencyCode = AgencyInformation.GetAgencyCodeByAgencyID(agencyID);
            return InsertStraightThruProcessingRecord(processingType, application, policyNumber, policyID, policyImageNum, effectiveDate, agencyID, agencyCode, userID, username);
        }

        public static int InsertStraightThruProcessingRecord(DataServicesCore.CommonObjects.Enums.Enums.PolicyQuoteProcessingType processingType, DataServicesCore.CommonObjects.Enums.Enums.PolicyQuotingApplication application, DC.Objects.Policy.Image img, int userID, string username)
        {
            return InsertStraightThruProcessingRecord(processingType, application, img.PolicyNumber, img.PolicyId, img.PolicyImageNum, img.EffectiveDate, img.AgencyId, img.Agency.Code, userID, username);
        }

        public static bool InsertStraightThruProcessingValidationItem(int stpID, int validationSeveritytypeID, string message)
        {
            using (var sql = new SQLexecuteObject(AppConfig.ConnDiamondReports))
            {
                sql.queryOrStoredProc = "usp_Insert_StraightThroughProcessingValidationItem";
                sql.InputParameters_TypeSafe = sql.InputParameters_TypeSafe.NewIfNull();
                sql.InputParameters_TypeSafe.Add(new System.Data.SqlClient.SqlParameter("@stp_id", stpID));
                if (validationSeveritytypeID.HasValue())
                    sql.InputParameters_TypeSafe.Add(new System.Data.SqlClient.SqlParameter("@validationseveritytype_id", validationSeveritytypeID));
                sql.InputParameters_TypeSafe.Add(new System.Data.SqlClient.SqlParameter("@message", message));
                sql.ExecuteStatement();
                if(sql.rowsAffected.IsNotZero() && sql.hasError == false)
                    return true;
                else
                    return false;
            }
        }

        public static bool UpdateStraightThruProcessingRecord(int stpID, bool stpSuccess, string stpComment)
        {
            using (var sql = new SQLexecuteObject(AppConfig.ConnDiamondReports))
            {
                sql.queryOrStoredProc = "usp_Update_StraightThroughProcessingRecord";
                sql.InputParameters_TypeSafe = sql.InputParameters_TypeSafe.NewIfNull();
                sql.InputParameters_TypeSafe.Add(new System.Data.SqlClient.SqlParameter("@stp_id", stpID));
                sql.InputParameters_TypeSafe.Add(new System.Data.SqlClient.SqlParameter("@success", stpSuccess == true ? true : false));
                sql.InputParameters_TypeSafe.Add(new System.Data.SqlClient.SqlParameter("@comment", stpComment));
                sql.ExecuteStatement();
                if (sql.rowsAffected.IsNotZero() && sql.hasError == false)
                    return true;
                else
                    return false;
            }
        }

        public static bool IssueQuoteAndUpdateSTPDatabase(DC.Objects.Policy.Image img, CommonObjects.Enums.Enums.PolicyQuoteProcessingType processingType, DataServicesCore.CommonObjects.Enums.Enums.PolicyQuotingApplication application, int userID, string username, List<String> errorMsgs)
        {
            bool stpSuccess = false;
            string stpComment;
            if(img != null)
            {
                int stpID = InsertStraightThruProcessingRecord(processingType, application, img, userID, username);
                if(stpID > 0)
                {
                    var rsp = IssueQuote(img, processingType, errorMsgs);
                    List<string> routeErrors = new List<string>();
                    string valItems = "";
                    if (rsp?.ResponseData != null)
                    {
                        if(rsp.ResponseData.OperationSuccessful == true)
                        {
                            stpSuccess = true;
                            stpComment = "Successful Issuance";
                        }
                        else
                        {
                            var listOfIssuanceFailedErrors = new List<string>();
                            if (rsp.DiamondValidation?.ValidationItems?.Count > 0)
                            {
                                foreach (var vi in rsp.DiamondValidation.ValidationItems)
                                {
                                    listOfIssuanceFailedErrors.Add(vi.Message);
                                    errorMsgs.Add(vi.Message);
                                    InsertStraightThruProcessingValidationItem(stpID, vi.ValidationSeverityType, vi.Message);
                                }
                            }
                            string addS = listOfIssuanceFailedErrors.Count > 1 ? "s" : "";
                            valItems = "Validation Item" + addS + ": " + String.Join("; ", listOfIssuanceFailedErrors);
                            stpComment = "Failed Issuance" + (listOfIssuanceFailedErrors.Count > 0 ? "; " + listOfIssuanceFailedErrors.Count + " validation item" + addS : "");
                            QuoteUserTransfer.RouteToUnderwriting(processingType, img.PolicyId, img.PolicyImageNum, img.AgencyId, userID, valItems, "API", ref routeErrors);
                            if(routeErrors.Count > 0)
                            {
                                //route call failed
                                //log? email?
                                //TODO: DJG - what to do here?
                                stpComment += "; unable to route to UW";
                            }
                            else
                            {
                                stpComment += "; routed to UW";
                            }
                        }
                    }
                    else
                    {
                        stpComment = "Problem calling AquirePendingImage service; ";
                        QuoteUserTransfer.RouteToUnderwriting(processingType, img.PolicyId, img.PolicyImageNum, img.AgencyId, userID, valItems, "API", ref routeErrors);
                        if (routeErrors.Count > 0)
                        {
                            //route call failed
                            //log? email?
                            //TODO: DJG - what to do here?
                            stpComment += "; unable to route to UW";
                        }
                        else
                        {
                            stpComment += "; routed to UW";
                        }
                    }
                    UpdateStraightThruProcessingRecord(stpID, stpSuccess, stpComment);
                }
            }
            return stpSuccess;
        }

        public static bool IssueQuoteAndUpdateSTPDatabase(int policyID, int policyImageNum, CommonObjects.Enums.Enums.PolicyQuoteProcessingType processingType, DataServicesCore.CommonObjects.Enums.Enums.PolicyQuotingApplication application, int userID, string username, List<String> errorMsgs)
        {
            DC.Objects.Policy.Image img = GetDiamondImage(policyID, policyImageNum, errorMsgs);
            return IssueQuoteAndUpdateSTPDatabase(img, processingType, application, userID, username, errorMsgs);
        }

        public static bool AcquireAndIssueQuoteAndUpdateSTPDatabase(int policyID, int policyImageNum, CommonObjects.Enums.Enums.PolicyQuoteProcessingType processingType, DataServicesCore.CommonObjects.Enums.Enums.PolicyQuotingApplication application, int userID, string username, List<String> errorMsgs)
        {
            DC.Objects.Policy.Image img = null;
            if (policyID > 0 && policyImageNum > 0)
            {
                var rsp = DataServicesCore.BusinessLogic.Diamond.Policy.QuoteUserTransfer.TransferQuoteToUser(policyID, policyImageNum, userID);
                if(rsp?.ResponseData?.PolicyImage != null)
                {
                    img = rsp.ResponseData.PolicyImage;
                }
            }
            return IssueQuoteAndUpdateSTPDatabase(img, processingType, application, userID, username, errorMsgs);
        }
    }
}
