using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcIntegrationTestFramework.Hosting;
using MvcIntegrationTestFramework.Browsing;
using System.Web.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;

namespace IFM.DataServices.Tests.Controllers.OnBase
{
    //https://github.com/i-e-b/MvcIntegrationTestFramework
    //https://www.nuget.org/packages/MvcIntegrationTestFramework
    // ****************  Matt A  *********************************************
    // Used Git to pull code and changed it to Post Application/Json rather than Form Data
    // Hopefully it never needs to be changed further but removed NuGet reference and
    // just have it pointed to the version with my changes on G:\ClassFiles
    // ****************  Matt A  *********************************************


    [TestClass]
    public class PolicyTests : BaseTest
    {
        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/OnBase/Policy/TestCases.xml", "TestGetPolicyInfo", DataAccessMethod.Sequential)]
        public void TestGetPolicyInfo()
        {
            CommonContextItems tci = GetCommonTestContextItems(TestContext);
            string policyNumber = TestContext.DataRow["policyNumber"].ToString();

            var expectAgencyCode = TestContext.DataRow["expectAgencyCode"].ToString();
            var expectAgencyGroupCode = TestContext.DataRow["expectAgencyGroupCode"].ToString();
            var expectAgencyLocationCode = TestContext.DataRow["expectAgencyLocationCode"].ToString();
            var expectAgencyName = TestContext.DataRow["expectAgencyName"].ToString();
            var expectAgencyState = TestContext.DataRow["expectAgencyState"].ToString();
            var expectAgencyTerritory = TestContext.DataRow["expectAgencyTerritory"].ToString();
            var expectPolicyholderName1 = TestContext.DataRow["expectPolicyholderName1"].ToString();
            var expectPolicyholderName2 = TestContext.DataRow["expectPolicyholderName2"].ToString();
            var expectPolicyholderName3 = TestContext.DataRow["expectPolicyholderName3"].ToString();
            var expectPolicyholderName4 = TestContext.DataRow["expectPolicyholderName4"].ToString();
            var expectPolicyholderName5 = TestContext.DataRow["expectPolicyholderName5"].ToString();
            var expectQuoteNumber1 = TestContext.DataRow["expectQuoteNumber1"].ToString();
            var expectOfficeAccount = TestContext.DataRow["expectOfficeAccount"].ToString();

            appHost.Start(session =>
            {
                var diamondResult = session.Get($"OnBase/Policy/policynumber/{policyNumber}");

                DoBasicResultTests(diamondResult, tci);
                if (!tci.ExpectsPayload)
                {                    
                    Assert.AreEqual(diamondResult.Response.StatusCode.ToString(), tci.ExpectedResult);
                }
                else
                {
                    var sr = JsonConvert.DeserializeObject<global::IFM.DataServicesCore.CommonObjects.OnBase.OnBasePolicyInformation>(diamondResult.ResponseText);

                    Assert.AreEqual(sr.AgencyCode, expectAgencyCode);
                    Assert.AreEqual(sr.AgencyGroupCode, expectAgencyGroupCode);
                    Assert.AreEqual(sr.AgencyLocationCode, expectAgencyLocationCode);
                    Assert.AreEqual(sr.AgencyName, expectAgencyName);
                    Assert.AreEqual(sr.AgencyState, expectAgencyState);
                    Assert.AreEqual(sr.AgencyTerritory, expectAgencyTerritory);
                    if (sr.PolicyholderName.Length > 0)
                    {
                        Assert.AreEqual(sr.PolicyholderName[0], expectPolicyholderName1);
                        if (sr.PolicyholderName.Length > 1)
                        {
                            Assert.AreEqual(sr.PolicyholderName[1], expectPolicyholderName2);
                            if (sr.PolicyholderName.Length > 2)
                            {
                                Assert.AreEqual(sr.PolicyholderName[2], expectPolicyholderName3);
                                if (sr.PolicyholderName.Length > 3)
                                {
                                    Assert.AreEqual(sr.PolicyholderName[3], expectPolicyholderName4);
                                    if (sr.PolicyholderName.Length > 4)
                                    {
                                        Assert.AreEqual(sr.PolicyholderName[4], expectPolicyholderName5);
                                    }
                                }
                            }
                        }
                    }
                    Assert.AreEqual(sr.QuoteNumber[0], expectQuoteNumber1);
                    Assert.AreEqual(sr.OfficeAccount, expectOfficeAccount);
                }

            });

        }





    }
}
