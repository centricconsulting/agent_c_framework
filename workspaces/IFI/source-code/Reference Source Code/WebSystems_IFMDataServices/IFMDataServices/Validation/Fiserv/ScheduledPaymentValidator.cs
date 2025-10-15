using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;

namespace IFM.DataServices.Validation.Fiserv
{
    public class ScheduledPaymentAddValidator : AbstractValidator<IFM.DataServicesCore.CommonObjects.Fiserv.ScheduledPayment>
    {
        public ScheduledPaymentAddValidator()
        {
            RuleFor(item => item.KeyIdentifier).NotEmpty();
            RuleFor(item => item.FundingAccountToken).NotEmpty();
            RuleFor(item => item.PolicyNumber).NotEmpty().Unless(item => item.AccountBillNumber.HasValue()).WithMessage("Must provide Policy Number or Account Bill Number.");
            RuleFor(item => item.PaymentScheduleDate).NotEmpty();
            RuleFor(item => item.PaymentAmount).NotEmpty().GreaterThan(0);
        }
    }
    public class AddScheduledPaymentBodyValidator : AbstractValidator<IFM.DataServicesCore.CommonObjects.Fiserv.AddScheduledPaymentBody>
    {
        public AddScheduledPaymentBodyValidator()
        {
            When(item => item.Payment == null, () => {
                RuleFor(item => item.Payment).NotNull().WithMessage("No scheduled payment data provided.");
            }).Otherwise(() => {
                RuleFor(item => item.Payment).SetValidator(new ScheduledPaymentAddValidator());
            });
        }
    }
    public class ScheduledPaymentUpdateValidator : AbstractValidator<IFM.DataServicesCore.CommonObjects.Fiserv.ScheduledPayment>
    {
        public ScheduledPaymentUpdateValidator()
        {
            RuleFor(item => item.ReferenceId).NotEmpty();
        }
    }
    public class UpdateScheduledPaymentBodyValidator : AbstractValidator<IFM.DataServicesCore.CommonObjects.Fiserv.UpdateScheduledPaymentBody>
    {
        public UpdateScheduledPaymentBodyValidator()
        {
            When(item => item.Payment == null, () => {
                RuleFor(item => item.Payment).NotNull().WithMessage("No scheduled payment data provided.");
            }).Otherwise(() => {
                RuleFor(item => item.Payment).SetValidator(new ScheduledPaymentUpdateValidator());
            });
        }
    }
    public class ScheduledPaymentDeleteValidator : AbstractValidator<IFM.DataServicesCore.CommonObjects.Fiserv.ScheduledPayment>
    {
        public ScheduledPaymentDeleteValidator()
        {
            RuleFor(item => item.ReferenceId).NotEmpty();
        }
    }
    public class DeleteScheduledPaymentBodyValidator : AbstractValidator<IFM.DataServicesCore.CommonObjects.Fiserv.DeleteScheduledPaymentBody>
    {
        public DeleteScheduledPaymentBodyValidator()
        {
            When(item => item.Payment == null, () => {
                RuleFor(item => item.Payment).NotNull().WithMessage("No scheduled payment data provided.");
            }).Otherwise(() => {
                RuleFor(item => item.Payment).SetValidator(new ScheduledPaymentDeleteValidator());
            });
        }
    }
}