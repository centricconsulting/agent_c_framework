using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFM.DataServicesCore.CommonObjects.Fiserv
{
    [System.Serializable]
    public class CalculateFeeBody //added 9/26/2024
    {
        public string PolicyNumber { get; set; }
        public string AccountBillNumber { get; set; }
        public string MemberIdentifier { get; set; }
        public decimal PaymentAmount { get; set; }
        public IFM_CreditCardProcessing.Enums.UserType UserType { get; set; }
        public string UserName { get; set; }
        public string Fiserv_AuthToken { get; set; } //obtained for use w/ iframe; not required if not using iframe; would only be used for Card payments (not Bank)
        public string Fiserv_SessionToken { get; set; } //obtained for use w/ iframe; not required if not using iframe; would only be used for Card payments (not Bank)
        public int Fiserv_SessionId { get; set; } //obtained while getting tokens used w/ iframe; not required if not using iframe; would only be used for Card payments (not Bank)
        public string Fiserv_FundingAccountToken { get; set; } //obtained from iframe; not required if not using iframe; would only be used for Card payments (not Bank)
        public bool IsFromOneView { get; set; } //should be set to True for service calls coming from OneView (likely won't update this info from there)
        public DataServices.API.Enums.PaymentInterface PaymentInterface { get; set; }
        public string FundingAccountType { get; set; } //Personal, Business; for Bank info; Create and Update
        public string BankAccountType { get; set; } //Checking, Saving; for Bank info; Create and Update
        public string ExpirationMonth { get; set; } //won't ever come back but used for Update; for Card info; would also be used for Create if we're supporting wallet functionality but not using Fiserv's iframe
        public string ExpirationYear { get; set; } //won't ever come back but used for Update; for Card info; would also be used for Create if we're supporting wallet functionality but not using Fiserv's iframe
        public string ZipCode { get; set; } //won't ever come back but used for Update; for Card info
        public string CreditCardNumber { get; set; } //won't ever come back; would only be used if we're supporting wallet functionality but not using Fiserv's iframe; for Create only; cannot be updated
        public string SecurityCode { get; set; } //won't ever come back; would only be used if we're supporting wallet functionality but not using Fiserv's iframe; for Create only; cannot be updated
        public string FirstName { get; set; } //for Bank info; Create and Update
        public string LastName { get; set; } //for Bank info; Create and Update
        public string RoutingNumber { get; set; } //won't ever come back; for Bank info; for Create only; cannot be updated
        public string CheckAccountNumber { get; set; } //won't ever come back; for Bank info; for Create only; cannot be updated
    }
}
