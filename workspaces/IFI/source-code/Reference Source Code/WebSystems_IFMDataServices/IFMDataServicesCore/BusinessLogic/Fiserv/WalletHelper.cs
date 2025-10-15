using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFM.DataServicesCore.CommonObjects.Fiserv;
using cc = IFM_CreditCardProcessing;
using IFM_CreditCardProcessing;
using IFM.DataServicesCore.BusinessLogic.Payments;
using Diamond.Common.Objects.Billing;
using APIResponses = IFM.DataServices.API.ResponseObjects;
using Diamond.Business.ThirdParty.QwikSignAPI;

namespace IFM.DataServicesCore.BusinessLogic.Fiserv
{
    public class WalletHelper : ModelBase
    {
        public List<WalletItem> WalletItemsForFiservWalletItems(List<IFM_FiservDatabaseObjects.FiservWalletItem> wis)
        {
            List<WalletItem> walletItems = null;

            if (wis != null && wis.Count > 0)
            {
                foreach (IFM_FiservDatabaseObjects.FiservWalletItem wi in wis)
                {
                    if (wi != null)
                    {
                        //WalletItem wItem = new WalletItem();
                        //wItem.BankAccountType = wi.bankAccountType;
                        //wItem.CheckAccountNumber = "";
                        //wItem.CreditCardNumber = "";
                        //wItem.FirstName = wi.firstName;
                        //wItem.ExpirationMonth = "";
                        //wItem.ExpirationYear = "";
                        //wItem.FiservWalletId = wi.fiservWalletId;
                        //wItem.FiservWalletItemId = wi.fiservWalletItemId;
                        //wItem.FundingAccountLastFourDigits = wi.fundingAccountLastFourDigit;
                        //wItem.FundingAccountToken = wi.fundingAccountToken;
                        //wItem.FundingAccountType = wi.fundingAccountType;
                        //wItem.FundingCategory = wi.fundingCategory;
                        //wItem.FundingMethod = wi.fundingMethod;
                        //if (wi.fs != null)
                        //{
                        //    wItem.KeyIdentifier = wi.fs.userId;
                        //}
                        //else
                        //{
                        //    wItem.KeyIdentifier = "";
                        //}
                        //wItem.LastName = wi.lastName;
                        //wItem.Nickname = wi.nickName;
                        //wItem.RoutingNumber = "";
                        //wItem.SecurityCode = "";
                        //wItem.ZipCode = "";
                        //if (walletItems == null)
                        //{
                        //    walletItems = new List<WalletItem>();
                        //}
                        //walletItems.Add(wItem);
                        //updated 7/29/2020 to use new method
                        WalletItem wItem = WalletItemForFiservWalletItem(wi);
                        if (wItem != null)
                        {
                            if (walletItems == null)
                            {
                                walletItems = new List<WalletItem>();
                            }
                            walletItems.Add(wItem);
                        }
                    }
                }
            }

            return walletItems;
        }
        //added 5/4/2023
        public List<IFM_FiservDatabaseObjects.FiservWalletItem> FiservWalletItemsForWalletItems(List<WalletItem> wis)
        {
            List<IFM_FiservDatabaseObjects.FiservWalletItem> walletItems = null;

            if (wis != null && wis.Count > 0)
            {
                foreach (WalletItem wi in wis)
                {
                    if (wi != null)
                    {
                        IFM_FiservDatabaseObjects.FiservWalletItem wItem = FiservWalletItemForWalletItem(wi);
                        if (wItem != null)
                        {
                            if (walletItems == null)
                            {
                                walletItems = new List<IFM_FiservDatabaseObjects.FiservWalletItem>();
                            }
                            walletItems.Add(wItem);
                        }
                    }
                }
            }

            return walletItems;
        }


        public List<WalletItem> WalletItemsForUserIds(List<string> userIds)
        {
            List<WalletItem> walletItems = null;

            if (userIds != null && userIds.Count > 0)
            {
                bool attemptedLookup = false;
                bool caughtDbError = false;
                string dbErrorMsg = "";
                List<IFM_FiservDatabaseObjects.FiservWalletItem> wis = creditCardMethods.Database_FiservWalletItems(active: CommonHelperClass.YesNoOrMaybe.Yes, userIds: userIds, attemptedLookup: ref attemptedLookup, caughtDatabaseError: ref caughtDbError, databaseErrorMessage: ref dbErrorMsg);
                walletItems = WalletItemsForFiservWalletItems(wis);
            }

            return walletItems;
        }


        public List<Wallet> WalletsForFiservWallets(List<IFM_FiservDatabaseObjects.FiservWallet> ws)
        {
            List<Wallet> wallets = null;

            if (ws != null && ws.Count > 0)
            {
                foreach (IFM_FiservDatabaseObjects.FiservWallet w in ws)
                {
                    if (w != null)
                    {
                        Wallet wall = new Wallet();
                        wall.ConsumerEmail = w.consumerEmail;
                        wall.ConsumerName = w.consumerName;
                        wall.ConsumerPhone = w.consumerPhone;
                        wall.FiservWalletId = w.fiservWalletId;
                        wall.Items = WalletItemsForFiservWalletItems(w.walletItems);
                        if (w.fs != null)
                        {
                            wall.KeyIdentifier = w.fs.userId;
                        }
                        else
                        {
                            wall.KeyIdentifier = "";
                        }
                        if (wallets == null)
                        {
                            wallets = new List<Wallet>();
                        }
                        wallets.Add(wall);
                    }
                }
            }

            return wallets;
        }
        //added 5/4/2023
        public List<IFM_FiservDatabaseObjects.FiservWallet> FiservWalletsForWallets(List<Wallet> ws)
        {
            List<IFM_FiservDatabaseObjects.FiservWallet> wallets = null;

            if (ws != null && ws.Count > 0)
            {
                foreach (Wallet w in ws)
                {
                    if (w != null)
                    {
                        IFM_FiservDatabaseObjects.FiservWallet wall = new IFM_FiservDatabaseObjects.FiservWallet();
                        wall.consumerEmail = w.ConsumerEmail;
                        wall.consumerName = w.ConsumerName;
                        wall.consumerPhone = w.ConsumerPhone;
                        wall.fiservWalletId = w.FiservWalletId;
                        wall.walletItems = FiservWalletItemsForWalletItems(w.Items);
                        if (string.IsNullOrWhiteSpace(w.KeyIdentifier) == false)
                        {
                            if (wall.fs == null)
                            {
                                wall.fs = new IFM_FiservDatabaseObjects.FiservSession();
                            }
                            wall.fs.userId = w.KeyIdentifier;
                        }
                        if (wallets == null)
                        {
                            wallets = new List<IFM_FiservDatabaseObjects.FiservWallet>();
                        }
                        wallets.Add(wall);
                    }
                }
            }

            return wallets;
        }


