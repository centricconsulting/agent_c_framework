using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcIntegrationTestFramework.Hosting;
using MvcIntegrationTestFramework.Browsing;
using System.Web.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using IFM.DataServicesCore.CommonObjects.OnBase;

namespace IFM.DataServices.Tests
{
    //https://github.com/i-e-b/MvcIntegrationTestFramework
    //https://www.nuget.org/packages/MvcIntegrationTestFramework
    // ****************  Matt A  *********************************************
    // Used Git to pull code and changed it to Post Application/Json rather than Form Data
    // Hopefully it never needs to be changed further but removed NuGet reference and
    // just have it pointed to the version with my changes on G:\ClassFiles
    // ****************  Matt A  *********************************************


    [TestClass]
    public class ClaimantListTests : BaseTest
    {



        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/OnBase/ClaimantList/TestCases.xml", "TestGetClaimantList", DataAccessMethod.Sequential)]
        public void TestGetClaimantList()
        {

            CommonContextItems tci = GetCommonTestContextItems(TestContext);
            string claimNumber = TestContext.DataRow["claimNumber"].ToString();

            var expectClaimantName_1 = TestContext.DataRow["expectClaimantName_1"].ToString();
            var expectAddress1_1 = TestContext.DataRow["expectAddress1_1"].ToString();
            var expectAddress2_1 = TestContext.DataRow["expectAddress2_1"].ToString();
            var expectCity_1 = TestContext.DataRow["expectCity_1"].ToString();
            var expectState_1 = TestContext.DataRow["expectState_1"].ToString();
            var expectZip_1 = TestContext.DataRow["expectZip_1"].ToString();
            var expectClaimantName_2 = TestContext.DataRow["expectClaimantName_2"].ToString();
            var expectAddress1_2 = TestContext.DataRow["expectAddress1_2"].ToString();
            var expectAddress2_2 = TestContext.DataRow["expectAddress2_2"].ToString();
            var expectCity_2 = TestContext.DataRow["expectCity_2"].ToString();
            var expectState_2 = TestContext.DataRow["expectState_2"].ToString();
            var expectZip_2 = TestContext.DataRow["expectZip_2"].ToString();
            var expectClaimantName_3 = TestContext.DataRow["expectClaimantName_3"].ToString();
            var expectAddress1_3 = TestContext.DataRow["expectAddress1_3"].ToString();
            var expectAddress2_3 = TestContext.DataRow["expectAddress2_3"].ToString();
            var expectCity_3 = TestContext.DataRow["expectCity_3"].ToString();
            var expectState_3 = TestContext.DataRow["expectState_3"].ToString();
            var expectZip_3 = TestContext.DataRow["expectZip_3"].ToString();
            var expectClaimantName_4 = TestContext.DataRow["expectClaimantName_4"].ToString();
            var expectAddress1_4 = TestContext.DataRow["expectAddress1_4"].ToString();
            var expectAddress2_4 = TestContext.DataRow["expectAddress2_4"].ToString();
            var expectCity_4 = TestContext.DataRow["expectCity_4"].ToString();
            var expectState_4 = TestContext.DataRow["expectState_4"].ToString();
            var expectZip_4 = TestContext.DataRow["expectZip_4"].ToString();

            appHost.Start(session =>
            {
                var diamondResult = session.Get($"OnBase/ClaimantList/claimnumber/{claimNumber}");

                DoBasicResultTests(diamondResult, tci);
                if (!tci.ExpectsPayload)
                {
                    Assert.AreEqual(diamondResult.Response.StatusCode, (int)HttpStatusCode.NotFound);
                }
                else
                {
                    List<OnBaseClaimantInformation> sr = JsonConvert.DeserializeObject<List<OnBaseClaimantInformation>>(diamondResult.ResponseText);

                    if (sr.Count > 0)
                    {
                        Assert.AreEqual(sr[0].ClaimantName, expectClaimantName_1);
                        Assert.AreEqual(sr[0].Address1, expectAddress1_1);
                        Assert.AreEqual(sr[0].Address2, expectAddress2_1);
                        Assert.AreEqual(sr[0].City, expectCity_1);
                        Assert.AreEqual(sr[0].State, expectState_1);
                        Assert.AreEqual(sr[0].Zip, expectZip_1);
                    }
                    if (sr.Count > 1)
                    {
                        Assert.AreEqual(sr[1].ClaimantName, expectClaimantName_2);
                        Assert.AreEqual(sr[1].Address1, expectAddress1_2);
                        Assert.AreEqual(sr[1].Address2, expectAddress2_2);
                        Assert.AreEqual(sr[1].City, expectCity_2);
                        Assert.AreEqual(sr[1].State, expectState_2);
                        Assert.AreEqual(sr[1].Zip, expectZip_2);
                    }
                    if (sr.Count > 2)
                    {
                        Assert.AreEqual(sr[2].ClaimantName, expectClaimantName_3);
                        Assert.AreEqual(sr[2].Address1, expectAddress1_3);
                        Assert.AreEqual(sr[2].Address2, expectAddress2_3);
                        Assert.AreEqual(sr[2].City, expectCity_3);
                        Assert.AreEqual(sr[2].State, expectState_3);
                        Assert.AreEqual(sr[2].Zip, expectZip_3);
                    }
                    if (sr.Count > 3)
                    {
                        Assert.AreEqual(sr[3].ClaimantName, expectClaimantName_4);
                        Assert.AreEqual(sr[3].Address1, expectAddress1_4);
                        Assert.AreEqual(sr[3].Address2, expectAddress2_4);
                        Assert.AreEqual(sr[3].City, expectCity_4);
                        Assert.AreEqual(sr[3].State, expectState_4);
                        Assert.AreEqual(sr[3].Zip, expectZip_4);
                    }
                }

            });

        }





    }
}
