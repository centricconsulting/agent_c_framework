using System;
using System.Collections.Generic;
using System.Linq;
using IFM.PrimitiveExtensions;
using static QuickQuote.CommonMethods.QuickQuoteHelperClass;
using DCO = Diamond.Common.Objects;
using IFM.DataServicesCore.BusinessLogic;
using IFM.DataServicesCore.BusinessLogic.Payments;
using Diamond.Common.Objects.Billing;
using DevExpress.Web.Internal.XmlProcessor;
using Diamond.Business.ThirdParty.tuxml;

#if DEBUG

using System.Diagnostics;


#endif

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class BillingInformation : ModelBase
    {
        private static int[] RecurringPayPlanIds = new int[]{ 18, 19 ,25, 26 }; //RCC, REFT, Account CC, Account EFT
        private static int[] RCCPayPlanIds = new int[] { 18, 25 }; //RCC, Account CC
        private static int[] REFTPayPlanIds = new int[] { 19, 26 }; // REFT, Account EFT
        private static int[] AccountBillPayPlanIds = new int[] { 24, 25, 26 }; // Account Monthly, Account CC, Account EFT
        private static int[] PayablePolicyStatusIds = new int[] { 1, 2, 4 }; //Inforce,Future,Pending
        private const int DirectBillId = 2; //BillMethodId
        private List<DCO.Billing.CashDetail> cashDetails = null;
        private DateTime FinalFlatCancelDate;
        private DateTime PolicyCancelDate;
        private DateTime LegalCancelNoticeDate;
        private DateTime FinalCancelDate;
        private DateTime ManualCancelDate;
        private DateTime TransactionEffectiveDate;
        private DateTime TransactionExpirationDate;

        public Int32 PolicyStatusId { get; set; }
        public string PolicyNumber { get; set; }
        public int PolicyId { get; set; }
        public int PolicyImageNumber { get; set; }
        public bool IsPartOfAccountBill { get; set; }
        public Int32 AccountBillId { get; set; }
        public string AccountNumber
        {
            get
            {
                if (this.AccountBillId > 0)
                {
                    return AccountBillId.GetAccountBillNumberFromBillingAccountId();
                }
                return "";
            }
        }
        public bool HasRecurringPaymentSetup { get; set; }
        public bool IsDirectBill { get; set; }
        public Int32 PayPlanId { get; set; }
        public string PayPlan { get; set; }
        public int PayplanDeductionDate { get; set; }
        public Int32 BillToId { get; set; }
        public string BillTo { get; set; }
        public Int32 BillingMethodId { get; set; }
        public string BillingMethod { get; set; }
        public double WrittenPremium { get; set; }
        public DateTime CarryDate { get; set; }
        public DateTime CancelDate {
            get
            {
                if (AppConfig.UseOtherCancellationStatusesInsteadOfPolicyCancelDateWhenApplicable)
                {
                    var DatesToChooseFrom = new List<DateTime>();
                    if (LegalCancelNoticeDate.IsEmptyOrDefaultDiamondDate() == false)
                    {
                        DatesToChooseFrom.Add(LegalCancelNoticeDate);
                    }
                    if (FinalFlatCancelDate.IsEmptyOrDefaultDiamondDate() == false)
                    {
                        DatesToChooseFrom.Add(FinalFlatCancelDate);
                    }
                    if (FinalCancelDate.IsEmptyOrDefaultDiamondDate() == false)
                    {
                        DatesToChooseFrom.Add(FinalCancelDate);
                    }
                    if (ManualCancelDate.IsEmptyOrDefaultDiamondDate() == false)
                    {
                        DatesToChooseFrom.Add(ManualCancelDate);
                    }
                    
                    if (DatesToChooseFrom.Count > 0)
                    {
                        return DatesToChooseFrom.Max();
                    }
                    else
                    {
                        return PolicyCancelDate;
                    }


                    //if (LegalCancelNoticeDate.IsEmptyOrDefaultDiamondDate() == false)
                    //{
                    //    return LegalCancelNoticeDate;
                    //}
                    //else if (FinalFlatCancelDate.IsEmptyOrDefaultDiamondDate() == false)
                    //{
                    //    return FinalFlatCancelDate;
                    //}
                    //else if(FinalCancelDate.IsEmptyOrDefaultDiamondDate() == false)
                    //{
                    //    return FinalCancelDate;
                    //}
                    //else if (ManualCancelDate.IsEmptyOrDefaultDiamondDate() == false)
                    //{
                    //    return ManualCancelDate;
                    //}
                    //else
                    //{
                    //    return PolicyCancelDate;
                    //}
                }
                else
                {
                    return PolicyCancelDate;
                }
            }
        } // 04-07-2020 - Task 43795

        public DateTime NewCodeCancelDate
        {
            get
            {
                    var DatesToChooseFrom = new List<DateTime>();
                    if (LegalCancelNoticeDate.IsEmptyOrDefaultDiamondDate() == false)
                    {
                        DatesToChooseFrom.Add(LegalCancelNoticeDate);
                    }
                    if (FinalFlatCancelDate.IsEmptyOrDefaultDiamondDate() == false)
                    {
                        DatesToChooseFrom.Add(FinalFlatCancelDate);
                    }
                    if (FinalCancelDate.IsEmptyOrDefaultDiamondDate() == false)
                    {
                        DatesToChooseFrom.Add(FinalCancelDate);
                    }
                    if (ManualCancelDate.IsEmptyOrDefaultDiamondDate() == false)
                    {
                        DatesToChooseFrom.Add(ManualCancelDate);
                    }

                    if (DatesToChooseFrom.Count > 0)
                    {
                        return DatesToChooseFrom.Max();
                    }
                    else
                    {
                        return PolicyCancelDate;
                    }
            }
        }

        public bool IsPayable
        {
            //Is direct Bill, Invoice has billable status, has some balance after any pending payments were to be applied
            get { return IsDirectBill && HasInvoiceBeenSent && (OutstandingBalance - PendingPayments.Sum((i) => { return i.PaymentAmount; }) > 0 || PayInFullAmount - PendingPayments.Sum((i) => { return i.PaymentAmount; }) > 0) && IsInPayableStatus; }
            set { }
        }

        public bool IsInPayableStatus
        {
            //Inforce,Future,Pending,New Business ??
            get { return IsDirectBill && PayablePolicyStatusIds.Contains(PolicyStatusId); }
            set { }
        }

        public double OutstandingBalance { get; set; } 
        //public double BillingAccountCurrentOutstanding { get; set; }

        public double PayInFullAmount { get; set; }

        public DateTime OutstandingDueDate { get; set; }

        public BillingItem LastPayment
        {
            get
            {
                if (this.BillingItems != null)
                {
                    var thisBillItem = (from b in this.BillingItems where b.ItemType == 2 && b.PaymentAmount < 0 && b.PolicyId == this.PolicyId select b).FirstOrDefault();
                    // dont show a credit as the past payment so must be less than zero to be a payment

                    if (thisBillItem != null && IsPartOfAccountBill)
                    {
                        var cds = cashDetails.FindAll(x => x.AddedDate == thisBillItem.ItemDate && x.PolicyId == thisBillItem.PolicyId);
                        thisBillItem.PaymentAmount = 0;
                        foreach (var cd in cds)
                        {
                            thisBillItem.PaymentAmount += Convert.ToDouble(cd.Amount);
                        }
                    }
                    return thisBillItem;
                }
                return null;
            }
            set { }
        }

        public bool HasInvoiceBeenSent { get; set; } = true;
        public double PriorOutstandingBalance { get; set; }

        public List<BillingItem> BillingItems { get; set; }

        public List<UnSweptPayment> PendingPayments { get; set; }

        public BillingAddress BillingAddress { get; set; }

        public List<FutureBillingActivityItem> FutureBillingActivity { get; set; }

        public double AccountOutstandingBalance { get; set; }
        public double AccountPayInFull { get; set; }
        public int AccountBillPriorityLevel { get; set; }
        public bool IsPrimaryAccountPolicy {get;set;}
        public bool IsPolicyInNoticeOfCancellation { get; set; }
        public bool IsRenewalStillFuture { get; set; }
        public bool IsRenewalImage { get; set; }
        public bool HasRenewalImage { get; set; }
        public bool HasUnpaidRenewal { get; set; }
        public decimal AmountToRenew { get; set; }
        public decimal DifferenceBetweenOutstandingDueAndAmountToRenew { get; set; }
        public bool UseDifferenceBetweenOutstandingAndRenewAmountForCurrentImage { get; set; }
        public bool HasCurrentImageAmountDue { get; set; }
        public bool HasCurrentImageFuturePrem { get; set; }
        public BillingInformation() { }

        internal BillingInformation(DCO.Policy.Image image, List<Int32> AllPolicyIds,int LobId)
        {

            if (image != null)
            {
                this.PolicyNumber = image.PolicyNumber;
                this.PolicyId = image.PolicyId;
                this.PolicyImageNumber = image.PolicyImageNum;
                this.TransactionExpirationDate = image.TransactionExpirationDate;
                this.TransactionEffectiveDate = image.TransactionEffectiveDate;
                this.PayPlanId = image.Policy.Account.BillingPayPlanId;
                this.PayPlan = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BillingPayPlanId, this.PayPlanId.ToString());
                if (this.PayPlan.HasValue() == false)
                {
                    Payments.GetCurrentPayPlanOptions ppOptions = new Payments.GetCurrentPayPlanOptions
                    {
                        PayPlanId = this.PayPlanId
                    };
                    var myPayPlan = global::IFM.DataServicesCore.BusinessLogic.Payments.PayPlanHelper.GetCurrentPayPlans(ppOptions).FirstOrDefault();
                    this.PayPlan = myPayPlan.BillingPayPlan;
                }
                this.PolicyCancelDate = image.Policy.CancelDate; // 04-07-2020 - Task 43795

                // 3-13-2019
                if (IsRCCPayPlanByName(this.PayPlan))
                {
                    using (var DS = Insuresoft.DiamondServices.BillingService.GetPolicyCreditCardInfo())
                    {
                        DS.RequestData.PolicyId = image.PolicyId;
                        this.PayplanDeductionDate = DS.Invoke()?.DiamondResponse?.ResponseData?.CreditCard?.DeductionDay ?? 0;
                    }
                }
                if (IsEFTPayPlanByName(this.PayPlan))
                {
                    var eftData = global::IFM.DataServicesCore.BusinessLogic.Payments.PayPlanHelper.GetEftInfo(this.PolicyId);
                    this.PayplanDeductionDate = eftData?.DeductionDay ?? 0;
                }

                this.IsPartOfAccountBill = image.Policy.BillingAccountId > 0 && IsAccountBillPayPlanByName(this.PayPlan);
                this.PolicyStatusId = image.PolicyStatusCodeId;
                if (this.IsPartOfAccountBill)
                    this.AccountBillPriorityLevel = BusinessLogic.OMP.Billing.GetAccountBillingPriorityLevel(LobId);



                using (var billingData = BusinessLogic.OMP.Billing.BillingData(image.PolicyId, IsPartOfAccountBill))
                {
                    if (billingData != null)
                    {
                        // if image.Policy.Account is null just let it die it is critical
                        //var myBillingData = image.Policy.BillingData; is always null
                        this.HasRecurringPaymentSetup = PayPlanIsRecurringByName(this.PayPlan);
                        //this.HasRecurringPaymentSetup = PayPlanIsRecurring(PayPlanId);
                        if (IsPartOfAccountBill)
                            this.AccountBillId = image.Policy.BillingAccountId;// billingData.Account.AccountNum;
                        this.BillToId = image.BillToId;
                        this.BillTo = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, this.BillToId.ToString());

                        this.BillingMethodId = image.Policy.Account.BillMethodId;
                        this.BillingMethod = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.BillMethodId, this.BillingMethodId.ToString());
                        this.IsDirectBill = image.Policy.Account.BillMethodId == DirectBillId;
                        this.WrittenPremium = Convert.ToDouble(image.WrittenPremium);

                        //this.IsRenewalStillFuture = PolicyHasFutureRenewal(billingData.Statements);
                        this.IsRenewalStillFuture = HasRenewalInFutures(billingData.Futures);

                        if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
                        {
                            using (var DS = Insuresoft.DiamondServices.BillingService.GetBillingSummary())
                            {
                                DS.RequestData.PolicyId = PolicyId;
                                if (image.Policy.BillingAccountId > 0)
                                {
                                    DS.RequestData.BillingAccountId = image.Policy.BillingAccountId;
                                }
                                var billingSummary = DS.Invoke()?.DiamondResponse?.ResponseData?.BillingSummary;
                                var billingRenewalVersion = billingSummary.Summary[0].RenewalVer;
                                var hasOutstandingAmount = billingSummary.Summary[0].CurrentOutstandingAmount > 0;
                                var hasFuturePrem = billingSummary.Summary[0].FuturePremiumAmount > 0;
                                HasUnpaidRenewal = billingSummary.Summary[0].UnpaidRenewal;
                                AmountToRenew = billingSummary.Summary[0].AmountToRenew;
                                DifferenceBetweenOutstandingDueAndAmountToRenew = billingSummary.Summary[0].CurrentOutstandingAmount - AmountToRenew;

                                if (image.RenewalVersion == billingRenewalVersion)
                                {
                                    //either current or Renewal Image
                                    IsRenewalImage = DateTime.Now <= this.TransactionEffectiveDate;
                                    HasRenewalImage = IsRenewalImage;
                                }
                                else
                                {
                                    if (billingRenewalVersion > image.RenewalVersion)
                                    {
                                        //Must be a current image with a Renewal
                                        HasRenewalImage = true;
                                        IsRenewalImage = false;
                                    }
                                    else
                                    {
                                        //Must be a current image with a Renewal
                                        HasRenewalImage = false;
                                        IsRenewalImage = false;
                                    }

                                }

                                if (this.IsRenewalImage && this.IsRenewalStillFuture)
                                    this.HasInvoiceBeenSent = false;

                                HasCurrentImageAmountDue = false;
                                if (hasOutstandingAmount)
                                {
                                    if ((HasRenewalImage == false || this.IsRenewalStillFuture))
                                    {
                                        HasCurrentImageAmountDue = true;
                                    }
                                    else if (AmountToRenew > 0)
                                    {
                                        if (DifferenceBetweenOutstandingDueAndAmountToRenew > 0)
                                        {
                                            HasCurrentImageAmountDue = true;
                                            UseDifferenceBetweenOutstandingAndRenewAmountForCurrentImage = true;
                                        }
                                    }
                                }
                                HasCurrentImageFuturePrem = false;
                                if (hasFuturePrem)
                                {
                                    if (HasRenewalImage == false || this.IsRenewalStillFuture || HasCurrentImageAmountDue)
                                    {
                                        HasCurrentImageFuturePrem = true;
                                    }
                                }

                                var FutureActivities = billingSummary.FutureActivities;
                                if (FutureActivities != null)
                                {
                                    this.FutureBillingActivity = new List<FutureBillingActivityItem>();
                                    foreach (var i in FutureActivities)
                                    {
                                        this.FutureBillingActivity.Add(new FutureBillingActivityItem(i));
                                    }
                                    this.FutureBillingActivity = (from b in this.FutureBillingActivity orderby b.DueDate ascending select b).ToList();
                                }
                            }
                        }

                        if (IsPartOfAccountBill)
                            this.cashDetails = billingData.CashDetails.ToList();

                        this.BillingItems = BusinessLogic.OMP.Billing.BillingStatements(billingData, IsPartOfAccountBill,image.PolicyImageNum);

                        if (AllPolicyIds != null && AllPolicyIds.Count > 1)
                        {
                            foreach (var pId in AllPolicyIds)
                            {
                                // don't re-pull current billing items
                                if (pId != image.PolicyId)
                                {
                                    var otherBillingItems = BusinessLogic.OMP.Billing.BillingStatements(BusinessLogic.OMP.Billing.BillingData(pId, false), IsPartOfAccountBill,image.PolicyImageNum);
                                    if (otherBillingItems != null)
                                        this.BillingItems.AddRange(otherBillingItems);
                                }
                            }
                        }

                        //Check if FinalFlatCancelDate exists in Futures. Not sure if this always exists, but there are definitely cases where this is more accurate than image.Policy.CancelDate
                        if (billingData.Futures?.IsLoaded() == true)
                        {
                            var cancelItems = billingData.Futures.Where(item => new List<int> { 1, 2, 3, 13 }.Contains(item.BillingNoticeTypeId));
                            foreach (var myFuture in cancelItems)
                            {
                                switch (myFuture.BillingNoticeTypeId)
                                {
                                    case 1:
                                        LegalCancelNoticeDate = myFuture.DueDate.DateTime;
                                        break;
                                    case 2:
                                        FinalCancelDate = myFuture.DueDate.DateTime;
                                        break;
                                    case 3:
                                        ManualCancelDate = myFuture.DueDate.DateTime;
                                        break;
                                    case 13:
                                        FinalFlatCancelDate = myFuture.DueDate.DateTime;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        this.BillingAddress = new BillingAddress(image.BillingAddressee);

                        this.CarryDate = billingData.CarryDate;

                        if (AppConfig.OMP_RemoveFurtureImage_Billing_from_currentImage)
                        {
                            if (billingData.Invoice != null && billingData.Invoice.Any())
                            {
                                DCO.Billing.Invoice invoice = null;
                                if (IsPartOfAccountBill)
                                {
                                    invoice = billingData.Invoice.Find(x => x.PolicyId == this.PolicyId);
                                }
                                else
                                {
                                    invoice = billingData.Invoice[0];
                                }
                                
                                if (invoice != null && (invoice.DueDate.DateTime >= image.EffectiveDate.DateTime))
                                {
                                    if (billingData.CarryDate <= image.ExpirationDate && (IsPartOfAccountBill || ((IsRenewalImage && HasUnpaidRenewal && IsRenewalStillFuture == false) || (IsRenewalImage == false && (HasCurrentImageAmountDue || HasCurrentImageFuturePrem)))))
                                    {
                                       //this.PayInFullAmount = Convert.ToDouble(invoice.FuturePremiumAmount + invoice.CurrentOutstandingAmount);
                                       //this.PayInFullAmount = Convert.ToDouble(invoice.CurrentOutstandingAmount);
                                       // DCO.Billing.Future _future = null;

                                       // if (billingData.Futures != null && billingData.Futures.Any())
                                       // {
                                       //     _future = billingData.Futures[0];
                                       // }
                                       // if ((IsRCCPayPlanByName(this.PayPlan) || IsEFTPayPlanByName(this.PayPlan)) && _future != null && image.ExpirationDate > _future.DueDate)
                                       // {
                                            this.PayInFullAmount = Convert.ToDouble(invoice.FuturePremiumAmount + invoice.CurrentOutstandingAmount);
                                        //}
                                        //else
                                        //{
                                        //    this.PayInFullAmount = Convert.ToDouble(invoice.CurrentOutstandingAmount);
                                        //}

                                        if (IsPartOfAccountBill)
                                        {
                                            decimal accountOutstanding = 0;
                                            decimal accountPayInFull = 0;
                                            foreach (var p in billingData.BillingAccountPolicyLinks)
                                            {
                                                accountOutstanding += (from i in billingData.Invoice where p.PolicyId == i.PolicyId select i.CurrentOutstandingAmount).Sum();
                                                accountPayInFull += (from i in billingData.Invoice where p.PolicyId == i.PolicyId select i.FuturePremiumAmount + i.CurrentOutstandingAmount).Sum();
                                            }
                                            this.AccountPayInFull = Convert.ToDouble(accountPayInFull);
                                            this.AccountOutstandingBalance = (AccountPayInFull > 0) ? Convert.ToDouble(accountOutstanding) : 0.0;
                                        }
                                    }
                                    else
                                    {
                                        this.PayInFullAmount = 0.0;
                                    }

                                    this.OutstandingBalance = PayInFullAmount > 0 ? invoice.CurrentOutstandingAmount.TryToGetDouble() : 0.0;

                                    if (UseDifferenceBetweenOutstandingAndRenewAmountForCurrentImage)
                                    {
                                        if (invoice.CurrentOutstandingAmount - AmountToRenew == DifferenceBetweenOutstandingDueAndAmountToRenew)
                                        {
                                            this.OutstandingBalance = PayInFullAmount > 0 ? DifferenceBetweenOutstandingDueAndAmountToRenew.TryToGetDouble() : 0.0;
                                        }
                                    }

                                    if (this.PolicyStatusId == 2)
                                    {
                                       // this.PayInFullAmount = Convert.ToDouble(invoice.FuturePremiumAmount + invoice.CurrentOutstandingAmount);

                                        //WS-1394 CPP1019776 - Is displaying incorrect amount due on OneView & Member Portal
                                        if (AmountToRenew > 0 && UseDifferenceBetweenOutstandingAndRenewAmountForCurrentImage) this.OutstandingBalance = Convert.ToDouble(AmountToRenew);
                                        if (billingData != null && billingData.Futures != null && billingData.Futures.Any() && billingData.Futures[0].Description.Contains("Renewal Installment 1"))
                                        {
                                            this.OutstandingBalance = Convert.ToDouble(billingData.Futures[0].Amount);
                                        }
                                    }

                                    this.OutstandingDueDate = invoice.DueDate;
                                    this.PriorOutstandingBalance = Convert.ToDouble(invoice.PriorOutstandingAmount);

                                }
                                else
                                {
                                    HasInvoiceBeenSent = false; //Appears to be a future image in which the invoice has not actually billed for the future. Do not present this as billable 
                                    //this.PayInFullAmount = Convert.ToDouble(invoice.FuturePremiumAmount);

                                    //if (billingData.Futures?.IsLoaded() == true)
                                    //{
                                    //    var renewalInstallments = billingData.Futures.Where(b => b.DueDate.DateTime >= image.EffectiveDate.DateTime).ToList();
                                    //    //FindFutureRenewalInstallments(billingData.Futures);
                                    //    if (renewalInstallments.IsLoaded())
                                    //    {
                                    //        var firstRenewalInstallment = renewalInstallments.First();
                                    //        this.OutstandingDueDate = firstRenewalInstallment.DueDate.DateTime;
                                    //        //this.OutstandingBalance = Convert.ToDouble(firstRenewalInstallment.Amount + invoice.CurrentOutstandingAmount);
                                    //        this.OutstandingBalance = Convert.ToDouble(firstRenewalInstallment.Amount);
                                    //        //this.PayInFullAmount = Convert.ToDouble(invoice.FuturePremiumAmount + invoice.CurrentOutstandingAmount);
                                    //    }
                                    //}
                                }
                            }

                            if (HasInvoiceBeenSent == true && IsPartOfAccountBill == false)
                            {
                                if (billingData.AccountReceivables.IsLoaded())
                                {
                                    if (billingData.CarryDate < image.ExpirationDate)
                                    {
                                        this.AccountOutstandingBalance = (this.AccountPayInFull > 0) ? Convert.ToDouble((from b in billingData.AccountReceivables select b.TotalOutstanding).Sum()) : 0.0;
                                        this.AccountPayInFull = Convert.ToDouble((from b in billingData.AccountReceivables select b.FuturePremiumOutstanding).Sum() + Convert.ToDecimal(this.AccountOutstandingBalance)); //Not sure if this would be use outside of account bill...
                                    }
                                    else
                                    {
                                        this.AccountPayInFull = 0.0;
                                        this.AccountOutstandingBalance = 0.0;
                                    }
                                }
                            }
                            
                        }
                        else
                        {
                            if (billingData.Invoice != null && billingData.Invoice.Any())
                            {
                                Invoice invoice = null;
                                if (IsPartOfAccountBill)
                                {
                                    invoice = billingData.Invoice.Find(x => x.PolicyId == this.PolicyId);
                                }
                                else
                                {
                                    invoice = billingData.Invoice[0];
                                }

                                if (invoice != null && (invoice.DueDate.DateTime >= image.EffectiveDate.DateTime))
                                {
                                    this.PayInFullAmount = Convert.ToDouble(invoice.FuturePremiumAmount + invoice.CurrentOutstandingAmount);
                                    this.OutstandingBalance = Convert.ToDouble(invoice.CurrentOutstandingAmount);
                                    //this.BillingAccountCurrentOutstanding = Convert.ToDouble(invoice.BillingAccountCurrentOutstanding); // is not account bill balance - probably unneeded
                                    this.OutstandingDueDate = invoice.DueDate;
                                    this.PriorOutstandingBalance = Convert.ToDouble(invoice.PriorOutstandingAmount);

                                    if (IsPartOfAccountBill)
                                    {
                                        decimal accountOutstanding = 0;
                                        decimal accountPayInFull = 0;
                                        foreach (var p in billingData.BillingAccountPolicyLinks)
                                        {
                                            accountOutstanding += (from i in billingData.Invoice where p.PolicyId == i.PolicyId select i.CurrentOutstandingAmount).Sum();
                                            accountPayInFull += (from i in billingData.Invoice where p.PolicyId == i.PolicyId select i.FuturePremiumAmount + i.CurrentOutstandingAmount).Sum();
                                        }
                                        this.AccountPayInFull = Convert.ToDouble(accountPayInFull);
                                        this.AccountOutstandingBalance = (AccountPayInFull > 0) ? Convert.ToDouble(accountOutstanding) : 0.0;
                                    }
                                }
                                else
                                {
                                    HasInvoiceBeenSent = false;
                                }
                            }

                            if (HasInvoiceBeenSent == true && IsPartOfAccountBill == false)
                            {
                                if (billingData.AccountReceivables.IsLoaded())
                                {
                                    this.AccountOutstandingBalance = Convert.ToDouble((from b in billingData.AccountReceivables select b.TotalOutstanding).Sum());
                                    this.AccountPayInFull = Convert.ToDouble((from b in billingData.AccountReceivables select b.FuturePremiumOutstanding).Sum()) + this.AccountOutstandingBalance;
                                }
                            }
                        }

                        // 04-07-2020 - Task 43795
                        if (this.CancelDate.HasValue() && this.CancelDate < this.OutstandingDueDate)
                            this.OutstandingDueDate = this.CancelDate;

                        PendingPayments = UnSweptPayment.GetPendingPaymentList(PolicyNumber);

                        bool hasDbError = false;
                        IsPolicyInNoticeOfCancellation = new CommonHelperClass().IsPolicyInNoticeOfCancellation(this.PolicyNumber, this.PolicyId, ref hasDbError);
                    }
                }
            }
            else
            {
#if !DEBUG
                global::IFM.IFMErrorLogging.LogIssue($"Image was null.", "IFMDATASERVICES -> BillingInformation.cs -> Function BillingInformation");
#else
                Debugger.Break();
#endif
            }
        }

        private bool HasRenewalInFutures(DCO.InsCollection<Future> Futures)
        {
            return Futures.Where(x => IsFutureItemRenewalInstallment(x) == true).Count() > 0;
        }

        private bool IsFutureItemRenewalInstallment(DCO.Billing.Future FutureItem)
        {
            if (FutureItem != null)
            {
                string renewalInstallment = "Renewal Installment 1";
                if (FutureItem.Description.StartsWith(renewalInstallment, StringComparison.OrdinalIgnoreCase) && FutureItem.DueDate.DateTime.IsEmptyOrDefaultDiamondDate() == false)
                {
                    return true;
                }
            }
            return false;
        }
        //This invoice object does not have policyImageNum for us to verify if it belongs to the current or future image of a policy.
        //Using this function to compare the invoice info against statement detail so that we can determine the policyImageNum associated with the invoice
        //Knowing the policyImageNum, we can determine if the invoice belongs to the current image being loaded.
        private bool IsInvoiceInBillingDataStatements(Invoice invoice, Data billingData)
        {
            return (IsInvoiceMatchedInThisImagesStatement(invoice, billingData.Statements)); //|| IsInvoiceMatchedInThisImagesStatement(invoice, billingData.StatementDetails));
        }

        private bool IsInvoiceMatchedInThisImagesStatement(Invoice invoice, DCO.InsCollection<Statement> statements)
        {
            if (invoice != null && statements != null)
            {
                //Was checking Outstanding balance amounts but there are so many things that could mess with that... lets just see if something is due on this date and what image it is for.
                var statementDueDateItem = statements.Find(x => x.PolicyId == PolicyId && x.PolicyImageNum == PolicyImageNumber && x.DueDate.DateTime == invoice.DueDate.DateTime);
                var latestStatementItems = statements.Where(x => x.PolicyId == PolicyId && x.PCAddedDate >= statementDueDateItem.PCAddedDate && x.Balance == invoice.CurrentOutstandingAmount).ToList(); //.Aggregate((x1, x2) => x1.BillingActivityOrder > x2.BillingActivityOrder ? x1 : x2);
                Statement latestStatementItem = null;
                if (latestStatementItems != null && latestStatementItems.Count > 0)
                {
                    latestStatementItem = latestStatementItems.Last();
                }
                
                if (statementDueDateItem != null && latestStatementItem != null)
                {
                    //if (IsInvoiceEndorsementPremiumChange(invoice, statement))
                    //{
                    //    return statement.Balance == invoice.CurrentOutstandingAmount;
                    //}
                    //else
                    //{
                    //    return true;
                    //}
                    return true;
                }
            }
            return false;
        }

        //private bool IsInvoiceEndorsementPremiumChange(Invoice invoice, Statement statement)
        //{
        //    if (invoice != null && statement != null && statement.Description.Contains("Endorsement Premium Eff"))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        private bool PolicyHasFutureRenewal(DCO.InsCollection<Statement> statements)
        {
            var statement = statements.Find(x => x.TransactionEffectiveDate == TransactionEffectiveDate && x.TransactionEffectiveDate.DateTime > DateTime.Now && x.PolicyId == PolicyId && x.PolicyImageNum == PolicyImageNumber && x.Description.Contains("Renewal Installment 1"));
            if(statement != null)
            {
                return true;
            }
            return false;
        }

        public static bool PayPlanIsRecurringByName(string PayPlanName)
        {
            return IsEFTPayPlanByName(PayPlanName) || IsRCCPayPlanByName(PayPlanName);
        }

        public static bool PayPlanIsRecurring(int PayPlanId)
        {
            return RecurringPayPlanIds.Contains(PayPlanId);
        }

        public static bool IsAccountBillPayPlanByName(string PayPlanName)
        {
            return PayPlanName.Contains("Account Bill");
        }

        public static bool IsAccountBillPayPlan(int PayPlanId)
        {
            return AccountBillPayPlanIds.Contains(PayPlanId);
        }

        public static bool IsRCCPayPlanByName(string PayPlanName)
        {
            return PayPlanName.Contains("Credit Card Monthly");
        }

        public static bool IsRCCPayPlan(int PayPlanId)
        {
            return RCCPayPlanIds.Contains(PayPlanId);
        }

        public static bool IsEFTPayPlanByName(string PayPlanName)
        {
            return PayPlanName.Contains("EFT Monthly");
        }

        public static bool IsEFTPayPlan(int PayPlanId)
        {
            return REFTPayPlanIds.Contains(PayPlanId);
        }
        public override string ToString()
        {
            return $"PolNum: {this.PolicyNumber} IsPayable: {this.IsPayable} Outstanding: {this.OutstandingBalance} Outstanding Due Date: {this.OutstandingDueDate} IsAccountBill: {this.IsPartOfAccountBill} IsPrimaryAccount: {this.IsPrimaryAccountPolicy}";
        }

    }
}