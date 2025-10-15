using IFM.VR.Validation.ObjectValidation;
using IFM.VR.Validation.ObjectValidation.AllLines;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VrTests.IFM_VR_Validation.ObjectValidation.AllLines
{
    [TestClass]
    public class AddessValidatorTest : VRQQLibBase
    {
        [TestMethod]
        public void AddressTestEmpty()
        {
            QuickQuote.CommonObjects.QuickQuoteAddress a = null;

            ValidationItemList Validations = AddressValidator.AddressValidation(a, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);

            // test for Null address
            Assert.IsTrue(Validations.ListHasValidationId(AddressValidator.AddressIsEmpty), "Failed AddressIsEmpty");

            // test for no street or po box
            a = new QuickQuote.CommonObjects.QuickQuoteAddress();
            a.StateId = ""; // was defaulted to 16
            //rerun validator
            Validations = AddressValidator.AddressValidation(a, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(AddressValidator.StreetAndPoBoxEmpty), "Failed StreetAndPoBoxEmpty");
            Assert.IsTrue(Validations.ListHasValidationId(AddressValidator.CityID), "Failed CityID");
            Assert.IsTrue(Validations.ListHasValidationId(AddressValidator.CountyID), "Failed CountyID");
            Assert.IsTrue(Validations.ListHasValidationId(AddressValidator.StateID), "Failed StateID");
            Assert.IsTrue(Validations.ListHasValidationId(AddressValidator.ZipCodeID), "Failed ZipCodeID");
        }

        [TestMethod]
        public void TestHouseNumStreetNamePoBoxAllSet()
        {
            QuickQuote.CommonObjects.QuickQuoteAddress a = new QuickQuote.CommonObjects.QuickQuoteAddress();
            // test for streetnum/streetname/and po box all set
            a.StreetName = "fake st";
            a.HouseNum = "1234";
            a.POBox = "999";
            a.City = "";
            a.StateId = "";
            a.Zip = "";
            a.County = "";
            //rerun validator
            ValidationItemList Validations = AddressValidator.AddressValidation(a, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(AddressValidator.StreetAndPoxBoxAreSet), "Failed StreetAndPoxBoxAreSet");
            Assert.IsTrue(Validations.ListHasValidationId(AddressValidator.CityID), "Failed CityID");
            Assert.IsTrue(Validations.ListHasValidationId(AddressValidator.CountyID), "Failed CountyID");
            Assert.IsTrue(Validations.ListHasValidationId(AddressValidator.StateID), "Failed StateID");
            Assert.IsTrue(Validations.ListHasValidationId(AddressValidator.ZipCodeID), "Failed ZipCodeID");
        }

        [TestMethod]
        public void TestHouseNum()
        {
            QuickQuote.CommonObjects.QuickQuoteAddress a = new QuickQuote.CommonObjects.QuickQuoteAddress();
            //Test house number
            a.StreetName = "fake st";
            a.HouseNum = "";
            a.POBox = "";
            a.City = "city";
            a.StateId = "a";
            a.Zip = "46041";
            a.County = "county";
            //rerun validator
            var Validations = AddressValidator.AddressValidation(a, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(AddressValidator.HouseNumberID), "Failed HouseNumberID");
            Assert.IsFalse(Validations.ListHasValidationId(AddressValidator.StreetNameID), "Failed StreetNameID");
            Assert.IsFalse(Validations.ListHasValidationId(AddressValidator.CityID), "Failed CityID");
            Assert.IsFalse(Validations.ListHasValidationId(AddressValidator.CountyID), "Failed CountyID");
            Assert.IsTrue(Validations.ListHasValidationId(AddressValidator.StateID), "Failed StateID");
            Assert.IsFalse(Validations.ListHasValidationId(AddressValidator.ZipCodeID), "Failed ZipCodeID");
        }

        [TestMethod]
        public void TestHouseNumIsAlphaNumeric()
        {
            QuickQuote.CommonObjects.QuickQuoteAddress a = new QuickQuote.CommonObjects.QuickQuoteAddress();
            //test house number is numeric
            a.StreetName = "fake st";
            a.HouseNum = "abc";
            a.POBox = "";
            a.City = "city";
            a.StateId = "16";
            a.Zip = "46041";
            a.County = "county";
            //rerun validator
            var Validations = AddressValidator.AddressValidation(a, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(AddressValidator.HouseNumberID), "Failed HouseNumberID");
            Assert.IsFalse(Validations.ListHasValidationId(AddressValidator.StreetNameID), "Failed StreetNameID");
            Assert.IsFalse(Validations.ListHasValidationId(AddressValidator.CityID), "Failed CityID");
            Assert.IsFalse(Validations.ListHasValidationId(AddressValidator.CountyID), "Failed CountyID");
            Assert.IsFalse(Validations.ListHasValidationId(AddressValidator.StateID), "Failed StateID");
            Assert.IsFalse(Validations.ListHasValidationId(AddressValidator.ZipCodeID), "Failed ZipCodeID");

            a.HouseNum = "#$%^abc";
            //rerun validator
            Validations = AddressValidator.AddressValidation(a, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(AddressValidator.HouseNumberID), "Failed HouseNumberID");
        }

        [TestMethod]
        public void TestHouseZipCode()
        {
            QuickQuote.CommonObjects.QuickQuoteAddress a = new QuickQuote.CommonObjects.QuickQuoteAddress();
            //test zipcode length
            a.StreetName = "fake st";
            a.HouseNum = "123";
            a.POBox = "";
            a.City = "city";
            a.StateId = "16";
            a.Zip = "456";
            a.County = "county";
            //rerun validator
            var Validations = AddressValidator.AddressValidation(a, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(AddressValidator.HouseNumberID), "Failed HouseNumberID");
            Assert.IsFalse(Validations.ListHasValidationId(AddressValidator.StreetNameID), "Failed StreetNameID");
            Assert.IsFalse(Validations.ListHasValidationId(AddressValidator.CityID), "Failed CityID");
            Assert.IsFalse(Validations.ListHasValidationId(AddressValidator.CountyID), "Failed CountyID");
            Assert.IsFalse(Validations.ListHasValidationId(AddressValidator.StateID), "Failed StateID");
            Assert.IsTrue(Validations.ListHasValidationId(AddressValidator.ZipCodeID), "Failed ZipCodeID length test");

            a.Zip = "abc";
            //rerun validator
            Validations = AddressValidator.AddressValidation(a, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(AddressValidator.ZipCodeID), "Failed ZipCodeID numeric test");

            a.Zip = "46041";
            //rerun validator
            Validations = AddressValidator.AddressValidation(a, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(AddressValidator.ZipCodeID), "Failed ZipCodeID numeric test");

            a.Zip = "46041-1234";
            //rerun validator
            Validations = AddressValidator.AddressValidation(a, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(Validations.ListHasValidationId(AddressValidator.ZipCodeID), "Failed ZipCodeID numeric test");

            a.Zip = "460411234456";
            //rerun validator
            Validations = AddressValidator.AddressValidation(a, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(Validations.ListHasValidationId(AddressValidator.ZipCodeID), "Failed ZipCodeID numeric test");
        }

        [TestMethod]
        public void TestMustBeInIndiana()
        {
            QuickQuote.CommonObjects.QuickQuoteAddress a = new QuickQuote.CommonObjects.QuickQuoteAddress();
            //test zipcode length
            a.StreetName = "fake st";
            a.HouseNum = "123";
            a.POBox = "";
            a.City = "city";
            a.StateId = "16";
            a.Zip = "456";
            a.County = "county";
            //rerun validator
            var Validations = AddressValidator.AddressValidation(a, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, true);
            Assert.IsFalse(Validations.ListHasValidationId(AddressValidator.AddressSatetNotIndiana), "Failed Must be In Indiana");

            a.StateId = "17";
            //rerun validator
            Validations = AddressValidator.AddressValidation(a, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, true);
            Assert.IsTrue(Validations.ListHasValidationId(AddressValidator.AddressSatetNotIndiana), "Failed Must be In Indiana");
        }

        [TestMethod]
        public void TestMustNotBeAPOBOXAddress()
        {
            QuickQuote.CommonObjects.QuickQuoteAddress a = new QuickQuote.CommonObjects.QuickQuoteAddress();
            //test zipcode length
            a.StreetName = "fake st";
            a.HouseNum = "123";
            a.POBox = "";
            a.City = "city";
            a.StateId = "16";
            a.Zip = "456";
            a.County = "county";
            //rerun validator
            var Validations = AddressValidator.AddressValidation(a, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, false, true);
            Assert.IsFalse(Validations.ListHasValidationId(AddressValidator.CanNotHavePOBOX), "Failed Can Not Have Po Box");

            a.StreetName = "";
            a.HouseNum = "";
            a.POBox = "123";
            //rerun validator
            Validations = AddressValidator.AddressValidation(a, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, false, true);
            Assert.IsTrue(Validations.ListHasValidationId(AddressValidator.CanNotHavePOBOX), "Failed Can Not Have Po Box");
        }
    }
}