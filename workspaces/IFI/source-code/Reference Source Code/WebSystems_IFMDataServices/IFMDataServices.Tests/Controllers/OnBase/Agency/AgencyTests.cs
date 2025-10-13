using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcIntegrationTestFramework.Hosting;
using MvcIntegrationTestFramework.Browsing;
using System.Web.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;

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
    public class AgencyTests : BaseTest
    {
               
        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/OnBase/Agency/TestCasesById.xml", "TestGetAgencyInfoById", DataAccessMethod.Sequential)]
        public void TestGetAgencyInfoById()
        {

            CommonContextItems tci = GetCommonTestContextItems(TestContext);
            string agencyId = TestContext.DataRow["agencyId"].ToString();

            var expectCode = TestContext.DataRow["expectCode"].ToString();
            var expectGroupCode = TestContext.DataRow["expectGroupCode"].ToString();
            var expectLocationCode = TestContext.DataRow["expectLocationCode"].ToString();
            var expectName = TestContext.DataRow["expectName"].ToString();
            var expectState= TestContext.DataRow["expectState"].ToString();
            var expectCommercialLinesTerritory = TestContext.DataRow["expectCommercialLinesTerritory"].ToString();
            var expectPersonalLinesTerritory = TestContext.DataRow["expectPersonalLinesTerritory"].ToString();

            appHost.Start(session =>
            {
                var diamondResult = session.Get($"OnBase/Agency/agencyid/{agencyId}");

                DoBasicResultTests(diamondResult, tci);
                if (!tci.ExpectsPayload)
                {
                    Assert.AreEqual(diamondResult.Response.StatusCode.ToString(), tci.ExpectedResult);
                }
                else
                {
                    var sr = JsonConvert.DeserializeObject<IFM.DataServicesCore.CommonObjects.OnBase.OnBaseAgencyInformation>(diamondResult.ResponseText);

                    Assert.AreEqual(sr.Code, expectCode);
                    Assert.AreEqual(sr.GroupCode, expectGroupCode);
                    Assert.AreEqual(sr.LocationCode, expectLocationCode);
                    Assert.AreEqual(sr.Name, expectName);
                    Assert.AreEqual(sr.State, expectState);
                    Assert.AreEqual(sr.CommercialLinesTerritory, expectCommercialLinesTerritory);
                    Assert.AreEqual(sr.PersonalLinesTerritory, expectPersonalLinesTerritory);
                }                

            });

        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/OnBase/Agency/TestCasesByCode.xml", "TestGetAgencyInfoByCode", DataAccessMethod.Sequential)]
        public void TestGetAgencyInfoByCode()
        {

            CommonContextItems tci = GetCommonTestContextItems(TestContext);
            string agencyCode = TestContext.DataRow["agencyCode"].ToString();

            var expectGroupCode = TestContext.DataRow["expectGroupCode"].ToString();
            var expectLocationCode = TestContext.DataRow["expectLocationCode"].ToString();
            var expectName = TestContext.DataRow["expectName"].ToString();
            var expectState = TestContext.DataRow["expectState"].ToString();
            var expectCommercialLinesTerritory = TestContext.DataRow["expectCommercialLinesTerritory"].ToString();
            var expectPersonalLinesTerritory = TestContext.DataRow["expectPersonalLinesTerritory"].ToString();

            appHost.Start(session =>
            {
                var diamondResult = session.Get($"OnBase/Agency/agencycode/{agencyCode}");

                DoBasicResultTests(diamondResult, tci);
                if (!tci.ExpectsPayload)
                {
                    Assert.AreEqual(diamondResult.Response.StatusCode.ToString(), tci.ExpectedResult);
                    Assert.IsTrue(string.IsNullOrWhiteSpace(diamondResult.ResponseText));
                }
                else
                {
                    var sr = JsonConvert.DeserializeObject<IFM.DataServicesCore.CommonObjects.OnBase.OnBaseAgencyInformation>(diamondResult.ResponseText);

                    Assert.AreEqual(sr.GroupCode, expectGroupCode);
                    Assert.AreEqual(sr.LocationCode, expectLocationCode);
                    Assert.AreEqual(sr.Name, expectName);
                    Assert.AreEqual(sr.State, expectState);
                    Assert.AreEqual(sr.CommercialLinesTerritory, expectCommercialLinesTerritory);
                    Assert.AreEqual(sr.PersonalLinesTerritory, expectPersonalLinesTerritory);
                }

            });

        }

    }
}
