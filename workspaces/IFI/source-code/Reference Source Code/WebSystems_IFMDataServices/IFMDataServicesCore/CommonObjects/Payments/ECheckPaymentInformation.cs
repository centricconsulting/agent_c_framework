using IFM.PrimitiveExtensions;
using System;

namespace IFM.DataServicesCore.CommonObjects.Payments
{
    [System.Serializable]
    public class ECheckPaymentInformation
    {
        public enum BankFileExclusion
        {
            Default,
            True,
            False
        }

        public Int32 AccountType { get; set; }//BankAccount Type 1 = Checking, 2 = Savings ' Me.Quote.EFT_BankAccountTypeId
        public string AccountNumber { get; set; }//Me.Quote.EFT_BankAccountNumber
        public string RoutingNumber { get; set; }//Me.Quote.EFT_BankRoutingNumber
        public BankFileExclusion ExcludeFromBankFile { get; set; } = BankFileExclusion.Default;
        public int AccountID { get; set; }

        public bool PassesBasicValidation()
        {
            return AccountType.EqualsAny(1, 2) && AccountNumber.IsNullEmptyOrWhitespace() == false && RoutingNumber.IsNullEmptyOrWhitespace() == false && RoutingNumber.Length == 9 && RoutingNumber.IsNumeric();
        }
    }
}