using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFM.DataServicesCore.CommonObjects.Fiserv;
using cc = IFM_CreditCardProcessing;
using Newtonsoft.Json;
using IFM.DataServicesCore.BusinessLogic.PublicDomain;
using APIResponse = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServicesCore.BusinessLogic.Fiserv
{
    public class ScheduledPaymentHelper : ModelBase
    {
        private const string location = "IFMDataServices.Core.BusinessLogic.Fiserv.ScheduledPaymentHelper";
        public List<ScheduledPayment> ScheduledPaymentsForFiservScheduledPayments(List<IFM_FiservDatabaseObjects.FiservScheduledPayment> sps)
        {
            List<ScheduledPayment> scheduledPayments = null;

            if (sps != null && sps.Count > 0)
            {
                foreach (IFM_FiservDatabaseObjects.FiservScheduledPayment sp in sps)
                {
                    if (sp != null)
                    {
                        ScheduledPayment sPayment = new ScheduledPayment();
                        sPayment.BankAccountType = sp.bankAccountType;
                        sPayment.EmailAddress = sp.emailAddress;
                        sPayment.FiservScheduledPaymentId = sp.fiservScheduledPaymentId;
                        sPayment.FiservWalletItemId = sp.fiservWalletItemId;
                        sPayment.FundingAccountLastFourDigits = sp.fundingAccountLastFourDigits;
                        sPayment.FundingAccountToken = sp.fundingAccountToken;
                        sPayment.FundingAccountType = sp.fundingAccountType;
                        sPayment.FundingCategory = sp.fundingAccountCategory;
                        if (sp.fs != null)
                        {
                            sPayment.KeyIdentifier = sp.fs.userId;
                        }
                        else
                        {
                            sPayment.KeyIdentifier = "";
                        }
                        sPayment.PaymentAmount = sp.paymentAmount;
                        sPayment.PaymentScheduleDate = sp.paymentScheduleDate;
                        sPayment.PolicyNumber = sp.policyNumber;
                        sPayment.ReferenceCode = sp.referenceCode;
                        sPayment.ReferenceId = sp.referenceId;
                        sPayment.FeeAmount = sp.feeAmountString; //added 9/25/2024

                        if (scheduledPayments == null)
                        {
                            scheduledPayments = new List<ScheduledPayment>();
                        }
                        scheduledPayments.Add(sPayment);
                    }
                }
            }

            return scheduledPayments;
        }


        public List<ScheduledPayment> ScheduledPaymentsForUserIds(List<string> userIds)
        {
            List<ScheduledPayment> scheduledPayments = null;

            if (userIds != null && userIds.Count > 0)
            {
                bool attemptedLookup = false;
                bool caughtDbError = false;
                string dbErrorMsg = "";
                List<IFM_FiservDatabaseObjects.FiservScheduledPayment> sps = creditCardMethods.Database_FiservScheduledPayments(active: CommonHelperClass.YesNoOrMaybe.Yes, processed: CommonHelperClass.YesNoOrMaybe.No, userIds: userIds, paymentScheduleDateMinimum: DateTime.Today.ToShortDateString(), attemptedLookup: ref attemptedLookup, caughtDatabaseError: ref caughtDbError, databaseErrorMessage: ref dbErrorMsg);

                if (caughtDbError && LogFiserveScheduledPaymentErrors())
                {
                    IFMErrorLogging.LogIssue($"Caught DB Error: {dbErrorMsg}; AttemptedLookup: {attemptedLookup};", "IFMDataServicesCore -> BusinessLogic -> Fiserv -> ScheduledPaymentHelper -> ScheduledPaymentsForUserIds();");
                }

                scheduledPayments = ScheduledPaymentsForFiservScheduledPayments(sps);
            }

            return scheduledPayments;
        }

        //added
        public List<ScheduledPayment> ScheduledPaymentsForPolicyNumbers(List<string> policyNumbers)
        {
            string errorLogLocation = $"{location}.ScheduledPaymentsForPolicyNumbers";
            List<ScheduledPayment> scheduledPayments = new List<ScheduledPayment>();

            //Just in case someone scheduled payments for policies before they became an account bill policy.
            var policiesToAdd = new List<string>();

            policyNumbers = policyNumbers.Distinct().ToList(); //make sure we don't have any duplicates so we don't needlessly lookup extra info.

            foreach (var policyNumber in policyNumbers)
            {
                if (policyNumber.IsPolicyOrAccountBillNumber())
                {
                    var accountAPI = new IFM.PolicyAPIModels.Request.AccountBillInquiry(AppConfig.PolicyInquiryAPIEndpoint);
                    var accountInfo = accountAPI.GetAccountBillInfoByAccountNumberOrPolicyNumber(policyNumber);
                    if (accountInfo?.HasResults == true)
                    {
                        foreach (var policy in accountInfo.ResponseData.PoliciesInAccount)
                        {
                            if (policiesToAdd.Contains(accountInfo.ResponseData.AccountNumber) == false && policyNumbers.Contains(policy.PolicyNumber) == false)
                            {
                                policiesToAdd.Add(policy.PolicyNumber);
                            }
                        }
                        if (policiesToAdd.Contains(accountInfo.ResponseData.AccountNumber) == false && policyNumbers.Contains(accountInfo.ResponseData.AccountNumber) == false)
                        {
                            policiesToAdd.Add(accountInfo.ResponseData.AccountNumber);
                        }
                    }
                    else
                    {
                        if (accountInfo.HasErrors)
                        {
                            if (accountAPI.ServiceResultHasException(accountInfo, out string errorMessage))
                            {
                                IFM.IFMErrorLogging.LogException(accountInfo.APIResponseForClientModel.Exception, $"{errorLogLocation}; {accountAPI.GetDebugInfo()}");
                            }
                        }
                    }
                }
            }

            if(policiesToAdd.Count > 0)
            {
                policyNumbers.AddRange(policiesToAdd);
            }

            if (policyNumbers?.Any() ??false)
            {
                bool attemptedLookup = false;
                bool caughtDbError = false;
                string dbErrorMsg = "";
                List<IFM_FiservDatabaseObjects.FiservScheduledPayment> sps =
                    creditCardMethods.Database_FiservScheduledPayments(active: CommonHelperClass.YesNoOrMaybe.Yes,
                                                                       processed: CommonHelperClass.YesNoOrMaybe.No,
                                                                       policyNumbers: policyNumbers,
                                                                       paymentScheduleDateMinimum: DateTime.Today.ToShortDateString(),
                                                                       attemptedLookup: ref attemptedLookup,
                                                                       caughtDatabaseError: ref caughtDbError,
                                                                       databaseErrorMessage: ref dbErrorMsg);

                if (caughtDbError && LogFiserveScheduledPaymentErrors())
                {
                    IFMErrorLogging.LogIssue($"Caught DB Error: {dbErrorMsg}; AttemptedLookup: {attemptedLookup}; PolicyNumbers:[{String.Join(",", policyNumbers)}]; PaymentScheduleDateMinimum: {DateTime.Today.ToShortDateString()}; ", "IFMDataServicesCore -> BusinessLogic -> Fiserv -> ScheduledPaymentHelper -> ScheduledPaymentsForPolicyNumbers();");
                }

                if (sps != null)
                {
                    scheduledPayments = ScheduledPaymentsForFiservScheduledPayments(sps);

                    foreach (var sp in scheduledPayments)
                    {
                        if (sp.PolicyNumber.IsAccountBillNumber())
                        {
                            sp.AccountBillNumber = sp.PolicyNumber;
                        }
                    }
                }
            }

            return scheduledPayments;
        }

        //added 6/30/2020
        public APIResponse.Common.ServiceResult<APIResponse.Fiserv.FiservAddScheduledPaymentResult> AddScheduledPayment(CommonObjects.Fiserv.AddScheduledPaymentBody addBody)
        {
            //scheduledPaymentInfo
            var sr = new APIResponse.Common.ServiceResult<APIResponse.Fiserv.FiservAddScheduledPaymentResult>();
            if (addBody != null)
            {
                if (addBody.Payment != null)
                {
                    if (addBody.Payment.AccountBillNumber.IsAccountBillNumber())
                    {
                        addBody.Payment.PolicyNumber = addBody.Payment.AccountBillNumber;
                    }
                    cc.Fiserv_AddSchedulePaymentRequestBody addSchedulePaymentRequestBody = new cc.Fiserv_AddSchedulePaymentRequestBody();
                    addSchedulePaymentRequestBody.emailAddress = addBody.Payment.EmailAddress;
                    addSchedulePaymentRequestBody.fundingAccountToken = addBody.Payment.FundingAccountToken;
                    addSchedulePaymentRequestBody.paymentAmount = Convert.ToDouble(addBody.Payment.PaymentAmount);
                    if (cc.Common.Fiserv_ImmediatePaymentRequestBody_AddPolicyNumberToPaymentCustomData() == true && string.IsNullOrWhiteSpace(addBody.Payment.PolicyNumber) == false)
                    {
                        addSchedulePaymentRequestBody.paymentCustomList = new cc.Fiserv_PaymentCustomList();
                        cc.Fiserv_PaymentCustomData customData = new cc.Fiserv_PaymentCustomData();
                        customData.customDataName = "PolicyNumber";
                        customData.customDataValue = addBody.Payment.PolicyNumber;
                        addSchedulePaymentRequestBody.paymentCustomList.Add(customData);
                    }
                    //addSchedulePaymentRequestBody.paymentScheduleDate = addBody.Payment.PaymentScheduleDate;
                    addSchedulePaymentRequestBody.paymentScheduleDate = cc.Common.DateForString(addBody.Payment.PaymentScheduleDate).ToString("yyyy-MM-dd");

                    cc.Enums.PaymentInterface pmtInterface = cc.Enums.PaymentInterface.MemberPortalSite;
                    if (addBody.IsFromOneView == true)
                    {
                        pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.OneView;
                    }
                    cc.UserInfo user = new cc.UserInfo();
                    user.Username = addBody.UserName;
                    user.UserType = addBody.UserType;
                    int fiservWalletItemId = addBody.Payment.FiservWalletItemId;
                    int fiservScheduledPaymentId = 0;
                    string referenceId = "";
                    cc.FiservErrorObject errorObject = null;
                    string responseMessage = "";
                    //bool success = creditCardMethods.SuccessfullyCreatedFiservScheduledPayment(addSchedulePaymentRequestBody, policyNumber: addBody.Payment.PolicyNumber, walletItemSessionUserId: addBody.Payment.KeyIdentifier, fiservWalletItemId: ref fiservWalletItemId, pmtInterface: pmtInterface, user: user, fiservScheduledPaymentId: ref fiservScheduledPaymentId, referenceId: ref referenceId, errorObject: ref errorObject, responseMessage: ref responseMessage);
                    //updated 7/1/2020 to adjust MemberIdentifer (EmailAddress)

                    //sr.ResponseData.Success = creditCardMethods.SuccessfullyCreatedFiservScheduledPayment(addSchedulePaymentRequestBody, policyNumber: addBody.Payment.PolicyNumber, walletItemSessionUserId: cc.Common.AdjustedEmailAddressForFiservApiCall(addBody.Payment.KeyIdentifier), fiservWalletItemId: ref fiservWalletItemId, pmtInterface: pmtInterface, user: user, fiservScheduledPaymentId: ref fiservScheduledPaymentId, referenceId: ref referenceId, errorObject: ref errorObject, responseMessage: ref responseMessage);
                    //upddated 9/25/2024
                    sr.ResponseData.Success = creditCardMethods.SuccessfullyCreatedFiservScheduledPayment_OptionalFeeAmount(addSchedulePaymentRequestBody, policyNumber: addBody.Payment.PolicyNumber, walletItemSessionUserId: cc.Common.AdjustedEmailAddressForFiservApiCall(addBody.Payment.KeyIdentifier), fiservWalletItemId: ref fiservWalletItemId, pmtInterface: pmtInterface, user: user, feeAmount: addBody.Payment.FeeAmount, fiservScheduledPaymentId: ref fiservScheduledPaymentId, referenceId: ref referenceId, errorObject: ref errorObject, responseMessage: ref responseMessage);

                    if (sr.ResponseData.Success == true)
                    {
                        sr.ResponseData.FiservScheduledPaymentId = fiservScheduledPaymentId;
                        sr.ResponseData.ReferenceId = referenceId;
                    }
                    else
                    {
                        string errorMessage = "problem adding scheduled payment";
                        //use something specific if possible; use new byref param responseMessage
                        if (string.IsNullOrWhiteSpace(responseMessage) == false)
                        {
                            errorMessage = errorMessage + ": " + responseMessage;
                        }
                        else if (errorObject != null && string.IsNullOrWhiteSpace(errorObject.ErrorMessage) == false)
                        {
                            errorMessage = errorObject.ErrorMessage;
                        }
                        if (LogFiserveScheduledPaymentErrors())
                        {
                            IFMErrorLogging.LogIssue($"Error adding Fiserv scheduled payment: {errorObject.ErrorMessage}; ResponseMessage: {responseMessage}; RequestBody: {Newtonsoft.Json.JsonConvert.SerializeObject(addSchedulePaymentRequestBody)}", "IFMDataServicesCore -> BusinessLogic -> Fiserv -> ScheduledPaymentHelper -> AddScheduledPayment();");
                        }

                        sr.Messages.CreateErrorMessage(errorMessage);
                        if (LogFiserveScheduledPaymentErrors())
                        {
                            IFMErrorLogging.LogIssue($"Error adding Fiserv scheduled payment: {errorObject.ErrorMessage}; ResponseMessage: {responseMessage}; RequestBody: {Newtonsoft.Json.JsonConvert.SerializeObject(addSchedulePaymentRequestBody)}", "IFMDataServicesCore -> BusinessLogic -> Fiserv -> ScheduledPaymentHelper -> AddScheduledPayment();");
                        }
                    }
                }
                else
                {
                    sr.Messages.CreateErrorMessage("No scheduled payment information provided.");
                }
            }

            return sr;
        }


        public APIResponse.Common.ServiceResult<APIResponse.Fiserv.FiservUpdateScheduledPaymentResult> UpdateScheduledPayment(CommonObjects.Fiserv.UpdateScheduledPaymentBody updateBody)
        {
            //scheduledPaymentInfo
            var sr = new APIResponse.Common.ServiceResult<APIResponse.Fiserv.FiservUpdateScheduledPaymentResult>();
            if (updateBody != null)
            {
                if (updateBody.Payment != null)
                {
                    string referenceId = updateBody.Payment.ReferenceId;
                    if (string.IsNullOrWhiteSpace(referenceId) == false)
                    {
                        if (updateBody.Payment.AccountBillNumber.IsAccountBillNumber())
                        {
                            updateBody.Payment.PolicyNumber = updateBody.Payment.AccountBillNumber;
                        }
                        cc.Fiserv_UpdateSchedulePaymentRequestBody updateSchedulePaymentRequestBody = new cc.Fiserv_UpdateSchedulePaymentRequestBody();
                        updateSchedulePaymentRequestBody.referenceId = referenceId;
                        if (string.IsNullOrWhiteSpace(updateBody.Payment.EmailAddress) == false)
                        {
                            updateSchedulePaymentRequestBody.emailAddress = updateBody.Payment.EmailAddress;
                        }
                        else
                        {
                            updateSchedulePaymentRequestBody.emailAddress = null;
                        }
                        if (string.IsNullOrWhiteSpace(updateBody.Payment.FundingAccountToken) == false)
                        {
                            updateSchedulePaymentRequestBody.fundingAccountToken = updateBody.Payment.FundingAccountToken;
                        }
                        else
                        {
                            updateSchedulePaymentRequestBody.fundingAccountToken = null;
                        }
                        updateSchedulePaymentRequestBody.paymentAmount = Convert.ToDouble(updateBody.Payment.PaymentAmount);
                        if (cc.Common.Fiserv_ImmediatePaymentRequestBody_AddPolicyNumberToPaymentCustomData() == true && string.IsNullOrWhiteSpace(updateBody.Payment.PolicyNumber) == false)
                        {
                            updateSchedulePaymentRequestBody.paymentCustomList = new cc.Fiserv_PaymentCustomList();
                            cc.Fiserv_PaymentCustomData customData = new cc.Fiserv_PaymentCustomData();
                            customData.customDataName = "PolicyNumber";
                            customData.customDataValue = updateBody.Payment.PolicyNumber;
                            updateSchedulePaymentRequestBody.paymentCustomList.Add(customData);
                        }
                        if (cc.Common.IsDateString(updateBody.Payment.PaymentScheduleDate) == true)
                        {
                            updateSchedulePaymentRequestBody.paymentScheduleDate = cc.Common.DateForString(updateBody.Payment.PaymentScheduleDate).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            updateSchedulePaymentRequestBody.paymentScheduleDate = null;
                        }

                        cc.Enums.PaymentInterface pmtInterface = cc.Enums.PaymentInterface.MemberPortalSite;
                        if (updateBody.IsFromOneView == true)
                        {
                            pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.OneView;
                        }
                        cc.UserInfo user = new cc.UserInfo();
                        user.Username = updateBody.UserName;
                        user.UserType = updateBody.UserType;
                        int fiservWalletItemId = updateBody.Payment.FiservWalletItemId;
                        int fiservScheduledPaymentId = 0;
                        cc.FiservErrorObject errorObject = null;
                        string responseMessage = "";
                        //bool success = creditCardMethods.SuccessfullyUpdatedFiservScheduledPayment(updateSchedulePaymentRequestBody, ref referenceId, ref fiservScheduledPaymentId, policyNumber: updateBody.Payment.PolicyNumber, scheduledPaymentSessionUserId: updateBody.Payment.KeyIdentifier, fiservWalletItemId: ref fiservWalletItemId, pmtInterface: pmtInterface, user: user, errorObject: ref errorObject, responseMessage: ref responseMessage);
                        //updated 7/1/2020 to adjust MemberIdentifer (EmailAddress)

                        //sr.ResponseData.Success = creditCardMethods.SuccessfullyUpdatedFiservScheduledPayment(updateSchedulePaymentRequestBody, ref referenceId, ref fiservScheduledPaymentId, policyNumber: updateBody.Payment.PolicyNumber, scheduledPaymentSessionUserId: cc.Common.AdjustedEmailAddressForFiservApiCall(updateBody.Payment.KeyIdentifier), fiservWalletItemId: ref fiservWalletItemId, pmtInterface: pmtInterface, user: user, errorObject: ref errorObject, responseMessage: ref responseMessage);
                        //upddated 9/25/2024
                        sr.ResponseData.Success = creditCardMethods.SuccessfullyUpdatedFiservScheduledPayment_OptionalFeeAmount(updateSchedulePaymentRequestBody, ref referenceId, ref fiservScheduledPaymentId, policyNumber: updateBody.Payment.PolicyNumber, scheduledPaymentSessionUserId: cc.Common.AdjustedEmailAddressForFiservApiCall(updateBody.Payment.KeyIdentifier), fiservWalletItemId: ref fiservWalletItemId, pmtInterface: pmtInterface, user: user, feeAmount: updateBody.Payment.FeeAmount, errorObject: ref errorObject, responseMessage: ref responseMessage);

                        if (sr.ResponseData.Success == false)
                        {
                            string errorMessage = "problem updating scheduled payment";
                            //use something specific if possible; use new byref param responseMessage
                            if (string.IsNullOrWhiteSpace(responseMessage) == false)
                            {
                                errorMessage = errorMessage + ": " + responseMessage;
                            }
                            else if (errorObject != null && string.IsNullOrWhiteSpace(errorObject.ErrorMessage) == false)
                            {
                                errorMessage = errorObject.ErrorMessage;
                            }
                            sr.Messages.CreateErrorMessage(errorMessage);
                            if (LogFiserveScheduledPaymentErrors())
                            {
                                IFMErrorLogging.LogIssue($"Error updating Fiserv scheduled payment: {errorObject.ErrorMessage}; ResponseMessage: {responseMessage}; RequestBody: {Newtonsoft.Json.JsonConvert.SerializeObject(updateSchedulePaymentRequestBody)}", "IFMDataServicesCore -> BusinessLogic -> Fiserv -> ScheduledPaymentHelper -> UpdateScheduledPayment();");
                            }
                        }
                    }
                    else
                    {
                        sr.Messages.CreateErrorMessage("Reference Id required.");
                    }
                }
                else
                {
                    sr.Messages.CreateErrorMessage("No scheduled payment information provided.");
                }
            }

            return sr;
        }



        public APIResponse.Common.ServiceResult<APIResponse.Fiserv.FiservDeleteScheduledPaymentResult> DeleteScheduledPayment(CommonObjects.Fiserv.DeleteScheduledPaymentBody deleteBody)
        {
            //scheduledPaymentInfo
            var sr = new APIResponse.Common.ServiceResult<APIResponse.Fiserv.FiservDeleteScheduledPaymentResult>();
            if (deleteBody != null)
            {
                if (deleteBody.Payment != null)
                {
                    string referenceId = deleteBody.Payment.ReferenceId;
                    if (string.IsNullOrWhiteSpace(referenceId) == false)
                    {
                        if (deleteBody.Payment.AccountBillNumber.IsAccountBillNumber())
                        {
                            deleteBody.Payment.PolicyNumber = deleteBody.Payment.AccountBillNumber;
                        }
                        cc.Enums.PaymentInterface pmtInterface = cc.Enums.PaymentInterface.MemberPortalSite;
                        if (deleteBody.IsFromOneView == true)
                        {
                            pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.OneView;
                        }
                        cc.UserInfo user = new cc.UserInfo();
                        user.Username = deleteBody.UserName;
                        user.UserType = deleteBody.UserType;
                        int fiservScheduledPaymentId = 0;
                        cc.FiservErrorObject errorObject = null;
                        string responseMessage = "";
                        //bool success = creditCardMethods.SuccessfullyDeletedFiservScheduledPayment(ref referenceId, ref fiservScheduledPaymentId, policyNumber: deleteBody.Payment.PolicyNumber, scheduledPaymentSessionUserId: deleteBody.Payment.KeyIdentifier, pmtInterface: pmtInterface, user: user, errorObject: ref errorObject, responseMessage: ref responseMessage);
                        //updated 7/1/2020 to adjust MemberIdentifer (EmailAddress)

                        sr.ResponseData.Success = creditCardMethods.SuccessfullyDeletedFiservScheduledPayment(ref referenceId, ref fiservScheduledPaymentId, policyNumber: deleteBody.Payment.PolicyNumber, scheduledPaymentSessionUserId: cc.Common.AdjustedEmailAddressForFiservApiCall(deleteBody.Payment.KeyIdentifier), pmtInterface: pmtInterface, user: user, errorObject: ref errorObject, responseMessage: ref responseMessage);

                        if (sr.ResponseData.Success == false)
                        {
                            string errorMessage = "problem deleting scheduled payment";
                            //use something specific if possible; use new byref param responseMessage
                            if (string.IsNullOrWhiteSpace(responseMessage) == false)
                            {
                                errorMessage = errorMessage + ": " + responseMessage;
                            }
                            else if (errorObject != null && string.IsNullOrWhiteSpace(errorObject.ErrorMessage) == false)
                            {
                                errorMessage = errorObject.ErrorMessage;
                            }
                            sr.Messages.CreateErrorMessage(errorMessage);
                            if (LogFiserveScheduledPaymentErrors())
                            {
                                IFMErrorLogging.LogIssue($"Error deleting Fiserv scheduled payment: {errorObject.ErrorMessage}; ResponseMessage: {responseMessage}; PolicyNumber: {Newtonsoft.Json.JsonConvert.SerializeObject(deleteBody)}", "IFMDataServicesCore -> BusinessLogic -> Fiserv -> ScheduledPaymentHelper -> DeleteScheduledPayment();");
                            }
                        }
                    }
                    else
                    {
                        sr.Messages.CreateErrorMessage("Reference Id required.");
                    }
                }
                else
                {
                    sr.Messages.CreateErrorMessage("No scheduled payment information provided.");
                }
            }

            return sr;
        }

        protected bool LogFiserveScheduledPaymentErrors()
        {
            var hasError = false;
            bool logError = new CommonHelperClass().GetApplicationXMLSettingForBoolean("FiservSettings_LogScheduledPaymentErrors", "FiservSettings.xml", ref hasError);
            if (hasError == false && logError)
            {
                return true;
            }
            return false;
        }
    }
}
