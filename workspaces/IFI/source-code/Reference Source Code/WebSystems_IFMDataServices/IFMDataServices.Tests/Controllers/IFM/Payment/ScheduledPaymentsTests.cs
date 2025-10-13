using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFM.PrimitiveExtensions;
using System.Diagnostics;
using Diamond.Business.ThirdParty.QwikSignAPI;
using APIResponses = IFM.DataServices.API.ResponseObjects;
using System.Collections.Generic;
using IFM.DataServicesCore.CommonObjects.Fiserv;

namespace IFM.DataServices.Tests.Controllers.IFM.Payment
{
    [TestClass]
    public class ScheduledPaymentsTests :BaseTest
    {
        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/IFM/Payment/TestCases.xml", "testMakePayment", DataAccessMethod.Sequential)]
        public void TestGetScheduledPayments()
        {
            CommonContextItems tci = GetCommonTestContextItems(TestContext);

            var policies = new List<string>();
            policies.Add("BOP1005958");
            policies.Add("CAP1011484");
            policies.Add("CAP1011609");
            policies.Add("CGL1011217");
            policies.Add("CIM1002252");
            policies.Add("CPP1002877");
            policies.Add("CPP1011120");
            policies.Add("CUP1001723");
            policies.Add("CUP1002734");
            policies.Add("FAR1029360");
            policies.Add("WCP1005808");
            policies.Add("WCP1007460");

            //[["BOP1005958","BOP1005958","CAP1011484","CAP1011609","CGL1011217","CIM1002252","CPP1002877","CPP1002877","CPP1011120","CUP1001723","CUP1002734","CUP1002734","FAR1029360","WCP1005808","WCP1007460","WCP1007460"]]

            appHost.Start(session =>
            {
                var json = ToJson(policies);
                var result = session.PostJson($"Fiserv/ScheduledPayments/ScheduledPaymentsForPolicyNumbers", json);
                var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<List<ScheduledPayment>>>(result);
                var returnedData = this.DeserializeServiceResponseData(sr);

                DoBasicResponseTestsWithData(sr, returnedData, tci);

                Assert.AreEqual(tci.ExpectsErrors, sr.HasErrors);
            });
        }
    }
}
