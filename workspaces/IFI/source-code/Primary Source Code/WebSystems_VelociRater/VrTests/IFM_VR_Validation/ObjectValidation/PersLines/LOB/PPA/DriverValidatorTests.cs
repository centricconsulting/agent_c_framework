using IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.PPA
{
    [TestClass]
    public class DriverValidatorTests : VRQQLibBase
    {
        [TestMethod]
        public void DriverValidator_Null_BreadCrums()
        {
            var Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, null, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.QuoteIsNull), "Quote Null failed.#1");

            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
            q.Drivers = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteDriver>();
            QuickQuote.CommonObjects.QuickQuoteDriver d = new QuickQuote.CommonObjects.QuickQuoteDriver();
            q.Drivers.Add(d);

            // testing not null quote and driver and breadcrums
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.QuoteIsNull), "Quote Null failed.#1");
            Assert.IsTrue(Validations.BreadCrums[0].BreadCrumIndicatorType == IFM.VR.Validation.ValidationBreadCrum.BCType.DriverIndex);
            Assert.IsTrue(Validations.BreadCrums[0].BreadCrumValue == "0");

            // testing name null
            d.Name = null;
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverNameIsNull), "Name is null");
        }

        [TestMethod]
        public void DriverValidator_Info()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal;
            q.Drivers = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteDriver>();
            QuickQuote.CommonObjects.QuickQuoteDriver d = new QuickQuote.CommonObjects.QuickQuoteDriver();
            q.Drivers.Add(d);

            // testing name DL Info for EMPTY
            d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            d.Name.FirstName = "";
            d.Name.LastName = "";
            d.Name.SexId = "";
            d.Name.BirthDate = "";
            d.Name.MaritalStatusId = "";
            d.Name.DriversLicenseNumber = ""; // optional on quote side, required on app side
            d.Name.DriversLicenseDate = "";
            d.Name.DriversLicenseStateId = "";
            d.DriverExcludeTypeId = ""; // 1 = Rated driver
            var Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverFirstname), "DriverFirstname" + PrintTestValue(d.Name.FirstName));
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverLastname), "DriverLastname" + PrintTestValue(d.Name.LastName));
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverGender), "DriverGender" + PrintTestValue(d.Name.SexId));
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverMaritalStatus), "DriverMaritalStatus" + PrintTestValue(d.Name.MaritalStatusId));
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverBirthDate), "DriverBirthDate" + PrintTestValue(d.Name.BirthDate));
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverDLNumber), "DriverDLNumber" + PrintTestValue(d.Name.DriversLicenseNumber));
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverDLDate), "DriverDLDate" + PrintTestValue(d.Name.DriversLicenseDate));
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverDLState), "DriverDLState" + PrintTestValue(d.Name.DriversLicenseStateId));
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverExcludedType), "DriverExcludedType" + PrintTestValue(d.DriverExcludeTypeId));

            // Testing Valid Inputs
            d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            d.Name.FirstName = "matt";
            d.Name.LastName = "amo";
            d.Name.SexId = "1";
            d.Name.BirthDate = "07/01/1979";
            d.Name.MaritalStatusId = "1";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverFirstname), "DriverFirstname" + PrintTestValue(d.Name.FirstName));
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverLastname), "DriverLastname" + PrintTestValue(d.Name.LastName));
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverGender), "DriverGender" + PrintTestValue(d.Name.SexId));
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverMaritalStatus), "DriverMaritalStatus" + PrintTestValue(d.Name.MaritalStatusId));
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverBirthDate), "DriverBirthDate" + PrintTestValue(d.Name.BirthDate));

            // Testing InValid Inputs
            d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            d.Name.SexId = "a";
            d.Name.BirthDate = DateTime.Now.AddDays(1).ToShortDateString();
            d.Name.MaritalStatusId = "b";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            //Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverGender), "DriverGender" + PrintTestValue(d.Name.SexId));
            //Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverMaritalStatus), "DriverMaritalStatus" + PrintTestValue(d.Name.MaritalStatusId));
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverBirthDate), "DriverBirthDate" + PrintTestValue(d.Name.BirthDate));

            // Testing InValid - default birth date
            d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            d.Name.SexId = "a";
            d.Name.BirthDate = IFM.Common.InputValidation.InputHelpers.DiamondDefaultDate_string;
            d.Name.MaritalStatusId = "b";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            //Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverGender), "DriverGender" + PrintTestValue(d.Name.SexId));
            //Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverMaritalStatus), "DriverMaritalStatus" + PrintTestValue(d.Name.MaritalStatusId));
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverBirthDate), "DriverBirthDate" + PrintTestValue(d.Name.BirthDate));
        }

        [TestMethod]
        public void DriverValidator_DriversLicenseInfo()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal;
            q.Drivers = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteDriver>();
            QuickQuote.CommonObjects.QuickQuoteDriver d = new QuickQuote.CommonObjects.QuickQuoteDriver();
            q.Drivers.Add(d);

            // testing DL Number Required on App side with Rated Driver
            d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            d.Name.DriversLicenseNumber = "1523-22-5623"; // optional on quote side, required on app side
            d.Name.DriversLicenseStateId = "16";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            var Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverDLNumber), "DriverDLNumber" + PrintTestValue(d.Name.DriversLicenseNumber));
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverExcludedType), "DriverExcludedType" + PrintTestValue(d.DriverExcludeTypeId));
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverDLState), "DriverDLState" + PrintTestValue(d.Name.DriversLicenseStateId));

            // testing DL Number Required on App side with Rated Driver with State Of Indiana
            d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            d.Name.DriversLicenseNumber = "1523-22-562"; // optional on quote side, required on app side - Must be 10 digits if in Indiana
            d.Name.DriversLicenseStateId = "16";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverDLNumber), "DriverDLNumber" + PrintTestValue(d.Name.DriversLicenseNumber));

            // testing DL Number Required on App side with Rated Driver with NOT State Of Indiana
            d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            d.Name.DriversLicenseNumber = "1523-22-562"; // optional on quote side, required on app side - Must be 10 digits if in Indiana
            d.Name.DriversLicenseStateId = "20";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverDLNumber), "DriverDLNumber" + PrintTestValue(d.Name.DriversLicenseNumber));

            d.Name.DriversLicenseNumber = "";
            d.Name.DriversLicenseStateId = "";

            // testing DL Date  Required on Quote side with Rated Driver  - Testing EMPTY
            d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            d.Name.DriversLicenseDate = "";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverDLDate), "DriverDLDate" + PrintTestValue(d.Name.DriversLicenseDate));

            // testing DL Date  on Quote side with Rated Driver  - Testing Future  - DOES NOT TEST THIS ON QUOTE SIDE
            //d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            //d.Name.DriversLicenseDate = DateTime.Now.AddDays(1).ToShortDateString();
            //d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            //Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            //Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverDLDate), "DriverDLDate" + PrintTestValue(d.Name.DriversLicenseDate));

            // testing DL Date Required on Quote side with Rated Driver  - Testing To Far in Past - DOES NOT TEST THIS ON QUOTE SIDE
            //d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            //d.Name.DriversLicenseDate = DateTime.Now.AddYears(-200).ToShortDateString();
            //d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            //Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            //Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverDLDate), "DriverDLDate" + PrintTestValue(d.Name.DriversLicenseDate));

            // testing DL Date Required on App side with Rated Driver  - Testing EMPTY
            d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            d.Name.DriversLicenseDate = "";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverDLDate), "DriverDLDate" + PrintTestValue(d.Name.DriversLicenseDate));

            // testing DL Date App side with Rated Driver  - Testing Future
            d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            d.Name.DriversLicenseDate = DateTime.Now.AddDays(1).ToShortDateString();
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverDLDate), "DriverDLDate" + PrintTestValue(d.Name.DriversLicenseDate));

            // testing DL Date App side with Rated Driver  - Testing Default Date
            d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            d.Name.DriversLicenseDate = IFM.Common.InputValidation.InputHelpers.DiamondDefaultDate_string;
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverDLDate), "DriverDLDate" + PrintTestValue(d.Name.DriversLicenseDate));

            // testing DL Date App side with Rated Driver  - Testing To Far in Past
            d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            d.Name.DriversLicenseDate = DateTime.Now.AddYears(-200).ToShortDateString();
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverDLDate), "DriverDLDate" + PrintTestValue(d.Name.DriversLicenseDate));
        }

        [TestMethod]
        public void DriverValidator_PHDuplicates()
        {
            // - MATT -Finish duplicate driver relationshipids Test
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal;
            q.Drivers = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteDriver>();
            QuickQuote.CommonObjects.QuickQuoteDriver d = new QuickQuote.CommonObjects.QuickQuoteDriver();
            q.Drivers.Add(d);
            // test duplicate PH#!
        }

        [TestMethod]
        public void DriverValidator_GoodStudent()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal;
            q.Drivers = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteDriver>();
            QuickQuote.CommonObjects.QuickQuoteDriver d = new QuickQuote.CommonObjects.QuickQuoteDriver();
            q.Drivers.Add(d);

            // Test Good Student - Valid
            d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            d.Name.BirthDate = DateTime.Now.AddYears(-24).ToShortDateString();
            d.GoodStudent = true;
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            var Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverGoodStudentAge), "DriverGoodStudentAge" + PrintTestValue(d.Name.BirthDate));

            // Test Good Student - InValid
            d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            d.Name.BirthDate = DateTime.Now.AddYears(-26).ToShortDateString();
            d.GoodStudent = true;
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverGoodStudentAge), "DriverGoodStudentAge" + PrintTestValue(d.Name.BirthDate));

            // Test Good Student - Valid - Distance
            d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            d.Name.BirthDate = DateTime.Now.AddYears(-24).ToShortDateString();
            d.GoodStudent = true;
            d.DistantStudent = true;
            d.SchoolDistance = "100";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverGoodStudentAge), "DriverGoodStudentAge" + PrintTestValue(d.Name.BirthDate));
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverDistanceToSchool), "DriverDistanceToSchool" + PrintTestValue(d.SchoolDistance));

            // Test Good Student - Invalid - Distance
            d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            d.Name.BirthDate = DateTime.Now.AddYears(-24).ToShortDateString();
            d.GoodStudent = true;
            d.DistantStudent = true;
            d.SchoolDistance = "1"; // must be >= 100 miles
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverGoodStudentAge), "DriverGoodStudentAge" + PrintTestValue(d.Name.BirthDate));
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverDistanceToSchool), "DriverDistanceToSchool" + PrintTestValue(d.SchoolDistance));

            // Test Good Student - InValid - Distance
            d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            d.Name.BirthDate = DateTime.Now.AddYears(-24).ToShortDateString();
            d.GoodStudent = true;
            d.DistantStudent = true;
            d.SchoolDistance = "a";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverGoodStudentAge), "DriverGoodStudentAge" + PrintTestValue(d.Name.BirthDate));
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverDistanceToSchool), "DriverDistanceToSchool" + PrintTestValue(d.SchoolDistance));

            // Test Good Student - No Good student items selected
            d.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            d.Name.BirthDate = DateTime.Now.AddYears(-35).ToShortDateString();
            d.GoodStudent = false;
            d.DistantStudent = false;
            d.SchoolDistance = "0";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverGoodStudentAge), "DriverGoodStudentAge" + PrintTestValue(d.Name.BirthDate));
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverDistanceToSchool), "DriverDistanceToSchool" + PrintTestValue(d.SchoolDistance));
        }

        [TestMethod]
        public void DriverValidator_DefensiveDriver()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal;
            q.Drivers = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteDriver>();
            QuickQuote.CommonObjects.QuickQuoteDriver d = new QuickQuote.CommonObjects.QuickQuoteDriver();
            q.Drivers.Add(d);

            // Test DefensiveDate - Not Required
            d.DefDriverDate = IFM.Common.InputValidation.InputHelpers.DiamondDefaultDate_string;
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            var Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverDefensiveDate), "DriverDefensiveDate" + PrintTestValue(d.DefDriverDate));

            // Test DefensiveDate - Not Required
            d.DefDriverDate = "";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverDefensiveDate), "DriverDefensiveDate" + PrintTestValue(d.DefDriverDate));

            // Test DefensiveDate - Valid
            d.DefDriverDate = DateTime.Now.ToShortDateString();
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverDefensiveDate), "DriverDefensiveDate" + PrintTestValue(d.DefDriverDate));

            // Test DefensiveDate - InValid
            d.DefDriverDate = "a";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverDefensiveDate), "DriverDefensiveDate" + PrintTestValue(d.DefDriverDate));

            // Test DefensiveDate - InValid - Default date - Not Required so now validation of defaulted date
            d.DefDriverDate = IFM.Common.InputValidation.InputHelpers.DiamondDefaultDate_string;
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverDefensiveDate), "DriverDefensiveDate" + PrintTestValue(d.DefDriverDate));
        }

        [TestMethod]
        public void DriverValidator_MatureDriver()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal;
            q.Drivers = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteDriver>();
            QuickQuote.CommonObjects.QuickQuoteDriver d = new QuickQuote.CommonObjects.QuickQuoteDriver();
            q.Drivers.Add(d);

            // Test MatureDriver Date - Not Required
            d.AccPreventionCourse = "";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            var Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverMatureDriverDate), "DriverMatureDriverDate" + PrintTestValue(d.AccPreventionCourse));

            // Test MatureDriver Date - Not Required
            d.AccPreventionCourse = IFM.Common.InputValidation.InputHelpers.DiamondDefaultDate_string;
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverMatureDriverDate), "DriverMatureDriverDate" + PrintTestValue(d.AccPreventionCourse));

            // Test MatureDriver Date - Valid
            d.AccPreventionCourse = DateTime.Now.ToShortDateString();
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverMatureDriverDate), "DriverMatureDriverDate" + PrintTestValue(d.AccPreventionCourse));

            // Test MatureDriver Date - InValid
            d.AccPreventionCourse = "a";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverMatureDriverDate), "DriverMatureDriverDate" + PrintTestValue(d.AccPreventionCourse));

            // Test MatureDriver Date - InValid - Default Date - Not Required so no validation on Default Date
            d.AccPreventionCourse = IFM.Common.InputValidation.InputHelpers.DiamondDefaultDate_string;
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverMatureDriverDate), "DriverMatureDriverDate" + PrintTestValue(d.AccPreventionCourse));
        }

        [TestMethod]
        public void DriverValidator_MotorCycle()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal;
            q.Drivers = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteDriver>();
            QuickQuote.CommonObjects.QuickQuoteDriver d = new QuickQuote.CommonObjects.QuickQuoteDriver();
            q.Drivers.Add(d);

            // Test MotorCycle Membership - missing which is fine
            d.MotorMembershipDate = IFM.Common.InputValidation.InputHelpers.DiamondDefaultDate_string;
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            var Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverMotorCycleTrainingDate), "DriverMotorCycleTrainingDate" + PrintTestValue(d.MotorMembershipDate));

            // Test MotorCycle Membership - missing which is fine
            d.MotorMembershipDate = "";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverMotorCycleTrainingDate), "DriverMotorCycleTrainingDate" + PrintTestValue(d.MotorMembershipDate));

            // Test MotorCycle Membership - valid
            d.MotorMembershipDate = DateTime.Now.ToShortDateString();
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverMotorCycleTrainingDate), "DriverMotorCycleTrainingDate" + PrintTestValue(d.MotorMembershipDate));

            // Test MotorCycle Membership - Invalid
            d.MotorMembershipDate = "a";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverMotorCycleTrainingDate), "DriverMotorCycleTrainingDate" + PrintTestValue(d.MotorMembershipDate));

            // Test MotorCycle Membership - Invalid    - Default Date  - Not Required so validation on default date
            d.MotorMembershipDate = IFM.Common.InputValidation.InputHelpers.DiamondDefaultDate_string;
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverMotorCycleTrainingDate), "DriverMotorCycleTrainingDate" + PrintTestValue(d.MotorMembershipDate));

            // Test MotorCycle Years - Missing which is fine
            d.MotorcycleYearsExperience = "";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverMotorCycleYearsOfExperience), "DriverMotorCycleYearsOfExperience" + PrintTestValue(d.MotorcycleYearsExperience));

            // Test MotorCycle Years - Valid
            d.MotorcycleYearsExperience = "5";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(DriverValidator.DriverMotorCycleYearsOfExperience), "DriverMotorCycleYearsOfExperience" + PrintTestValue(d.MotorcycleYearsExperience));

            // Test MotorCycle Years - Invalid
            d.MotorcycleYearsExperience = "a";
            d.DriverExcludeTypeId = "1"; // 1 = Rated driver
            Validations = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverValidator.ValidateDriver(0, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(DriverValidator.DriverMotorCycleYearsOfExperience), "DriverMotorCycleYearsOfExperience" + PrintTestValue(d.MotorcycleYearsExperience));
        }
    }
}