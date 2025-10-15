using IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.PPA
{
    [TestClass]
    public class ScheduledItemsValidator : VRQQLibBase
    {
        [TestMethod]
        public void ScheduledItemsValidator_Tests()
        {
            var q = GetNewQuickQuote();
            q.LobId = "1";
            var v = new QuickQuote.CommonObjects.QuickQuoteVehicle();
            v.ScheduledItems = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteScheduledItem>();
            var si = new QuickQuote.CommonObjects.QuickQuoteScheduledItem();
            q.Vehicles = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteVehicle>();
            q.Vehicles.Add(v);
            v.ScheduledItems.Add(si);

            // testing EMPTY
            var vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.ScheduledItemValidator_PPA.ScheduledItemViolation(si);
            Assert.IsTrue(vals.ListHasValidationId(ScheduledItemValidator_PPA.EquipmentDescription), PrintTestValue(si.Description));
            Assert.IsTrue(vals.ListHasValidationId(ScheduledItemValidator_PPA.EquipmentAmount), PrintTestValue(si.Amount));

            // testing Valid
            si.Description = "1";
            si.Amount = "500";
            vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.ScheduledItemValidator_PPA.ScheduledItemViolation(si);
            Assert.IsFalse(vals.ListHasValidationId(ScheduledItemValidator_PPA.EquipmentDescription), PrintTestValue(si.Description));
            Assert.IsFalse(vals.ListHasValidationId(ScheduledItemValidator_PPA.EquipmentAmount), PrintTestValue(si.Amount));

            // testing InValid
            si.Description = "";
            si.Amount = "abc";
            vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.ScheduledItemValidator_PPA.ScheduledItemViolation(si);
            Assert.IsTrue(vals.ListHasValidationId(ScheduledItemValidator_PPA.EquipmentDescription), PrintTestValue(si.Description));
            Assert.IsTrue(vals.ListHasValidationId(ScheduledItemValidator_PPA.EquipmentAmount), PrintTestValue(si.Amount));

            // testing InValid
            si.Description = "";
            si.Amount = "-500";
            vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.ScheduledItemValidator_PPA.ScheduledItemViolation(si);
            Assert.IsTrue(vals.ListHasValidationId(ScheduledItemValidator_PPA.EquipmentDescription), PrintTestValue(si.Description));
            Assert.IsTrue(vals.ListHasValidationId(ScheduledItemValidator_PPA.EquipmentAmount), PrintTestValue(si.Amount));
        }
    }
}