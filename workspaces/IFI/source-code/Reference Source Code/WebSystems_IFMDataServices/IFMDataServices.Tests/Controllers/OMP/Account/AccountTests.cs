using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcIntegrationTestFramework.Hosting;
using MvcIntegrationTestFramework.Browsing;
using System.Web.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;
using APIResponses = IFM.DataServices.API.ResponseObjects;
using IFM.DataServicesCore.CommonObjects.OMP;
using IFM.DataServices.API.ResponseObjects.Common;
using System.Diagnostics;

namespace IFM.DataServices.Tests.Controllers.OMP.Account
{
    //https://github.com/i-e-b/MvcIntegrationTestFramework
    //https://www.nuget.org/packages/MvcIntegrationTestFramework
    // ****************  Matt A  *********************************************
    // Used Git to pull code and changed it to Post Application/Json rather than Form Data
    // Hopefully it never needs to be changed further but removed NuGet reference and
    // just have it pointed to the version with my changes on G:\ClassFiles
    // ****************  Matt A  *********************************************


    [TestClass]
    public class AccountTests :BaseTest
    {
        [TestMethod]
        public void TestMassLoadGetRegisteredPolicyInfo()
        {
            var AccountPolicies = new List<MemberAccountPolicy>
            {
                new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "PPA2115533", NickName = "test" }
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

            appHost.Start(session =>
            {
                var data = ToJson(AccountPolicies);
                //var result = session.PostJson("omp/account/accountpolicies", data);
                //APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
               // var returnedData = this.DeserializeServiceResponseData<List<IFM.DataServicesCore.CommonObjects.OMP.AccountRegistedPolicy>>(sr);
             });
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/OMP/Account/TestCases.xml", "TestGetRegisteredPolicyInfo", DataAccessMethod.Sequential)]
        public void TestGetRegisteredPolicyInfo()
        {
            CommonContextItems tci = GetCommonTestContextItems(TestContext);
            //string policyNumber = TestContext.DataRow["policyNumber"].ToString();
            //string nickName = TestContext.DataRow["nickName"].ToString();

            var AccountPolicies = new List<MemberAccountPolicy>
            {
                //new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "FAR1017427", NickName = "Farm" }, //Account Bill CGL1008292
                //new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "FUP1009473", NickName = "Farm2" } //Account Bill CGL1008292

                //new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "CPP1018959", NickName = "test1" }, //Account Bill CGL1008292
                //new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "CAP1012120", NickName = "test2" }, //Account Bill CGL1008292
                //new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "WCP1008834", NickName = "test3" } //Account Bill CGL1008292

                //new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "CGL1001267", NickName = "test3" } //Account Bill CGL1008292

                //new MemberAccountPolicy() { PolicyNumber = "PPA2089949", NickName = "Test" } //CUP1002836
                //new MemberAccountPolicy() { PolicyNumber = "FAR1034550", NickName = "Test" }
                //new MemberAccountPolicy() { PolicyNumber = "PPA2008111", NickName = "Test" }
                //new MemberAccountPolicy() { PolicyNumber = "CPP1017198", NickName = "Test" }
                //new MemberAccountPolicy() { PolicyNumber = "HOM2096477", NickName = "Test" }
                //new MemberAccountPolicy() { PolicyNumber = "CUP1003416", NickName = "Test" }
                new MemberAccountPolicy() { PolicyNumber = "HOM2010184", NickName = "Test" }

                //new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "PPA2015953", NickName = "hjgb" }, // Becca
                //new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "HOM2012337", NickName = "dnhjd" }, // Becca
                //new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "HOM2114872", NickName = "dyk" }, // Becca
                //new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "PPA1000014", NickName = "hjhgjk" }, //Cancel-Rewrite PPA1000014 to PPA1000076
                //new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "BOP1000317", NickName = "bghjghj" }, //Multiple PolicyIds BOP1000317
                //new DataServicesCore.CommonObjects.OMP.MemberAccountPolicy() { PolicyNumber = "CPR1000703", NickName = "" } //New Policy CPR1000703 PID:1224727 CID:1027030 I#:1
            };

            appHost.Start(session =>
            {
                var data = ToJson(AccountPolicies);
                Stopwatch sw = Stopwatch.StartNew();
                var result = session.PostJson("omp/account/accountpolicies", data);
                sw.Stop();

                var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<List<AccountRegistedPolicy>>>(result);
                DoBasicResponseTestsWithData(sr, sr.ResponseData, tci);
            });
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/OMP/Account/TestCases.xml", "TestVerification", DataAccessMethod.Sequential)]
        public void TestVerification()
        {

            CommonContextItems tci = GetCommonTestContextItems(TestContext);

            string accountNumber = TestContext.DataRow["accountNumber"].ToString();
            string policyNumber = TestContext.DataRow["policyNumber"].ToString();
            string name = TestContext.DataRow["name"].ToString();
            string zip = TestContext.DataRow["zip"].ToString();
            string dln = TestContext.DataRow["dln"].ToString();
            string dob = TestContext.DataRow["dob"].ToString();
            string ssn = TestContext.DataRow["ssn"].ToString();
            string onlinePaymentNumber = TestContext.DataRow["onlinePaymentNumber"].ToString();

            DataServicesCore.CommonObjects.Enums.Enums.PolicyInformationVerificationLevel PolicyInformationVerificationLevel = (DataServicesCore.CommonObjects.Enums.Enums.PolicyInformationVerificationLevel)Convert.ToInt32(TestContext.DataRow["PolicyInformationVerificationLevel"].ToString());

            var Pav = new PolicyAccessVerification
            {
                VerificationTypeId = PolicyInformationVerificationLevel,
                AccountNumber = accountNumber,
                PolicyNumber = policyNumber,
                Name = name,
                Zip = zip,
                DLN = dln,
                DOB = dob,
                FEIN = ssn,
                OnlinePaymentNumber = onlinePaymentNumber.TryToGetInt32()
            };

            appHost.Start(session =>
            {
                var data = ToJson(Pav);
                var result = session.PostJson("omp/account/policyVerification", data);
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);

                DoBasicResponseTestsWithData(sr, sr.ResponseData, tci);
                Assert.AreEqual(Convert.ToBoolean(tci.ExpectedResult), sr.ResponseData);
            });
        }

        //added 3/20/2018
        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/OMP/Account/TestCases.xml", "TestGetPolicyInformation", DataAccessMethod.Sequential)]
        public void TestGetPolicyInformation()
        {
            CommonContextItems tci = GetCommonTestContextItems(TestContext);
            string policyId = TestContext.DataRow["policyId"].ToString();
            string imageNumber = TestContext.DataRow["imageNumber"].ToString();

            policyId = "173776";
            imageNumber = "21";

            appHost.Start(session =>
            {
                var result = session.Get($"omp/account/policies/{policyId}/{imageNumber}");
                var sr = DeserializeServiceResponse<ServiceResult<BasicPolicyInformation>>(result);
                var returnedData = this.DeserializeServiceResponseData(sr);

                DoBasicResponseTestsWithData(sr, returnedData, tci);
                //if (returnedData != null)
                //{
                //    Console.WriteLine($"Returned {returnedData.Count} result items.");
                //    Assert.AreEqual((returnedData.Count > 0).ToString().ToLower(), tci.ExpectedResult);
                //}
            });
        }
    }
}
