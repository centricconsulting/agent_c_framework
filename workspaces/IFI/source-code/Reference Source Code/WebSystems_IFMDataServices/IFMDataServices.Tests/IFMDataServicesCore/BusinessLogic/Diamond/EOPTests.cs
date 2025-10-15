using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IFM.DataServices.Tests.IFMDataServicesCore.BusinessLogic.Diamond
{
    [TestClass]
    public class EOPTests : BaseTest
    {
        [TestMethod]
        public void TestIsEOPRunning()
        {
            appHost.Start(session =>
            {
                var isRunning = global::IFM.DataServicesCore.BusinessLogic.Diamond.EOP.IsEOPRunning();                
                Assert.IsTrue(isRunning == false);
            });


        }

        [TestMethod]
        public void TestGetEOPStatus()
        {
            appHost.Start(session =>
            {
                
                //var status = global::IFM.DataServicesCore.BusinessLogic.Diamond.EOP.EOPStatusDetails();
                //Assert.IsTrue(status != null);

                var isRunning = global::IFM.DataServicesCore.BusinessLogic.Diamond.EOP.IsEOPRunning();
                if (isRunning ?? false)
                {
                    var status = global::IFM.DataServicesCore.BusinessLogic.Diamond.EOP.EOPStatusDetails();
                    Assert.IsTrue(status != null);
                }                
                
            });


        }
    }
}
