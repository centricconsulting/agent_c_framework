using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using IFM.PrimitiveExtensions;
using System.Linq;
using System.Diagnostics;
using DCO = Diamond.Common.Objects;
using Diamond.Common.Objects.Billing;
using System.Security.Policy;


namespace IFM.DataServicesCore.BusinessLogic.Payments.eCheck
{

    public class eCheckProcessing
    {
        private readonly string eCheckUserName = AppConfig.PrintUserName;
        private readonly string echeckPassword = AppConfig.PrintUserPassword;

        private readonly eCheckProcessRequest request;

        public eCheckProcessing(eCheckProcessRequest request)
        {
            this.request = request;
        }

        private IFM_CreditCardProcessing.EcheckInfo eCheckRequestToCCLibraryEcheckInfo()
        {
            IFM_CreditCardProcessing.EcheckInfo EcheckInfo = new IFM_CreditCardProcessing.EcheckInfo();

            EcheckInfo.Amount = request.paymentamount.TryToGetDecimal();
            EcheckInfo.AccountBillNumber = request.AccountBillAccountNumber;
            EcheckInfo.PolicyNumber = request.policyNumber;
            EcheckInfo.PolicyId = request.polID;
            EcheckInfo.EmailAddress = request.emailAddress;

            string firstName = "";
            string lastName = "";
            var agencyId = GetAgencyAndInsuredInfo(ref firstName, ref lastName);
            if (string.IsNullOrWhiteSpace(firstName) == false)
            {
                firstName = IFM_CreditCardProcessing.Common.Fiserv_Formatted_CheckDetail_FirstName(firstName);
            }
            if (string.IsNullOrWhiteSpace(lastName) == false)
            {
                lastName = IFM_CreditCardProcessing.Common.Fiserv_Formatted_CheckDetail_LastName(lastName);
            }
            else
            {
                lastName = this.request.policyNumber; //just defaulting something so it could potentially succeed in the case we can't pull insured info
            }

            IFM_CreditCardProcessing.Fiserv_CheckDetail checkDetail = new IFM_CreditCardProcessing.Fiserv_CheckDetail();
            EcheckInfo.RoutingNumber = this.request.aba;
            EcheckInfo.CheckAccountNumber = this.request.bankAccountNumber;
            EcheckInfo.FirstName = firstName; //started using 8/18/2020
            EcheckInfo.LastName = lastName; //started using 8/18/2020

            if (this.request.bankAccoutType == eCheckProcessRequest.bank_account_type.savings)
            {
                EcheckInfo.CheckAccountType = IFM_CreditCardProcessing.EcheckInfo.CheckAccountTypes.Saving;
            }
            else
            {
                EcheckInfo.CheckAccountType = IFM_CreditCardProcessing.EcheckInfo.CheckAccountTypes.Checking;
            }

            EcheckInfo.UserInfo = new IFM_CreditCardProcessing.UserInfo();
            EcheckInfo.UserInfo.Username = this.request.policyHolderUserName;

            if (this.request.source == 3)
            {
                EcheckInfo.PaymentInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.OneView;
                EcheckInfo.UserInfo.UserType = IFM_CreditCardProcessing.Enums.UserType.Staff;
            }
            else
            {
                EcheckInfo.PaymentInterface = (IFM_CreditCardProcessing.Enums.PaymentInterface)(int)this.request.PaymentInterface;
                EcheckInfo.UserInfo.UserType = (IFM_CreditCardProcessing.Enums.UserType)this.request.source;
            }           

            return EcheckInfo;
        }

