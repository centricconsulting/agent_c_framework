using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace VrTests.VR_Common.Helpers
{
    [TestClass]
    public class AgencyProducers : VRQQLibBase
    {
        [TestMethod]
        public void AgencyProducers_GetProducersByAgencyId()
        {
            Assert.IsFalse(IFM.VR.Common.Helpers.AgencyProducers.GetProducersByAgencyId(0).Any());
            Assert.IsTrue(IFM.VR.Common.Helpers.AgencyProducers.GetProducersByAgencyId(17).Any());
        }
    }
}