        public List<Wallet> WalletsForUserIds(List<string> userIds)
        {
            List<Wallet> wallets = null;

            if (userIds != null && userIds.Count > 0)
            {
                //bool attemptedLookup = false;
                //bool caughtDbError = false;
                //string dbErrorMsg = "";
                ////List<IFM_FiservDatabaseObjects.FiservWallet> ws = creditCardMethods.Database_FiservWallets(userIds: userIds, attemptedLookup: ref attemptedLookup, caughtDatabaseError: ref caughtDbError, databaseErrorMessage: ref dbErrorMsg, includeWalletItems: true, activeFlagForWalletItems: CommonHelperClass.YesNoOrMaybe.Yes);
                ////updated 7/19/2020 to use new method; note: this will only return results where there are active walletItems, so if there aren't any, you won't even get the wallet info back
                //List<IFM_FiservDatabaseObjects.FiservWallet> ws = creditCardMethods.Database_FiservWallets_ForWalletItems(active: CommonHelperClass.YesNoOrMaybe.Yes, userIdsOrActiveRccOrScheduledPaymentPolNums: userIds, attemptedLookup: ref attemptedLookup, caughtDatabaseError: ref caughtDbError, databaseErrorMessage: ref dbErrorMsg);
                //updated 5/4/2023
                List<IFM_FiservDatabaseObjects.FiservWallet> ws = FiservWalletsForUserIds(userIds);
                wallets = WalletsForFiservWallets(ws);
            }

            return wallets;
        }

        //added 5/4/2023
        public List<IFM_FiservDatabaseObjects.FiservWallet> FiservWalletsForUserIds(List<string> userIds)
        {
            List<IFM_FiservDatabaseObjects.FiservWallet> ws = null;

            if (userIds != null && userIds.Count > 0)
            {
                bool attemptedLookup = false;
                bool caughtDbError = false;
                string dbErrorMsg = "";
                ws = creditCardMethods.Database_FiservWallets_ForWalletItems(active: CommonHelperClass.YesNoOrMaybe.Yes, userIdsOrActiveRccOrScheduledPaymentPolNums: userIds, attemptedLookup: ref attemptedLookup, caughtDatabaseError: ref caughtDbError, databaseErrorMessage: ref dbErrorMsg);
            }

            return ws;
        }


        //added 6/29/2020
        //public FiservAddWalletItemServiceResult AddWalletItem(CommonObjects.Fiserv.WalletItem item)
        //{
        //    //walletItemInfo
        //    FiservAddWalletItemServiceResult sr = new FiservAddWalletItemServiceResult();
        //    if (item != null)
        //    {
        //        cc.Fiserv_InsertWalletItemBodyRequest insertWalletItemBodyRequest = new cc.Fiserv_InsertWalletItemBodyRequest();
        //        string authToken = "";
        //        string sessionToken = "";
        //        int sessionId = 0;
        //        cc.UserInfo user = new cc.UserInfo();
        //        string fundingAccountToken = "";
        //        int fiservWalletId = 0;
        //        int fiservWalletItemId = 0;
        //        cc.FiservErrorObject errorObject = null;
        //        bool success = creditCardMethods.SuccessfullyCreatedFiservWalletItem(insertWalletItemBodyRequest, memberIdentifier: item.KeyIdentifier, authToken: ref authToken, sessionToken: ref sessionToken, sessionId: ref sessionId, pmtInterface: cc.Enums.PaymentInterface.MemberPortalSite, user: user, fundingAccountToken: ref fundingAccountToken, fiservWalletId: ref fiservWalletId, fiservWalletItemId: ref fiservWalletItemId, errorObject: ref errorObject);
        //    }