        public eCheckProcessResponse ProcesseCheckRequest()
        {
            eCheckProcessResponse response = new eCheckProcessResponse(this.request);
            try
            {
                if (this.request == null)
                {
                    response.errorMsgs.Add("The provided request object was null.");
                    response.userFeedBack.Add("An unexpected system error occurred. Please try again later.#i5366");
#if DEBUG
                    Debugger.Break();
#endif
                    return response;
                }


                if (AlreadyHasEcheckPaymentToday())
                {
    #if DEBUG
                        Debugger.Break();
#else
                    response.errorMsgs.Add("Only one echeck payment allowed per day per policy on the same bank account.");
                    response.userFeedBack.Add("Only one echeck payment allowed per day per policy on the same bank account.");
                    return response;
#endif
                }


                response.DiamondUserId = Login(response);

                //bool excludeFromBankFile = false;
                //string confirmationNumber = "";
                bool sendToFiserv = IFM_CreditCardProcessing.Common.ECheck_SendRealtimePaymentsToVendor();

                Func<bool> ExecuteAsAfterHoursCompleted = () => {
                    int afterHoursRecordId = processAsAfterHours(response); //updated 7/24/2020 to send optional Fiserv params
                    if (afterHoursRecordId > 0)
                    {
                        if (response.confirmationNumber.HasValue() == false)
                        {
                            response.confirmationNumber = $"AH-{afterHoursRecordId}";
                        }
                        response.fullyCompleted = true;
                        return true; //not really needed since it will happen below since afterHoursId > 0
                    }
                    else
                    {
                        //global::IFM.IFMErrorLogging.LogIssue($"Failed to process as after hours eCheck Payment. DiamondUserId: {diamondUserID}");
                        //global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(AppConfig.ErrorEmailAddress, AppConfig.NoReplyEmailAddress, "eCheck Payment Error", $"Failed to process as after hours eCheck Payment. DiamondUserId: {diamondUserID}");
                        //response.errorMsgs.Add("Failed to process as after hours eCheck Payment.");
                        //response.userFeedBack.Add("The billing service is temporarily unavailable. #y7837365265");
                        //updated 7/24/2020
                        string failMsgTextToAdd = "";
                        if (response.PaymentId > 0)
                        {
                            failMsgTextToAdd = IFM_CreditCardProcessing.Common.Append(failMsgTextToAdd, "FiservPaymentId: " + response.PaymentId.ToString(), appendText: "; ");
                        }
                        if (response.confirmationNumber.HasValue())
                        {
                            failMsgTextToAdd = IFM_CreditCardProcessing.Common.Append(failMsgTextToAdd, "ConfirmationNumber: " + response.confirmationNumber, appendText: "; ");
                        }
                        if (string.IsNullOrWhiteSpace(failMsgTextToAdd) == false)
                        {
                            failMsgTextToAdd = "; " + failMsgTextToAdd;
                        }
                        string failMsg = $"Failed to process as after hours eCheck Payment. DiamondUserId: {response.DiamondUserId}{failMsgTextToAdd}";
                        global::IFM.IFMErrorLogging.LogIssue(failMsg, "IFMDATASERVICES -> eCheckProcessing -> Function ProcesseCheckRequest");
                        global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(AppConfig.ErrorEmailAddress, AppConfig.NoReplyEmailAddress, "eCheck Payment Error", failMsg);
                        if (response.confirmationNumber.HasValue())
                        {
                            //already successful through Fiserv; need to return Success
                            //response.confirmationNumber = response.confirmationNumber;
                            response.fullyCompleted = true;
                            return true; //returning true here since the payment was already successful and we don't want the user to see errors because our table insert failed
                        }
                        else
                        {
                            response.errorMsgs.Add("Failed to process as after hours eCheck Payment.");
                            response.userFeedBack.Add("The billing service is temporarily unavailable. #y7837365265");
                        }
                    }

                    return afterHoursRecordId > 0;
                };

                if (response.DiamondUserId > 0)
                {
                    //added 7/24/2020 for Fiserv
                    //bool okayToProceed = true;
                    if (sendToFiserv == true)
                    {                        
                        var EcheckInfo = eCheckRequestToCCLibraryEcheckInfo();

                        IFM_CreditCardProcessing.FiservErrorObject errorObject = null;

                        IFM_CreditCardProcessing.CreditCardMethods ccm = new IFM_CreditCardProcessing.CreditCardMethods();
                        int fiservPaymentId = 0;
                        bool successfullyProcessedWithFiserv = ccm.SuccessfullyProcessedFiservEcheckPayment(EcheckInfo, fiservPaymentId: ref fiservPaymentId, errorObject: ref errorObject);
                        response.PaymentId = fiservPaymentId;

                        if (successfullyProcessedWithFiserv == true)
                        {
                            //note: needs to return success to the user since the payment has been successfully processed
                            response.confirmationNumber = EcheckInfo.ConfirmationNumber;
                            response.fullyCompleted = true;
                        }
                        else
                        {
                            string failureMsg = $"Failed to process eCheck Payment.";
                            response.errorMsgs.Add(failureMsg);

                            if (EcheckInfo.PaymentResponseMessage.IsNotNullEmptyOrWhitespace())
                            {
                                response.DetailedErrorMsgs.Add(EcheckInfo.PaymentResponseMessage);
                            }
                            if (errorObject.ErrorMessage.IsNotNullEmptyOrWhitespace())
                            {
                                response.DetailedErrorMsgs.Add(errorObject.ErrorMessage);
                            }
                            if (errorObject.ExceptionMessage.IsNotNullEmptyOrWhitespace())
                            {
                                response.DetailedErrorMsgs.Add(errorObject.ExceptionMessage);
                            }
                            if (errorObject.ExceptionResponseStatusDescription.IsNotNullEmptyOrWhitespace())
                            {
                                response.DetailedErrorMsgs.Add(errorObject.ExceptionResponseStatusDescription);
                            }
                            if (errorObject.ExceptionToString.IsNotNullEmptyOrWhitespace())
                            {
                                response.DetailedErrorMsgs.Add(errorObject.ExceptionToString);
                            }
                            if (errorObject.FiservErrorType != IFM_CreditCardProcessing.Enums.Fiserv_JsonTransaction_ErrorType.None)
                            {
                                response.DetailedErrorMsgs.Add(errorObject.FiservErrorType.ToString());
                            }
                            if (errorObject.ExceptionResponseStatusCode != 0)
                            {
                                response.DetailedErrorMsgs.Add(errorObject.ExceptionResponseStatusCode.ToString());
                            }
                            if (errorObject.ExceptionResponseStream.IsNotNullEmptyOrWhitespace())
                            {
                                response.DetailedErrorMsgs.Add(errorObject.ExceptionResponseStream);
                            }

                            response.userFeedBack.Add(failureMsg);
                        }
                    }

                    if (response.fullyCompleted) //added IF 7/24/2020
                    {
                        //updated 7/24/2020
                        bool createEftAccount = true;
                        if (request.ExcludeFromBankFile)
                        {
                            if (this.request.aba.HasValue() == false || this.request.bankAccountNumber.HasValue() == false)
                            {
                                createEftAccount = false;
                            }
                            else
                            {
                                //we have the info to create it; now see if we should
                                if (CreateEftAccountOnExcludeFromBankFile() == false)
                                {
                                    createEftAccount = false;
                                }
                            }
                        }

                        if (createEftAccount == true)
                        {
                            response.EftAccountId = CreateDiamondEftRecord();
                        }

                        if (response.EftAccountId > 0 || request.ExcludeFromBankFile == true)
                        {
                            ApplyCash applyDiamondCash = new ApplyCash(response);
                            if (applyDiamondCash.DoApplyCash())
                            {
                                //int echeckTableId = insertToDB(diamondUserID, EftAccountId, response, fiservPaymentId: fiservPaymentId, excludeFromBankFile: request.ExcludeFromBankFile, confirmationNumber: confirmationNumber, paymentTypeId: (int)this.request.bankAccoutType);
                                int echeckTableId = insertToDB(response);
                                if (echeckTableId > 0)
                                {
                                    if (response.confirmationNumber.HasValue() == false) //added IF 7/24/2020; original logic in ELSE
                                    {
                                        response.confirmationNumber = SetPaymentConfirmationNumber(response.EftAccountId, echeckTableId);
                                    }
                                    response.fullyCompleted = (string.IsNullOrWhiteSpace(response.confirmationNumber)) ? false : true;
                                    return response;
                                }
                                else
                                {
                                    //updated 7/24/2020
                                    string failMsgTextToAdd = "";
                                    if (response.PaymentId > 0)
                                    {
                                        failMsgTextToAdd = IFM_CreditCardProcessing.Common.Append(failMsgTextToAdd, "FiservPaymentId: " + response.PaymentId.ToString(), appendText: "; ");
                                    }
                                    if (response.confirmationNumber.HasValue())
                                    {
                                        failMsgTextToAdd = IFM_CreditCardProcessing.Common.Append(failMsgTextToAdd, "ConfirmationNumber: " + response.confirmationNumber, appendText: "; ");
                                    }
                                    if (string.IsNullOrWhiteSpace(failMsgTextToAdd) == false)
                                    {
                                        failMsgTextToAdd = "; " + failMsgTextToAdd;
                                    }
                                    string failMsg = $"Insert into Usp_ECheckPayments_Insert failed. Will attempt to process as after hours. DiamondUserId: {response.DiamondUserId}, EFTAccountId: {response.EftAccountId}{failMsgTextToAdd}";
                                    global::IFM.IFMErrorLogging.LogIssue(failMsg, "IFMDATASERVICES -> eCheckProcessing -> Function ProcesseCheckRequest");
                                    global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(AppConfig.ErrorEmailAddress, AppConfig.NoReplyEmailAddress, "eCheck Payment Error", failMsg);
                                    if (!ExecuteAsAfterHoursCompleted())
                                    {
                                        response.errorMsgs.Add("Could not add payment record to echeck table.");
                                        response.userFeedBack.Add("The billing service is temporarily unavailable. #y69162385");
#if DEBUG
                                        Debugger.Break();
#endif
                                    }
                                }

                            }
                            else
                            {
                                //updated 7/24/2020
                                string failMsgTextToAdd = "";
                                if (response.PaymentId > 0)
                                {
                                    failMsgTextToAdd = IFM_CreditCardProcessing.Common.Append(failMsgTextToAdd, "FiservPaymentId: " + response.PaymentId.ToString(), appendText: "; ");
                                }
                                if (response.confirmationNumber.HasValue())
                                {
                                    failMsgTextToAdd = IFM_CreditCardProcessing.Common.Append(failMsgTextToAdd, "ConfirmationNumber: " + response.confirmationNumber, appendText: "; ");
                                }
                                if (string.IsNullOrWhiteSpace(failMsgTextToAdd) == false)
                                {
                                    failMsgTextToAdd = "; " + failMsgTextToAdd;
                                }
                                string failMsg = $"Could not apply cash. Will attempt to process as after hours. DiamondUserId: {response.DiamondUserId}, EFTAccountId: {response.EftAccountId}{failMsgTextToAdd}";
                                global::IFM.IFMErrorLogging.LogIssue(failMsg, "IFMDATASERVICES -> eCheckProcessing -> Function ProcesseCheckRequest");
                                global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(AppConfig.ErrorEmailAddress, AppConfig.NoReplyEmailAddress, "eCheck Payment Error", failMsg);
                                if (!ExecuteAsAfterHoursCompleted())
                                {
                                    response.errorMsgs.Add("Could not apply cash to policy!");
                                    response.userFeedBack.Add("The billing service is temporarily unavailable. #c9251423");
#if DEBUG
                                    Debugger.Break();
#endif
                                }
                            }

                        }
                        else
                        {
                            global::IFM.IFMErrorLogging.LogIssue("Could not create EFTID. Will attempt to process as after hours.", "IFMDATASERVICES -> eCheckProcessing -> Function ProcesseCheckRequest");
                            global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(AppConfig.ErrorEmailAddress, AppConfig.NoReplyEmailAddress, "eCheck Payment Error", $"Could not create EFTID. Will attempt to process as after hours.");
                            if (!ExecuteAsAfterHoursCompleted())
                            {
                                response.errorMsgs.Add("Could not create EFTID.");
                                response.userFeedBack.Add("The billing service is temporarily unavailable. #a23643");

#if DEBUG
                                Debugger.Break();
#endif
                            }
                        }
                    }
                }
                else
                {
                    response.errorMsgs.Add("eCheckConfirm login user ERROR!");
                    response.userFeedBack.Add("The billing service is temporarily unavailable. #j8945");
                    global::IFM.IFMErrorLogging.LogIssue("Failed to login for eCheck Payment.", "IFMDATASERVICES -> eCheckProcessing -> Function ProcesseCheckRequest");
                    global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(AppConfig.ErrorEmailAddress, AppConfig.NoReplyEmailAddress, "eCheck Payment Error", "Failed to login for eCheck Payment.");
#if DEBUG
                    Debugger.Break();
#endif
                }
            }
            catch (Exception ex)
            {
                response.errorMsgs.Add("eCheckConfirm ERROR! ");
#if DEBUG
                Debugger.Break();
#endif
            }

            return response;
        }

