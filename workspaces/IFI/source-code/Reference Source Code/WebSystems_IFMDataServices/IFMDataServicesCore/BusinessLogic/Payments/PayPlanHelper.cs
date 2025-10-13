using IFM.DataServicesCore.CommonObjects.Payments;
using IFM.PrimitiveExtensions;
using System;
using System.Diagnostics;
using System.Text;
using static IFM.DataServicesCore.CommonObjects.Enums.Enums;
using DCO = Diamond.Common.Objects;
using System.Linq;
using System.Collections.Generic;
using IFM.DataServicesCore.CommonObjects.OMP;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using Diamond.Common.Objects.Billing;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Web;
using Diamond.Common.Objects.Lookup.Advanced;
using APIResponse = IFM.DataServices.API.ResponseObjects;
using IFM_FiservDatabaseObjects;
using IFM_CreditCardProcessing;
using IFM.DataServicesCore.CommonObjects.Fiserv;
using QuickQuote.CommonMethods;
using IFM.RCC;

namespace IFM.DataServicesCore.BusinessLogic.Payments
{
    public static class PayPlanHelper
    {
        //for DirectBill only -  12 = annual 2, 13 = semi-annual 2, 14 = quarterly 2, 15 = monthly 2,18 = renewal credit card monthly 2, 19 = renewal eft monthly 2,

        public static APIResponse.Payments.PayplanChangedResult SetPayPlanNow(PayPlanData data, int prevPayPlanId) //updated 7/30/2020 for prevPayPlanId
        {
            APIResponse.Payments.PayplanChangedResult sr = new APIResponse.Payments.PayplanChangedResult();

            if (data != null)
            {
                //var payPlanIsChanging = DataServicesCore.BusinessLogic.Payments.PayPlanHelper.CurrentPayPlanId(data.PolicyId, data.ImageNumber) != (int)data.PayPlanId;
                //updated 7/30/2020 to use new prevPayPlanId param
                if (prevPayPlanId <= 0)
                {
                    prevPayPlanId = DataServicesCore.BusinessLogic.Payments.PayPlanHelper.CurrentPayPlanId(data.PolicyId, data.ImageNumber);
                }
                var payPlanIsChanging = prevPayPlanId != (int)data.PayPlanId;
                var transactionType = data.TransactionType;//global::Diamond.Common.Enums.Billing.BillingTransactionType.PayPlanChange;

                //added 7/30/2020
                int newPayPlanId = (int)data.PayPlanId;
                GetCurrentPayPlanObject prevPayPlan = new GetCurrentPayPlanObject();
                GetCurrentPayPlanObject newPayPlan = new GetCurrentPayPlanObject();
                //WS-2732 added else if (transactionType == BillingTransactionType.EditEftInfo || transactionType == BillingTransactionType.EditCreditCardInfo) in if condition
                if (transactionType == BillingTransactionType.PayPlanChange || transactionType == BillingTransactionType.EditEftInfo || transactionType == BillingTransactionType.EditCreditCardInfo)
                {

                    var t1 = Task.Factory.StartNew(() =>
                    {
                        GetCurrentPayPlanOptions ppOptionsPrev = new GetCurrentPayPlanOptions
                        {
                            PayPlanId = prevPayPlanId
                        };
                        prevPayPlan = GetCurrentPayPlans(ppOptionsPrev).FirstOrDefault();
                    });

                    var t2 = Task.Factory.StartNew(() =>
                    {
                        GetCurrentPayPlanOptions ppOptionsNew = new GetCurrentPayPlanOptions
                        {
                            PayPlanId = newPayPlanId
                        };
                        newPayPlan = GetCurrentPayPlans(ppOptionsNew).FirstOrDefault();
                    });

                    Task.WaitAll(t1, t2);
                }
                //WS-2732
                //else if(transactionType == BillingTransactionType.EditEftInfo || transactionType == BillingTransactionType.EditCreditCardInfo)
                //{
                //        GetCurrentPayPlanOptions ppOptionsNew = new GetCurrentPayPlanOptions
                //        {
                //            PayPlanId = newPayPlanId
                //        };
                //        newPayPlan = GetCurrentPayPlans(ppOptionsNew).FirstOrDefault();
                //}
                bool okayToContinue = true;
                if (Utilities.DisableRccFunctionality() == true)
                {
                    if (CommonObjects.OMP.BillingInformation.IsRCCPayPlanByName(prevPayPlan.BillingPayPlan) || CommonObjects.OMP.BillingInformation.IsRCCPayPlanByName(newPayPlan.BillingPayPlan))
                    {
                        okayToContinue = false;
                    }
                }

                if (okayToContinue == true) //added IF 7/30/2020 (so we can prevent RCC-related changes if needed)
                {
                    //if (CommonObjects.OMP.BillingInformation.PayPlanIsRecurring((int)data.PayPlanId))
                    //updated 7/30/2020 to use new variable
                    if (CommonObjects.OMP.BillingInformation.PayPlanIsRecurringByName(newPayPlan.BillingPayPlan))
                    {
                        if (CommonObjects.OMP.BillingInformation.IsRCCPayPlanByName(newPayPlan.BillingPayPlan))
                        {
                            //added 9/8/2020 to make sure we don't have masked card # on RCC Create
                            if (RccDataHasWalletItemId(data.RecurringCreditCardInformation) == false && RccDataHasFundingAccountToken(data.RecurringCreditCardInformation) == false && HasMaskedCardNumber(data.RecurringCreditCardInformation) == true)
                            {
                                //only okay for Update when not using iframe or switching to different wallet item
                                if (CommonObjects.OMP.BillingInformation.IsRCCPayPlanByName(prevPayPlan.BillingPayPlan) == false)
                                {
                                    //doesn't appear to currently have RCC payplan in Diamond; check for active RCC account
                                    if (HasActiveRccAccount(data.PolicyNumber) == false)
                                    {
                                        //doesn't appear to be an Update; don't allow
                                        okayToContinue = false;
                                    }
                                }
                            }

                            if (okayToContinue == true) //added 9/8/2020 to make sure we don't have masked card # on RCC Create
                            {
                                // 3-13-2019 Bug 31620
                                if (!payPlanIsChanging)
                                    transactionType = data.TransactionType;//global::Diamond.Common.Enums.Billing.BillingTransactionType.EditCreditCardInfo;

                                SetPayplan(sr, data.PolicyId, data.ImageNumber, data.ClientId, data.RecurringCreditCardInformation.DeductionDay, data.PayPlanId, null, data.RecurringCreditCardInformation, transactionType);
                                if (!sr.HasErrors)
                                {
                                    UpdateInsertRccService(sr, data.PolicyNumber, data.RecurringCreditCardInformation, payTraceCreateUpdate.payTraceCU.CreateCustomer);
                                }
                            }
                            else //new 9/8/2020
                            {
                                sr.Messages.CreateErrorMessage("Please enter a valid card number for your RCC account.");
                            }
                        }
                        else
                        {
                            if (data.RecurringEftInformation.IsNotNull() && data.RecurringEftInformation.PassesBasicValidation())
                            {
                                DCO.EFT.Eft eftInfo = new DCO.EFT.Eft
                                {
                                    BankAccountTypeId = data.RecurringEftInformation.AccountType,
                                    AccountNumber = data.RecurringEftInformation.AccountNumber,
                                    RoutingNumber = data.RecurringEftInformation.RoutingNumber,
                                    DeductionDay = data.RecurringEftInformation.DeductionDay,
                                    PolicyId = data.PolicyId,
                                    PolicyImageNum = data.ImageNumber,
                                    EftAccountId = data.RecurringEftInformation.AccountID
                                };
                                if (eftInfo.EftAccountId == 0)
                                {
                                    Int32 eftAccountNumber = CreateEFTAccount(eftInfo);
                                    sr.Messages.CreateGeneralMessage("New EFT record created.");
                                    SetPayplan(sr, data.PolicyId, data.ImageNumber, data.ClientId, data.RecurringEftInformation.DeductionDay, PayPlans.rEft, eftInfo, null, transactionType);
                                    if (!sr.HasErrors)
                                    {
                                        sr.RecurringPaymentDataFailed = false;
                                        sr.Messages.CreateGeneralMessage("Setup of Recurring EFT completed.");
                                    }
                                    else
                                    {
                                        sr.RecurringPaymentDataFailed = true;
                                        sr.Messages.CreateErrorMessage("Setup of Recurring EFT failed.");
                                    }
                                }
                                else if (eftInfo.EftAccountId > 0)
                                {

                                    // 3-13-2019 Bug 31620
                                    // doesn't seem to change anything if you try to process as an edit rather than a payplan change
                                    //if (!payPlanIsChanging)
                                    //    transactionType = global::Diamond.Common.Enums.Billing.BillingTransactionType.EditEftInfo;
                                    SetPayplan(sr, data.PolicyId, data.ImageNumber, data.ClientId, data.RecurringEftInformation.DeductionDay, PayPlans.rEft, eftInfo, null, transactionType);
                                    if (!sr.HasErrors)
                                    {
                                        sr.RecurringPaymentDataFailed = false;
                                        sr.Messages.CreateGeneralMessage("EFT information updated succesfully.");
                                    }
                                    else
                                    {
                                        sr.RecurringPaymentDataFailed = true;
                                        sr.Messages.CreateErrorMessage("EFT information updated failed.");
                                    }
                                }
                                //added 8/21/2023
                                if (sr.RecurringPaymentDataFailed == false && CommonObjects.OMP.BillingInformation.IsRCCPayPlanByName(prevPayPlan.BillingPayPlan))
                                {
                                    if (DeleteRCCAccountInfo(data.PolicyNumber))
                                    {
                                        sr.Messages.CreateGeneralMessage("Current RCC information removed.");
                                    }
                                    else
                                    {
                                        sr.Messages.CreateGeneralMessage("No RCC information available for removal.");
                                    }
                                }                                
                            }
                            else
                            {
                                if (data.RecurringEftInformation.IsNotNull() && data.RecurringEftInformation.Fiserv_WalletItemId > 0) //added IF 4/25/2023; original logic in ELSE
                                {
                                    sr.Messages.CreateErrorMessage("Problem loading EFT data from wallet item.");
                                }
                                else
                                {
                                    sr.Messages.CreateErrorMessage("Invalid recurring EFT data.");
                                }                                    
                            }
                        }
                    }
                    else
                    {
                        // Non Recurring
                        //No longer using this method to remove recurring plans. Should never reach this point but should catch error if it does...
                        //added 8/21/2023 just in case; removed since there's no other logic in this code path
                        //if (CommonObjects.OMP.BillingInformation.IsRCCPayPlanByName(prevPayPlan.BillingPayPlan))
                        //{
                        //    if (DeleteRCCAccountInfo(data.PolicyNumber))
                        //    {
                        //        sr.Messages.CreateGeneralMessage("Current RCC information removed.");
                        //    }
                        //    else
                        //    {
                        //        sr.Messages.CreateGeneralMessage("No RCC information available for removal.");
                        //    }
                        //}                        
                    }
                }
                else //new 7/30/2020
                {
                    sr.Messages.CreateErrorMessage("RCC-related PayPlan changes are not currently allowed. Please try again later.");
                }
            }
            else
            {
                sr.Messages.CreateErrorMessage("No payplan data provided.");
            }

            return sr;
        }

