using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Diamond.Common.Objects.Claims.NotifyUnderwriting;
using FluentValidation;

namespace IFM.DataServices.Validation
{
    public class PaymentDataValidator : AbstractValidator<global::IFM.DataServices.API.RequestObjects.Payments.PaymentData>
    {
        IFM.DataServicesCore.CommonObjects.OMP.BasicPolicyInformation info = null;
        IFM.DataServicesCore.CommonObjects.OMP.BasicPolicyInformation futureImage = null;
        IFM.DataServicesCore.BusinessLogic.Payments.eCheck.eCheckDuplicateAndNSFChecking _eCheckDuplicateAndNSF = null;

        public enum ValidationInterface
        {
            MemberPortalOrOneView,
            eCheckAfterHours,
            ApplyCash,
            Other
        }

        public PaymentDataValidator()
        {
            RequirePaymentInterface();
        }

        public PaymentDataValidator(ValidationInterface Interface)
        {
            switch (Interface)
            {
                case ValidationInterface.MemberPortalOrOneView:
                    MemberPortalValidation();
                    break;
                case ValidationInterface.Other:
                    RequirePaymentInterface();
                    break;
                case ValidationInterface.eCheckAfterHours:
                    eCheckAfterHoursValidation();
                    break;
                case ValidationInterface.ApplyCash:
                    ApplyCashValidation();
                    break;
            }
        }

        private void MemberPortalValidation()
        {
            RuleFor(item => item.UserAgreedWithTerms).NotEmpty().WithMessage("User didn't agree with terms of use.");
            CorePaymentValidation();
            RuleFor(item => item).Must(item => HasBalance(item)).WithMessage("Policy has no balance.");
        }

