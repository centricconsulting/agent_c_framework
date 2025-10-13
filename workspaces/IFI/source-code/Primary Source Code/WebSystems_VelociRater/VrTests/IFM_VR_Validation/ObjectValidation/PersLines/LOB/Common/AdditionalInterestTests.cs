using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.Common
{
    [TestClass]
    public class AdditionalInterestTests : VRQQLibBase
    {
        [TestMethod]
        public void AdditionalInterestItem_Tests()
        {
            var ai = new QuickQuote.CommonObjects.QuickQuoteAdditionalInterest();

            var valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate;

            // testing type - Valid
            ai.BillTo = true;
            ai.TypeId = "41";
            var vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.AdditionalInterestValidation(ai, valType);
            Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.AIType), "Typeid is valid");

            // testing type - Invalid
            ai.BillTo = true;
            ai.TypeId = "";
            vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.AdditionalInterestValidation(ai, valType);
            Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.AIType), "Typeid is invalid");

            // testing billto mortgagee - valid
            ai.BillTo = true;
            ai.TypeId = "42";
            vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.AdditionalInterestValidation(ai, valType);
            Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.BillToMortgageeIneligible), "Bill to of mortgagee is valid");

            // testing billto mortgagee - invalid
            ai.BillTo = true;
            ai.TypeId = "40";
            vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.AdditionalInterestValidation(ai, valType);
            Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.AdditionalInterestValidator.BillToMortgageeIneligible), "Bill to of mortgagee is invalid");

            // not going to test name/phone/address here just because it is well tested in other places - might do it later
        }
    }
}