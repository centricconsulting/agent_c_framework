using IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.HOM
{
    [TestClass]
    public class PropertyUpdatesValidatorTests : VRQQLibBase
    {
        [TestMethod]
        public void PropertyUpdatesValidator_Tests()
        {
            var Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.PropertyUpdatesValidator.ValidateHOMPropertyUpdates(null, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.QuoteNull), "Quote Null failed.#1");

            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;

            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.PropertyUpdatesValidator.ValidateHOMPropertyUpdates(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(PropertyUpdatesValidator.QuoteNull), "Quote Null failed.#2");
            Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.UpdatesNoLocations), "No Locations");

            q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
            var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
            q.Locations.Add(l);
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.PropertyUpdatesValidator.ValidateHOMPropertyUpdates(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(PropertyUpdatesValidator.UpdatesNoLocations), "No Locations");

            // PropertyAgeThatRequiresUpdates holds a constant that determines the age of building that will require updates
            // less than 30 years so no updates needed
            l = new QuickQuote.CommonObjects.QuickQuoteLocation();
            q.Locations.Clear();
            q.Locations.Add(l);
            l.YearBuilt = DateTime.Now.AddYears(-1 * (PropertyUpdatesValidator.PropertyAgeThatRequiresUpdates - 1)).Year.ToString();
            l.Updates = new QuickQuote.CommonObjects.QuickQuoteUpdatesRecord();
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.PropertyUpdatesValidator.ValidateHOMPropertyUpdates(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.Any(), "Should have no Validations");

            //old than 30 years so need updates
            l = new QuickQuote.CommonObjects.QuickQuoteLocation();
            q.Locations.Clear();
            q.Locations.Add(l);
            l.YearBuilt = DateTime.Now.AddYears(-1 * (PropertyUpdatesValidator.PropertyAgeThatRequiresUpdates + 1)).Year.ToString();
            l.Updates = new QuickQuote.CommonObjects.QuickQuoteUpdatesRecord();
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.PropertyUpdatesValidator.ValidateHOMPropertyUpdates(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.Any(), "Should have Validations");

            //old than 30 years so need updates - Test All
            l = new QuickQuote.CommonObjects.QuickQuoteLocation();
            q.Locations.Clear();
            q.Locations.Add(l);
            l.YearBuilt = DateTime.Now.AddYears(-1 * (PropertyUpdatesValidator.PropertyAgeThatRequiresUpdates + 1)).Year.ToString();
            l.Updates = new QuickQuote.CommonObjects.QuickQuoteUpdatesRecord();
            var u = l.Updates;
            u.RoofUpdateYear = "2000";
            u.RoofUpdateTypeId = "1";
            u.RoofAsphaltShingle = true;

            u.CentralHeatUpdateYear = "2000";
            u.CentralHeatUpdateTypeId = "1";
            u.CentralHeatElectric = true;

            u.ElectricUpdateYear = "2000";
            u.ElectricUpdateTypeId = "1";
            u.Electric100Amp = true;

            u.PlumbingUpdateYear = "2000";
            u.PlumbingUpdateTypeId = "1";
            u.PlumbingCopper = true;

            u.InspectionDate = DateTime.Now.ToShortDateString();
            u.InspectionUpdateTypeId = "1";
            u.InspectionRemarks = "Remark";
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.PropertyUpdatesValidator.ValidateHOMPropertyUpdates(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.Any(), "Should have no Validations. Testing All.");

            //old than 30 years so need updates - Test All
            l = new QuickQuote.CommonObjects.QuickQuoteLocation();
            q.Locations.Clear();
            q.Locations.Add(l);
            l.YearBuilt = DateTime.Now.AddYears(-1 * (PropertyUpdatesValidator.PropertyAgeThatRequiresUpdates + 1)).Year.ToString();
            l.Updates = new QuickQuote.CommonObjects.QuickQuoteUpdatesRecord();
            u = l.Updates;
            u.RoofUpdateYear = "a";
            u.RoofUpdateTypeId = "b";
            //u.RoofAsphaltShingle = true;

            u.CentralHeatUpdateYear = "c";
            u.CentralHeatUpdateTypeId = "d";
            //u.CentralHeatElectric = true;

            u.ElectricUpdateYear = "e";
            u.ElectricUpdateTypeId = "f";
            //u.Electric100Amp = true;

            u.PlumbingUpdateYear = "g";
            u.PlumbingUpdateTypeId = "h";
            //u.PlumbingCopper = true;

            u.InspectionDate = "ee"; // DateTime.Now.ToShortDateString();
            u.InspectionUpdateTypeId = "i";
            u.InspectionRemarks = "";
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.PropertyUpdatesValidator.ValidateHOMPropertyUpdates(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.UpdatesRoofYear), "UpdatesRoofYear" + PrintTestValue(u.RoofUpdateYear));
            Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.UpadtesRoofUpdateType), "UpadtesRoofUpdateType" + PrintTestValue(u.RoofUpdateTypeId));
            Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.UpdatesRoofMaterialType), "UpdatesRoofMaterialType" + PrintTestValue("Non True"));

            Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.UpdatesHeatYear), "UpdatesHeatYear" + PrintTestValue(u.CentralHeatUpdateYear));
            Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.UpdatesHeatUpdateType), "UpdatesHeatUpdateType" + PrintTestValue(u.CentralHeatUpdateTypeId));
            Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.UpdatesHeatMaterialType), "UpdatesHeatMaterialType" + PrintTestValue("Non True"));

            Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.UpdatesElectricYear), "UpdatesElectricYear" + PrintTestValue(u.ElectricUpdateYear));
            Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.UpdatesElectricUpdateType), "UpdatesElectricUpdateType" + PrintTestValue(u.ElectricUpdateTypeId));
            Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.UpdatesElectricMaterialType), "UpdatesElectricMaterialType" + PrintTestValue("Non True"));

            Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.UpdatesPlumbingYear), "UpdatesPlumbingYear" + PrintTestValue(u.PlumbingUpdateYear));
            Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.UpdatesPlumbingUpdateType), "UpdatesPlumbingUpdateType" + PrintTestValue(u.PlumbingUpdateTypeId));
            Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.UpdatesPlumbingMaterialType), "UpdatesPlumbingMaterialType" + PrintTestValue("Non True"));

            Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.UpdatesInspectionDate), "UpdatesInspectionDate" + PrintTestValue(u.InspectionDate));
            Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.UpdatesInspectionUpdateType), "UpdatesInspectionUpdateType" + PrintTestValue(u.InspectionUpdateTypeId));
            //Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.UpdatesInspectionRemarks), "UpdatesInspectionRemarks" + PrintTestValue(u.InspectionRemarks));

            // testing default date
            u.InspectionDate = IFM.Common.InputValidation.InputHelpers.DiamondDefaultDate_string;
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.PropertyUpdatesValidator.ValidateHOMPropertyUpdates(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.UpdatesInspectionDate), "UpdatesInspectionDate" + PrintTestValue(u.InspectionDate));
        }

        [TestMethod]
        public void PropertyUpdatesValidator_Test_MobileHomeWidthLength()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
            q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
            var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
            q.Locations.Add(l);

            string[] formTypes_Mobile = new string[] { "6", "7" };
            foreach (string formType in formTypes_Mobile)
            {
                l.FormTypeId = formType;

                l.MobileHomeWidth = "";
                l.MobileHomeLength = "";
                var Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.PropertyUpdatesValidator.ValidateHOMPropertyUpdates(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.MobileHomeWidth), "MobileHomeWidth" + PrintTestValue(l.MobileHomeWidth));
                Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.MobileHomeLength), "MobileHomeLength" + PrintTestValue(l.MobileHomeLength));

                l.MobileHomeWidth = "5";
                l.MobileHomeLength = "6";
                Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.PropertyUpdatesValidator.ValidateHOMPropertyUpdates(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsFalse(Validations.ListHasValidationId(PropertyUpdatesValidator.MobileHomeWidth), "MobileHomeWidth" + PrintTestValue(l.MobileHomeWidth));
                Assert.IsFalse(Validations.ListHasValidationId(PropertyUpdatesValidator.MobileHomeLength), "MobileHomeLength" + PrintTestValue(l.MobileHomeLength));

                l.MobileHomeWidth = "a";
                l.MobileHomeLength = "b";
                Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.PropertyUpdatesValidator.ValidateHOMPropertyUpdates(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.MobileHomeWidth), "MobileHomeWidth" + PrintTestValue(l.MobileHomeWidth));
                Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.MobileHomeLength), "MobileHomeLength" + PrintTestValue(l.MobileHomeLength));

                l.MobileHomeWidth = "1000";
                l.MobileHomeLength = "1000";
                Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.PropertyUpdatesValidator.ValidateHOMPropertyUpdates(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.MobileHomeWidth), "MobileHomeWidth" + PrintTestValue(l.MobileHomeWidth));
                Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.MobileHomeLength), "MobileHomeLength" + PrintTestValue(l.MobileHomeLength));
            }

            // TEST NON MOBILE FORM TYPE - Should not need width or length
            l.FormTypeId = "3";
            l.MobileHomeWidth = "";
            l.MobileHomeLength = "";
            var V = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.PropertyUpdatesValidator.ValidateHOMPropertyUpdates(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(V.ListHasValidationId(PropertyUpdatesValidator.MobileHomeWidth), "MobileHomeWidth" + PrintTestValue(l.MobileHomeWidth));
            Assert.IsFalse(V.ListHasValidationId(PropertyUpdatesValidator.MobileHomeLength), "MobileHomeLength" + PrintTestValue(l.MobileHomeLength));
        }

        [TestMethod]
        public void PropertyUpdatesValidator_Test_Farm_Occupants()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm;
            q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
            var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
            q.Locations.Add(l);

            l.Remarks = "";
            var Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.PropertyUpdatesValidator.ValidateHOMPropertyUpdates(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(PropertyUpdatesValidator.Occupant), "");

            l.Remarks = "I'm the occupant";
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.PropertyUpdatesValidator.ValidateHOMPropertyUpdates(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(PropertyUpdatesValidator.Occupant), "");
        }
    }
}