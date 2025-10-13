using IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.PPA
{
    [TestClass]
    public class VehicleListValidatorTests : VRQQLibBase
    {
        [TestMethod]
        public void VehicleListValidator_Tests()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal;
            q.Drivers = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteDriver>();
            QuickQuote.CommonObjects.QuickQuoteDriver d = new QuickQuote.CommonObjects.QuickQuoteDriver();
            q.Drivers.Add(d);

            // testing null/empty vehicle list
            var Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.VehicleListValidator.ValidateVehicleList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(VehicleListValidator.VehicleListNoVehicles), "VehicleListNoVehicles" + PrintTestValue(q.Vehicles));

            // testing empty
            q.Vehicles = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteVehicle>();
            QuickQuote.CommonObjects.QuickQuoteVehicle v = new QuickQuote.CommonObjects.QuickQuoteVehicle();
            q.Vehicles.Add(v);
            d.DriverExcludeTypeId = "1";
            v.PrincipalDriverNum = "";
            v.BodyTypeId = "42"; // MotorCycle = 42
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.VehicleListValidator.ValidateVehicleList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(VehicleListValidator.VehicleListNotAllDriversAreAssignedToAVehicle), "VehicleListNoHasMotorCycleVehicles" + PrintTestValue(null));
            Assert.IsTrue(Validations.ListHasValidationId(VehicleListValidator.VehicleListNoHasMotorCycleVehicles), "VehicleListNoHasMotorCycleVehicles" + PrintTestValue(null));

            // testing Valid
            d.DriverExcludeTypeId = "1";
            v.PrincipalDriverNum = "1";
            v.BodyTypeId = "14"; // MotorCycle = 42, Car = 14
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.VehicleListValidator.ValidateVehicleList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(VehicleListValidator.VehicleListNotAllDriversAreAssignedToAVehicle), "VehicleListNoHasMotorCycleVehicles" + PrintTestValue(null));
            Assert.IsFalse(Validations.ListHasValidationId(VehicleListValidator.VehicleListNoHasMotorCycleVehicles), "VehicleListNoHasMotorCycleVehicles" + PrintTestValue(null));
        }
    }
}