        //    return sr;
        //}
        public APIResponses.Common.ServiceResult<APIResponses.Fiserv.FiservAddWalletItemResult> AddWalletItem(CommonObjects.Fiserv.AddWalletItemBody addBody)
        {
            //walletItemInfo
            var sr = new APIResponses.Common.ServiceResult<APIResponses.Fiserv.FiservAddWalletItemResult>();
            if (addBody != null)
            {
                if (addBody.Item != null)
                {
                    string fundingAccountToken = addBody.Fiserv_FundingAccountToken;
                    if (string.IsNullOrWhiteSpace(addBody.Item.FundingAccountToken))
                    {
                        addBody.Item.FundingAccountToken = fundingAccountToken;
                    }
                    else if (string.IsNullOrWhiteSpace(fundingAccountToken))
                    {
                        fundingAccountToken = addBody.Item.FundingAccountToken;
                    }

                    bool hasFundingAcctToken = !string.IsNullOrWhiteSpace(addBody.Item.FundingAccountToken);
                    bool hasCardInfo = !(string.IsNullOrWhiteSpace(addBody.Item.CreditCardNumber) || string.IsNullOrWhiteSpace(addBody.Item.ExpirationMonth) || string.IsNullOrWhiteSpace(addBody.Item.ExpirationYear));
                    bool hasBankInfo = !(string.IsNullOrWhiteSpace(addBody.Item.RoutingNumber) || string.IsNullOrWhiteSpace(addBody.Item.CheckAccountNumber));
                    if (hasFundingAcctToken || hasCardInfo || hasBankInfo)
                    {
                        cc.Fiserv_InsertWalletItemBodyRequest insertWalletItemBodyRequest = new cc.Fiserv_InsertWalletItemBodyRequest();
                        //insertWalletItemBodyRequest.nickName = addBody.Item.Nickname;
                        //updated 8/18/2020 to handle for max length
                        insertWalletItemBodyRequest.nickName = IFM_CreditCardProcessing.Common.Fiserv_Formatted_WalletItem_Nickname(addBody.Item.Nickname);
                        insertWalletItemBodyRequest.emailAddress = addBody.Item.EmailAddress; //added 11/12/2020
                        if (hasCardInfo)
                        {
                            insertWalletItemBodyRequest.cardDetail = new cc.Fiserv_CardDetail();
                            insertWalletItemBodyRequest.cardDetail.cardNumber = addBody.Item.CreditCardNumber;
                            CommonHelperClass chc = new CommonHelperClass();
                            insertWalletItemBodyRequest.cardDetail.expirationDate = cc.Common.FiservExpirationDateString(chc.IntegerForString(addBody.Item.ExpirationMonth), chc.IntegerForString(addBody.Item.ExpirationYear));
                            insertWalletItemBodyRequest.cardDetail.firstName = addBody.Item.FirstName;
                            insertWalletItemBodyRequest.cardDetail.lastName = addBody.Item.LastName;
                            insertWalletItemBodyRequest.cardDetail.nameOnCard = cc.Common.Append(addBody.Item.FirstName, addBody.Item.LastName, appendText: " ");
                            insertWalletItemBodyRequest.cardDetail.securityCode = addBody.Item.SecurityCode;
                            insertWalletItemBodyRequest.cardDetail.zipCode = addBody.Item.ZipCode;
                        }
                        else if (hasBankInfo)
                        {
                            insertWalletItemBodyRequest.checkDetail = new cc.Fiserv_CheckDetail();
                            insertWalletItemBodyRequest.checkDetail.checkAccountNumber = addBody.Item.CheckAccountNumber;
                            if (string.IsNullOrWhiteSpace(addBody.Item.FundingAccountType) == false && addBody.Item.FundingAccountType == "Business")
                            {
                                insertWalletItemBodyRequest.checkDetail.checkAccountType_Enum = cc.Enums.Fiserv_CheckAccountType.Business;
                            }
                            else
                            {
                                insertWalletItemBodyRequest.checkDetail.checkAccountType_Enum = cc.Enums.Fiserv_CheckAccountType.Personal;
                            }
                            insertWalletItemBodyRequest.checkDetail.firstName = addBody.Item.FirstName;
                            if (string.IsNullOrWhiteSpace(addBody.Item.BankAccountType) == false && addBody.Item.BankAccountType == "Saving")
                            {
                                insertWalletItemBodyRequest.checkDetail.fundingAccountType_Enum = cc.Enums.Fiserv_FundingAccountType.Saving;
                            }
                            else
                            {
                                insertWalletItemBodyRequest.checkDetail.fundingAccountType_Enum = cc.Enums.Fiserv_FundingAccountType.Checking;
                            }
                            insertWalletItemBodyRequest.checkDetail.lastName = addBody.Item.LastName;
                            insertWalletItemBodyRequest.checkDetail.routingNumber = addBody.Item.RoutingNumber;
                        }
                        else
                        {
                            insertWalletItemBodyRequest.fundingAccountToken = fundingAccountToken;
                        }

                        string authToken = addBody.Fiserv_AuthToken;
                        string sessionToken = addBody.Fiserv_SessionToken;
                        int sessionId = addBody.Fiserv_SessionId;
                        cc.Enums.PaymentInterface pmtInterface = cc.Enums.PaymentInterface.MemberPortalSite;
                        if (addBody.IsFromOneView == true)
                        {
                            pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.OneView;
                        }
                        cc.UserInfo user = new cc.UserInfo();
                        user.Username = addBody.UserName;
                        user.UserType = addBody.UserType;
                        int fiservWalletId = addBody.Item.FiservWalletId;
                        int fiservWalletItemId = 0;
                        string fundingMethod = addBody.Item.FundingMethod;
                        string fundingAccountLastFourDigit = addBody.Item.FundingAccountLastFourDigits;
                        cc.FiservErrorObject errorObject = null;
                        string responseMessage = "";
                        bool fiservWalletJustCreated = false;
                        //bool success = creditCardMethods.SuccessfullyCreatedFiservWalletItem(insertWalletItemBodyRequest, memberIdentifier: addBody.Item.KeyIdentifier, authToken: ref authToken, sessionToken: ref sessionToken, sessionId: ref sessionId, pmtInterface: pmtInterface, user: user, fundingAccountToken: ref fundingAccountToken, fiservWalletId: ref fiservWalletId, fiservWalletItemId: ref fiservWalletItemId, errorObject: ref errorObject, responseMessage: ref responseMessage);
                        //updated 7/1/2020 to adjust MemberIdentifer (EmailAddress)

                        sr.ResponseData.Success = creditCardMethods.SuccessfullyCreatedFiservWalletItem(insertWalletItemBodyRequest, memberIdentifier: cc.Common.AdjustedEmailAddressForFiservApiCall(addBody.Item.KeyIdentifier), authToken: ref authToken, sessionToken: ref sessionToken, sessionId: ref sessionId, pmtInterface: pmtInterface, user: user, fundingAccountToken: ref fundingAccountToken, fiservWalletId: ref fiservWalletId, fiservWalletItemId: ref fiservWalletItemId, fundingMethod: ref fundingMethod, fundingAccountLastFourDigit: ref fundingAccountLastFourDigit, fiservWalletJustCreated: ref fiservWalletJustCreated, errorObject: ref errorObject, responseMessage: ref responseMessage);

                        if (sr.ResponseData.Success == true)
                        {
                            sr.ResponseData.FiservWalletItemId = fiservWalletItemId;
                            sr.ResponseData.FundingAccountToken = fundingAccountToken;
                            sr.ResponseData.FiservWalletId = fiservWalletId;
                            sr.ResponseData.FundingMethod = fundingMethod;
                            sr.ResponseData.FundingAccountLastFourDigit = fundingAccountLastFourDigit;
                        }
                        else
                        {
                            string errorMessage = "problem adding wallet item";
                            //use something specific if possible; use new byref param responseMessage
                            if (string.IsNullOrWhiteSpace(responseMessage) == false)
                            {
                                errorMessage = errorMessage + ": " + responseMessage;
                            }
                            else if (errorObject != null && string.IsNullOrWhiteSpace(errorObject.ErrorMessage) == false)
                            {
                                errorMessage = errorObject.ErrorMessage;
                            }
                            sr.Messages.CreateErrorMessage(errorMessage);
                        }
                    }
                    else
                    {
                        sr.Messages.CreateErrorMessage("Funding Account Token or Complete Card/Bank Information required.");
                    }
                }
                else
                {
                    sr.Messages.CreateErrorMessage("No wallet item information provided.");
                }
            }

            return sr;
        }



