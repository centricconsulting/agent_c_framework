using IFM.DataServicesCore.BusinessLogic.Payments.eCheck;
using System;
using IFM.PrimitiveExtensions;
using System.Linq;
using Diamond.Common.Objects.Billing;
using Diamond.Business.ThirdParty.ISO.Passport.RCPOS.Objects;
using APIResponses = IFM.DataServices.API.ResponseObjects;
using APIRequests = IFM.DataServices.API.RequestObjects;
using IFM.DataServicesCore.BusinessLogic.Diamond;
using static IFM.DataServices.API.Enums;
using Diamond.Common.Objects;
using System.Collections.Generic;
using Diamond.Common.Objects.Administration;
using IFM.DataServices.API.RequestObjects.Payments;

namespace IFM.DataServicesCore.BusinessLogic.Payments
{

    public static class ProcessPayment
    {
        private const string location = "IFMDataServices.Core.BusinessLogic.Payments.ProcessPayment";

        public static bool IsAfterHoursPayment()
        {
            if (AppConfig.AutomaticallyPostDiaPayments.TryToGetBoolean() == true)
            {
                if (AppConfig.UseAutomaticPostTimes.TryToGetBoolean() == false)
                {
                    return false;
                }
                else
                {
                    var startTime = AppConfig.AutomaticPostStartTime.TryToGetDateTime();
                    var endTime = AppConfig.AutomaticPostEndTime.TryToGetDateTime();
                    if (startTime < DateTime.Now && endTime > DateTime.Now)
                    {
                        return false;
                    }
                }

            }
            return true;
        }

        public static APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> MakePayment(global::IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentInfo)
        {
            APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr = new APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult>();
            if (paymentInfo != null)
            {
                GetUserInfo(paymentInfo);

                if (paymentInfo.CreditCardPaymentInformation != null)
                {
                    PayWithCreditCard(sr, paymentInfo);
                }
                else if (paymentInfo.ECheckPaymentInformation != null)
                {
                    PayWithEcheck(sr, paymentInfo);
                }
                else if (paymentInfo.WalletItemInfo != null)
                {
                    BusinessLogic.Fiserv.WalletHelper walletHelper = new BusinessLogic.Fiserv.WalletHelper();
                    walletHelper.PayWithWalletItem(sr, paymentInfo);
                }
                else if(paymentInfo.EFTInformation != null)
                {
                    PayWithEFT(sr, paymentInfo);
                }
                else
                {
                    sr.Messages.CreateErrorMessage("No payment information provided.");
                }
                if (sr.ResponseData.PaymentCompleted)
                {
                    sr.Messages.CreateGeneralMessage("Payment Completed.");
                }
            }

            if (sr.ResponseData.PaymentCompleted)
            {
                try
                {
                    StoreSuccessfullPayment(paymentInfo, sr);
                }
                catch (Exception ex)
                {
                    sr.DetailedErrorMessages.CreateErrorMessage("Unable to store payment in APIPayment table. Error: " + ex.Message);
                }
            }

            return sr;
        }

        public static APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> PostPayment(global::IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentInfo)
        {
            APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr = new APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult>();
            if (paymentInfo != null)
            {
                GetUserInfo(paymentInfo);

                if (paymentInfo.CreditCardPaymentInformation != null)
                {
                    PostWithCreditCard(sr, paymentInfo);
                }
                else if (paymentInfo.ECheckPaymentInformation != null)
                {
                    PostWithEcheck(sr, paymentInfo);
                }
                else
                {
                    sr.Messages.CreateErrorMessage("No payment information provided.");
                }

                if (sr.ResponseData.PaymentCompleted)
                {
                    sr.Messages.CreateGeneralMessage("Post Completed.");
                }
            }

            return sr;
        }