        private int Login(eCheckProcessResponse response)
        {
            try
            {
                switch (this.request.source)
                {
                    case 1:
                        BusinessLogic.Diamond.Login.LoginNow(eCheckUserName, echeckPassword);
                        break;
                    case 2:
                        BusinessLogic.Diamond.Login.LoginNow(request.agentUserName, request.agentPassword);
                        break;
                    case 3:
                        BusinessLogic.Diamond.Login.LoginNow(eCheckUserName, echeckPassword);
                        break;
                    default:
                        response.errorMsgs.Add("echeck failed to log into the system.");
                        break;
                }
            }
            catch (Exception ex)
            {
                IFMErrorLogging.LogException(ex, "IFMITDATASERVICES -> eCheckProcessing -> Login");
                response.errorMsgs.Add("echeck failed to log into the system.");
            }

            try
            {
                return IFM.DataServicesCore.BusinessLogic.Diamond.Login.GetUserId();
            }
            catch (Exception ex)
            {
                IFMErrorLogging.LogException(ex, "IFMITDATASERVICES -> eCheckProcessing -> Login");
                response.errorMsgs.Add("echeck userid is invalid.");
            }

            return -1;
        }

        private int CreateDiamondEftRecord()
        {
            DCO.EFT.Eft eftInfo = new DCO.EFT.Eft
            {
                PolicyId = this.request.polID,
                PolicyImageNum = this.request.policyImageNum,
                RoutingNumber = this.request.aba,
                AccountNumber = this.request.bankAccountNumber,
                BankAccountTypeId = (int)this.request.bankAccoutType,
                DeductionDay = System.DateTime.Now.Day
            };
            return IFM.DataServicesCore.BusinessLogic.Payments.PayPlanHelper.CreateEFTAccount(eftInfo);
        }

