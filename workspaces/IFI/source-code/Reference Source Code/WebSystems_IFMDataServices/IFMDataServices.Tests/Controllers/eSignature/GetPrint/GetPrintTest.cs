using System;
using System.IO;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Tests.Controllers.eSignature.GetPrint
{
    [TestClass]
    public class GetPrintTest:BaseTest
    {
        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/eSignature/GetPrint/TestCases.xml", "getPrintInfo", DataAccessMethod.Sequential)]
        public void TestGetPrint()
        {
            CommonContextItems tci = GetCommonTestContextItems(TestContext);
            string policyNumber = TestContext.DataRow["policyNumber"].ToString();
            string printFormCategories = TestContext.DataRow["printFormCategories"].ToString();

            appHost.Start(session =>
            {
                var result = session.Get($"eSignature/Print/GetPrintForEsignature/{policyNumber}/{printFormCategories}");
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);

                if (sr.ResponseData != null)
                {
                    var myPDF = Convert.FromBase64String(sr.ResponseData.ToString());
                    System.IO.File.WriteAllBytes(@"C:\users\dagug\desktop\DiamondPDF-" + policyNumber + ".pdf", myPDF);
                }
                else
                {

                }

                

            });
        }
    }
}
