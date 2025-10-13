using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;

namespace IFM.DataServices.Validation
{
    public class MemberAccountPolicyValidator : AbstractValidator<IFM.DataServicesCore.CommonObjects.OMP.MemberAccountPolicy>
    {
        public MemberAccountPolicyValidator()
        {
            RuleFor(item => item.NickName).NotNull();
            RuleFor(item => item.PolicyNumber).NotEmpty().MinimumLength(7);            
        }

    }
}