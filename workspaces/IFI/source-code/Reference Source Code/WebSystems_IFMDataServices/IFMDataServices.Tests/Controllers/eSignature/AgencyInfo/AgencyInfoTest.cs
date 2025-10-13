using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Tests.Controllers.eSignature.AgencyInfo
{
    [TestClass]
    public class AgencyInfo:BaseTest
    {
        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/eSignature/AgencyInfo/TestCasesAgencyCode.xml", "agencyInfo", DataAccessMethod.Sequential)]
        public void TestAgencyInfoWithAgencyCode()
        {
            CommonContextItems tci = GetCommonTestContextItems(TestContext);
            string agencyCode = TestContext.DataRow["agencyCode"].ToString();

            appHost.Start(session =>
            {
                //var result = session.PostJson("eSignature/AgencyInfo", ToJson(agencyCode));
                var result = session.Get($"eSignature/Agency/AgencyInfoByAgencyCode/{agencyCode}");
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                //var returnedData = this.DeserializeServiceResponseData<List<global::IFM.VR.Common.Helpers.ZipLookupResult>>(sr);

                DoBasicResponseTests(sr, tci);
            });
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/eSignature/AgencyInfo/TestCasesPolicyNumber.xml", "agencyInfo", DataAccessMethod.Sequential)]
        public void TestAgencyInfoWithPolicyNumber()
        {
            CommonContextItems tci = GetCommonTestContextItems(TestContext);
            string policyNumber = TestContext.DataRow["policyNumber"].ToString();

            appHost.Start(session =>
            {
                //var result = session.PostJson("eSignature/AgencyInfo", ToJson(agencyCode));
                var result = session.Get($"eSignature/Agency/AgencyInfoByPolicyNumber/{policyNumber}");
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                //var returnedData = this.DeserializeServiceResponseData<List<global::IFM.VR.Common.Helpers.ZipLookupResult>>(sr);

                DoBasicResponseTests(sr, tci);
            });
        }
    }
}