        private void CorePaymentValidation()
        {
            When(item => item.UserType != global::IFM.DataServices.API.Enums.UserType.Staff, () => {
                RuleFor(item => item.EmailAddress).EmailAddress().MaximumLength(255);
            });

            RuleFor(item => item.PolicyNumber).NotEmpty().Unless(item => item.AccountBillNumber.HasValue()).WithMessage("Must provide Policy Number or Account Bill Number.");


            When(item => item.AccountBillNumber.HasValue(), () => {
                RuleFor(item => item).Must(item => item.AccountBillNumber.IsAccountBillNumber()).WithMessage("Account Bill Number not in proper format.");
            }).Otherwise(() =>
            {
                RuleFor(item => item).Must(item => item.PolicyNumber.IsPolicyNumber()).WithMessage("Policy Number not in proper format.");
                // just to show in pending payments
                RuleFor(item => item.PolicyId).NotEmpty();
                RuleFor(item => item.PolicyImageNumber).NotEmpty();
            });

            RuleFor(item => item.PaymentAmount).NotEmpty().GreaterThan(0).LessThan(100000);

            When(item => item.UserId.HasValue() == false, () =>
            {
                RuleFor(item => item.Username).NotNull().WithMessage("UserId or Username is required.");
            });
            

            // I know it is null just showing message
            //RuleFor(Function(item) item.ECheckPaymentInformation).NotNull()
            //When(item => item.CreditCardPaymentInformation == null && item.ECheckPaymentInformation == null, () => { RuleFor(item => item.CreditCardPaymentInformation).NotNull().WithMessage("No payment credit card or eCheck data provided."); });
            //updated 6/29/2020 for Fiserv
            When(item => item.CreditCardPaymentInformation == null && item.ECheckPaymentInformation == null && item.WalletItemInfo == null && item.EFTInformation == null, () => { RuleFor(item => item.CreditCardPaymentInformation).NotNull().WithMessage("No payment data provided."); });

            //When(item => item.CreditCardPaymentInformation != null || item.ECheckPaymentInformation != null, () =>
            //updated 6/29/2020 for Fiserv
            When(item => item.CreditCardPaymentInformation != null || item.ECheckPaymentInformation != null || item.WalletItemInfo != null, () =>
            {
                //Unless(item => item.CreditCardPaymentInformation == null, () => { RuleFor(item => item.CreditCardPaymentInformation).SetValidator(new CreditCardPaymentInformationValidator()); });
                //updated 8/19/2020
                Unless(item => item.CreditCardPaymentInformation == null, () =>
                {
                    When(item => IsFiservAndHasFundingAccountToken(item) == false, () =>
                    {
                        RuleFor(item => item.CreditCardPaymentInformation).SetValidator(new CreditCardPaymentInformationValidator());
                    });
                });
                Unless(item => item.ECheckPaymentInformation == null, () => 
                {
                    When(item => item.PaymentSettings?.Echeck_DuplicatePayments_DoCheck == true, () =>
                    {
                        RuleFor(item => item).Must(item => HasDuplicateEcheckPayment(item) == false).WithMessage(item => GetDuplicatePaymentsMessage());
                    });
                    When(item => item.PaymentSettings?.Echeck_NSF_DoCheck == true, () =>
                    {
                        RuleFor(item => item).Must(item => HasNSFEcheckPayment(item) == false).WithMessage(item => GetNSFPaymentsMessage());
                    });
                    RuleFor(item => item.ECheckPaymentInformation).SetValidator(new ECheckPaymentInformationValidator()); 
                });
                //added 6/29/2020 for Fiserv
                Unless(item => item.WalletItemInfo == null, () => { RuleFor(item => item.WalletItemInfo).SetValidator(new WalletItemPaymentInformationValidator()); });
            });

            When(item => item.CreditCardPaymentInformation == null && item.ECheckPaymentInformation == null && item.WalletItemInfo == null && item.EFTInformation != null, () =>
            {
                RuleFor(item => item.UserId).NotEmpty();
                RuleFor(item => item.CashInSource).IsInEnum().NotEqual(API.Enums.CashSource.NA).WithMessage("CashInSource is required.");
            });

            RuleFor(item => item).Must(item => IsDirectBill(item)).WithMessage("Policy is only payable via the agency.");
            //RuleFor(item => item).Must(item => HasBalance(item)).WithMessage("Policy has no balance.");
            RuleFor(item => item).Must(item => IsInPayableStatus(item)).WithMessage("Policy status doesn't permit payments.");
            When(item => item.PaymentSettings?.CheckForImmediateDuplicatePayments == true, () =>
            {
                RuleFor(item => item).Must(item => HasImmediateDuplicatePayment(item)).WithMessage(item => GetImmediateDuplicatePaymentErrorMessage(item));
            });
        }

        private void ApplyCashValidation()
        {
            RuleFor(item => item.PolicyNumber).NotEmpty().Unless(item => item.AccountBillNumber.HasValue()).WithMessage("Must provide Policy Number or Account Bill Number.");

            When(item => item.AccountBillNumber.HasValue(), () => {
                RuleFor(item => item).Must(item => item.AccountBillNumber.IsAccountBillNumber()).WithMessage("Account Bill Number not in proper format.");
            }).Otherwise(() =>
            {
                RuleFor(item => item).Must(item => item.PolicyNumber.IsPolicyNumber()).WithMessage("Policy Number not in proper format.");
                // just to show in pending payments
                //RuleFor(item => item.PolicyId).NotEmpty(); //Can be empty. Fiserv scheduled payments for example, we will figure out which policyid to apply before we call to diamond
                //RuleFor(item => item.PolicyImageNumber).NotEmpty(); //Can be empty. Fiserv scheduled payments for example, we will figure out which image to apply before we call to diamond
            });

            RuleFor(item => item.PaymentAmount).NotEmpty().GreaterThan(0).LessThan(100000);

            //We have already taken the payment, this is just making sure Diamond reflects as much... these validations should have happened before taking the payment.
            //RuleFor(item => item).Must(item => IsDirectBill(item)).WithMessage("Policy is only payable via the agency.");
            //RuleFor(item => item).Must(item => HasBalance(item)).WithMessage("Policy has no balance.");
            //RuleFor(item => item).Must(item => IsInPayableStatus(item)).WithMessage("Policy status doesn't permit payments.");
        }