        private static void PostWithEcheck(APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr, global::IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentInfo)
        {
            if (paymentInfo != null && paymentInfo.ECheckPaymentInformation != null)
            {
                if (paymentInfo.ECheckPaymentInformation.AccountNumber.IsNotNullEmptyOrWhitespace())
                {
                    paymentInfo.ECheckPaymentInformation.AccountNumber = paymentInfo.ECheckPaymentInformation.AccountNumber.Trim();
                }
                if (paymentInfo.ECheckPaymentInformation.RoutingNumber.IsNotNullEmptyOrWhitespace())
                {
                    paymentInfo.ECheckPaymentInformation.RoutingNumber = paymentInfo.ECheckPaymentInformation.RoutingNumber.Trim();
                }

                if (paymentInfo.ECheckPaymentInformation.ExcludeFromBankFile == global::IFM.DataServices.API.Enums.BankFileExclusion.Default)
                {
                    if (AppConfig.DefaultExcludeFromBankFileOption.HasValue() && AppConfig.DefaultExcludeFromBankFileOption.TryToGetBoolean())
                    {
                        paymentInfo.ECheckPaymentInformation.ExcludeFromBankFile = global::IFM.DataServices.API.Enums.BankFileExclusion.True;
                    }
                    else
                    {
                        paymentInfo.ECheckPaymentInformation.ExcludeFromBankFile = global::IFM.DataServices.API.Enums.BankFileExclusion.False;
                    }
                }

                var echeckProcessor = new eCheck.EcheckProcessor(paymentInfo);
                echeckProcessor.ProcessPostVendorEcheckPayment();

                foreach (string st in echeckProcessor.EcheckResponse.UserFeedBack)
                {
                    sr.Messages.CreateErrorMessage(st);
                }
                foreach (string st in echeckProcessor.EcheckResponse.DetailedErrorMsgs)
                {
                    sr.DetailedErrorMessages.CreateErrorMessage(st);
                }
                sr.ResponseData.PaymentConfirmationNumber = echeckProcessor.EcheckResponse.ConfirmationNumber;
                sr.ResponseData.PaymentCompleted = echeckProcessor.EcheckResponse.Completed;
            }
        }

        private static void PayWithEcheck(APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr, global::IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentInfo)
        {
            if (paymentInfo != null && paymentInfo.ECheckPaymentInformation != null)
            {
                paymentInfo.ECheckPaymentInformation.AccountNumber = paymentInfo.ECheckPaymentInformation.AccountNumber.Trim();
                paymentInfo.ECheckPaymentInformation.RoutingNumber = paymentInfo.ECheckPaymentInformation.RoutingNumber.Trim();

                if (paymentInfo.ECheckPaymentInformation.ExcludeFromBankFile == global::IFM.DataServices.API.Enums.BankFileExclusion.Default)
                {
                    if (AppConfig.DefaultExcludeFromBankFileOption.HasValue() && AppConfig.DefaultExcludeFromBankFileOption.TryToGetBoolean())
                    {
                        paymentInfo.ECheckPaymentInformation.ExcludeFromBankFile = global::IFM.DataServices.API.Enums.BankFileExclusion.True;
                    }
                    else
                    {
                        paymentInfo.ECheckPaymentInformation.ExcludeFromBankFile = global::IFM.DataServices.API.Enums.BankFileExclusion.False;
                    }
                }

                var echeckProcessor = new eCheck.EcheckProcessor(paymentInfo);
                echeckProcessor.ProcessEcheckPayment();

                foreach (string st in echeckProcessor.EcheckResponse.UserFeedBack)
                {
                    sr.Messages.CreateErrorMessage(st);
                }
                foreach (string st in echeckProcessor.EcheckResponse.DetailedErrorMsgs)
                {
                    sr.DetailedErrorMessages.CreateErrorMessage(st);
                }
                sr.ResponseData.PaymentConfirmationNumber = echeckProcessor.EcheckResponse.ConfirmationNumber;
                sr.ResponseData.PaymentCompleted = echeckProcessor.EcheckResponse.Completed;
            }
        }