        private bool CreateEftAccountOnExcludeFromBankFile() //added 7/24/2020
        {
            CommonHelperClass chc = new CommonHelperClass();
            bool keyExists = false;
            return chc.ConfigurationAppSettingValueAsBoolean("CreateEftAccountOnExcludeFromBankFile", configurationAppSettingExists: ref keyExists);
        }

        private bool AlreadyHasEcheckPaymentToday()
        {
            // you can't make two payments on the same policy with the same bank information on the same day
            Func<string,List<PriorEcheckPayments>> GetAllEcheckPayments = delegate (string polnum) {
                var priorPayments = new List<PriorEcheckPayments>();
                using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand("IFM_Reports.dbo.usp_ECheck_GetAllPayments",conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("polNum", polnum);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var pp = new PriorEcheckPayments
                                    {
                                        AccountNumber = reader.GetStringIgnoreDBNull("AcctNum"),
                                        AccountType = reader.GetIntIgnoreDBNull("AccountTypeId"),
                                        Amount = Convert.ToDouble(reader.GetDecimalIgnoreDBNull("Amount")),
                                        InsertedDate = reader.GetDateTimeIgnoreDBNull("InsertedDate"),
                                        PolicyNumber = reader.GetStringIgnoreDBNull("PolicyNumber"),
                                        RoutingNumber = reader.GetStringIgnoreDBNull("RoutingNum")
                                    };
                                    priorPayments.Add(pp);
                                }
                            }
                        }
                    }
                }
                return priorPayments;
            };
            //var systemDate = new Diamond.SystemDate().GetSystemDate();
            return (from p in GetAllEcheckPayments(this.request.policyNumber) where p.InsertedDate.ToShortDateString() == DateTime.Now.ToShortDateString() && (p.AccountNumber == this.request.bankAccountNumber || this.request.bankAccountNumber.DoubleEncrypt() == p.AccountNumber) select p).Any();
        }

        private int GetAgencyId()
        {
            int agencyId = 0;
            using (PolicyNumberObject pol = new PolicyNumberObject(this.request.policyNumber))
            {
                pol.GetAllAgencyInfo = true;
                pol.GetPolicyInfo(PolicyNumberObject.PolicySystem.Diamond);
                if (pol.hasPolicyInfo)
                {
                    agencyId = Convert.ToInt32(pol.AgencyID);
                }
            }
            return agencyId;
        }
        private int GetAgencyAndInsuredInfo(ref string firstName, ref string lastName) //added 8/18/2020 to get more info than GetAgencyId
        {
            int agencyId = 0;
            firstName = "";
            lastName = "";

            using (PolicyNumberObject pol = new PolicyNumberObject(this.request.policyNumber))
            {
                pol.GetAllAgencyInfo = true;
                pol.GetAllInsuredInfo = true;
                if (this.request.polID > 0)
                {
                    pol.GetPolicyInfo(PolicyNumberObject.PolicySystem.Diamond, this.request.polID.ToString());
                }
                else
                {
                    pol.GetPolicyInfo(PolicyNumberObject.PolicySystem.Diamond);
                }

                if (pol.hasPolicyInfo)
                {
                    CommonHelperClass chc = new CommonHelperClass();
                    agencyId = chc.IntegerForString(pol.AgencyID);

                    if (pol.InsuredInfo != null)
                    {
                        if (pol.Name1Flag == "P" && pol.InsuredInfo.InsuredName1 != null && string.IsNullOrWhiteSpace(pol.InsuredInfo.InsuredName1.NameLast) == false)
                        {
                            firstName = pol.InsuredInfo.InsuredName1.NameFirst;
                            lastName = pol.InsuredInfo.InsuredName1.NameLast;
                        }
                        else
                        {
                            if (pol.Name1Flag == "B" && pol.InsuredInfo.InsuredBusname1 != null && string.IsNullOrWhiteSpace(pol.InsuredInfo.InsuredBusname1.BusnameName) == false)
                            {
                                lastName = pol.InsuredInfo.InsuredBusname1.BusnameName;
                            }
                            else
                            {
                                lastName = pol.InsuredInfo.FullName1;
                            }
                        }
                    }
                }
            }
            return agencyId;
        }

