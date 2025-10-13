using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Tests.Controllers.Diamond
{
    [TestClass]
    public class AgencyAccessTest : BaseTest
    {
        [TestMethod]
        //[DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/Diamond/AgencyAccess/TestCasesAgencyAccess.xml", "Info", DataAccessMethod.Sequential)]
        public void TestAssignMissingAgenciesToUserAsSecondaryAgencies()
        {
            //CommonContextItems tci = GetCommonTestContextItems(TestContext);
            //string DiamondUserID = TestContext.DataRow["DiamondUserID"].ToString();
            string DiamondUserID = "10648"; //EOMTEST2
            //string DiamondUserID = "10027";//PATCH
            appHost.Start(session =>
            {
                //var result = session.PostJson("eSignature/AgencyInfo", ToJson(agencyCode));
                var result = session.Get($"Diamond/AgencyAccess/AssignMissingAgenciesToUserAsSecondaryAgencies/{DiamondUserID}");
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                //var returnedData = this.DeserializeServiceResponseData<List<global::IFM.VR.Common.Helpers.ZipLookupResult>>(sr);

               //DoBasicResposeTests(sr, tci);
            });
        }
    }
}