        public static APIResponse.Payments.PayplanChangedResult RemoveRecurringPayPlan(PayPlanData data)
        {
            APIResponse.Payments.PayplanChangedResult sr = new APIResponse.Payments.PayplanChangedResult();

            //GetCurrentPayPlanObject prevPayPlan = new GetCurrentPayPlanObject();
            //GetCurrentPayPlanObject newPayPlan = new GetCurrentPayPlanObject();

            var prevPayPlanId = DataServicesCore.BusinessLogic.Payments.PayPlanHelper.CurrentPayPlanId(data.PolicyId, data.ImageNumber);
            var ppOptionsPrev = new GetCurrentPayPlanOptions
            {
                PayPlanId = prevPayPlanId
            };
            var prevPayPlan = GetCurrentPayPlans(ppOptionsPrev).FirstOrDefault();

            //var t1 = Task.Run(() =>
            //{
            //    var prevPayPlanId = DataServicesCore.BusinessLogic.Payments.PayPlanHelper.CurrentPayPlanId(data.PolicyId, data.ImageNumber);
            //    var ppOptionsPrev = new GetCurrentPayPlanOptions
            //    {
            //        PayPlanId = prevPayPlanId
            //    };
            //    prevPayPlan = GetCurrentPayPlans(ppOptionsPrev).FirstOrDefault();
            //});

            //var t2 = Task.Run(() =>
            //{
            //    int newPayPlanId = 15;
            //    var ppOptionsNew = new GetCurrentPayPlanOptions
            //    {
            //        PayPlanId = newPayPlanId
            //    };
            //    newPayPlan = GetCurrentPayPlans(ppOptionsNew).FirstOrDefault();
            //});

            //Task.WaitAll(t1, t2);

            if (CommonObjects.OMP.BillingInformation.IsRCCPayPlanByName(prevPayPlan.BillingPayPlan))
            {
                // Pretty sure the Paytrace dll only handles RCC stuff.... wouldn't do anything for EFT
                // Non Recurring
                if (DeleteRCCAccountInfo(data.PolicyNumber))
                {
                    sr.Messages.CreateGeneralMessage("Current RCC information removed.");
                }
                else
                {
                    sr.Messages.CreateGeneralMessage("No RCC information available for removal.");
                }
            }

            //Payplan goes to Id 15 - Monthly, Installment Bill, Direct Bill
            SetPayplan(sr, data.PolicyId, data.ImageNumber, data.ClientId, 13, PayPlans.monthly, null, null, BillingTransactionType.PayPlanChange);

            return sr;
        }

        private static DCO.Billing.CreditCard SaveCreditCardInformation(APIResponse.Payments.PayplanChangedResult sr, int policyID, RecurringCreditCardInformation ccInfo)
        {
            if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {
                var creditCardData = new DCO.Billing.CreditCard();
                creditCardData.PolicyId = policyID;
                creditCardData.CVSCode = "123";
                creditCardData.AccountNumber = "11111111";// never user ready credit card numbers we don't want that data in diamond
                creditCardData.DeductionDay = ccInfo.DeductionDay; // updated 3-13-2019 was set to 14

                //MC = 1, Visa = 2,Dis = 3,American = 4
                creditCardData.CreditCardType = 2;
                //switch (IFM_CreditCardProcessing.Common.CreditCardTypeForNumber(ccInfo.CardNumber))
                //{
                //    case IFM_CreditCardProcessing.Enums.CreditCardType.MasterCard:
                //        creditCardData.CreditCardType = 1;
                //        //MC = 1, Visa = 2,Dis = 3,American = 4
                //        break;

                //    case IFM_CreditCardProcessing.Enums.CreditCardType.Visa:
                //        creditCardData.CreditCardType = 2;
                //        //MC = 1, Visa = 2,Dis = 3,American = 4
                //        break;

                //    case IFM_CreditCardProcessing.Enums.CreditCardType.Discover:
                //        creditCardData.CreditCardType = 3;
                //        //MC = 1, Visa = 2,Dis = 3,American = 4
                //        break;

                //    case IFM_CreditCardProcessing.Enums.CreditCardType.AmericanExpress:
                //        creditCardData.CreditCardType = 4;
                //        //MC = 1, Visa = 2,Dis = 3,American = 4
                //        break;
                //}
                //updated 7/30/2020
                ccInfo.Process_CardRelatedFields();
                switch (ccInfo.IFM_CC_Type)
                {
                    case IFM_CreditCardProcessing.Enums.CreditCardType.MasterCard:
                        creditCardData.CreditCardType = 1;
                        //MC = 1, Visa = 2,Dis = 3,American = 4
                        break;

                    case IFM_CreditCardProcessing.Enums.CreditCardType.Visa:
                        creditCardData.CreditCardType = 2;
                        //MC = 1, Visa = 2,Dis = 3,American = 4
                        break;

                    case IFM_CreditCardProcessing.Enums.CreditCardType.Discover:
                        creditCardData.CreditCardType = 3;
                        //MC = 1, Visa = 2,Dis = 3,American = 4
                        break;

                    case IFM_CreditCardProcessing.Enums.CreditCardType.AmericanExpress:
                        creditCardData.CreditCardType = 4;
                        //MC = 1, Visa = 2,Dis = 3,American = 4
                        break;
                }

                creditCardData.ExpirationDate = "12/2100";
                creditCardData.IsOneTimePayment = false;

                using (var DS = Insuresoft.DiamondServices.BillingService.SavePolicyCreditCardInfo())
                {
                    DS.RequestData.PolicyId = policyID;
                    DS.RequestData.CreditCard = creditCardData;
                    var response = DS.Invoke()?.DiamondResponse?.ResponseData;
                    if (response?.Success == false)
                    {
                        sr.Messages.CreateErrorMessage("Could not create credit card information in Diamond system.");
                    }
                    else
                    {
                        sr.Messages.CreateGeneralMessage("Diamond credit card information updated.");
                    }
                }
                return creditCardData;
            }
            else
            {
                sr.Messages.CreateErrorMessage("Could not create credit card information in Diamond system. Not logged in.");
            }
            return null;
        }

