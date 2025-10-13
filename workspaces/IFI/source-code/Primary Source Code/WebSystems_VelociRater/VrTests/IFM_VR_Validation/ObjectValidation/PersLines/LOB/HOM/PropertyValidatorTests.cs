using IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.HOM
{
    [TestClass]
    public class PropertyValidatorTests : VRQQLibBase
    {
        [TestMethod]
        public void PropertyValidator_Test_Residence()
        {
            var Validations = LocationResidenceValidator.ValidateHOMLocationResidence(null, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(LocationResidenceValidator.QuoteIsNull), "QuoteIsNull" + PrintTestValue(null));

            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
            q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
            var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
            q.Locations.Add(l);

            string[] formTypes_year = new string[] { "1", "2", "3", "5", "6", "7", "22", "23", "24", "25", "26" };
            foreach (string formType in formTypes_year)
            {
                // test year built - Empty
                l.FormTypeId = formType;
                l.YearBuilt = "";
                Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsTrue(Validations.ListHasValidationId(LocationResidenceValidator.LocationYearBuilt), "LocationYearBuilt" + PrintTestValue(l.YearBuilt));

                // test year built - This Year
                l.FormTypeId = formType;
                l.YearBuilt = DateTime.Now.Year.ToString();
                Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsFalse(Validations.ListHasValidationId(LocationResidenceValidator.LocationYearBuilt), "LocationYearBuilt" + PrintTestValue(l.YearBuilt));

                // test year built - Two Years in future
                l.FormTypeId = formType;
                l.YearBuilt = DateTime.Now.AddYears(2).Year.ToString();
                Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsTrue(Validations.ListHasValidationId(LocationResidenceValidator.LocationYearBuilt), "LocationYearBuilt" + PrintTestValue(l.YearBuilt));

                // test year built - year way too far in past
                l.FormTypeId = formType;
                l.YearBuilt = "1492";
                Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsTrue(Validations.ListHasValidationId(LocationResidenceValidator.LocationYearBuilt), "LocationYearBuilt" + PrintTestValue(l.YearBuilt));

                // test year built - Home more than 50 years old warning
                l.FormTypeId = formType;
                l.YearBuilt = DateTime.Now.AddYears(-51).Year.ToString();
                Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsTrue(Validations.ListHasValidationId(LocationResidenceValidator.LocationYearBuilt), "LocationYearBuilt" + PrintTestValue(l.YearBuilt));
            }

            // test year built - Mobile Home more than 15 years old warning - form Type 6
            l.FormTypeId = "6";
            l.YearBuilt = DateTime.Now.AddYears(-16).Year.ToString();
            Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(LocationResidenceValidator.LocationYearBuilt), "LocationYearBuilt" + PrintTestValue(l.YearBuilt));

            // test year built - Mobile Home more than 15 years old warning - form type 7
            l.FormTypeId = "7";
            l.YearBuilt = DateTime.Now.AddYears(-16).Year.ToString();
            Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(LocationResidenceValidator.LocationYearBuilt), "LocationYearBuilt" + PrintTestValue(l.YearBuilt));

            // test year built - Form Type 4 does not require year built
            l.FormTypeId = "4";
            l.YearBuilt = "";
            Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(LocationResidenceValidator.LocationYearBuilt), "LocationYearBuilt" + PrintTestValue(l.YearBuilt));

            // test year built - Form Type 4 does not require year built - But yaer built must still be valid
            l.FormTypeId = "4";
            l.YearBuilt = "abc";
            Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(LocationResidenceValidator.LocationYearBuilt), "LocationYearBuilt" + PrintTestValue(l.YearBuilt));

            string[] formtypes_squarefeet = new string[] { "1", "2", "3", "22", "23", "24" };

            foreach (string formType in formtypes_squarefeet)
            {
                // test Square Feet - INVALID
                l.FormTypeId = formType;
                l.SquareFeet = "abc";
                Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsTrue(Validations.ListHasValidationId(LocationResidenceValidator.LocationSquareFeet), "LocationSquareFeet" + PrintTestValue(l.SquareFeet));

                // test Square Feet - EMPTY
                l.FormTypeId = formType;
                l.SquareFeet = "";
                Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsTrue(Validations.ListHasValidationId(LocationResidenceValidator.LocationSquareFeet), "LocationSquareFeet" + PrintTestValue(l.SquareFeet));

                // test Square Feet - should be valid
                l.FormTypeId = formType;
                l.SquareFeet = "1550";
                Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsFalse(Validations.ListHasValidationId(LocationResidenceValidator.LocationSquareFeet), "LocationSquareFeet" + PrintTestValue(l.SquareFeet));
            }

            string[] formtypes_squarefeet_NotRequired = new string[] { "4", "5" };

            foreach (string formType in formtypes_squarefeet_NotRequired)
            {
                // test Square Feet - INVALID
                l.FormTypeId = formType;
                l.SquareFeet = "abc";
                Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsTrue(Validations.ListHasValidationId(LocationResidenceValidator.LocationSquareFeet), "LocationSquareFeet" + PrintTestValue(l.SquareFeet));

                // test Square Feet - EMPTY - Not required for this form type
                l.FormTypeId = formType;
                l.SquareFeet = "";
                Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsFalse(Validations.ListHasValidationId(LocationResidenceValidator.LocationSquareFeet), "LocationSquareFeet" + PrintTestValue(l.SquareFeet));

                // test Square Feet - should be valid
                l.FormTypeId = formType;
                l.SquareFeet = "1550";
                Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsFalse(Validations.ListHasValidationId(LocationResidenceValidator.LocationSquareFeet), "LocationSquareFeet" + PrintTestValue(l.SquareFeet));
            }

            string[] formtypes_Structure = new string[] { "1", "2", "3", "4", "5", "6", "7" };
            foreach (string formType in formtypes_Structure)
            {
                // test empty
                l.FormTypeId = formType;
                l.StructureTypeId = "";
                Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsTrue(Validations.ListHasValidationId(LocationResidenceValidator.LocationStructure), "LocationStructure" + PrintTestValue(l.StructureTypeId));

                //test for valid
                l.StructureTypeId = "2";
                Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsFalse(Validations.ListHasValidationId(LocationResidenceValidator.LocationStructure), "LocationStructure" + PrintTestValue(l.StructureTypeId));
            }

            string[] formtypes_Occupancy = new string[] { "1", "2", "3", "4", "5", "6", "7" };
            foreach (string formType in formtypes_Occupancy)
            {
                // test empty
                l.FormTypeId = formType;
                l.OccupancyCodeId = "";
                Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsTrue(Validations.ListHasValidationId(LocationResidenceValidator.LocationOccupancy), "LocationOccupancy" + PrintTestValue(l.OccupancyCodeId));

                //test for valid
                l.OccupancyCodeId = "2";
                Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsFalse(Validations.ListHasValidationId(LocationResidenceValidator.LocationOccupancy), "LocationOccupancy" + PrintTestValue(l.OccupancyCodeId));
            }

            string[] formtypes_Construction = new string[] { "1", "2", "3", "5" };
            foreach (string formType in formtypes_Construction)
            {
                // test empty
                l.FormTypeId = formType;
                l.ConstructionTypeId = "";
                Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsTrue(Validations.ListHasValidationId(LocationResidenceValidator.LocationConstruction), "LocationConstruction" + PrintTestValue(l.ConstructionTypeId));

                //test for valid
                l.ConstructionTypeId = "2";
                Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsFalse(Validations.ListHasValidationId(LocationResidenceValidator.LocationConstruction), "LocationConstruction" + PrintTestValue(l.ConstructionTypeId));
            }

            string[] formtypes_Construction_NotRequired = new string[] { "4", "6", "7" };
            foreach (string formType in formtypes_Construction_NotRequired)
            {
                // test empty
                l.FormTypeId = formType;
                l.ConstructionTypeId = "";
                Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsFalse(Validations.ListHasValidationId(LocationResidenceValidator.LocationConstruction), "LocationConstruction" + PrintTestValue(l.ConstructionTypeId));
            }

            // testing new home discount removal on Pre-Fab structure types less than 10 years old
            l.StructureTypeId = "14";
            l.YearBuilt = DateTime.Now.Year.ToString();
            Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(LocationResidenceValidator.PreFabNewHomeDiscountRemovedWarning), "LocationConstruction" + PrintTestValue(l.ConstructionTypeId));

            // testing new home discount removal on Pre-Fab structure types more than 10 years old
            l.StructureTypeId = "14";
            l.YearBuilt = (DateTime.Now.Year - 11).ToString();
            Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(LocationResidenceValidator.PreFabNewHomeDiscountRemovedWarning), "LocationConstruction" + PrintTestValue(l.ConstructionTypeId));

            // testing new home discount removal warning message on HO-4 that are less than 10 years old
            l.StructureTypeId = "12";
            l.FormTypeId = "4";
            l.YearBuilt = DateTime.Now.Year.ToString();
            Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(LocationResidenceValidator.HO4NewHomeDiscountRemovedWarning), "Form Type" + PrintTestValue(l.FormTypeId));

            // testing new home discount removal warning message on HO-4 that are less than 10 years old
            l.StructureTypeId = "12";
            l.FormTypeId = "4";
            l.YearBuilt = (DateTime.Now.Year - 11).ToString();
            Validations = LocationResidenceValidator.ValidateHOMLocationResidence(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(LocationResidenceValidator.HO4NewHomeDiscountRemovedWarning), "Form Type" + PrintTestValue(l.FormTypeId));
        }

        [TestMethod]
        public void PropertyValidator_Test_MobileSection()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
            q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
            var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
            q.Locations.Add(l);

            string[] formTypes_Mobile = new string[] { "6", "7" };
            //All should fail
            foreach (string formType in formTypes_Mobile)
            {
                l.FormTypeId = formType;
                l.MobileHomeTieDownTypeId = "";
                l.MobileHomeSkirtTypeId = "";
                l.FoundationTypeId = "";
                var Validations = LocationMobileHomeValidator.ValidateHOMMobileHome(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsTrue(Validations.ListHasValidationId(LocationMobileHomeValidator.LocationTieDown), "LocationTieDown" + PrintTestValue(l.MobileHomeTieDownTypeId));
                Assert.IsTrue(Validations.ListHasValidationId(LocationMobileHomeValidator.LocationSkirting), "LocationSkirting" + PrintTestValue(l.MobileHomeSkirtTypeId));
                Assert.IsTrue(Validations.ListHasValidationId(LocationMobileHomeValidator.LocationFoundationType), "LocationFoundationType" + PrintTestValue(l.FoundationTypeId));
            }

            // all should pass
            foreach (string formType in formTypes_Mobile)
            {
                l.FormTypeId = formType;
                l.MobileHomeTieDownTypeId = "2";
                l.MobileHomeSkirtTypeId = "2";
                l.FoundationTypeId = "1";
                var Validations = LocationMobileHomeValidator.ValidateHOMMobileHome(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsFalse(Validations.ListHasValidationId(LocationMobileHomeValidator.LocationTieDown), "LocationTieDown" + PrintTestValue(l.MobileHomeTieDownTypeId));
                Assert.IsFalse(Validations.ListHasValidationId(LocationMobileHomeValidator.LocationSkirting), "LocationSkirting" + PrintTestValue(l.MobileHomeSkirtTypeId));
                Assert.IsFalse(Validations.ListHasValidationId(LocationMobileHomeValidator.LocationFoundationType), "LocationFoundationType" + PrintTestValue(l.FoundationTypeId));
            }

            string[] formTypes_NonMobile = new string[] { "1", "2", "3", "4", "5" };
            //All should be fine not required with these non mobile formtypes
            foreach (string formType in formTypes_NonMobile)
            {
                l.FormTypeId = formType;
                l.MobileHomeTieDownTypeId = "";
                l.MobileHomeSkirtTypeId = "";
                l.FoundationTypeId = "";
                var Validations = LocationMobileHomeValidator.ValidateHOMMobileHome(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsFalse(Validations.ListHasValidationId(LocationMobileHomeValidator.LocationTieDown), "LocationTieDown" + PrintTestValue(l.MobileHomeTieDownTypeId));
                Assert.IsFalse(Validations.ListHasValidationId(LocationMobileHomeValidator.LocationSkirting), "LocationSkirting" + PrintTestValue(l.MobileHomeSkirtTypeId));
                Assert.IsFalse(Validations.ListHasValidationId(LocationMobileHomeValidator.LocationFoundationType), "LocationFoundationType" + PrintTestValue(l.FoundationTypeId));
            }
        }

        [TestMethod]
        public void PropertyValidator_Test_ProtectionClass()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
            q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
            var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
            q.Locations.Add(l);

            string[] formTypes = new string[] { "1", "2", "3", "4", "5", "6", "7" };
            //All should fail
            foreach (string formType in formTypes)
            {
                l.FormTypeId = formType;
                l.ProtectionClassId = "";
                var Validations = LocationProtectionClassValidator.ValidateHOMProtectionClass(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsTrue(Validations.ListHasValidationId(LocationProtectionClassValidator.LocationProtectionClass), "LocationProtectionClass" + PrintTestValue(l.ProtectionClassId));
            }

            //All should  Pass
            foreach (string formType in formTypes)
            {
                l.FormTypeId = formType;
                l.ProtectionClassId = "2";
                var Validations = LocationProtectionClassValidator.ValidateHOMProtectionClass(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                Assert.IsFalse(Validations.ListHasValidationId(LocationProtectionClassValidator.LocationProtectionClass), "LocationProtectionClass" + PrintTestValue(l.ProtectionClassId));
            }

            //test that protection class id of 11 is not valid on mobile home form types
            foreach (string formType in formTypes)
            {
                l.FormTypeId = formType;
                l.ProtectionClassId = "11";
                var Validations = LocationProtectionClassValidator.ValidateHOMProtectionClass(q, 0, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                if (formType == "6" || formType == "7")
                {
                    Assert.IsTrue(Validations.ListHasValidationId(LocationProtectionClassValidator.LocationProtectionClass), "LocationProtectionClass" + PrintTestValue(l.ProtectionClassId));
                }
                else
                {
                    Assert.IsFalse(Validations.ListHasValidationId(LocationProtectionClassValidator.LocationProtectionClass), "LocationProtectionClass" + PrintTestValue(l.ProtectionClassId));
                }
            }
        }

        [TestMethod]
        public void PropertyValidator_Test_FirstWrittenDate()
        {
            foreach (var lobType in IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs_Enums())
            {
                switch (lobType)
                {
                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal:
                        {//scope control
                            var q = GetNewQuickQuote();
                            q.LobType = lobType;
                            q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
                            var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
                            q.Locations.Add(l);

                            // test invalid date
                            q.FirstWrittenDate = "a";
                            var Validations = PropertyValidator.ValidateHOMLocation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                            Assert.IsTrue(Validations.ListHasValidationId(PropertyValidator.FirstWrittenDate), "FirstWrittenDate" + PrintTestValue(q.FirstWrittenDate));

                            // test invalid date - default
                            q.FirstWrittenDate = IFM.Common.InputValidation.InputHelpers.DiamondDefaultDate_string;
                            Validations = PropertyValidator.ValidateHOMLocation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                            Assert.IsTrue(Validations.ListHasValidationId(PropertyValidator.FirstWrittenDate), "FirstWrittenDate" + PrintTestValue(q.FirstWrittenDate));

                            // test invalid date - mUst be than  effective date or in past - unless effective date isn't set then must be today or in past
                            q.FirstWrittenDate = DateTime.Now.AddDays(1).ToShortDateString();
                            Validations = PropertyValidator.ValidateHOMLocation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                            Assert.IsTrue(Validations.ListHasValidationId(PropertyValidator.FirstWrittenDate), "FirstWrittenDate" + PrintTestValue(q.FirstWrittenDate));

                            // test invalid  - Can not be today or in future
                            q.FirstWrittenDate = DateTime.Now.AddDays(-1).ToShortDateString();
                            Validations = PropertyValidator.ValidateHOMLocation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                            Assert.IsFalse(Validations.ListHasValidationId(PropertyValidator.FirstWrittenDate), "FirstWrittenDate" + PrintTestValue(q.FirstWrittenDate));
                        }
                        break;

                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal:
                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm:
                        {//scope control
                            var q = GetNewQuickQuote();
                            q.LobType = lobType;
                            q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
                            var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
                            q.Locations.Add(l);

                            // test invalid date
                            q.FirstWrittenDate = "a";
                            var Validations = PropertyValidator.ValidateHOMLocation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                            Assert.IsFalse(Validations.ListHasValidationId(PropertyValidator.FirstWrittenDate), "FirstWrittenDate" + PrintTestValue(q.FirstWrittenDate));

                            // test invalid date - default
                            q.FirstWrittenDate = IFM.Common.InputValidation.InputHelpers.DiamondDefaultDate_string;
                            Validations = PropertyValidator.ValidateHOMLocation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                            Assert.IsFalse(Validations.ListHasValidationId(PropertyValidator.FirstWrittenDate), "FirstWrittenDate" + PrintTestValue(q.FirstWrittenDate));

                            // test invalid date - mUst be than  effective date or in past - unless effective date isn't set then must be today or in past
                            q.FirstWrittenDate = DateTime.Now.AddDays(1).ToShortDateString();
                            Validations = PropertyValidator.ValidateHOMLocation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                            Assert.IsFalse(Validations.ListHasValidationId(PropertyValidator.FirstWrittenDate), "FirstWrittenDate" + PrintTestValue(q.FirstWrittenDate));

                            // test invalid  - Can not be today or in future
                            q.FirstWrittenDate = DateTime.Now.AddDays(-1).ToShortDateString();
                            Validations = PropertyValidator.ValidateHOMLocation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                            Assert.IsFalse(Validations.ListHasValidationId(PropertyValidator.FirstWrittenDate), "FirstWrittenDate" + PrintTestValue(q.FirstWrittenDate));
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}