        public void PayWithWalletItem(APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr, global::IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentInfo)
        {
            //paymentInfo
            if (paymentInfo != null && paymentInfo.WalletItemInfo != null)
            {
                string fundingAccountToken = paymentInfo.FiservProperties.FundingAccountToken;
                if (string.IsNullOrWhiteSpace(paymentInfo.WalletItemInfo.FundingAccountToken))
                {
                    paymentInfo.WalletItemInfo.FundingAccountToken = fundingAccountToken;
                }
                else if (string.IsNullOrWhiteSpace(fundingAccountToken))
                {
                    fundingAccountToken = paymentInfo.WalletItemInfo.FundingAccountToken;
                }

                cc.UserInfo user = new cc.UserInfo();
                user.Username = paymentInfo.Username;
                user.UserType = (cc.Enums.UserType)paymentInfo.UserType;
                string confirmationNumber = "";
                int fiservPaymentId = 0;
                cc.FiservErrorObject errorObject = null;
                string responseMessage = "";

                //CommonHelperClass chc = new CommonHelperClass();
                //bool success = creditCardMethods.SuccessfullyProcessedFiservWalletPayment(paymentInfo.WalletItemInfo.FundingAccountToken, paymentInfo.PolicyNumber, Convert.ToDecimal(paymentInfo.PaymentAmount), policyId: paymentInfo.PolicyId, fiservWalletItemId: fiservWalletItemId, walletItemSessionUserId: paymentInfo.WalletItemInfo.KeyIdentifier, pmtType: ref pmtType, fundingMethod: ref fundingMethod, ccType: ref ccType, pmtInterface: pmtInterface, user: user, emailAddress:paymentInfo.EmailAddress, insertTransactionRecordForPostToDiamond:true, sendConfirmationEmailOnSuccess:true, confirmationNumber: ref confirmationNumber, paymentResponseMessage: ref responseMessage, fiservPaymentId: ref fiservPaymentId, errorObject: ref errorObject);
                //updated 7/1/2020 to adjust MemberIdentifer (EmailAddress)
                //bool success = creditCardMethods.SuccessfullyProcessedFiservWalletPayment(paymentInfo.WalletItemInfo.FundingAccountToken, paymentInfo.PolicyNumber, Convert.ToDecimal(paymentInfo.PaymentAmount), policyId: paymentInfo.PolicyId, fiservWalletItemId: fiservWalletItemId, walletItemSessionUserId: cc.Common.AdjustedEmailAddressForFiservApiCall(paymentInfo.WalletItemInfo.KeyIdentifier), pmtType: ref pmtType, fundingMethod: ref fundingMethod, ccType: ref ccType, pmtInterface: pmtInterface, user: user, emailAddress:paymentInfo.EmailAddress, insertTransactionRecordForPostToDiamond:true, sendConfirmationEmailOnSuccess:true, confirmationNumber: ref confirmationNumber, paymentResponseMessage: ref responseMessage, fiservPaymentId: ref fiservPaymentId, errorObject: ref errorObject);
                //updated 7/11/2020 for additional database fields; note: sendConfirmationEmailOnSuccess now only controls the IFM email (will still only send if Sender key not set to Vendor only; could update to just send False if we check the key first and verify that the IFM email should not be sent - done); Fiserv email will be sent as long as Sender key not set to IndianaFarmers only

                bool sendConfirmationEmailOnSuccess = true;
                if (cc.Common.PaymentConfirmationEmail_Sender() == cc.Enums.PaymentConfirmationEmailSender.Vendor)
                {
                    sendConfirmationEmailOnSuccess = false;
                }

                //bool success = creditCardMethods.SuccessfullyProcessedFiservWalletPayment(paymentInfo.WalletItemInfo.FundingAccountToken, paymentInfo.PolicyNumber, Convert.ToDecimal(paymentInfo.PaymentAmount), policyId: paymentInfo.PolicyId, fiservWalletItemId: fiservWalletItemId, walletItemSessionUserId: cc.Common.AdjustedEmailAddressForFiservApiCall(paymentInfo.WalletItemInfo.KeyIdentifier), pmtType: ref pmtType, fundingMethod: ref fundingMethod, ccType: ref ccType, pmtInterface: pmtInterface, user: user, emailAddress: paymentInfo.EmailAddress, insertMainTransactionRecords: true, insertSecondaryTransactionRecords: true, insertTransactionRecordForPostToDiamond: true, sendConfirmationEmailOnSuccess: sendConfirmationEmailOnSuccess, confirmationNumber: ref confirmationNumber, paymentResponseMessage: ref responseMessage, fiservPaymentId: ref fiservPaymentId, errorObject: ref errorObject);

                var walletPaymentProcessor = new IFM_CreditCardProcessing.Fiserv_WalletPaymentProcessor();
                walletPaymentProcessor.FundingAccountToken = paymentInfo.WalletItemInfo.FundingAccountToken;
                walletPaymentProcessor.PolicyNumber = paymentInfo.PolicyNumber;
                walletPaymentProcessor.AccountBillNumber = paymentInfo.AccountBillNumber;
                walletPaymentProcessor.Amount = paymentInfo.PaymentAmount.TryToGetDecimal();
                walletPaymentProcessor.PolicyId = paymentInfo.PolicyId;
                walletPaymentProcessor.FiservWalletItemId = paymentInfo.WalletItemInfo.WalletItemId;
                walletPaymentProcessor.WalletItemSessionUserId = cc.Common.AdjustedEmailAddressForFiservApiCall(paymentInfo.WalletItemInfo.KeyIdentifier);
                walletPaymentProcessor.PaymentType = cc.Enums.PaymentType.None;
                walletPaymentProcessor.FundingMethod = paymentInfo.WalletItemInfo.FundingMethod;
                walletPaymentProcessor.CCType = cc.Enums.CreditCardType.None;
                walletPaymentProcessor.PaymentInterface = (IFM_CreditCardProcessing.Enums.PaymentInterface)paymentInfo.PaymentInterface;
                walletPaymentProcessor.User = user;
                walletPaymentProcessor.EmailAddress = paymentInfo.EmailAddress;
                walletPaymentProcessor.InsertMainTransactionRecords = true;
                walletPaymentProcessor.InsertSecondaryTransactionRecords = true;
                walletPaymentProcessor.SendConfirmationEmailOnSuccess = sendConfirmationEmailOnSuccess;
                walletPaymentProcessor.feeAmount = paymentInfo.FeeAmount; //added 9/25/2024
                if (IFM.DataServicesCore.BusinessLogic.Payments.ProcessPayment.IsAfterHoursPayment())
                {
                    //Insert Record into Diamond Agent Payment table with swept = N so the PaymentSweeper app will process the record and update Diamond.
                    walletPaymentProcessor.InsertTransactionRecordForPostToDiamond = true;
                }
                else
                {
                    //Don't insert record as we are going to attempt to Automatically Post the payment to Diamond and then we will manually update the Diamond Agent Payment table.
                    walletPaymentProcessor.InsertTransactionRecordForPostToDiamond = false;
                }

                sr.ResponseData.PaymentCompleted = walletPaymentProcessor.ProcessWalletPayment(ref confirmationNumber, ref responseMessage, ref fiservPaymentId, ref errorObject);

                if (sr.ResponseData.PaymentCompleted == true)
                {
                    //Payment to Vendor was successful
                    sr.ResponseData.PaymentConfirmationNumber = confirmationNumber;
                    if (walletPaymentProcessor.InsertTransactionRecordForPostToDiamond == false)
                    {
                        if (walletPaymentProcessor.FundingMethod != "ACH")
                        {
                            ProcessWalletCCPayment(paymentInfo, walletPaymentProcessor, sr);
                        }
                        else
                        {
                            ProcessWalletEcheckPayment(paymentInfo, walletPaymentProcessor, sr);
                        }
                    }
                }
                else
                {
                    SetWalletError("Problem paying with wallet item", responseMessage, errorObject, sr.Messages, sr.DetailedErrorMessages);
                }
            }
        }

        private void ProcessWalletCCPayment(global::IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentInfo, IFM_CreditCardProcessing.Fiserv_WalletPaymentProcessor walletPaymentProcessor, APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr)
        {
            //Automatic Payment Post - Lets insert the info into Diamond using the ApplyCash service.
            Payments.ApplyCash applyDiamondCash = new Payments.ApplyCash(walletPaymentProcessor);
            applyDiamondCash.PolicyImageNum = paymentInfo.PolicyImageNumber;
            bool appliedCashSuccessfully = applyDiamondCash.DoApplyCash();
            if (appliedCashSuccessfully)
            {
                //Lets insert a record of our Diamond Post transaction into DiamondAgentPayment DB - Marking Swept as Y to show it was processed automatically.
                IFM_CreditCardProcessing.DiamondAgentPaymentDBRecord DiamondPaymentRecord = new DiamondAgentPaymentDBRecord(walletPaymentProcessor);
                DiamondPaymentRecord.Swept = "Y";
                DiamondPaymentRecord.InsertRecord();
            }
            else
            {
                SetWalletError("Problem making wallet payment", applyDiamondCash.ErrorMessage, sr.Messages, sr.DetailedErrorMessages);
                SetWalletError("Problem making wallet payment", applyDiamondCash.ErrorMessage, sr.Messages, sr.DetailedErrorMessages);
            }
        }

        private void ProcessWalletEcheckPayment(global::IFM.DataServices.API.RequestObjects.Payments.PaymentData paymentInfo, IFM_CreditCardProcessing.Fiserv_WalletPaymentProcessor walletPaymentProcessor, APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult> sr)
        {
            Payments.eCheck.EcheckProcessor echeckProcessor = new Payments.eCheck.EcheckProcessor(walletPaymentProcessor);
            echeckProcessor.EcheckRequest.PolicyImageNum = paymentInfo.PolicyImageNumber;
            echeckProcessor.ProcessPostVendorEcheckPayment();
            if (echeckProcessor.EcheckResponse.Completed == false)
            {
                SetWalletError("Problem making wallet payment", echeckProcessor.EcheckResponse.ErrorMsgs.ListToCSV(), sr.Messages, sr.DetailedErrorMessages);
            }
        }

