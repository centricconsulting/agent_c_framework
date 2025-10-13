using IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.PPA
{
    [TestClass]
    public class PolicydriverLossViolationValidatorTests : VRQQLibBase
    {
        private System.Random rnd = new System.Random();

        private string GetRandomMinorViolation()
        {
            return IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.PolicyDriverLossesAndViolationValidator.minorViolations[rnd.Next(0, IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.PolicyDriverLossesAndViolationValidator.minorViolations.Length - 1)]; // get random minor
        }

        private string GetRandomMajorViolation()
        {
            return IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.PolicyDriverLossesAndViolationValidator.majorViolations[rnd.Next(0, IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.PolicyDriverLossesAndViolationValidator.majorViolations.Length - 1)]; // get random major
        }

        private string GetRandomUnacceptableViolation()
        {
            return IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.PolicyDriverLossesAndViolationValidator.unacceptableViolations[rnd.Next(0, IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.PolicyDriverLossesAndViolationValidator.unacceptableViolations.Length - 1)]; // get random unacceptable
        }

        [TestMethod]
        public void PolicydriverLossViolationValidatorTest_PolicyLossesIn3Years()
        {
            var q = GetNewQuickQuote();
            q.EffectiveDate = DateTime.Now.ToShortDateString();

            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal;
            q.Drivers = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteDriver>();
            var d1 = new QuickQuote.CommonObjects.QuickQuoteDriver();
            var d2 = new QuickQuote.CommonObjects.QuickQuoteDriver();
            q.Drivers.Add(d1);
            q.Drivers.Add(d2);

            d1.AccidentViolations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteAccidentViolation>();
            d1.LossHistoryRecords = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord>();
            d2.AccidentViolations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteAccidentViolation>();
            d2.LossHistoryRecords = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord>();

            d1.DriverExcludeTypeId = "1"; // 1 = rated driver
            d2.DriverExcludeTypeId = "1"; // 1 = rated driver

            var l1 = new QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord();
            l1.LossHistoryFaultId = "1"; // '1'=at fault
            l1.LossDate = DateTime.Now.ToShortDateString();

            // make sure that you get an error if there is more than 1 'at fault' loss in the last 3 years

            // This should pass with one or two losses
            d1.LossHistoryRecords.Add(l1);
            var valItems = PolicyDriverLossesAndViolationValidator.ValidatePolicyDriversLossesAndViolations(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(valItems.ListHasValidationId(PolicyDriverLossesAndViolationValidator.PolicyAtFaultLossesInPast3YearsExceeeded));

            // should fail with more than 2 - now this should fail
            d2.LossHistoryRecords.Add(l1);
            d2.LossHistoryRecords.Add(l1);
            valItems = PolicyDriverLossesAndViolationValidator.ValidatePolicyDriversLossesAndViolations(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(valItems.ListHasValidationId(PolicyDriverLossesAndViolationValidator.PolicyAtFaultLossesInPast3YearsExceeeded));
        }

        [TestMethod]
        public void _NOTDONE_PolicydriverLossViolationValidatorTest()
        {
            var q = GetNewQuickQuote();
            q.EffectiveDate = DateTime.Now.ToShortDateString();

            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal;
            q.Drivers = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteDriver>();
            var d1 = new QuickQuote.CommonObjects.QuickQuoteDriver();
            var d2 = new QuickQuote.CommonObjects.QuickQuoteDriver();
            q.Drivers.Add(d1);
            q.Drivers.Add(d2);

            d1.AccidentViolations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteAccidentViolation>();
            d1.LossHistoryRecords = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord>();
            d2.AccidentViolations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteAccidentViolation>();
            d2.LossHistoryRecords = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord>();

            d1.DriverExcludeTypeId = "1"; // 1 = rated driver
            d2.DriverExcludeTypeId = "1"; // 1 = rated driver

            d1.Name.BirthDate = DateTime.Now.AddYears(-19).ToString();
            d2.Name.BirthDate = DateTime.Now.AddYears(-19).ToString();

            var a1 = new QuickQuote.CommonObjects.QuickQuoteAccidentViolation();
            a1.AccidentsViolationsTypeId = GetRandomMinorViolation();
            a1.AvDate = DateTime.Now.ToShortDateString();

            var l1 = new QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord();
            l1.LossHistoryFaultId = "1"; // '1'=at fault
            l1.LossDate = DateTime.Now.ToShortDateString();

            var valItems = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.PolicyDriverLossesAndViolationValidator.ValidatePolicyDriversLossesAndViolations(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
        }
    }
}