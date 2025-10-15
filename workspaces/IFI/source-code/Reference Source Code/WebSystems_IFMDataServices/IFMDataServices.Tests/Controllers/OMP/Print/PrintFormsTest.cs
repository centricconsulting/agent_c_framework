using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFM.PrimitiveExtensions;

namespace IFM.DataServices.Tests.Controllers.OMP.Print
{
    [TestClass]
    public class PrintTest : BaseTest
    {
        [TestMethod]
        //[DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/OMP/Print/PrintTest_TestCases.xml", "testLoad", DataAccessMethod.Sequential)]
        public void TestPrintPDF()
        {
            //CommonContextItems tci = GetCommonTestContextItems(TestContext);
            //int PolicyId = TestContext.DataRow["policyId"].ToString().TryToGetInt32();
            //int XmlId = TestContext.DataRow["xmlId"].ToString().TryToGetInt32();
            //string Description = TestContext.DataRow["descriptionParam"].ToString();
            //int PrintFormNumber = TestContext.DataRow["printFormNumber"].ToString().TryToGetInt32();
            //string encodedDecription = System.Web.HttpUtility.UrlEncode(Description);

            appHost.Start(session =>
            {
                var result = session.Get($"OMP/Print/Document/247750/16043441/Auto ID Cards/484");
                //var result = session.Get($"OMP/Print/Document/{PolicyId}/{XmlId}/{encodedDecription}/{PrintFormNumber}");
                //DoBasicResposeTestsForPDFReturn(result, tci);
                DoBasicResponseTestsForPDFReturn(result, true);
            });
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "Controllers/OMP/Print/PrintTest_TestCases.xml", "TestPrintPDFByPrintURL", DataAccessMethod.Sequential)]
        public void TestPrintPDFByPrintURL()
        {
            CommonContextItems tci = GetCommonTestContextItems(TestContext);
            string PrintURL = TestContext.DataRow["printURL"].ToString();

            appHost.Start(session =>
            {
                var result = session.Get($"{PrintURL}");
                DoBasicResponseTestsForPDFReturn(result, tci);
            });
        }
    }
}