//        private bool ApplyCash(int agencyId, int diamondUserId, int EftAccountId)
//        {
//            bool cashApplied;

//            DCO.Billing.ApplyCash cash = new DCO.Billing.ApplyCash
//            {
//                AgencyId = agencyId,
//                PolicyId = this.request.polID,
//                PolicyImageNum = this.request.policyImageNum,
//                PolicyNo = this.request.policyNumber,
//                CashAmount = this.request.paymentamount.TryToGetDecimal(),
//                CashInSource = 10030, //eCheck
//                CashType = 1, //payment
//                ReasonId = 0, //none
//                UsersId = diamondUserId,
//                EftAccountId = EftAccountId,
//                CheckDate = new DCO.InsDateTime(DateTime.Now) // might want to make it the system date
//            };

//            if (this.request.AccountBillAccountNumber.IsAccountBillNumber())
//            {
//                cash.AccountPayment = true;
//                cash.BillingAccountId = this.request.AccountBillAccountNumber.GetBillingAccountIdFromAccountNumber();
//                cashApplied = CallApplyBillingAccountPaymentService(cash);
//            }
//            else
//            {
//                cashApplied = CallApplyCashService(cash);
//            }

//            return cashApplied;
//        }

//        private bool CallApplyCashService(DCO.Billing.ApplyCash cash)
//        {
//            using (var DS = Insuresoft.DiamondServices.BillingService.ApplyCash())
//{
//                DS.RequestData.ApplyCash = cash;
//                var res = DS.Invoke();
//                if (res?.DiamondResponse?.ResponseData != null)
//                {
//                    var cashApplied = res.DiamondResponse.ResponseData.Success;
//                    if (res.ex != null)
//                    {
//#if DEBUG
//                        Debugger.Break();
//#endif
//                        //IFMErrorLogging.LogException(res.ex);
//                        using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.Conn))
//                        {
//                            conn.Open();
//                            using (var cmd = new System.Data.SqlClient.SqlCommand("IFMTESTER.dbo.usp_LogAgentPaymentErrors", conn) { CommandType = System.Data.CommandType.StoredProcedure })
//                            {
//                                cmd.Parameters.AddWithValue("@policyNumber", this.request.policyNumber);
//                                cmd.Parameters.AddWithValue("@exceptionMessage", res.ex.ToString());
//                                cmd.ExecuteNonQuery();
//                            }
//                        }
//                    }
//                    return cashApplied;
//                }
//            }
//            return false;
//        }

