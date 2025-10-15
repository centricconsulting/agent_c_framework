using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;

namespace IFM.DataServices.Validation
{
    public class ClaimReportValidator : AbstractValidator<IFM.DataServicesCore.CommonObjects.OMP.ClaimReport>
    {


        public ClaimReportValidator()
        {
            RuleFor(item => item.IpAddress).NotEmpty();
            RuleFor(item => item.LossDescription).NotEmpty();
            //might want a min length
            RuleFor(item => item.Name).NotEmpty().MaximumLength(255);
            RuleFor(item => item.PolicyNumber).NotEmpty().MaximumLength(20).MinimumLength(7);
            RuleFor(item => item.LossType).NotEmpty();
            RuleFor(item => item.EmailAddress).EmailAddress().MaximumLength(255);
            RuleFor(item => item.PhoneNumber).NotEmpty().MaximumLength(20).MinimumLength(10);
            RuleFor(item => item.InjuriesExist).NotNull();
            RuleFor(item => item.LossDateTime).NotEmpty();
            //RuleFor(item => item.LossType).IsInEnum();
            //RuleFor(Function(item) item.ToAddress).Must(Function(item) CommonValidations.IsValidEmail(item)).WithMessage("Invalid email address")


        }

    }
}