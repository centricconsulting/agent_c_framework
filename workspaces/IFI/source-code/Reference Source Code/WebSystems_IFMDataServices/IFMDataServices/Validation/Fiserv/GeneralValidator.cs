using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;

namespace IFM.DataServices.Validation.Fiserv
{
    public class CalculateFeeValidator : AbstractValidator<IFM.DataServicesCore.CommonObjects.Fiserv.CalculateFeeBody>
    {
        public CalculateFeeValidator()
        {
            RuleFor(item => item.PaymentAmount).NotEmpty().GreaterThan(0);
            When(item => string.IsNullOrWhiteSpace(item.Fiserv_SessionToken) && item.Fiserv_SessionId <= 0 && string.IsNullOrWhiteSpace(item.PolicyNumber) && string.IsNullOrWhiteSpace(item.AccountBillNumber) && string.IsNullOrWhiteSpace(item.MemberIdentifier), () => { RuleFor(item => item.Fiserv_SessionToken).NotEmpty().WithMessage("No session information provided; policyNumber, accountBillNumber, or memberIdentifier needed."); });
            //note: should also add validation to check for fundingAcctToken, CardInfo, or BankInfo
            When(item => string.IsNullOrWhiteSpace(item.Fiserv_FundingAccountToken) && string.IsNullOrWhiteSpace(item.CreditCardNumber) && string.IsNullOrWhiteSpace(item.CheckAccountNumber), () => { RuleFor(item => item.Fiserv_FundingAccountToken).NotEmpty().WithMessage("No Funding Account Token, Card Information, or Bank Information provided."); });
            //RuleFor(item => item.EmailAddress).NotEmpty();
        }
    }
}