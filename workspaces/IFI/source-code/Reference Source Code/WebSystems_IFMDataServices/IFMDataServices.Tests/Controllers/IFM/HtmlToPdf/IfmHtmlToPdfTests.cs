using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IFM.DataServices.Tests.Controllers.IFM.HtmlToPdf
{
    [TestClass]
    public class IfmHtmlToPdfTests : BaseTest
    {
        [TestMethod]
        public void TestHtmlToPdf()
        {            
            appHost.Start(session =>
            {
                var controller = new global::IFM.DataServices.Controllers.IFM.HtmlToPdf.IFMHtmlToPdf_ProcessorController();
                var result = controller.GetWebPageAsPdf("http://www.indianafarmers.com");
                Assert.IsTrue(result.FileStream.Length > 500);

                result = controller.GetWebPageAsPdf("http://www.yahoo.com");
                Assert.IsTrue(result.FileStream.Length < 500);
            });
            
        }
    }
}
