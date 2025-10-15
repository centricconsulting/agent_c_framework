using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.HOM
{
    [TestClass]
    public class RvWatercraftTests : VRQQLibBase
    {
        [TestMethod]
        public void RvWaterCraft_AppSide_Tests()
        {
            //var q = GetNewQuickQuote();
            //q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
            //var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
            //q.Locations.Add(l);
            //l.RvWatercrafts = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteRvWatercraft>();
            //var rv = new QuickQuote.CommonObjects.QuickQuoteRvWatercraft();
            //l.RvWatercrafts.Add(rv);

            //// testing empty - valType of non-quote
            //var valTYpe = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.app;
            //rv.RvWatercraftTypeId = "7";
            //rv.SerialNumber = "";
            //rv.Manufacturer = "";
            //rv.Model = "";
            //var vals = RvWaterCraftValidator.ValidateRvWaterCraft(rv, valTYpe);
            //Assert.IsTrue(vals.ListHasValidationId(RvWaterCraftValidator.SerialNumberMissing), PrintTestValue(rv.SerialNumber));
            //Assert.IsTrue(vals.ListHasValidationId(RvWaterCraftValidator.ManufacturerMissing), PrintTestValue(rv.Manufacturer));
            //Assert.IsTrue(vals.ListHasValidationId(RvWaterCraftValidator.ModelMissing), PrintTestValue(rv.Model));

            //// testing empty - val type of quote
            //valTYpe = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
            //rv.RvWatercraftTypeId = "7";
            //rv.SerialNumber = "";
            //rv.Manufacturer = "";
            //rv.Model = "";
            //vals = RvWaterCraftValidator.ValidateRvWaterCraft(rv, valTYpe);
            //Assert.IsFalse(vals.ListHasValidationId(RvWaterCraftValidator.SerialNumberMissing), PrintTestValue(rv.SerialNumber));
            //Assert.IsFalse(vals.ListHasValidationId(RvWaterCraftValidator.ManufacturerMissing), PrintTestValue(rv.Manufacturer));
            //Assert.IsFalse(vals.ListHasValidationId(RvWaterCraftValidator.ModelMissing), PrintTestValue(rv.Model));

            //// testing invalid - valType of non-quote
            //valTYpe = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.app;
            //rv.RvWatercraftTypeId = "7";
            //rv.SerialNumber = "123";
            //rv.Manufacturer = "^&*%^*";
            //rv.Model = "^&*^&*(";
            //vals = RvWaterCraftValidator.ValidateRvWaterCraft(rv, valTYpe);
            //Assert.IsTrue(vals.ListHasValidationId(RvWaterCraftValidator.SerialNumberInvalid), PrintTestValue(rv.SerialNumber));
            //Assert.IsTrue(vals.ListHasValidationId(RvWaterCraftValidator.ManufacturerInvalid), PrintTestValue(rv.Manufacturer));
            //Assert.IsTrue(vals.ListHasValidationId(RvWaterCraftValidator.ModelInvalid), PrintTestValue(rv.Model));

            //// testing valid - valType of non-quote
            //valTYpe = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.app;
            //rv.RvWatercraftTypeId = "7";
            //rv.SerialNumber = "12345678912345612345641234";
            //rv.Manufacturer = "man";
            //rv.Model = "model";
            //vals = RvWaterCraftValidator.ValidateRvWaterCraft(rv, valTYpe);
            //Assert.IsFalse(vals.ListHasValidationId(RvWaterCraftValidator.SerialNumberInvalid), PrintTestValue(rv.SerialNumber));
            //Assert.IsFalse(vals.ListHasValidationId(RvWaterCraftValidator.ManufacturerInvalid), PrintTestValue(rv.Manufacturer));
            //Assert.IsFalse(vals.ListHasValidationId(RvWaterCraftValidator.ModelInvalid), PrintTestValue(rv.Model));

            //// testing valid - Type Accessory - valType of non-quote
            //valTYpe = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.app;
            //rv.RvWatercraftTypeId = "5";
            //rv.SerialNumber = ""; // not required
            //rv.Manufacturer = ""; // not required
            //rv.Model = ""; // not required
            //vals = RvWaterCraftValidator.ValidateRvWaterCraft(rv, valTYpe);
            //Assert.IsFalse(vals.ListHasValidationId(RvWaterCraftValidator.SerialNumberMissing), PrintTestValue(rv.SerialNumber));
            //Assert.IsFalse(vals.ListHasValidationId(RvWaterCraftValidator.ManufacturerMissing), PrintTestValue(rv.Manufacturer));
            //Assert.IsFalse(vals.ListHasValidationId(RvWaterCraftValidator.ModelMissing), PrintTestValue(rv.Model));
        }
    }
}