using IFM.DataServicesCore.BusinessLogic.Payments.eCheck;
using System;

namespace IFM.DataServicesCore.CommonObjects.Payments
{
    [System.Serializable]
    public class RecurringEftInformation : ECheckPaymentInformation
    {
        public Int32 DeductionDay { get; set; }

        public string EmailAddress { get; set; }
        public int Fiserv_WalletItemId { get; set; } //added 4/24/2023

        public RecurringEftInformation()
        {
        }

        public RecurringEftInformation(ECheckPaymentInformation eCheckPaymentInfo)
        {
            this.AccountNumber = eCheckPaymentInfo.AccountNumber;
            this.AccountType = eCheckPaymentInfo.AccountType;
            this.RoutingNumber = eCheckPaymentInfo.RoutingNumber;
            this.AccountID = eCheckPaymentInfo.AccountID;
        }

        new public bool PassesBasicValidation()
        {
            LoadFiservWalletItemInfoIfNeeded(); //added 4/25/2023
            return base.PassesBasicValidation() && DeductionDay > 0 && DeductionDay < 32;
        }

        protected internal void LoadFiservWalletItemInfoIfNeeded() //added 4/25/2023
        {
            if (this.Fiserv_WalletItemId > 0 && base.PassesBasicValidation() == false)
            {
                IFM_FiservDatabaseObjects.FiservWalletItem wi = null;
                string routingNum = "";
                string accountNum = "";
                Int32 accountType = 0;
                BusinessLogic.Payments.PayPlanHelper.SetEftInfoForWalletItemId(this.Fiserv_WalletItemId, ref wi, ref routingNum, ref accountNum, ref accountType);

                if (string.IsNullOrWhiteSpace(routingNum) == false && string.IsNullOrWhiteSpace(accountNum) == false && accountType > 0)
                {
                    this.RoutingNumber = routingNum;
                    this.AccountNumber = accountNum;
                    this.AccountType = accountType;
                }
            }
        }
    }
}