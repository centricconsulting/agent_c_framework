using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Mapster;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class PrintDistribution
    {
        public string PolicyNumber { get; set; }
        public int PolicyId { get; set; }
        public bool CreditCardDeclinedNotification { get; set; }
        public bool CreditCardExpiredNotification { get; set; }
        public string EmailAddress { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool LegalNoticeNotification { get; set; }
        public string LoginName { get; set; }
        public bool NSFNotification { get; set; }
        public int NumberOfDays { get; set; }
        public bool PaymentPostedNotification { get; set; }
        public bool PaymentReminderNotification { get; set; }
        public int PolicyPrintDistributionId { get; set; }
        public int PrintDistributionTypeId { get; set; }
        public bool SendEmails { get; set; }
        public bool SendTexts { get; set; }
        public string StatusDescription { get; set; }
        public string TextPhoneNumber { get; set; }
        public int ClaimPrintDistributionId { get; set; }
        public string ClaimEmailAddress { get; set; }
        public string Response { get; set; }
        public string ErrorMessage { get; set; }

        public DCO.Printing.PolicyPrintDistribution ToDiamondPrintDistibution()
        {
            var dp = new DCO.Printing.PolicyPrintDistribution();
            //return this.Adapt<DCO.Printing.PolicyPrintDistribution>(); //Appears to be the only thing in the whole project using mapster... Lets just switch this to manual mapping and get rid of the mapster dependency.

            dp.PolicyId = this.PolicyId;
            dp.CreditCardDeclinedNotification = this.CreditCardDeclinedNotification;
            dp.CreditCardExpiredNotification = this.CreditCardExpiredNotification;
            dp.EmailAddress = this.EmailAddress;
            dp.LastModifiedDate = this.LastModifiedDate;
            dp.LegalNoticeNotification = this.LegalNoticeNotification;
            dp.LoginName = this.LoginName;
            dp.NSFNotification = this.NSFNotification;
            dp.NumberOfDays = this.NumberOfDays;
            dp.PaymentPostedNotification = this.PaymentPostedNotification;
            dp.PaymentReminderNotification = this.PaymentReminderNotification;
            dp.PolicyPrintDistributionId = this.PolicyPrintDistributionId;
            dp.PrintDistributionTypeId = this.PrintDistributionTypeId;
            dp.SendEmails = this.SendEmails;
            dp.SendTexts = this.SendTexts;
            //dp.StatusDescription = this.StatusDescription; //property is read only on Diamond's side
            dp.TextPhoneNumber = this.TextPhoneNumber;
            dp.ClaimPrintDistributionId = this.ClaimPrintDistributionId;
            dp.ClaimEmailAddress = this.ClaimEmailAddress;
            return dp;
        }

        public PrintDistribution() { }
        public PrintDistribution(DCO.Printing.PolicyPrintDistribution dPrintDistribution)
        {
            if (dPrintDistribution != null)
            {
                this.PolicyId = dPrintDistribution.PolicyId;
                this.CreditCardDeclinedNotification = dPrintDistribution.CreditCardDeclinedNotification;
                this.CreditCardExpiredNotification = dPrintDistribution.CreditCardExpiredNotification;
                this.EmailAddress = dPrintDistribution.EmailAddress;
                this.LastModifiedDate = dPrintDistribution.LastModifiedDate;
                this.LegalNoticeNotification = dPrintDistribution.LegalNoticeNotification;
                this.LoginName = dPrintDistribution.LoginName;
                this.NSFNotification = dPrintDistribution.NSFNotification;
                this.NumberOfDays = dPrintDistribution.NumberOfDays;
                this.PaymentPostedNotification = dPrintDistribution.PaymentPostedNotification;
                this.PaymentReminderNotification = dPrintDistribution.PaymentReminderNotification;
                this.PolicyPrintDistributionId = dPrintDistribution.PolicyPrintDistributionId;
                this.PrintDistributionTypeId = dPrintDistribution.PrintDistributionTypeId;
                this.SendEmails = dPrintDistribution.SendEmails;
                this.SendTexts = dPrintDistribution.SendTexts;
                this.StatusDescription = dPrintDistribution.StatusDescription;
                this.TextPhoneNumber = dPrintDistribution.TextPhoneNumber;
                this.ClaimPrintDistributionId = dPrintDistribution.ClaimPrintDistributionId;
                this.ClaimEmailAddress = dPrintDistribution.ClaimEmailAddress;
            }
        }
    }
}
