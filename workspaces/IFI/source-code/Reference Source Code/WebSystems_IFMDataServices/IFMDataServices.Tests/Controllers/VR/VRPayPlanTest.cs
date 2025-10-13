using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IFM.DataServices.Tests.Controllers.VR
{
    [TestClass]
    public class VRPayPlanTest : BaseTest
    {
        [TestMethod]
        public void PayPlanTest()
        {
            appHost.Start(session =>
            {
                var lobId = 1;
                
                var result = session.Get($"VR/PayPlan/Factors/{lobId}");
                var data = DeserializeJson <List<global::IFM.DataServicesCore.CommonObjects.VR.Payplan.PayplanFactor>>(result.ResponseText);

                Assert.IsTrue(data.Any());

            });
        }
    }
}