        private static void PayWithCreditCard(APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr, global::IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentInfo)
        {
            //paymentInfo
            if (paymentInfo != null && paymentInfo.CreditCardPaymentInformation != null)
            {
                IFM_CreditCardProcessing.CreditCardTransaction cct = FillCreditCardTransactionObject(paymentInfo, sr);

                //DetermineCreditCardPaymentType(cct, paymentInfo);

                IFM_CreditCardProcessing.CreditCardMethods ccm = new IFM_CreditCardProcessing.CreditCardMethods();

                IFM_CreditCardProcessing.CreditCardTransactionResult cctr = ccm.ProcessCreditCardTransaction(cct);

                if (cctr != null)
                {
                    if (cctr.Success)
                    {
                        PostWithCreditCard(cct, cctr, sr, paymentInfo);
                    }
                    else
                    {
                        sr.Messages.CreateErrorMessage(cctr.CustomerFriendlyErrorMessage);
                        sr.DetailedErrorMessages.CreateErrorMessage(cctr.DetailedErrorMessage);
                        if (cctr.AttemptedToSendCreditCardTransactionToVendor)
                        {
                            if (cctr.CreditCardTransactionDeclined)
                            {
                                //declined
                                sr.Messages.CreateErrorMessage(cctr.DeclineMessage);
                            }
                            else if (cctr.EncounteredVendorError)
                            {
                                //vendor error
                                sr.Messages.CreateErrorMessage("Credit card payment failed. Our 3rd party processing center is temporarily out of service.");
                            }
                            else
                            {
                                //general error don't know what happened
                                sr.Messages.CreateErrorMessage("Credit card payment failed.");
                            }
                        }
                        else
                        {
                            //either failed validation or failed to insert transaction
                            if (cctr.Attempted_IFM_CreditCardTransaction_Insert == true)
                            {
                                if (cctr.SuccessfullyInserted_IFM_CreditCardTransaction == false)
                                {
                                    //insert failed
                                    sr.Messages.CreateErrorMessage("Credit card payment failed #1.");
                                }
                                else
                                {
                                    //shouldn't get here; show generic message
                                    sr.Messages.CreateErrorMessage("Credit card payment failed #2.");
                                }
                            }
                            else
                            {
                                //general error don't know what happened
                                sr.Messages.CreateErrorMessage("Credit card payment failed #3.");
                            }
                        }
                    }
                }
                else
                {
                    //general error don't know what happened
                    sr.Messages.CreateErrorMessage("Credit card payment failed #4.");
                }
            }
        }

        private static void PostWithCreditCard(APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr, global::IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentInfo)
        {
            if (paymentInfo != null && paymentInfo.CreditCardPaymentInformation != null && sr != null)
            {
                IFM_CreditCardProcessing.CreditCardTransaction cct = FillCreditCardTransactionObject(paymentInfo, sr);
                PostWithCreditCard(cct, null, sr, paymentInfo);
            }
        }

        private static void PostWithCreditCard(IFM_CreditCardProcessing.CreditCardTransaction cct, IFM_CreditCardProcessing.CreditCardTransactionResult cctr, APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr, global::IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentInfo)
        {
            if (paymentInfo != null && paymentInfo.CreditCardPaymentInformation != null && cct != null && sr != null)
            {
                //Will not use after hours CC processing through API yet since code is actually pretty different... but this is probably something that would be needed for if/when we try it.
                if (paymentInfo.PostPaymentInfo != null && paymentInfo.PostPaymentInfo.ConfirmationNumber.IsNotNullEmptyOrWhitespace())
                {
                    sr.ResponseData.PaymentConfirmationNumber = paymentInfo.PostPaymentInfo.ConfirmationNumber;
                }

                //DetermineCreditCardPaymentType(cct, paymentInfo);

                bool successfullyInsertedCreditCardPayment;
                string creditCardPaymentInsertErrMsg = "";

                //successfullyInsertedCreditCardPayment = ccm.SuccessfullyInsertedCreditCardPayment(cct.PolicyInfo.PolicyNumber, cct.PaymentAmount, "Y",
                //    IFM_CreditCardProcessing.Enums.UserType.Policyholder, cct.CreditCardInfo.CreditCardType, IFM_CreditCardProcessing.Enums.PaymentInterface.MemberPortalSite,
                //    false, ref creditCardPaymentInsertErrMsg);
                //sending False for optional withApp param; could send ifm_ccType for ccType param, but using property on CreditCardInfo in case library changes it
                //updated 5/29/2020 for pmtInterface and userType
                //successfullyInsertedCreditCardPayment = ccm.SuccessfullyInsertedCreditCardPayment(cct.PolicyInfo.PolicyNumber, cct.PaymentAmount, "Y",
                //    userType, cct.CreditCardInfo.CreditCardType, cct.PaymentInterface,
                //    false, ref creditCardPaymentInsertErrMsg);

                IFM_CreditCardProcessing.CreditCardPaymentDBRecord CCPaymentDBRecord;
                if (cctr != null)
                {
                    CCPaymentDBRecord = new IFM_CreditCardProcessing.CreditCardPaymentDBRecord(cct, cctr);
                }
                else
                {
                    CCPaymentDBRecord = new IFM_CreditCardProcessing.CreditCardPaymentDBRecord(cct);
                }
                successfullyInsertedCreditCardPayment = CCPaymentDBRecord.InsertRecord(ref creditCardPaymentInsertErrMsg);

                //completedWithOuterror = ccm.SuccessfullyInsertedDiamondAgentPayment(cct.PolicyInfo.PolicyNumber, cct.PaymentAmount, (int)cardPaymentType,
                //    (int)global::Diamond.Common.Enums.Billing.BillingCashType.Payment, (int)global::Diamond.Common.Enums.Billing.BillingReason.None, "N", errorMsg: ref errMsg);
                //should add policyId and policyImageNum at some point
                //updated 4/23/2018

                if (paymentInfo.PaymentSettings?.IgnoreAfterHoursPostingRules != true && IsAfterHoursPayment())
                {
                    PostAfterHoursCreditCardPayment(cct, cctr, sr);
                }
                else
                {
                    PostRealtimeCreditCardPayment(cct, cctr, sr);
                }
            }
        }

