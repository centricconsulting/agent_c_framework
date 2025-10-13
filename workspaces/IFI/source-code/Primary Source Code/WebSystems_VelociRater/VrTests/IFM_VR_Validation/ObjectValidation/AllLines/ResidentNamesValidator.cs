using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VrTests.IFM_VR_Validation.ObjectValidation.AllLines
{
    [TestClass]
    public class ResidentNamesValidator : VRQQLibBase
    {
        [TestMethod]
        public void ResidentName_Tests()
        {
            var qq = GetNewQuickQuote();
            qq.LobId = "17";
            qq.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>() { new QuickQuote.CommonObjects.QuickQuoteLocation() };

            var loc = qq.Locations[0];
            loc.ResidentNames = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteResidentName>();
            loc.ResidentNames.Add(new QuickQuote.CommonObjects.QuickQuoteResidentName());

            // test Empty
            loc.ResidentNames[0].Name.FirstName = "";
            loc.ResidentNames[0].Name.LastName = "";
            loc.ResidentNames[0].Name.BirthDate = "";
            loc.ResidentNames[0].Name.Salutation = "";
            var Validations = IFM.VR.Validation.ObjectValidation.FarmLines.ResidentNameValidator.ValidateResidentName(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsTrue(Validations.ListHasValidationId(IFM.VR.Validation.ObjectValidation.FarmLines.ResidentNameValidator.Resident_FirstName), "");
            Assert.IsTrue(Validations.ListHasValidationId(IFM.VR.Validation.ObjectValidation.FarmLines.ResidentNameValidator.Resident_LastName), "");
            Assert.IsTrue(Validations.ListHasValidationId(IFM.VR.Validation.ObjectValidation.FarmLines.ResidentNameValidator.Resident_DOB), "");
            Assert.IsTrue(Validations.ListHasValidationId(IFM.VR.Validation.ObjectValidation.FarmLines.ResidentNameValidator.Resident_Relationship), "");

            // test Valid
            loc.ResidentNames[0].Name.FirstName = "matt";
            loc.ResidentNames[0].Name.LastName = "amonett";
            loc.ResidentNames[0].Name.BirthDate = "7/1/1979";
            loc.ResidentNames[0].Name.Salutation = "the man";
            Validations = IFM.VR.Validation.ObjectValidation.FarmLines.ResidentNameValidator.ValidateResidentName(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsFalse(Validations.ListHasValidationId(IFM.VR.Validation.ObjectValidation.FarmLines.ResidentNameValidator.Resident_FirstName), "");
            Assert.IsFalse(Validations.ListHasValidationId(IFM.VR.Validation.ObjectValidation.FarmLines.ResidentNameValidator.Resident_LastName), "");
            Assert.IsFalse(Validations.ListHasValidationId(IFM.VR.Validation.ObjectValidation.FarmLines.ResidentNameValidator.Resident_DOB), "");
            Assert.IsFalse(Validations.ListHasValidationId(IFM.VR.Validation.ObjectValidation.FarmLines.ResidentNameValidator.Resident_Relationship), "");

            // test Invalid DOB
            loc.ResidentNames[0].Name.BirthDate = "abc";
            Validations = IFM.VR.Validation.ObjectValidation.FarmLines.ResidentNameValidator.ValidateResidentName(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsTrue(Validations.ListHasValidationId(IFM.VR.Validation.ObjectValidation.FarmLines.ResidentNameValidator.Resident_DOB), "");
        }
    }
}