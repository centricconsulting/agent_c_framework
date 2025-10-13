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
    public class ProviderTests : BaseTest
    {
               


        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/OnBase/Provider/TestCases.xml", "TestGetProviderInfo", DataAccessMethod.Sequential)]
        public void TestGetProviderInfo()
        {

            CommonContextItems tci = GetCommonTestContextItems(TestContext);
            string providerName = TestContext.DataRow["providerName"].ToString();

            var expectProviderFein = TestContext.DataRow["expectProviderFein"].ToString();

            appHost.Start(session =>
            {
                var diamondResult = session.Get($"OnBase/Provider/providername/{providerName}");

                DoBasicResultTests(diamondResult, tci);
                if (!tci.ExpectsPayload)
                {
                    Assert.AreEqual(diamondResult.Response.StatusCode.ToString(), tci.ExpectedResult);
                }
                else
                {
                    var providerFein = JsonConvert.DeserializeObject(diamondResult.ResponseText);
                    Assert.AreEqual(providerFein, expectProviderFein);
                }
            });

        }





    }
}
