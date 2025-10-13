using IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.PPA
{
    [TestClass]
    public class VehicleValidatorTests : VRQQLibBase
    {
        [TestMethod]
        public void _NOTDONE_VehicleTest()
        {
            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleCostNew), "Cost New" + PrintTestValue(v.CostNew));
            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleVIN), "Vin" + PrintTestValue(v.Vin));
            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleMake), "Make" + PrintTestValue(v.Make));
            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleModel), "Model" + PrintTestValue(v.Model));
            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleYear), "Vehicle Year" + PrintTestValue(v.Year));
            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleBodyType), "Body Type" + PrintTestValue(v.BodyTypeId));
            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleUse), "Use" + PrintTestValue(v.VehicleUseTypeId));
            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehiclePerformace), "Performance" + PrintTestValue(v.PerformanceTypeId));

            //Assert.IsFalse(Validations.ListHasValidationId(VehicleValidator.VehicleMotorCycleType), "MotorCycleType" + PrintTestValue(v.VehicleTypeId));
            //Assert.IsFalse(Validations.ListHasValidationId(VehicleValidator.VehicleMotorCycleHorsePower), "Horse Power" + PrintTestValue(v.CubicCentimeters));

            //Assert.IsFalse(Validations.ListHasValidationId(VehicleValidator.VehicleStatedAmount), "Stated amount" + PrintTestValue(v.StatedAmount));

            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehiclePrimaryAssignedDriver), "Prim Driver" + PrintTestValue(v.PrincipalDriverNum));

            //Assert.IsFalse(Validations.ListHasValidationId(VehicleValidator.VehicleBothNonOwnerTypesSelected), "Non owners" + PrintTestValue(v.NonOwned));

            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleSymbols), "Symbols" + PrintTestValue(v.VehicleSymbols));

            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.vehicleActualCashValue), "Actual Cash Value" + PrintTestValue(v.ActualCashValue));

            var q = GetNewQuickQuote();
            q.LobId = "1";

            // Test NULL
            var Validations = VehicleValidator.VehicleValidation(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.NoVehicles), "No vehicles" + PrintTestValue(q.Vehicles));

            q.Vehicles = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteVehicle>();
            QuickQuote.CommonObjects.QuickQuoteVehicle v = new QuickQuote.CommonObjects.QuickQuoteVehicle();
            q.Vehicles.Add(v);
            // Test EMPTY
            Validations = VehicleValidator.VehicleValidation(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleVIN), "Vin" + PrintTestValue(v.Vin));
            Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleMake), "Make" + PrintTestValue(v.Make));
            Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleModel), "Model" + PrintTestValue(v.Model));
            Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleYear), "Vehicle Year" + PrintTestValue(v.Year));
            Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleBodyType), "Body Type" + PrintTestValue(v.BodyTypeId));
            Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleUse), "Use" + PrintTestValue(v.VehicleUseTypeId));
            Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehiclePerformace), "Performance" + PrintTestValue(v.PerformanceTypeId));
            Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehiclePrimaryAssignedDriver), "Prim Driver" + PrintTestValue(v.PrincipalDriverNum));

            // Now testing all of this is hard because there are so many things to test in so many combinations
            // do to all the combinations not all validation logic is ran each time..
            // an example of this is say you have non-named-no-owner selected true - things like use or performance or motorcycle type will not even be tested for because they are not valid
            // in that context

            // Test Valid
            v.Vin = "1567agfsfgfgh&";
            v.Make = "make";
            v.Model = "model";
            v.Year = DateTime.Now.Year.ToString();
            v.BodyTypeId = "1";
            v.VehicleUseTypeId = "1";
            v.PerformanceTypeId = "1";
            v.PrincipalDriverNum = "1";
            Validations = VehicleValidator.VehicleValidation(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(VehicleValidator.VehicleVIN), "Vin" + PrintTestValue(v.Vin));
            Assert.IsFalse(Validations.ListHasValidationId(VehicleValidator.VehicleMake), "Make" + PrintTestValue(v.Make));
            Assert.IsFalse(Validations.ListHasValidationId(VehicleValidator.VehicleModel), "Model" + PrintTestValue(v.Model));
            Assert.IsFalse(Validations.ListHasValidationId(VehicleValidator.VehicleYear), "Vehicle Year" + PrintTestValue(v.Year));
            Assert.IsFalse(Validations.ListHasValidationId(VehicleValidator.VehicleBodyType), "Body Type" + PrintTestValue(v.BodyTypeId));
            Assert.IsFalse(Validations.ListHasValidationId(VehicleValidator.VehicleUse), "Use" + PrintTestValue(v.VehicleUseTypeId));
            Assert.IsFalse(Validations.ListHasValidationId(VehicleValidator.VehiclePerformace), "Performance" + PrintTestValue(v.PerformanceTypeId));
            Assert.IsFalse(Validations.ListHasValidationId(VehicleValidator.VehiclePrimaryAssignedDriver), "Prim Driver" + PrintTestValue(v.PrincipalDriverNum));

            //// Test EMPTY
            //Validations = VehicleValidator.VehicleValidation(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleCostNew), "Cost New" + PrintTestValue(v.CostNew));
            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleVIN), "Vin" + PrintTestValue(v.Vin));
            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleMake), "Make" + PrintTestValue(v.Make));
            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleModel), "Model" + PrintTestValue(v.Model));
            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleYear), "Vehicle Year" + PrintTestValue(v.Year));
            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleBodyType), "Body Type" + PrintTestValue(v.BodyTypeId));
            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleUse), "Use" + PrintTestValue(v.VehicleUseTypeId));
            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehiclePerformace), "Performance" + PrintTestValue(v.PerformanceTypeId));

            //Assert.IsFalse(Validations.ListHasValidationId(VehicleValidator.VehicleMotorCycleType), "MotorCycleType" + PrintTestValue(v.VehicleTypeId));
            //Assert.IsFalse(Validations.ListHasValidationId(VehicleValidator.VehicleMotorCycleHorsePower), "Horse Power" + PrintTestValue(v.CubicCentimeters));

            //Assert.IsFalse(Validations.ListHasValidationId(VehicleValidator.VehicleStatedAmount), "Stated amount" + PrintTestValue(v.StatedAmount));

            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehiclePrimaryAssignedDriver), "Prim Driver" + PrintTestValue(v.PrincipalDriverNum));

            //Assert.IsFalse(Validations.ListHasValidationId(VehicleValidator.VehicleBothNonOwnerTypesSelected), "Non owners" + PrintTestValue(v.NonOwned));

            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.VehicleSymbols), "Symbols" + PrintTestValue(v.VehicleSymbols));

            //Assert.IsTrue(Validations.ListHasValidationId(VehicleValidator.vehicleActualCashValue), "Actual Cash Value" + PrintTestValue(v.ActualCashValue));

            
        }
    }
}