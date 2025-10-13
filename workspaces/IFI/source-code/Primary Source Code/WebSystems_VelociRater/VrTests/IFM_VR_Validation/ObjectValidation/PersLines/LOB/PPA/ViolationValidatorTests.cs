using IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.PPA
{
    [TestClass]
    public class ViolationValidatorTests : VRQQLibBase
    {
        [TestMethod]
        public void ViolationValidator_Tests()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal;
            q.Drivers = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteDriver>();
            QuickQuote.CommonObjects.QuickQuoteDriver d = new QuickQuote.CommonObjects.QuickQuoteDriver();
            q.Drivers.Add(d);
            d.AccidentViolations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteAccidentViolation>();
            QuickQuote.CommonObjects.QuickQuoteAccidentViolation v = new QuickQuote.CommonObjects.QuickQuoteAccidentViolation();
            d.AccidentViolations.Add(v);

            // testing empty
            v.AvDate = "";
            v.AccidentsViolationsTypeId = "";
            var Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.ViolationValidator.ValidateViolation(0, 0, q);
            Assert.IsTrue(Validations.ListHasValidationId(ViolationValidator.ViolationDate), "ViolationDate" + PrintTestValue(v.AvDate));
            Assert.IsTrue(Validations.ListHasValidationId(ViolationValidator.ViolationType), "ViolationType" + PrintTestValue(v.AccidentsViolationsTypeId));

            // testing invalid _ Deafult
            v.AvDate = IFM.Common.InputValidation.InputHelpers.DiamondDefaultDate_string;
            v.AccidentsViolationsTypeId = "a";
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.ViolationValidator.ValidateViolation(0, 0, q);
            Assert.IsTrue(Validations.ListHasValidationId(ViolationValidator.ViolationDate), "ViolationDate" + PrintTestValue(v.AvDate));
            Assert.IsTrue(Validations.ListHasValidationId(ViolationValidator.ViolationType), "ViolationType" + PrintTestValue(v.AccidentsViolationsTypeId));

            // testing invalid
            v.AvDate = "a";
            v.AccidentsViolationsTypeId = "a";
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.ViolationValidator.ValidateViolation(0, 0, q);
            Assert.IsTrue(Validations.ListHasValidationId(ViolationValidator.ViolationDate), "ViolationDate" + PrintTestValue(v.AvDate));
            Assert.IsTrue(Validations.ListHasValidationId(ViolationValidator.ViolationType), "ViolationType" + PrintTestValue(v.AccidentsViolationsTypeId));

            // testing Valid
            v.AvDate = DateTime.Now.ToShortDateString();
            v.AccidentsViolationsTypeId = "1";
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.ViolationValidator.ValidateViolation(0, 0, q);
            Assert.IsFalse(Validations.ListHasValidationId(ViolationValidator.ViolationDate), "ViolationDate" + PrintTestValue(v.AvDate));
            Assert.IsFalse(Validations.ListHasValidationId(ViolationValidator.ViolationType), "ViolationType" + PrintTestValue(v.AccidentsViolationsTypeId));
        }
    }
}