//        private bool CallApplyBillingAccountPaymentService(DCO.Billing.ApplyCash cash)
//        {
//            using (var DS = Insuresoft.DiamondServices.BillingService.ApplyBillingAccountPayment())
//{
//                DS.RequestData.ApplyCash = cash;
//                var res = DS.Invoke();
//                if (res?.DiamondResponse?.ResponseData != null)
//                {
//                    var cashApplied = res.DiamondResponse.ResponseData.Success;
//                    if (res.ex != null)
//                    {
//#if DEBUG
//                        Debugger.Break();
//#endif
//                        //IFMErrorLogging.LogException(res.ex);
//                        using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.Conn))
//                        {
//                            conn.Open();
//                            using (var cmd = new System.Data.SqlClient.SqlCommand("IFMTESTER.dbo.usp_LogAgentPaymentErrors", conn) { CommandType = System.Data.CommandType.StoredProcedure })
//                            {
//                                cmd.Parameters.AddWithValue("@policyNumber", this.request.policyNumber);
//                                cmd.Parameters.AddWithValue("@exceptionMessage", res.ex.ToString());
//                                cmd.Parameters.AddWithValue("@AccountBillNumber", this.request.AccountBillAccountNumber);
//                                cmd.ExecuteNonQuery();
//                            }
//                        }
//                    }
//                    return cashApplied;
//                }
//            }
//            return false;
//        }

        private string SetPaymentConfirmationNumber(int EftAccountId,int echeckTableId)
        {
            try
            {
                string eCheckPyamentConfirmationNumber = $"E-{EftAccountId}";
                //update echecks table with confirmation number
                using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand("IFM_Reports.dbo.usp_SetConfirmationNumberForECheckPayment", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@confirmationNumber", eCheckPyamentConfirmationNumber);
                        cmd.Parameters.AddWithValue("@eCheckId", echeckTableId);
                        cmd.ExecuteNonQuery();
                        return eCheckPyamentConfirmationNumber;
                    }
                }
            }
            catch
            {
                return string.Empty;
            }


        }

        private int insertToDB(eCheckProcessResponse response) //added optional params 7/24/2020 for Fiserv
        {
            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand("IFM_Reports.dbo.Usp_ECheckPayments_Insert", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PolicyID", response.Request.polID);
                        cmd.Parameters.AddWithValue("@EFTAccountID", response.EftAccountId);
                        cmd.Parameters.AddWithValue("@SourceID", this.request.source);
                        cmd.Parameters.AddWithValue("@ProcessedFlagID", 1);
                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(this.request.paymentamount));
                        //cmd.Parameters.AddWithValue("@Confirmation", 10); // was 0 and it didn't like it Matt A 8-29-17
                        //updated 7/24/2020
                        if (string.IsNullOrWhiteSpace(response.confirmationNumber) == true)
                        {
                            response.confirmationNumber = "10";
                        }
                        cmd.Parameters.AddWithValue("@Confirmation", response.confirmationNumber);
                        cmd.Parameters.AddWithValue("@ProcessedDate", System.DateTime.Now);
                        cmd.Parameters.AddWithValue("@PrintedDate", System.DateTime.Now);
                        cmd.Parameters.AddWithValue("@InsertedDate", System.DateTime.Now);
                        cmd.Parameters.AddWithValue("@PolicyNumber", response.Request.policyNumber);
                        cmd.Parameters.AddWithValue("@UserID", response.DiamondUserId);
                        cmd.Parameters.AddWithValue("@EOD", false);
                        cmd.Parameters.AddWithValue("@UserName", this.eCheckUserName);
                        cmd.Parameters.AddWithValue("@AccountBillNumber", response.Request.AccountBillAccountNumber);

                        //added 7/24/2020 for Fiserv
                        if (response.PaymentId > 0)
                        {
                            cmd.Parameters.AddWithValue("@fiservPaymentId", response.PaymentId);
                        }
                        cmd.Parameters.AddWithValue("@excludeFromBankFile", IFM_CreditCardProcessing.Common.BooleanToInt(response.Request.ExcludeFromBankFile));
                        if (response.Request.bankAccoutType > 0) //note: what's stored in ECheckAfterHoursPayments table for PaymentTypeId should already be using the values from Diamond's BankAccountType table (-1=blank, 0=N/A, 1=Checking, 2=Savings) and not IFM_Reports' EcheckPaymentTypes table (1=Savings, 2=Checking; doesn't appear to be used at all)
                        {
                            cmd.Parameters.AddWithValue("@PaymentTypeID", response.Request.bankAccoutType);
                        }

                        cmd.Parameters.Add("@ECheckPaymentID", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();
                        int identity = Convert.ToInt32(cmd.Parameters["@ECheckPaymentID"].Value.ToString());
                        return identity;

                    }
                }
            }
            catch (Exception err)
            {
#if DEBUG
                Debugger.Break();
#else
                IFMErrorLogging.LogException(err, "IFMITDATASERVICES -> eCheckProcessing -> insertToDB");
#endif
                // if it fails just return false and stop further progression
            }
            return 0;

        }

//        private int insertToDB(int eChecksUserID, int EFTaccountID, eCheckProcessResponse response, int fiservPaymentId = 0, bool excludeFromBankFile = false, string confirmationNumber = "", int paymentTypeId = 0) //added optional params 7/24/2020 for Fiserv
//        {
//            try
//            {
//                using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
//                {
//                    conn.Open();
//                    using (var cmd = new System.Data.SqlClient.SqlCommand("IFM_Reports.dbo.Usp_ECheckPayments_Insert", conn) { CommandType = System.Data.CommandType.StoredProcedure })
//                    {
//                        cmd.CommandType = CommandType.StoredProcedure;
//                        cmd.Parameters.AddWithValue("@PolicyID", response.Request.polID);
//                        cmd.Parameters.AddWithValue("@EFTAccountID", EFTaccountID);
//                        cmd.Parameters.AddWithValue("@SourceID", this.request.source);
//                        cmd.Parameters.AddWithValue("@ProcessedFlagID", 1);
//                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(this.request.paymentamount));
//                        //cmd.Parameters.AddWithValue("@Confirmation", 10); // was 0 and it didn't like it Matt A 8-29-17
//                        //updated 7/24/2020
//                        if (string.IsNullOrWhiteSpace(confirmationNumber) == true)
//                        {
//                            confirmationNumber = "10";
//                        }
//                        cmd.Parameters.AddWithValue("@Confirmation", confirmationNumber);
//                        cmd.Parameters.AddWithValue("@ProcessedDate", System.DateTime.Now);
//                        cmd.Parameters.AddWithValue("@PrintedDate", System.DateTime.Now);
//                        cmd.Parameters.AddWithValue("@InsertedDate", System.DateTime.Now);
//                        cmd.Parameters.AddWithValue("@PolicyNumber", response.Request.policyNumber);
//                        cmd.Parameters.AddWithValue("@UserID", eChecksUserID);
//                        cmd.Parameters.AddWithValue("@EOD", false);
//                        cmd.Parameters.AddWithValue("@UserName", this.eCheckUserName);
//                        cmd.Parameters.AddWithValue("@AccountBillNumber", response.Request.AccountBillAccountNumber);