        private static void PayWithEFT(APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr, IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentData)
        {
            IFM.DataServicesCore.BusinessLogic.Payments.AgencyEFTPayment eft = new AgencyEFTPayment(paymentData);
            if (eft.MakeAgencyEFTPayment())
            {
                sr.ResponseData.PaymentCompleted = true;
            }
            else
            {
                sr.ResponseData.PaymentCompleted = false;
            }
        }

        private static void PostAfterHoursCreditCardPayment(IFM_CreditCardProcessing.CreditCardTransaction cct, IFM_CreditCardProcessing.CreditCardTransactionResult cctr, APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr)
        {
            string swept = "N";
            InsertCreditCardDiamondPaymentRecord(swept, cct, cctr, sr);
        }

        private static void PostRealtimeCreditCardPayment(IFM_CreditCardProcessing.CreditCardTransaction cct, IFM_CreditCardProcessing.CreditCardTransactionResult cctr, APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr)
        {
            ApplyCash applyDiamondCash = new ApplyCash(cct);
            var appliedCash = applyDiamondCash.DoApplyCash();

            if (appliedCash)
            {
                string swept = "Y";
                InsertCreditCardDiamondPaymentRecord(swept, cct, cctr, sr);
            }
            else
            {
                sr.Messages.CreateErrorMessage("Unable to complete payment at this time.");
                sr.DetailedErrorMessages.CreateErrorMessage(applyDiamondCash.ErrorMessage);
            }
        }

        private static void InsertCreditCardDiamondPaymentRecord(string swept, IFM_CreditCardProcessing.CreditCardTransaction cct, IFM_CreditCardProcessing.CreditCardTransactionResult cctr, APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr)
        {
            string errMsg = "";
            IFM_CreditCardProcessing.DiamondAgentPaymentDBRecord DiamondPaymentDBRecord;
            if (cctr == null)
            {
                DiamondPaymentDBRecord = new IFM_CreditCardProcessing.DiamondAgentPaymentDBRecord(cct);
            }
            else
            {
                DiamondPaymentDBRecord = new IFM_CreditCardProcessing.DiamondAgentPaymentDBRecord(cct, cctr);
                sr.ResponseData.PaymentConfirmationNumber = cctr.ConfirmationNumber;
            }

            if (cct.PaymentInterface != IFM_CreditCardProcessing.Enums.PaymentInterface.None)
                DiamondPaymentDBRecord.StatusUpdateUserForSweptY = "API_" + cct.PaymentInterface.ToString();
            else
                DiamondPaymentDBRecord.StatusUpdateUserForSweptY = "API_IFMDataServices";

            DiamondPaymentDBRecord.Swept = swept;
            bool completedTableInsertWithOutError = DiamondPaymentDBRecord.InsertRecord(ref errMsg);

            if (completedTableInsertWithOutError == false)
            {
                IFM.IFMErrorLogging.LogIssue(errMsg, $"{location}.InsertCreditCardDiamondPaymentRecord()");
            }
            
            sr.ResponseData.PaymentCompleted = true;
            //sr.Messages.CreateGeneralMessage("Payment completed.");
        }