        //added 6/30/2020
        public APIResponses.Common.ServiceResult<APIResponses.Fiserv.FiservUpdateWalletItemResult> UpdateWalletItem(CommonObjects.Fiserv.UpdateWalletItemBody updateBody)
        {
            //walletItemInfo
            var sr = new APIResponses.Common.ServiceResult<APIResponses.Fiserv.FiservUpdateWalletItemResult>();
            if (updateBody != null)
            {
                if (updateBody.Item != null)
                {
                    string fundingAccountToken = updateBody.Item.FundingAccountToken;
                    if (string.IsNullOrWhiteSpace(fundingAccountToken) == false)
                    {
                        cc.Fiserv_UpdateWalletItemBodyRequest updateWalletItemBodyRequest = new cc.Fiserv_UpdateWalletItemBodyRequest();
                        updateWalletItemBodyRequest.fundingAccountToken = fundingAccountToken;
                        if (string.IsNullOrWhiteSpace(updateBody.Item.FundingCategory) == false)
                        {
                            updateWalletItemBodyRequest.fundingCategory = updateBody.Item.FundingCategory; //shouldn't change
                        }
                        else
                        {
                            updateWalletItemBodyRequest.fundingCategory = null;
                        }
                        updateWalletItemBodyRequest.isDefaultFundingSource = false; //may need to add property so it doesn't get overwritten; may not matter as Fiserv may leave unchanged if nothing else in wallet is set as default
                        if (string.IsNullOrWhiteSpace(updateBody.Item.Nickname) == false)
                        {
                            //updateWalletItemBodyRequest.nickName = updateBody.Item.Nickname;
                            //updated 8/18/2020 to handle for max length - Max Length = 30 characters
                            updateWalletItemBodyRequest.nickName = updateBody.Item.Nickname.TruncateString(30);
                        }
                        else
                        {
                            updateWalletItemBodyRequest.nickName = null;
                        }
                        if (string.IsNullOrWhiteSpace(updateBody.Item.FundingAccountType) == false)
                        {
                            if (updateBody.Item.FundingAccountType == "Business")
                            {
                                updateWalletItemBodyRequest.updatedCheckType_Enum = cc.Enums.Fiserv_CheckAccountType.Business;
                            }
                            else
                            {
                                updateWalletItemBodyRequest.updatedCheckType_Enum = cc.Enums.Fiserv_CheckAccountType.Personal;
                            }
                        }
                        else
                        {
                            updateWalletItemBodyRequest.updatedCheckType = null;
                        }
                        if (string.IsNullOrWhiteSpace(updateBody.Item.ExpirationMonth) == false || string.IsNullOrWhiteSpace(updateBody.Item.ExpirationYear) == false)
                        {
                            updateWalletItemBodyRequest.updatedExpirationDate = cc.Common.FiservExpirationDateString(updateBody.Item.ExpirationMonth.TryToGetInt32(), updateBody.Item.ExpirationYear.TryToGetInt32());
                        }
                        else
                        {
                            updateWalletItemBodyRequest.updatedExpirationDate = null;
                        }
                        if (string.IsNullOrWhiteSpace(updateBody.Item.FirstName) == false)
                        {
                            updateWalletItemBodyRequest.updatedFirstName = updateBody.Item.FirstName;
                        }
                        else
                        {
                            updateWalletItemBodyRequest.updatedFirstName = null;
                        }
                        if (string.IsNullOrWhiteSpace(updateBody.Item.BankAccountType) == false)
                        {
                            if (updateBody.Item.BankAccountType == "Saving")
                            {
                                updateWalletItemBodyRequest.updatedFundingAccountType_Enum = cc.Enums.Fiserv_FundingAccountType.Saving;
                            }
                            else
                            {
                                updateWalletItemBodyRequest.updatedFundingAccountType_Enum = cc.Enums.Fiserv_FundingAccountType.Checking;
                            }
                        }
                        else
                        {
                            updateWalletItemBodyRequest.updatedFundingAccountType = null;
                        }
                        if (string.IsNullOrWhiteSpace(updateBody.Item.LastName) == false)
                        {
                            updateWalletItemBodyRequest.updatedLastName = updateBody.Item.LastName;
                        }
                        else
                        {
                            updateWalletItemBodyRequest.updatedLastName = null;
                        }
                        if (string.IsNullOrWhiteSpace(updateBody.Item.ZipCode) == false)
                        {
                            //updateWalletItemBodyRequest.updatedZipcode = updateBody.Item.ZipCode;
                            //updated 7/31/2020 for Fiserv format
                            updateWalletItemBodyRequest.updatedZipcode = IFM_CreditCardProcessing.Common.Fiserv_Formatted_Zip(updateBody.Item.ZipCode);
                        }
                        else
                        {
                            updateWalletItemBodyRequest.updatedZipcode = null;
                        }

                        cc.Enums.PaymentInterface pmtInterface = cc.Enums.PaymentInterface.MemberPortalSite;
                        if (updateBody.IsFromOneView == true)
                        {
                            pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.OneView;
                        }
                        cc.UserInfo user = new cc.UserInfo();
                        user.Username = updateBody.UserName;
                        user.UserType = updateBody.UserType;
                        int fiservWalletItemId = updateBody.Item.FiservWalletItemId;
                        cc.FiservErrorObject errorObject = null;
                        string responseMessage = "";
                        bool fiservWalletJustCreated = false;
                        string authToken = "";
                        string sessionToken = "";
                        int sessionId = 0;
                        //bool success = creditCardMethods.SuccessfullyUpdatedFiservWalletItem(updateWalletItemBodyRequest, ref fundingAccountToken, ref fiservWalletItemId, walletItemSessionUserId: updateBody.Item.KeyIdentifier, pmtInterface: pmtInterface, user: user, errorObject: ref errorObject, responseMessage: ref responseMessage);
                        //updated 7/1/2020 to adjust MemberIdentifer (EmailAddress)
                        
                        sr.ResponseData.Success = creditCardMethods.SuccessfullyUpdatedFiservWalletItem(updateWalletItemBodyRequest, ref fundingAccountToken, ref fiservWalletItemId, walletItemSessionUserId: cc.Common.AdjustedEmailAddressForFiservApiCall(updateBody.Item.KeyIdentifier), authToken: ref authToken, sessionToken: ref sessionToken, sessionId: ref sessionId, pmtInterface: pmtInterface, user: user, fiservWalletJustCreated: ref fiservWalletJustCreated, errorObject: ref errorObject, responseMessage: ref responseMessage);
                        
                        if (sr.ResponseData.Success == false)
                        {
                            SetWalletError("Problem updating wallet item", responseMessage, errorObject, sr.Messages, sr.DetailedErrorMessages);
                        }
                    }
                    else
                    {
                        sr.Messages.CreateErrorMessage("Funding Account Token required.");
                    }
                }
                else
                {
                    sr.Messages.CreateErrorMessage("No wallet item information provided.");
                }
            }

            return sr;
        }



