using IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.Common
{
    [TestClass]
    public class InsuredValidatorTests : VRQQLibBase
    {
        [TestMethod]
        public void ValidateInsured_TestNull()
        {
            int PhIndex = 0; // 0 = PolicyHolder, 1 = Policyholder2
            var Validations = InsuredValidator.ValidateInsured(PhIndex, null, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.QuoteIsNull), "Quote Null failed #1.");

            //testing NULL PH
            var q = GetNewQuickQuote();
            QuickQuote.CommonObjects.QuickQuotePolicyholder ph = null;
            q.Policyholder = ph;
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.QuoteIsNull), "Quote Null failed#2.");
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderIsNull), "PolicHolder Null failed#2.");
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderNameIsNull), "PolicHolder Name Null failed#2.");

            //Testing NULL Name
            ph = new QuickQuote.CommonObjects.QuickQuotePolicyholder();
            q.Policyholder = ph;
            ph.Name = null;
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.QuoteIsNull), "Quote Null failed#3.");
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderIsNull), "PolicHolder Null failed#3.");
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderNameIsNull), "PolicHolder Name Null failed#3.");

            ph.Name = new QuickQuote.CommonObjects.QuickQuoteName();
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.QuoteIsNull), "Quote Null failed#4.");
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderIsNull), "PolicHolder Null failed#4.");
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderNameIsNull), "PolicHolder Name Null failed#4.");
        }

        [TestMethod]
        public void ValidateInsured_PH1_TestName()
        {
            int PhIndex = 0; // 0 = PolicyHolder, 1 = Policyholder2
            var q = GetNewQuickQuote();
            var ph = (PhIndex == 0) ? q.Policyholder : q.Policyholder2;

            // Test EMPTY
            ph.Name.TypeId = "1";
            ph.Name.FirstName = "";
            ph.Name.LastName = "";
            ph.Name.SexId = "";
            ph.Name.BirthDate = "";
            var Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderFirstNameID), "Name Missing" + PrintTestValue(ph.Name.FirstName));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderLastNameID), "Name Missing" + PrintTestValue(ph.Name.FirstName));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderGenderID), "Gender/SexId Missing" + PrintTestValue(ph.Name.SexId));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderBirthDate), "Birth Date Missing" + PrintTestValue(ph.Name.BirthDate));

            // Test Comm Name
            ph.Name.TypeId = "2";
            ph.Name.CommercialName1 = "Wal-Mart";
            ph.Name.FirstName = "";
            ph.Name.LastName = "";
            ph.Name.SexId = "";
            ph.Name.BirthDate = "";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.CommercialName), "Name Missing" + PrintTestValue(ph.Name.FirstName));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderGenderID), "Gender/SexId Missing" + PrintTestValue(ph.Name.SexId));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderBirthDate), "Birth Date Missing" + PrintTestValue(ph.Name.BirthDate));
            ph.Name.CommercialName1 = "";


            // Test Empty Last Name
            ph.Name.TypeId = "1";
            ph.Name.FirstName = "Matt";
            ph.Name.LastName = "";
            ph.Name.SexId = "";
            ph.Name.BirthDate = "";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.CommAndPersNameComponentsEmpty), "Name Missing" + PrintTestValue(ph.Name.FirstName));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderLastNameID), "Last name Missing" + PrintTestValue(ph.Name.FirstName));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderGenderID), "Gender/SexId Missing" + PrintTestValue(ph.Name.SexId));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderBirthDate), "Birth Date Missing" + PrintTestValue(ph.Name.BirthDate));

            // Test BirthDate - Default Date
            ph.Name.BirthDate = IFM.Common.InputValidation.InputHelpers.DiamondDefaultDate_string;
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderBirthDate), "Birth Date Missing" + PrintTestValue(ph.Name.BirthDate));

            // all should be valid
            ph.Name.TypeId = "1";
            ph.Name.FirstName = "Matt";
            ph.Name.LastName = "Amon";
            ph.Name.SexId = "1";
            ph.Name.BirthDate = "07/01/1979";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderFirstNameID), "First Name is Valid" + PrintTestValue(ph.Name.FirstName));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderLastNameID), "Last Name is Valid" + PrintTestValue(ph.Name.LastName));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderGenderID), "Gender/SexId is Valid" + PrintTestValue(ph.Name.SexId));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderBirthDate), "Birth Date is Valid" + PrintTestValue(ph.Name.BirthDate));

            ph.Name.SexId = "a";
            ph.Name.BirthDate = "26/45/1979";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderGenderID), "Gender/SexId Not Numeric" + PrintTestValue(ph.Name.SexId));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderBirthDate), "Birth Date Not Valid Date" + PrintTestValue(ph.Name.BirthDate));

            ph.Name.BirthDate = DateTime.Now.AddDays(1).ToShortDateString();
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderBirthDate), "Birth Date is in future" + PrintTestValue(ph.Name.BirthDate));

            // test too old
            ph.Name.BirthDate = "01/01/1700";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderBirthDate), "Birth Date is to long ago" + PrintTestValue(ph.Name.BirthDate));

            // no need to crazy test SSN because crazy testing is done elsewhere
            ph.Name.TaxNumber = "";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderSSNID), "SSN Missing Should be Optional" + PrintTestValue(ph.Name.TaxNumber));

            ph.Name.TaxNumber = "131-22-1234";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderSSNID), "SSN Should be Valid" + PrintTestValue(ph.Name.TaxNumber));
        }

        [TestMethod]
        public void ValidateInsured_PH2_TestName()
        {
            int PhIndex = 1; // 0 = PolicyHolder, 1 = Policyholder2
            // Should ignore the last of PH#2 when there is no first or last name
            var q = GetNewQuickQuote();
            var ph = (PhIndex == 0) ? q.Policyholder : q.Policyholder2;

            // test EMPTY
            ph.Name.TypeId = "1";
            ph.Name.FirstName = "";
            ph.Name.LastName = "";
            ph.Name.SexId = "";
            ph.Name.BirthDate = "";
            var Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderFirstNameID), "First Name Missing" + PrintTestValue(ph.Name.FirstName));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderLastNameID), "Last Name Missing" + PrintTestValue(ph.Name.LastName));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderGenderID), "Gender/SexId Missing" + PrintTestValue(ph.Name.SexId));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderBirthDate), "Birth Date Missing" + PrintTestValue(ph.Name.BirthDate));

            // all should be valid
            ph.Name.TypeId = "1";
            ph.Name.FirstName = "Matt";
            ph.Name.LastName = "Amon";
            ph.Name.SexId = "1";
            ph.Name.BirthDate = "07/01/1979";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderFirstNameID), "First Name is Valid" + PrintTestValue(ph.Name.FirstName));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderLastNameID), "Last Name is Valid" + PrintTestValue(ph.Name.LastName));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderGenderID), "Gender/SexId is Valid" + PrintTestValue(ph.Name.SexId));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderBirthDate), "Birth Date is Valid" + PrintTestValue(ph.Name.BirthDate));

            ph.Name.SexId = "a";
            ph.Name.BirthDate = "26/45/1979";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderGenderID), "Gender/SexId Not Numeric" + PrintTestValue(ph.Name.SexId));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderBirthDate), "Birth Date Not Valid Date" + PrintTestValue(ph.Name.BirthDate));

            ph.Name.BirthDate = DateTime.Now.AddDays(1).ToShortDateString();
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderBirthDate), "Birth Date is in future" + PrintTestValue(ph.Name.BirthDate));

            // date too old
            ph.Name.BirthDate = "01/01/1700";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderBirthDate), "Birth Date is to long ago" + PrintTestValue(ph.Name.BirthDate));

            // no need to crazy test SSN because crazy testing is done elsewhere
            ph.Name.TaxNumber = "";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderSSNID), "SSN Missing Should be Optional" + PrintTestValue(ph.Name.TaxNumber));

            ph.Name.TaxNumber = "132-22-1234";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderSSNID), "SSN Should be Valid" + PrintTestValue(ph.Name.TaxNumber));
        }

        [TestMethod]
        public void ValidateInsured_PH1_Address()
        {
            int PhIndex = 0; // 0 = PolicyHolder, 1 = Policyholder2
            var q = GetNewQuickQuote();
            var ph = (PhIndex == 0) ? q.Policyholder : q.Policyholder2;

            ph.Address = null;
            var Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderAddressIsEmpty), "Address Object Null" + PrintTestValue(ph.Address));

            ph.Address = new QuickQuote.CommonObjects.QuickQuoteAddress();
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderAddressIsEmpty), "Address Object Null" + PrintTestValue(ph.Address));

            // testing EMPTY
            var a = ph.Address;
            a.HouseNum = "";
            a.StreetName = "";
            a.POBox = "";
            a.City = "";
            a.StateId = "";
            a.Zip = "";
            a.County = "";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderAddressIsEmpty), "Address Object Null" + PrintTestValue(ph.Address));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderStreetAndPoBoxEmpty), "PolicyHolderStreetAndPoBoxEmpty" + PrintTestValue(a.HouseNum));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderHouseNumberID), "PolicyHolderHouseNumberID" + PrintTestValue(a.HouseNum));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderPOBOXID), "PolicyHolderPOBOXID" + PrintTestValue(a.POBox));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderCityID), "PolicyHolderCityID" + PrintTestValue(a.City));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderStateID), "PolicyHolderStateID" + PrintTestValue(a.StateId));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderZipCodeID), "PolicyHolderZipCodeID" + PrintTestValue(a.Zip));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderCountyID), "PolicyHolderCountyID" + PrintTestValue(a.County));

            // testing street and Po box infor entered - not valid
            a.HouseNum = "123";
            a.StreetName = "street";
            a.POBox = "666";
            a.City = "";
            a.StateId = "";
            a.Zip = "";
            a.County = "";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderAddressIsEmpty), "Address Object Null" + PrintTestValue(ph.Address));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderStreetAndPoBoxEmpty), "PolicyHolderStreetAndPoBoxEmpty" + PrintTestValue(a.HouseNum));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderStreetAndPoxBoxAreSet), "PolicyHolderStreetAndPoxBoxAreSet" + PrintTestValue(a.HouseNum));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderHouseNumberID), "PolicyHolderHouseNumberID" + PrintTestValue(a.HouseNum));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderPOBOXID), "PolicyHolderPOBOXID" + PrintTestValue(a.POBox));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderCityID), "PolicyHolderCityID" + PrintTestValue(a.City));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderStateID), "PolicyHolderStateID" + PrintTestValue(a.StateId));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderZipCodeID), "PolicyHolderZipCodeID" + PrintTestValue(a.Zip));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderCountyID), "PolicyHolderCountyID" + PrintTestValue(a.County));

            a.HouseNum = "";
            a.StreetName = "street";
            a.POBox = "";
            a.City = "";
            a.StateId = "";
            a.Zip = "";
            a.County = "";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderAddressIsEmpty), "Address Object Null" + PrintTestValue(ph.Address));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderStreetAndPoBoxEmpty), "PolicyHolderStreetAndPoBoxEmpty" + PrintTestValue(a.HouseNum));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderStreetAndPoxBoxAreSet), "PolicyHolderStreetAndPoxBoxAreSet" + PrintTestValue(a.HouseNum));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderHouseNumberID), "PolicyHolderHouseNumberID" + PrintTestValue(a.HouseNum));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderPOBOXID), "PolicyHolderPOBOXID" + PrintTestValue(a.POBox));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderCityID), "PolicyHolderCityID" + PrintTestValue(a.City));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderStateID), "PolicyHolderStateID" + PrintTestValue(a.StateId));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderZipCodeID), "PolicyHolderZipCodeID" + PrintTestValue(a.Zip));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderCountyID), "PolicyHolderCountyID" + PrintTestValue(a.County));

            a.HouseNum = "abc";
            a.StreetName = "street";
            a.POBox = "";
            a.City = "city";
            a.StateId = "a";
            a.Zip = "a";
            a.County = "";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderAddressIsEmpty), "Address Object Null" + PrintTestValue(ph.Address));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderStreetAndPoBoxEmpty), "PolicyHolderStreetAndPoBoxEmpty" + PrintTestValue(a.HouseNum));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderStreetAndPoxBoxAreSet), "PolicyHolderStreetAndPoxBoxAreSet" + PrintTestValue(a.HouseNum));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderHouseNumberID), "PolicyHolderHouseNumberID" + PrintTestValue(a.HouseNum));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderPOBOXID), "PolicyHolderPOBOXID" + PrintTestValue(a.POBox));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderCityID), "PolicyHolderCityID" + PrintTestValue(a.City));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderStateID), "PolicyHolderStateID" + PrintTestValue(a.StateId));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderZipCodeID), "PolicyHolderZipCodeID" + PrintTestValue(a.Zip));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderCountyID), "PolicyHolderCountyID" + PrintTestValue(a.County));

            a.HouseNum = "123";
            a.StreetName = "fake st";
            a.POBox = "";
            a.City = "city";
            a.StateId = "16";
            a.Zip = "46041";
            a.County = "clinton";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderAddressIsEmpty), "Address Object Null" + PrintTestValue(ph.Address));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderStreetAndPoBoxEmpty), "PolicyHolderStreetAndPoBoxEmpty" + PrintTestValue(a.HouseNum));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderHouseNumberID), "PolicyHolderHouseNumberID" + PrintTestValue(a.HouseNum));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderPOBOXID), "PolicyHolderPOBOXID" + PrintTestValue(a.POBox));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderCityID), "PolicyHolderCityID" + PrintTestValue(a.City));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderStateID), "PolicyHolderStateID" + PrintTestValue(a.StateId));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderZipCodeID), "PolicyHolderZipCodeID" + PrintTestValue(a.Zip));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderCountyID), "PolicyHolderCountyID" + PrintTestValue(a.County));

            a.HouseNum = "";
            a.StreetName = "";
            a.POBox = "999";
            a.City = "city";
            a.StateId = "16";
            a.Zip = "46041";
            a.County = "clinton";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderAddressIsEmpty), "Address Object Null" + PrintTestValue(ph.Address));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderStreetAndPoBoxEmpty), "PolicyHolderStreetAndPoBoxEmpty" + PrintTestValue(a.HouseNum));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderHouseNumberID), "PolicyHolderHouseNumberID" + PrintTestValue(a.HouseNum));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderPOBOXID), "PolicyHolderPOBOXID" + PrintTestValue(a.POBox));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderCityID), "PolicyHolderCityID" + PrintTestValue(a.City));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderStateID), "PolicyHolderStateID" + PrintTestValue(a.StateId));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderZipCodeID), "PolicyHolderZipCodeID" + PrintTestValue(a.Zip));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderCountyID), "PolicyHolderCountyID" + PrintTestValue(a.County));
        }

        [TestMethod]
        public void ValidateInsured_PH1_Phone_Email()
        {
            int PhIndex = 0; // 0 = PolicyHolder, 1 = Policyholder2
            var q = GetNewQuickQuote();
            var ph = (PhIndex == 0) ? q.Policyholder : q.Policyholder2;

            ph.Phones = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuotePhone>();
            ph.Phones.Add(new QuickQuote.CommonObjects.QuickQuotePhone());
            var p = ph.Phones[0];

            // testing EMPTY
            p.Number = "";
            p.TypeId = "";
            p.Extension = "";
            var Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderPhoneNumber), "PolicyHolderPhoneNumber" + PrintTestValue(p.Number));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderPhoneType), "PolicyHolderPhoneType" + PrintTestValue(p.TypeId));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderPhoneExtension), "PolicyHolderPhoneExtension" + PrintTestValue(p.Extension));

            // testing Extension - Defaulted
            p.Number = "";
            p.TypeId = "";
            p.Extension = "0";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderPhoneExtension), "PolicyHolderPhoneExtension" + PrintTestValue(p.Extension));

            p.Number = "123";
            p.TypeId = "";
            p.Extension = "abc";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderPhoneNumber), "PolicyHolderPhoneNumber" + PrintTestValue(p.Number));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderPhoneType), "PolicyHolderPhoneType" + PrintTestValue(p.TypeId));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderPhoneExtension), "PolicyHolderPhoneExtension" + PrintTestValue(p.Extension));

            // testing all valid
            p.Number = "(555)123-5612";
            p.TypeId = "1";
            p.Extension = "456";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderPhoneNumber), "PolicyHolderPhoneNumber" + PrintTestValue(p.Number));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderPhoneType), "PolicyHolderPhoneType" + PrintTestValue(p.TypeId));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderPhoneExtension), "PolicyHolderPhoneExtension" + PrintTestValue(p.Extension));

            ph.Emails = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteEmail>();
            ph.Emails.Add(new QuickQuote.CommonObjects.QuickQuoteEmail());
            var e = ph.Emails[0];

            e.Address = "";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderEmail), "PolicyHolderEmail" + PrintTestValue(e.Address));

            // should be valid
            e.Address = "user@site.com";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderEmail), "PolicyHolderEmail" + PrintTestValue(e.Address));

            e.Address = "@site.com";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderEmail), "PolicyHolderEmail" + PrintTestValue(e.Address));

            // either a phone number or an email is required on app side only - APP SIDE
            e.Address = "";
            p.Number = "";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate);
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderEmailAndPhoneIsEmpty), "PolicyHolderEmail" + PrintTestValue(e.Address));

            // either a phone number or an email is required on app side only - QUOTE SIDE
            e.Address = "";
            p.Number = "";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderEmailAndPhoneIsEmpty), "PolicyHolderEmail" + PrintTestValue(e.Address));
        }

        [TestMethod]
        public void ValidateInsured_PH1_TestName_Entity()
        {
            int PhIndex = 0; // 0 = PolicyHolder, 1 = Policyholder2
            var q = GetNewQuickQuote();
            var ph = (PhIndex == 0) ? q.Policyholder : q.Policyholder2;

            // Test EMPTY
            ph.Name.TypeId = "1";
            ph.Name.FirstName = "";
            ph.Name.LastName = "";
            ph.Name.SexId = "";
            ph.Name.BirthDate = "";
            var Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderFirstNameID), "Name Missing" + PrintTestValue(ph.Name.FirstName));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderLastNameID), "Name Missing" + PrintTestValue(ph.Name.FirstName));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderGenderID), "Gender/SexId Missing" + PrintTestValue(ph.Name.SexId));
            Assert.IsTrue(Validations.ListHasValidationId(InsuredValidator.PolicyHolderBirthDate), "Birth Date Missing" + PrintTestValue(ph.Name.BirthDate));
            ph.Name.TypeId = "";

            // Test Comm Name
            ph.Name.TypeId = "2";
            ph.Name.CommercialName1 = "Wal-Mart";
            ph.Name.FirstName = "";
            ph.Name.LastName = "";
            ph.Name.SexId = "";
            ph.Name.BirthDate = "";
            Validations = InsuredValidator.ValidateInsured(PhIndex, q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.CommercialName), "Name Missing" + PrintTestValue(ph.Name.FirstName));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderGenderID), "Gender/SexId Missing" + PrintTestValue(ph.Name.SexId));
            Assert.IsFalse(Validations.ListHasValidationId(InsuredValidator.PolicyHolderBirthDate), "Birth Date Missing" + PrintTestValue(ph.Name.BirthDate));
            ph.Name.CommercialName1 = "";
            ph.Name.TypeId = "";
        }
    }
}