        private static IFM_CreditCardProcessing.CreditCardTransaction FillCreditCardTransactionObject(APIRequests.Payments.PaymentData paymentInfo, APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr)
        {
            IFM_CreditCardProcessing.CreditCardTransaction cct = new IFM_CreditCardProcessing.CreditCardTransaction();
            cct.CreditCardInfo = new IFM_CreditCardProcessing.CreditCardInfo();
            //cct.IsPaymentWithApplication = paymentInfo.WithApp;
            cct.PaymentAmount = paymentInfo.PaymentAmount.ToString();
            cct.ConfirmationEmail = paymentInfo.EmailAddress ?? "";
            cct.SendConfirmationEmailOnSuccess = false; //added 8/20/2020 since we already have Leaf and/or Fiserv confirmation emails; default is true, but emails were previously failing since mailhost key was missing from config

            cct.PaymentInterface = (IFM_CreditCardProcessing.Enums.PaymentInterface)(int)paymentInfo.PaymentInterface;

            cct.InsertIntoCreditCardTableOnSuccess = false;
            cct.InsertIntoAgentPaymentDiamondTableOnSuccess = false;

            cct.Fiserv_FeeAmount = paymentInfo.FeeAmount; //added 9/25/2024

            cct.PolicyInfo = new IFM_CreditCardProcessing.PolicyInfo()
            {
                PolicyNumber = paymentInfo.PolicyNumber,
                PolicyId = paymentInfo.PolicyId,
                PolicyImageNum = paymentInfo.PolicyImageNumber,
                AccountBillNumber = paymentInfo.AccountBillNumber
            };

            cct.UserInfo = new IFM_CreditCardProcessing.UserInfo
            {
                DiamondUsername = paymentInfo.Username,
                DiamondUserId = paymentInfo.UserId,
                DiamondUserLoginDomain = paymentInfo.UserLoginDomain,
                UserType = (IFM_CreditCardProcessing.Enums.UserType)paymentInfo.UserType
            };

            if (string.IsNullOrWhiteSpace(paymentInfo.CreditCardPaymentInformation.CardNumber) == false)
            {
                paymentInfo.CreditCardPaymentInformation.CardNumber = paymentInfo.CreditCardPaymentInformation.CardNumber.Trim();
            }

            cct.CreditCardInfo.CreditCardNumber = paymentInfo.CreditCardPaymentInformation.CardNumber;
            cct.CreditCardInfo.CreditCardExpirationMonth = paymentInfo.CreditCardPaymentInformation.CardExpireMonth.ToString().PadLeft(2, '0');
            cct.CreditCardInfo.CreditCardExpirationYear = paymentInfo.CreditCardPaymentInformation.CardExpireYear.ToString();

            cct.CreditCardInfo.BillingAddressee = new IFM_CreditCardProcessing.BillingAddressee();
            if (paymentInfo.CreditCardPaymentInformation.NameOnCard.IsNotNullEmptyOrWhitespace())
            {
                cct.CreditCardInfo.BillingAddressee.Name = new IFM_CreditCardProcessing.Name()
                {
                    Last = paymentInfo.CreditCardPaymentInformation.NameOnCard
                };
            }
            else
            {
                if(paymentInfo.CreditCardPaymentInformation.FirstName.IsNotNullEmptyOrWhitespace() || paymentInfo.CreditCardPaymentInformation.MiddleName.IsNotNullEmptyOrWhitespace() || paymentInfo.CreditCardPaymentInformation.LastName.IsNotNullEmptyOrWhitespace())
                {
                    cct.CreditCardInfo.BillingAddressee.Name = new IFM_CreditCardProcessing.Name();
                    cct.CreditCardInfo.BillingAddressee.Name.First = cct.CreditCardInfo.BillingAddressee.Name.First.SetIfVariableHasValue(paymentInfo.CreditCardPaymentInformation.FirstName);
                    cct.CreditCardInfo.BillingAddressee.Name.Middle = cct.CreditCardInfo.BillingAddressee.Name.Middle.SetIfVariableHasValue(paymentInfo.CreditCardPaymentInformation.MiddleName);
                    cct.CreditCardInfo.BillingAddressee.Name.Last = cct.CreditCardInfo.BillingAddressee.Name.Last.SetIfVariableHasValue(paymentInfo.CreditCardPaymentInformation.LastName);
                }
            }

            if (paymentInfo.CreditCardPaymentInformation.ZIPCode.IsNotNullEmptyOrWhitespace()) //added IF 8/19/2020
            {
                cct.CreditCardInfo.BillingAddressee.Address = new IFM_CreditCardProcessing.Address()
                {
                    Zip = paymentInfo.CreditCardPaymentInformation.ZIPCode //added 7/19/2020
                }; //added 7/19/2020
            }

            cct.CreditCardInfo.SecurityCode = paymentInfo.CreditCardPaymentInformation.SecurityCode; //added 7/19/2020

            //added 5/29/2020 for Fiserv
            if (IFM_CreditCardProcessing.Common.CreditCardVendor() == IFM_CreditCardProcessing.Enums.CreditCardVendor.Fiserv)
            {
                cct.Fiserv_AuthToken = paymentInfo.FiservProperties.AuthToken;
                cct.Fiserv_SessionToken = paymentInfo.FiservProperties.SessionToken;
                cct.Fiserv_SessionId = paymentInfo.FiservProperties.SessionId;
                cct.Fiserv_FundingAccountToken = paymentInfo.FiservProperties.FundingAccountToken;
                cct.Fiserv_AddToWallet = paymentInfo.FiservProperties.AddToWallet;
                cct.Fiserv_WalletItem_Nickname = paymentInfo.FiservProperties.WalletItemNickname;
                cct.Fiserv_IframeResponse = paymentInfo.FiservProperties.IframeResponse; //added 6/29/2020
                cct.Fiserv_FundingAccountToken_FundingMethod = paymentInfo.FiservProperties.FundingMethod;
                cct.Fiserv_IframeResponse = paymentInfo.FiservProperties.IframeResponse;
            }

            paymentInfo.CashInSource = DetermineCreditCardPaymentTypeAndCashSource(cct, paymentInfo);

            if (IFM_CreditCardProcessing.Common.CreditCardVendor() != IFM_CreditCardProcessing.Enums.CreditCardVendor.Fiserv || string.IsNullOrWhiteSpace(paymentInfo.FiservProperties.FundingAccountToken)) //added IF 5/29/2020 for Fiserv; previously happening every time
            {
                if (paymentInfo.CashInSource == CashSource.NA || cct.CreditCardInfo.CreditCardType == IFM_CreditCardProcessing.Enums.CreditCardType.None)
                {
                    if (paymentInfo?.CreditCardPaymentInformation?.CardNumber != null)
                        IFMErrorLogging.LogIssue("Was unable to identify card type.", paymentInfo.CreditCardPaymentInformation.CardNumber.MaskString());
                    else
                        IFMErrorLogging.LogIssue(nameof(paymentInfo.CreditCardPaymentInformation.CardNumber) + " was null.");

                    sr.Messages.CreateErrorMessage("Unable to determine card type.");
                }
            }

            return cct;
        }

