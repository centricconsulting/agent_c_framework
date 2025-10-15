using IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.Common
{
    [TestClass]
    public class LossListValidatorTests : VRQQLibBase
    {
        [TestMethod]
        public void LossListValidatorTest_Auto()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal;

            q.Drivers = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteDriver>();
            q.Drivers.Add(new QuickQuote.CommonObjects.QuickQuoteDriver());

            q.Drivers[0].LossHistoryRecords = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord>();
            var l = new QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord();
            var l1 = new QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord();
            q.Drivers[0].LossHistoryRecords.Add(l);
            q.Drivers[0].LossHistoryRecords.Add(l1);

            // check for duplicates should find a duplicate
            l.LossDate = DateTime.Now.ToShortDateString();
            l.Amount = "5";
            l.TypeOfLossId = "0"; // LossTypeId Zero - N/A is valid in this case
            l.LossHistoryFaultId = "2";
            l1.LossDate = DateTime.Now.ToShortDateString();
            l1.Amount = "5";
            l1.TypeOfLossId = "0"; // LossTypeId Zero - N/A is valid in this case
            l1.LossHistoryFaultId = "2";
            var Validations = LossListValidator.ValidateLossList(0, null, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(LossListValidator.LossDateDuplicatedInList));

            // all should be valid
            l.LossDate = DateTime.Now.ToShortDateString();
            l.Amount = "5";
            l.TypeOfLossId = "0"; // LossTypeId Zero - N/A is valid in this case
            l.LossHistoryFaultId = "2";
            l1.LossDate = DateTime.Now.AddDays(1).ToShortDateString();
            l1.Amount = "5";
            l1.TypeOfLossId = "0"; // LossTypeId Zero - N/A is valid in this case
            l1.LossHistoryFaultId = "2";
            Validations = LossListValidator.ValidateLossList(0, null, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(LossListValidator.LossDateDuplicatedInList));
        }
    }
}