//                        //added 7/24/2020 for Fiserv
//                        if (fiservPaymentId > 0)
//                        {
//                            cmd.Parameters.AddWithValue("@fiservPaymentId", fiservPaymentId);
//                        }
//                        cmd.Parameters.AddWithValue("@excludeFromBankFile", IFM_CreditCardProcessing.Common.BooleanToInt(excludeFromBankFile));
//                        if (paymentTypeId > 0) //note: what's stored in ECheckAfterHoursPayments table for PaymentTypeId should already be using the values from Diamond's BankAccountType table (-1=blank, 0=N/A, 1=Checking, 2=Savings) and not IFM_Reports' EcheckPaymentTypes table (1=Savings, 2=Checking; doesn't appear to be used at all)
//                        {
//                            cmd.Parameters.AddWithValue("@PaymentTypeID", paymentTypeId);
//                        }

//                        cmd.Parameters.Add("@ECheckPaymentID", SqlDbType.Int).Direction = ParameterDirection.Output;
//                        cmd.ExecuteNonQuery();
//                        int identity = Convert.ToInt32(cmd.Parameters["@ECheckPaymentID"].Value.ToString());
//                        return identity;

//                    }
//                }
//            }
//            catch (Exception err)
//            {
//#if DEBUG
//                Debugger.Break();
//#else
//                IFMErrorLogging.LogException(err);
//#endif
//                // if it fails just return false and stop further progression
//            }
//            return 0;

