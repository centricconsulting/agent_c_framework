using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcIntegrationTestFramework.Hosting;
using MvcIntegrationTestFramework.Browsing;
using System.Web.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;

namespace IFM.DataServices.Tests.Controllers.OnBase.Claims
{
    //https://github.com/i-e-b/MvcIntegrationTestFramework
    //https://www.nuget.org/packages/MvcIntegrationTestFramework
    // ****************  Matt A  *********************************************
    // Used Git to pull code and changed it to Post Application/Json rather than Form Data
    // Hopefully it never needs to be changed further but removed NuGet reference and
    // just have it pointed to the version with my changes on G:\ClassFiles
    // ****************  Matt A  *********************************************


    [TestClass]
    public class ClaimTests :BaseTest
    {
               


        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/OnBase/Claim/TestCases.xml", "TestGetClaimInfo", DataAccessMethod.Sequential)]
        public void TestGetClaimInfo()
        {

            CommonContextItems tci = GetCommonTestContextItems(TestContext);
            //string claimNumber = TestContext.DataRow["claimNumber"].ToString();

            //var expectClaimantName1 = TestContext.DataRow["expectClaimantName1"].ToString();
            //var expectClaimantName2 = TestContext.DataRow["expectClaimantName2"].ToString();
            //var expectClaimantName3 = TestContext.DataRow["expectClaimantName3"].ToString();
            //var expectPolicyHolderName1 = TestContext.DataRow["expectPolicyHolderName1"].ToString();
            //var expectPolicyHolderName2 = TestContext.DataRow["expectPolicyHolderName2"].ToString();
            //var expectPolicyNumber = TestContext.DataRow["expectPolicyNumber"].ToString();
            //var expectOfficeAccount = TestContext.DataRow["expectOfficeAccount"].ToString();
            //var expectedRepName = TestContext.DataRow["expectedRepName"].ToString();
            //var expectedRepUserName = TestContext.DataRow["expectedRepUserName"].ToString();

            //string claimNumber = "1218749"; Prod long loading issue claim
            //string claimNumber = "1170749";
            //string claimNumber = "1145937"; //Prod 404
            string claimNumber = "1184569"; //Prod 404

            appHost.Start(session =>
            {
                var warmup1 = session.Get($"OnBase/Claim/claimNumber/{claimNumber}");
                var warmup2 = session.Get($"OnBase/Claim/claimNumber2/{claimNumber}");

                var a = Stopwatch.StartNew();
                var diamondResult = session.Get($"OnBase/Claim/claimNumber/{claimNumber}");
                a.Stop();

                var b = Stopwatch.StartNew();
                var diamondResult2 = session.Get($"OnBase/Claim/claimNumberV1/{claimNumber}");
                b.Stop();

                DoBasicResultTests(diamondResult, tci);
                //if (!tci.ExpectsPayload)
                //{
                //    Assert.AreEqual(diamondResult.Response.StatusCode, (int)HttpStatusCode.NotFound);
                //}
                //else
                //{
                //    var sr = JsonConvert.DeserializeObject<global::IFM.DataServicesCore.CommonObjects.OnBase.OnBaseClaimInformation>(diamondResult.ResponseText);

                //    if (sr.ClaimantName.Length > 0)
                //    {
                //        Assert.AreEqual(sr.ClaimantName[0], expectClaimantName1);
                //        if (sr.ClaimantName.Length > 1)
                //        {
                //            Assert.AreEqual(sr.ClaimantName[1], expectClaimantName2);
                //            if (sr.ClaimantName.Length > 2)
                //            {
                //                Assert.AreEqual(sr.ClaimantName[2], expectClaimantName3);
                //            }
                //        }
                //    }
                //    if (sr.PolicyHolderName.Length > 0)
                //    {
                //        Assert.AreEqual(sr.PolicyHolderName[0], expectPolicyHolderName1);
                //        if (sr.PolicyHolderName.Length > 1)
                //        {
                //            Assert.AreEqual(sr.PolicyHolderName[1], expectPolicyHolderName2);
                //        }
                //    }
                //    Assert.AreEqual(sr.PolicyNumber, expectPolicyNumber);
                //    Assert.AreEqual(sr.OfficeAccount, expectOfficeAccount);
                //    Assert.AreEqual(sr.ClaimAdjuster, expectedRepName);
                //    Assert.AreEqual(sr.ClaimAdjusterUserName, expectedRepUserName);
                //}

            });

        }





    }
}
