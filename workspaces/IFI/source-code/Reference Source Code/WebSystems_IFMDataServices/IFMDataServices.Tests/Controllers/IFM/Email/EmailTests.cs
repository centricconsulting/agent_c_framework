using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Tests.Controllers.IFM
{
    [TestClass]
    public class EmailTests:BaseTest
    {
        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/IFM/Email/TestCases.xml", "TestEmailDocument", DataAccessMethod.Sequential)]
        public void TestEmailDocument()
        {
            CommonContextItems tci = GetCommonTestContextItems(TestContext);
            string toAddress = TestContext.DataRow["toAddress"].ToString();
            string subject = TestContext.DataRow["subject"].ToString();
            string body = TestContext.DataRow["body"].ToString();

            appHost.Start(session =>
            {
                var eDoc = new global::IFM.DataServicesCore.CommonObjects.EmailDocument
                {
                    ToAddress = toAddress,
                    Subject = subject,
                    Body = body
                };

                var result = session.PostJson("ifm/email/document",ToJson(eDoc));
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);                

                 DoBasicResponseTests(sr,tci);                

            });
        }
    }
}
