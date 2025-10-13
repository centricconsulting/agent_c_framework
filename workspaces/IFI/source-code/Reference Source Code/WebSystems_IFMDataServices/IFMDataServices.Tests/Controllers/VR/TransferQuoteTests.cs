using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Tests.Controllers.VR
{
    [TestClass]
    public class TransferQuoteTests : BaseTest
    {
        [TestMethod]
        public void TransferQuoteTest()
        {
            return; // just don't want test failing in future when this quote doesn't exist after a refresh
            appHost.Start(session =>
            {
                var policyId = 1591594;
                var userId = 8001;
                var result = session.Get($"VR/QuoteTransfer/AquireByUserId/{policyId}/{userId}");
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                Assert.IsFalse(sr.HasErrors);
                
            });
        }

        [TestMethod]
        public void TransferQuoteByQuoteNumberToAgencyUserTest()
        {
            return; // just don't want test failing in future when this quote doesn't exist after a refresh
            appHost.Start(session =>
            {
                var quoteNumber = "QPPA526097";
                var agencyCode = "6013-1840"; //brewton
                var result = session.Get($"VR/QuoteTransfer/TransferQuoteToAgencyUser/{quoteNumber}/{agencyCode}");
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                Assert.IsFalse(sr.HasErrors);

            });
        }


    }
}