        private void RequirePaymentInterface()
        {
            CorePaymentValidation();
            RuleFor(item => item).Must(item => item.PaymentInterface != API.Enums.PaymentInterface.None).WithMessage("Payment Interface is required.");
        }

        private void eCheckAfterHoursValidation()
        {
            When(item => item.ECheckPaymentInformation == null || item.PostPaymentInfo == null, () =>
            {
                RuleFor(item => item.CreditCardPaymentInformation).NotNull().WithMessage("Must provide ECheckPaymentInformation object and PostPaymentInfo object for after hours payments");
            });
            //RuleFor(item => item.Username).NotNull().Unless(item => item.UserId > 0).WithMessage("Must provide Username or UserId.");
        }

        private bool IsDirectBill(global::IFM.DataServices.API.RequestObjects.Payments.PaymentData datas)
        {
            if (PolicyInfo(datas) != null)
            {
                if (datas.UserType == global::IFM.DataServices.API.Enums.UserType.Staff)
                    return true;
                else
                {
                    if (info.BillingInformation != null)
                    {
                        return info.BillingInformation.IsDirectBill;
                    }
                }                
            }
            return false;
        }

        private bool HasBalance(global::IFM.DataServices.API.RequestObjects.Payments.PaymentData datas)
        {
            var returnVar = false;
            if (PolicyInfo(datas) != null)
            {
                if (datas.UserType == global::IFM.DataServices.API.Enums.UserType.Staff)
                    return true;
                else
                {
                    if (info.BillingInformation != null)
                    {
                        if (info.BillingInformation.IsPartOfAccountBill)
                        {
                            returnVar = info.BillingInformation.AccountOutstandingBalance > 0 || info.BillingInformation.AccountPayInFull > 0;
                        }
                        else
                        {
                            returnVar = info.BillingInformation.OutstandingBalance > 0 || info.PayInFullBalance > 0;
                        }
                        
                        if (returnVar == false)
                        {
                            //current image is not payable... but a future might exist that needs payment... we need to check for this.
                            if (GetFuturePolicyInfo(datas) != null)
                            {
                                if (info.BillingInformation.IsPartOfAccountBill)
                                {
                                    returnVar = futureImage.BillingInformation.AccountOutstandingBalance > 0 || futureImage.BillingInformation.AccountPayInFull > 0;
                                }
                                else
                                {
                                    returnVar = futureImage.BillingInformation.OutstandingBalance > 0 || futureImage.PayInFullBalance > 0;
                                }
                            }
                        }
                    }
                }
            }
            return returnVar;
        }

        private bool IsInPayableStatus(global::IFM.DataServices.API.RequestObjects.Payments.PaymentData datas)
        {
            if (PolicyInfo(datas) != null)
            {
                if (datas.UserType == global::IFM.DataServices.API.Enums.UserType.Staff)
                    return true;
                else
                {
                    if (info.BillingInformation != null)
                    {
                        return info.BillingInformation.IsInPayableStatus;
                    }
                }
            }
            return false;
        }

        private bool HasDuplicateEcheckPayment(global::IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentData)
        {
            GetDuplicateOrNSFEcheckPayments(paymentData);
            if (_eCheckDuplicateAndNSF != null)
            {
                return _eCheckDuplicateAndNSF.Output_HasDuplicatePaymentError;
            }
            return false;
        }

        private string GetDuplicatePaymentsMessage()
        {
            if (_eCheckDuplicateAndNSF != null)
            {
                return _eCheckDuplicateAndNSF.Output_DuplicatePaymentsErrorMessage;
            }
            return "";
        }

