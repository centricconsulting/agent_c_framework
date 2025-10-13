using IFM.Common.InputValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.HOM
{
    [TestClass]
    public class Coverages_HOM_ValidatorTests : VRQQLibBase
    {
        [TestMethod]
        public void Coverage_HOM_Issuance_Tests()
        {
            string[] forms = new string[] { "1,", "2", "3", "4", "5", "6", "7" };

            var valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.issuance;
            foreach (var formType in forms)
            {
                var q = GetNewQuickQuote();
                q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
                q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
                var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
                q.Locations.Add(l);

                l.FormTypeId = formType;

                // test valid for Cov A 250,000 and occupancyID 4 or 5
                l.OccupancyCodeId = "4";
                l.A_Dwelling_LimitIncreased = "250000";
                var vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.CovA_Occupancy));

                l.OccupancyCodeId = "5";
                vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.CovA_Occupancy));

                l.OccupancyCodeId = "3";
                vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.CovA_Occupancy));

                // test valid for Cov A 250,000 and occupancyID 4 or 5 - Invalid
                l.OccupancyCodeId = "4";
                l.A_Dwelling_LimitIncreased = "250001";
                vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.CovA_Occupancy));

                l.OccupancyCodeId = "5";
                vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.CovA_Occupancy));

                l.OccupancyCodeId = "3"; // should get no error because it is not the right occupancyid to trigger the error
                vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.CovA_Occupancy));

                // test when formtypeid = 4 and cov C is greater than 100,000
                l.C_PersonalProperty_LimitIncreased = "";
                if (formType == "4")
                {
                    // valid
                    l.C_PersonalProperty_LimitIncreased = "100000";
                    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                    Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.CovC_HO4_ExceedsLimit));

                    // invalid
                    l.C_PersonalProperty_LimitIncreased = "100001";
                    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                    Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.CovC_HO4_ExceedsLimit));
                }
                else
                {
                    // valid - wrong for type so no check
                    l.C_PersonalProperty_LimitIncreased = "100000";
                    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                    Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.CovC_HO4_ExceedsLimit));

                    // valid - wrong for type so no check
                    l.C_PersonalProperty_LimitIncreased = "100001";
                    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                    Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.CovC_HO4_ExceedsLimit));
                }

                // test when formid = 5 and cov A > 150,000
                if (formType == "5")
                {
                    // valid
                    l.A_Dwelling_LimitIncreased = "150000";
                    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                    Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.CovA_HO6_ExceedsLimit));

                    // invalid
                    l.A_Dwelling_LimitIncreased = "150001";
                    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                    Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.CovA_HO6_ExceedsLimit));
                }
                else
                {
                    // valid - wrong for type so no check
                    l.A_Dwelling_LimitIncreased = "150000";
                    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                    Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.CovA_HO6_ExceedsLimit));

                    // valid - wrong for type so no check
                    l.A_Dwelling_LimitIncreased = "150001";
                    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                    Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.CovA_HO6_ExceedsLimit));
                }

                /*** modified KLJ 10/16/2022 - this test should fail as the wrong validation constant is being returned by the validator
                 *** that said we are removing the vlaidation, so this test can go as well 
                 ***/
                // test - When Coverage B limit is greater than 30% of Coverage A,
                //Valid
                //l.B_OtherStructures_LimitIncreased = "30000";
                //l.A_Dwelling_LimitIncreased = "100000";
                //vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                //Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.CovB_ExceedsLimit));

                //Valid
                //l.B_OtherStructures_LimitIncreased = "30001";
                //l.A_Dwelling_LimitIncreased = "100000";
                //vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                //Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.CovB_ExceedsLimit));

                // test - When the form type is ML2 and the Coverage C limit is greater than the Coverage A limit,
                if (formType == "6")
                {
                    // valid
                    l.C_PersonalProperty_LimitIncreased = "100000";
                    l.A_Dwelling_LimitIncreased = "100000";
                    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                    Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ML2_CovC_Exceeds_CovA));

                    // invalid
                    l.C_PersonalProperty_LimitIncreased = "100001";
                    l.A_Dwelling_LimitIncreased = "100000";
                    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                    Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ML2_CovC_Exceeds_CovA));
                }
                else
                {
                    // valid - wrong for type so no check
                    l.C_PersonalProperty_LimitIncreased = "100000";
                    l.A_Dwelling_LimitIncreased = "100000";
                    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                    Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ML2_CovC_Exceeds_CovA));

                    // valid - wrong for type so no check
                    l.C_PersonalProperty_LimitIncreased = "100001";
                    l.A_Dwelling_LimitIncreased = "100000";
                    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                    Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ML2_CovC_Exceeds_CovA));
                }

                // test acreage of greater  than 5 - Valid
                l.Acreage = "5";
                vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.Location_Acerage_Exceeded));

                // test acreage of greater  than 5 - InValid
                l.Acreage = "6";
                vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.Location_Acerage_Exceeded));

                // Testing - When the total limit for all Inland Marine coverages and RV/Watercraft coverages is greater than 25% of Coverage A limit
                l.InlandMarines = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteInlandMarine>();
                var inland = new QuickQuote.CommonObjects.QuickQuoteInlandMarine();
                l.InlandMarines.Add(inland);

                l.RvWatercrafts = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteRvWatercraft>();
                var rv = new QuickQuote.CommonObjects.QuickQuoteRvWatercraft();
                l.RvWatercrafts.Add(rv);

                double inlandTotalLimit = (l.InlandMarines != null) ? l.InlandMarines.Sum(i => InputHelpers.TryToGetDouble(i.IncreasedLimit)) : 0.0;
                double rvTotalLimit = (l.RvWatercrafts != null) ? l.RvWatercrafts.Sum(i => InputHelpers.TryToGetDouble(i.CostNew)) : 0.0;

                { // just for scope control
                    // testing Valid
                    l.A_Dwelling_LimitIncluded = "0";
                    l.A_Dwelling_LimitIncreased = "100000";
                    inland.IncreasedLimit = "12500";
                    rv.CostNew = "12500";
                    List<string> formsThatApply = new string[] { "1", "2", "3", "6" }.ToList<string>();
                    if (formsThatApply.Contains(formType))
                    {
                        // should trigger check
                        vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                        Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.InlandRvLimits_Exceeded_CovA));
                    }
                    else
                    {
                        // should not even try validation since it is not the right formtypeid
                        vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                        Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.InlandRvLimits_Exceeded_CovA));
                    }

                    // testing INValid
                    l.A_Dwelling_LimitIncluded = "0";
                    l.A_Dwelling_LimitIncreased = "100000";
                    inland.IncreasedLimit = "12500";
                    rv.CostNew = "12501";
                    if (formsThatApply.Contains(formType))
                    {
                        // should trigger check and should fail
                        vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                        Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.InlandRvLimits_Exceeded_CovA));
                    }
                    else
                    {
                        // should not even try validation since it is not the right formtypeid
                        vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                        Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.InlandRvLimits_Exceeded_CovA));
                    }
                }

                // Testing - When the total limit for all Inland Marine coverages and RV/Watercraft coverages is greater than 25% of Coverage C limit
                { // just for scope control
                    // testing Valid
                    l.C_PersonalProperty_LimitIncluded = "0";
                    l.C_PersonalProperty_LimitIncreased = "100000";
                    inland.IncreasedLimit = "12500";
                    rv.CostNew = "12500";
                    List<string> formsThatApply = new string[] { "4", "5", "7" }.ToList<string>();
                    if (formsThatApply.Contains(formType))
                    {
                        // should trigger check
                        vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                        Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.InlandRvLimits_Exceeded_CovC));
                    }
                    else
                    {
                        // should not even try validation since it is not the right formtypeid
                        vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                        Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.InlandRvLimits_Exceeded_CovC));
                    }

                    // testing INValid
                    inland.IncreasedLimit = "12500";
                    rv.CostNew = "12501";
                    if (formsThatApply.Contains(formType))
                    {
                        // should trigger check and should fail
                        vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                        Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.InlandRvLimits_Exceeded_CovC));
                    }
                    else
                    {
                        // should not even try validation since it is not the right formtypeid
                        vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.ValidateHOMCoverages(q, valType);
                        Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.Coverages_Hom_Validator.InlandRvLimits_Exceeded_CovC));
                    }
                }
            }
        }
    }
}