using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFM.PrimitiveExtensions;
using APIResponses = IFM.DataServices.API.ResponseObjects;
using System.Collections.Generic;
using Newtonsoft.Json;
using IFM.DataServicesCore.CommonObjects.OMP;

namespace IFM.DataServices.Tests.Controllers.OMP.Print
{
    [TestClass]
    public class DistributionTest : BaseTest
    {
        [TestMethod]
        //[DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/OMP/Print/TestCases.xml", "testLoad", DataAccessMethod.Sequential)]
        public void TestLoadPolicyPrintDistribution()
        {
            //CommonContextItems tci = GetCommonTestContextItems(TestContext);

            int PolicyId = 1593066;// TestContext.DataRow["policyId"].ToString().TryToGetInt32();

            appHost.Start(session =>
            {
                var result = session.Get($"OMP/Print/Distribution/PolicyPrintDistribution/{PolicyId}");
                var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<global::IFM.DataServicesCore.CommonObjects.OMP.PrintDistribution>>(result);
                var returnedData = this.DeserializeServiceResponseData(sr);
                Assert.AreEqual(PolicyId, returnedData.PolicyId);
                //DoBasicResposeTestsWithData(sr, returnedData, tci);

                //Assert.AreEqual(tci.ExpectsErrors, sr.HasErrors);
            });

        }

        [TestMethod]        
        public void TestUpdatePolicyPrintDistribution()
        {            
            int PolicyId = 2131897;

            appHost.Start(session =>
            {
                global::IFM.DataServicesCore.CommonObjects.OMP.PrintDistribution currentRecord = null;
                {
                    var result = session.Get($"OMP/Print/Distribution/PolicyPrintDistribution/{PolicyId}");
                    var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<global::IFM.DataServicesCore.CommonObjects.OMP.PrintDistribution>>(result);
                    currentRecord = this.DeserializeServiceResponseData(sr);
                }

                {
                    var payLoad = currentRecord;
                    payLoad.EmailAddress = "sthompson@indianafarmers.com";
                    payLoad.PaymentReminderNotification = false;
                    payLoad.PaymentPostedNotification = false;
                    payLoad.PolicyId = PolicyId;
                    var jsonText = ToJson(payLoad);
                    var result = session.PostJson($"OMP/Print/Distribution/PolicyPrintDistribution", jsonText);
                    var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<bool>>(result);
                    var returnedData = this.DeserializeServiceResponseData(sr);
                    Assert.IsTrue(returnedData);
                }
            });

        }
       [TestMethod]
        public void TestBulkSavePolicyPrintDistribution()
        {
            //int PolicyId = 213358;
            //List<global::IFM.DataServicesCore.CommonObjects.OMP.PrintDistribution> settings = new List<DataServicesCore.CommonObjects.OMP.PrintDistribution>();
            List<PrintDistribution> printDistribution = new List<PrintDistribution>();
            printDistribution.Add(new PrintDistribution() { PolicyId =883350 , SendEmails = false, SendTexts = false });//
            printDistribution.Add(new PrintDistribution() { PolicyId = 2334453, SendEmails = false, SendTexts = false });
            appHost.Start(session =>
            {
                
                     
                    
                    var jsonText = ToJson(printDistribution);
                    //var jsonText = ToJson(payLoad);
                    var result = session.PostJson($"OMP/Print/Distribution/BulkSavePolicyPrintDistribution", jsonText);
                var sr = DeserializeServiceResponse<APIResponses.Common.ServiceResult<global::IFM.DataServicesCore.CommonObjects.OMP.PrintDistribution>>(result);
                //DeserializeServiceResponse<APIResponses.Common.ServiceResult<bool>>(result);
                    var returnedData = this.DeserializeServiceResponseData(sr);
                Assert.IsFalse(sr.HasErrors);

            });

        }

    }
}
