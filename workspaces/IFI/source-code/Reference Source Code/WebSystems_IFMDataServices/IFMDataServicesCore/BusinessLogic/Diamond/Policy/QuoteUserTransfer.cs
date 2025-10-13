using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFM.PrimitiveExtensions;
using DC = Diamond.Common;

namespace IFM.DataServicesCore.BusinessLogic.Diamond.Policy
{
    public class QuoteUserTransfer
    {
        static public global::Diamond.Common.Services.Messages.PolicyService.AcquirePendingImage.Response TransferQuoteToUser(int policyId, int imageNumber, int userId)
        {
            if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {
                using (var DS = Insuresoft.DiamondServices.PolicyService.AcquirePendingImage())
                {
                    DS.RequestData.PolicyId = policyId;
                    DS.RequestData.PolicyImageNum = imageNumber;
                    DS.RequestData.UsersId = userId;

                    var response = DS.Invoke()?.DiamondResponse;
                    return response;
                }
            }
            return null;
        }

        static public global::Diamond.Common.Services.Messages.PolicyService.AcquirePendingImage.Response TransferQuoteToAgency(int policyId, int imageNumber, string agencyCode)
        {
            var agencyUsers = IFM.DataServicesCore.BusinessLogic.Diamond.DiamondUsers.GetAllActiveAgencyUsers(agencyCode);
            if (agencyUsers.Any())
                return TransferQuoteToUser(policyId, imageNumber, agencyUsers[0]);
            return null;
        }

        static public global::Diamond.Common.Services.Messages.PolicyService.AcquirePendingImage.Response TransferQuoteToAgency(string quoteNumber, string agencyCode)
        {
            var history = new Diamond.Policy.QuoteHistoryLookup(quoteNumber);
            if (history.MostCurrentPolicyHistory != null && history.MostCurrentPolicyHistory.PolicyId > 0)
            {
                var agencyUsers = IFM.DataServicesCore.BusinessLogic.Diamond.DiamondUsers.GetAllActiveAgencyUsers(agencyCode);
                if (agencyUsers.Any())
                    return TransferQuoteToUser(history.MostCurrentPolicyHistory.PolicyId, history.MostCurrentPolicyHistory.PolicyImageNum, agencyUsers[0]);
            }
            return null;
        }

        public static bool RouteToUnderwriting(CommonObjects.Enums.Enums.PolicyQuoteProcessingType processingType, int policyID, int policyImageNum, int agencyID, int userID, string routeMsg, string routeFromPage, ref List<string> errorMsgs, bool attemptedIssuance = true, bool finalizeQueue = false)
        {
            bool success = false;
            if (policyID > 0 && policyImageNum >= 0 && agencyID > 0)
            {
                try
                {
                    var request = new DC.Services.Messages.WorkflowService.TransferTaskToAgencyQueue.Request();
                    var response = new DC.Services.Messages.WorkflowService.TransferTaskToAgencyQueue.Response();

                    string routeToUwRemarks = "routed to UW from " + routeFromPage;
                    routeToUwRemarks += attemptedIssuance == true ? " (failed issuance via STP)" : "";
                    routeToUwRemarks += routeMsg.HasValue() ? "; " + routeMsg : "";

                    userID = Login.GetUserId();

                    var rd = request.RequestData;
                    rd.PolicyId = policyID;
                    rd.PolicyImageNum = policyImageNum;
                    rd.AgencyId = agencyID;
                    rd.CurrentUsersId = userID;
                    rd.NewUsersId = userID;
                    rd.WorkflowQueueTypeId = finalizeQueue == true ? DC.Enums.Workflow.WorkflowQueueType.Finalize : DC.Enums.Workflow.WorkflowQueueType.Help;
                    rd.Remarks = routeToUwRemarks;
                    rd.Urgent = true;
                    rd.Mandatory = true;

                    using (var workflowService = new DC.Services.Proxies.Workflow.WorkflowServiceProxy())
                    {
                        response = workflowService.TransferTaskToAgencyQueue(request);
                    }

                    if (response?.ResponseData != null)
                    {
                        success = response.ResponseData.OperationSuccessful;
                        if (response.DiamondValidation?.ValidationItems?.Count > 0)
                        {
                            foreach (var vi in response.DiamondValidation.ValidationItems)
                            {
                                errorMsgs = errorMsgs.NewIfNull();
                                errorMsgs.Add(vi.Message);
                            }
                        }
                    }
                    else
                    {
                        errorMsgs.Add("No response data; routing to UW failed.");
                    }
                }
                catch
                {
                    errorMsgs.Add("An unknown error occurred. This quote was not routed to UW.");
                }
            }
            else
            {
                errorMsgs.Add("We do not have all the information necessary to attempt a route to UW.");
            }
            return success;
        }

    }
}