        private static void DetermineCreditCardPaymentType(IFM_CreditCardProcessing.CreditCardTransaction cct, IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentData)
        {
            CashSource cardCashSource = DetermineCreditCardPaymentTypeAndCashSource(cct, paymentData);
        }

        private static CashSource DetermineCreditCardPaymentTypeAndCashSource(IFM_CreditCardProcessing.CreditCardTransaction cct, IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentData)
        {
            CashSource cardCashSource = CashSource.NA;

            if (cct.CreditCardInfo.CreditCardNumber.HasValue()) //shouldn't be getting used anymore as we always pass the CC num to Fiserv ahead of time. Should be using FundingMethod now.
            {
                cardCashSource = DetermineCreditCardPaymentTypeAndCashSourceByCardNumber(cct);
            }
            else if (cct.Fiserv_FundingAccountToken_FundingMethod.HasValue())
            {
                if (paymentData.FiservProperties.CCTypeText.IsNotNullEmptyOrWhitespace())
                {
                    cardCashSource = DetermineCreditCardPaymentTypeAndCashSourceByFundingMethodAndFiservCCTypeText(cct, paymentData);
                }
                else
                {
                    cardCashSource = DetermineCreditCardPaymentTypeAndCashSourceByFundingMethod(cct);
                }
            }            
            return cardCashSource;
        }

        private static CashSource DetermineCreditCardPaymentTypeAndCashSourceByFundingMethodAndFiservCCTypeText(IFM_CreditCardProcessing.CreditCardTransaction cct, IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentData)
        {
            bool hasPreferred = false;
            List<string> fundingMethodList = new List<string>();
            if (cct.Fiserv_FundingAccountToken_FundingMethod.IsNotNullEmptyOrWhitespace())
            {
                fundingMethodList.Add(cct.Fiserv_FundingAccountToken_FundingMethod);
            }
            if(paymentData.FiservProperties.CCTypeText.IsNotNullEmptyOrWhitespace())
            {
                fundingMethodList.Add(paymentData.FiservProperties.CCTypeText);
            }
            CashSource cardCashSource = GetCreditCardPaymentType(IFM_CreditCardProcessing.Common.PreferredCreditCardTypeForFiservPaymentMethodsOrExisting(fundingMethodList, cct.CreditCardInfo.CreditCardType, ref hasPreferred), cct);
            return cardCashSource;
        }

        private static CashSource DetermineCreditCardPaymentTypeAndCashSourceByFundingMethod(IFM_CreditCardProcessing.CreditCardTransaction cct)
        {
            bool hasPreferred = false;
            CashSource cardCashSource = GetCreditCardPaymentType(IFM_CreditCardProcessing.Common.PreferredCreditCardTypeForFiservPaymentMethodOrExisting(cct.Fiserv_FundingAccountToken_FundingMethod, cct.CreditCardInfo.CreditCardType, ref hasPreferred), cct);
            return cardCashSource;
        }

