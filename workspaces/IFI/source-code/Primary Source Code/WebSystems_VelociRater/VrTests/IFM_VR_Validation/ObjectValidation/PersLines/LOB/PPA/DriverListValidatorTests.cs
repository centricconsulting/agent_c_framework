using IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.PPA
{
    [TestClass]
    public class DriverListValidatorTests : VRQQLibBase
    {
        [TestMethod]
        public void DriverListValidator_Tests()
        {
            // test null quote
            var Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverListValidator.ValidateDriverList(null, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(DriverListValidator.QuoteIsNull), "Quote Null failed.#1");

            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal;
            q.Drivers = null;

            // test null drivers list
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverListValidator.ValidateDriverList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverListValidator.QuoteIsNull), "Quote Null failed.#1");
            Assert.IsTrue(Validations.ListHasValidationId(DriverListValidator.DriverListNoDrivers), "No Drivers");

            // test no rated drivers
            q.Drivers = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteDriver>();
            QuickQuote.CommonObjects.QuickQuoteDriver driver = new QuickQuote.CommonObjects.QuickQuoteDriver();
            q.Drivers.Add(driver);
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverListValidator.ValidateDriverList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(DriverListValidator.DriverListNoRatedDrivers), "No rated Drivers");

            // test has rated drivers
            driver.DriverExcludeTypeId = "1"; // Rated Driver = 1
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverListValidator.ValidateDriverList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverListValidator.DriverListNoRatedDrivers), "No rated Drivers");
        }
    }
}