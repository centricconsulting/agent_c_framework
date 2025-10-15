using IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.Common
{
    [TestClass]
    public class LossValidatorTests : VRQQLibBase
    {
        [TestMethod]
        public void LossValidatorTest_Auto()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal;

            q.Drivers = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteDriver>();
            q.Drivers.Add(new QuickQuote.CommonObjects.QuickQuoteDriver());

            var Validations = LossValidator.ValidateLoss(0, 0, q);
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossObjectNull), "Quote Null failed.#1");

            q.Drivers[0].LossHistoryRecords = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord>();
            var l = new QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord();
            q.Drivers[0].LossHistoryRecords.Add(l);

            // testing EMPTY
            l.LossDate = "";
            l.Amount = "";
            l.TypeOfLossId = "";
            l.LossHistoryFaultId = "";
            Validations = LossValidator.ValidateLoss(0, 0, q);
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossObjectNull), "Quote Null failed.#2");
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossDate), "LossDate" + PrintTestValue(l.LossDate));
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossAmount), "LossAmount" + PrintTestValue(l.Amount));
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossType), "LossType" + PrintTestValue(l.TypeOfLossId));
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossFault), "LossFault" + PrintTestValue(l.LossHistoryFaultId));

            // testing Defaulted Date
            l.LossDate = IFM.Common.InputValidation.InputHelpers.DiamondDefaultDate_string;
            Validations = LossValidator.ValidateLoss(0, 0, q);
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossDate), "LossDate" + PrintTestValue(l.LossDate));

            // all should be valid
            l.LossDate = DateTime.Now.ToShortDateString();
            l.Amount = "5";
            l.TypeOfLossId = "0"; // LossTypeId Zero - N/A is valid in this case
            l.LossHistoryFaultId = "2";
            Validations = LossValidator.ValidateLoss(0, 0, q);
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossObjectNull), "Quote Null failed.#2");
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossDate), "LossDate" + PrintTestValue(l.LossDate));
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossAmount), "LossAmount" + PrintTestValue(l.Amount));
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossType), "LossType" + PrintTestValue(l.TypeOfLossId));
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossFault), "LossFault" + PrintTestValue(l.LossHistoryFaultId));

            l.LossDate = DateTime.Now.AddDays(1).ToShortDateString();
            l.Amount = "-5";
            //l.TypeOfLossId = "-2";
            //l.LossHistoryFaultId = "-2";
            Validations = LossValidator.ValidateLoss(0, 0, q);
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossObjectNull), "Quote Null failed.#2");
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossDate), "LossDate" + PrintTestValue(l.LossDate));
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossAmount), "LossAmount" + PrintTestValue(l.Amount));
            //Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossType), "LossType" + PrintTestValue(l.TypeOfLossId));
            //Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossFault), "LossFault" + PrintTestValue(l.LossHistoryFaultId));

            // testing empty Loss Type
            l.LossDate = DateTime.Now.ToShortDateString();
            l.Amount = "5";
            l.TypeOfLossId = "-1"; // LossTypeId Zero - N/A is valid in this case
            l.LossHistoryFaultId = "2";
            Validations = LossValidator.ValidateLoss(0, 0, q);
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossType), "LossType" + PrintTestValue(l.TypeOfLossId));
        }

        [TestMethod]
        public void LossValidatorTest_HOME_PolicyLevel()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;

            var Validations = LossValidator.ValidateLoss(0, 0, q);
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossObjectNull), "Quote Null failed.#1");

            // test at policy Level
            q.LossHistoryRecords = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord>();
            var l = new QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord();
            q.LossHistoryRecords.Add(l);

            // testing EMPTY
            l.LossDate = "";
            l.Amount = "";
            l.TypeOfLossId = "";
            l.LossHistoryFaultId = ""; // not used on HOM
            Validations = LossValidator.ValidateLoss(0, 0, q);
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossObjectNull), "Quote Null failed.#2");
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossDate), "LossDate" + PrintTestValue(l.LossDate));
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossAmount), "LossAmount" + PrintTestValue(l.Amount));
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossType), "LossType" + PrintTestValue(l.TypeOfLossId));
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossFault), "LossFault" + PrintTestValue(l.LossHistoryFaultId));

            // testing Default Date
            l.LossDate = "";
            Validations = LossValidator.ValidateLoss(0, 0, q);
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossDate), "LossDate" + PrintTestValue(l.LossDate));

            // testing all Valid
            l.LossDate = DateTime.Now.ToShortDateString();
            l.Amount = "5";
            l.TypeOfLossId = "2";
            Validations = LossValidator.ValidateLoss(0, 0, q);
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossObjectNull), "Quote Null failed.#2");
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossDate), "LossDate" + PrintTestValue(l.LossDate));
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossAmount), "LossAmount" + PrintTestValue(l.Amount));
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossType), "LossType" + PrintTestValue(l.TypeOfLossId));

            l.LossDate = DateTime.Now.AddDays(1).ToShortDateString();
            l.Amount = "-5";
            //l.TypeOfLossId = "-2";
            //l.LossHistoryFaultId = "-2";
            Validations = LossValidator.ValidateLoss(0, 0, q);
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossObjectNull), "Quote Null failed.#2");
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossDate), "LossDate" + PrintTestValue(l.LossDate));
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossAmount), "LossAmount" + PrintTestValue(l.Amount));
            //Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossType), "LossType" + PrintTestValue(l.TypeOfLossId));
            //Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossFault), "LossFault" + PrintTestValue(l.LossHistoryFaultId));
        }

        [TestMethod]
        public void LossValidatorTest_HOME_ApplicantLevel()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;

            var Validations = LossValidator.ValidateLoss(0, 0, q);
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossObjectNull), "Quote Null failed.#1");

            // test at Applicant Level
            q.Applicants = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteApplicant>();
            var app = new QuickQuote.CommonObjects.QuickQuoteApplicant();
            q.Applicants.Add(app);
            app.LossHistoryRecords = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord>();

            var l = new QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord();
            app.LossHistoryRecords.Add(l);

            // testing EMPTY
            l.LossDate = "";
            l.Amount = "";
            l.TypeOfLossId = "";
            l.LossHistoryFaultId = ""; // not used on HOM
            Validations = LossValidator.ValidateLoss(0, 0, q);
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossObjectNull), "Quote Null failed.#2");
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossDate), "LossDate" + PrintTestValue(l.LossDate));
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossAmount), "LossAmount" + PrintTestValue(l.Amount));
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossType), "LossType" + PrintTestValue(l.TypeOfLossId));
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossFault), "LossFault" + PrintTestValue(l.LossHistoryFaultId));

            // testing all valid
            l.LossDate = DateTime.Now.ToShortDateString();
            l.Amount = "5";
            l.TypeOfLossId = "2";
            Validations = LossValidator.ValidateLoss(0, 0, q);
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossObjectNull), "Quote Null failed.#2");
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossDate), "LossDate" + PrintTestValue(l.LossDate));
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossAmount), "LossAmount" + PrintTestValue(l.Amount));
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossType), "LossType" + PrintTestValue(l.TypeOfLossId));

            l.LossDate = DateTime.Now.AddDays(1).ToShortDateString();
            l.Amount = "-5";
            //l.TypeOfLossId = "-2";
            //l.LossHistoryFaultId = "-2";
            Validations = LossValidator.ValidateLoss(0, 0, q);
            Assert.IsFalse(Validations.ListHasValidationId(LossValidator.LossObjectNull), "Quote Null failed.#2");
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossDate), "LossDate" + PrintTestValue(l.LossDate));
            Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossAmount), "LossAmount" + PrintTestValue(l.Amount));
            //Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossType), "LossType" + PrintTestValue(l.TypeOfLossId));
            //Assert.IsTrue(Validations.ListHasValidationId(LossValidator.LossFault), "LossFault" + PrintTestValue(l.LossHistoryFaultId));
        }
    }
}