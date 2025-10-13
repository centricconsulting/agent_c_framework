using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IFM.PrimativeExtensions;

namespace VrTests.VR_Common.Helpers
{
    [TestClass]
    public class LossHistoryHelper_HOMTests : VRQQLibBase
    {
        [TestMethod]
        public void LossHistoryHelper_HOM_GetAllHOMLosses()
        {
            var q = new QuickQuote.CommonObjects.QuickQuoteObject();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;

            q.Applicants = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteApplicant>();
            var app = new QuickQuote.CommonObjects.QuickQuoteApplicant();
            q.Applicants.Add(app);

            Assert.IsNull(IFM.VR.Common.Helpers.HOM.LossHistoryHelper_HOM.GetAllHOMLosses(q), "Expected null");

            app.LossHistoryRecords = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord>();
            var lh = new QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord();
            app.LossHistoryRecords.Add(lh);

            Assert.IsNotNull(IFM.VR.Common.Helpers.HOM.LossHistoryHelper_HOM.GetAllHOMLosses(q), "Expected a non-null result");
            Assert.IsTrue(IFM.VR.Common.Helpers.HOM.LossHistoryHelper_HOM.GetAllHOMLosses(q).Count == 1, "Expected loss count of one.");

            q.LossHistoryRecords = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord>();
            q.LossHistoryRecords.Add(lh);

            Assert.IsTrue(IFM.VR.Common.Helpers.HOM.LossHistoryHelper_HOM.GetAllHOMLosses(q).Count == 2, "Expected loss count of two.");
        }

        [TestMethod]
        public void LossHistoryHelper_HOM_GetSurchargeableHOMLosses()
        {
            var q = new QuickQuote.CommonObjects.QuickQuoteObject();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;

            q.LossHistoryRecords = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord>();
            var lh = new QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord();
            q.LossHistoryRecords.Add(lh);

            // BUG 6482 States - Surcharges do not apply to HO4, HO6, ML2 and ML4 form types
            q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
            q.Locations.Add(new QuickQuote.CommonObjects.QuickQuoteLocation());
            q.Locations[0].FormTypeId = "1";

            Assert.IsTrue(IFM.VR.Common.Helpers.HOM.LossHistoryHelper_HOM.GetSurchargeableHOMLosses(q).Count == 0, "Expected loss count of zero.");

            // tests
            // amount must be > $500.99
            // LossHistorySurchargeId must = '1'
            // LossDate must be less than 5 years ago

            lh.Amount = "501.99";
            lh.LossHistorySurchargeId = "1";
            lh.LossDate = DateTime.Now.ToShortDateString();
            Assert.IsTrue(IFM.VR.Common.Helpers.HOM.LossHistoryHelper_HOM.GetSurchargeableHOMLosses(q).Count == 1, "Expected loss count of one.");

            lh.Amount = "500.99"; // to low to pass
            lh.LossHistorySurchargeId = "1";
            lh.LossDate = DateTime.Now.ToShortDateString();
            Assert.IsTrue(IFM.VR.Common.Helpers.HOM.LossHistoryHelper_HOM.GetSurchargeableHOMLosses(q).Count == 0, "Expected loss count of zero. Amount to low.");

            lh.Amount = "501.99";
            lh.LossHistorySurchargeId = "1";
            lh.LossDate = DateTime.Now.AddYears(-5).AddDays(-2).ToShortDateString(); // to long ago to pass
            Assert.IsTrue(IFM.VR.Common.Helpers.HOM.LossHistoryHelper_HOM.GetSurchargeableHOMLosses(q).Count == 0, "Expected loss count of zero. Loss date years to high.");

            lh.Amount = "501.99";
            lh.LossHistorySurchargeId = "1";
            lh.LossDate = DateTime.Now.AddYears(-5).ToShortDateString();
            Assert.IsTrue(IFM.VR.Common.Helpers.HOM.LossHistoryHelper_HOM.GetSurchargeableHOMLosses(q).Count == 1, "Expected loss count of one. Just barely not over 5 years.");

            lh.Amount = "501.99";
            lh.LossHistorySurchargeId = "2"; // should be '1' to pass
            lh.LossDate = DateTime.Now.ToShortDateString();
            Assert.IsTrue(IFM.VR.Common.Helpers.HOM.LossHistoryHelper_HOM.GetSurchargeableHOMLosses(q).Count == 0, "Expected loss count of zero. Surcharge flag not '1'");
        }
    }
}