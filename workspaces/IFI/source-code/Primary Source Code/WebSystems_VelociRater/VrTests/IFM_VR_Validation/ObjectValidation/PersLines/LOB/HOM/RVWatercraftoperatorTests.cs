using IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.HOM
{
    [TestClass]
    public class RVWatercraftoperatorTests : VRQQLibBase
    {
        [TestMethod]
        public void Test_HOM_Watercraft_Operators()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;

            q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
            var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
            q.Locations.Add(l);

            l.RvWatercrafts = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteRvWatercraft>();
            var rv = new QuickQuote.CommonObjects.QuickQuoteRvWatercraft();
            l.RvWatercrafts.Add(rv);

            rv.Operators = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteOperator>();
            var op = new QuickQuote.CommonObjects.QuickQuoteOperator();
            rv.Operators.Add(op);

            // Testing empty
            op.Name.TypeId = "1";
            op.Name.FirstName = "";
            op.Name.LastName = "";
            op.Name.BirthDate = "";
            var vals = RVWatercraftOperatorsValidator.ValidateRvWaterCraftOperator(op, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsTrue(vals.ListHasValidationId(RVWatercraftOperatorsValidator.FirstName), PrintTestValue(op.Name.FirstName));
            Assert.IsTrue(vals.ListHasValidationId(RVWatercraftOperatorsValidator.LastName), PrintTestValue(op.Name.LastName));
            Assert.IsTrue(vals.ListHasValidationId(RVWatercraftOperatorsValidator.BirthDate), PrintTestValue(op.Name.BirthDate));

            // Testing Invalid
            op.Name.FirstName = "youthful";
            op.Name.LastName = "operator";
            op.Name.BirthDate = "";
            vals = RVWatercraftOperatorsValidator.ValidateRvWaterCraftOperator(op, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsTrue(vals.ListHasValidationId(RVWatercraftOperatorsValidator.FirstName), PrintTestValue(op.Name.FirstName));
            Assert.IsTrue(vals.ListHasValidationId(RVWatercraftOperatorsValidator.LastName), PrintTestValue(op.Name.LastName));
            Assert.IsTrue(vals.ListHasValidationId(RVWatercraftOperatorsValidator.BirthDate), PrintTestValue(op.Name.BirthDate));

            // Testing Invalid Date
            op.Name.FirstName = "matt";
            op.Name.LastName = "amo";
            op.Name.BirthDate = DateTime.Now.AddYears(-26).ToShortDateString();
            vals = RVWatercraftOperatorsValidator.ValidateRvWaterCraftOperator(op, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsFalse(vals.ListHasValidationId(RVWatercraftOperatorsValidator.FirstName), PrintTestValue(op.Name.FirstName));
            Assert.IsFalse(vals.ListHasValidationId(RVWatercraftOperatorsValidator.LastName), PrintTestValue(op.Name.LastName));
            Assert.IsTrue(vals.ListHasValidationId(RVWatercraftOperatorsValidator.BirthDate), PrintTestValue(op.Name.BirthDate));

            // Testing Valid
            op.Name.FirstName = "matt";
            op.Name.LastName = "amo";
            op.Name.BirthDate = DateTime.Now.AddYears(-24).ToShortDateString();
            vals = RVWatercraftOperatorsValidator.ValidateRvWaterCraftOperator(op, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsFalse(vals.ListHasValidationId(RVWatercraftOperatorsValidator.FirstName), PrintTestValue(op.Name.FirstName));
            Assert.IsFalse(vals.ListHasValidationId(RVWatercraftOperatorsValidator.LastName), PrintTestValue(op.Name.LastName));
            Assert.IsFalse(vals.ListHasValidationId(RVWatercraftOperatorsValidator.BirthDate), PrintTestValue(op.Name.BirthDate));
        }
    }
}