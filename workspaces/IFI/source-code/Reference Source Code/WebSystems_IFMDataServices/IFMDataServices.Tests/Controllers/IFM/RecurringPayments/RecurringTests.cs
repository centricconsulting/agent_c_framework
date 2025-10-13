using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFM.PrimitiveExtensions;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using Diamond.Business.ThirdParty.QwikSignAPI;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Tests.Controllers.IFM.RecurringPayments
{
    [TestClass]
    public class RecurringTests : BaseTest
    {
        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/IFM/RecurringPayments/TestCases.xml", "testSetPayPlan", DataAccessMethod.Sequential)]
        public void TestSetPayPlan()
        {
            CommonContextItems tci = GetCommonTestContextItems(TestContext);

            var PayPlanData = new global::IFM.DataServicesCore.CommonObjects.Payments.PayPlanData
            {
                PayPlanId = (DataServicesCore.CommonObjects.Enums.Enums.PayPlans)TestContext.DataRow["payPlanId"].ToString().TryToGetInt32(),
                UserAgreedWithTerms = Convert.ToBoolean(TestContext.DataRow["agreedWithTerms"].ToString())
            };

            if (Convert.ToBoolean(TestContext.DataRow["eftUseData"].ToString()))
            {
                PayPlanData.RecurringEftInformation = new DataServicesCore.CommonObjects.Payments.RecurringEftInformation
                {
                    AccountNumber = TestContext.DataRow["eftAccountNumber"].ToString(),
                    AccountType = TestContext.DataRow["eftAccountType"].ToString().TryToGetInt32(),
                    RoutingNumber = TestContext.DataRow["eftRoutingNumber"].ToString(),
                    DeductionDay = Convert.ToInt32(TestContext.DataRow["eftDeductionDay"].ToString()),
                    EmailAddress = TestContext.DataRow["emailAddress"].ToString()
                };
            }


            if (Convert.ToBoolean(TestContext.DataRow["ccUseData"].ToString()))
            {
                PayPlanData.RecurringCreditCardInformation = new DataServicesCore.CommonObjects.Payments.RecurringCreditCardInformation
                {
                    CardNumber = TestContext.DataRow["ccNumber"].ToString(),
                    CardExpireMonth = Convert.ToInt32(TestContext.DataRow["ccExpireMonth"].ToString()),
                    CardExpireYear = Convert.ToInt32(TestContext.DataRow["ccExpireYear"].ToString()),
                    DeductionDay = Convert.ToInt32(TestContext.DataRow["ccDeductionDay"].ToString()),
                    EmailAddress = TestContext.DataRow["emailAddress"].ToString()
                };
            }

            appHost.Start(session =>
            {
                var testPolicy = GetTestPolicies().AsList()[0];

                PayPlanData.PolicyNumber = testPolicy.PolicyNumber;
                PayPlanData.ClientId = testPolicy.ClientId;
                PayPlanData.PolicyId = testPolicy.PolicyId;
                PayPlanData.ImageNumber = testPolicy.ImageNum;

                //PayPlanData.PolicyNumber = "PPA2050444";
                //PayPlanData.PolicyId = 1520437;
                //PayPlanData.ClientId = 112496;                
                //PayPlanData.ImageNumber = 1;
                if (PayPlanData?.RecurringCreditCardInformation != null) PayPlanData.RecurringCreditCardInformation.DeductionDay = 5;
                if (PayPlanData?.RecurringEftInformation != null) PayPlanData.RecurringEftInformation.DeductionDay = 5;

                PayPlanData.TransactionType = DataServicesCore.CommonObjects.Enums.Enums.BillingTransactionType.EditEftInfo;

                var result = session.PostJson($"IFM/RecurringPayments/Processor/payplan", ToJson(PayPlanData));
                var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<APIResponses.Payments.PayplanChangedResult>>(result);
                var returnedData = this.DeserializeServiceResponseData(sr);

                DoBasicResponseTestsWithData(sr, returnedData, tci);

                Assert.AreEqual(tci.ExpectsErrors, sr.HasErrors);


            });



        }


        private IEnumerable<PolicyTestData> GetTestPolicies()
        {
            var results = new List<PolicyTestData>();

            string queryText = "SELECT TOP 20 current_policy as PolicyNumber, client_id as ClientId, policy_id as PolicyId, lastimage_num as ImageNum FROM [Diamond].[dbo].[Policy] where policycurrentstatus_id = 2 order by policy_id desc";
            using (IDbConnection conn = OpenConnection(global::IFM.DataServicesCore.BusinessLogic.AppConfig.ConnDiamond))
            {
                results = conn.Query<PolicyTestData>(queryText,commandType: CommandType.Text).AsList();
            }
            return results;
        }

        [TestMethod]
        public void TestGetRecurringData()
        {
            string policyNumber = "PPA2050444";
            int policyId = 1520437;
            appHost.Start(session =>
            {
                var result = session.Get($"IFM/RecurringPayments/Processor/payplan/{policyNumber}/{policyId}");
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                
            });
        }

        
               


    }

    public class PolicyTestData
    {   
        
        public string PolicyNumber { get; set; }
        
        public int ClientId { get; set; }
        
        public int PolicyId { get; set; }
        
        public int ImageNum { get; set; }

    }


}