        public APIResponses.Common.ServiceResult<APIResponses.Fiserv.FiservDeleteWalletItemResult> DeleteWalletItem(CommonObjects.Fiserv.DeleteWalletItemBody deleteBody)
        {
            //walletItemInfo
            var sr = new APIResponses.Common.ServiceResult<APIResponses.Fiserv.FiservDeleteWalletItemResult>();
            if (deleteBody != null)
            {
                if (deleteBody.Item != null)
                {
                    string fundingAccountToken = deleteBody.Item.FundingAccountToken;
                    if (string.IsNullOrWhiteSpace(fundingAccountToken) == false)
                    {
                        cc.Enums.PaymentInterface pmtInterface = cc.Enums.PaymentInterface.MemberPortalSite;
                        if (deleteBody.IsFromOneView == true)
                        {
                            pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.OneView;
                        }
                        cc.UserInfo user = new cc.UserInfo();
                        user.Username = deleteBody.UserName;
                        user.UserType = deleteBody.UserType;
                        int fiservWalletItemId = deleteBody.Item.FiservWalletItemId;
                        cc.FiservErrorObject errorObject = null;
                        string responseMessage = "";
                        //bool success = creditCardMethods.SuccessfullyDeletedFiservWalletItem(ref fundingAccountToken, ref fiservWalletItemId, walletItemSessionUserId: deleteBody.Item.KeyIdentifier, pmtInterface: pmtInterface, user: user, errorObject: ref errorObject, responseMessage: ref responseMessage);
                        //updated 7/1/2020 to adjust MemberIdentifer (EmailAddress)
                        
                        sr.ResponseData.Success = creditCardMethods.SuccessfullyDeletedFiservWalletItem(ref fundingAccountToken, ref fiservWalletItemId, walletItemSessionUserId: cc.Common.AdjustedEmailAddressForFiservApiCall(deleteBody.Item.KeyIdentifier), pmtInterface: pmtInterface, user: user, errorObject: ref errorObject, responseMessage: ref responseMessage);

                        if (sr.ResponseData.Success == false)
                        {
                            SetWalletError("Problem deleting wallet item", responseMessage, errorObject, sr.Messages, sr.DetailedErrorMessages);
                        }
                    }
                    else
                    {
                        sr.Messages.CreateErrorMessage("Funding Account Token required.");
                    }
                }
                else
                {
                    sr.Messages.CreateErrorMessage("No wallet item information provided.");
                }
            }

            return sr;
        }


        //added 7/29/2020
        public WalletItem WalletItemForWalletItemId(int walletItemId)
        {
            WalletItem wItem = null;

            if (walletItemId > 0)
            {
                //bool attemptedLookup = false;
                //bool caughtDbError = false;
                //string dbErrorMsg = "";
                //IFM_FiservDatabaseObjects.FiservWalletItem wi = creditCardMethods.Database_FiservWalletItem(fiservWalletItemId: walletItemId, active: CommonHelperClass.YesNoOrMaybe.Yes, attemptedLookup: ref attemptedLookup, caughtDatabaseError: ref caughtDbError, databaseErrorMessage: ref dbErrorMsg);
                //updated 7/30/2020 to use new method
                IFM_FiservDatabaseObjects.FiservWalletItem wi = FiservWalletItemForWalletItemId(walletItemId);
                wItem = WalletItemForFiservWalletItem(wi);
            }

            return wItem;
        }

        public WalletItem WalletItemForFiservWalletItem(IFM_FiservDatabaseObjects.FiservWalletItem wi)
        {
            WalletItem wItem = null;

            if (wi != null)
            {
                wItem = new WalletItem();
                wItem.BankAccountType = wi.bankAccountType;
                wItem.CheckAccountNumber = "";
                wItem.CreditCardNumber = "";
                wItem.FirstName = wi.firstName;
                wItem.ExpirationMonth = "";
                wItem.ExpirationYear = "";
                wItem.FiservWalletId = wi.fiservWalletId;
                wItem.FiservWalletItemId = wi.fiservWalletItemId;
                wItem.FundingAccountLastFourDigits = wi.fundingAccountLastFourDigit;
                wItem.FundingAccountToken = wi.fundingAccountToken;
                wItem.FundingAccountType = wi.fundingAccountType;
                wItem.FundingCategory = wi.fundingCategory;
                wItem.FundingMethod = wi.fundingMethod;
                if (wi.fs != null)
                {
                    wItem.KeyIdentifier = wi.fs.userId;
                }
                else
                {
                    wItem.KeyIdentifier = "";
                }
                wItem.LastName = wi.lastName;
                wItem.Nickname = wi.nickName;
                wItem.RoutingNumber = "";
                wItem.SecurityCode = "";
                wItem.ZipCode = "";

                //added 11/10/2020
                wItem.ActiveRccPolicies = wi.activeRccPolicies;
                wItem.ActiveAndUnprocessedScheduledPaymentPolicies = wi.activeAndUnProcessedScheduledPaymentPolicies;

                //added 5/4/2022
                wItem.EncryptedRoutingNumber = wi.encryptedBankRoutingNumber;
                wItem.EncryptedCheckAccountNumber = wi.encryptedBankAccountNumber;
            }

            return wItem;
        }
        //added 5/4/2023
        public IFM_FiservDatabaseObjects.FiservWalletItem FiservWalletItemForWalletItem(WalletItem wi)
        {
            IFM_FiservDatabaseObjects.FiservWalletItem wItem = null;

            if (wi != null)
            {
                wItem = new IFM_FiservDatabaseObjects.FiservWalletItem();
                wItem.bankAccountType = wi.BankAccountType;
                wItem.encryptedBankAccountNumber = "";
                //wItem.CreditCardNumber = "";
                wItem.firstName = wi.FirstName;
                //wItem.ExpirationMonth = "";
                //wItem.ExpirationYear = "";
                wItem.fiservWalletId = wi.FiservWalletId;
                wItem.fiservWalletItemId = wi.FiservWalletItemId;
                wItem.fundingAccountLastFourDigit = wi.FundingAccountLastFourDigits;
                wItem.fundingAccountToken = wi.FundingAccountToken;
                wItem.fundingAccountType = wi.FundingAccountType;
                wItem.fundingCategory = wi.FundingCategory;
                wItem.fundingMethod = wi.FundingMethod;
                if (string.IsNullOrWhiteSpace(wi.KeyIdentifier) == false)
                {
                    if (wItem.fs == null)
                    {
                        wItem.fs = new IFM_FiservDatabaseObjects.FiservSession();
                    }
                    wItem.fs.userId = wi.KeyIdentifier;
                }
                wItem.lastName = wi.LastName;
                wItem.nickName = wi.Nickname;
                wItem.encryptedBankRoutingNumber = "";
                //wItem.SecurityCode = "";
                //wItem.ZipCode = "";

                wItem.activeRccPolicies = wi.ActiveRccPolicies;
                wItem.activeAndUnProcessedScheduledPaymentPolicies = wi.ActiveAndUnprocessedScheduledPaymentPolicies;

                wItem.encryptedBankRoutingNumber = wi.EncryptedRoutingNumber;
                wItem.encryptedBankAccountNumber = wi.EncryptedCheckAccountNumber;
            }

            return wItem;
        }

