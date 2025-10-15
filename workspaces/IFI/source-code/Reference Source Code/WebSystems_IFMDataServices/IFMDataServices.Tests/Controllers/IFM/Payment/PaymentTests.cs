using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFM.PrimitiveExtensions;
using System.Diagnostics;
using Diamond.Business.ThirdParty.QwikSignAPI;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Tests.Controllers.IFM.Payment
{
    [TestClass]
    public class PaymentTests :BaseTest
    {
        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/IFM/Payment/TestCases.xml", "testMakePayment", DataAccessMethod.Sequential)]
        public void TestPaymentData()
        {
            CommonContextItems tci = GetCommonTestContextItems(TestContext);

            var PaymentData = new global::IFM.DataServicesCore.CommonObjects.Payments.MemberPortalPaymentData()
            {
                PolicyNumber = TestContext.DataRow["policyNumber"].ToString(),
                AccountNumber = TestContext.DataRow["accountNumber"].ToString(),
                PolicyId = TestContext.DataRow["policyId"].ToString().TryToGetInt32(),
                ImageNumber = TestContext.DataRow["imageNumber"].ToString().TryToGetInt32(),
                UserAgreedWithTerms = Convert.ToBoolean(TestContext.DataRow["agreedWithTerms"].ToString()),
                PaymentAmount = TestContext.DataRow["paymentamount"].ToString().TryToGetDouble(),
                UserName = TestContext.DataRow["username"].ToString(),
                UserType = (global::IFM_CreditCardProcessing.Enums.UserType) Convert.ToInt32(TestContext.DataRow["usertype"].ToString()),
                EmailAddress = TestContext.DataRow["emailAddress"].ToString()
            };

            if (Convert.ToBoolean(TestContext.DataRow["echeckUseData"].ToString()))
            {
                PaymentData.ECheckPaymentInformation = new global::IFM.DataServices.API.RequestObjects.Payments.ECheckPaymentInformation
                {
                    AccountNumber = TestContext.DataRow["echeckAccountNumber"].ToString(),
                    AccountType = TestContext.DataRow["echeckAccountType"].ToString().TryToGetInt32(),
                    RoutingNumber = TestContext.DataRow["echeckRoutingNumber"].ToString()
                };
            }

            if (Convert.ToBoolean(TestContext.DataRow["ccUseData"].ToString()))
            {
                PaymentData.CreditCardPaymentInformation = new global::IFM.DataServices.API.RequestObjects.Payments.CreditCardPaymentInformation
                {
                    CardNumber = TestContext.DataRow["ccNumber"].ToString(),
                    CardExpireMonth = TestContext.DataRow["ccExpireMonth"].ToString().TryToGetInt32(),
                    CardExpireYear = TestContext.DataRow["ccExpireYear"].ToString().TryToGetInt32(),
                    SecurityCode = TestContext.DataRow["ccSecurityCode"].ToString(),
                    NameOnCard = "Card HoldersName",
                    ZIPCode = "46041"
                };
            }

            appHost.Start(session =>
            {
                var json = ToJson(PaymentData);
                var result = session.PostJson($"IFM/Payment/Processor/paymentdata", json);
                var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentServiceResult>>(result);
                var returnedData = this.DeserializeServiceResponseData(sr);

                DoBasicResponseTestsWithData(sr, returnedData, tci);

                Assert.AreEqual(tci.ExpectsErrors, sr.HasErrors);
            });
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/IFM/Payment/TestCases.xml", "testMakePayment", DataAccessMethod.Sequential)]
        public void TestMakePayment()
        {
            CommonContextItems tci = GetCommonTestContextItems(TestContext);

            var PaymentData = new global::IFM.DataServices.API.RequestObjects.Payments.PaymentData()
            {
                PolicyNumber = TestContext.DataRow["policyNumber"].ToString(),
                AccountBillNumber = TestContext.DataRow["accountNumber"].ToString(),
                PolicyId = TestContext.DataRow["policyId"].ToString().TryToGetInt32(),
                PolicyImageNumber = TestContext.DataRow["imageNumber"].ToString().TryToGetInt32(),
                UserAgreedWithTerms = Convert.ToBoolean(TestContext.DataRow["agreedWithTerms"].ToString()),
                PaymentAmount = TestContext.DataRow["paymentamount"].ToString().TryToGetDouble(),
                Username = TestContext.DataRow["username"].ToString(),
                UserId = TestContext.DataRow["userID"].TryToGetInt32(),
                UserType = (global::IFM.DataServices.API.Enums.UserType)Convert.ToInt32(TestContext.DataRow["usertype"].ToString()),
                EmailAddress = TestContext.DataRow["emailAddress"].ToString(),
                CashInSource = (API.Enums.CashSource)TestContext.DataRow["cashinsource"].TryToGetInt32(),
                PaymentInterface = (API.Enums.PaymentInterface)TestContext.DataRow["paymentinterface"].TryToGetInt32()
            };

            if (Convert.ToBoolean(TestContext.DataRow["eftUseData"].ToString()))
            {
                PaymentData.EFTInformation = new global::IFM.DataServices.API.RequestObjects.Payments.EFTInformation
                {
                    AgencyEFTAccountId = TestContext.DataRow["eftAccountId"].TryToGetInt32(),
                    LegacyUserId = TestContext.DataRow["eftLegacyUserId"].TryToGetInt32(),
                    LegacyAgencyId = TestContext.DataRow["eftLegacyAgencyId"].TryToGetInt32()
                };
            }

            if (Convert.ToBoolean(TestContext.DataRow["echeckUseData"].ToString()))
            {
                PaymentData.ECheckPaymentInformation = new global::IFM.DataServices.API.RequestObjects.Payments.ECheckPaymentInformation
                {
                    AccountNumber = TestContext.DataRow["echeckAccountNumber"].ToString(),
                    AccountType = TestContext.DataRow["echeckAccountType"].ToString().TryToGetInt32(),
                    RoutingNumber = TestContext.DataRow["echeckRoutingNumber"].ToString()
                };
            }

            if (Convert.ToBoolean(TestContext.DataRow["ccUseData"].ToString()))
            {
                PaymentData.CreditCardPaymentInformation = new global::IFM.DataServices.API.RequestObjects.Payments.CreditCardPaymentInformation
                {
                    CardNumber = TestContext.DataRow["ccNumber"].ToString(),
                    CardExpireMonth = TestContext.DataRow["ccExpireMonth"].ToString().TryToGetInt32(),
                    CardExpireYear = TestContext.DataRow["ccExpireYear"].ToString().TryToGetInt32(),
                    SecurityCode = TestContext.DataRow["ccSecurityCode"].ToString(),
                    NameOnCard = "Card HoldersName",
                    ZIPCode = "46041"
                };
            }

            appHost.Start(session =>
            {

                var json = ToJson(PaymentData);
                Stopwatch sw = Stopwatch.StartNew();
                var result = session.PostJson($"IFM/Payment/Processor/makepayment", json);
                sw.Stop();
                var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult>>(result);
                //var returnedData = this.DeserializeServiceResponseData<APIResponses.Payments.PaymentResult>(sr);

                DoBasicResponseTestsWithData(sr, sr.ResponseData, tci);

                Assert.AreEqual(tci.ExpectsErrors, sr.HasErrors);
            });
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/IFM/Payment/TestCases.xml", "testPostPayment", DataAccessMethod.Sequential)]
        public void TestPostPayment()
        {
            CommonContextItems tci = GetCommonTestContextItems(TestContext);

            var PaymentData = new global::IFM.DataServices.API.RequestObjects.Payments.PaymentData("http://ifmdiapatch:1080")
            {
                PolicyNumber = TestContext.DataRow["policyNumber"].ToString(),
                AccountBillNumber = TestContext.DataRow["accountNumber"].ToString(),
                PolicyId = TestContext.DataRow["policyId"].ToString().TryToGetInt32(),
                PolicyImageNumber = TestContext.DataRow["imageNumber"].ToString().TryToGetInt32(),
                PaymentAmount = TestContext.DataRow["paymentamount"].ToString().TryToGetDouble(),
                CashInSource = API.Enums.CashSource.CreditCard,
                UserId = TestContext.DataRow["userID"].TryToGetInt32()
            };

            var returnData = PaymentData.ApplyCash();
            if (returnData?.ResponseData != null)
            {
                Assert.IsTrue(returnData.ResponseData.PaymentCompleted);
            }

            //appHost.Start(session =>
            //{
            //    var json = ToJson(PaymentData);
            //    var result = session.PostJson($"IFM/Payment/Processor/ApplyCash", json);
            //    var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<APIResponses.Payments.PaymentResult>>(result);

            //    DoBasicResposeTestsWithData(sr, sr.ResponseData, tci);

            //    Assert.AreEqual(tci.ExpectsErrors, sr.HasErrors);
            //});
        }

        [TestMethod]
        public void TestGetPendingPayments()
        {
            appHost.Start(session =>
            {
                var pendingPayments = global::IFM.DataServicesCore.CommonObjects.OMP.UnSweptPayment.GetPendingPaymentList("HOM2113324");
                Debugger.Break();
            });


        }
    }
}
