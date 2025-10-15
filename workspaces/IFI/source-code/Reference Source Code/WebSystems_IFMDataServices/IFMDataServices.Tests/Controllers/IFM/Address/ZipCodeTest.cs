using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Tests.Controllers.IFM.Address
{
    [TestClass]
    public class ZipCodeTest: BaseTest
    {
        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/IFM/Address/TestCases.xml", "zipLookup", DataAccessMethod.Sequential)]
        public void TestZipCode()
        {
            CommonContextItems tci = GetCommonTestContextItems(TestContext);
            string zipCode = TestContext.DataRow["zipCode"].ToString(); //"46071"


            appHost.Start(session =>
            {
                var result = session.Get($"ifm/address/zipcodes/{zipCode}");
                var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<List<global::IFM.VR.Common.Helpers.ZipLookupResult>>> (result);
                var returnedData = this.DeserializeServiceResponseData(sr);

                DoBasicResponseTestsWithData(sr, returnedData,tci);
                if (returnedData != null)
                {
                    Console.WriteLine($"Returned {returnedData.Count} result items.");
                    Assert.AreEqual((returnedData.Count > 0).ToString().ToLower(), tci.ExpectedResult);
                }
            });
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/IFM/Address/TestCases.xml", "zipLookup", DataAccessMethod.Sequential)]
        public void TestOutOfStateGaraging()
        {
            CommonContextItems tci = GetCommonTestContextItems(TestContext);
            //string zipCode = TestContext.DataRow["zipCode"].ToString(); //"46071"
            string OriginState = "IN";

            DataServicesCore.CommonObjects.OMP.Address myAddress = new DataServicesCore.CommonObjects.OMP.Address();
            myAddress.City = "LE ROY";
            myAddress.County = "DE WITT";
            myAddress.Zip = "61752";
            myAddress.StateAbbrev = "IL";

            string errorMessage = "";

            appHost.Start(session =>
            {
                var result = global::IFM.DataServicesCore.BusinessLogic.OMP.AddressHelper.IsAddressOutOfState(myAddress, OriginState, out errorMessage);
                Assert.AreEqual(result, true);
            });
        }
    }
}