        //added 7/30/2020
        public IFM_FiservDatabaseObjects.FiservWalletItem FiservWalletItemForWalletItemId(int walletItemId)
        {
            IFM_FiservDatabaseObjects.FiservWalletItem wi = null;

            if (walletItemId > 0)
            {
                bool attemptedLookup = false;
                bool caughtDbError = false;
                string dbErrorMsg = "";
                wi = creditCardMethods.Database_FiservWalletItem(fiservWalletItemId: walletItemId, active: CommonHelperClass.YesNoOrMaybe.Yes, attemptedLookup: ref attemptedLookup, caughtDatabaseError: ref caughtDbError, databaseErrorMessage: ref dbErrorMsg);
            }

            return wi;
        }

        //public FiservUpdateWalletServiceResult UpdateWallet(CommonObjects.Fiserv.UpdateWalletBody updateBody)
        //{
        //    //walletInfo
        //    FiservUpdateWalletServiceResult sr = new FiservUpdateWalletServiceResult();
        //    if (updateBody != null)
        //    {
        //        if (updateBody.Item != null)
        //        {
        //            cc.Enums.PaymentInterface pmtInterface = cc.Enums.PaymentInterface.MemberPortalSite;
        //            if (updateBody.IsFromOneView == true)
        //            {
        //                pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.OneView;
        //            }
        //            cc.UserInfo user = new cc.UserInfo();
        //            user.Username = updateBody.UserName;
        //            user.UserType = updateBody.UserType;

        //            bool attemptedServiceCall = false;
        //            string errorMessage = "";
        //            bool success = UpdateWallet(updateBody.Item, user, pmtInterface, ref errorMessage);
        //            if (success == true)
        //            {
        //                //all good
        //                sr.Success = true;
        //            }
        //            else
        //            {
        //                if (string.IsNullOrWhiteSpace(errorMessage) == true)
        //                {
        //                    errorMessage = "problem updating wallet";
        //                }
        //                sr.Messages.CreateErrorMessage(errorMessage);
        //                SetWalletError("Problem updating wallet", responseMessage, errorObject, sr);
        //            }
        //        }
        //        else
        //        {
        //            sr.Messages.CreateErrorMessage("No wallet information provided.");
        //        }
        //    }

        //    return sr;
        //}

        public APIResponses.Common.ServiceResult<APIResponses.Fiserv.FiservUpdateWalletResult> UpdateWallet(CommonObjects.Fiserv.UpdateWalletBody updateBody)
        {
            var sr = new APIResponses.Common.ServiceResult<APIResponses.Fiserv.FiservUpdateWalletResult>();
            if (updateBody != null)
            {
                if (updateBody.Item != null)
                {
                    cc.Enums.PaymentInterface pmtInterface = cc.Enums.PaymentInterface.MemberPortalSite;
                    if (updateBody.IsFromOneView == true)
                    {
                        pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.OneView;
                    }
                    cc.UserInfo user = new cc.UserInfo();
                    user.Username = updateBody.UserName;
                    user.UserType = updateBody.UserType;

                    //walletInfo
                    cc.Fiserv_UpdateWalletBodyRequest updateWalletBodyRequest = new cc.Fiserv_UpdateWalletBodyRequest();
                    updateWalletBodyRequest.walletProfile = new cc.Fiserv_WalletProfile();
                    if (string.IsNullOrWhiteSpace(updateBody.Item.ConsumerEmail) == false)
                    {
                        updateWalletBodyRequest.walletProfile.consumerEmail = new List<string>();
                        updateWalletBodyRequest.walletProfile.consumerEmail.Add(updateBody.Item.ConsumerEmail);
                    }
                    if (string.IsNullOrWhiteSpace(updateBody.Item.ConsumerPhone) == false)
                    {
                        updateWalletBodyRequest.walletProfile.consumerPhone = new List<string>();
                        //updateWalletBodyRequest.walletProfile.consumerPhone.Add(w.ConsumerPhone);
                        //updateWalletBodyRequest.walletProfile.consumerPhone.Add(w.ConsumerPhone.Replace("-", "").Replace("(", "").Replace(")", ""));
                        //updated 7/31/2020 to use helper for Fiserv format
                        updateWalletBodyRequest.walletProfile.consumerPhone.Add(IFM_CreditCardProcessing.Common.Fiserv_Formatted_Phone(updateBody.Item.ConsumerPhone));
                    }
                    if (string.IsNullOrWhiteSpace(updateBody.Item.ConsumerName) == false)
                    {
                        updateWalletBodyRequest.walletProfile.consumerName = updateBody.Item.ConsumerName;
                    }

                    int fiservWalletId = updateBody.Item.FiservWalletId;
                    cc.FiservErrorObject errorObject = null;
                    string responseMessage = "";
                    bool fiservWalletJustCreated = false;
                    string authToken = "";
                    string sessionToken = "";
                    int sessionId = 0;

                    sr.ResponseData.Success = creditCardMethods.SuccessfullyUpdatedFiservWallet(updateWalletBodyRequest, fiservWalletId: ref fiservWalletId, walletSessionUserId: cc.Common.AdjustedEmailAddressForFiservApiCall(updateBody.Item.KeyIdentifier), authToken: ref authToken, sessionToken: ref sessionToken, sessionId: ref sessionId, pmtInterface: pmtInterface, user: user, fiservWalletJustCreated: ref fiservWalletJustCreated, errorObject: ref errorObject, responseMessage: ref responseMessage);

                    if (sr.ResponseData.Success == false)
                    {
                        SetWalletError("Problem updating wallet", responseMessage, errorObject, sr.Messages, sr.DetailedErrorMessages);
                    }
                }
                else
                {
                    sr.Messages.CreateErrorMessage("No wallet information provided.");
                    sr.DetailedErrorMessages.CreateErrorMessage("UpdateBody.Item is null");
                }
            }
            else
            {
                sr.Messages.CreateErrorMessage("No wallet information provided.");
                sr.DetailedErrorMessages.CreateErrorMessage("UpdateBody is null");
            }

            return sr;
        }