        private static void SetPayplan(APIResponse.Payments.PayplanChangedResult sr, int PolicyID, int DiamondPolicyImageNum, int DiamondClientId, Int32 DeductionDay, PayPlans payplanId, DCO.EFT.Eft PolicyEft, RecurringCreditCardInformation ccInfo, IFM.DataServicesCore.CommonObjects.Enums.Enums.BillingTransactionType transactionType)
        {
            sr.PayPlanChanged = false;
            if(BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {
                Func<int, int, int> GetAccountNumber = (int pId, int DiamondPolicyImageNumber) =>
                  {
                      var _accountNUmber = 0;
                      using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                      {
                          conn.Open();
                          using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetAccountNumberForPolicyId", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                          {
                              cmd.CommandTimeout = 90; ///TODO: do we still need this?
                              cmd.Parameters.AddWithValue("@policyId", pId);
                              using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                              {
                                  if (reader.HasRows)
                                  {
                                      reader.Read();
                                      if (!reader.IsDBNull(0))
                                      {
                                          _accountNUmber = reader.GetInt32(0);
                                      }
                                  }
                              }
                          }
                      }
                      return _accountNUmber;
                  };

                DCO.Billing.TransactionData trans = new DCO.Billing.TransactionData();

                var payplan = new GetCurrentPayPlanObject();

                var t1 = Task.Run(() =>
                {
                    GetCurrentPayPlanOptions currentPayPlanOptions = new GetCurrentPayPlanOptions();
                    currentPayPlanOptions.PayPlanId = Convert.ToInt32(payplanId);
                    payplan = GetCurrentPayPlans(currentPayPlanOptions).FirstOrDefault();
                });

                var t2 = Task.Run(() =>
                {
                    trans.AccountNum = GetAccountNumber(PolicyID, DiamondPolicyImageNum);
                });

                Task.WaitAll(t1, t2);

                trans.ClientId = DiamondClientId;
                trans.PolicyId = PolicyID;
                trans.PolicyImageNum = DiamondPolicyImageNum;

                trans.BillingPayPlanId = payplan.BillingPayPlanId;

                //trans.BillingTransactionTypeId = (int)global::Diamond.Common.Enums.Billing.BillingTransactionType.PayPlanChange;

                trans.BillingTransactionTypeId = (int)transactionType;

                //trans.BillingPayPlanTypeId = payplanId == PayPlans.rcc || payplanId == PayPlans.rEft ? 1 : 0;//0 = Installment Bill, 1 = EFT/CC, 3 = Payroll Deduction
                trans.BillingPayPlanTypeId = CommonObjects.OMP.BillingInformation.PayPlanIsRecurringByName(payplan.BillingPayPlan) ? 1 : 0; //switched to using PayPlanIsRecurringByName - 11/14/2022 - DJG


                //if (payplanId == PayPlans.accountBillCCMontly || payplanId == PayPlans.accountBillEFTMonthly || payplanId == PayPlans.accountBillMonthly)
                //if (CommonObjects.OMP.BillingInformation.IsAccountBillPayPlanByName(payplan.BillingPayPlan))
                //    trans.BillToId = 5;
                //else
                //    trans.BillToId = 1;

                trans.BillToId = CommonObjects.OMP.BillingInformation.IsAccountBillPayPlanByName(payplan.BillingPayPlan) ? 5 : 1; //one liner

                //Removed for now as we don't have any applicable code for this scenario
//                if (CommonObjects.OMP.BillingInformation.IsAccountBillPayPlanByName(payplan.BillingPayPlan))
//                {
//                    // need to have account > 0 if not create and assign but only if eligible
//#if DEBUG
//                    Debugger.Break();
//#endif
//                }

                // if ((payplanId == PayPlans.rEft || payplanId == PayPlans.accountBillEFTMonthly) && PolicyEft.IsNotNull())
                if (CommonObjects.OMP.BillingInformation.IsEFTPayPlanByName(payplan.BillingPayPlan) && PolicyEft.IsNotNull())
                {
                    trans.PolicyEFT = new DCO.EFT.EftAccountPolicy();

                    trans.PolicyEFT.EftAccount.AccountNumber = PolicyEft.AccountNumber;
                    trans.PolicyEFT.EftAccount.RoutingNumber = PolicyEft.RoutingNumber;
                    trans.PolicyEFT.EftAccount.BankAccountType = PolicyEft.BankAccountTypeId;
                    trans.PolicyEFT.PolicyId = PolicyEft.PolicyId;

                    trans.PolicyEFT.EftAccount.PolicyImageNumber = DiamondPolicyImageNum;
                    trans.PolicyEFT.EftAccount.DeductionDay = DeductionDay;

                    trans.PolicyEFT.EftAccount.EftTransactionType = 1;// 1= Debit, 2 = Credit
                    trans.PolicyEFT.EftAccount.AccountId.Id = PolicyEft.EftAccountId;
                    trans.PolicyEFT.BillingActivityType = 5;// Edit Billing
                    trans.PolicyEFT.EftAccountId = PolicyEft.EftAccountId;

                    //trans.PolicyEFT.PendingNBOrRenewal = PolicyEft.
                }

                // if (payplanId == PayPlans.rcc || payplanId == PayPlans.accountBillCCMontly)
                if (CommonObjects.OMP.BillingInformation.IsRCCPayPlanByName(payplan.BillingPayPlan))
                 {
                    trans.CreditCardData = SaveCreditCardInformation(sr, PolicyID, ccInfo);
                }

                try
                {
                    using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                    {
                        conn.Open();
                        using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetBillingPayplanAndPaymentType", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                        {
                            //cmd.Parameters.AddWithValue("@payPlanId", Convert.ToInt32(payplanId));
                            cmd.Parameters.AddWithValue("@payPlanId", payplan.BillingPayPlanId); //Switched to using payplan.BillingPayPlanId from payplanId to actually look up the accurate payplan id.
                            using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    trans.BillingPayPlanTypeId = reader.GetInt32(0);
                                    //0 = Installment Bill, 1 = EFT/CC, 3 = Payroll Deduction
                                    trans.PaymentTypeId = reader.GetInt32(1);
                                }
                                else
                                {
                                    sr.Messages.CreateErrorMessage("Could not gather paymenttype_id.");
                                    return;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    return;
                }


                if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
                {

                    using (var DS = Insuresoft.DiamondServices.BillingService.IssueTransaction())
                    {
                        DS.RequestData.TransactionData = new DCO.InsCollection<DCO.Billing.TransactionData>();
                        DS.RequestData.TransactionData.Add(trans);
                        
                        var response = DS.Invoke();
                        if (response.dv != null)
                        {
                            if (!response.dv.HasAnyItems())// && transactionType == global::Diamond.Common.Enums.Billing.BillingTransactionType.PayPlanChange)
                                sr.PayPlanChanged = true;

                            foreach (var v in response.dv.ValidationItems)
                            {
                                // TODO: Don't send exceptions back to user! 3-30-2019
                                sr.Messages.CreateErrorMessage(v.Message);
                            }
                        }
                    }
                }
                else
                {
                    sr.Messages.CreateErrorMessage("Could not login to perform action.");
                }


            }
        }

        public static RecurringData GetCurrentRecurringData(string PolicyNumber, Int32 PolicyId)
        {
            //RecurringData data = new RecurringData();
            //data.PolicyId = PolicyId;
            //data.PolicyNumber = PolicyNumber;
            //var payPlanId = DataServicesCore.BusinessLogic.Payments.PayPlanHelper.CurrentPayPlanId(data.PolicyId);
            //GetCurrentPayPlanOptions ppOptionsPrev = new GetCurrentPayPlanOptions
            //{
            //    PayPlanId = payPlanId
            //};
            //var PayPlan = GetCurrentPayPlans(ppOptionsPrev).FirstOrDefault();
            //if (CommonObjects.OMP.BillingInformation.IsRCCPayPlanByName(PayPlan.BillingPayPlan))
            //{
            //    data.RecurringCreditCardInformation = GetCurrentRccInfo(data.PolicyNumber, PolicyId);
            //}
            //if (CommonObjects.OMP.BillingInformation.IsEFTPayPlanByName(PayPlan.BillingPayPlan))
            //{
            //    data.RecurringEftInformation = GetCurrentRecurringEFTInfo(data.PolicyId);
            //}
            //return data;

            //updated 5/4/2023
            return GetCurrentRecurringData_UseWalletsOrWalletIdentifiers(PolicyNumber, PolicyId, null, null, null);
        }
        //added 5/4/2023
        public static RecurringData GetCurrentRecurringData_UseWalletsOrWalletIdentifiers(string PolicyNumber, Int32 PolicyId, List<FiservWallet> fws, List<Wallet> ws, List<string> walletIdentifiers)
        {
            RecurringData data = new RecurringData();
            data.PolicyId = PolicyId;
            data.PolicyNumber = PolicyNumber;
            var payPlanId = DataServicesCore.BusinessLogic.Payments.PayPlanHelper.CurrentPayPlanId(data.PolicyId);
            GetCurrentPayPlanOptions ppOptionsPrev = new GetCurrentPayPlanOptions
            {
                PayPlanId = payPlanId
            };
            var PayPlan = GetCurrentPayPlans(ppOptionsPrev).FirstOrDefault();
            if (CommonObjects.OMP.BillingInformation.IsRCCPayPlanByName(PayPlan.BillingPayPlan))
            {
                data.RecurringCreditCardInformation = GetCurrentRccInfo(data.PolicyNumber, PolicyId);
            }
            if (CommonObjects.OMP.BillingInformation.IsEFTPayPlanByName(PayPlan.BillingPayPlan))
            {
                if (ws != null && ws.Count > 0 && (fws == null || fws.Count == 0))
                {
                    DataServicesCore.BusinessLogic.Fiserv.WalletHelper helper = new DataServicesCore.BusinessLogic.Fiserv.WalletHelper();
                    fws = helper.FiservWalletsForWallets(ws);
                }
                if (fws != null && fws.Count > 0)
                {
                    data.RecurringEftInformation = GetCurrentRecurringEFTInfo_UseWallets(data.PolicyId, fws);
                }
                else
                {
                    data.RecurringEftInformation = GetCurrentRecurringEFTInfo_UseWalletsFromIdentifiers(data.PolicyId, walletIdentifiers);
                }                
            }
            return data;
        }
        public static RecurringData GetCurrentRecurringData_UseWalletItems(string PolicyNumber, Int32 PolicyId, List<FiservWalletItem> wis)
        {
            List<FiservWallet> ws = null;

            if (wis != null && wis.Count > 0)
            {
                FiservWallet w = new FiservWallet();
                w.walletItems = wis;
                ws = new List<FiservWallet>();
                ws.Add(w);
            }

            return GetCurrentRecurringData_UseWalletsOrWalletIdentifiers(PolicyNumber, PolicyId, ws, null, null);
        }

        public static IFM.DataServicesCore.CommonObjects.EmailDocument GeneratePayPlanChangeEmail(PayPlanData data, int prevPayPlanId) //added prevPayPlanId param 3/23/2018
        {
            if (data.IsEftPlan() || data.IsRccPlan())
            {
                StringBuilder sb = new StringBuilder();
                IFM.DataServicesCore.CommonObjects.EmailDocument doc = new CommonObjects.EmailDocument();
                doc.FromAddress = "NOReplyAutomatedEmail@indianafarmers.com";
                if (data.IsRccPlan())
                {
                    doc.ToAddress = data.RecurringCreditCardInformation.EmailAddress;
                    doc.Subject = "Indiana Farmers Insurance Recurring Credit Card Notice";

                    //sb.AppendLine($"<p>Your payments for policy number {data.PolicyNumber} are now set up for recurring credit card effective {DateTime.Now.ToShortDateString()}. Payments will be made using your {IFM_CreditCardProcessing.Common.CreditCardTypeForNumber(data.RecurringCreditCardInformation.CardNumber)} ending in {data.RecurringCreditCardInformation.CardNumber.LastNChars(4)}. </p>");
                    //updated 7/28/2020
                    //sb.AppendLine($"<p>Your payments for policy number {data.PolicyNumber} are now set up for recurring credit card effective {DateTime.Now.ToShortDateString()}. Payments will be made using your {data.RecurringCreditCardInformation.CardTypeText} ending in {data.RecurringCreditCardInformation.CardNumber.LastNChars(4)}. </p>");
                    //updated again 7/30/2020
                    data.RecurringCreditCardInformation.Process_CardRelatedFields();
                    //sb.AppendLine($"<p>Your payments for policy number {data.PolicyNumber} are now set up for recurring credit card effective {DateTime.Now.ToShortDateString()}. Payments will be made using your {data.RecurringCreditCardInformation.CardTypeText} ending in {data.RecurringCreditCardInformation.FundingAccountLastFourDigits}. </p>");
                    //updated 9/8/2020
                    string cardType = "card";
                    string lastFour = "****";
                    if (string.IsNullOrWhiteSpace(data.RecurringCreditCardInformation.CardTypeText) == false)
                    {
                        cardType = data.RecurringCreditCardInformation.CardTypeText;
                    }
                    else
                    {
                        if (HasMaskedCardNumber(data.RecurringCreditCardInformation) == true)
                        {
                            string ccType = RccCardType(data.PolicyNumber);
                            if (string.IsNullOrWhiteSpace(ccType) == false)
                            {
                                string cardTypeText = Utilities.CCTextForType(ccType);
                                if (string.IsNullOrWhiteSpace(cardTypeText) == false)
                                {
                                    cardType = cardTypeText;
                                }
                            }
                        }
                    }
                    if (string.IsNullOrWhiteSpace(data.RecurringCreditCardInformation.FundingAccountLastFourDigits) == false)
                    {
                        lastFour = data.RecurringCreditCardInformation.FundingAccountLastFourDigits;
                    }
                    else
                    {
                        //can add lookup, but will hold off for now as Leaf should be able to provide the mastedCardNum, which should allow everything to work
                    }
                    sb.AppendLine($"<p>Your payments for policy number {data.PolicyNumber} are now set up for recurring credit card effective {DateTime.Now.ToShortDateString()}. Payments will be made using your {cardType} ending in {lastFour}. </p>");
                    //sb.AppendLine($"<p>If you need to make changes to the credit card information or would like to cancel your recurring payments, please visit <a href=\"www.indianafarmers.com\">www.indianafarmers.com</a>, log in to your account, and select the Recurring Payment option. You can use the Add, Update, and Delete buttons to make changes.</p>");
                    //updated 3/21/2018
                    sb.AppendLine($"<p>If you need to make changes to the credit card information or would like to cancel your recurring payments, please visit <a href=\"www.indianafarmers.com\">www.indianafarmers.com</a>, log in to your account, and select the Recurring Payment option. You can use the Edit and Cancel buttons to make changes.</p>");
                    sb.AppendLine($"<p>Questions? Please contact Customer Service at 800.477.1660.</p>");
                    sb.AppendLine($"<p>Thank you for your business.</p>");


                }
                if (data.IsEftPlan())
                {
                    doc.ToAddress = data.RecurringEftInformation.EmailAddress;
                    doc.Subject = "Indiana Farmers Insurance Payment Notice";

                    sb.AppendLine($"<p>Your electronic payments for policy number {data.PolicyNumber} are now set up to be automatically withdrawn from your bank account effective {DateTime.Now.ToShortDateString()}. Payments will be made using your bank account ending in {data.RecurringEftInformation.AccountNumber.LastNChars(4)}. </p>");
                    //sb.AppendLine($"<p>If you need to make changes to the credit card information or would like to cancel your recurring payments, please visit <a href=\"www.indianafarmers.com\">www.indianafarmers.com</a>, log in to your account, and select the Recurring Payment option. You can use the Add, Update, and Delete buttons to make changes.</p>");
                    //updated 3/21/2018
                    sb.AppendLine($"<p>If you need to make changes to the bank account information or would like to cancel your recurring payments, please visit <a href=\"www.indianafarmers.com\">www.indianafarmers.com</a>, log in to your account, and select the Recurring Payment option. You can use the Edit and Cancel buttons to make changes.</p>");
                    sb.AppendLine($"<p>Questions? Please contact Customer Service at 800.477.1660.</p>");
                    sb.AppendLine($"<p>Thank you for your business.</p>");
                }
                doc.Body = sb.ToString();

                return doc;
            }
            else //added 3/23/2018
            {
                if (prevPayPlanId > 0)
                {
                    if (System.Enum.TryParse<PayPlans>(prevPayPlanId.ToString(), out PayPlans prevPayPlanIdEnum))
                    {
                        if (prevPayPlanIdEnum == PayPlans.rcc || prevPayPlanIdEnum == PayPlans.accountBillCCMontly || prevPayPlanIdEnum == PayPlans.rEft || prevPayPlanIdEnum == PayPlans.accountBillEFTMonthly)
                        {
                            StringBuilder sb = new StringBuilder();
                            IFM.DataServicesCore.CommonObjects.EmailDocument doc = new CommonObjects.EmailDocument();
                            doc.FromAddress = "NOReplyAutomatedEmail@indianafarmers.com";

                            if (prevPayPlanIdEnum == PayPlans.rcc || prevPayPlanIdEnum == PayPlans.accountBillCCMontly)
                            {
                                if (data.RecurringCreditCardInformation != null) //note: recurring info is not currently being sent on removal so may not have email address
                                {
                                    doc.ToAddress = data.RecurringCreditCardInformation.EmailAddress;
                                }
                                doc.Subject = "Indiana Farmers Insurance Recurring Credit Card Notice";

                                sb.AppendLine($"<p>As requested, your recurring credit card payments for policy number {data.PolicyNumber} have been deleted from our system.</p>");
                                sb.AppendLine($"<p>Effective {DateTime.Now.ToShortDateString()}, your payments will no longer be processed automatically.</p>");
                                sb.AppendLine($"<p>Questions? Please contact Customer Service at 800.477.1660.</p>");
                                sb.AppendLine($"<p>Thank you for your business.</p>");
                            }
                            if (prevPayPlanIdEnum == PayPlans.rEft || prevPayPlanIdEnum == PayPlans.accountBillEFTMonthly)
                            {
                                if (data.RecurringEftInformation != null) //note: recurring info is not currently being sent on removal so may not have email address
                                {
                                    doc.ToAddress = data.RecurringEftInformation.EmailAddress;
                                }
                                doc.Subject = "Indiana Farmers Insurance Payment Notice";

                                sb.AppendLine($"<p>As requested, your recurring payments for policy number {data.PolicyNumber} have been deleted from our system.</p>");
                                sb.AppendLine($"<p>Effective {DateTime.Now.ToShortDateString()}, your payments will no longer be processed automatically.</p>");
                                sb.AppendLine($"<p>Questions? Please contact Customer Service at 800.477.1660.</p>");
                                sb.AppendLine($"<p>Thank you for your business.</p>");
                            }

                            doc.Body = sb.ToString();

                            if (string.IsNullOrWhiteSpace(doc.ToAddress)) //try to look up email address if not present
                            {
                                doc.ToAddress = RecurringEmailAddress(data.PolicyNumber, data.PolicyId, data.ImageNumber, prevPayPlanIdEnum);
                            }

                            if (string.IsNullOrWhiteSpace(doc.ToAddress) == false) //only return doc if email address is present
                            {
                                return doc;
                            }
                        }
                    }
                    //PayPlans prevPayPlanIdEnum = Convert.ChangeType(prevPayPlanId, PayPlans);
                    //PayPlans prevPayPlanIdEnum = (PayPlans)prevPayPlanId;
                    //if (System.Enum.IsDefined(typeof(PayPlans), prevPayPlanIdEnum))
                    //{

                    //}
                }
            }
            return null;
        }

        //added 3/22/2018
        public static int CurrentPayPlanId(int policyId, int imageNum = 0)
        {
            int payPlanId = 0;

            if (policyId > 0)
            {
                try
                {
                    using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                    {
                        conn.Open();
                        using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetCurrentBillingPayplanId", conn) { CommandType = System.Data.CommandType.StoredProcedure})
                        {
                            cmd.Parameters.AddWithValue("@policyId", policyId);

                            using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    payPlanId = reader.GetInt32(0);
                                }
                                else
                                {

                                    ///TODO: handle this case?
                                    //unable to get payPlanId
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debugger.Break();
#endif
                }
            }

            return payPlanId;
        }
        //added 3/26/2018
        public static string RecurringEmailAddress(string polNum, int policyId, int imageNum, PayPlans payPlanId)
        {
            string eml = "";

            if (payPlanId == PayPlans.rcc || payPlanId == PayPlans.accountBillCCMontly)
            {
                if (IFM_CreditCardProcessing.Common.RecurringCreditCardVendor() == IFM_CreditCardProcessing.Enums.CreditCardVendor.Fiserv) //added IF 7/29/2020 for Fiserv so it can also use wallet email address if needed
                {
                    using (RCCAccount rccAcct = new RCCAccount(RCCAccount.AccountLookupType.ByPolNum, polNum))
                    {
                        if (rccAcct.hasData == true)
                        {
                            eml = rccAcct.emailAddress;
                        }
                    }
                }
                //else
                //{
                //    eml = RccEmailAddress(polNum);
                //}
            }

            if(string.IsNullOrEmpty(eml))
            {
                eml = FindRecurringPaymentEmailAddress(polNum);
            }

            //these are handled in the above call now
            //if (string.IsNullOrWhiteSpace(eml)) //should maybe just check this one when RCC (since that's all it's been used for historically)
            //{
            //    eml = PolicyPrintDistributionEmailAddress(policyId, imageNum);
            //}

            //if (string.IsNullOrWhiteSpace(eml)) //will attempt to use MemberPortal account email
            //{
            //    eml = LastMemberPortalEmailAddressForPolicyNumber(polNum);
            //}

            return eml;
        }


        public static string FindRecurringPaymentEmailAddress(string polNum)
        {
            string eml = "";

            if (string.IsNullOrWhiteSpace(polNum) == false)
            {
                try
                {
                    using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                    {
                        conn.Open();
                        using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_FindRecurringPaymentEmailAddress", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                        {
                            cmd.Parameters.AddWithValue("@policyNumber", polNum);
                            using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    eml = reader.GetString(0);
                                }
                                else
                                {
                                    //unable to get email address
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debugger.Break();
#endif
                }
            }

            return eml;
        }

        public static List<int> GetCurrentPayPlanIds(GetCurrentPayPlanOptions PayPlanOptions)
        {
            List<int> ids = new List<int>();
            var payPlans = GetCurrentPayPlans(PayPlanOptions);
            foreach (var payplan in payPlans)
            {
                ids.Add(payplan.BillingPayPlanId);
            }
            return ids;
        }

        public static List<GetCurrentPayPlanObject> GetCurrentPayPlans(GetCurrentPayPlanOptions PayPlanOptions)
        {
            IEnumerable<GetCurrentPayPlanObject> PayPlans;
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
            {
                var proc = "dbo.usp_GetPayPlans";
                var parameters = new DynamicParameters();
                if (PayPlanOptions.PayPlanId.HasValue())
                {
                    parameters.Add("PayPlanId", PayPlanOptions.PayPlanId);
                }
                if (PayPlanOptions.PayPlanName.HasValue())
                {
                    parameters.Add("PayPlanName", PayPlanOptions.PayPlanName);
                }
                if (PayPlanOptions.LobId.HasValue())
                {
                    parameters.Add("LOBId", PayPlanOptions.LobId);
                }
                if (PayPlanOptions.StateId.HasValue())
                {
                    parameters.Add("StateId", PayPlanOptions.StateId);
                }
                if (PayPlanOptions.BillMethodId.HasValue())
                {
                    parameters.Add("BillMethodId", PayPlanOptions.BillMethodId);
                }
                if (PayPlanOptions.PolicyTermId.HasValue())
                {
                    parameters.Add("PolicyTermId", PayPlanOptions.PolicyTermId);
                }
                if (PayPlanOptions.GetExpiredPayPlans == true)
                {
                    parameters.Add("GetExpiredPayPlans", 1, dbType:DbType.Boolean);
                }
                if (PayPlanOptions.IsRenewal == true)
                {
                    parameters.Add("IsRenewal", 1, dbType: DbType.Boolean);
                }
                if (PayPlanOptions.EffectiveDate.HasValue())
                {
                    parameters.Add("EffectiveDate", PayPlanOptions.EffectiveDate, dbType: DbType.String);
                }
                PayPlans = connection.Query<GetCurrentPayPlanObject>(proc, parameters, commandType: CommandType.StoredProcedure);
            }
            return PayPlans.ToList();
        }

        #region "Reccurring Payments"

        private static bool DeleteRCCAccountInfo(string PolicyNumber)
        {
            paytraceDELETE oPayTraceDelete = new paytraceDELETE(PolicyNumber);
            oPayTraceDelete.Execute();
            oPayTraceDelete.doCreateIFMPayPlanEntry = false;
            if (!oPayTraceDelete.hasError)
            {
                return true;
            }
            return false;
        }

        private static RecurringCreditCardInformation GetCurrentRccInfo(string PolicyNumber, int policyId)
        {
            using (var _rccAccount = new RCCAccount())
            {
                using (ExportPayTraceCustomers exportObj = new ExportPayTraceCustomers(PolicyNumber))
                {
                    exportObj.Export();
                    if (exportObj.HasCustomers == true && exportObj.CustomerCount == 1 && exportObj.HasError == false)
                    {
                        var cus = exportObj.Customers[0];
                        _rccAccount.FillRccAccountObjectFromPayTraceCustomerObject(ref cus);
                        RecurringCreditCardInformation data = new RecurringCreditCardInformation();
                        //data.CardExpireMonth = Convert.ToInt32(_rccAccount.ccExpMonth);
                        //data.CardExpireYear = Convert.ToInt32(_rccAccount.ccExpYear);
                        //updated 7/27/2020 to handle for expMonth/expYear not being returned from Fiserv
                        CommonHelperClass chc = new CommonHelperClass();
                        data.CardExpireMonth = chc.IntegerForString(_rccAccount.ccExpMonth);
                        data.CardExpireYear = chc.IntegerForString(_rccAccount.ccExpYear);
                        data.CardNumber = _rccAccount.ccCardNum;
                        //added 7/27/2020
                        data.Fiserv_FundingAccountToken = _rccAccount.fiservFundingAccountToken;
                        data.Fiserv_WalletItemId = _rccAccount.fiservWalletItemId;
                        // 3-13-2019
                        if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
                        {
                            using (var DS = Insuresoft.DiamondServices.BillingService.GetPolicyCreditCardInfo())
                            {
                                DS.RequestData.PolicyId = policyId;
                                data.DeductionDay = DS.Invoke()?.DiamondResponse?.ResponseData?.CreditCard?.DeductionDay ?? 0;
                            }
                        }

                        data.EmailAddress = _rccAccount.emailAddress;
                        return data;
                    }
                }
            }
            return null;
        }

        private static void UpdateInsertRccService(APIResponse.Payments.PayplanChangedResult sr, string PolicyNumber, RecurringCreditCardInformation ccInfo, payTraceCreateUpdate.payTraceCU rccTransType)
        {
            //string ccType = "";
            //switch (IFM_CreditCardProcessing.Common.CreditCardTypeForNumber(ccInfo.CardNumber))
            //{
            //    case IFM_CreditCardProcessing.Enums.CreditCardType.Visa:
            //        ccType = "V";
            //        break;

            //    case IFM_CreditCardProcessing.Enums.CreditCardType.MasterCard:
            //        ccType = "M";
            //        break;

            //    case IFM_CreditCardProcessing.Enums.CreditCardType.AmericanExpress:
            //        ccType = "A";
            //        break;

            //    case IFM_CreditCardProcessing.Enums.CreditCardType.Discover:
            //        ccType = "D";
            //        break;
            //} //removed 7/30/2020
            //added 7/27/2020; removed 7/30/2020
            //string fiservFundingAccountLastFourDigit = "";
            //string fiservFundingMethod = "";
            //string maskedCardNum = "";
            //if (string.IsNullOrWhiteSpace(ccType) == true && string.IsNullOrWhiteSpace(ccInfo.Fiserv_IframeResponse) == false)
            //{
            //    IFM_FiservResponseObjects.FiservFundingAccountResults fundingAcctResults = null;
            //    try
            //    {
            //        fundingAcctResults = IFM_CreditCardProcessing.Common.ObjectForJsonString<IFM_FiservResponseObjects.FiservFundingAccountResults>(ccInfo.Fiserv_IframeResponse);
            //        if (fundingAcctResults != null)
            //        {
            //            if (fundingAcctResults.fundingAccountValidationResult != null)
            //            {
            //                IFM_CreditCardProcessing.Enums.CreditCardType ifmCCType = IFM_CreditCardProcessing.Enums.CreditCardType.None;
            //                System.Collections.Generic.List<string> fundingMethodList = new System.Collections.Generic.List<string>();
            //                if (fundingAcctResults.fundingAccountValidationResult.cardValidationDetail != null)
            //                {
            //                    if (fundingAcctResults.fundingAccountValidationResult.cardValidationDetail.fundingMethodDetail != null)
            //                    {
            //                        fundingMethodList.Add(fundingAcctResults.fundingAccountValidationResult.cardValidationDetail.fundingMethodDetail.fundingMethod);
            //                    }
            //                }
            //                if (fundingAcctResults.fundingAccountValidationResult.fundingAccount != null)
            //                {
            //                    fundingMethodList.Add(fundingAcctResults.fundingAccountValidationResult.fundingAccount.fundingMethod);
            //                    if (string.IsNullOrWhiteSpace(fundingAcctResults.fundingAccountValidationResult.fundingAccount.fundingAccountLastFourDigit) == false)
            //                    {
            //                        fiservFundingAccountLastFourDigit = fundingAcctResults.fundingAccountValidationResult.fundingAccount.fundingAccountLastFourDigit;
            //                    }
            //                }

            //                if (fundingMethodList != null && fundingMethodList.Count > 0)
            //                {
            //                    bool hasPreferred = false;
            //                    ifmCCType = IFM_CreditCardProcessing.Common.PreferredCreditCardTypeForFiservPaymentMethodsOrExisting(fundingMethodList, existingCCtype: ifmCCType, hasPreferred: ref hasPreferred);
            //                    if (System.Enum.IsDefined(typeof(IFM_CreditCardProcessing.Enums.CreditCardType), ifmCCType) == true && ifmCCType != IFM_CreditCardProcessing.Enums.CreditCardType.None)
            //                    {
            //                        string rccTypeText = Utilities.CCTypeForCreditCardType(ifmCCType);
            //                        if (string.IsNullOrWhiteSpace(rccTypeText) == false)
            //                        {
            //                            ccType = rccTypeText;
            //                        }
            //                    }

            //                    foreach (string fm in fundingMethodList)
            //                    {
            //                        if (string.IsNullOrWhiteSpace(fm) == false)
            //                        {
            //                            fiservFundingMethod = fm;
            //                            break;
            //                        }
            //                    }
            //                }

            //                if (string.IsNullOrWhiteSpace(fiservFundingAccountLastFourDigit) == false)
            //                {
            //                    maskedCardNum = Utilities.MaskCreditCardByCardType(fiservFundingAccountLastFourDigit, cardType: ccType);
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}
            //updated 7/30/2020
            ccInfo.Process_CardRelatedFields();

            //Create New
            //payTraceCreateUpdate oPayTraceCreateUpdate = new payTraceCreateUpdate((int)rccTransType)
            //{
            //    policyNumber = PolicyNumber,
            //    //Only these 5 are required or will be saved at all
            //    cardNumber = ccInfo.CardNumber.Replace(" ",string.Empty),
            //    ccType = ccInfo.CardTypeAbbreviation, //updated 7/30/2020 from ccType
            //    expirationMonth = ccInfo.CardExpireMonth.ToString(),
            //    expirationYear = ccInfo.CardExpireYear.ToString(),
            //    email = ccInfo.EmailAddress,

            //    actionUser = "member",
            //    actionUserType = RCCAccount.UserType.P
            //};
            //updated 8/31/2020
            string cardNum = "";
            string ccType = "";
            string expMonth = "";
            string expYear = "";
            string email = "";
            if (ccInfo != null)
            {
                if (string.IsNullOrEmpty(ccInfo.CardNumber) == false)
                {
                    cardNum = ccInfo.CardNumber;
                    if (cardNum.Contains(" ") == true)
                    {
                        cardNum = cardNum.Replace(" ", string.Empty);
                    }
                }
                if (string.IsNullOrWhiteSpace(ccInfo.CardTypeAbbreviation) == false)
                {
                    ccType = ccInfo.CardTypeAbbreviation;
                }
                if (ccInfo.CardExpireMonth >= 1 && ccInfo.CardExpireMonth <= 12)
                {
                    expMonth = ccInfo.CardExpireMonth.ToString();
                    expYear = ccInfo.CardExpireYear.ToString();
                }
                if (string.IsNullOrWhiteSpace(ccInfo.EmailAddress) == false)
                {
                    email = ccInfo.EmailAddress;
                }
            }
            payTraceCreateUpdate oPayTraceCreateUpdate = new payTraceCreateUpdate((int)rccTransType)
            {
                policyNumber = PolicyNumber,
                //Only these 5 are required or will be saved at all
                cardNumber = cardNum,
                ccType = ccType,
                expirationMonth = expMonth,
                expirationYear = expYear,
                email = email,

                actionUser = "member",
                actionUserType = RCCAccount.UserType.P
            };

            //added 7/27/2020 for Fiserv
            //if (string.IsNullOrWhiteSpace(oPayTraceCreateUpdate.cardNumber) == true || string.IsNullOrWhiteSpace(maskedCardNum) == false) //removed 7/30/2020
            //{
            //    oPayTraceCreateUpdate.cardNumber = maskedCardNum;
            //}
            if (string.IsNullOrWhiteSpace(oPayTraceCreateUpdate.CreditCardSecurityCode) == true || string.IsNullOrWhiteSpace(ccInfo.SecurityCode) == false)
            {
                oPayTraceCreateUpdate.CreditCardSecurityCode = ccInfo.SecurityCode;
            }
            if (string.IsNullOrWhiteSpace(oPayTraceCreateUpdate.billingZip) == true || string.IsNullOrWhiteSpace(ccInfo.ZipCode) == false)
            {
                oPayTraceCreateUpdate.billingZip = ccInfo.ZipCode;
            }
            oPayTraceCreateUpdate.FiservAuthToken = ccInfo.Fiserv_AuthToken;
            oPayTraceCreateUpdate.FiservSessionToken = ccInfo.Fiserv_SessionToken;
            oPayTraceCreateUpdate.FiservSessionId = ccInfo.Fiserv_SessionId;

            bool fundingAccountTokenChanged = false;
            bool walletItemIdChanged = false;
            if (string.IsNullOrWhiteSpace(oPayTraceCreateUpdate.fiservFundingAccountToken) == false && string.IsNullOrWhiteSpace(ccInfo.Fiserv_FundingAccountToken) == false && oPayTraceCreateUpdate.fiservFundingAccountToken.ToUpper() != ccInfo.Fiserv_FundingAccountToken.ToUpper())
            {
                fundingAccountTokenChanged = true;
            }
            if (oPayTraceCreateUpdate.fiservWalletItemId > 0 && ccInfo.Fiserv_WalletItemId > 0 & oPayTraceCreateUpdate.fiservWalletItemId != ccInfo.Fiserv_WalletItemId)
            {
                walletItemIdChanged = true;
            }
            if (string.IsNullOrWhiteSpace(oPayTraceCreateUpdate.fiservFundingAccountToken) == true || string.IsNullOrWhiteSpace(ccInfo.Fiserv_FundingAccountToken) == false || walletItemIdChanged == true)
            {
                oPayTraceCreateUpdate.fiservFundingAccountToken = ccInfo.Fiserv_FundingAccountToken;
            }
            if (oPayTraceCreateUpdate.fiservWalletItemId <= 0 || ccInfo.Fiserv_WalletItemId > 0 || fundingAccountTokenChanged == true)
            {
                oPayTraceCreateUpdate.fiservWalletItemId = ccInfo.Fiserv_WalletItemId;
            }
            //if (string.IsNullOrWhiteSpace(oPayTraceCreateUpdate.fiservFundingAccountLastFourDigit) == true || string.IsNullOrWhiteSpace(fiservFundingAccountLastFourDigit) == false || fundingAccountTokenChanged == true || walletItemIdChanged == true)
            //{
            //    oPayTraceCreateUpdate.fiservFundingAccountLastFourDigit = fiservFundingAccountLastFourDigit;
            //}
            //if (string.IsNullOrWhiteSpace(oPayTraceCreateUpdate.fiservFundingMethod) == true || string.IsNullOrWhiteSpace(fiservFundingMethod) == false || fundingAccountTokenChanged == true || walletItemIdChanged == true)
            //{
            //    oPayTraceCreateUpdate.fiservFundingMethod = fiservFundingMethod;
            //}
            //updated 7/30/2020
            if (string.IsNullOrWhiteSpace(oPayTraceCreateUpdate.fiservFundingAccountLastFourDigit) == true || string.IsNullOrWhiteSpace(ccInfo.FundingAccountLastFourDigits) == false || fundingAccountTokenChanged == true || walletItemIdChanged == true)
            {
                oPayTraceCreateUpdate.fiservFundingAccountLastFourDigit = ccInfo.FundingAccountLastFourDigits;
            }
            if (string.IsNullOrWhiteSpace(oPayTraceCreateUpdate.fiservFundingMethod) == true || string.IsNullOrWhiteSpace(ccInfo.FundingMethod) == false || fundingAccountTokenChanged == true || walletItemIdChanged == true)
            {
                oPayTraceCreateUpdate.fiservFundingMethod = ccInfo.FundingMethod;
            }
            //added 7/30/2020
            if (ccInfo.FiservWalletItem != null)
            {
                oPayTraceCreateUpdate.FiservWalletItem = ccInfo.FiservWalletItem;
                oPayTraceCreateUpdate.FiservWalletItemAlreadyVerified = true;
            }

            if (rccTransType == payTraceCreateUpdate.payTraceCU.CreateCustomer)
            {
                if (oPayTraceCreateUpdate.isExistingRCCAccount == false)
                {
                    oPayTraceCreateUpdate.createPTrcc();
                }
                else
                {
                    UpdateInsertRccService(sr, PolicyNumber, ccInfo, payTraceCreateUpdate.payTraceCU.UpdateCustomer);
                    return;
                }
            }
            else
            {
                oPayTraceCreateUpdate.updatePTrcc();
            }

            //added 7/28/2020 so that info will be available later for email, etc.
            bool hasPotentialChange = false; //added 7/30/2020
            if (string.IsNullOrWhiteSpace(ccInfo.CardNumber) == true || string.IsNullOrWhiteSpace(oPayTraceCreateUpdate.cardNumber) == false)
            {
                ccInfo.CardNumber = oPayTraceCreateUpdate.cardNumber;
                hasPotentialChange = true; //added 7/30/2020
            }
            //if (string.IsNullOrWhiteSpace(ccInfo.CardTypeText) == true || string.IsNullOrWhiteSpace(oPayTraceCreateUpdate.ccType) == false)
            //{
            //    ccInfo.Set_CardTypeText(Utilities.CCTextForType(oPayTraceCreateUpdate.ccType));
            //}
            //updated 7/30/2020
            if (string.IsNullOrWhiteSpace(ccInfo.CardTypeAbbreviation) == true || string.IsNullOrWhiteSpace(oPayTraceCreateUpdate.ccType) == false)
            {
                ccInfo.CardTypeAbbreviation = oPayTraceCreateUpdate.ccType;
                hasPotentialChange = true;
            }
            if (hasPotentialChange == true)
            {
                ccInfo.Process_CardRelatedFields();
            }

            if (oPayTraceCreateUpdate.hasError)
            {
                sr.RecurringPaymentDataFailed = true;
                sr.Messages.CreateErrorMessage(oPayTraceCreateUpdate.errorMessage);
                switch (rccTransType)
                {
                    case payTraceCreateUpdate.payTraceCU.CreateCustomer:
                        sr.Messages.CreateErrorMessage("Setup of Recurring Credit Card failed.");
                        break;

                    case payTraceCreateUpdate.payTraceCU.UpdateCustomer:
                        sr.Messages.CreateErrorMessage("Update of Recurring Credit Card failed.");
                        break;
                }

            }
            else
            {
                sr.RecurringPaymentDataFailed = false;
                switch (rccTransType)
                {
                    case payTraceCreateUpdate.payTraceCU.CreateCustomer:
                        sr.Messages.CreateGeneralMessage("Setup of Recurring Credit Card completed.");
                        break;

                    case payTraceCreateUpdate.payTraceCU.UpdateCustomer:
                        sr.Messages.CreateGeneralMessage("Update of Recurring Credit Card completed.");

                        break;
                }
            }
        }

        public static DCO.EFT.Eft GetEftInfo(Int32 policyId)
        {
            if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {
                using (var DS = Insuresoft.DiamondServices.BillingService.GetPolicyEftInfo())
                {
                    DS.RequestData.PolicyId = policyId;
                    var eftInfo = DS.Invoke()?.DiamondResponse?.ResponseData?.Eft;
                    if (eftInfo != null && eftInfo.EftAccountId > 0)
                        return eftInfo;
                }
            }
            return null;
        }

        public static Int32 CreateEFTAccount(DCO.EFT.Eft eft)
        {
            if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {
                using (var DS = Insuresoft.DiamondServices.BillingService.SavePolicyEftInfo())
                {
                    DS.RequestData.Eft = eft;
                    var dResponse = DS.Invoke()?.DiamondResponse;
                    var response = dResponse?.ResponseData;
                    if (response != null)
                    {
                        if (response.EftAccountId < 1)
                        {
                            IFMErrorLogging.LogIssue("Failed to create an EFT Account", (dResponse?.DiamondValidation?.ValidationItems !=  null) ? string.Join(Environment.NewLine,from v in dResponse.DiamondValidation.ValidationItems select v.Message):"" , eft);
                        }
                        eft.EftAccountId = response.EftAccountId;
                        return response.EftAccountId;
                    }
                }
            }
            return 0;
        }

        private static RecurringEftInformation GetCurrentRecurringEFTInfo(Int32 policyId)
        {
            //if (policyId > 0)
            //{
            //    var eftInfo = GetEftInfo(policyId);
            //    if (eftInfo != null)
            //    {
            //        RecurringEftInformation echeckInfo = new RecurringEftInformation
            //        {
            //            DeductionDay = eftInfo.DeductionDay,
            //            AccountNumber = eftInfo.AccountNumber,
            //            AccountType = eftInfo.BankAccountTypeId,
            //            RoutingNumber = eftInfo.RoutingNumber,
            //            AccountID=eftInfo.EftAccountId

            //        };
            //        eftInfo.Dispose();
            //        return echeckInfo;
            //    }
            //}
            //return null;

            //updated 5/1/2023
            return GetCurrentRecurringEFTInfo_UseWallets(policyId, null);
        }
        private static RecurringEftInformation GetCurrentRecurringEFTInfo_UseWalletItems(Int32 policyId, List<FiservWalletItem> wis)
        {
            List<FiservWallet> ws = null;

            if (wis != null && wis.Count > 0)
            {
                FiservWallet w = new FiservWallet();
                w.walletItems = wis;
                ws = new List<FiservWallet>();
                ws.Add(w);
            }

            return GetCurrentRecurringEFTInfo_UseWallets(policyId, ws);
        }
        private static RecurringEftInformation GetCurrentRecurringEFTInfo_UseWallets(Int32 policyId, List<FiservWallet> ws)
        {
            if (policyId > 0)
            {
                var eftInfo = GetEftInfo(policyId);
                if (eftInfo != null)
                {
                    RecurringEftInformation echeckInfo = new RecurringEftInformation
                    {
                        DeductionDay = eftInfo.DeductionDay,
                        AccountNumber = eftInfo.AccountNumber,
                        AccountType = eftInfo.BankAccountTypeId,
                        RoutingNumber = eftInfo.RoutingNumber,
                        AccountID = eftInfo.EftAccountId
                    };
                    eftInfo.Dispose();

                    FiservWalletItem wi = FiservWalletItemWithMatchingEftInfo(ws, echeckInfo.RoutingNumber, echeckInfo.AccountNumber, echeckInfo.AccountType);
                    if (wi != null)
                    {
                        echeckInfo.Fiserv_WalletItemId = wi.fiservWalletItemId;
                    }

                    return echeckInfo;
                }
            }
            return null;
        }
        //added 5/4/2023
        private static RecurringEftInformation GetCurrentRecurringEFTInfo_UseWalletsFromIdentifiers(Int32 policyId, List<string> walletIdentifiers)
        {
            if (policyId > 0)
            {
                DataServicesCore.BusinessLogic.Fiserv.WalletHelper helper = new DataServicesCore.BusinessLogic.Fiserv.WalletHelper();
                List<FiservWallet> ws = helper.FiservWalletsForUserIds(walletIdentifiers);
                return GetCurrentRecurringEFTInfo_UseWallets(policyId, ws);
            }
            return null;
        }

        //added 5/1/2023
        public static FiservWalletItem FiservWalletItemWithMatchingEftInfo(List<FiservWalletItem> wis, string routingNumber, string accountNumber, Int32 accountType)
        {
            FiservWalletItem wiMatch = null;
            if (string.IsNullOrWhiteSpace(routingNumber) == false && string.IsNullOrWhiteSpace(accountNumber) == false && wis != null && wis.Count > 0)
            {
                foreach (IFM_FiservDatabaseObjects.FiservWalletItem wi in wis)
                {
                    if (wiMatch == null && wi != null)
                    {
                        IFM_CreditCardProcessing.Enums.CreditCardType cct = IFM_CreditCardProcessing.Enums.CreditCardType.None;
                        string errorMsg = "";
                        //if (IFM_CreditCardProcessing.Common.PaymentTypeForFiservPaymentMethod(wi.fundingMethod, ref cct, ref errorMsg) == IFM_CreditCardProcessing.Enums.PaymentType.EFT)
                        IFM_CreditCardProcessing.Enums.PaymentType pmtType = IFM_CreditCardProcessing.Common.PaymentTypeForFiservPaymentMethod(wi.fundingMethod, ref cct, ref errorMsg);
                        if (pmtType == IFM_CreditCardProcessing.Enums.PaymentType.EFT || pmtType == Enums.PaymentType.ECheck)
                        {
                            string wiRoutingNumber = "";
                            string wiAccountNumber = "";
                            Int32 wiAccountType = 0;
                            wi.DecryptBankRoutingAndAccountNumbers(ref wiRoutingNumber, ref wiAccountNumber);

                            if (string.IsNullOrWhiteSpace(wiRoutingNumber) == false && string.IsNullOrWhiteSpace(wiAccountNumber) == false)
                            {
                                if (string.IsNullOrWhiteSpace(wi.bankAccountType) == false)
                                {
                                    switch (wi.bankAccountType.ToUpper())
                                    {
                                        case "CHECKING":
                                            wiAccountType = 1;
                                            break;

                                        case "SAVING":
                                            wiAccountType = 2;
                                            break;
                                    }
                                }
                                if (routingNumber == wiRoutingNumber && accountNumber == wiAccountNumber && accountType == wiAccountType)
                                {
                                    wiMatch = wi;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return wiMatch;
        }
        public static FiservWalletItem FiservWalletItemWithMatchingEftInfo(List<FiservWallet> ws, string routingNumber, string accountNumber, Int32 accountType)
        {
            FiservWalletItem wiMatch = null;
            if (string.IsNullOrWhiteSpace(routingNumber) == false && string.IsNullOrWhiteSpace(accountNumber) == false && ws != null && ws.Count > 0)
            {
                foreach (IFM_FiservDatabaseObjects.FiservWallet w in ws)
                {
                    if (wiMatch == null && w != null)
                    {
                        wiMatch = FiservWalletItemWithMatchingEftInfo(w.walletItems, routingNumber, accountNumber, accountType);
                        if (wiMatch != null)
                        {
                            break;
                        }
                    }
                }
            }
            return wiMatch;
        }

        //added 7/29/2020
        //private static void SetCardFieldsForRccData(RecurringCreditCardInformation ccInfo, ref string fiservFundingAccountLastFourDigit, ref IFM_CreditCardProcessing.Enums.CreditCardType ifmCCType, ref string ccTypeText, ref string fiservFundingMethod, ref string maskedCardNum)
        //{
        //    fiservFundingAccountLastFourDigit = "";
        //    ifmCCType = IFM_CreditCardProcessing.Enums.CreditCardType.None;
        //    ccTypeText = "";
        //    fiservFundingMethod = "";
        //    maskedCardNum = "";

        //    if (ccInfo != null)
        //    {
        //        if (string.IsNullOrWhiteSpace(ccInfo.CardNumber) == false)
        //        {
        //            ifmCCType = IFM_CreditCardProcessing.Common.CreditCardTypeForNumber(ccInfo.CardNumber);
        //            if (System.Enum.IsDefined(typeof(IFM_CreditCardProcessing.Enums.CreditCardType), ifmCCType) == true && ifmCCType != IFM_CreditCardProcessing.Enums.CreditCardType.None)
        //            {
        //                ccTypeText = Utilities.CCTypeForCreditCardType(ifmCCType);
        //            }
        //            maskedCardNum = Utilities.MaskCreditCardByCardType(ccInfo.CardNumber, cardType: ccTypeText);
        //        }

        //        if (string.IsNullOrWhiteSpace(ccInfo.Fiserv_IframeResponse) == false)
        //        {
        //            IFM_FiservResponseObjects.FiservFundingAccountResults fundingAcctResults = null;
        //            try
        //            {
        //                fundingAcctResults = IFM_CreditCardProcessing.Common.ObjectForJsonString<IFM_FiservResponseObjects.FiservFundingAccountResults>(ccInfo.Fiserv_IframeResponse);
        //                if (fundingAcctResults != null)
        //                {
        //                    if (fundingAcctResults.fundingAccountValidationResult != null)
        //                    {
        //                        System.Collections.Generic.List<string> fundingMethodList = new System.Collections.Generic.List<string>();
        //                        if (fundingAcctResults.fundingAccountValidationResult.cardValidationDetail != null)
        //                        {
        //                            if (fundingAcctResults.fundingAccountValidationResult.cardValidationDetail.fundingMethodDetail != null)
        //                            {
        //                                fundingMethodList.Add(fundingAcctResults.fundingAccountValidationResult.cardValidationDetail.fundingMethodDetail.fundingMethod);
        //                            }
        //                        }
        //                        if (fundingAcctResults.fundingAccountValidationResult.fundingAccount != null)
        //                        {
        //                            fundingMethodList.Add(fundingAcctResults.fundingAccountValidationResult.fundingAccount.fundingMethod);
        //                            if (string.IsNullOrWhiteSpace(fundingAcctResults.fundingAccountValidationResult.fundingAccount.fundingAccountLastFourDigit) == false)
        //                            {
        //                                fiservFundingAccountLastFourDigit = fundingAcctResults.fundingAccountValidationResult.fundingAccount.fundingAccountLastFourDigit;
        //                            }
        //                        }

        //                        if (fundingMethodList != null && fundingMethodList.Count > 0)
        //                        {
        //                            bool hasPreferred = false;
        //                            ifmCCType = IFM_CreditCardProcessing.Common.PreferredCreditCardTypeForFiservPaymentMethodsOrExisting(fundingMethodList, existingCCtype: ifmCCType, hasPreferred: ref hasPreferred);
        //                            if (System.Enum.IsDefined(typeof(IFM_CreditCardProcessing.Enums.CreditCardType), ifmCCType) == true && ifmCCType != IFM_CreditCardProcessing.Enums.CreditCardType.None)
        //                            {
        //                                ccTypeText = Utilities.CCTypeForCreditCardType(ifmCCType);
        //                            }

        //                            foreach (string fm in fundingMethodList)
        //                            {
        //                                if (string.IsNullOrWhiteSpace(fm) == false)
        //                                {
        //                                    fiservFundingMethod = fm;
        //                                    break;
        //                                }
        //                            }
        //                        }

        //                        if (string.IsNullOrWhiteSpace(fiservFundingAccountLastFourDigit) == false)
        //                        {
        //                            maskedCardNum = Utilities.MaskCreditCardByCardType(fiservFundingAccountLastFourDigit, cardType: ccTypeText);
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //        }
        //    }
        //}
        public static void SetCardFieldsForFiservIframeResponse(string iframeResponse, ref string fiservFundingAccountLastFourDigit, ref IFM_CreditCardProcessing.Enums.CreditCardType ifmCCType, ref string ccTypeAbbreviation, ref string fiservFundingMethod, ref string maskedCardNum, ref string cardTypeText)
        {
            fiservFundingAccountLastFourDigit = "";
            ifmCCType = IFM_CreditCardProcessing.Enums.CreditCardType.None;
            ccTypeAbbreviation = "";
            fiservFundingMethod = "";
            maskedCardNum = "";
            cardTypeText = "";

            if (string.IsNullOrWhiteSpace(iframeResponse) == false)
            {
                IFM_FiservResponseObjects.FiservFundingAccountResults fundingAcctResults = null;
                try
                {
                    fundingAcctResults = IFM_CreditCardProcessing.Common.ObjectForJsonString<IFM_FiservResponseObjects.FiservFundingAccountResults>(iframeResponse);
                    if (fundingAcctResults != null)
                    {
                        if (fundingAcctResults.fundingAccountValidationResult != null)
                        {
                            System.Collections.Generic.List<string> fundingMethodList = new System.Collections.Generic.List<string>();
                            if (fundingAcctResults.fundingAccountValidationResult.cardValidationDetail != null)
                            {
                                if (fundingAcctResults.fundingAccountValidationResult.cardValidationDetail.fundingMethodDetail != null)
                                {
                                    fundingMethodList.Add(fundingAcctResults.fundingAccountValidationResult.cardValidationDetail.fundingMethodDetail.fundingMethod);
                                }
                            }
                            if (fundingAcctResults.fundingAccountValidationResult.fundingAccount != null)
                            {
                                fundingMethodList.Add(fundingAcctResults.fundingAccountValidationResult.fundingAccount.fundingMethod);
                                if (string.IsNullOrWhiteSpace(fundingAcctResults.fundingAccountValidationResult.fundingAccount.fundingAccountLastFourDigit) == false)
                                {
                                    fiservFundingAccountLastFourDigit = fundingAcctResults.fundingAccountValidationResult.fundingAccount.fundingAccountLastFourDigit;
                                }
                            }

                            if (fundingMethodList != null && fundingMethodList.Count > 0)
                            {
                                bool hasPreferred = false;
                                ifmCCType = IFM_CreditCardProcessing.Common.PreferredCreditCardTypeForFiservPaymentMethodsOrExisting(fundingMethodList, existingCCtype: ifmCCType, hasPreferred: ref hasPreferred);
                                if (System.Enum.IsDefined(typeof(IFM_CreditCardProcessing.Enums.CreditCardType), ifmCCType) == true && ifmCCType != IFM_CreditCardProcessing.Enums.CreditCardType.None)
                                {
                                    ccTypeAbbreviation = Utilities.CCTypeForCreditCardType(ifmCCType);
                                    if (string.IsNullOrWhiteSpace(ccTypeAbbreviation) == false)
                                    {
                                        cardTypeText = Utilities.CCTextForType(ccTypeAbbreviation);
                                    }
                                }

                                foreach (string fm in fundingMethodList)
                                {
                                    if (string.IsNullOrWhiteSpace(fm) == false)
                                    {
                                        fiservFundingMethod = fm;
                                        break;
                                    }
                                }
                            }

                            if (string.IsNullOrWhiteSpace(fiservFundingAccountLastFourDigit) == false)
                            {
                                maskedCardNum = Utilities.MaskCreditCardByCardType(fiservFundingAccountLastFourDigit, cardType: ccTypeAbbreviation);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
        //public static void SetCardFieldsForCardNumber(string cardNumber, ref string fundingAccountLastFourDigit, ref IFM_CreditCardProcessing.Enums.CreditCardType ifmCCType, ref string ccTypeAbbreviation, ref string cardTypeText)
        //{
        //    fundingAccountLastFourDigit = "";
        //    ifmCCType = IFM_CreditCardProcessing.Enums.CreditCardType.None;
        //    ccTypeAbbreviation = "";
        //    cardTypeText = "";

        //    if (string.IsNullOrWhiteSpace(cardNumber) == false)
        //    {
        //        ifmCCType = IFM_CreditCardProcessing.Common.CreditCardTypeForNumber(cardNumber);
        //        if (System.Enum.IsDefined(typeof(IFM_CreditCardProcessing.Enums.CreditCardType), ifmCCType) == true && ifmCCType != IFM_CreditCardProcessing.Enums.CreditCardType.None)
        //        {
        //            ccTypeAbbreviation = Utilities.CCTypeForCreditCardType(ifmCCType);
        //            if (string.IsNullOrWhiteSpace(ccTypeAbbreviation) == false)
        //            {
        //                cardTypeText = Utilities.CCTextForType(ccTypeAbbreviation);
        //            }
        //        }
        //        if (cardNumber.Length >= 4)
        //        {
        //            fundingAccountLastFourDigit = cardNumber.Substring(cardNumber.Length - 4, 4);
        //        }
        //    }
        //}
        //updated 7/30/2020
        public static void SetCardFieldsForCardNumber(string cardNumber, string currentCardTypeAbbreviation, ref string fundingAccountLastFourDigit, ref IFM_CreditCardProcessing.Enums.CreditCardType ifmCCType, ref string ccTypeAbbreviation, ref string cardTypeText)
        {
            fundingAccountLastFourDigit = "";
            ifmCCType = IFM_CreditCardProcessing.Enums.CreditCardType.None;
            ccTypeAbbreviation = "";
            cardTypeText = "";

            if (string.IsNullOrWhiteSpace(cardNumber) == false)
            {
                ifmCCType = IFM_CreditCardProcessing.Common.CreditCardTypeForNumber(cardNumber);
                if (System.Enum.IsDefined(typeof(IFM_CreditCardProcessing.Enums.CreditCardType), ifmCCType) == true && ifmCCType != IFM_CreditCardProcessing.Enums.CreditCardType.None)
                {
                    ccTypeAbbreviation = Utilities.CCTypeForCreditCardType(ifmCCType);
                    if (string.IsNullOrWhiteSpace(ccTypeAbbreviation) == false)
                    {
                        cardTypeText = Utilities.CCTextForType(ccTypeAbbreviation);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(currentCardTypeAbbreviation) == false)
                        {
                            ccTypeAbbreviation = currentCardTypeAbbreviation;
                            cardTypeText = Utilities.CCTextForType(currentCardTypeAbbreviation);
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(currentCardTypeAbbreviation) == false)
                    {
                        ccTypeAbbreviation = currentCardTypeAbbreviation;
                        cardTypeText = Utilities.CCTextForType(currentCardTypeAbbreviation);
                    }
                }
                if (cardNumber.Length >= 4)
                {
                    fundingAccountLastFourDigit = cardNumber.Substring(cardNumber.Length - 4, 4);
                }
            }
        }
        //public static void SetCardFieldForWalletItemId(int walletItemId, ref string fundingAccountToken, ref string fiservFundingAccountLastFourDigit, ref IFM_CreditCardProcessing.Enums.CreditCardType ifmCCType, ref string ccTypeAbbreviation, ref string fiservFundingMethod, ref string maskedCardNum, ref string cardTypeText)
        //{
        //    fundingAccountToken = "";
        //    fiservFundingAccountLastFourDigit = "";
        //    ifmCCType = IFM_CreditCardProcessing.Enums.CreditCardType.None;
        //    ccTypeAbbreviation = "";
        //    fiservFundingMethod = "";
        //    maskedCardNum = "";
        //    cardTypeText = "";

        //    if (walletItemId > 0)
        //    {
        //        DataServicesCore.BusinessLogic.Fiserv.WalletHelper helper = new DataServicesCore.BusinessLogic.Fiserv.WalletHelper();
        //        DataServicesCore.CommonObjects.Fiserv.WalletItem wItem = helper.WalletItemForWalletItemId(walletItemId);
        //        if (wItem != null)
        //        {
        //            fundingAccountToken = wItem.FundingAccountToken;
        //            fiservFundingAccountLastFourDigit = wItem.FundingAccountLastFourDigits;
        //            fiservFundingMethod = wItem.FundingMethod;

        //            if (string.IsNullOrWhiteSpace(fiservFundingMethod) == false)
        //            {
        //                ifmCCType = IFM_CreditCardProcessing.Common.CreditCardTypeForFiservPaymentMethod(fiservFundingMethod);
        //                if (System.Enum.IsDefined(typeof(IFM_CreditCardProcessing.Enums.CreditCardType), ifmCCType) == true && ifmCCType != IFM_CreditCardProcessing.Enums.CreditCardType.None)
        //                {
        //                    ccTypeAbbreviation = Utilities.CCTypeForCreditCardType(ifmCCType);
        //                    if (string.IsNullOrWhiteSpace(ccTypeAbbreviation) == false)
        //                    {
        //                        cardTypeText = Utilities.CCTextForType(ccTypeAbbreviation);
        //                    }
        //                }
        //            }

        //            if (string.IsNullOrWhiteSpace(fiservFundingAccountLastFourDigit) == false)
        //            {
        //                maskedCardNum = Utilities.MaskCreditCardByCardType(fiservFundingAccountLastFourDigit, cardType: ccTypeAbbreviation);
        //            }
        //        }
        //    }
        //}
        public static void SetCardFieldForWalletItemId(int walletItemId, ref IFM_FiservDatabaseObjects.FiservWalletItem fiservWalletItem, ref string fundingAccountToken, ref string fiservFundingAccountLastFourDigit, ref IFM_CreditCardProcessing.Enums.CreditCardType ifmCCType, ref string ccTypeAbbreviation, ref string fiservFundingMethod, ref string maskedCardNum, ref string cardTypeText)
        {
            fiservWalletItem = null;
            fundingAccountToken = "";
            fiservFundingAccountLastFourDigit = "";
            ifmCCType = IFM_CreditCardProcessing.Enums.CreditCardType.None;
            ccTypeAbbreviation = "";
            fiservFundingMethod = "";
            maskedCardNum = "";
            cardTypeText = "";

            if (walletItemId > 0)
            {
                DataServicesCore.BusinessLogic.Fiserv.WalletHelper helper = new DataServicesCore.BusinessLogic.Fiserv.WalletHelper();
                fiservWalletItem = helper.FiservWalletItemForWalletItemId(walletItemId);
                if (fiservWalletItem != null)
                {
                    fundingAccountToken = fiservWalletItem.fundingAccountToken;
                    fiservFundingAccountLastFourDigit = fiservWalletItem.fundingAccountLastFourDigit;
                    fiservFundingMethod = fiservWalletItem.fundingMethod;

                    if (string.IsNullOrWhiteSpace(fiservFundingMethod) == false)
                    {
                        ifmCCType = IFM_CreditCardProcessing.Common.CreditCardTypeForFiservPaymentMethod(fiservFundingMethod);
                        if (System.Enum.IsDefined(typeof(IFM_CreditCardProcessing.Enums.CreditCardType), ifmCCType) == true && ifmCCType != IFM_CreditCardProcessing.Enums.CreditCardType.None)
                        {
                            ccTypeAbbreviation = Utilities.CCTypeForCreditCardType(ifmCCType);
                            if (string.IsNullOrWhiteSpace(ccTypeAbbreviation) == false)
                            {
                                cardTypeText = Utilities.CCTextForType(ccTypeAbbreviation);
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(fiservFundingAccountLastFourDigit) == false)
                    {
                        maskedCardNum = Utilities.MaskCreditCardByCardType(fiservFundingAccountLastFourDigit, cardType: ccTypeAbbreviation);
                    }
                }
            }
        }

        //added 9/8/2020
        public static bool RccDataHasWalletItemId(IFM.DataServicesCore.CommonObjects.Payments.RecurringCreditCardInformation rccData)
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
        public static bool RccDataHasFundingAccountToken(IFM.DataServicesCore.CommonObjects.Payments.RecurringCreditCardInformation rccData)
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
        public static bool HasMaskedCardNumber(DataServicesCore.CommonObjects.Payments.RecurringCreditCardInformation rccData)
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
        public static string RccCardType(string polNum)
        {
            string ccType = "";

            if (string.IsNullOrWhiteSpace(polNum) == false)
            {
                try
                {
                    using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(AppConfig.Conn))
                    {
                        conn.Open();
                        using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.Parameters.AddWithValue("@polNum", polNum);
                            cmd.CommandText = "SELECT Rcc.ccType FROM tbl_RCC_Accounts as Rcc WHERE Rcc.polNum = @polNum and Rcc.active = 1 ORDER BY Rcc.id DESC";
                            using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    ccType = reader.GetString(0);
                                }
                                else
                                {
                                    //unable to get email address
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debugger.Break();
#endif
                }
            }

            return ccType;
        }
        public static bool HasActiveRccAccount(string polNum)
        {
            bool hasIt = false;

            if (string.IsNullOrWhiteSpace(polNum) == false)
            {
                try
                {
                    using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(AppConfig.Conn))
                    {
                        conn.Open();
                        using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.Parameters.AddWithValue("@polNum", polNum);
                            cmd.CommandText = "SELECT Rcc.id FROM tbl_RCC_Accounts as Rcc WHERE Rcc.polNum = @polNum and Rcc.active = 1 ORDER BY Rcc.id DESC";
                            using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    int rccId = reader.GetInt32(0);
                                    if (rccId > 0)
                                    {
                                        hasIt = true;
                                    }
                                }
                                else
                                {
                                    //unable to get email address
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debugger.Break();
#endif
                }
            }

            return hasIt;
        }

        //added 4/25/2023
        public static void SetEftInfoForWalletItemId(int walletItemId, ref IFM_FiservDatabaseObjects.FiservWalletItem fiservWalletItem, ref string routingNumber, ref string accountNumber, ref Int32 accountType)
        {
            fiservWalletItem = null;
            routingNumber = "";
            accountNumber = "";
            accountType = 0;

            string strLog = "";            
            QuickQuoteHelperClass qqHelper = new QuickQuoteHelperClass();

            string logEmail = "";
            bool logIt = FiservSettings_SetEftInfoForWalletItemId_LogViaEmail(emailToUse: ref logEmail);
            if (logIt)
            {
                strLog = "IFM.DataServicesCore.BusinessLogic.Payments.PayPlanHelper.SetEftInfoForWalletItemId: ";
            }

            if (walletItemId > 0)
            {
                if (logIt)
                {
                    strLog = qqHelper.appendText(strLog, "-walletItemId is greater than 0 (" + walletItemId.ToString() + ")", splitter: "<br />");
                }                
                DataServicesCore.BusinessLogic.Fiserv.WalletHelper helper = new DataServicesCore.BusinessLogic.Fiserv.WalletHelper();
                fiservWalletItem = helper.FiservWalletItemForWalletItemId(walletItemId);
                if (fiservWalletItem != null)
                {
                    if (logIt)
                    {
                        strLog = qqHelper.appendText(strLog, "-fiservWalletItem returned", splitter: "<br />");
                        strLog = qqHelper.appendText(strLog, "--fiservWalletItem.bankAccountType: " + fiservWalletItem.bankAccountType, splitter: "<br />");
                        strLog = qqHelper.appendText(strLog, "--fiservWalletItem.fundingMethod: " + fiservWalletItem.fundingMethod, splitter: "<br />");
                        strLog = qqHelper.appendText(strLog, "--fiservWalletItem.encryptedBankRoutingNumber: " + fiservWalletItem.encryptedBankRoutingNumber, splitter: "<br />");
                        strLog = qqHelper.appendText(strLog, "--fiservWalletItem.encryptedBankAccountNumber: " + fiservWalletItem.encryptedBankAccountNumber, splitter: "<br />");
                    }                    
                    IFM_CreditCardProcessing.Enums.CreditCardType cct = IFM_CreditCardProcessing.Enums.CreditCardType.None;
                    string errorMsg = "";
                    //if(IFM_CreditCardProcessing.Common.PaymentTypeForFiservPaymentMethod(fiservWalletItem.fundingMethod, ref cct, ref errorMsg) == IFM_CreditCardProcessing.Enums.PaymentType.EFT)
                    IFM_CreditCardProcessing.Enums.PaymentType pmtType = IFM_CreditCardProcessing.Common.PaymentTypeForFiservPaymentMethod(fiservWalletItem.fundingMethod, ref cct, ref errorMsg);
                    if (pmtType == IFM_CreditCardProcessing.Enums.PaymentType.EFT || pmtType == IFM_CreditCardProcessing.Enums.PaymentType.ECheck)
                    {
                        if (logIt)
                        {
                            strLog = qqHelper.appendText(strLog, "--IFM_CreditCardProcessing.Common.PaymentTypeForFiservPaymentMethod returned IFM_CreditCardProcessing.Enums.PaymentType.EFT or IFM_CreditCardProcessing.Enums.PaymentType.ECheck", splitter: "<br />");
                        }                        
                        fiservWalletItem.DecryptBankRoutingAndAccountNumbers(ref routingNumber, ref accountNumber);
                        if (logIt)
                        {
                            strLog = qqHelper.appendText(strLog, "---called fiservWalletItem.DecryptBankRoutingAndAccountNumbers", splitter: "<br />");
                            strLog = qqHelper.appendText(strLog, "----routingNumber: " + routingNumber, splitter: "<br />");
                            strLog = qqHelper.appendText(strLog, "----accountNumber: " + accountNumber, splitter: "<br />");
                        }                        

                        if (string.IsNullOrWhiteSpace(fiservWalletItem.bankAccountType) == false)
                        {
                            if (logIt)
                            {
                                strLog = qqHelper.appendText(strLog, "---fiservWalletItem.bankAccountType is something... will try to set accountType variable", splitter: "<br />");
                            }                            
                            switch (fiservWalletItem.bankAccountType.ToUpper())
                            {
                                case "CHECKING":
                                    accountType = 1;
                                    break;

                                case "SAVING":
                                    accountType = 2;
                                    break;
                            }
                            if (logIt)
                            {
                                strLog = qqHelper.appendText(strLog, "----accountType: " + accountType, splitter: "<br />");
                            }                            
                        }
                        else
                        {
                            if (logIt)
                            {
                                strLog = qqHelper.appendText(strLog, "---fiservWalletItem.bankAccountType is not something... will not try to set accountType variable", splitter: "<br />");
                            }                            
                        }
                    }
                    else
                    {
                        if (logIt)
                        {
                            strLog = qqHelper.appendText(strLog, "--IFM_CreditCardProcessing.Common.PaymentTypeForFiservPaymentMethod did not return IFM_CreditCardProcessing.Enums.PaymentType.EFT or IFM_CreditCardProcessing.Enums.PaymentType.ECheck", splitter: "<br />");
                        }                        
                    }
                }
                else
                {
                    if (logIt)
                    {
                        strLog = qqHelper.appendText(strLog, "-fiservWalletItem not returned", splitter: "<br />");
                    }                    
                }
            }else
            {
                if (logIt)
                {
                    strLog = qqHelper.appendText(strLog, "-walletItemId is not greater than 0", splitter: "<br />");
                }                
            }
            if (logIt)
            {
                if (string.IsNullOrWhiteSpace(logEmail) == true)
                {
                    logEmail = "dmink@indianafarmers.com";
                }
                IFM.DataServicesCore.BusinessLogic.OMP.General.SendEmail(logEmail, "EftWalletItemTest@indianafarmers.com", "EFT WalletItem Test Email", strLog);
            }            
        }
        public static bool FiservSettings_SetEftInfoForWalletItemId_LogViaEmail(ref string emailToUse)
        {
            bool isOkay = false;
            emailToUse = "";

            CommonHelperClass chc = new CommonHelperClass();
            isOkay = chc.GetApplicationXMLSettingForBoolean("FiservSettings_SetEftInfoForWalletItemId_LogViaEmail", "FiservSettings.xml");

            if (isOkay)
            {
                emailToUse = chc.GetApplicationXMLSetting("FiservSettings_SetEftInfoForWalletItemId_EmailToUse", "FiservSettings.xml");
            }

            return isOkay;
        }


        #endregion "Reccurring Payments"


    }
}
