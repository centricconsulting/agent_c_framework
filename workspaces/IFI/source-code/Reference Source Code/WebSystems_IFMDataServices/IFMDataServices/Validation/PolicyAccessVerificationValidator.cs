using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;

namespace IFM.DataServices.Validation
{
    public class PolicyAccessVerificationValidator : AbstractValidator<IFM.DataServicesCore.CommonObjects.OMP.PolicyAccessVerification>
    {


        public PolicyAccessVerificationValidator()
        {
            //type must be valid
            RuleFor(item => item.VerificationTypeId).IsInEnum();

            //when OneTimePaymentByName verification
            When(item => item.VerificationTypeId == IFM.DataServicesCore.CommonObjects.Enums.Enums.PolicyInformationVerificationLevel.OneTimePaymentByName, () =>
            {
                //if policynum,name,zip
                RuleFor(item => item.PolicyNumber).NotEmpty();
                RuleFor(item => item.Name).NotEmpty().WithName("Policyholder Name");
                RuleFor(item => item.Zip).NotEmpty().WithName("ZIP Code").MinimumLength(5);
            });

            //when OneTimePaymentByAccountNumber verification
            When(item => item.VerificationTypeId == IFM.DataServicesCore.CommonObjects.Enums.Enums.PolicyInformationVerificationLevel.OneTimePaymentByAccountNumber, () =>
            {
                //if accountnumber,name,zip
                RuleFor(item => item.AccountNumber).NotEmpty();
                RuleFor(item => item.Name).NotEmpty().WithName("Policyholder Name");
                RuleFor(item => item.Zip).NotEmpty().WithName("ZIP Code").MinimumLength(5);
            });

            //when OneTimePaymentByOnlinePaymentNumber verification
            When(item => item.VerificationTypeId == IFM.DataServicesCore.CommonObjects.Enums.Enums.PolicyInformationVerificationLevel.OneTimePaymentByOnlinePaymentNumber, () =>
            {
                //if online payment number
                RuleFor(item => item.PolicyNumber).NotEmpty();
                RuleFor(item => item.OnlinePaymentNumber).NotEmpty();
            });

            //policy and name verification
            When(item => item.VerificationTypeId == IFM.DataServicesCore.CommonObjects.Enums.Enums.PolicyInformationVerificationLevel.PolicyAndName, () =>
            {
                RuleFor(item => item.PolicyNumber).NotEmpty();
                RuleFor(item => item.Name).NotEmpty().WithName("Policyholder Name");
            });

            //full verification
            When(item => item.VerificationTypeId == IFM.DataServicesCore.CommonObjects.Enums.Enums.PolicyInformationVerificationLevel.PolicyFull, () =>
            {
                RuleFor(item => item.PolicyNumber).NotEmpty();
                RuleFor(item => item.Name).NotEmpty().WithName("Policyholder Name");
                //if ppa
                When(item => item.PolicyNumber.ToUpper().StartsWith("PPA"), () =>
                {
                    RuleFor(item => item.DLN).NotEmpty().WithName("License Number");
                    //WS-1857 - removing DL# Validation if DL does not match vehicle garaging state

                    //RuleFor(item => item.DLN).Length(10, 10).WithName("License Number"); //This assumed we were always dealing with Indiana memeber. Now that we have expanded states, we need to check different state DLN patterns
                    //RuleFor(item => item.Zip).NotEmpty().WithName("ZIP Code").MinimumLength(5).DependentRules(() =>
                    //{
                    //    //Dependant rules will only be attempted if the first rule passes. Now we know we have a somewhat valid zipcode for our zip lookup.
                    //    RuleFor(item => item).Custom((item, context) =>
                    //    {
                    //        var lookupResults = global::IFM.VR.Common.Helpers.GetCityCountyFromZipCode.GetCityCountyFromZipCode(item.Zip);
                    //        if(lookupResults.Any())
                    //        {
                    //            string state = lookupResults[0].StateAbbrev;
                    //            string errMsgPre = "'Drivers License Number' for '" + state + "' ";
                    //            string errMsg = "";
                    //            if (String.IsNullOrEmpty(state) == false)
                    //            {
                    //                if(item.DLN.IsValidDriversLicenseNumber(state, ref errMsg) == false)
                    //                {
                    //                    context.AddFailure("DLN", errMsgPre + errMsg.ToLower());
                    //                }
                    //            }
                    //            else
                    //            {
                    //                context.AddFailure("DLN", errMsgPre + errMsg.ToLower());
                    //            }
                    //        }
                    //        else
                    //        {
                    //            context.AddFailure("DLN", "Unable to verify 'Drivers License Number' based on zip code.");
                    //        }
                    //    });
                    //});
                    RuleFor(item => item.Zip).NotEmpty().WithName("ZIP Code").MinimumLength(5);
                });

                //if hom/pup
                When(item => item.PolicyNumber.ToUpper().StartsWith("HOM") || item.PolicyNumber.ToUpper().StartsWith("PUP"), () =>
                {
                    RuleFor(item => item.DOB).NotEmpty().WithName("Date of Birth");
                    RuleFor(item => item.Zip).NotEmpty().WithName("ZIP Code").MinimumLength(5);
                });

                //if not above LOBs
                Unless(item => item.PolicyNumber.ToUpper().StartsWith("HOM") || item.PolicyNumber.ToUpper().StartsWith("PUP") || item.PolicyNumber.ToUpper().StartsWith("PPA"), () =>
                {
                   // RuleFor(item => item.FEIN).NotEmpty().WithName("FEIN").Length(4, 4);
                    RuleFor(item => item.Zip).NotEmpty().WithName("ZIP Code").MinimumLength(5);
                });
            });
            //Staff verification
            //RuleFor(Function(item) item).Must(Function(item) item.VerificationTypeId <> IFM.DataServicesCore.CommonObjects.Enums.IFMEnums.PolicyInformationVerificationLevel.StaffVerification).WithMessage("Invalid verification type.")

            //RuleFor(Function(item) item.ToAddress).Must(Function(item) CommonValidations.IsValidEmail(item)).WithMessage("Invalid email address")
        }
    }
}