        public bool UpdateWallet(CommonObjects.Fiserv.Wallet w, cc.UserInfo user, cc.Enums.PaymentInterface pmtInterface, ref bool attemptedServiceCall, ref string errorMessage)
        {
            bool success = false;
            errorMessage = "";
            attemptedServiceCall = false;

            //walletInfo
            if (w != null)
            {
                cc.Fiserv_UpdateWalletBodyRequest updateWalletBodyRequest = new cc.Fiserv_UpdateWalletBodyRequest();
                updateWalletBodyRequest.walletProfile = new cc.Fiserv_WalletProfile();
                if (string.IsNullOrWhiteSpace(w.ConsumerEmail) == false)
                {
                    updateWalletBodyRequest.walletProfile.consumerEmail = new List<string>();
                    updateWalletBodyRequest.walletProfile.consumerEmail.Add(w.ConsumerEmail);
                }
                if (string.IsNullOrWhiteSpace(w.ConsumerPhone) == false)
                {
                    updateWalletBodyRequest.walletProfile.consumerPhone = new List<string>();
                    //updateWalletBodyRequest.walletProfile.consumerPhone.Add(w.ConsumerPhone);
                    //updateWalletBodyRequest.walletProfile.consumerPhone.Add(w.ConsumerPhone.Replace("-", "").Replace("(", "").Replace(")", ""));
                    //updated 7/31/2020 to use helper for Fiserv format
                    updateWalletBodyRequest.walletProfile.consumerPhone.Add(IFM_CreditCardProcessing.Common.Fiserv_Formatted_Phone(w.ConsumerPhone));
                }
                if (string.IsNullOrWhiteSpace(w.ConsumerName) == false)
                {
                    updateWalletBodyRequest.walletProfile.consumerName = w.ConsumerName;
                }

                int fiservWalletId = w.FiservWalletId;
                cc.FiservErrorObject errorObject = null;
                string responseMessage = "";
                bool fiservWalletJustCreated = false;
                string authToken = "";
                string sessionToken = "";
                int sessionId = 0;
                attemptedServiceCall = true;

                success = creditCardMethods.SuccessfullyUpdatedFiservWallet(updateWalletBodyRequest, fiservWalletId: ref fiservWalletId, walletSessionUserId: cc.Common.AdjustedEmailAddressForFiservApiCall(w.KeyIdentifier), authToken: ref authToken, sessionToken: ref sessionToken, sessionId: ref sessionId, pmtInterface: pmtInterface, user: user, fiservWalletJustCreated: ref fiservWalletJustCreated, errorObject: ref errorObject, responseMessage: ref responseMessage);
                if (success == true)
                {
                    //nothing to do
                }
                else
                {
                    errorMessage = "problem updating wallet";
                    //use something specific if possible; use new byref param responseMessage
                    if (string.IsNullOrWhiteSpace(responseMessage) == false)
                    {
                        errorMessage = errorMessage + ": " + responseMessage;
                    }
                    else if (errorObject != null && string.IsNullOrWhiteSpace(errorObject.ErrorMessage) == false)
                    {
                        errorMessage = errorObject.ErrorMessage;
                    }
                }
            }
            else
            {
                errorMessage = "No wallet information provided.";
            }

            return success;
        }

        public APIResponses.Common.ServiceResult<APIResponses.Fiserv.FiservUpdateWalletsResult> UpdateWallets(CommonObjects.Fiserv.UpdateWalletsBody updateBody)
        {
            //walletInfo
            var sr = new APIResponses.Common.ServiceResult<APIResponses.Fiserv.FiservUpdateWalletsResult>();
            if (updateBody != null)
            {
                if (updateBody.Items != null && updateBody.Items.Count > 0)
                {
                    cc.Enums.PaymentInterface pmtInterface = cc.Enums.PaymentInterface.MemberPortalSite;
                    if (updateBody.IsFromOneView == true)
                    {
                        pmtInterface = IFM_CreditCardProcessing.Enums.PaymentInterface.OneView;
                    }
                    cc.UserInfo user = new cc.UserInfo();
                    user.Username = updateBody.UserName;
                    user.UserType = updateBody.UserType;

                    bool hasSuccess = false;
                    //bool hasFailure = false; //not really needed
                    bool attemptedAny = false;
                    string bestErrorMessage = "";

                    foreach (Wallet w in updateBody.Items)
                    {
                        bool success_currentWallet = false;
                        bool attemptedServiceCall_currentWallet = false;
                        string errorMessage_currentWallet = "";

                        //CommonObjects.Fiserv.UpdateWalletBody singleUpdateBody = new UpdateWalletBody(updateBody, w);


                        success_currentWallet = UpdateWallet(w, user, pmtInterface, ref attemptedServiceCall_currentWallet, ref errorMessage_currentWallet);
                        if (success_currentWallet == true)
                        {
                            //all good for at least 1; return success for entire method
                            attemptedAny = true;
                            hasSuccess = true; //added 5/4/2023 (not sure how it was missed originally)
                        }
                        else
                        {
                            //hasFailure = true; //not really needed
                            if (attemptedServiceCall_currentWallet == true)
                            {
                                attemptedAny = true;
                                if (string.IsNullOrWhiteSpace(errorMessage_currentWallet) == false)
                                {
                                    bestErrorMessage = errorMessage_currentWallet;
                                }
                            }
                        }
                    }

                    if (hasSuccess == true)
                    {
                        sr.ResponseData.Success = true;
                    }
                    else
                    {
                        string errorMessage = "";
                        if (attemptedAny == false)
                        {
                            errorMessage = "No wallet information provided.";
                        }
                        else
                        {
                            errorMessage = bestErrorMessage;
                            if (string.IsNullOrWhiteSpace(errorMessage) == true)
                            {
                                errorMessage = "problem updating wallet";
                            }
                        }
                        sr.Messages.CreateErrorMessage(errorMessage);
                    }
                }
                else
                {
                    sr.Messages.CreateErrorMessage("No wallet information provided.");
                }
            }

            return sr;
        }

        private void SetWalletError(string errorMessage, string DetailedErrorMessage, APIResponses.Common.MessagesList ml, APIResponses.Common.MessagesList detailedErrors)
        {
            //use something specific if possible; use new byref param responseMessage
            if (errorMessage.IsNotNullEmptyOrWhitespace())
            {
                ml.CreateErrorMessage(errorMessage);
            }

            if (DetailedErrorMessage.IsNotNullEmptyOrWhitespace())
            {
                detailedErrors.CreateErrorMessage(DetailedErrorMessage);
            }
        }

        private void SetWalletError(string errorMessage, string responseMessage, FiservErrorObject errorObject, APIResponses.Common.MessagesList ml, APIResponses.Common.MessagesList detailedErrors)
        {
            //use something specific if possible; use new byref param responseMessage
            if (string.IsNullOrWhiteSpace(responseMessage) == false)
            {
                errorMessage = errorMessage + ": " + responseMessage;
            }

            ml.CreateErrorMessage(errorMessage);

            if (errorObject != null)
            {
                if (errorObject.ErrorMessage.IsNotNullEmptyOrWhitespace())
                {
                    detailedErrors.CreateErrorMessage(errorObject.ErrorMessage);
                }
                if (errorObject.ExceptionMessage.IsNotNullEmptyOrWhitespace())
                {
                    detailedErrors.CreateErrorMessage(errorObject.ExceptionMessage);
                }
                if (errorObject.ExceptionResponseStatusDescription.IsNotNullEmptyOrWhitespace())
                {
                    detailedErrors.CreateErrorMessage(errorObject.ExceptionResponseStatusDescription);
                }
                if (errorObject.ExceptionToString.IsNotNullEmptyOrWhitespace())
                {
                    detailedErrors.CreateErrorMessage(errorObject.ExceptionToString);
                }
                if (errorObject.FiservErrorType != IFM_CreditCardProcessing.Enums.Fiserv_JsonTransaction_ErrorType.None)
                {
                    detailedErrors.CreateErrorMessage(errorObject.FiservErrorType.ToString());
                }
                if (errorObject.ExceptionResponseStatusCode != 0)
                {
                    detailedErrors.CreateErrorMessage(errorObject.ExceptionResponseStatusCode.ToString());
                }
                if (errorObject.ExceptionResponseStream.IsNotNullEmptyOrWhitespace())
                {
                    detailedErrors.CreateErrorMessage(errorObject.ExceptionResponseStream);
                }
            }
        }
    }
}
