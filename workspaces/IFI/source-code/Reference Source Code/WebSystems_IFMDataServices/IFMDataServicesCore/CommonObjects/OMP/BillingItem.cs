using System;
using Diamond.Common.Objects.Billing;
using IFM.PrimitiveExtensions;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class BillingItem : ModelBase
    {
        public Int32 ItemType { get; set; }// 0 = Invoice, Notice =1, Payment = 2

        //All
        public double Balance { get; set; }

        public string Description { get; set; }
        public Int32 PolicyId { get; set; }
        public Int32 PolicyImageNum { get; set; }
        public Int32 PrintProcessId { get; set; }
        public Int32 ItemNumber { get; set; }
        public DateTime ItemDate { get; set; }
        public string PrintDescription { get; set; }
        public Int32 BillingGroupNumber { get; set; }
        public Int32 BillingActivityOrder { get; set; }

        //Invoices
        public double BilledAmount { get; set; }

        public DateTime DueDate { get; set; }

        //payments
        public double PaymentAmount { get; set; }
        public double AccountPaymentAmount { get; set; }

        public string PaymentType { get; set; }

        public string RenewalVer { get; set; }
        public int StatusCode { get; set; }

        public string PrintUrl
        {
            //If Description.IsNullEmptyOrWhitespace = False AndAlso PrintProcessId > 0 Then
            //    Return "Print.ashx?policyId={0}&printProcessId={1}&description={2}".FormatIFM(PolicyId, PrintProcessId, System.Web.HttpUtility.UrlEncode(Description))
            //End If
            get { return null; }
            set { }
        }

        public string CurrentPolicyNumber { get; set; }

        public DateTime? PCAddedDate { get; set; }

        public BillingItem() { }

        public BillingItem(DCO.Billing.Statement dStatement)
        {
            // if dStatement is null let it die.
            this.Balance = Convert.ToDouble(dStatement.Balance);
            this.Description = dStatement.Description;

            this.PolicyId = dStatement.PolicyId;
            this.PolicyImageNum = dStatement.PolicyImageNum;
            this.PrintProcessId = dStatement.PrintProcessId;
            this.PrintDescription = dStatement.PrintDescription;
            this.BillingGroupNumber = dStatement.BillingGroupingNum;
            this.BillingActivityOrder = dStatement.BillingActivityOrder;
            this.ItemDate = dStatement.TransDate;
            this.CurrentPolicyNumber = dStatement.CurrentPolicy;
            this.PCAddedDate = dStatement.PCAddedDate;
            this.RenewalVer = dStatement.RenewalVer;
            this.StatusCode = dStatement.StatusCode;
            switch (dStatement.Source.ToUpper())
            {
                case "INV":
                    this.ItemType = 0;
                    this.ItemNumber = dStatement.BillingChargeCreditNum;
                    this.DueDate = dStatement.DueDate;
                    this.BilledAmount = dStatement.BilledAmount.TryToGetDouble();
                    break;

                case "REC":
                    this.ItemType = 2;
                    if (dStatement.PrintDescription.ToUpper() == "Policy Submission".ToUpper())
                    {
                        this.Description = dStatement.PrintDescription;
                    }
                    if (dStatement.Description.ToUpper() != "Policy Submission".ToUpper() && dStatement.PrintDescription.ToUpper() != "Policy Submission".ToUpper())
                    {
                        if (dStatement.Description.ToUpper() != "Premium".ToUpper())
                        {
                            if (dStatement.BillingAccountPayment)
                            {
                                this.Description = "Account Bill Payment";
                                this.PaymentAmount = dStatement.PaidAmount.TryToGetDouble();
                                this.AccountPaymentAmount = dStatement.PaidAmount.TryToGetDouble();
                            }
                            else
                            {
                                this.Description = "Payment";
                                this.PaymentAmount = dStatement.PaidAmount.TryToGetDouble();
                            }
                            
                        }
                        this.ItemNumber = dStatement.BillingCashNum;
                        this.PaymentType = dStatement.Type;
                    }

                    break;

                case "NOT":
                    this.ItemType = 1;
                    this.ItemNumber = dStatement.BillingNoticeNum;
                    break;
            }
        }

        public override string ToString()
{
            return $"PCDATE: {this.PCAddedDate} Activity Order: {this.BillingActivityOrder} ItemNum: {this.ItemNumber} PolId: {this.PolicyId} Trans Date: {this.ItemDate} Desc: {this.Description} Type:{this.PaymentType} Due: {this.DueDate} Bill: {this.BilledAmount} Paid: {this.PaymentAmount} Balance: {this.Balance}";
        }


    }
}