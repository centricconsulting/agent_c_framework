using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IFM.DataServices.Tests.IFMDataServicesCore.BusinessLogic.Diamond
{
    [TestClass]
    public class SystemDateTests : BaseTest
    {
        [TestMethod]
        public void TestSystemDate()
        {
            appHost.Start(session =>
            {
                var sy = new global::IFM.DataServicesCore.BusinessLogic.Diamond.SystemDate();
                var currentSystemDate = sy.GetSystemDate();
                Assert.IsTrue(currentSystemDate > DateTime.MinValue);
            });

            
        }
    }
}
