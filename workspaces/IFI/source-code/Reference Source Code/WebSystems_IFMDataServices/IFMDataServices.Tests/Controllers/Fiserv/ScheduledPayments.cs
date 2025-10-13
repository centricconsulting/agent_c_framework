using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcIntegrationTestFramework.Hosting;
using MvcIntegrationTestFramework.Browsing;
using System.Web.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;
using APIResponses = IFM.DataServices.API.ResponseObjects;
using IFM.DataServicesCore.CommonObjects.OMP;
using IFM.DataServicesCore.CommonObjects.Fiserv;

namespace IFM.DataServices.Tests.Controllers.Fiserv
{
    //https://github.com/i-e-b/MvcIntegrationTestFramework
    //https://www.nuget.org/packages/MvcIntegrationTestFramework
    // ****************  Matt A  *********************************************
    // Used Git to pull code and changed it to Post Application/Json rather than Form Data
    // Hopefully it never needs to be changed further but removed NuGet reference and
    // just have it pointed to the version with my changes on G:\ClassFiles
    // ****************  Matt A  *********************************************


    [TestClass]
    public class ScheduledPaymentTests :BaseTest
    {
        [TestMethod]
        public void TestGetScheduledPayments()
        {
            var policies = new List<string> { "BOP1003959", "BOP1005328", "BOP1005979", "BOP1006516", "BOP1008104", "CAP1001199", "CAP1009640", "CAP1010774", "CAP1012407", "CGL1000569", "CPP1013512", "CPP1014952", "CPP1015404", "CUP1001365", "CUP1002278", "CUP1002665", "CUP1002770", "CUP1002784", "CUP1003177", "CUP1004204", "DFR1019207", "FAR1035086", "FUP1010868", "GAR1000116", "HOM2146241", "HOM2146243", "PPA2141981", "PPA2143257", "PUP1010646", "WCP1000687", "WCP1004346", "WCP1006742", "WCP1006775", "WCP1007365", "WCP1007488", "WCP1007505", "WCP1008012", "WCP1008976"};

            appHost.Start(session =>
            {
                var data = ToJson(policies);
                var result = session.PostJson("Fiserv/ScheduledPayments/ScheduledPaymentsForPolicyNumbers", data);
                var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<List<DataServicesCore.CommonObjects.Fiserv.ScheduledPayment>>>(result);
                // var returnedData = this.DeserializeServiceResponseData<List<IFM.DataServicesCore.CommonObjects.OMP.AccountRegistedPolicy>>(sr);
                var returnedData = this.DeserializeServiceResponseData(sr);
                Assert.IsNotNull(returnedData);
             });
        }       
    }
}
