using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFM.DataServicesCore.BusinessLogic.Payments.eCheck
{
    public class eCheckDuplicateAndNSFChecking
    {
        public string Input_PolicyNumber { get; set; }
        public int Input_PolicyId { get; set; }
        public string Input_AccountNumber { get; set; } = "";
        public string Input_RoutingNumber { get; set; } = "";
        public int Settings_DuplicatePaymentPeriodInHours { get; set; }
        public bool Settings_CheckNSF { get; set; }
        public int Settings_NSFPeriodInDays { get; set; }
        public int Settings_NSFErrorQuantity { get; set; }
        public int Settings_StartingErrorQuantityForPaymentsOnPolicyNumber { get; set; }
        public int Settings_StartingErrorQuantityForPaymentsOnPolicyId { get; set; }
        public int Settings_StartingErrorQuantityForPaymentsOnPolicyIdWithSameBankInfo { get; set; }
        private int _nsfCount;
        public bool Output_HasNSFPaymentsError { 
            get 
            {
                return _nsfCount > Settings_NSFErrorQuantity ? true : false;
            }
        }
        private bool _hasDuplicatePaymentError = false;
        public bool Output_HasDuplicatePaymentError
        {
            get
            {
                return _hasDuplicatePaymentError;
            }
        }
        private string _duplicatePaymentsErrorMessage = "";
        public string Output_DuplicatePaymentsErrorMessage {
            get
            {
                return _duplicatePaymentsErrorMessage;
            }
        }
        private string _nsfPaymentsErrorMessage = "";
        public string Output_NSFPaymentsErrorMessage
        {
            get
            {
                return _nsfPaymentsErrorMessage;
            }
        }
        private List<EcheckPaymentInfo> _echeckPayments = new List<EcheckPaymentInfo>();
        public void GetDuplicateAndNSFInfo()
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_ECheck_GetAllPayments", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@polNum", Input_PolicyNumber);
                    if (Settings_DuplicatePaymentPeriodInHours > 0)
                    {
                        cmd.Parameters.AddWithValue("@compareDt", DateTime.Now.AddHours(Settings_DuplicatePaymentPeriodInHours * -1));
                    }

                    if (Settings_CheckNSF)
                    {
                        cmd.Parameters.AddWithValue("@checkNSF", 1);
                        cmd.Parameters.AddWithValue("@compareDtNSF", DateTime.Now.AddDays(Settings_NSFPeriodInDays * -1));
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if (reader["queryName"].TryToGetString() == "PaymentInfo")
                                {
                                    EcheckPaymentInfo echeckPayment = new EcheckPaymentInfo
                                    {
                                        AccountNumber = reader["AcctNum"].TryToGetString(),
                                        RoutingNumber = reader["RoutingNum"].TryToGetString(),
                                        BillingCashInSourceDesc = reader["billingcashinsource_dscr"].TryToGetString(),
                                        BillingCashInSourceId = reader["billingcashinsource_id"].TryToGetInt32(),
                                        Amount = reader["Amount"].TryToGetDecimal(),
                                        InsertedDate = reader["InsertedDate"].TryToGetDateTime(),
                                        PaymentStatus = reader["PaymentStatus"].TryToGetInt32(),
                                        PolicyNumber = reader["PolicyNumber"]?.ToString(),
                                        PolicyId = reader["PolicyID"].TryToGetInt32()
                                    };
                                    echeckPayment.DecryptInfo();
                                    _echeckPayments.Add(echeckPayment);
                                }
                                else if(reader["queryName"].TryToGetString() == "NSFLookup")
                                {
                                    _nsfCount = reader["NSFCount"].TryToGetInt32();
                                    if (_nsfCount > Settings_NSFErrorQuantity)
                                        _nsfPaymentsErrorMessage = $"This policy recently had {Settings_NSFErrorQuantity} or more eCheck payments result in insufficient funds.";
                                }
                            }
                        }
                    }
                }
            }
            GetDuplicatePaymentMessage();
        }

        private void GetDuplicatePaymentMessage()
        {
            int duplicatePaymentCountByPolicyNumber = 0;
            int duplicatePaymentCountByPolicyId = 0;
            int duplicatePaymentCountByPolicyIdAndAccountInfo = 0;

            if (_echeckPayments.Count > 0)
            {
                duplicatePaymentCountByPolicyNumber = _echeckPayments.FindAll(x => x.PolicyNumber == Input_PolicyNumber).Count;
                if (Input_PolicyId > 0)
                {
                    duplicatePaymentCountByPolicyId = _echeckPayments.FindAll(x => x.PolicyNumber == Input_PolicyNumber && x.PolicyId == Input_PolicyId).Count;
                    if (Input_RoutingNumber.HasValue() && Input_AccountNumber.HasValue())
                    {
                        duplicatePaymentCountByPolicyIdAndAccountInfo = _echeckPayments.FindAll(x => x.PolicyId == Input_PolicyId && x.AccountNumber == Input_AccountNumber && x.RoutingNumber == Input_RoutingNumber).Count;
                    }
                }
            }

            if (duplicatePaymentCountByPolicyId >= Settings_StartingErrorQuantityForPaymentsOnPolicyId)
            {
                _hasDuplicatePaymentError = true;
                _duplicatePaymentsErrorMessage = "You have reached the maximum amount of e-check payments in a 24 hour period for this policy term.";
            }
            else if (duplicatePaymentCountByPolicyIdAndAccountInfo >= Settings_StartingErrorQuantityForPaymentsOnPolicyIdWithSameBankInfo)
            {
                _hasDuplicatePaymentError = true;
                _duplicatePaymentsErrorMessage = "You have reached the maximum amount of e-check payments in a 24 hour period for this policy term with this bank account information. If you would like to make another payment within this 24 hour period, please use different banking information.";
            }
            else if (duplicatePaymentCountByPolicyNumber >= Settings_StartingErrorQuantityForPaymentsOnPolicyNumber)
            {
                _hasDuplicatePaymentError = true;
                _duplicatePaymentsErrorMessage = "You have reached the maximum amount of e-check payments in a 24 hour period for this policy.";
            }
        }
    }

    public class EcheckPaymentInfo
    {
        public string PolicyNumber { get; set; }
        public int PolicyId { get; set; }
        public decimal Amount { get; set; }
        public DateTime InsertedDate { get; set; }
        public int PaymentStatus { get; set; }
        public string BillingCashInSourceDesc { get; set; }
        public int BillingCashInSourceId { get; set; }
        public string AccountNumber { get; set; }
        public string RoutingNumber { get; set; }
        public void DecryptInfo()
        {
            if (RoutingNumber.IsNumeric() == false)
            {
                RoutingNumber.DoubleDecrypt();
            }
            if (AccountNumber.IsNumeric() == false)
            {
                AccountNumber.DoubleDecrypt();
            }
        }
    }
}
