using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickQuote.CommonMethods;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IFM.DataServices.Tests
{
    [TestClass]
    public class DebugTests: BaseTest
    {
        [TestMethod]
        public void CheckPolicyForExistingEndorsementImage()
        {
            string polNum = "PPA2136818";
            int polID = 42;
            QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo qqPLI = null;
            bool dbError = false;
            var HasPendingEndorsement = QuickQuoteHelperClass.HasPendingEndorsementImage(polNum, polID, ref qqPLI, ref dbError);
            if (1 == 1)
            {

            }
        }

        [TestMethod]
        public void GetAccountPolicyJson()
        {
            var AccountPolicies = new List<IFM.DataServicesCore.CommonObjects.OMP.MemberAccountPolicy>
            {
                new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "PPA2021502", NickName = "fdsyufsdghsd" }
                //new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "HOM2144064", NickName = "fdsyufsdghsd" }
                 //new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "PPA2121458", NickName = "fdsyufsdghsd" }
                //new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "PPA2104729", NickName = "fdsyufsdghsd" }, //Craig
                //new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "WCP1006144", NickName = "bghjhgtfrjghj" }, //Account Bill
                //new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "DFR1016543", NickName = "fdsyufsdghsd" },
                ////new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "WCP1006144", NickName = "bghjhgtfrjghj" }, //Account Bill
                //new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "HOM2099985", NickName = "dfgdhjddhgjdg" }
                ////new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "CPP1015027", NickName = "adfgahghjdghjf" },
                ////new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "CAP1006074", NickName = "jisopsap" },
                ////new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "FAR1027118", NickName = "scfhscbns" },
                ////new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "PUP1005760", NickName = "gswyukgshksghj," },
                ////new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "WCP1006144", NickName = "bghjhgtfrjghj" } //Account Bill
            };

            var data = ToJson(AccountPolicies);
            if (1 == 1)
            {

            }
        }
    }
}
