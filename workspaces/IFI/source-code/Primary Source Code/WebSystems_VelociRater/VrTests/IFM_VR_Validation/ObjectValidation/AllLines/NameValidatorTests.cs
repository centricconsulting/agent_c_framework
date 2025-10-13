using IFM.VR.Validation.ObjectValidation.AllLines;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VrTests.IFM_VR_Validation.ObjectValidation.AllLines
{
    [TestClass]
    public class NameValidatorTests : VRQQLibBase
    {
        [TestMethod]
        public void NameTests()
        {
            var name = new QuickQuote.CommonObjects.QuickQuoteName();

            //testing empty
            var valItems = NameValidator.ValidateNameObject(name, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(valItems.ListHasValidationId(NameValidator.CommAndPersNameComponentsEmpty), "Name is empty");

            // test invalid
            name.TypeId = "1";
            name.FirstName = "";
            name.LastName = "hjhj";
            name.TaxNumber = "abc";
            name.SexId = "";
            name.BirthDate = "asb";
            valItems = NameValidator.ValidateNameObject(name, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(valItems.ListHasValidationId(NameValidator.CommAndPersNameComponentsEmpty), "Name is empty");
            Assert.IsTrue(valItems.ListHasValidationId(NameValidator.FirstNameID), PrintTestValue(name.FirstName));
            Assert.IsTrue(valItems.ListHasValidationId(NameValidator.GenderID), PrintTestValue(name.SexId));
            Assert.IsTrue(valItems.ListHasValidationId(NameValidator.SSNID), PrintTestValue(name.TaxNumber));
            Assert.IsTrue(valItems.ListHasValidationId(NameValidator.BirthDate), PrintTestValue(name.BirthDate));

            //testing Valid
            name.TypeId = "1";
            name.FirstName = "matt";
            name.LastName = "amo";
            name.TaxNumber = "123-23-1234";
            name.SexId = "2";
            name.BirthDate = "12/12/1970";
            valItems = NameValidator.ValidateNameObject(name, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(valItems.ListHasValidationId(NameValidator.FirstNameID), PrintTestValue(name.FirstName));
            Assert.IsFalse(valItems.ListHasValidationId(NameValidator.GenderID), PrintTestValue(name.SexId));
            Assert.IsFalse(valItems.ListHasValidationId(NameValidator.SSNID), PrintTestValue(name.TaxNumber));
            Assert.IsFalse(valItems.ListHasValidationId(NameValidator.BirthDate), PrintTestValue(name.BirthDate));

            // test invalid
            name.CommercialName1 = "ghjghj";
            name.FirstName = "";
            name.LastName = "hjhj";

            valItems = NameValidator.ValidateNameObject(name, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(valItems.ListHasValidationId(NameValidator.CommAndPersNameComponentsAllSet), "Personal and commercial names set");

        }

        [TestMethod]
        public void NameTests_BusinessStarted()

        {
            var name = new QuickQuote.CommonObjects.QuickQuoteName();

            //testing empty
            var valItems = NameValidator.ValidateNameObject(name, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(valItems.ListHasValidationId(NameValidator.CommAndPersNameComponentsEmpty), "Name is empty");


            // text business Name Valid
            name.TypeId = "2";
            name.CommercialName1 = "Comm Name";
            name.FirstName = "";
            name.MiddleName = "";
            name.LastName = "";
            valItems = NameValidator.ValidateNameObject(name, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(valItems.ListHasValidationId(NameValidator.CommercialName), "Missing Comm Name");

            // text business Name InValid
            name.TypeId = "2";
            name.CommercialName1 = "";
            valItems = NameValidator.ValidateNameObject(name, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(valItems.ListHasValidationId(NameValidator.CommercialName), "Missing Comm Name");

            // text business started valid because on quote side
            name.TypeId = "2";
            name.CommercialName1 = "Comm Name";
            name.DateBusinessStarted = "";
            valItems = NameValidator.ValidateNameObject(name, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(valItems.ListHasValidationId(NameValidator.CommercialName), "Missing Comm Name");
            Assert.IsFalse(valItems.ListHasValidationId(NameValidator.BusinessStartedDate), "Missing Business Started");

            // text business started Invalid because on app side
            name.TypeId = "2";
            name.CommercialName1 = "Comm Name";
            name.DateBusinessStarted = "";
            valItems = NameValidator.ValidateNameObject(name, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsFalse(valItems.ListHasValidationId(NameValidator.CommercialName), "Missing Comm Name");
            Assert.IsTrue(valItems.ListHasValidationId(NameValidator.BusinessStartedDate), "Missing Business Started");

            // text business started Invalid because has 'Business Started Date' less than 3 years ago but has no 'Years of Experience'
            name.TypeId = "2";
            name.CommercialName1 = "Comm Name";
            name.DateBusinessStarted = System.DateTime.Now.AddDays(-10).ToShortDateString();
            name.YearsOfExperience = "";
            valItems = NameValidator.ValidateNameObject(name, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsFalse(valItems.ListHasValidationId(NameValidator.CommercialName), "Missing Comm Name");
            Assert.IsFalse(valItems.ListHasValidationId(NameValidator.BusinessStartedDate), "Missing Business Started");
            Assert.IsTrue(valItems.ListHasValidationId(NameValidator.YearsOfExperience), "Missing Years of Experience");

            // text business started valid because has 'Business Started Date' less than 3 years ago but has  'Years of Experience'
            name.TypeId = "2";
            name.CommercialName1 = "Comm Name";
            name.DateBusinessStarted = System.DateTime.Now.AddDays(-10).ToShortDateString();
            name.YearsOfExperience = "2";
            valItems = NameValidator.ValidateNameObject(name, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsFalse(valItems.ListHasValidationId(NameValidator.CommercialName), "Missing Comm Name");
            Assert.IsFalse(valItems.ListHasValidationId(NameValidator.BusinessStartedDate), "Missing Business Started");
            Assert.IsFalse(valItems.ListHasValidationId(NameValidator.YearsOfExperience), "Missing Years of Experience");

        }


    }
    }

  