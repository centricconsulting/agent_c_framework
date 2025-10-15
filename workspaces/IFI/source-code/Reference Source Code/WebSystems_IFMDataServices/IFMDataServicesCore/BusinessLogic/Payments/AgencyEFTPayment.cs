using Diamond.Business.ThirdParty.SEP;
using Diamond.Common.Objects.Billing;
using IFM.DataServicesCore.BusinessLogic.Diamond;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFM.DataServicesCore.BusinessLogic.Payments
{
    public class AgencyEFTPayment
    {
        private string location = "IFM.DataServicesCore.BusinessLogic.Payments.AgencyEFTPayment";
        public int AgencyId { get; set; }
        public string PolicyNumber { get; set; }
        public int PolicyId { get; set; }
        public int PolicyImageNumber { get; set; }
        public int AgencyEFTAccountId { get; set; }
        public int LegacyAgencyId { get; set; }
        public int LegacyUserId { get; set; }
        public int DiamondUserId { get; set; }
        public string DiamondUserName { get; set; }
        public string DiamondLoginDomain { get; set; }
        public decimal Amount { get; set; }
        public string AccountBillNumber { get; set; }
        public DataServices.API.Enums.CashSource CashInSource { get; set; }
        public DataServices.API.Enums.PaymentInterface PaymentInterface { get; set; }
        private bool swept = false;

        public AgencyEFTPayment(IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentData)
        {
            if (paymentData != null)
            {
                Diamond.UserInfo user = new Diamond.UserInfo();
                CommonObjects.Diamond.AgencyEFTInfo agencyInfo = new CommonObjects.Diamond.AgencyEFTInfo();

                var TasksToRun = new List<Task>();
                if (paymentData.EFTInformation.LegacyUserId <= 0 && paymentData.UserId > 0)
                {
                    TasksToRun.Add(Task.Run(() =>
                    {
                        user.GetLegacyUserInfo(paymentData.UserId);
                    }));
                }
                else
                {
                    LegacyUserId = paymentData.EFTInformation.LegacyUserId;
                }

                if (paymentData.EFTInformation.LegacyAgencyId <= 0 || paymentData.EFTInformation.AgencyEFTAccountId <= 0)
                {
                    TasksToRun.Add(Task.Run(() =>
                    {
                        agencyInfo = AgencyInformation.GetAgencyEFTInformation(paymentData.PolicyId, paymentData.PolicyImageNumber);
                    }));
                }
                else
                {
                    LegacyAgencyId = paymentData.EFTInformation.LegacyAgencyId;
                }

                AgencyId = paymentData.AgencyId;
                AccountBillNumber = paymentData.AccountBillNumber;
                PolicyNumber = paymentData.PolicyNumber;
                PolicyId = paymentData.PolicyId;
                PolicyImageNumber = paymentData.PolicyImageNumber;
                DiamondUserId = paymentData.UserId;
                DiamondUserName = paymentData.Username;
                DiamondLoginDomain = paymentData.UserLoginDomain;
                PaymentInterface = paymentData.PaymentInterface;
                Amount = paymentData.PaymentAmount.TryToGetDecimal();
                CashInSource = paymentData.CashInSource;
                AgencyEFTAccountId = paymentData.EFTInformation.AgencyEFTAccountId;
                
                if (TasksToRun.Count > 0)
                {
                    Task.WaitAll(TasksToRun.ToArray());

                    if (LegacyUserId <= 0)
                    {
                        LegacyUserId = user.LegacyUserId;
                    }
                    if (LegacyAgencyId <= 0)
                    {
                        LegacyAgencyId = agencyInfo.LegacyAgencyId;
                    }
                    if (AgencyEFTAccountId <= 0)
                    {
                        AgencyEFTAccountId = agencyInfo.EFTAccountId; //Pretty sure validation is in place to stop payments getting this far withouth AgencyEFT if it is needed...buuuut... just in case.
                    }
                }
            }
        }

        public bool MakeAgencyEFTPayment()
        {
            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.Conn))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.sp_Insert_EFT", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        //cmd.Parameters.AddWithValue("@agid", this.AgencyId);
                        cmd.Parameters.AddWithValue("@agid", this.LegacyAgencyId);
                        cmd.Parameters.AddWithValue("@polnum", this.PolicyNumber);
                        if (this.AccountBillNumber.HasValue())
                        {
                            cmd.Parameters.AddWithValue("@account_number", this.AccountBillNumber);
                        }
                        cmd.Parameters.AddWithValue("@user", this.LegacyUserId);
                        cmd.Parameters.AddWithValue("@diaUser", this.DiamondUserId);
                        cmd.Parameters.AddWithValue("@amount", this.Amount);

                        ApplyCash applyCash = new ApplyCash(this);
                        if (applyCash.DoApplyCash())
                            swept = true;

                        cmd.Parameters.AddWithValue("@swept", swept == true ? "Y": "N");

                        cmd.ExecuteNonQuery();

                        InsertAgentPaymentRecord();

                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                IFM.IFMErrorLogging.LogException(ex, $"{location}.MakeAgencyEFTPayment;");
                return false;
            }
        }

        private bool InsertAgentPaymentRecord()
        {
            IFM_CreditCardProcessing.DiamondAgentPaymentDBRecord AgentPaymentDBRecord = new IFM_CreditCardProcessing.DiamondAgentPaymentDBRecord
            {
                Amount = this.Amount,
                CashSource = (int)IFM.DataServices.API.Enums.CashSource.WebAgencyEFT,
                CashReason = 0, //None
                CashType = 1, //Payment
                DiamondDomain = this.DiamondLoginDomain,
                DiamondUserId = this.DiamondUserId,
                DiamondUsername = this.DiamondUserName,
                PaymentInterface = (IFM_CreditCardProcessing.Enums.PaymentInterface)this.PaymentInterface,
                PolicyId = this.PolicyId,
                PolicyNumber = this.PolicyNumber,
                PolicyImageNum = this.PolicyImageNumber,
                AgencyId = this.AgencyId,
                AgencyEFTAccountId = this.AgencyEFTAccountId,
                Swept = swept == true ? "Y" : "N",
                StatusUpdateUserForSweptY = "AgentPayment Page"
            };
            AgentPaymentDBRecord.AccountBillNumber = AgentPaymentDBRecord.AccountBillNumber.SetIfVariableHasValue(this.AccountBillNumber);
            return AgentPaymentDBRecord.InsertRecord();
        }
    }
}
