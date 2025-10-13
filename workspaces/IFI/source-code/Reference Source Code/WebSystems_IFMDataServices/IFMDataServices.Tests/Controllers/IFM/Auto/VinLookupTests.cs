using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using IFM.PrimitiveExtensions;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Tests.Controllers.IFM.Auto
{
    [TestClass]
    public class VinLookupTests: BaseTest
    {

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/IFM/Auto/TestCases.xml", "makesByYear", DataAccessMethod.Sequential)]
        public void TestMakesByYear()
        {
            var tci = GetCommonTestContextItems(TestContext);
            string year = TestContext.DataRow["year"].ToString(); //"2016"


            Console.WriteLine($"Testing with '{year}'. Expected to have result items '{tci.ExpectedResult}'.");
            Assert.IsTrue(year.IsNumeric(), $"'{year}' is not numeric. Service route requires year to be an integer.");

            appHost.Start(session =>
            {
                var result = session.Get($"ifm/auto/MakesByYear?year={year}");
                var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<List<string>>>(result);
                var returnedData = this.DeserializeServiceResponseData(sr);

                DoBasicResponseTestsWithData(sr, returnedData,tci);

                Console.WriteLine($"Returned {returnedData.Count} result items.");
                Assert.AreEqual((returnedData.Count > 0).ToString().ToLower(), tci.ExpectedResult);

            });
        }


        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/IFM/Auto/TestCases.xml", "TestModelsByYear", DataAccessMethod.Sequential)]
        public void TestModelsByYear()
        {
            var tci = GetCommonTestContextItems(TestContext);
            string year = TestContext.DataRow["year"].ToString(); //"2016"
            string make = TestContext.DataRow["make"].ToString(); //"jeep"

            appHost.Start(session =>
            {
                var result = session.Get($"ifm/auto/ModelsByYearMake?year={year}&make={make}");
                var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<List<string>>>(result);
                var returnedData = this.DeserializeServiceResponseData(sr);

                DoBasicResponseTestsWithData(sr, returnedData,tci);


                Assert.IsTrue(returnedData.Count > 0, $"No models results returned. Year:{year} Make:{make}");

            });
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/IFM/Auto/TestCases.xml", "TestModelInfoByVin", DataAccessMethod.Sequential)]
        public void TestModelInfoByVin()
        {
            var tci = GetCommonTestContextItems(TestContext);
            string vin = TestContext.DataRow["vin"].ToString(); //"1C4NJDEB9GD559179"

            appHost.Start(session =>
            {
                var versionId = 128;
                var result = session.Get($"ifm/auto/ModelInfoByVin?vin={vin}&versionid={versionId}");
                var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<List<DataServicesCore.CommonObjects.IFM.Auto.VinLookupResult>>>(result);

                DoBasicResponseTestsWithData(sr, sr.ResponseData,tci);


                Assert.IsTrue(sr.ResponseData.Count > 0, $"No VIN results returned. VIN:{vin}");

            });
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/IFM/Auto/TestCases.xml", "TestModelInfoByYearMakeModel", DataAccessMethod.Sequential)]
        public void TestModelInfoByYearMakeModel()
        {
            var tci = GetCommonTestContextItems(TestContext);
            string year = TestContext.DataRow["year"].ToString();
            string make = TestContext.DataRow["make"].ToString();
            string model = TestContext.DataRow["model"].ToString();

            appHost.Start(session =>
            {
                var versionId = 245;
                var result = session.Get($"ifm/auto/ModelInfoByYearMakeModel?year={year}&make={make}&model={model}&versionid={versionId}");
                var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<List<DataServicesCore.CommonObjects.IFM.Auto.VinLookupResult>>>(result);

                DoBasicResponseTestsWithData(sr, sr.ResponseData,tci);


                Assert.IsTrue(sr.ResponseData.Count > 0, $"No results returned. Year:{year} Make:{make} Model:{model}");

            });
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/IFM/Auto/TestCases.xml", "TestModelInfoByYearMakeModel", DataAccessMethod.Sequential)]
        public void TestGetModels()
        {
            var tci = GetCommonTestContextItems(TestContext);
            string year = TestContext.DataRow["year"].ToString();
            string make = TestContext.DataRow["make"].ToString();
            string model = TestContext.DataRow["model"].ToString();

            appHost.Start(session =>
            {
                var versionId = 245;
                var result = session.Get($"ifm/auto/GetModels?year={year}&make={make}&versionid={versionId}");
                var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<List<KeyValuePair<string, string>>>>(result);

                DoBasicResponseTestsWithData(sr, sr.ResponseData, tci);


                Assert.IsTrue(sr.ResponseData.Count > 0, $"No results returned. Year:{year} Make:{make} VersionId:{versionId}");

            });
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/IFM/Auto/TestCases.xml", "TestModelInfoByYearMakeModel", DataAccessMethod.Sequential)]
        public void TestGetMakes()
        {
            var tci = GetCommonTestContextItems(TestContext);
            string year = TestContext.DataRow["year"].ToString();
            string make = TestContext.DataRow["make"].ToString();
            string model = TestContext.DataRow["model"].ToString();

            appHost.Start(session =>
            {
                var versionId = 245;
                var result = session.Get($"ifm/auto/GetMakes?year={year}&versionid={versionId}");
                var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<List<KeyValuePair<string, string>>>>(result);

                DoBasicResponseTestsWithData(sr, sr.ResponseData, tci);


                Assert.IsTrue(sr.ResponseData.Count > 0, $"No results returned. Year:{year} VersionId:{versionId}");

            });
        }

    }
}
