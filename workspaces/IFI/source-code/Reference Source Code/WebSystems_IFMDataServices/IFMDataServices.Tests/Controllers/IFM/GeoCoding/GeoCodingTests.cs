using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Tests.Controllers.IFM.GeoCoding
{
    [TestClass]
    public class GeoCodingTests:BaseTest
    {


        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/IFM/GeoCoding/TestCases.xml", "TestAddressInformation", DataAccessMethod.Sequential)]
        public void TestAddressInformation()
        {
            CommonContextItems tci = GetCommonTestContextItems(TestContext);
            string address = TestContext.DataRow["address"].ToString(); //"46071"            
            appHost.Start(session =>
            {
                var result = session.Get($"ifm/GeoCoding/AddressInformation?address={address}");
                var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<IfmGeoCoder.GeoCodeLookupResult>>(result);
                var returnedData = this.DeserializeServiceResponseData(sr);

                DoBasicResponseTestsWithData(sr, returnedData,tci);

                Assert.IsFalse(string.IsNullOrWhiteSpace(returnedData.ZipCode), $"No ZIP code returned in result. Address:{address}");
            });
        }


        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/IFM/GeoCoding/TestCases.xml", "TestStraightLineDistance", DataAccessMethod.Sequential)]
        public void TestStraightLineDistance()
        {
            CommonContextItems tci = GetCommonTestContextItems(TestContext);
            string address = TestContext.DataRow["address"].ToString(); //"46071"            
            string address2 = TestContext.DataRow["address2"].ToString(); //"46071"            

            appHost.Start(session =>
            {   
                var result = session.Get($"ifm/geocoding/StraightLineDistance?address1={address}&address2={address2}");
                var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<string>>(result);
                var returnedData = this.DeserializeServiceResponseData(sr);

                DoBasicResponseTestsWithData(sr, returnedData,tci);


                Assert.IsTrue(string.IsNullOrWhiteSpace(returnedData) == false && returnedData != "0", $"No distance returned. Address:{address} Address2:{address2}");

            });
        }



    }
}
