using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;

namespace IFM.DataServices.Validation
{
    public class PayPlanDataValidator : AbstractValidator<IFM.DataServicesCore.CommonObjects.Payments.PayPlanData>
    {

        public PayPlanDataValidator()
        {
            RuleFor(item => item.UserAgreedWithTerms).NotEmpty().WithMessage("User didn't agree with terms of use");

            RuleFor(item => item.PolicyNumber).NotEmpty().MaximumLength(20).MinimumLength(7);
            RuleFor(item => item.PolicyId).NotEmpty().GreaterThan(0);
            RuleFor(item => item.ImageNumber).NotEmpty().GreaterThan(0);

            RuleFor(item => item.ClientId).NotEmpty().GreaterThan(0);

            RuleFor(item => item.PayPlanId).NotEmpty();
            RuleFor(item => item.PayPlanId).IsInEnum();

            When(item => item.RequiresRecurringData(), () =>
            {
                When(item => item.IsRccPlan(), () =>
                {
                    RuleFor(item => item.RecurringCreditCardInformation).NotEmpty();
                    When(item => item.RecurringCreditCardInformation != null, () =>
                    {
                        //RuleFor(item => item.RecurringCreditCardInformation.CardNumber).NotEmpty().CreditCard();
                        ////RuleFor(Function(item) item.NameOnCard).NotEmpty().MaximumLength(255)
                        //RuleFor(item => item.RecurringCreditCardInformation.CardExpireMonth).NotEmpty().GreaterThanOrEqualTo(1).LessThanOrEqualTo(12);
                        //RuleFor(item => item.RecurringCreditCardInformation.CardExpireYear).NotEmpty().GreaterThanOrEqualTo(DateTime.Now.Year);
                        //When(item => item.RecurringCreditCardInformation.CardExpireYear == DateTime.Now.Year, () => { RuleFor(item => item.RecurringCreditCardInformation.CardExpireMonth).NotEmpty().GreaterThanOrEqualTo(DateTime.Now.Month); });
                        //RuleFor(item => item.RecurringCreditCardInformation.DeductionDay).NotEmpty().InclusiveBetween(1, 31);
                        //RuleFor(item => item.RecurringCreditCardInformation.EmailAddress).NotEmpty().EmailAddress();
                        //updated 7/30/2020 for Fiserv
                        RuleFor(item => item.RecurringCreditCardInformation.DeductionDay).NotEmpty().InclusiveBetween(1, 31);
                        When(item => RccDataHasWalletItemId(item.RecurringCreditCardInformation) == false, () =>
                        {
                            RuleFor(item => item.RecurringCreditCardInformation.EmailAddress).NotEmpty().EmailAddress();
                            When(item => RccDataHasFundingAccountToken(item.RecurringCreditCardInformation) == false, () =>
                            {
                                //RuleFor(item => item.RecurringCreditCardInformation.CardNumber).NotEmpty().CreditCard();
                                //updated 9/8/2020
                                When(item => HasMaskedCardNumber(item.RecurringCreditCardInformation) == false, () =>
                                {
                                    RuleFor(item => item.RecurringCreditCardInformation.CardNumber).NotEmpty().CreditCard();
                                });
                                //RuleFor(Function(item) item.NameOnCard).NotEmpty().MaximumLength(255)
                                RuleFor(item => item.RecurringCreditCardInformation.CardExpireMonth).NotEmpty().GreaterThanOrEqualTo(1).LessThanOrEqualTo(12);
                                RuleFor(item => item.RecurringCreditCardInformation.CardExpireYear).NotEmpty().GreaterThanOrEqualTo(DateTime.Now.Year);
                                When(item => item.RecurringCreditCardInformation.CardExpireYear == DateTime.Now.Year, () => { RuleFor(item => item.RecurringCreditCardInformation.CardExpireMonth).NotEmpty().GreaterThanOrEqualTo(DateTime.Now.Month); });
                            });
                        });
                    });
                });

                When(item => item.IsEftPlan(), () =>
                {
                    RuleFor(item => item.RecurringEftInformation).NotEmpty();
                    When(item => item.RecurringEftInformation != null, () =>
                    {
                        //RuleFor(item => item.RecurringEftInformation.AccountNumber).NotEmpty().MinimumLength(5).MaximumLength(20);
                        //RuleFor(item => item.RecurringEftInformation.RoutingNumber).NotEmpty().MinimumLength(9).MaximumLength(9);
                        //RuleFor(item => item.RecurringEftInformation.AccountType).NotEmpty().InclusiveBetween(1, 2);
                        //updated 4/25/2023
                        When(item => EftDataHasWalletItemId(item.RecurringEftInformation) == false, () =>
                        {
                            RuleFor(item => item.RecurringEftInformation.AccountNumber).NotEmpty().MinimumLength(5).MaximumLength(20);
                            RuleFor(item => item.RecurringEftInformation.RoutingNumber).NotEmpty().MinimumLength(9).MaximumLength(9);
                            RuleFor(item => item.RecurringEftInformation.AccountType).NotEmpty().InclusiveBetween(1, 2);
                        });
                        RuleFor(item => item.RecurringEftInformation.DeductionDay).NotEmpty().InclusiveBetween(1, 31);
                        RuleFor(item => item.RecurringEftInformation.EmailAddress).NotEmpty().EmailAddress();
                    });
                });

            });

            When(item => item.RequiresRecurringData() == false, () =>
            {
                RuleFor(item => item.RecurringCreditCardInformation).Null().WithMessage("No credit card information needed for selected payplan.");
                RuleFor(item => item.RecurringEftInformation).Null().WithMessage("No EFT information needed for selected payplan.");
            });




        }


        //added 7/30/2020
        private bool RccDataHasWalletItemId(IFM.DataServicesCore.CommonObjects.Payments.RecurringCreditCardInformation rccData)
        {
            if (rccData != null)
            {
                if (rccData.Fiserv_WalletItemId > 0)
                {
                    return true;
                }
            }
            return false;
        }
        private bool RccDataHasFundingAccountToken(IFM.DataServicesCore.CommonObjects.Payments.RecurringCreditCardInformation rccData)
        {
            if (rccData != null)
            {
                if (string.IsNullOrWhiteSpace(rccData.Fiserv_FundingAccountToken) == false)
                {
                    return true;
                }
            }
            return false;
        }

        //added 9/8/2020
        private bool HasMaskedCardNumber(DataServicesCore.CommonObjects.Payments.RecurringCreditCardInformation rccData)
        {
            bool hasIt = false;

            if (rccData != null)
            {
                if (string.IsNullOrWhiteSpace(rccData.CardNumber) == false)
                {
                    if (rccData.CardNumber.Length >= 15)
                    {
                        if (rccData.CardNumber.Contains("*") == true || rccData.CardNumber.ToUpper().Contains("X") == true)
                        {
                            hasIt = true;
                        }
                    }
                }
            }

            return hasIt;
        }

        //added 4/25/2023
        private bool EftDataHasWalletItemId(IFM.DataServicesCore.CommonObjects.Payments.RecurringEftInformation eftData)
        {
            if (eftData != null)
            {
                if (eftData.Fiserv_WalletItemId > 0)
                {
                    return true;
                }
            }
            return false;
        }

    }
}