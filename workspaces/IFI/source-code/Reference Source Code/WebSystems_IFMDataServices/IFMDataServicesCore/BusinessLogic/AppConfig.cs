using System;
using System.Collections;
using System.Collections.Generic;
using IFM.PrimitiveExtensions;

namespace IFM.DataServicesCore.BusinessLogic
{
    public static class AppConfig
    {
        private static IEnumerable<string> SplitConfigList(string key)
        {
            // let it die if anything goes wrong just like it would if you attempted to pull a value from a key that didn't exist
            var val = System.Configuration.ConfigurationManager.AppSettings[key];
            if (val.Contains("|"))
                return val.Split('|');
            else if (val.Contains(","))
                return val.Split(',');
            else
                return new List<string>() { val };
        }

        public static string Conn
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["conn"]; }
        }
        public static string ConnFNOL
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["connFNOL"]; }
        }
        public static string ConnQQ
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["connQQ"]; }
        }

        public static string ConnDiamond
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["connDiamond"]; }
        }

        public static string ConnDiamondReports
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["connDiamondReports"]; }
        }

        public static string ConnMemberPortal//added 3/26/2018
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["connMemberPortal"]; }
        }

        public static string PrintUserName
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["ompDiamondUserName"]; }
        }

        public static string PrintUserPassword
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["ompDiamondUserPassword"]; }
        }

        public static string EndorsementUsername
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["ompEndorsementUserName"]; }
        }

        public static string EndorsementPassword
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["ompEndorsementUserPassword"]; }
        }

        public static string ClaimSubmittedEmailNotificationEmail
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["ompClaimSubmittedEmailNotificationEmail"]; }
        }

        public static DateTime FiservBillingFormsStartDate
        {
            get 
            {
                string stringDate = System.Configuration.ConfigurationManager.AppSettings["Fiserv_BillingFormsStartDate"];
                return (stringDate.IsNullEmptyOrWhitespace() == false && stringDate.IsDate(out var myDate)) ? myDate : DateTime.MinValue;
            }
        }

        public static string ErrorEmailAddress
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["errorEmail"]; }
        }

        public static string NoReplyEmailAddress
        {
            get { return "noreply@indianafarmers.com"; }
        }

        public static IEnumerable<string> HomeOfficeAgencyCodes
        {
            get{return SplitConfigList("ompHomeOfficeAgencyCodes");}
        }

        public static string HomeOfficeAgencyEmailAddress
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["ompHomeOfficeAgencyEmailAddress"]; }
        }

        public static string HomeOfficeAgencyWebSiteAddress
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["ompHomeOfficeAgencyWebSiteAddress"]; }
        }

        public static string HomeOfficeAgencyPhoneNumber
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["ompHomeOfficeAgencyPhoneNumber"]; }
        }

        public static bool OMP_RemoveFurtureImage_Billing_from_currentImage
        {
            get
            {
                var configValue = System.Configuration.ConfigurationManager.AppSettings["OMP_RemoveFurtureImage_Billing_from_currentImage"];
                bool.TryParse(configValue, out var result);
                return result; 
            }
        }
        //public static string DiamondClaimsFNOL_ReportedDate_UseLossDate
        //{
        //    get { return System.Configuration.ConfigurationManager.AppSettings["DiamondClaimsFNOL_ReportedDate_UseLossDate"]; }
        //}
        public static bool DiamondClaimsFNOL_ReportedDate_UseLossDate
        {
            get
            {
                CommonHelperClass chc = new CommonHelperClass();
                Boolean keyExists = false;
                return chc.ConfigurationAppSettingValueAsBoolean("DiamondClaimsFNOL_ReportedDate_UseLossDate", ref keyExists);
            }
        }
        public static string DiamondClaimsFNOL_OmitInsuredClaimants
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["DiamondClaimsFNOL_OmitInsuredClaimants"]; }
        }

        public static string FNOLInsideAdjusterID
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["FNOLInsideAdjusterID"]; }
        }
        public static string FNOLAdministratorID
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["FNOLAdministratorID"]; }
        }
        public static string FNOLClaimOfficeID
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["FNOLClaimOfficeID"]; }
        }
        public static string TestOrProd
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["TestOrProd"]; }
        }
        public static string DECFolder
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["DECFolder"];}
        }
        public static string FNOLCA_CSU_Email
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["FNOLCA_CSU_Email"]; }
        }
        public static string FNOLClaimAssign_LossReportingEmailAddress
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["FNOLClaimAssign_LossReportingEmailAddress"]; }
        } 
        public static string FNOLCA_SendCopyOfAssignmentEmail
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["FNOLCA_SendCopyOfAssignmentEmail"]; }
        }
        public static string FNOLCA_CopyOfAssignment_EmailAddress
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["FNOLCA_CopyOfAssignment_EmailAddress"]; }
        } 
        public static string FNOLCA_EnableCATThirdPartyEmails
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["FNOLCA_EnableCATThirdPartyEmails"]; }
        }
        public static string mailhost
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["mailhost"]; }
        }
        public static string IFMDataServices_EndPointBaseUrl
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["IFMDataServices_EndPointBaseUrl"]; }
        }
        public static string LossReportingErrorEmail
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["LossReportingErrorEmail"]; }
        }
        public static string PolicyInquiryAPIEndpoint
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["PolicyInquiryAPIEndpoint"]; }
        }
        public static string DefaultExcludeFromBankFileOption
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["DefaultExcludeFromBankFileOption"]; }
        }
        public static string AutomaticallyPostDiaPayments
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["AutomaticallyPostDiaPayments"]; }
        }
        public static string UseAutomaticPostTimes
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["UseAutomaticPostTimes"]; }
        }
        public static string AutomaticPostStartTime
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["AutomaticPostStartTime"]; }
        }
        public static string AutomaticPostEndTime
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["AutomaticPostEndTime"]; }
        }
        public static string CreateEftAccountOnExcludeFromBankFile
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["CreateEftAccountOnExcludeFromBankFile"]; }
        }
        public static string UseRAPAApiForVehicleSymbolLookup
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["UseRAPAApiForVehicleSymbolLookup"]; }
        }
        public static bool UseOtherCancellationStatusesInsteadOfPolicyCancelDateWhenApplicable
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["UseOtherCancellationsStatusInsteadOfPolicyCancelDateWhenApplicable"]?.TryToGetBoolean() ?? false; }
        }
        public static string SnapLogicEodUrl
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["SnapLogicEodUrl"]?.TryToGetString() ?? ""; }
        }

        public static bool DiamondClaimsFNOL_SetLnImageNoteProps
        {
            get
            {
                CommonHelperClass chc = new CommonHelperClass();
                Boolean keyExists = false;
                return chc.ConfigurationAppSettingValueAsBoolean("DiamondClaimsFNOL_SetLnImageNoteProps", ref keyExists);
            }
        }
        public static bool DiamondClaimsFNOL_SetLnImageNoteProps_OnlyIfKeyExists
        {
            get
            {
                CommonHelperClass chc = new CommonHelperClass();
                Boolean keyExists = false;
                return chc.ConfigurationAppSettingValueAsBoolean("DiamondClaimsFNOL_SetLnImageNoteProps_OnlyIfKeyExists", ref keyExists);
            }
        }
        public static string DiamondClaimsFNOL_LnImageNotesDefault(ref bool keyExists)
        {
            CommonHelperClass chc = new CommonHelperClass();
            return chc.ConfigurationAppSettingValueAsString("DiamondClaimsFNOL_LnImageNotesDefault", ref keyExists);
        }
        public static int DiamondClaimsFNOL_LnImageNotesTypeIdDefault(ref bool keyExists)
        {
            CommonHelperClass chc = new CommonHelperClass();
            return chc.ConfigurationAppSettingValueAsInteger("DiamondClaimsFNOL_LnImageNotesTypeIdDefault", ref keyExists);
        }
        public static string DiamondClaimsFNOL_LnImageNoteTitleDefault(ref bool keyExists)
        {
            CommonHelperClass chc = new CommonHelperClass();
            return chc.ConfigurationAppSettingValueAsString("DiamondClaimsFNOL_LnImageNoteTitleDefault", ref keyExists);
        }

        public static string RestrictCreditCardsForRecurringPayments_Date
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["RestrictCreditCardsForRecurringPayments_Date"]; }
        }

        public static string VR_PPA_NewRAPASymbols_Settings
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["VR_PPA_NewRAPASymbols_Settings"]; }
        }
    }
}