        private static CashSource DetermineCreditCardPaymentTypeAndCashSourceByCardNumber(IFM_CreditCardProcessing.CreditCardTransaction cct)
        {
            CashSource cardCashSource = GetCreditCardPaymentType(IFM_CreditCardProcessing.Common.CreditCardTypeForNumber(cct.CreditCardInfo.CreditCardNumber), cct);
            return cardCashSource;
        }

        private static CashSource GetCreditCardPaymentType(IFM_CreditCardProcessing.Enums.CreditCardType CCType, IFM_CreditCardProcessing.CreditCardTransaction cct)
        {
            CashSource cardCashSource = CashSource.NA;
            switch (CCType)
            {
                case IFM_CreditCardProcessing.Enums.CreditCardType.Visa:
                    cct.CreditCardInfo.CreditCardType = IFM_CreditCardProcessing.Enums.CreditCardType.Visa;
                    if (cct.PaymentInterface == IFM_CreditCardProcessing.Enums.PaymentInterface.AgentsOnlySite)
                        cardCashSource = cct.IsPaymentWithApplication ? CashSource.WebAgencyCCWithAppVisa : CashSource.WebAgencyCCVisa;
                    else
                        cardCashSource = CashSource.WebCCVisa;
                    break;

                case IFM_CreditCardProcessing.Enums.CreditCardType.MasterCard:
                    cct.CreditCardInfo.CreditCardType = IFM_CreditCardProcessing.Enums.CreditCardType.MasterCard;
                    if (cct.PaymentInterface == IFM_CreditCardProcessing.Enums.PaymentInterface.AgentsOnlySite)
                        cardCashSource = cct.IsPaymentWithApplication ? CashSource.WebAgencyCCWithAppMasterCard : CashSource.WebAgencyCCMasterCard;
                    else
                        cardCashSource = CashSource.WebCCMasterCard;
                    break;

                case IFM_CreditCardProcessing.Enums.CreditCardType.AmericanExpress:
                    cct.CreditCardInfo.CreditCardType = IFM_CreditCardProcessing.Enums.CreditCardType.AmericanExpress;
                    if (cct.PaymentInterface == IFM_CreditCardProcessing.Enums.PaymentInterface.AgentsOnlySite)
                        cardCashSource = cct.IsPaymentWithApplication ? CashSource.WebAgencyCCWithAppAmericanExpress : CashSource.WebAgencyCCAmericanExpress;
                    else
                        cardCashSource = CashSource.WebCCAmericanExpress;
                    break;

                case IFM_CreditCardProcessing.Enums.CreditCardType.Discover:
                    cct.CreditCardInfo.CreditCardType = IFM_CreditCardProcessing.Enums.CreditCardType.Discover;
                    if (cct.PaymentInterface == IFM_CreditCardProcessing.Enums.PaymentInterface.AgentsOnlySite)
                        cardCashSource = cct.IsPaymentWithApplication ? CashSource.WebAgencyCCWithAppDiscover : CashSource.WebAgencyCCDiscover;
                    else
                        cardCashSource = CashSource.WebCCDiscover;
                    break;
                default:
                    cct.CreditCardInfo.CreditCardType = IFM_CreditCardProcessing.Enums.CreditCardType.None;
                    cardCashSource = CashSource.CreditCard;
                    break;
            }
            return cardCashSource;
        }

        private static void GetUserInfo(APIRequests.Payments.PaymentData payment)
        {
            bool continueLookup = true;

            //Check if it is a memberportal payment, the lookup won't work for those
            switch (payment.PaymentInterface)
            {
                case PaymentInterface.MemberPortalSite:
                case PaymentInterface.MobileSite:
                case PaymentInterface.OneTimePayment:
                case PaymentInterface.MobileOneTimePayment:
                    continueLookup = false;
                    break;
            }

            if (continueLookup == true && payment.Username.HasValue() && payment.UserId.HasValue() && payment.UserLoginDomain.HasValue())
            {
                if (payment.EFTInformation != null)
                {
                    if (payment.EFTInformation.LegacyUserId.HasValue() && payment.EFTInformation.LegacyUserId != payment.UserId)
                    {
                        continueLookup = false;
                    }
                }
                else
                {
                    continueLookup = false;
                }
            }

            if (continueLookup)
            {
                UserInfo userInfo = new UserInfo();
                if (payment.Username.IsNullEmptyOrWhitespace() && payment.UserId == 0)
                {
                    userInfo.GetUserInfo(AppConfig.PrintUserName);
                }
                else if (payment.UserId > 0)
                {
                    userInfo.GetUserInfo(payment.UserId);
                }
                else if (payment.Username.IsNotNullEmptyOrWhitespace())
                {
                    userInfo.GetUserInfo(payment.Username);
                }
                payment.Username = userInfo.Username;
                payment.UserId = userInfo.UserId;
                payment.UserLoginDomain = userInfo.UserLoginDomain;
                if (payment.EFTInformation != null)
                {
                    payment.EFTInformation.LegacyUserId = userInfo.LegacyUserId;
                }
            }
        }

