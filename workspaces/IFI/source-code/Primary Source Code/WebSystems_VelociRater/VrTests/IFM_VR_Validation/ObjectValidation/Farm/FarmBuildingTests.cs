using IFM.VR.Validation.ObjectValidation.FarmLines.Buildings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace VrTests.IFM_VR_Validation.ObjectValidation.Farm
{
    [TestClass]
    public class FarmBuildingTests : VRQQLibBase
    {
        [TestMethod]
        public void FarmBuildingPropertyTests()
        {
            var qq = GetNewQuickQuote();
            qq.LobId = "17";
            qq.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>() { new QuickQuote.CommonObjects.QuickQuoteLocation() };
            QuickQuote.CommonObjects.QuickQuoteBuilding building = null;

            // test all required are entered
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.FarmStructureTypeId = "10"; // Barn = 10 - Required
            building.ConstructionId = "7"; // Frame = 7 - Required
            building.E_Farm_Limit = "500000"; // required
            building.E_Farm_DeductibleLimitId = "75"; // 2500 = 75  - required
            building.YearBuilt = DateTime.Now.Year.ToString(); // - required
            building.SquareFeet = "2500";
            building.Dimensions = "50x50"; // - required
            building.FarmTypeId = "1"; // Type 1 = 1 - required unless farmstructureid = 27(grain dryer)
            building.Description = "Description";
            var Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.quoteIsNull), "Quote Null.");
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.farmStructureId), "");
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.construction), "");
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.limit_val), "");
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.deductible), "");
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.yearConstructed), "");
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.squareFeet), "");
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.dimensions), "");
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.buildingType), "");

            // test all required are not entered
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.FarmStructureTypeId = ""; // Barn = 10 - Required
            building.ConstructionId = ""; // Frame = 7 - Required
            building.E_Farm_Limit = ""; // required
            building.E_Farm_DeductibleLimitId = ""; // 2500 = 75  - required
            building.YearBuilt = ""; // - required
            building.SquareFeet = "";
            building.Dimensions = ""; // - required
            building.FarmTypeId = ""; // Type 1 = 1 - required unless farmstructureid = 27(grain dryer)
            building.Description = "";
            Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.quoteIsNull), "Quote Null.");
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.farmStructureId), "");
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.construction), "");
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.limit_val), "");
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.deductible), "");
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.yearConstructed), "");
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.squareFeet), "");
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.dimensions), "");
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.buildingType), "");

            // test square feet is invalid
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.SquareFeet = "abcd";
            Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.quoteIsNull), "Quote Null.");
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.squareFeet), "");
        }

        [TestMethod]
        public void FarmBuildingPropertyTests_YearBuilt()
        {
            var qq = GetNewQuickQuote();
            qq.LobId = "17";
            qq.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>() { new QuickQuote.CommonObjects.QuickQuoteLocation() };
            QuickQuote.CommonObjects.QuickQuoteBuilding building = null;

            // test year built must be current year plus 1 or less
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.YearBuilt = (DateTime.Now.Year + 2).ToString(); // - required
            var Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.yearConstructed), "");

            // test year built must be current year plus 1 or less
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.YearBuilt = "abc"; // - required
            Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.yearConstructed), "");
        }

        [TestMethod]
        public void FarmBuildingPropertyTests_SquareFeet()
        {
            var qq = GetNewQuickQuote();
            qq.LobId = "17";
            qq.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>() { new QuickQuote.CommonObjects.QuickQuoteLocation() };
            QuickQuote.CommonObjects.QuickQuoteBuilding building = null;

            // test square feet is numeric
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.SquareFeet = "abc";
            var Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.squareFeet), "");

            // test square feet is numeric and greater than zero
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.SquareFeet = "0";
            Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.squareFeet), "");

            // test square feet is required on Farm Dwellings (18) //added 10-5-2015
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.FarmStructureTypeId = "18";
            building.SquareFeet = "";
            Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.squareFeet), "");
        }

        [TestMethod]
        public void FarmBuildingPropertyTests_Heat()
        {
            var qq = GetNewQuickQuote();
            qq.LobId = "17";
            qq.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>() { new QuickQuote.CommonObjects.QuickQuoteLocation() };
            QuickQuote.CommonObjects.QuickQuoteBuilding building = null;

            string[] heatCapableFarmStructureTypeIds = new string[] { "18", "10", "11", "13", "14", "15", "17", "20", "29" };

            // heat not supported for building type
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.FarmStructureTypeId = "16"; // Silo
            building.HeatedBuildingSurchargeOther = true;
            var Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.heatIsNotSupportedOnThisFarmStructureType), "");

            foreach (var t in heatCapableFarmStructureTypeIds)
            {
                // heat is supported for building type
                qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
                building = qq.Locations[0].Buildings[0];
                building.FarmStructureTypeId = t; // Silo
                building.HeatedBuildingSurchargeOther = true;
                Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.heatIsNotSupportedOnThisFarmStructureType), "");
            }
        }

        [TestMethod]
        public void FarmBuildingPropertyTests_DeductibleOfAllBuildingsMustBeSame()
        {
            var qq = GetNewQuickQuote();
            qq.LobId = "17";
            qq.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>() { new QuickQuote.CommonObjects.QuickQuoteLocation() };
            QuickQuote.CommonObjects.QuickQuoteBuilding building = null;

            // test all required are entered
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.FarmStructureTypeId = "10"; // Barn = 10 - Required
            building.ConstructionId = "7"; // Frame = 7 - Required
            building.E_Farm_Limit = "500000"; // required
            building.E_Farm_DeductibleLimitId = "75"; // 2500 = 75  - required
            building.YearBuilt = DateTime.Now.Year.ToString(); // - required
            building.SquareFeet = "2500";
            building.Dimensions = "50x50"; // - required
            building.FarmTypeId = "1"; // Type 1 = 1 - required unless farmstructureid = 27(grain dryer)
            building.Description = "Description";

            qq.Locations[0].Buildings.Add(new QuickQuote.CommonObjects.QuickQuoteBuilding());
            var secondBuilding = qq.Locations[0].Buildings[1];

            // all buildings deductibles match so all should be good
            secondBuilding.E_Farm_DeductibleLimitId = building.E_Farm_DeductibleLimitId;
            secondBuilding.FarmStructureTypeId = "10"; // Barn = 10 - Required
            secondBuilding.ConstructionId = "7"; // Frame = 7 - Required
            secondBuilding.E_Farm_Limit = "500000"; // required
            secondBuilding.YearBuilt = DateTime.Now.Year.ToString(); // - required
            secondBuilding.Dimensions = "50x50"; // - required
            var Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 1, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.deductible), "");

            // all buildings deductibles Do NOT match so all should fail
            secondBuilding.E_Farm_DeductibleLimitId = "5"; // some random deductibleid
            Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 1, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.deductible), "");
        }

        [TestMethod]
        public void FarmBuildingPropertyTests_TestDeductibleVsLimit()
        {
            var qq = GetNewQuickQuote();
            qq.LobId = "17";
            qq.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>() { new QuickQuote.CommonObjects.QuickQuoteLocation() };
            QuickQuote.CommonObjects.QuickQuoteBuilding building = null;

            // test that limit must be higher than deductible
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.FarmStructureTypeId = "10"; // Barn = 10 - Required
            building.ConstructionId = "7"; // Frame = 7 - Required
            building.E_Farm_Limit = "2501"; // required
            building.E_Farm_DeductibleLimitId = "75"; // 2500 = 75  - required
            building.FarmTypeId = "1"; // Type 1 = 1 - required unless farmstructureid = 27(grain dryer)
            var Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.quoteIsNull), "Quote Null.");
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.deductible), "");

            // test that limit must be higher than deductible
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.FarmStructureTypeId = "10"; // Barn = 10 - Required
            building.ConstructionId = "7"; // Frame = 7 - Required
            building.E_Farm_Limit = "2500"; // required
            building.E_Farm_DeductibleLimitId = "75"; // 2500 = 75  - required
            building.FarmTypeId = "1"; // Type 1 = 1 - required unless farmstructureid = 27(grain dryer)
            Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.quoteIsNull), "Quote Null.");
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.deductible), "");
        }

        [TestMethod]
        public void FarmBuildingPropertyTest_FarmStructureType_GrainDryer()
        {
            // test relationship between farm structuretype, limit, and farmtypeid(building type)

            var qq = GetNewQuickQuote();
            qq.LobId = "17";
            qq.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>() { new QuickQuote.CommonObjects.QuickQuoteLocation() };
            QuickQuote.CommonObjects.QuickQuoteBuilding building = null;

            // test when farmstructuretypeid = 27(Grain Dryer) the farmtypeid must be n/a(5)
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.FarmStructureTypeId = "27";
            building.E_Farm_Limit = "5000";
            building.FarmTypeId = "5"; // must be n/a (5)
            var Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.buildingType), "");

            // test when farmstructuretypeid = 27(Grain Dryer) the farmtypeid is not needed
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.FarmStructureTypeId = "27";
            building.E_Farm_Limit = "5000";
            building.FarmTypeId = "1"; // invalid in this case
            Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.buildingType), "");
        }

        [TestMethod]
        public void FarmBuildingPropertyTest_FarmStructureType_GrainBin()
        {
            // test relationship between farm structuretype, limit, and farmtypeid(building type)

            var qq = GetNewQuickQuote();
            qq.LobId = "17";
            qq.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>() { new QuickQuote.CommonObjects.QuickQuoteLocation() };
            QuickQuote.CommonObjects.QuickQuoteBuilding building = null;

            // test when struct type is 12 and limit is under 1,000 then farmtype must be [type 3]
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.FarmStructureTypeId = "12"; // Grain Bin
            building.E_Farm_Limit = "999"; // required
            building.FarmTypeId = "1"; // must be [type 3] = 3 because limit is under $1000
            var Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.buildingType), "");

            // test when struct type is 12 and limit is under 1,000 then farmtype must be [type 3]
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.FarmStructureTypeId = "12"; // Grain Bin
            building.E_Farm_Limit = "1000"; // required
            building.FarmTypeId = "1"; // must be [type 3] = 3 or [Type 2] = 2 because limit is under $3000
            Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.buildingType), "");

            // test when struct type is 12 and limit is under 1,000 then farmtype must be [type 3]
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.FarmStructureTypeId = "12"; // Grain Bin
            building.E_Farm_Limit = "1000"; // required
            building.FarmTypeId = "2"; // must be [type 3] = 3 or [Type 2] = 2 because limit is under $3000
            Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.buildingType), "");

            // test when struct type is 12 and limit is under 1,000 then farmtype must be [type 3]
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.FarmStructureTypeId = "12"; // Grain Bin
            building.E_Farm_Limit = "1000"; // required
            building.FarmTypeId = "3"; // must be [type 3] = 3 or [Type 2] = 2 because limit is under $3000
            Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.buildingType), "");
        }

        [TestMethod]
        public void FarmBuildingPropertyTest_FarmStructureType_HoopBuilding()
        {
            // test relationship between farm structuretype, limit, and farmtypeid(building type)

            var qq = GetNewQuickQuote();
            qq.LobId = "17";
            qq.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>() { new QuickQuote.CommonObjects.QuickQuoteLocation() };
            QuickQuote.CommonObjects.QuickQuoteBuilding building = null;

            // test when struct type is 19 then farmtype must be [type 2 open]
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.FarmStructureTypeId = "19"; // Hoop Building
            building.E_Farm_Limit = "999"; // required
            building.FarmTypeId = "8"; // no higher than [type 2 open] = 8
            var Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.buildingType), "");

            // test when struct type is 19 then farmtype must be [type 2 open]
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.FarmStructureTypeId = "19"; // Hoop Building
            building.E_Farm_Limit = "999"; // required
            building.FarmTypeId = "3"; // no higher than [type 2 open] = 8
            Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.buildingType), "");

            // test when struct type is 19 then farmtype must be [type 2 open]
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.FarmStructureTypeId = "19"; // Hoop Building
            building.E_Farm_Limit = "999"; // required
            building.FarmTypeId = "2"; // no higher than [type 2 open] = 8
            Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.buildingType), "");
        }

        [TestMethod]
        public void FarmBuildingPropertyTest_FarmStructureType_GreenHouse()
        {
            // test relationship between farm structuretype, limit, and farmtypeid(building type)

            var qq = GetNewQuickQuote();
            qq.LobId = "17";
            qq.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>() { new QuickQuote.CommonObjects.QuickQuoteLocation() };
            QuickQuote.CommonObjects.QuickQuoteBuilding building = null;

            // test when struct type is 32 then farmtype must be [type 3]
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.FarmStructureTypeId = "32"; // GreenHouse
            building.E_Farm_Limit = "999"; // required
            building.FarmTypeId = "3"; // must be [type 3] = 3
            var Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingValidator.buildingType), "");

            // test when struct type is 32 then farmtype must be [type 3]
            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];
            building.FarmStructureTypeId = "32"; // GreenHouse
            building.E_Farm_Limit = "999"; // required
            building.FarmTypeId = "8"; // must be [type 3] = 3
            Validations = FarmBuildingValidator.ValidateFARBuildingProperty(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingValidator.buildingType), "");
        }

        [TestMethod]
        public void FarmBuildingCoverageTests()
        {
            var qq = GetNewQuickQuote();
            qq.LobId = "17";
            qq.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>() { new QuickQuote.CommonObjects.QuickQuoteLocation() };
            QuickQuote.CommonObjects.QuickQuoteBuilding building = null;

            qq.Locations[0].Buildings = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteBuilding>() { new QuickQuote.CommonObjects.QuickQuoteBuilding() };
            building = qq.Locations[0].Buildings[0];

            QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE perilsCov = new QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE();
            perilsCov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Barn_Additional_Perils;
            QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE eqCov = new QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE();
            eqCov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Earthquake_Contents;

            building.OptionalCoverageEs = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE>();

            // only farmstructuretypeid of 17 and 18 allow 'Additional Perils' and 'EQ Contents' coverages - VALID
            building.OptionalCoverageEs.Clear();
            building.FarmStructureTypeId = "10"; // Barn = 10 - Required
            var Validations = FarmBuildingCoveragesValidator.ValidateFARBuildingCoverages(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingCoveragesValidator.additionalPerils), "");
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingCoveragesValidator.eq_contents), "");

            // only farmstructuretypeid of 17 and 18 allow 'Additional Perils' and 'EQ Contents' coverages - INVALID
            building.OptionalCoverageEs.Clear();
            building.OptionalCoverageEs.Add(perilsCov);
            building.OptionalCoverageEs.Add(eqCov);
            building.FarmStructureTypeId = "10"; // Barn = 10 - Required
            Validations = FarmBuildingCoveragesValidator.ValidateFARBuildingCoverages(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingCoveragesValidator.additionalPerils), "");
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingCoveragesValidator.eq_contents), "");

            // only farmstructuretypeid of 17 and 18 allow 'Additional Perils' and 'EQ Contents' coverages - VALID
            building.OptionalCoverageEs.Clear();
            building.OptionalCoverageEs.Add(perilsCov);
            building.OptionalCoverageEs.Add(eqCov);
            building.FarmStructureTypeId = "17"; // Barn = 10 - Required
            Validations = FarmBuildingCoveragesValidator.ValidateFARBuildingCoverages(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingCoveragesValidator.additionalPerils), "");
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingCoveragesValidator.eq_contents), "");

            // only farmstructuretypeid of 17 and 18 allow 'Additional Perils' and 'EQ Contents' coverages - VALID
            building.OptionalCoverageEs.Clear();
            building.OptionalCoverageEs.Add(perilsCov);
            building.OptionalCoverageEs.Add(eqCov);
            building.FarmStructureTypeId = "17"; // Barn = 10 - Required
            Validations = FarmBuildingCoveragesValidator.ValidateFARBuildingCoverages(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingCoveragesValidator.additionalPerils), "");
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingCoveragesValidator.eq_contents), "");

            // only specific counties allow mine subsidence
            QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE mineSubCov = new QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE();
            mineSubCov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Farm_Mine_Subsidence;
            building.OptionalCoverageEs.Clear();
            building.OptionalCoverageEs.Add(mineSubCov);
            Validations = FarmBuildingCoveragesValidator.ValidateFARBuildingCoverages(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingCoveragesValidator.mineSubsidence), "");

            // only specific counties allow mine subsidence - VALID
            mineSubCov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Farm_Mine_Subsidence;
            building.OptionalCoverageEs.Clear();
            building.OptionalCoverageEs.Add(mineSubCov);
            qq.Locations[0].Address.County = "PIKE";
            Validations = FarmBuildingCoveragesValidator.ValidateFARBuildingCoverages(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(FarmBuildingCoveragesValidator.mineSubsidence), "");

            // must have business income limit
            QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE incomeCov = new QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE();
            incomeCov.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.LossOfIncome_Rents;
            building.OptionalCoverageEs.Clear();
            building.OptionalCoverageEs.Add(incomeCov);
            Validations = FarmBuildingCoveragesValidator.ValidateFARBuildingCoverages(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingCoveragesValidator.businessIncome), "");

            // must have VALID business income limit
            building.OptionalCoverageEs.Clear();
            building.OptionalCoverageEs.Add(incomeCov);
            incomeCov.IncreasedLimit = "abc";
            Validations = FarmBuildingCoveragesValidator.ValidateFARBuildingCoverages(qq, 0, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(FarmBuildingCoveragesValidator.businessIncome), "");
        }
    }
}