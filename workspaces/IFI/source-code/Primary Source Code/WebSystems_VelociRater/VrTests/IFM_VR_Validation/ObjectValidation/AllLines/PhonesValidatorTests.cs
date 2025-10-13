using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace VrTests.IFM_VR_Validation.ObjectValidation.AllLines
{
    [TestClass]
    public class PhonesValidatorTests : VRQQLibBase
    {
        [TestMethod]
        public void PhonesListTests()
        {
            var phonesList = new List<QuickQuote.CommonObjects.QuickQuotePhone>();
            phonesList.Add(new QuickQuote.CommonObjects.QuickQuotePhone());
            var p = phonesList[0];

            // testing empty
            p.Number = "";
            p.Extension = "";
            p.TypeId = "";
            var vals = IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.ValidatePhoneList(phonesList, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneNumberEmpty), PrintTestValue(p.Number));
            Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneExtensionEmpty), PrintTestValue(p.Extension));
            Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneTypeEmpty), PrintTestValue(p.TypeId));

            // testing Invalid
            p.Number = "abc";
            p.Extension = "sdhh";
            p.TypeId = "u";
            vals = IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.ValidatePhoneList(phonesList, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneNumberInvalid), PrintTestValue(p.Number));
            Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneExtensionInvalid), PrintTestValue(p.Extension));
            Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneTypeInvalid), PrintTestValue(p.TypeId));

            // testing Valid
            p.Number = "(123)456-2314";
            p.Extension = "222";
            p.TypeId = "2";
            vals = IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.ValidatePhoneList(phonesList, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneNumberEmpty), PrintTestValue(p.Number));
            Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneExtensionEmpty), PrintTestValue(p.Extension));
            Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneTypeEmpty), PrintTestValue(p.TypeId));
            Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneNumberInvalid), PrintTestValue(p.Number));
            Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneExtensionInvalid), PrintTestValue(p.Extension));
            Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneTypeInvalid), PrintTestValue(p.TypeId));

            // testing InValid - No Phone# so should not have extension or phone type
            p.Number = "";
            p.Extension = "222";
            p.TypeId = "2";
            vals = IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.ValidatePhoneList(phonesList, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneNumberEmpty), PrintTestValue(p.Number));
            Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneExtensionInvalid), PrintTestValue(p.Extension));
            Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneTypeInvalid), PrintTestValue(p.TypeId));
        }
    }
}