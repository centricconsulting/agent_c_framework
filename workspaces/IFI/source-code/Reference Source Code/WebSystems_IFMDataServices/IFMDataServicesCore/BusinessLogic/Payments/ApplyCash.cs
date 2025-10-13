using Diamond.Business.ThirdParty.ISO.Passport.RCPOS.Objects;
using Diamond.Business.ThirdParty.QwikSignAPI;
using Diamond.Common.Objects.Billing;
using IFM.DataServices.API.RequestObjects.Payments;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static IFM.DataServices.API.Enums;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.BusinessLogic.Payments
{
    public class ApplyCash
    {
        int AgencyId { get; set; }
        public int PolicyId { get; set; }
        public int PolicyImageNum { get; set; }
        public string PolicyNumber { get; set; }
        public decimal CashAmount { get; set; }
        public CashSource CashInSource { get; set; }
        public int CashType { get; set; } = 1; //payment
        public int ReasonId { get; set; } = 0; //none
        public int UsersId { get; set; }
        public int EftAccountId { get; set; }
        public string checkNum { get; set; }
        public string AccountBillNumber { get; set; }
        private string _errorMessage = string.Empty;
        public string ErrorMessage
        { 
            get 
            { 
                return _errorMessage; 
            } 
        }

        public ApplyCash(IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentData)
        {
            PolicyId = paymentData.PolicyId;
            PolicyImageNum = paymentData.PolicyImageNumber;
            PolicyNumber = paymentData.PolicyNumber;
            AccountBillNumber = paymentData.AccountBillNumber;
            CashInSource = paymentData.CashInSource;
            CashAmount = paymentData.PaymentAmount.TryToGetDecimal();
            CashType = 1; //Payment
            ReasonId = 0; //N/A
            UsersId = paymentData.UserId;
            AgencyId = paymentData.AgencyId;
        }

        public ApplyCash(AgencyEFTPayment eft)
        {
            PolicyId = eft.PolicyId;
            PolicyImageNum = eft.PolicyImageNumber;
            PolicyNumber = eft.PolicyNumber;
            AccountBillNumber = eft.AccountBillNumber;
            CashAmount = eft.Amount;
            CashInSource = eft.CashInSource;
            CashType = 1; //Payment
            ReasonId = 0; //N/A
            UsersId = eft.DiamondUserId;
            AgencyId = eft.AgencyId;
            EftAccountId = eft.AgencyEFTAccountId;
        }

        public ApplyCash(eCheck.EcheckProcessor eCheck)
        {
            PolicyId = eCheck.EcheckRequest.PolID;
            PolicyImageNum = eCheck.EcheckRequest.PolicyImageNum;
            PolicyNumber = eCheck.EcheckRequest.PolicyNumber;
            CashAmount = eCheck.EcheckRequest.PaymentAmount.TryToGetDecimal();
            CashInSource = CashSource.eCheck;
            CashType = 1; //Payment
            ReasonId = 0; //N/A
            if (eCheck.EcheckRequest.UserId > 0)
            {
                UsersId = eCheck.EcheckRequest.UserId;
            }
            else
            {
                UsersId = eCheck.EcheckResponse.DiamondUserId;
            }
            EftAccountId = eCheck.EcheckResponse.EftAccountId;
            AccountBillNumber = eCheck.EcheckRequest.AccountBillAccountNumber;
        }

        public ApplyCash(IFM_CreditCardProcessing.CreditCardTransaction cct)
        {
            PolicyId = cct.PolicyInfo.PolicyId.HasValue() ? cct.PolicyInfo.PolicyId: cct.PolicyId;
            PolicyImageNum = cct.PolicyInfo.PolicyImageNum.HasValue() ? cct.PolicyInfo.PolicyImageNum : cct.PolicyImageNum;
            PolicyNumber = cct.PolicyInfo.PolicyNumber.HasValue() ? cct.PolicyInfo.PolicyNumber : cct.PolicyNumber;
            CashAmount = cct.PaymentAmount.TryToGetDecimal();
            CashType = 1; //Payment
            ReasonId = 0; //N/A
            UsersId = cct.DiamondUserId;
            AccountBillNumber = cct.PolicyInfo.AccountBillNumber;
            if (cct.CreditCardInfo.CreditCardType != IFM_CreditCardProcessing.Enums.CreditCardType.None)
            {
                CashInSource = DetermineCashSourceForCreditCard(cct.CreditCardInfo.CreditCardType, cct.PaymentInterface);
            }
            else
            {
                CashInSource = DetermineCashSourceForCreditCard(cct.CreditCardType, cct.PaymentInterface);
            }
        }

        public ApplyCash(IFM_CreditCardProcessing.Fiserv_WalletPaymentProcessor walletPayment)
        {
            PolicyId = walletPayment.PolicyId;
            PolicyNumber = walletPayment.PolicyNumber;
            CashAmount = walletPayment.Amount;
            CashType = 1; //Payment
            ReasonId = 0; //N/A
            UsersId = walletPayment.User.DiamondUserId;
            AccountBillNumber = walletPayment.AccountBillNumber;
            if (walletPayment.FundingMethod == "ACH")
            {
                CashInSource = CashSource.eCheck;
            }
            else
            {
                bool hasPreferred = false;
                IFM_CreditCardProcessing.Enums.CreditCardType ccType = IFM_CreditCardProcessing.Common.PreferredCreditCardTypeForFiservPaymentMethodOrExisting(walletPayment.FundingMethod, walletPayment.CCType, ref hasPreferred);
                CashInSource = DetermineCashSourceForCreditCard(ccType, walletPayment.PaymentInterface);
            }
        }

        private CashSource DetermineCashSourceForCreditCard(IFM_CreditCardProcessing.Enums.CreditCardType ccType, IFM_CreditCardProcessing.Enums.PaymentInterface pmtInterface)
        {
            CashSource returnVar = CashSource.NA;
            if (ccType != IFM_CreditCardProcessing.Enums.CreditCardType.None)
            {
                switch (ccType)
                {
                    case IFM_CreditCardProcessing.Enums.CreditCardType.AmericanExpress:
                        returnVar = pmtInterface == IFM_CreditCardProcessing.Enums.PaymentInterface.AgentsOnlySite ? CashSource.WebAgencyCCAmericanExpress : CashSource.WebCCAmericanExpress;
                        break;
                    case IFM_CreditCardProcessing.Enums.CreditCardType.Discover:
                        returnVar = pmtInterface == IFM_CreditCardProcessing.Enums.PaymentInterface.AgentsOnlySite ? CashSource.WebAgencyCCDiscover : CashSource.WebCCDiscover;
                        break;
                    case IFM_CreditCardProcessing.Enums.CreditCardType.MasterCard:
                        returnVar = pmtInterface == IFM_CreditCardProcessing.Enums.PaymentInterface.AgentsOnlySite ? CashSource.WebAgencyCCMasterCard : CashSource.WebCCMasterCard;
                        break;
                    case IFM_CreditCardProcessing.Enums.CreditCardType.Visa:
                        returnVar = pmtInterface == IFM_CreditCardProcessing.Enums.PaymentInterface.AgentsOnlySite ? CashSource.WebAgencyCCVisa : CashSource.WebCCVisa;
                        break;
                    default:
                        returnVar = CashSource.CreditCard;
                        break;
                }
            }
            return returnVar;
        }

        public global::IFM.DataServices.API.ResponseObjects.Common.ServiceResult<global::IFM.DataServices.API.ResponseObjects.Payments.PaymentResult> DoApplyCashServiceResult()
        {
            global::IFM.DataServices.API.ResponseObjects.Common.ServiceResult<global::IFM.DataServices.API.ResponseObjects.Payments.PaymentResult> sr = new global::IFM.DataServices.API.ResponseObjects.Common.ServiceResult<global::IFM.DataServices.API.ResponseObjects.Payments.PaymentResult>();

            var appliedSuccessfully = DoApplyCash();
            
            if (appliedSuccessfully)
            {
                sr.ResponseData.PaymentCompleted = true;
            }
            else
            {
                sr.Messages.CreateErrorMessage("Unable to apply cash at this time");
                sr.DetailedErrorMessages.CreateErrorMessage(ErrorMessage);
            }
            return sr;
        }

        public bool DoApplyCash()
        {
            bool cashApplied;
            if (Diamond.Login.IsLoggedIN() == false)
            {
                Diamond.Login.LoginNow(AppConfig.PrintUserName, AppConfig.PrintUserPassword);
                if(Diamond.Login.IsLoggedIN() == false)
                {
                    _errorMessage = "Unable to login to Diamond to ApplyCash.";
                }
            }

            //Also Sets checknum to "W/ APP" if CashSource is WebAgencyEftWithApp - don't believe App gets used anymore though but just in case
            SetCashSourceToAgencyEftWhenNeeded(); 

            GetBasicPolicyInfo();

            DCO.Billing.ApplyCash cash = new DCO.Billing.ApplyCash
            {
                AgencyId = this.AgencyId,
                PolicyId = this.PolicyId,
                PolicyImageNum = this.PolicyImageNum,
                PolicyNo = this.PolicyNumber,
                CashAmount = this.CashAmount,
                CashInSource = (int)this.CashInSource,
                CashType = this.CashType,
                ReasonId = this.ReasonId,
                UsersId = this.UsersId,
                EftAccountId = this.EftAccountId,
                CheckDate = new DCO.InsDateTime(DateTime.Now) // might want to make it the system date
            };

            if (this.checkNum.HasValue())
            {
                cash.CheckNum = this.checkNum;
            }

            if (AccountBillNumber.IsAccountBillNumber())
            {
                cash.AccountPayment = true;
                cash.BillingAccountId = AccountBillNumber.GetBillingAccountIdFromAccountNumber();
                cashApplied = CallApplyBillingAccountPaymentService(cash);
            }
            else
            {
                cashApplied = CallApplyCashService(cash);
            }
            return cashApplied;
        }

        private bool CallApplyCashService(DCO.Billing.ApplyCash cash)
        {
            using (var DS = Insuresoft.DiamondServices.BillingService.ApplyCash())
            {
                DS.RequestData.ApplyCash = cash;
                var exceptionCaught = false;
                var res = new Insuresoft.DiamondServices.IFMResponse<global::Diamond.Common.Services.Messages.BillingService.ApplyCash.Response>();
                try
                {
                    res = DS.Invoke();
                }
                catch(Exception ex)
                {
                    exceptionCaught = true;
                    _errorMessage = _errorMessage.AppendText(ex.Message, "; ");
                }

                bool responseIsNull = res == null ? true : false;
                Exception responseException = responseIsNull ? null : res.ex;
                DCO.DiamondValidation ResponseDV = responseIsNull ? null : res.dv;
                DCO.DiamondValidation DiamondResponseDV = responseIsNull ? null : res?.DiamondResponse?.DiamondValidation;

                if (exceptionCaught == false)
                {
                    if (res?.DiamondResponse?.ResponseData != null)
                    {
                        var cashApplied = res.DiamondResponse.ResponseData.Success;
                        if (cashApplied == false)
                        {
                            APICashNotAppliedErrorHandling(cash, responseException, ResponseDV, DiamondResponseDV);
                        }
                        return cashApplied;
                    }
                    else
                    {
                        string cashStr = Newtonsoft.Json.JsonConvert.SerializeObject(cash);
                        string loc = "IFMDataServicesCore.BusinessLogic.Payments.ApplyCash.CallApplyCashService;";
                        APIReturnedNullErrorHandling(loc, cash, responseIsNull, responseException, ResponseDV, DiamondResponseDV);
                    }
                }
            }
            return false;
        }

        private bool CallApplyBillingAccountPaymentService(DCO.Billing.ApplyCash cash)
        {
            using (var DS = Insuresoft.DiamondServices.BillingService.ApplyBillingAccountPayment())
            {
                DS.RequestData.ApplyCash = cash;
                var exceptionCaught = false;
                var res = new Insuresoft.DiamondServices.IFMResponse<global::Diamond.Common.Services.Messages.BillingService.ApplyBillingAccountPayment.Response>();
                try
                {
                    res = DS.Invoke();
                }
                catch (Exception ex)
                {
                    exceptionCaught = true;
                    _errorMessage = _errorMessage.AppendText(ex.Message, "; ");
                }

                bool responseIsNull = res == null ? true : false;
                Exception responseException = responseIsNull ? null : res.ex;
                DCO.DiamondValidation ResponseDV = responseIsNull ? null : res.dv;
                DCO.DiamondValidation DiamondResponseDV = responseIsNull ? null : res?.DiamondResponse?.DiamondValidation;

                if (exceptionCaught == false)
                {
                    if (res?.DiamondResponse?.ResponseData != null)
                    {
                        var cashApplied = res.DiamondResponse.ResponseData.Success;
                        if (cashApplied == false)
                        {
                            APICashNotAppliedErrorHandling(cash, responseException, ResponseDV, DiamondResponseDV);
                        }
                        return cashApplied;
                    }
                    else
                    {
                        string cashStr = Newtonsoft.Json.JsonConvert.SerializeObject(cash);
                        string loc = "IFMDataServicesCore.BusinessLogic.Payments.ApplyCash.CallApplyBillingAccountPaymentSrvice;";
                        
                        APIReturnedNullErrorHandling(loc, cash,responseIsNull, responseException, ResponseDV, DiamondResponseDV);
                    }
                }
            }
            return false;
        }

        private void APICashNotAppliedErrorHandling(DCO.Billing.ApplyCash cash, Exception responseException, DCO.DiamondValidation dv, DCO.DiamondValidation DiamondResponseValidations)
        {
            if (responseException != null)
            {
#if DEBUG
                Debugger.Break();
#endif
                _errorMessage = _errorMessage.AppendText(responseException.Message, ";");
                LogAgentPaymentError(PolicyNumber, responseException.Message, cash.BillingAccountId.GetAccountBillNumberFromBillingAccountId());
            }
            if (dv.ValidationItems.Count > 0)
            {
                foreach (var val in dv.ValidationItems)
                {
                    _errorMessage = _errorMessage.AppendText(val.Message, ";");
                }
            }
            if (DiamondResponseValidations.ValidationItems.Count > 0)
            {
                foreach (var val in DiamondResponseValidations.ValidationItems)
                {
                    _errorMessage = _errorMessage.AppendText(val.Message, ";");
                }
            }
        }

        private void APIReturnedNullErrorHandling(string loc, DCO.Billing.ApplyCash cash, bool responseIsNull, Exception responseException, DCO.DiamondValidation dv, DCO.DiamondValidation DiamondResponseValidations)
        {
            string cashStr = Newtonsoft.Json.JsonConvert.SerializeObject(cash);

            if (responseException != null)
            {
                if (cash != null)
                {
                    IFM.IFMErrorLogging.LogException(responseException, loc + " " + cashStr);
                }
                else
                {
                    IFM.IFMErrorLogging.LogException(responseException, loc + " cash obj is null.");
                }
            }
            else
            {
                if (responseIsNull)
                {
                    IFM.IFMErrorLogging.LogIssue("response is null", loc);
                }
                else
                {
                    if (dv?.ValidationItems?.Count > 0)
                    {
                        foreach (var val in dv.ValidationItems)
                        {
                            _errorMessage = _errorMessage.AppendText(val.Message, ";");
                        }
                    }
                    else if (DiamondResponseValidations?.ValidationItems?.Count > 0)
                    {
                        foreach (var val in DiamondResponseValidations?.ValidationItems)
                        {
                            _errorMessage = _errorMessage.AppendText(val.Message, ";");
                        }
                    }
                    else
                    {
                        IFM.IFMErrorLogging.LogIssue("Response data is null", loc);
                    }
                }
            }
        }

        private void LogAgentPaymentError(string PolicyNumber, string exceptionString)
        {
            LogAgentPaymentError(PolicyNumber, exceptionString, "");
        }

        private void LogAgentPaymentError(string PolicyNumber, string exceptionString, string AccountBillNumber)
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.Conn))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_LogAgentPaymentErrors", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@policyNumber", PolicyNumber);
                    cmd.Parameters.AddWithValue("@exceptionMessage", exceptionString);
                    if (AccountBillNumber.HasValue())
                    {
                        cmd.Parameters.AddWithValue("@AccountBillNumber", AccountBillNumber);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void GetBasicPolicyInfo()
        {
            bool requiresAgencyInfo = RequiresAgencyInfo();
            string procToUse = "";
            bool usePolicyNumber = false;
            bool doContinue = true;

            
            if (PolicyId > 0)
            {
                procToUse = "usp_GetAgencyEFTInfoByPolicyId";
            }
            else if (PolicyNumber.IsNotNullEmptyOrWhitespace())
            {
                usePolicyNumber = true;
                procToUse = "usp_GetAgencyEFTInfoByPolicyNumber";
            }
            else
            {
                doContinue = false;
            }

            if (doContinue && ((usePolicyNumber == false && PolicyNumber.IsNullEmptyOrWhitespace()) || (usePolicyNumber && PolicyId <= 0) || PolicyImageNum <= 0 || (requiresAgencyInfo && (AgencyId <= 0 || EftAccountId <= 0))))
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand(procToUse, conn) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        if (usePolicyNumber)
                        {
                            cmd.Parameters.AddWithValue("@PolicyNumber", PolicyNumber);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@PolicyId", PolicyId);
                        }
                        
                        if (PolicyImageNum > 0)
                        {
                            cmd.Parameters.AddWithValue("@PolicyImageNum", PolicyImageNum);
                        }
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    if (requiresAgencyInfo)
                                    {
                                        if (AgencyId <= 0)
                                            AgencyId = reader["agency_id"].TryToGetInt32();

                                        if (EftAccountId <= 0)
                                            EftAccountId = reader["eftaccount_id"].TryToGetInt32();
                                    }

                                    if (usePolicyNumber == false && PolicyNumber.IsNullEmptyOrWhitespace())
                                        PolicyNumber = reader["current_policy"].TryToGetString();

                                    if (usePolicyNumber && PolicyId <= 0)
                                        PolicyId = reader["policy_id"].TryToGetInt32();

                                    if (PolicyImageNum <= 0)
                                        PolicyImageNum = reader["policyimage_num"].TryToGetInt32();
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool RequiresAgencyInfo()
        {
            bool returnVar = false;
            switch (this.CashInSource)
            {
                case CashSource.AgencyEFT:
                case CashSource.WebAgencyEftWithApp:
                case CashSource.WebAgencyEFT:
                case CashSource.EFT:
                    returnVar = true;
                    break;
                default:
                    break;
            }
            return returnVar;
        }

        private void SetCashSourceToAgencyEftWhenNeeded()
        {
            //Seems to affect records getting into the bank file... better set it to AgencyEFT instead of WebAgencyEFT or WebAgencyEftWithApp
            switch (this.CashInSource)
            {
                case CashSource.WebAgencyEftWithApp:
                    this.checkNum = "W/ APP";
                    this.CashInSource = CashSource.AgencyEFT;
                    break;
                case CashSource.WebAgencyEFT:
                    this.CashInSource = CashSource.AgencyEFT;
                    break;
                default:
                    break;
            }
        }
    }
}