        public static bool HasImmediateDuplicatePayment(string policyNumber, int SecondsAgoToCheck = 120)
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetDuplicatePaymentsByPolicyNumber", conn) { CommandType = System.Data.CommandType.StoredProcedure })
{
                    cmd.Parameters.AddWithValue("@PolicyNumber", policyNumber);
                    cmd.Parameters.AddWithValue("@WithinNumberOfSecondsAgo", SecondsAgoToCheck);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool StoreSuccessfullPayment(global::IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentData, APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr)
        {
            var APIPaymentTypeId = DetermineAPIPaymentType(paymentData);

            using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_Insert_APIPayment", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@AccountBillNumber", paymentData.AccountBillNumber);
                    cmd.Parameters.AddWithValue("@PolicyNumber", paymentData.PolicyNumber);
                    cmd.Parameters.AddWithValue("@PaymentAmount", paymentData.PaymentAmount);
                    cmd.Parameters.AddWithValue("@UserId", paymentData.UserId);
                    cmd.Parameters.AddWithValue("@Username", paymentData.Username);
                    cmd.Parameters.AddWithValue("@PaymentInterfaceId", (int)paymentData.PaymentInterface);
                    cmd.Parameters.AddWithValue("@APIPaymentTypeId", APIPaymentTypeId);

                    cmd.ExecuteNonQuery();
                }
            }
            return false;
        }

        public static IFM.DataServicesCore.CommonObjects.Enums.Enums.APIPaymentTypeId DetermineAPIPaymentType(global::IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentData)
        {
            IFM.DataServicesCore.CommonObjects.Enums.Enums.APIPaymentTypeId APIPaymentTypeId = CommonObjects.Enums.Enums.APIPaymentTypeId.NA;
            if (paymentData != null)
            {
                if (paymentData.CreditCardPaymentInformation != null)
                {
                    APIPaymentTypeId = CommonObjects.Enums.Enums.APIPaymentTypeId.CreditCard;
                }
                else if (paymentData.ECheckPaymentInformation != null)
                {
                    APIPaymentTypeId = CommonObjects.Enums.Enums.APIPaymentTypeId.Echeck;
                }
                else if(paymentData.EFTInformation != null)
                {
                    APIPaymentTypeId = CommonObjects.Enums.Enums.APIPaymentTypeId.AgencyEFT;
                }
                else if (paymentData.WalletItemInfo != null)
                {
                    APIPaymentTypeId = DetermineAPIPaymentTypeFromWalletItem(paymentData.WalletItemInfo.FundingAccountToken);
                }
            }
            return APIPaymentTypeId;
        }

        public static IFM.DataServicesCore.CommonObjects.Enums.Enums.APIPaymentTypeId DetermineAPIPaymentTypeFromWalletItem(string fundingAccountToken)
        {
            IFM.DataServicesCore.CommonObjects.Enums.Enums.APIPaymentTypeId APIPaymentTypeId = CommonObjects.Enums.Enums.APIPaymentTypeId.NA;

            string fundingCategory = "";
            using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.Conn))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_Get_FiservWalletItem", conn) { CommandType = System.Data.CommandType.StoredProcedure })
{
                    cmd.Parameters.AddWithValue("@fundingAccountToken", fundingAccountToken);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                fundingCategory = reader["fundingCategory"].TryToGetString();
                            }
                        }
                    }
                }
            }

            switch (fundingCategory)
            {
                case "Credit":
                case "Debit":
                    APIPaymentTypeId = CommonObjects.Enums.Enums.APIPaymentTypeId.WalletCreditCard;
                    break;
                case "Check":
                    APIPaymentTypeId = CommonObjects.Enums.Enums.APIPaymentTypeId.WalletEcheck;
                    break;
            }

            return APIPaymentTypeId;
        }
    }
}