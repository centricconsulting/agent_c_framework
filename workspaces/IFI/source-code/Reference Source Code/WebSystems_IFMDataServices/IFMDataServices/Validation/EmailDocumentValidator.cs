using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;

namespace IFM.DataServices.Validation
{
    public class EmailDocumentValidator : AbstractValidator<global::IFM.DataServicesCore.CommonObjects.EmailDocument>
    {


        public EmailDocumentValidator()
        {
            RuleFor(item => item.ToAddress).NotEmpty();
            //RuleFor(item => item.ToAddress).EmailAddress();
            //RuleFor(Function(item) item.ToAddress).Must(Function(item) CommonValidations.IsValidEmail(item)).WithMessage("Invalid email address")
            RuleFor(item => item.Subject).NotEmpty();
            RuleFor(item => item.Body).NotEmpty();

        }



    }

}