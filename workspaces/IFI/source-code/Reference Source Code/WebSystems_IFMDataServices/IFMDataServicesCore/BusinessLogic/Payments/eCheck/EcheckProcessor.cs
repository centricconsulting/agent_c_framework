using Diamond.Business.ThirdParty.ISO.Passport.RCPOS.Objects;
using Diamond.Business.ThirdParty.SEP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.BusinessLogic.Payments.eCheck
{
    public class EcheckProcessor
    {
        private readonly string location = "IFMDataServicesCore.BusinessLogic.Payments.eCheck.EcheckProcessor";
        private readonly string eCheckUserName = AppConfig.PrintUserName;
        private readonly string echeckPassword = AppConfig.PrintUserPassword;
        private bool _isAfterHoursPayment = false;
        private bool _isAfterHoursChecked = false;
        private bool attemptedDiamondLogin { get; set; } = false;
        public EcheckRequest EcheckRequest { get; set; } = new EcheckRequest();
        public EcheckResponse EcheckResponse { get; set; } = new EcheckResponse();
        public AfterHoursEcheckInfo AfterHoursEcheckInfo { get; set; } = new AfterHoursEcheckInfo();

        public EcheckProcessor(global::IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentData)
        {
            this.EcheckRequest.UserLoginDomain = paymentData.UserLoginDomain;
            this.EcheckRequest.UserId = paymentData.UserId;
            this.EcheckRequest.Username = paymentData.Username;
            this.EcheckRequest.UserPassword = paymentData.UserPassword;
            this.EcheckRequest.PolicyNumber = paymentData.PolicyNumber;
            this.EcheckRequest.PolID = paymentData.PolicyId;
            this.EcheckRequest.PolicyImageNum = paymentData.PolicyImageNumber;
            this.EcheckRequest.PaymentAmount = paymentData.PaymentAmount.ToString();
            this.EcheckRequest.ABA = paymentData.ECheckPaymentInformation.RoutingNumber;
            this.EcheckRequest.BankAccountNumber = paymentData.ECheckPaymentInformation.AccountNumber;
            this.EcheckRequest.BankAccoutType = (EcheckRequest.bank_account_type)paymentData.ECheckPaymentInformation.AccountType;
            this.EcheckRequest.AccountBillAccountNumber = paymentData.AccountBillNumber;
            this.EcheckRequest.Source = (int)paymentData.UserType; // 1 = policyholder, 2 = agent, 3 = staff
            this.EcheckRequest.PaymentInterface = paymentData.PaymentInterface;
            this.EcheckRequest.ExcludeFromBankFile = paymentData.ECheckPaymentInformation.ExcludeFromBankFile == global::IFM.DataServices.API.Enums.BankFileExclusion.True ? true : false;
            this.EcheckRequest.StaffABABypass = paymentData.ECheckPaymentInformation.StaffABABypass;

            if (paymentData.PostPaymentInfo != null && paymentData.PostPaymentInfo.EcheckAfterHoursId > 0)
            {
                this.AfterHoursEcheckInfo.PaymentId = paymentData.PostPaymentInfo.PaymentId;
                this.AfterHoursEcheckInfo.eCheckAfterHoursId = paymentData.PostPaymentInfo.EcheckAfterHoursId;
                this.AfterHoursEcheckInfo.ProcessedFlagId = paymentData.PostPaymentInfo.EcheckProcessedFlagId;
                this.AfterHoursEcheckInfo.ConfirmationNumber = paymentData.PostPaymentInfo.ConfirmationNumber;
            }
            if (string.IsNullOrWhiteSpace(paymentData.EmailAddress) == false) //added 8/18/2020
            {
                this.EcheckRequest.EmailAddress = paymentData.EmailAddress;
            }

            DetermintePaymentInterfaceAndSourceIfNecessary();
        }

        public EcheckProcessor(IFM_CreditCardProcessing.Fiserv_WalletPaymentProcessor walletProcessor)
        {
            this.EcheckRequest.Username = walletProcessor.User.Username;
            this.EcheckRequest.PolicyNumber = walletProcessor.PolicyNumber;
            this.EcheckRequest.PolID = walletProcessor.PolicyId;
            this.EcheckRequest.PaymentAmount = walletProcessor.Amount.ToString();
            this.EcheckRequest.ABA = walletProcessor.CheckRoutingNumber;
            this.EcheckRequest.BankAccountNumber = walletProcessor.CheckAccountNumber;
            this.EcheckRequest.BankAccoutType = (EcheckRequest.bank_account_type)walletProcessor.CheckBankAccountTypeId;
            this.EcheckRequest.AccountBillAccountNumber = walletProcessor.AccountBillNumber;
            this.EcheckRequest.Source = (int)walletProcessor.User.UserType; // 1 = policyholder, 2 = agent, 3 = staff
            this.EcheckRequest.PaymentInterface = (IFM.DataServices.API.Enums.PaymentInterface)walletProcessor.PaymentInterface;
            this.EcheckRequest.ExcludeFromBankFile = true;

            if (string.IsNullOrWhiteSpace(walletProcessor.EmailAddress) == false) //added 8/18/2020
            {
                this.EcheckRequest.EmailAddress = walletProcessor.EmailAddress;
            }


            DetermintePaymentInterfaceAndSourceIfNecessary();
        }

        public bool AlreadyHasEcheckPaymentToday()
        {
            // you can't make two payments on the same policy with the same bank information on the same day
            Func<string, List<PriorEcheckPayments>> GetAllEcheckPayments = delegate (string polnum) {
                var priorPayments = new List<PriorEcheckPayments>();
                using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_ECheck_GetAllPayments", conn))
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
                                        AccountNumber = reader["AcctNum"].TryToGetString(),
                                        AccountType = reader["AccountTypeId"].TryToGetInt32(),
                                        Amount = Convert.ToDouble(reader["Amount"].TryToGetDecimal()),
                                        InsertedDate = reader["InsertedDate"].TryToGetDateTime(),
                                        PolicyNumber = reader["PolicyNumber"].TryToGetString(),
                                        RoutingNumber = reader["RoutingNum"].TryToGetString()
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
            return (from p in GetAllEcheckPayments(this.EcheckRequest.PolicyNumber) where p.InsertedDate.ToShortDateString() == DateTime.Now.ToShortDateString() && (p.AccountNumber == this.EcheckRequest.BankAccountNumber || this.EcheckRequest.BankAccountNumber.DoubleEncrypt() == p.AccountNumber) select p).Any();
        }

        public void ProcessEcheckPaymentAsAfterHours()
        {
            int afterHoursRecordId = processAsAfterHours(); //updated 7/24/2020 to send optional Fiserv params
            if (afterHoursRecordId > 0)
            {
                if (EcheckRequest.StaffABABypass)
                {
                    InsertABABypassToDB();
                }
                if (EcheckResponse.ConfirmationNumber.HasValue() == false)
                {
                    EcheckResponse.ConfirmationNumber = $"AH-{afterHoursRecordId}";
                }
                EcheckResponse.Completed = true;
            }
            else
            {
                string failMsg = $"Failed to process as after hours eCheck Payment.";
                global::IFM.IFMErrorLogging.LogIssue(failMsg, $"{location}.{nameof(ProcessEcheckPaymentAsAfterHours)}; {GetErrorPolicyInfo()}");
                global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(AppConfig.ErrorEmailAddress, AppConfig.NoReplyEmailAddress, "eCheck Payment Error", failMsg + GetErrorPolicyInfo(true));
                if (EcheckResponse.ConfirmationNumber.HasValue())
                {
                    //already successful through Fiserv; need to return Success
                    //EcheckResponse.ConfirmationNumber = EcheckResponse.ConfirmationNumber;
                    EcheckResponse.Completed = true;
                }
                else
                {
                    EcheckResponse.ErrorMsgs.Add("Failed to process as after hours eCheck Payment.");
                    EcheckResponse.UserFeedBack.Add("The billing service is temporarily unavailable. #y7837365265");
                }
            }
        }

        /// <summary>
        /// Will send payment to vendor, create EFTRecord in Diamond, Post payment to Diamond, and then store the record in IFM database.
        /// </summary>
        public void ProcessEcheckPayment()
        {
            bool continueWithPayment = true;
            if (this.EcheckRequest == null)
            {
                EcheckResponse.ErrorMsgs.Add("The provided request object was null.");
                EcheckResponse.UserFeedBack.Add("An unexpected system error occurred. Please try again later.#i5366");
#if DEBUG
                Debugger.Break();
#endif
                continueWithPayment = false;
            }

            if (AlreadyHasEcheckPaymentToday())
            {
#if DEBUG
                Debugger.Break();
#else
                    EcheckResponse.ErrorMsgs.Add("Only one echeck payment allowed per day per policy on the same bank account.");
                    EcheckResponse.UserFeedBack.Add("Only one echeck payment allowed per day per policy on the same bank account.");
                    continueWithPayment = false;
#endif
            }

            if (continueWithPayment)
            {
                Login();

                if (EcheckResponse.DiamondUserId > 0)
                {
                    MakeEcheckVendorPayment();

                    if (EcheckResponse.Completed)
                    {
                        ProcessPostVendorEcheckPayment();
                    }
                }
                else
                {
                    EcheckResponse.ErrorMsgs.Add("eCheckConfirm login user ERROR!");
                    EcheckResponse.UserFeedBack.Add("The billing service is temporarily unavailable. #j8945");
                    global::IFM.IFMErrorLogging.LogIssue("Failed to login for eCheck Payment.", location + ".ProcesseCheckRequest;" + GetErrorPolicyInfo());
                    global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(AppConfig.ErrorEmailAddress, AppConfig.NoReplyEmailAddress, "eCheck Payment Error", "Failed to login for eCheck Payment." + GetErrorPolicyInfo(true));
#if DEBUG
                    Debugger.Break();
#endif
                }
            }
        }
        
        /// <summary>
        /// Will create EFTRecord in Diamond, Post Payment to Diamond and store record in IFM database. Does not make payment to vendor.
        /// </summary>
        public void ProcessPostVendorEcheckPayment()
        {
            if (EcheckResponse.DiamondUserId <= 0 && this.attemptedDiamondLogin == false)
            {
                Login();
            }
            if (EcheckResponse.DiamondUserId > 0)
            {
                bool createEftAccount = true;
                if (EcheckRequest.ExcludeFromBankFile)
                {
                    if (this.EcheckRequest.ABA.HasValue() == false || this.EcheckRequest.BankAccountNumber.HasValue() == false)
                    {
                        createEftAccount = false;
                    }
                    else
                    {
                        //we have the info to create it; now see if we should
                        if (AppConfig.CreateEftAccountOnExcludeFromBankFile.TryToGetBoolean() == false)
                        {
                            createEftAccount = false;
                        }
                    }
                }

                if (createEftAccount == true)
                {
                    EcheckResponse.EftAccountId = CreateDiamondEftRecord();
                }

                if (EcheckResponse.EftAccountId > 0 || EcheckRequest.ExcludeFromBankFile == true)
                {
                    ApplyCash applyDiamondCash = new ApplyCash(this);
                    if (applyDiamondCash.DoApplyCash())
                    {
                        //int echeckTableId = insertToDB(diamondUserID, EftAccountId, EcheckResponse, fiservPaymentId: fiservPaymentId, excludeFromBankFile: EcheckRequest.ExcludeFromBankFile, ConfirmationNumber: ConfirmationNumber, paymentTypeId: (int)this.EcheckRequest.bankAccoutType);
                        if (IsAfterHoursPostToDiamond() && this.AfterHoursEcheckInfo.ConfirmationNumber.HasValue())
                        {
                            this.EcheckResponse.ConfirmationNumber = this.AfterHoursEcheckInfo.ConfirmationNumber;
                        }

                        int echeckTableId = InsertIntoEcheckPaymentsTable();
                        if (echeckTableId > 0)
                        {
                            if (EcheckRequest.StaffABABypass)
                            {
                                InsertABABypassToDB();
                            }
                            if (IsAfterHoursPostToDiamond())
                            {
                                UpdateECheckAfterHoursPayments(3, this.EcheckResponse.EftAccountId, this.AfterHoursEcheckInfo.eCheckAfterHoursId);
                            }
                            if (EcheckResponse.ConfirmationNumber.HasValue() == false) //added IF 7/24/2020; original logic in ELSE
                            {
                                EcheckResponse.ConfirmationNumber = SetPaymentConfirmationNumber();
                            }
                            EcheckResponse.Completed = (string.IsNullOrWhiteSpace(EcheckResponse.ConfirmationNumber)) ? false : true;
                        }
                        else
                        {                            
                            if (IsAfterHoursPostToDiamond())
                            {
                                string failMsg = $"Insert into Usp_ECheckPayments_Insert failed.";
                                string objStr = Newtonsoft.Json.JsonConvert.SerializeObject(this);
                                global::IFM.IFMErrorLogging.LogIssue(failMsg, location + ".ProcessPostVendorEcheckPayment;" + objStr);
                                global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(AppConfig.ErrorEmailAddress, AppConfig.NoReplyEmailAddress, "eCheck Payment Error", failMsg + "<br /><br />" + objStr);
                                UpdateECheckAfterHoursPayments(7, this.EcheckResponse.EftAccountId, this.AfterHoursEcheckInfo.eCheckAfterHoursId);
#if DEBUG
                                Debugger.Break();
#endif
                            }
                            else
                            {
                                string failMsg = $"Insert into Usp_ECheckPayments_Insert failed. Will attempt to process as after hours.";
                                global::IFM.IFMErrorLogging.LogIssue(failMsg, location + ".ProcessPostVendorEcheckPayment;" + GetErrorPolicyInfo());
                                global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(AppConfig.ErrorEmailAddress, AppConfig.NoReplyEmailAddress, "eCheck Payment Error", failMsg + GetErrorPolicyInfo(true));
                                if (!AttemptAfterHoursPost()) //Only attempt after hours if we are not attempting an after hours post right now.
                                {
                                    EcheckResponse.ErrorMsgs.Add("Could not add payment record to echeck table.");
                                    EcheckResponse.UserFeedBack.Add("The billing service is temporarily unavailable. #y69162385");
#if DEBUG
                                    Debugger.Break();
#endif
                                }
                            }
                            
                        }

                    }
                    else
                    {
                        if (IsAfterHoursPostToDiamond())
                        {
                            string failMsg = $"Could not apply cash for AfterHours process.";
                            global::IFM.IFMErrorLogging.LogIssue(failMsg, location + ".ProcessPostVendorEcheckPayment; " + applyDiamondCash.ErrorMessage + " " + GetErrorPolicyInfo());
                            global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(AppConfig.ErrorEmailAddress, AppConfig.NoReplyEmailAddress, "eCheck Payment Error", failMsg + "<br /><br />" + applyDiamondCash.ErrorMessage + "<br /><br />" + GetErrorPolicyInfo(true));
                            if (this.AfterHoursEcheckInfo.ProcessedFlagId == 1)
                            {
                                UpdateECheckAfterHoursPayments(4, this.EcheckResponse.EftAccountId, this.AfterHoursEcheckInfo.eCheckAfterHoursId);
                            }
                            else if (this.AfterHoursEcheckInfo.ProcessedFlagId == 4)
                            {
                                UpdateECheckAfterHoursPayments(5, this.EcheckResponse.EftAccountId, this.AfterHoursEcheckInfo.eCheckAfterHoursId);
                            }
#if DEBUG
                            Debugger.Break();
#endif
                        }
                        else
                        {
                            if (!AttemptAfterHoursPost())
                            {
                                string failMsg = $"Could not apply cash.";
                                global::IFM.IFMErrorLogging.LogIssue(failMsg, location + ".ProcessPostVendorEcheckPayment; " + applyDiamondCash.ErrorMessage + " " + GetErrorPolicyInfo());
                                global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(AppConfig.ErrorEmailAddress, AppConfig.NoReplyEmailAddress, "eCheck Payment Error", failMsg + "<br /><br />" + applyDiamondCash.ErrorMessage + "<br /><br />" + GetErrorPolicyInfo(true));

                                EcheckResponse.ErrorMsgs.Add("Could not apply cash to policy!");
                                EcheckResponse.UserFeedBack.Add("The billing service is temporarily unavailable. #c9251423");
#if DEBUG
                                Debugger.Break();
#endif
                            }
                        }
                        
                    }

                }
                else
                {
                    if (IsAfterHoursPostToDiamond())
                    {
                        UpdateECheckAfterHoursPayments(6, this.AfterHoursEcheckInfo.eCheckAfterHoursId);
                        global::IFM.IFMErrorLogging.LogIssue("Could not create EFTID", location + ".ProcessPostVendorEcheckPayment;" + GetErrorPolicyInfo());
                        global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(AppConfig.ErrorEmailAddress, AppConfig.NoReplyEmailAddress, "EcheckAfterHours - eCheck Payment Error", "Could not create EFTID." + GetErrorPolicyInfo(true));
#if DEBUG
                        Debugger.Break();
#endif
                    }
                    else
                    {
                        global::IFM.IFMErrorLogging.LogIssue("Could not create EFTID. Will attempt to process as after hours.", location + ".ProcessPostVendorEcheckPayment;" + GetErrorPolicyInfo());
                        global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(AppConfig.ErrorEmailAddress, AppConfig.NoReplyEmailAddress, "eCheck Payment Error", $"Could not create EFTID. Will attempt to process as after hours." + GetErrorPolicyInfo(true));
                        if (!AttemptAfterHoursPost())
                        {
                            EcheckResponse.ErrorMsgs.Add("Could not create EFTID.");
                            EcheckResponse.UserFeedBack.Add("The billing service is temporarily unavailable. #a23643");

#if DEBUG
                            Debugger.Break();
#endif
                        }
                    }
                }
            }
            else
            {
                EcheckResponse.ErrorMsgs.Add("eCheckConfirm login user ERROR!");
                EcheckResponse.UserFeedBack.Add("The billing service is temporarily unavailable. #j8945");
                global::IFM.IFMErrorLogging.LogIssue("Failed to login for eCheck Payment.", location + ".ProcessPostVendorEcheckPayment");
                global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(AppConfig.ErrorEmailAddress, AppConfig.NoReplyEmailAddress, "eCheck Payment Error", "Failed to login for eCheck Payment.");
#if DEBUG
                Debugger.Break();
#endif
            }
        }

        private void DetermintePaymentInterfaceAndSourceIfNecessary()
        {
            if (EcheckRequest.PaymentInterface == DataServices.API.Enums.PaymentInterface.None || EcheckRequest.Source <= 0)
            {
                if (EcheckRequest.PaymentInterface == DataServices.API.Enums.PaymentInterface.None && EcheckRequest.Username == "IVR_User")
                {
                    EcheckRequest.PaymentInterface = DataServices.API.Enums.PaymentInterface.IVR_Pay_By_Phone;
                }

                if (EcheckRequest.Source <= 0 && EcheckRequest.PaymentInterface != DataServices.API.Enums.PaymentInterface.None)
                {
                    switch (EcheckRequest.PaymentInterface)
                    {
                        case DataServices.API.Enums.PaymentInterface.MemberPortalSite:
                        case DataServices.API.Enums.PaymentInterface.MobileApplications:
                        case DataServices.API.Enums.PaymentInterface.IVR_Pay_By_Phone:
                        case DataServices.API.Enums.PaymentInterface.MobileSite:
                        case DataServices.API.Enums.PaymentInterface.RecurringCreditCard:
                        case DataServices.API.Enums.PaymentInterface.ConsumerQuotingSite:
                        case DataServices.API.Enums.PaymentInterface.OneTimePayment:
                        case DataServices.API.Enums.PaymentInterface.MobileOneTimePayment:
                            EcheckRequest.Source = 1;
                            break;
                        case DataServices.API.Enums.PaymentInterface.AgentsOnlySite:
                            EcheckRequest.Source = 2;
                            break;
                        case DataServices.API.Enums.PaymentInterface.StaffPaymentSite:
                        case DataServices.API.Enums.PaymentInterface.OneView:
                            EcheckRequest.Source = 3;
                            break;
                        default:
                            EcheckRequest.Source = 0;
                            break;
                    }
                }

                if (EcheckRequest.Source > 0 && EcheckRequest.PaymentInterface == DataServices.API.Enums.PaymentInterface.None)
                {
                    switch (EcheckRequest.Source)
                    {
                        case 1:
                            EcheckRequest.PaymentInterface = DataServices.API.Enums.PaymentInterface.MemberPortalSite;
                            break;
                        case 2:
                            EcheckRequest.PaymentInterface = DataServices.API.Enums.PaymentInterface.AgentsOnlySite;
                            break;
                        case 3:
                            EcheckRequest.PaymentInterface = DataServices.API.Enums.PaymentInterface.OneView;
                            break;
                    }
                }

                if (EcheckRequest.Source == 0 && EcheckRequest.PaymentInterface == DataServices.API.Enums.PaymentInterface.None)
                {
                    IFM.IFMErrorLogging.LogIssue("Could not determine Source and PaymentInterface.", location + ".DetermintePaymentInterfaceAndSourceIfNecessary;");
                }
            }
        }

        private string GetErrorPolicyInfo(bool useHTML = false)
        {
            StringBuilder errorInfo = new StringBuilder();

            errorInfo.Append(GetHtmlBreakOrNonHtmlSpace(useHTML));
            errorInfo.Append("PolicyNumber: " + this.EcheckRequest.PolicyNumber);
            errorInfo.Append(GetHtmlBreakOrNonHtmlSpace(useHTML));
            errorInfo.Append("PolicyId: " + this.EcheckRequest.PolID);
            errorInfo.Append(GetHtmlBreakOrNonHtmlSpace(useHTML));
            errorInfo.Append("DiamondUserId: " + this.EcheckResponse.DiamondUserId);
            errorInfo.Append(GetHtmlBreakOrNonHtmlSpace(useHTML));
            errorInfo.Append("Amount: " + this.EcheckRequest.PaymentAmount);

            if(AfterHoursEcheckInfo.eCheckAfterHoursId > 0)
            {
                errorInfo.Append(GetHtmlBreakOrNonHtmlSpace(useHTML));
                errorInfo.Append("ECheckAfterHoursId: " + AfterHoursEcheckInfo.eCheckAfterHoursId.ToString());
            }
            if (EcheckResponse.PaymentId > 0)
            {
                errorInfo.Append(GetHtmlBreakOrNonHtmlSpace(useHTML));
                errorInfo.Append("FiservPaymentId: " + EcheckResponse.PaymentId.ToString());
            }
            if (EcheckResponse.ConfirmationNumber.HasValue())
            {
                errorInfo.Append(GetHtmlBreakOrNonHtmlSpace(useHTML));
                errorInfo.Append("ConfirmationNumber: " + EcheckResponse.ConfirmationNumber);
            }
            if (EcheckResponse.EftAccountId > 0)
            {
                errorInfo.Append(GetHtmlBreakOrNonHtmlSpace(useHTML));
                errorInfo.Append("EFTAccountId: " + this.EcheckResponse.EftAccountId);
            }

            return errorInfo.ToString();
        }

        private string GetHtmlBreakOrNonHtmlSpace(bool useHtml = false, string stringToUse = ";")
        {
            if (useHtml)
            {
                return "<br />";
            }
            else
            {
                return stringToUse;
            }
        }

        private bool AttemptAfterHoursPost()
        {
            ProcessEcheckPaymentAsAfterHours();
            return this.EcheckResponse.Completed;
        }

        private void MakeEcheckVendorPayment()
        {
            if (EcheckResponse.DiamondUserId > 0)
            {
                bool sendToFiserv = IFM_CreditCardProcessing.Common.ECheck_SendRealtimePaymentsToVendor();
                //added 7/24/2020 for Fiserv
                //bool okayToProceed = true;
                if (sendToFiserv == true)
                {
                    var EcheckInfo = eCheckRequestToCCLibraryEcheckInfo();

                    IFM_CreditCardProcessing.FiservErrorObject errorObject = null;

                    IFM_CreditCardProcessing.CreditCardMethods ccm = new IFM_CreditCardProcessing.CreditCardMethods();
                    int fiservPaymentId = 0;
                    //if ()
                    //{

                    //}
                    bool successfullyProcessedWithFiserv = ccm.SuccessfullyProcessedFiservEcheckPayment(EcheckInfo, fiservPaymentId: ref fiservPaymentId, errorObject: ref errorObject);
                    EcheckResponse.PaymentId = fiservPaymentId;

                    if (successfullyProcessedWithFiserv == true)
                    {
                        //note: needs to return success to the user since the payment has been successfully processed
                        EcheckResponse.ConfirmationNumber = EcheckInfo.ConfirmationNumber;
                        EcheckResponse.Completed = true;
                    }
                    else
                    {
                        string failureMsg = $"Failed to process eCheck Payment.";
                        EcheckResponse.ErrorMsgs.Add(failureMsg);

                        if (EcheckInfo.PaymentResponseMessage.IsNotNullEmptyOrWhitespace())
                        {
                            EcheckResponse.DetailedErrorMsgs.Add(EcheckInfo.PaymentResponseMessage);
                        }
                        if (errorObject.ErrorMessage.IsNotNullEmptyOrWhitespace())
                        {
                            EcheckResponse.DetailedErrorMsgs.Add(errorObject.ErrorMessage);
                        }
                        if (errorObject.ExceptionMessage.IsNotNullEmptyOrWhitespace())
                        {
                            EcheckResponse.DetailedErrorMsgs.Add(errorObject.ExceptionMessage);
                        }
                        if (errorObject.ExceptionResponseStatusDescription.IsNotNullEmptyOrWhitespace())
                        {
                            EcheckResponse.DetailedErrorMsgs.Add(errorObject.ExceptionResponseStatusDescription);
                        }
                        if (errorObject.ExceptionToString.IsNotNullEmptyOrWhitespace())
                        {
                            EcheckResponse.DetailedErrorMsgs.Add(errorObject.ExceptionToString);
                        }
                        if (errorObject.FiservErrorType != IFM_CreditCardProcessing.Enums.Fiserv_JsonTransaction_ErrorType.None)
                        {
                            EcheckResponse.DetailedErrorMsgs.Add(errorObject.FiservErrorType.ToString());
                        }
                        if (errorObject.ExceptionResponseStatusCode != 0)
                        {
                            EcheckResponse.DetailedErrorMsgs.Add(errorObject.ExceptionResponseStatusCode.ToString());
                        }
                        if (errorObject.ExceptionResponseStream.IsNotNullEmptyOrWhitespace())
                        {
                            EcheckResponse.DetailedErrorMsgs.Add(errorObject.ExceptionResponseStream);
                        }

                        EcheckResponse.UserFeedBack.Add(failureMsg);
                    }
                }
            }
            else
            {
                EcheckResponse.ErrorMsgs.Add("eCheckConfirm login user ERROR!");
                EcheckResponse.UserFeedBack.Add("The billing service is temporarily unavailable. #j8945");
                global::IFM.IFMErrorLogging.LogIssue("Failed to login for eCheck Payment.", location + ".ProcesseCheckRequest;" + GetErrorPolicyInfo());
                global::IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(AppConfig.ErrorEmailAddress, AppConfig.NoReplyEmailAddress, "eCheck Payment Error", "Failed to login for eCheck Payment." + GetErrorPolicyInfo(true));
#if DEBUG
                Debugger.Break();
#endif
            }
        }

        private void InsertABABypassToDB()
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_Insert_EcheckABAOverride", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EcheckId", this.EcheckResponse.eCheckTableId);
                    cmd.Parameters.AddWithValue("@PolicyID", this.EcheckRequest.PolID);
                    cmd.Parameters.AddWithValue("@UserID", this.EcheckResponse.DiamondUserId);
                    cmd.Parameters.AddWithValue("@RoutingNumber", this.EcheckRequest.ABA);
                    cmd.Parameters.AddWithValue("@PolicyNumber", this.EcheckRequest.PolicyNumber);
                    cmd.Parameters.AddWithValue("@UserName", this.eCheckUserName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private int InsertIntoEcheckPaymentsTable() //added optional params 7/24/2020 for Fiserv
        {
            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.Usp_ECheckPayments_Insert", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PolicyID", this.EcheckRequest.PolID);
                        cmd.Parameters.AddWithValue("@EFTAccountID", this.EcheckResponse.EftAccountId);
                        cmd.Parameters.AddWithValue("@SourceID", this.EcheckRequest.Source);
                        cmd.Parameters.AddWithValue("@ProcessedFlagID", 1);
                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(this.EcheckRequest.PaymentAmount));
                        //cmd.Parameters.AddWithValue("@Confirmation", 10); // was 0 and it didn't like it Matt A 8-29-17
                        //updated 7/24/2020
                        if (this.EcheckResponse.ConfirmationNumber.IsNullEmptyOrWhitespace())
                        {
                            this.EcheckResponse.ConfirmationNumber = "10";
                        }
                        cmd.Parameters.AddWithValue("@Confirmation", this.EcheckResponse.ConfirmationNumber);
                        cmd.Parameters.AddWithValue("@ProcessedDate", System.DateTime.Now);
                        cmd.Parameters.AddWithValue("@PrintedDate", System.DateTime.Now);
                        cmd.Parameters.AddWithValue("@InsertedDate", System.DateTime.Now);
                        cmd.Parameters.AddWithValue("@PolicyNumber", this.EcheckRequest.PolicyNumber);

                        //if an agent or staff makes a payment, capture the diamond info... Members will log the username but the diamond id will be that of the internal echeck user.
                        if (this.EcheckRequest.UserId > 0)
                        {
                            cmd.Parameters.AddWithValue("@UserID", this.EcheckRequest.UserId);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@UserID", this.EcheckResponse.DiamondUserId);
                        }

                        if (this.EcheckRequest.Username.HasValue())
                        {
                            cmd.Parameters.AddWithValue("@UserName", this.EcheckRequest.Username.TruncateString(50)); //table column max size is 50}
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@UserName", this.eCheckUserName);
                        }

                        cmd.Parameters.AddWithValue("@EOD", false);
                        cmd.Parameters.AddWithValue("@AccountBillNumber", this.EcheckRequest.AccountBillAccountNumber);

                        if (IsAfterHoursPostToDiamond() && this.AfterHoursEcheckInfo.PaymentId > 0)
                        {
                            cmd.Parameters.AddWithValue("@fiservPaymentId", this.AfterHoursEcheckInfo.PaymentId);
                        }
                        else
                        {
                            //added 7/24/2020 for Fiserv
                            if (this.EcheckResponse.PaymentId > 0)
                            {
                                cmd.Parameters.AddWithValue("@fiservPaymentId", this.EcheckResponse.PaymentId);
                            }
                        }
                        
                        cmd.Parameters.AddWithValue("@excludeFromBankFile", IFM_CreditCardProcessing.Common.BooleanToInt(this.EcheckRequest.ExcludeFromBankFile));
                        if (this.EcheckRequest.BankAccoutType > 0) //note: what's stored in ECheckAfterHoursPayments table for PaymentTypeId should already be using the values from Diamond's BankAccountType table (-1=blank, 0=N/A, 1=Checking, 2=Savings) and not IFM_Reports' EcheckPaymentTypes table (1=Savings, 2=Checking; doesn't appear to be used at all)
                        {
                            cmd.Parameters.AddWithValue("@PaymentTypeID", this.EcheckRequest.BankAccoutType);
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
                IFMErrorLogging.LogException(err, location + ".insertToDB; SourceId: " + this.EcheckRequest.Source);
#endif
                // if it fails just return false and stop further progression
            }
            return 0;

        }

        private IFM_CreditCardProcessing.EcheckInfo eCheckRequestToCCLibraryEcheckInfo()
        {
            IFM_CreditCardProcessing.EcheckInfo EcheckInfo = new IFM_CreditCardProcessing.EcheckInfo();

            EcheckInfo.Amount = this.EcheckRequest.PaymentAmount.TryToGetDecimal();
            EcheckInfo.AccountBillNumber = this.EcheckRequest.AccountBillAccountNumber;
            EcheckInfo.PolicyNumber = this.EcheckRequest.PolicyNumber;
            EcheckInfo.PolicyId = this.EcheckRequest.PolID;
            EcheckInfo.EmailAddress = this.EcheckRequest.EmailAddress;

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
                lastName = this.EcheckRequest.PolicyNumber; //just defaulting something so it could potentially succeed in the case we can't pull insured info
            }

            IFM_CreditCardProcessing.Fiserv_CheckDetail checkDetail = new IFM_CreditCardProcessing.Fiserv_CheckDetail();
            EcheckInfo.RoutingNumber = this.EcheckRequest.ABA;
            EcheckInfo.CheckAccountNumber = this.EcheckRequest.BankAccountNumber;
            EcheckInfo.FirstName = firstName; //started using 8/18/2020
            EcheckInfo.LastName = lastName; //started using 8/18/2020

            if (this.EcheckRequest.BankAccoutType == EcheckRequest.bank_account_type.savings)
            {
                EcheckInfo.CheckAccountType = IFM_CreditCardProcessing.EcheckInfo.CheckAccountTypes.Saving;
            }
            else
            {
                EcheckInfo.CheckAccountType = IFM_CreditCardProcessing.EcheckInfo.CheckAccountTypes.Checking;
            }

            EcheckInfo.UserInfo = new IFM_CreditCardProcessing.UserInfo();
            EcheckInfo.UserInfo.Username = this.EcheckRequest.Username;
            
            if (this.EcheckRequest.PaymentInterface != IFM.DataServices.API.Enums.PaymentInterface.None)
            {
                EcheckInfo.PaymentInterface = (IFM_CreditCardProcessing.Enums.PaymentInterface)this.EcheckRequest.PaymentInterface;
                EcheckInfo.UserInfo.UserType = (IFM_CreditCardProcessing.Enums.UserType)this.EcheckRequest.Source;
            }
            else
            {
                if (this.EcheckRequest.Source == 3)
                {
                    EcheckInfo.PaymentInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.OneView;
                    EcheckInfo.UserInfo.UserType = IFM_CreditCardProcessing.Enums.UserType.Staff;
                }
                else
                {
                    EcheckInfo.PaymentInterface = (IFM_CreditCardProcessing.Enums.PaymentInterface)(int)this.EcheckRequest.PaymentInterface;
                    EcheckInfo.UserInfo.UserType = (IFM_CreditCardProcessing.Enums.UserType)this.EcheckRequest.Source;
                }
            }

            return EcheckInfo;
        }

        private int CreateDiamondEftRecord()
        {
            DCO.EFT.Eft eftInfo = new DCO.EFT.Eft
            {
                PolicyId = this.EcheckRequest.PolID,
                PolicyImageNum = this.EcheckRequest.PolicyImageNum,
                RoutingNumber = this.EcheckRequest.ABA,
                AccountNumber = this.EcheckRequest.BankAccountNumber,
                BankAccountTypeId = (int)this.EcheckRequest.BankAccoutType,
                DeductionDay = System.DateTime.Now.Day
            };
            return IFM.DataServicesCore.BusinessLogic.Payments.PayPlanHelper.CreateEFTAccount(eftInfo);
        }

        private void Login()
        {
            EcheckResponse.DiamondUserId = -1;
            this.attemptedDiamondLogin = true;
            if (this.EcheckRequest.Source.IsNotNullEmptyOrWhitespace())
            {
                try
                {
                    switch (this.EcheckRequest.Source)
                    {
                        case 1:
                            BusinessLogic.Diamond.Login.LoginNow(eCheckUserName, echeckPassword);
                            break;
                        case 2:
                            if (this.EcheckRequest.Username.IsNotNullEmptyOrWhitespace() && this.EcheckRequest.UserPassword.IsNotNullEmptyOrWhitespace())
                            {
                                BusinessLogic.Diamond.Login.LoginNow(this.EcheckRequest.Username, this.EcheckRequest.UserPassword);
                            }
                            else
                            {
                                BusinessLogic.Diamond.Login.LoginNow(eCheckUserName, echeckPassword);
                            }
                            break;
                        case 3:
                            BusinessLogic.Diamond.Login.LoginNow(eCheckUserName, echeckPassword);
                            break;
                        default:
                            if (AfterHoursEcheckInfo.eCheckAfterHoursId > 0)
                            {
                                if (this.EcheckRequest.Username.IsNotNullEmptyOrWhitespace() && this.EcheckRequest.UserPassword.IsNotNullEmptyOrWhitespace())
                                {
                                    BusinessLogic.Diamond.Login.LoginNow(this.EcheckRequest.Username, this.EcheckRequest.UserPassword);
                                }
                                else
                                {
                                    BusinessLogic.Diamond.Login.LoginNow(eCheckUserName, echeckPassword);
                                }
                            }
                            else
                            {
                                this.EcheckResponse.ErrorMsgs.Add("echeck failed to log into the system.");
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    IFMErrorLogging.LogException(ex, location + ".Login;");
                    this.EcheckResponse.ErrorMsgs.Add("echeck failed to log into the system.");
                }
            }
            else
            {
                try
                {
                    BusinessLogic.Diamond.Login.LoginNow(this.EcheckRequest.Username, this.EcheckRequest.UserPassword);
                }
                catch (Exception ex)
                {
                    IFMErrorLogging.LogException(ex, location + ".Login; EcheckRequest.Source is null;");
                    this.EcheckResponse.ErrorMsgs.Add("echeck failed to log into the system.");
                }
            }

            try
            {
                EcheckResponse.DiamondUserId = IFM.DataServicesCore.BusinessLogic.Diamond.Login.GetUserId();
            }
            catch (Exception ex)
            {
                IFMErrorLogging.LogException(ex, location + ".Login;");
                this.EcheckResponse.ErrorMsgs.Add("echeck userid is invalid.");
            }
        }

        private int processAsAfterHours() //added optional params 7/24/2020 for Fiserv
        {
            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_ECheckAfterHoursPayments_Insert", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PolicyID", this.EcheckRequest.PolID);
                        cmd.Parameters.AddWithValue("@EFTAccountID", 0);
                        cmd.Parameters.AddWithValue("@SourceID", this.EcheckRequest.Source);
                        cmd.Parameters.AddWithValue("@ProcessedFlagID", 1);
                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(this.EcheckRequest.PaymentAmount));
                        cmd.Parameters.AddWithValue("@PolicyNumber", this.EcheckRequest.PolicyNumber);
                        cmd.Parameters.AddWithValue("@InsertedDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UserID", this.EcheckResponse.DiamondUserId);

                        cmd.Parameters.AddWithValue("@routingNumber", this.EcheckRequest.ABA.DoubleEncrypt());
                        cmd.Parameters.AddWithValue("@accountNumber", this.EcheckRequest.BankAccountNumber.DoubleEncrypt());

                        cmd.Parameters.AddWithValue("@paymentTypeID", this.EcheckRequest.BankAccoutType);
                        cmd.Parameters.AddWithValue("@emailAddress", this.EcheckRequest.EmailAddress);

                        if (this.EcheckRequest.Username.HasValue())
                        {
                            cmd.Parameters.AddWithValue("@UserName", this.EcheckRequest.Username.TruncateString(50)); //table column max size is 50
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@UserName", this.eCheckUserName);
                        }
                        
                        cmd.Parameters.AddWithValue("@AccountBillNumber", this.EcheckRequest.AccountBillAccountNumber);

                        //added 7/24/2020 for Fiserv
                        if (this.EcheckResponse.PaymentId > 0)
                        {
                            cmd.Parameters.AddWithValue("@fiservPaymentId", this.EcheckResponse.PaymentId);
                        }
                        cmd.Parameters.AddWithValue("@excludeFromBankFile", IFM_CreditCardProcessing.Common.BooleanToInt(this.EcheckRequest.ExcludeFromBankFile));

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
                IFMErrorLogging.LogException(err, location + ".processAsAfterHours;" + GetErrorPolicyInfo());
#endif
                // if it fails just return false and stop further progression
            }
            return 0;
        }

        private void UpdateECheckAfterHoursPayments(int NewProcessFlagId, int eftID, int eCheckAHId)
        {
            string updateString = "UPDATE ECheckAfterHoursPayments SET ProcessedFlagID = " + NewProcessFlagId + ", EFTAccountID = " + eftID + " WHERE ECheckAfterHoursID = " + eCheckAHId;
            SQLexecuteObject sqExecute = new SQLexecuteObject(AppConfig.ConnDiamondReports, updateString);
            sqExecute.ExecuteStatement();
            if (sqExecute.hasError)
            {
                IFM.IFMErrorLogging.LogIssue(sqExecute.errorMsg, location + ".UpdateECheckAfterHoursPayments(int, int, int);");
            }
        }

        private void UpdateECheckAfterHoursPayments(int NewProcessFlagId, int eCheckAHId)
        {
            string updateString = "UPDATE ECheckAfterHoursPayments SET ProcessedFlagID = " + NewProcessFlagId + " WHERE ECheckAfterHoursID = " + eCheckAHId;
            SQLexecuteObject sqExecute = new SQLexecuteObject(AppConfig.ConnDiamondReports, updateString);
            sqExecute.ExecuteStatement();
            if (sqExecute.hasError)
            {
                IFM.IFMErrorLogging.LogIssue(sqExecute.errorMsg, location + ".UpdateECheckAfterHoursPayments(int, int);");
            }
        }

        private bool IsAfterHoursPostToDiamond()
        {
            if (_isAfterHoursChecked)
            {
                return _isAfterHoursPayment;
            }
            else
            {
                _isAfterHoursChecked = true;
                _isAfterHoursPayment = (AfterHoursEcheckInfo != null && AfterHoursEcheckInfo.eCheckAfterHoursId > 0) ? true : false ;
                return _isAfterHoursPayment;
            }
        }

        private int GetAgencyAndInsuredInfo(ref string firstName, ref string lastName) //added 8/18/2020 to get more info than GetAgencyId
        {
            int agencyId = 0;
            firstName = "";
            lastName = "";

            using (PolicyNumberObject pol = new PolicyNumberObject(this.EcheckRequest.PolicyNumber))
            {
                pol.GetAllAgencyInfo = true;
                pol.GetAllInsuredInfo = true;
                if (this.EcheckRequest.PolID > 0)
                {
                    pol.GetPolicyInfo(PolicyNumberObject.PolicySystem.Diamond, this.EcheckRequest.PolID.ToString());
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

        private string SetPaymentConfirmationNumber()
        {
            try
            {
                string confirmationNumber;
                string ShortStyleConfirmationNumber = $"E-{EcheckResponse.EftAccountId}";
                string LongStyleConfirmationNumber = DateTime.Now.DayOfYear.ToString().PadLeft(3, '0');
                LongStyleConfirmationNumber += EcheckRequest.PolID.ToString().PadLeft(6, '0');
                LongStyleConfirmationNumber += EcheckResponse.DiamondUserId.ToString().PadLeft(5, '0');
                LongStyleConfirmationNumber += EcheckResponse.eCheckTableId.ToString().PadLeft(6, '0');

                if (EcheckRequest.PaymentInterface == DataServices.API.Enums.PaymentInterface.OneView || EcheckRequest.PaymentInterface == DataServices.API.Enums.PaymentInterface.MemberPortalSite)
                {
                    confirmationNumber = ShortStyleConfirmationNumber;
                }
                else
                {
                    confirmationNumber = LongStyleConfirmationNumber;
                }

                //update echecks table with confirmation number
                using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_SetConfirmationNumberForECheckPayment", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@ConfirmationNumber", confirmationNumber);
                        cmd.Parameters.AddWithValue("@eCheckId", EcheckResponse.eCheckTableId);
                        cmd.ExecuteNonQuery();
                        return confirmationNumber;
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetBankNameFromAbaLookUp(string routingNumber)
        {
            string returnVar = "";

            using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnQQ))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_get_BankNameFromAba", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@routing_number", routingNumber);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            returnVar = reader.GetString(reader.GetOrdinal("bank_name"));
                        }
                    }
                }
            }

            return returnVar;
        }
    }

    public class EcheckResponse
    {
        public string ConfirmationNumber;
        public List<string> ErrorMsgs = new List<string>();
        public List<string> DetailedErrorMsgs = new List<string>();
        public List<string> UserFeedBack = new List<string>();
        public bool Completed { get; set; } = false;
        internal int DiamondUserId { get; set; }
        internal int EftAccountId { get; set; }
        internal int PaymentId { get; set; }
        internal int eCheckTableId { get; set; }
    }

    public class EcheckRequest
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
        public int Source;
        public global::IFM.DataServices.API.Enums.PaymentInterface PaymentInterface;

        //public string AgentUserName;

        //public string AgentPassword;

        //public string PolicyHolderUserName;
        public string Username;
        public string UserPassword;
        public int UserId;
        public string UserLoginDomain;
        public string PolicyNumber;
        public string PaymentAmount;
        public string ABA;
        public string BankAccountNumber;
        public string AccountBillAccountNumber;
        public bool ExcludeFromBankFile;
        public bool StaffABABypass;

        //1=checking, 2=savings
        public bank_account_type BankAccoutType;

        public Int32 PolID;
        public Int32 PolicyImageNum;

        public string EmailAddress = "";
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

    public class AfterHoursEcheckInfo
    {
        public string ConfirmationNumber { get; set; }
        public int PaymentId { get; set; }
        public int eCheckAfterHoursId { get; set; }
        public int ProcessedFlagId { get; set; }
    }
}