        private bool HasNSFEcheckPayment(global::IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentData)
        {
            GetDuplicateOrNSFEcheckPayments(paymentData);
            if (_eCheckDuplicateAndNSF != null)
            {
                return _eCheckDuplicateAndNSF.Output_HasNSFPaymentsError;
            }
            return false;
        }

        private string GetNSFPaymentsMessage()
        {
            if (_eCheckDuplicateAndNSF != null)
            {
                return _eCheckDuplicateAndNSF.Output_DuplicatePaymentsErrorMessage;
            }
            return "";
        }

        private bool HasImmediateDuplicatePayment(global::IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentData)
        {
            if (PolicyInfo(paymentData) != null)
            {
                if (paymentData.PolicyNumber.IsNotNullEmptyOrWhitespace())
                {
                    return global::IFM.DataServicesCore.BusinessLogic.Payments.ProcessPayment.HasImmediateDuplicatePayment(paymentData.PolicyNumber);
                }
            }
            return false;
        }

        private string GetImmediateDuplicatePaymentErrorMessage(global::IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentData)
        {
            if (PolicyInfo(paymentData) != null)
            {
                if (paymentData.PaymentSettings?.CheckForImmediateDuplicatePayments == true)
                {
                    string minutesVal = (paymentData.PaymentSettings.ImmediateDuplicatePaymentTimeframeInSeconds / 60).FormatNumber(0);
                    string minutesText = minutesVal.TryToGetInt32() > 1 ? $"{minutesVal} minutes" : $"{minutesVal} minute";

                    return $"Policy has had a payment made within the last {minutesText}. If you wish to make another payment, please wait {minutesText} and try again.";
                }
            }
            return "";
        }

        private void GetDuplicateOrNSFEcheckPayments(global::IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentData)
        {
            if (_eCheckDuplicateAndNSF == null && PolicyInfo(paymentData) != null)
            {
                if (paymentData.PaymentSettings?.Echeck_DuplicatePayments_DoCheck == true || paymentData.PaymentSettings?.Echeck_NSF_DoCheck == true)
                {
                    _eCheckDuplicateAndNSF = new IFM.DataServicesCore.BusinessLogic.Payments.eCheck.eCheckDuplicateAndNSFChecking
                    {
                        Input_PolicyNumber = paymentData.PolicyNumber,
                        Input_PolicyId = paymentData.PolicyId,
                        Input_AccountNumber = paymentData.ECheckPaymentInformation?.AccountNumber,
                        Input_RoutingNumber = paymentData.ECheckPaymentInformation?.RoutingNumber,
                    };
                    if (paymentData.PaymentSettings != null)
                    {
                        _eCheckDuplicateAndNSF.Settings_NSFErrorQuantity = paymentData.PaymentSettings.Echeck_NSF_ErrorQuantity;
                        _eCheckDuplicateAndNSF.Settings_NSFPeriodInDays = paymentData.PaymentSettings.Echeck_NSF_PeriodInDays;
                        _eCheckDuplicateAndNSF.Settings_CheckNSF = paymentData.PaymentSettings.Echeck_NSF_DoCheck;
                        _eCheckDuplicateAndNSF.Settings_StartingErrorQuantityForPaymentsOnPolicyId = paymentData.PaymentSettings.Echeck_DuplicatePayments_StartingErrorQuantityByPolicyID;
                        _eCheckDuplicateAndNSF.Settings_StartingErrorQuantityForPaymentsOnPolicyIdWithSameBankInfo = paymentData.PaymentSettings.Echeck_DuplicatePayments_StartingErrorQuantityByPolicyIDWithSameBankInfo;
                        _eCheckDuplicateAndNSF.Settings_StartingErrorQuantityForPaymentsOnPolicyNumber = paymentData.PaymentSettings.Echeck_DuplicatePayments_StartingErrorQuantityByPolicyNumber;
                        _eCheckDuplicateAndNSF.Settings_DuplicatePaymentPeriodInHours = paymentData.PaymentSettings.Echeck_DuplicatePayments_PeriodInHours;
                    }
                    _eCheckDuplicateAndNSF.GetDuplicateAndNSFInfo();
                }
            }
        }

