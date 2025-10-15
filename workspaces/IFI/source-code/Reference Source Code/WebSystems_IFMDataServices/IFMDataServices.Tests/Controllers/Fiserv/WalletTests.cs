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
    public class WalletTests :BaseTest
    {
        [TestMethod]
        public void TestUpdateWalletItem()
        {
            var updateWalletItem = new UpdateWalletItemBody()
            {
                IsFromOneView = false,
                UserName = "dgugenheim@indianafarmers.com",
                UserType = IFM_CreditCardProcessing.Enums.UserType.Policyholder
            };

            updateWalletItem.Item = new WalletItem()
            {
                FiservWalletId = 14442,
                Nickname = "VISA",
                FundingAccountToken = "F9FA55A5-3817-4E5D-B215-B03B94A5BE6F",
                ExpirationMonth = "4",
                ExpirationYear = "2024",
                BankAccountType = "Saving",
                ZipCode = "46122",
                FiservWalletItemId = 11671
            };

            appHost.Start(session =>
            {
                var data = ToJson(updateWalletItem);
                var result = session.PostJson("Fiserv/Wallets/UpdateWalletItemTesting", data);
                var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<APIResponses.Fiserv.FiservUpdateWalletResult>>(result);
                // var returnedData = this.DeserializeServiceResponseData<List<IFM.DataServicesCore.CommonObjects.OMP.AccountRegistedPolicy>>(sr);
                var returnedData = this.DeserializeServiceResponseData(sr);
                Assert.IsTrue(returnedData.Success);
             });
        }       
    }
}