//        }

        private bool RealtimePostingNotAvailable()
        {
            return true;// IFM.DataServicesCore.BusinessLogic.Diamond.EOP.IsRunningPaymentBlockingPeriod(IFM.DataServicesCore.BusinessLogic.Diamond.EOP.EOPStatusDetails()) ?? false;
        }

        private int processAsAfterHours(eCheckProcessResponse response) //added optional params 7/24/2020 for Fiserv
        {
            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand("IFM_Reports.dbo.usp_ECheckAfterHoursPayments_Insert", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PolicyID", response.Request.polID);
                        cmd.Parameters.AddWithValue("@EFTAccountID", 0);
                        cmd.Parameters.AddWithValue("@SourceID", response.Request.source);
                        cmd.Parameters.AddWithValue("@ProcessedFlagID", 1);
                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(response.Request.paymentamount));
                        cmd.Parameters.AddWithValue("@PolicyNumber", response.Request.policyNumber);
                        cmd.Parameters.AddWithValue("@InsertedDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UserID", response.DiamondUserId);

                        cmd.Parameters.AddWithValue("@routingNumber", response.Request.aba.DoubleEncrypt());
                        cmd.Parameters.AddWithValue("@accountNumber", response.Request.bankAccountNumber.DoubleEncrypt());

                        cmd.Parameters.AddWithValue("@paymentTypeID", response.Request.bankAccoutType);
                        cmd.Parameters.AddWithValue("@emailAddress", response.Request.emailAddress);

                        cmd.Parameters.AddWithValue("@UserName", this.eCheckUserName);
                        cmd.Parameters.AddWithValue("@AccountBillNumber", response.Request.AccountBillAccountNumber);

                        //added 7/24/2020 for Fiserv
                        if (response.PaymentId > 0)
                        {
                            cmd.Parameters.AddWithValue("@fiservPaymentId", response.PaymentId);
                        }
                        cmd.Parameters.AddWithValue("@excludeFromBankFile", IFM_CreditCardProcessing.Common.BooleanToInt(response.Request.ExcludeFromBankFile));

                        cmd.Parameters.Add("@ECheckAfterHoursID", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();

                        int identity = Convert.ToInt32(cmd.Parameters["@ECheckAfterHoursID"].Value.ToString());
                        return identity;
                    }
                }
            }
            catch (Exception err)
            {
#if DEBUG
                Debugger.Break();
#else
                IFMErrorLogging.LogException(err, "IFMITDATASERVICES -> eCheckProcessing -> processAsAfterHours");
#endif
                // if it fails just return false and stop further progression
            }
            return 0;
        }

        private int processAsAfterHours(int eChecksUserID, int fiservPaymentId = 0, bool excludeFromBankFile = false) //added optional params 7/24/2020 for Fiserv
        {
            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand("IFM_Reports.dbo.usp_ECheckAfterHoursPayments_Insert", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PolicyID", this.request.polID);
                        cmd.Parameters.AddWithValue("@EFTAccountID", 0);
                        cmd.Parameters.AddWithValue("@SourceID", this.request.source);
                        cmd.Parameters.AddWithValue("@ProcessedFlagID", 1);
                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(this.request.paymentamount));
                        cmd.Parameters.AddWithValue("@PolicyNumber", this.request.policyNumber);
                        cmd.Parameters.AddWithValue("@InsertedDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UserID", eChecksUserID);

                        cmd.Parameters.AddWithValue("@routingNumber", this.request.aba.DoubleEncrypt());
                        cmd.Parameters.AddWithValue("@accountNumber", this.request.bankAccountNumber.DoubleEncrypt());

                        cmd.Parameters.AddWithValue("@paymentTypeID", this.request.bankAccoutType);
                        cmd.Parameters.AddWithValue("@emailAddress", this.request.emailAddress);

                        cmd.Parameters.AddWithValue("@UserName", this.eCheckUserName);
                        cmd.Parameters.AddWithValue("@AccountBillNumber", this.request.AccountBillAccountNumber);

                        //added 7/24/2020 for Fiserv
                        if (fiservPaymentId > 0)
                        {
                            cmd.Parameters.AddWithValue("@fiservPaymentId", fiservPaymentId);
                        }
                        cmd.Parameters.AddWithValue("@excludeFromBankFile", IFM_CreditCardProcessing.Common.BooleanToInt(excludeFromBankFile));

                        cmd.Parameters.Add("@ECheckAfterHoursID", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();

                        int identity = Convert.ToInt32(cmd.Parameters["@ECheckAfterHoursID"].Value.ToString());
                        return identity;

                    }
                }
            }
            catch (Exception err)
            {
#if DEBUG
                Debugger.Break();
#else
                IFMErrorLogging.LogException(err, "IFMITDATASERVICES -> eCheckProcessing -> processAsAfterHours");
#endif
                // if it fails just return false and stop further progression
            }
            return 0;
        }

    }


    public class eCheckProcessRequest
    {
        public enum bank_account_type
        {
            checking = 1,
            savings = 2
        }

        readonly private string _requestID = Guid.NewGuid().ToString();

        public string RequestID
        {
            get { return _requestID; }
        }

        //1=policyholder,2=agent,3=staff++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public int source;
        public global::IFM.DataServices.API.Enums.PaymentInterface PaymentInterface;

        public string agentUserName;

        public string agentPassword;

        public string policyHolderUserName;
        public string policyNumber;
        public string paymentamount;
        public string aba;
        public string bankAccountNumber;
        public string AccountBillAccountNumber;
        public bool ExcludeFromBankFile;

        //1=checking, 2=savings
        public bank_account_type bankAccoutType;

        public Int32 polID;
        public Int32 policyImageNum;

        public string emailAddress = "";

        public eCheckProcessRequest(string policyHolderUserName, string policynumber, string paymentamount, string aba, string bankaccountNumber, eCheckProcessRequest.bank_account_type bankAccountType, Int32 polID, Int32 polImageNumber, string AccountBillAccountNumber = "")
        {
            this.policyHolderUserName = policyHolderUserName;
            this.policyNumber = policynumber;
            this.polID = polID;
            this.policyImageNum = polImageNumber;
            this.paymentamount = paymentamount;
            this.aba = aba;
            this.bankAccountNumber = bankaccountNumber;
            this.bankAccoutType = bankAccountType;
            this.AccountBillAccountNumber = AccountBillAccountNumber;
            this.ExcludeFromBankFile = true;
            source = 1;
        }

        public eCheckProcessRequest(global::IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentData)
        {
            this.policyHolderUserName = paymentData.UserName;
            this.policyNumber = paymentData.PolicyNumber;
            this.polID = paymentData.PolicyId;
            this.policyImageNum = paymentData.ImageNumber;
            this.paymentamount = paymentData.PaymentAmount.ToString();
            this.aba = paymentData.ECheckPaymentInformation.RoutingNumber;
            this.bankAccountNumber = paymentData.ECheckPaymentInformation.AccountNumber;
            this.bankAccoutType = (eCheckProcessRequest.bank_account_type)paymentData.ECheckPaymentInformation.AccountType;
            this.AccountBillAccountNumber = paymentData.AccountBillNumber;
            this.source = (int)paymentData.UserType; // 1 = policyholder, 2 = agent, 3 = staff
            this.PaymentInterface = paymentData.PaymentInterface;
            this.ExcludeFromBankFile = paymentData.ECheckPaymentInformation.ExcludeFromBankFile == global::IFM.DataServices.API.Enums.BankFileExclusion.True ? true : false;
            if (string.IsNullOrWhiteSpace(paymentData.EmailAddress) == false) //added 8/18/2020
            {
                this.emailAddress = paymentData.EmailAddress;
            }
        }

        private eCheckProcessRequest()
        {
        }


        public static bank_account_type Convert_OMP_LIB_BankAccountType(Int32 ompBankTypeAsInt)
        {
            //omp type is 0 based this dlls type is 1 based
            return (bank_account_type)(ompBankTypeAsInt + 1);
        }



    }

    public class eCheckProcessResponse
    {
        public string confirmationNumber;
        public List<string> errorMsgs = new List<string>();
        public List<string> DetailedErrorMsgs = new List<string>();

        public string Errors
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (string Err in errorMsgs)
                {
                    sb.AppendLine(Err);
                }
                return sb.ToString();
            }
        }

        public List<string> userFeedBack = new List<string>();

        public bool fullyCompleted = false;

        internal int DiamondUserId { get; set; }
        internal int EftAccountId { get; set; }
        internal int PaymentId { get; set; }

        //public bool afterhoursFullyCompleted = false;
        readonly private eCheckProcessRequest _request;

        public eCheckProcessRequest Request
        {
            get { return _request; }
        }

        public eCheckProcessResponse(eCheckProcessRequest request)
        {
            _request = request;
        }
    }

    public class PriorEcheckPayments
    {
        public string AccountNumber { get; set; }
        public int AccountType { get; set; }
        public string PolicyNumber { get; set; }
        public double Amount { get; set; }
        public DateTime InsertedDate { get; set; }
        public string RoutingNumber { get; set; }
    }

}