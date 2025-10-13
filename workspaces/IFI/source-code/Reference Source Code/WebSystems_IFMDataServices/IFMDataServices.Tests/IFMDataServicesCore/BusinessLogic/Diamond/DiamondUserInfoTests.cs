using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IFM.DataServices.Tests.IFMDataServicesCore.BusinessLogic.Diamond
{
    [TestClass]
    public class DiamondUserInfoTests : BaseTest
    {
        [TestMethod]
        public void TestGetUserInfo()
        {
            appHost.Start(session =>
            {
                var userInfo = new global::IFM.DataServicesCore.BusinessLogic.Diamond.UserInfo();
                userInfo.GetUserInfo(272);
                if (1 == 1) { }
            });
        }

    }
}
