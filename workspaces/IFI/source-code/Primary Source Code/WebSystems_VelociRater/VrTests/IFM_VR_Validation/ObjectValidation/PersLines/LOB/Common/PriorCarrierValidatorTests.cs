using IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.Common
{
    [TestClass]
    public class PriorCarrierValidatorTests : VRQQLibBase
    {
        [TestMethod]
        public void ValidatePriorCarrier_Tests()
        {
            var Validations = PriorCarrierValidator.ValidatePriorCarrier(null, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(PriorCarrierValidator.QuoteNull), "Quote Null failed.#1");

            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal;
            q.PriorCarrier = null;
            Validations = PriorCarrierValidator.ValidatePriorCarrier(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.QuoteNull), "Quote Null failed.#2");
            Assert.IsTrue(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierNull), "Prior Carrier Null failed.#1");

            q.PriorCarrier = new QuickQuote.CommonObjects.QuickQuotePriorCarrier();
            q.PriorCarrier.PreviousInsurerTypeId = "0"; //0 = 'None'  is valid but can't have expiration date
            q.PriorCarrier.PriorDurationTypeId = "";
            q.PriorCarrier.PriorDurationWithCompany = "5";
            q.PriorCarrier.PriorExpirationDate = "";
            Validations = PriorCarrierValidator.ValidatePriorCarrier(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierNull), "Prior Carrier Null failed.#2");
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorcarrierPreviousInsurer), "PriorcarrierPreviousInsurer failed" + PrintTestValue(q.PriorCarrier.PreviousInsurerTypeId));
            Assert.IsTrue(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierDuration), "PriorCarrierDuration failed" + PrintTestValue(q.PriorCarrier.PriorDurationWithCompany));
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierDurationType), "PriorCarrierDurationType failed" + PrintTestValue(q.PriorCarrier.PriorDurationTypeId));
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierExpirationDate), "PriorCarrierExpirationDate failed" + PrintTestValue(q.PriorCarrier.PriorExpirationDate));

            q.PriorCarrier = new QuickQuote.CommonObjects.QuickQuotePriorCarrier();
            q.PriorCarrier.PreviousInsurerTypeId = "0"; //0 = 'None'  is valid but can't have expiration date
            q.PriorCarrier.PriorDurationTypeId = "";
            q.PriorCarrier.PriorDurationWithCompany = "7";
            q.PriorCarrier.PriorExpirationDate = DateTime.Now.AddDays(-1).ToShortDateString();
            Validations = PriorCarrierValidator.ValidatePriorCarrier(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorcarrierPreviousInsurer), "PriorcarrierPreviousInsurer failed" + PrintTestValue(q.PriorCarrier.PreviousInsurerTypeId));
            Assert.IsTrue(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierDuration), "PriorCarrierDuration failed" + PrintTestValue(q.PriorCarrier.PriorDurationWithCompany));
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierDurationType), "PriorCarrierDurationType failed" + PrintTestValue(q.PriorCarrier.PriorDurationTypeId));
            Assert.IsTrue(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierExpirationDate), "PriorCarrierExpirationDate failed" + PrintTestValue(q.PriorCarrier.PriorExpirationDate));

            q.PriorCarrier = new QuickQuote.CommonObjects.QuickQuotePriorCarrier();
            q.PriorCarrier.PreviousInsurerTypeId = "";
            q.PriorCarrier.PriorDurationTypeId = "";
            q.PriorCarrier.PriorDurationWithCompany = "";
            q.PriorCarrier.PriorExpirationDate = DateTime.Now.AddDays(-1).ToShortDateString();
            Validations = PriorCarrierValidator.ValidatePriorCarrier(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(PriorCarrierValidator.PriorcarrierPreviousInsurer), "PriorcarrierPreviousInsurer failed" + PrintTestValue(q.PriorCarrier.PreviousInsurerTypeId));
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierDuration), "PriorCarrierDuration failed" + PrintTestValue(q.PriorCarrier.PriorDurationWithCompany));
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierDurationType), "PriorCarrierDurationType failed" + PrintTestValue(q.PriorCarrier.PriorDurationTypeId));
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierExpirationDate), "PriorCarrierExpirationDate failed" + PrintTestValue(q.PriorCarrier.PriorExpirationDate));

            q.PriorCarrier = new QuickQuote.CommonObjects.QuickQuotePriorCarrier();
            q.PriorCarrier.PreviousInsurerTypeId = "0"; //0 = 'None'  is valid but can't have expiration date
            q.PriorCarrier.PriorDurationTypeId = "";
            q.PriorCarrier.PriorDurationWithCompany = "";
            q.PriorCarrier.PriorExpirationDate = "";//
            Validations = PriorCarrierValidator.ValidatePriorCarrier(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorcarrierPreviousInsurer), "PriorcarrierPreviousInsurer failed" + PrintTestValue(q.PriorCarrier.PreviousInsurerTypeId));
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierDuration), "PriorCarrierDuration failed" + PrintTestValue(q.PriorCarrier.PriorDurationWithCompany));
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierDurationType), "PriorCarrierDurationType failed" + PrintTestValue(q.PriorCarrier.PriorDurationTypeId));
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierExpirationDate), "PriorCarrierExpirationDate failed" + PrintTestValue(q.PriorCarrier.PriorExpirationDate));

            q.PriorCarrier = new QuickQuote.CommonObjects.QuickQuotePriorCarrier();
            q.PriorCarrier.PreviousInsurerTypeId = "2";
            q.PriorCarrier.PriorDurationTypeId = "3";
            q.PriorCarrier.PriorDurationWithCompany = "1";
            q.PriorCarrier.PriorExpirationDate = DateTime.Now.AddDays(-1).ToShortDateString();
            Validations = PriorCarrierValidator.ValidatePriorCarrier(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorcarrierPreviousInsurer), "PriorcarrierPreviousInsurer failed" + PrintTestValue(q.PriorCarrier.PreviousInsurerTypeId));
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierDuration), "PriorCarrierDuration failed" + PrintTestValue(q.PriorCarrier.PriorDurationWithCompany));
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierDurationType), "PriorCarrierDurationType failed" + PrintTestValue(q.PriorCarrier.PriorDurationTypeId));
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierExpirationDate), "PriorCarrierExpirationDate failed" + PrintTestValue(q.PriorCarrier.PriorExpirationDate));

            // testing defaulted date
            q.PriorCarrier = new QuickQuote.CommonObjects.QuickQuotePriorCarrier();
            q.PriorCarrier.PreviousInsurerTypeId = "2";
            q.PriorCarrier.PriorDurationTypeId = "3";
            q.PriorCarrier.PriorDurationWithCompany = "1";
            q.PriorCarrier.PriorExpirationDate = IFM.Common.InputValidation.InputHelpers.DiamondDefaultDate_string;
            Validations = PriorCarrierValidator.ValidatePriorCarrier(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorcarrierPreviousInsurer), "PriorcarrierPreviousInsurer failed" + PrintTestValue(q.PriorCarrier.PreviousInsurerTypeId));
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierDuration), "PriorCarrierDuration failed" + PrintTestValue(q.PriorCarrier.PriorDurationWithCompany));
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierDurationType), "PriorCarrierDurationType failed" + PrintTestValue(q.PriorCarrier.PriorDurationTypeId));
            Assert.IsTrue(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierExpirationDate), "PriorCarrierExpirationDate failed" + PrintTestValue(q.PriorCarrier.PriorExpirationDate));

            q.PriorCarrier = new QuickQuote.CommonObjects.QuickQuotePriorCarrier();
            q.PriorCarrier.PreviousInsurerTypeId = "a";
            q.PriorCarrier.PriorDurationTypeId = "b";
            q.PriorCarrier.PriorDurationWithCompany = "c";
            q.PriorCarrier.PriorExpirationDate = DateTime.Now.AddDays(30).ToShortDateString();
            Validations = PriorCarrierValidator.ValidatePriorCarrier(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(PriorCarrierValidator.PriorcarrierPreviousInsurer), "PriorcarrierPreviousInsurer failed" + PrintTestValue(q.PriorCarrier.PreviousInsurerTypeId));
            Assert.IsTrue(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierDuration), "PriorCarrierDuration failed" + PrintTestValue(q.PriorCarrier.PriorDurationWithCompany));
            Assert.IsTrue(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierDurationType), "PriorCarrierDurationType failed" + PrintTestValue(q.PriorCarrier.PriorDurationTypeId));
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierExpirationDate), "PriorCarrierExpirationDate failed" + PrintTestValue(q.PriorCarrier.PriorExpirationDate));

            q.PriorCarrier = new QuickQuote.CommonObjects.QuickQuotePriorCarrier();
            q.PriorCarrier.PreviousInsurerTypeId = "a";
            q.PriorCarrier.PriorDurationTypeId = "b";
            q.PriorCarrier.PriorDurationWithCompany = "c";
            q.PriorCarrier.PriorExpirationDate = "abc";
            Validations = PriorCarrierValidator.ValidatePriorCarrier(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(PriorCarrierValidator.PriorcarrierPreviousInsurer), "PriorcarrierPreviousInsurer failed" + PrintTestValue(q.PriorCarrier.PreviousInsurerTypeId));
            Assert.IsTrue(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierDuration), "PriorCarrierDuration failed" + PrintTestValue(q.PriorCarrier.PriorDurationWithCompany));
            Assert.IsTrue(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierDurationType), "PriorCarrierDurationType failed" + PrintTestValue(q.PriorCarrier.PriorDurationTypeId));
            Assert.IsTrue(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierExpirationDate), "PriorCarrierExpirationDate failed" + PrintTestValue(q.PriorCarrier.PriorExpirationDate));

            // testing to make sure there are no errors even when IFM is the prior carrier
            q.PriorCarrier = new QuickQuote.CommonObjects.QuickQuotePriorCarrier();
            q.PriorCarrier.PreviousInsurerTypeId = "73"; // IFM
            q.PriorCarrier.PriorDurationTypeId = "3";
            q.PriorCarrier.PriorDurationWithCompany = "1";
            q.PriorCarrier.PriorExpirationDate = DateTime.Now.AddDays(-1).ToShortDateString();
            Validations = PriorCarrierValidator.ValidatePriorCarrier(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorcarrierPreviousInsurer), "PriorcarrierPreviousInsurer failed" + PrintTestValue(q.PriorCarrier.PreviousInsurerTypeId));
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierDuration), "PriorCarrierDuration failed" + PrintTestValue(q.PriorCarrier.PriorDurationWithCompany));
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierDurationType), "PriorCarrierDurationType failed" + PrintTestValue(q.PriorCarrier.PriorDurationTypeId));
            Assert.IsFalse(Validations.ListHasValidationId(PriorCarrierValidator.PriorCarrierExpirationDate), "PriorCarrierExpirationDate failed" + PrintTestValue(q.PriorCarrier.PriorExpirationDate));
        }
    }
}