        private IFM.DataServicesCore.CommonObjects.OMP.BasicPolicyInformation GetFuturePolicyInfo(global::IFM.DataServices.API.RequestObjects.Payments.PaymentData datas)
        {
            if(futureImage == null && PolicyInfo(datas) != null)
            {
                //current image is not payable... but a future might exist that needs payment... we need to check for this.
                var PolicyHistories = DataServicesCore.BusinessLogic.OMP.PolicyAccess.GetPolicyHistories(info.PolicyNumber);
                if (PolicyHistories.IsLoaded())
                {
                    var Future = (from p in PolicyHistories where p.PolicyStatusCodeId == 2 orderby p.EffectiveDate descending select p).FirstOrDefault();
                    if (Future != null)
                    {
                        futureImage = DataServicesCore.BusinessLogic.OMP.PolicyAccess.GetPolicyInformation(Future.PolicyId, Future.PolicyImageNumber);
                    }
                }
            }
            return futureImage;
        }

        private IFM.DataServicesCore.CommonObjects.OMP.BasicPolicyInformation PolicyInfo(global::IFM.DataServices.API.RequestObjects.Payments.PaymentData datas)
        {
            if (info == null)
            {
                info = IFM.DataServicesCore.BusinessLogic.OMP.PolicyAccess.GetPolicyInformation(datas.PolicyId, datas.PolicyImageNumber);
            }
            return info;
        }

        private bool IsFiservAndHasFundingAccountToken(global::IFM.DataServices.API.RequestObjects.Payments.PaymentData pd)
        {
            if (pd != null)
            {
                if (IFM_CreditCardProcessing.Common.CreditCardVendor() == IFM_CreditCardProcessing.Enums.CreditCardVendor.Fiserv && string.IsNullOrWhiteSpace(pd.FiservProperties.FundingAccountToken) == false)
                {
                    return true;
                }
            }
            return false;
        }

    }

    public class CreditCardPaymentInformationValidator : AbstractValidator<global::IFM.DataServices.API.RequestObjects.Payments.CreditCardPaymentInformation>
    {

        public CreditCardPaymentInformationValidator()
        {
            RuleFor(item => item.CardNumber).NotEmpty().CreditCard();
            //RuleFor(Function(item) item.NameOnCard).NotEmpty().MaximumLength(255)
            RuleFor(item => item.CardExpireMonth).NotEmpty().GreaterThanOrEqualTo(1).LessThanOrEqualTo(12);
            RuleFor(item => item.CardExpireYear).NotEmpty().GreaterThanOrEqualTo(DateTime.Now.Year);
            When(item => item.CardExpireYear == DateTime.Now.Year, () => { RuleFor(item => item.CardExpireMonth).NotEmpty().GreaterThanOrEqualTo(DateTime.Now.Month); });
            //RuleFor(Function(item) item.ZIPCode).NotEmpty().MinimumLength(5).MaximumLength(5)
        }
    }

    public class ECheckPaymentInformationValidator : AbstractValidator<global::IFM.DataServices.API.RequestObjects.Payments.ECheckPaymentInformation>
    {

        public ECheckPaymentInformationValidator()
        {
            RuleFor(item => item.AccountNumber).NotEmpty().MinimumLength(5).MaximumLength(17);
            RuleFor(item => item.RoutingNumber).NotEmpty().MinimumLength(9).MaximumLength(9);
            RuleFor(item => item.AccountType).NotEmpty();
        }
    }

    //added 6/29/2020 for Fiserv
    public class WalletItemPaymentInformationValidator : AbstractValidator<global::IFM.DataServices.API.RequestObjects.Payments.WalletItem>
    {

        public WalletItemPaymentInformationValidator()
        {
            RuleFor(item => item.FundingAccountToken).NotEmpty();
        }
    }

}