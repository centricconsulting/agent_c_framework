using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFM.DataServicesCore.CommonObjects.Fiserv
{
    [System.Serializable]
    public class PaymentSettings : ModelBase
    {
        public bool AllowScheduledPayments { get; set; } //determines whether or not scheduled payments can be added
        public bool AllowSplitPayments { get; set; } //determines whether or not payments can be split between different payment methods
        public bool AllowUseOfWalletItems { get; set; } //determines whether or not wallet items can be added
        public string FiservApiKey { get; set; } //needed in call to Fiserv iframe
        public string FiservBankIframeUrl { get; set; } //url to use in Fiserv iframe for Bank info; we won't be using this
        public string FiservCardIframeUrl { get; set; }  //url to use in Fiserv iframe for Card info
        public bool FiservUseIframeForCreditCardInfo { get; set; } //determines whether or not we should use Fiserv's iframe to collect Card info; should use the current fields when set to False
        public bool FiservUseIframeForRecurringCreditCardInfo { get; set; } //determines whether or not we should use Fiserv's iframe to collect Card info used for RCC (Recurring Credit Card) accounts; should use the current fields when set to False
        public int NumberOfPaymentMethodsToAllowOnSplitPayments { get; set; } //only used if AllowSplitPayments is true; 0 means unlimited; any other value > 0 would represent the max # of payment methods to allow
        public bool CollectSecurityCodeForCreditCardPayments { get; set; } //determines whether or not we need to capture the security code for Credit Card Payments; not needed if using Fiserv's iframe according to FiservUseIframeForCreditCardInfo
        public bool CollectZipCodeForCreditCardPayments { get; set; } //determines whether or not we need to capture the zip code for Credit Card Payments; not needed if using Fiserv's iframe according to FiservUseIframeForCreditCardInfo
        public bool CollectSecurityCodeForRecurringCreditCardPayments { get; set; } //determines whether or not we need to capture the security code for Recurring Credit Card Payments; not needed if using Fiserv's iframe according to FiservUseIframeForRecurringCreditCardInfo
        public bool CollectZipCodeForRecurringCreditCardPayments { get; set; } //determines whether or not we need to capture the security code for Recurring Credit Card Payments; not needed if using Fiserv's iframe according to FiservUseIframeForRecurringCreditCardInfo
        public bool SendCreditCardPaymentConfirmationEmails { get; set; } //determines whether or not UI should generate payment confirmation email on successful CC payment; would only be False if we're using Fiserv for CC payments and we're using Fiserv emails
        public bool SendEcheckPaymentConfirmationEmails { get; set; } //determines whether or not UI should generate payment confirmation email on successful eCheck payment; would only be False if we're using Fiserv for eCheck payments and we're using Fiserv emails
        public bool SendWalletPaymentConfirmationEmails { get; set; } //determines whether or not UI should generate payment confirmation email on successful wallet payment; would only apply when AllowUseOfWalletItems = true; would only be False if we're using Fiserv emails
        public int NumberOfPoliciesToAllowOnSplitPayments { get; set; } //only used if AllowSplitPayments is true; 0 means unlimited; any other value > 0 would represent the max # of policies to allow
        public bool UseTemporaryWalletItemToPayMultiplePoliciesWithSameCard { get; set; } //determines whether or not we need to create temporary wallet items when paying multiple policies with the same card info; would only apply when FiservUseIframeForCreditCardInfo = True
        public string EmailAddressToUseForTemporaryCardWalletItemUsedToPayMultiplePolicies { get; set; } //the email to use for the Fiserv session used to generate the fundingAccountToken; would also be the KeyIdentifier to use for the Temporary WalletItem; only applies when UseTemporaryWalletItemToPayMultiplePoliciesWithSameCard = True
        public decimal MaximumVendorPaymentAmount { get; set; } //determines the maximum payment amount allowed by our third party CC and eCheck vendor.
        public bool CalculateFeeAmountOnCreditCardPayments { get; set; } //added 9/25/2024
        public string CalculateFeeAmountOnCreditCardPayments_Date { get; set; } //added 9/25/2024
        public string RestrictCreditCardsForRecurringPayments_Date { get; set; } //added 11/05/2024

    }
}
