using IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.Common
{
    [TestClass]
    public class AdditionalInterestListTests : VRQQLibBase
    {
        [TestMethod]
        public void AdditionalInterestList_HOM_Tests()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;

            // testing Null
            var vals = AdditionalInterestListValidator.ValidateAdditionalInterestList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(vals.ListHasValidationId(AdditionalInterestListValidator.AiListIsNull), "Is Null");

            q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
            q.Locations.Add(new QuickQuote.CommonObjects.QuickQuoteLocation());
            var l = q.Locations[0];
            l.AdditionalInterests = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteAdditionalInterest>();

            var ai1 = new QuickQuote.CommonObjects.QuickQuoteAdditionalInterest();
            var ai2 = new QuickQuote.CommonObjects.QuickQuoteAdditionalInterest();
            var ai3 = new QuickQuote.CommonObjects.QuickQuoteAdditionalInterest();
            l.AdditionalInterests.Add(ai1);
            l.AdditionalInterests.Add(ai2);
            l.AdditionalInterests.Add(ai3);

            // test Third Mortgagee issuance
            ai1.TypeId = "42"; // first mortgagee
            ai2.TypeId = "11"; // 2nd mortgagee
            ai3.TypeId = "15"; // 3rd mortgagee
            vals = AdditionalInterestListValidator.ValidateAdditionalInterestList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.issuance);
            Assert.IsFalse(vals.ListHasValidationId(AdditionalInterestListValidator.AiListIsNull), "Is Null");
            Assert.IsTrue(vals.ListHasValidationId(AdditionalInterestListValidator.HasThirdMortgagee), "Should fail on Third Mortgagee");

            // test Third Mortgagee   -   quote
            ai1.TypeId = "42"; // first mortgagee
            ai2.TypeId = "11"; // 2nd mortgagee
            ai3.TypeId = "15"; // 3rd mortgagee
            vals = AdditionalInterestListValidator.ValidateAdditionalInterestList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(vals.ListHasValidationId(AdditionalInterestListValidator.HasThirdMortgagee), "Should NOT fail on Third Mortgagee");

            // testing Duplicate Types
            ai1.TypeId = "42"; // first mortgagee
            ai2.TypeId = "42"; // first mortgagee - AGAIN
            ai3.TypeId = "15"; // 3rd mortgagee
            vals = AdditionalInterestListValidator.ValidateAdditionalInterestList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(vals.ListHasValidationId(AdditionalInterestListValidator.MortgageeTypeUsedMultipleTimes), "Same Type Multiple times");
        }

        [TestMethod]
        public void AdditionalInterestList_Loan_Lease()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal;

            q.Vehicles = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteVehicle>();
            q.Vehicles.Add(new QuickQuote.CommonObjects.QuickQuoteVehicle());
            var v = q.Vehicles[0];

            // when Loan/Lease is set on a vehicle you must have an AI
            v.HasAutoLoanOrLease = false;
            var vals = AdditionalInterestListValidator.ValidateAdditionalInterestList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, 0);
            Assert.IsFalse(vals.ListHasValidationId(AdditionalInterestListValidator.VehicleRequiredAiButNone), "");

            // has no AI so should fail
            v.HasAutoLoanOrLease = true;
            vals = AdditionalInterestListValidator.ValidateAdditionalInterestList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, 0);
            Assert.IsTrue(vals.ListHasValidationId(AdditionalInterestListValidator.VehicleRequiredAiButNone), "");

            // has a AI so should not fail
            v.HasAutoLoanOrLease = true;
            v.AdditionalInterests = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteAdditionalInterest>();
            v.AdditionalInterests.Add(new QuickQuote.CommonObjects.QuickQuoteAdditionalInterest());
            vals = AdditionalInterestListValidator.ValidateAdditionalInterestList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, 0);
            Assert.IsFalse(vals.ListHasValidationId(AdditionalInterestListValidator.VehicleRequiredAiButNone), "");

            // has a AI so should not fail
            v.HasAutoLoanOrLease = false;
            v.AdditionalInterests = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteAdditionalInterest>();
            v.AdditionalInterests.Add(new QuickQuote.CommonObjects.QuickQuoteAdditionalInterest());
            vals = AdditionalInterestListValidator.ValidateAdditionalInterestList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, 0);
            Assert.IsFalse(vals.ListHasValidationId(AdditionalInterestListValidator.VehicleRequiredAiButNone), "");
        }

        [TestMethod]
        public void AdditionalInterestList_AI_Requires_CompCollision()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal;

            q.Vehicles = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteVehicle>();
            q.Vehicles.Add(new QuickQuote.CommonObjects.QuickQuoteVehicle());
            var v = q.Vehicles[0];
            v.AdditionalInterests = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteAdditionalInterest>();
            v.AdditionalInterests.Add(new QuickQuote.CommonObjects.QuickQuoteAdditionalInterest());

            string[] AiTypesids_that_require_comp_collision = new string[] { "53", "54", "8", "32", "14" };

            List<IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType> valTypes = new List<IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType>();
            valTypes.Add(IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            valTypes.Add(IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.issuance);

            // test quote validation then test issuance validation - should only test during issuance validations
            foreach (var valtype in valTypes)
            {
                foreach (var id in AiTypesids_that_require_comp_collision)
                {
                    // should be fine because it has comp and collision
                    v.AdditionalInterests[0].TypeId = id;
                    v.ComprehensiveDeductibleLimitId = "4";
                    v.CollisionDeductibleLimitId = "4";
                    var vals = AdditionalInterestListValidator.ValidateAdditionalInterestList(q, valtype, 0);
                    Assert.IsFalse(vals.ListHasValidationId(AdditionalInterestListValidator.CompCollRequiredWithAi), "");
                }

                foreach (var id in AiTypesids_that_require_comp_collision)
                {
                    // should NOT be fine because it doesn't have comp and collision - if valtype is Issuance
                    v.AdditionalInterests[0].TypeId = id;
                    v.ComprehensiveDeductibleLimitId = "";
                    v.CollisionDeductibleLimitId = "";
                    var vals = AdditionalInterestListValidator.ValidateAdditionalInterestList(q, valtype, 0);
                    if (valtype == IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.issuance)
                    {
                        Assert.IsTrue(vals.ListHasValidationId(AdditionalInterestListValidator.CompCollRequiredWithAi), "");
                    }
                    else
                    {
                        Assert.IsFalse(vals.ListHasValidationId(AdditionalInterestListValidator.CompCollRequiredWithAi), "");
                    }
                }
            }
        }
    }
}