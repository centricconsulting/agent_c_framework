using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;

namespace IFM.DataServices.Validation.Fiserv
{
    public class WalletItemAddValidator : AbstractValidator<IFM.DataServicesCore.CommonObjects.Fiserv.WalletItem>
    {
        public WalletItemAddValidator()
        {
            RuleFor(item => item.KeyIdentifier).NotEmpty();
            RuleFor(item => item.Nickname).NotEmpty();
            //note: should also add validation to check for fundingAcctToken, CardInfo, or BankInfo
            When(item => string.IsNullOrWhiteSpace(item.FundingAccountToken) && string.IsNullOrWhiteSpace(item.CreditCardNumber) && string.IsNullOrWhiteSpace(item.CheckAccountNumber), () => { RuleFor(item => item.FundingAccountToken).NotEmpty().WithMessage("No Funding Account Token, Card Information, or Bank Information provided."); });
            RuleFor(item => item.EmailAddress).NotEmpty();
        }
    }
    public class AddWalletItemBodyValidator : AbstractValidator<IFM.DataServicesCore.CommonObjects.Fiserv.AddWalletItemBody>
    {
        public AddWalletItemBodyValidator()
        {
            When(item => item.Item == null, () => {
                RuleFor(item => item.Item).NotNull().WithMessage("No wallet item data provided.");
            }).Otherwise(() => {
                RuleFor(item => item.Item).SetValidator(new WalletItemAddValidator());
            });
        }
    }
    public class WalletItemUpdateValidator : AbstractValidator<IFM.DataServicesCore.CommonObjects.Fiserv.WalletItem>
    {
        public WalletItemUpdateValidator()
        {
            RuleFor(item => item.FundingAccountToken).NotEmpty();
        }
    }
    public class UpdateWalletItemBodyValidator : AbstractValidator<IFM.DataServicesCore.CommonObjects.Fiserv.UpdateWalletItemBody>
    {
        public UpdateWalletItemBodyValidator()
        {
            When(item => item.Item == null, () => {
                RuleFor(item => item.Item).NotNull().WithMessage("No wallet item data provided.");
            }).Otherwise(() => {
                RuleFor(item => item.Item).SetValidator(new WalletItemUpdateValidator());
            });
        }
    }
    public class WalletItemDeleteValidator : AbstractValidator<IFM.DataServicesCore.CommonObjects.Fiserv.WalletItem>
    {
        public WalletItemDeleteValidator()
        {
            RuleFor(item => item.FundingAccountToken).NotEmpty();
        }
    }
    public class DeleteWalletItemBodyValidator : AbstractValidator<IFM.DataServicesCore.CommonObjects.Fiserv.DeleteWalletItemBody>
    {
        public DeleteWalletItemBodyValidator()
        {
            When(item => item.Item == null, () => {
                RuleFor(item => item.Item).NotNull().WithMessage("No wallet item data provided.");
            }).Otherwise(() => {
                RuleFor(item => item.Item).SetValidator(new WalletItemDeleteValidator());
            });
        }
    }


    //added 7/30/2020
    public class UpdateWalletBodyValidator : AbstractValidator<IFM.DataServicesCore.CommonObjects.Fiserv.UpdateWalletBody>
    {
        public UpdateWalletBodyValidator()
        {
            When(item => item.Item == null, () => {
                RuleFor(item => item.Item).NotNull().WithMessage("No wallet data provided.");
            });
        }
    }
    public class UpdateWalletsBodyValidator : AbstractValidator<IFM.DataServicesCore.CommonObjects.Fiserv.UpdateWalletsBody>
    {
        public UpdateWalletsBodyValidator()
        {
            When(item => item.Items == null, () => {
                RuleFor(item => item.Items).NotNull().WithMessage("No wallet data provided.");
            });
        }
    }


}