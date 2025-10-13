using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VrTests.VR_Common.Helpers
{
    [TestClass]
    public class ProtectionClassLookupTest
    {
        [TestMethod]
        public void ProtectionClassLookup()
        {
            var results = IFM.VR.Common.Helpers.ProtectionClassLookupHelper.GetProtectionClassRawData("Thorntown", true, 16, false);

            Assert.IsNotNull(results, "[Is Null]");
            if (results != null)
            {
                Assert.IsTrue(results.Count == 1, "Expected one result");
                if (results.Count == 1)
                {
                    Assert.IsTrue(results[0].County == "BOONE", "County is invalid.");
                }
            }
        }
    }
}