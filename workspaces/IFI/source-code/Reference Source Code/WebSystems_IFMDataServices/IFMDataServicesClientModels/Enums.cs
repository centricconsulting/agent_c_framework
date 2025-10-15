using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFM.DataServices.API
{
    public class Enums
    {
        public enum MessageType : int
        {
            None = 0,
            GeneralMessage = 1,
            ValidationMessage = 2
        }
        public enum MessageSeverityType : int
        {
            None = 0,
            Warning = 1,
            StandardError = 2,
            FullStopError = 3
        }
        public enum PaymentInterface
        {
            None,
            MemberPortalSite,
            AgentsOnlySite,
            StaffPaymentSite,
            IVR_Pay_By_Phone,
            MobileApplications,
            MobileSite,
            ConsumerQuotingSite,
            RecurringCreditCard,
            OneView,
            FiservRealtimePaymentNotificationAPI,
            OneTimePayment,
            MobileOneTimePayment
        }
        public enum UserType
        {
            None,
            Policyholder,
            Agent,
            Staff
        }
        public enum BankFileExclusion
        {
            Default,
            True,
            False
        }
        public enum CreditCardType
        {
            None = 0,
            Visa = 1,
            MasterCard = 2,
            Discover = 3,
            AmericanExpress = 4,
            //'added 5/7/2020 for Fiserv
            Pulse = 5,
            Star = 6,
            Accel = 7,
            Nyce = 8
        }

        public enum CashSource
        {
            NA = 0,
            EFT = 6,
            AgencyEFT = 8,
            CreditCard = 11,
            RecurringCreditCard = 18,
            WebAgencyCCAmericanExpress = 10016,
            WebAgencyCCDiscover = 10017,
            WebAgencyCCMasterCard = 10018,
            WebAgencyCCVisa = 10019,
            WebAgencyCCWithAppAmericanExpress = 10020,
            WebAgencyCCWithAppDiscover = 10021,
            WebAgencyCCWithAppMasterCard = 10022,
            WebAgencyCCWithAppVisa = 10023,
            WebAgencyEFT = 10024,
            WebAgencyEftWithApp = 10025,
            WebCCAmericanExpress = 10026,
            WebCCDiscover = 10027,
            WebCCMasterCard = 10028,
            WebCCVisa = 10029,
            eCheck = 10030
        }
    }
}