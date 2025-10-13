using IFM.Common.InputValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VrTests.InputValidation
{
    [TestClass]
    public class CommonValidationTest
    {
        [TestMethod]
        public void IsAlhaNumericlySequential()
        {
            string testVal = null;
            Assert.IsTrue(CommonValidations.IsAlhaNumericlySequential(testVal = "12345"), testVal);
            Assert.IsTrue(CommonValidations.IsAlhaNumericlySequential(testVal = "87654321"), testVal);
            Assert.IsTrue(CommonValidations.IsAlhaNumericlySequential(testVal = "efghijk"), testVal);
            Assert.IsTrue(CommonValidations.IsAlhaNumericlySequential(testVal = "qrSTUvW"), testVal);

            Assert.IsFalse(CommonValidations.IsAlhaNumericlySequential(testVal = "6769787679"), testVal);
            Assert.IsFalse(CommonValidations.IsAlhaNumericlySequential(testVal = "ghjksghjksghjk"), testVal);
            Assert.IsFalse(CommonValidations.IsAlhaNumericlySequential(testVal = string.Empty), "[EMPTY]");
            Assert.IsFalse(CommonValidations.IsAlhaNumericlySequential(testVal = null), "[NULL]");
        }

        [TestMethod]
        public void IsAlphabetOnly()
        {
            string testVal = null;
            Assert.IsTrue(CommonValidations.IsAlphabetOnly(testVal = "abc"), testVal);
            Assert.IsFalse(CommonValidations.IsAlphabetOnly(testVal = "123"), testVal);
            Assert.IsFalse(CommonValidations.IsAlphabetOnly(string.Empty), "[EMPTY]");
            Assert.IsFalse(CommonValidations.IsAlphabetOnly(null), "[NULL]");
        }

        [TestMethod]
        public void IsAlphaNumeric()
        {
            string testVal = null;
            Assert.IsTrue(CommonValidations.IsAlphaNum(testVal = "abcdef"), testVal);
            Assert.IsTrue(CommonValidations.IsAlphaNum(testVal = "abc123"), testVal);
            Assert.IsFalse(CommonValidations.IsAlphaNum(testVal = "abc%^$("), testVal);
            Assert.IsFalse(CommonValidations.IsAlphaNum(string.Empty), "[EMPTY]");
            Assert.IsFalse(CommonValidations.IsAlphaNum(null), "[NULL]");
        }

        [TestMethod]
        public void IsBoolean()
        {
            string testVal = null;
            Assert.IsTrue(CommonValidations.IsBoolean(testVal = "true"), testVal);
            Assert.IsTrue(CommonValidations.IsBoolean(testVal = "false"), testVal);
            Assert.IsTrue(CommonValidations.IsBoolean(testVal = "TRUE"), testVal);
            Assert.IsTrue(CommonValidations.IsBoolean(testVal = "FALSE"), testVal);
            Assert.IsTrue(CommonValidations.IsBoolean(testVal = "1"), testVal);
            Assert.IsTrue(CommonValidations.IsBoolean(testVal = "0"), testVal);
            Assert.IsFalse(CommonValidations.IsBoolean(testVal = "a"), testVal);
            Assert.IsFalse(CommonValidations.IsBoolean(testVal = "1/1/2014"), testVal);
            Assert.IsFalse(CommonValidations.IsBoolean(string.Empty), "[EMPTY]");
            Assert.IsFalse(CommonValidations.IsBoolean(null), "[NULL]");
        }

        [TestMethod]
        public void IsDateInRange()
        {
            Assert.IsTrue(CommonValidations.IsDateInRange("1/1/2014", "1/1/2014", "1/1/2014"), "Same Day");
            Assert.IsTrue(CommonValidations.IsDateInRange("1/1/2014", "1/1/2000", "1/1/2050"), "Test1");
            Assert.IsFalse(CommonValidations.IsDateInRange("2/1/2014", "1/1/2000", "1/1/2010"), "Test2");
            Assert.IsFalse(CommonValidations.IsDateInRange(string.Empty, string.Empty, string.Empty), "[EMPTY]");
            Assert.IsFalse(CommonValidations.IsDateInRange(null, null, null), "[NULL]");
        }

        [TestMethod]
        public void IsNonNegativeNumber()
        {
            string testVal = null;
            Assert.IsTrue(CommonValidations.IsNonNegativeNumber(testVal = "0"), testVal);
            Assert.IsTrue(CommonValidations.IsNonNegativeNumber(testVal = "1"), testVal);
            Assert.IsFalse(CommonValidations.IsNonNegativeNumber(testVal = "-1"), testVal);
            Assert.IsFalse(CommonValidations.IsNonNegativeNumber(string.Empty), "[EMPTY]");
            Assert.IsFalse(CommonValidations.IsNonNegativeNumber(null), "[NULL]");
        }

        [TestMethod]
        public void IsNonNegativeWholeNumber()
        {
            string testVal = null;
            Assert.IsTrue(CommonValidations.IsNonNegativeWholeNumber(testVal = "0"), testVal);
            Assert.IsTrue(CommonValidations.IsNonNegativeWholeNumber(testVal = "1"), testVal);
            Assert.IsFalse(CommonValidations.IsNonNegativeWholeNumber(testVal = "-1"), testVal);
            Assert.IsFalse(CommonValidations.IsNonNegativeWholeNumber(testVal = "1.1"), testVal);
            Assert.IsFalse(CommonValidations.IsNonNegativeWholeNumber(string.Empty), "[EMPTY]");
            Assert.IsFalse(CommonValidations.IsNonNegativeWholeNumber(null), "[NULL]");
        }

        [TestMethod]
        public void IsNumeberInRange()
        {
            string testVal = null;
            Assert.IsTrue(CommonValidations.IsNumberInRange(testVal = "0", 0, 0), testVal);
            Assert.IsTrue(CommonValidations.IsNumberInRange(testVal = "0", -1, 1), testVal);
            Assert.IsFalse(CommonValidations.IsNumberInRange(testVal = "5", -1, 1), testVal);
            Assert.IsFalse(CommonValidations.IsNumberInRange(testVal = string.Empty, -1, 1), "[EMPTY]");
            Assert.IsFalse(CommonValidations.IsNumberInRange(testVal = null, -1, 1), "[NULL]");
        }

        [TestMethod]
        public void IsPositiveNumber()
        {
            string testVal = null;
            Assert.IsTrue(CommonValidations.IsPositiveNumber(testVal = "1"), testVal);
            Assert.IsFalse(CommonValidations.IsPositiveNumber(testVal = "0"), testVal);
            Assert.IsFalse(CommonValidations.IsPositiveNumber(testVal = "-1"), testVal);
            Assert.IsFalse(CommonValidations.IsPositiveNumber(testVal = string.Empty), "[EMPTY]");
            Assert.IsFalse(CommonValidations.IsPositiveNumber(testVal = null), "[NULL]");
        }

        [TestMethod]
        public void IsPositiveWholeNumber()
        {
            string testVal = null;
            Assert.IsTrue(CommonValidations.IsPositiveWholeNumber(testVal = "1"), testVal);
            Assert.IsFalse(CommonValidations.IsPositiveWholeNumber(testVal = "-1"), testVal);
            Assert.IsFalse(CommonValidations.IsPositiveWholeNumber(testVal = "1.1"), testVal);
            Assert.IsFalse(CommonValidations.IsPositiveWholeNumber(testVal = string.Empty), "[EMPTY]");
            Assert.IsFalse(CommonValidations.IsPositiveWholeNumber(testVal = null), "[NULL]");
        }

        [TestMethod]
        public void IsPositiveWholeNumberOrEmptyOrNullOrWhitespace()
        {
            string testVal = null;
            Assert.IsTrue(CommonValidations.IsPositiveWholeNumberOrEmptyOrNullOrWhitespace(testVal = "1"), testVal);

            Assert.IsFalse(CommonValidations.IsPositiveWholeNumberOrEmptyOrNullOrWhitespace(testVal = "1.1"), testVal);
            Assert.IsFalse(CommonValidations.IsPositiveWholeNumberOrEmptyOrNullOrWhitespace(testVal = "-1.1"), testVal);
            Assert.IsFalse(CommonValidations.IsPositiveWholeNumberOrEmptyOrNullOrWhitespace(testVal = "-1"), testVal);

            Assert.IsTrue(CommonValidations.IsPositiveWholeNumberOrEmptyOrNullOrWhitespace(testVal = string.Empty), "[EMPTY]");
            Assert.IsTrue(CommonValidations.IsPositiveWholeNumberOrEmptyOrNullOrWhitespace(testVal = null), "[NULL]");
        }

        [TestMethod]
        public void IsRepeatedCharacters()
        {
            string testVal = null;
            Assert.IsTrue(CommonValidations.IsRepeatedCharacters(testVal = "111111"), testVal);
            Assert.IsTrue(CommonValidations.IsRepeatedCharacters(testVal = "aaaaaa"), testVal);
            Assert.IsFalse(CommonValidations.IsRepeatedCharacters(testVal = "123"), testVal);
            Assert.IsFalse(CommonValidations.IsRepeatedCharacters(testVal = " 111"), testVal);
            Assert.IsFalse(CommonValidations.IsRepeatedCharacters(testVal = string.Empty), "[EMPTY]");
            Assert.IsFalse(CommonValidations.IsRepeatedCharacters(testVal = null), "[NULL]");
        }

        [TestMethod]
        public void IsTextLenghtInRange()
        {
            Assert.IsTrue(CommonValidations.IsTextLenghtInRange("1234", 4, 4), "test1");
            Assert.IsTrue(CommonValidations.IsTextLenghtInRange("1234", 0, 4), "test2");
            Assert.IsTrue(CommonValidations.IsTextLenghtInRange("1234", 1, 4), "test3");
            Assert.IsFalse(CommonValidations.IsTextLenghtInRange("1234", 5, 10), "test4");
            Assert.IsTrue(CommonValidations.IsTextLenghtInRange(string.Empty, 0, 4), "[EMPTY1]");
            Assert.IsFalse(CommonValidations.IsTextLenghtInRange(string.Empty, 1, 4), "[EMPTY2]");
            Assert.IsTrue(CommonValidations.IsTextLenghtInRange(null, 0, 4), "[NULL1]");
            Assert.IsFalse(CommonValidations.IsTextLenghtInRange(null, 1, 4), "[NULL2]");
        }

        [TestMethod]
        public void IsValidAlphaNumeric()
        {
            string testVal = null;
            Assert.IsTrue(CommonValidations.IsValidAlphaNumeric(testVal = "123"), testVal);
            Assert.IsTrue(CommonValidations.IsValidAlphaNumeric(testVal = "abc"), testVal);
            Assert.IsTrue(CommonValidations.IsValidAlphaNumeric(testVal = "abl123"), testVal);
            Assert.IsFalse(CommonValidations.IsValidAlphaNumeric(testVal = "123&123"), testVal);
            Assert.IsFalse(CommonValidations.IsValidAlphaNumeric(testVal = "123,123"), testVal);
            Assert.IsFalse(CommonValidations.IsValidAlphaNumeric(testVal = "123 123"), testVal);
            Assert.IsFalse(CommonValidations.IsValidAlphaNumeric(testVal = string.Empty), "[EMPTY]");
            Assert.IsFalse(CommonValidations.IsValidAlphaNumeric(testVal = null), "[NULL]");
        }

        [TestMethod]
        public void IsValidEmail()
        {
            string testVal = null;
            Assert.IsTrue(CommonValidations.IsValidEmail(testVal = "aa@bb.com"), testVal);
            Assert.IsTrue(CommonValidations.IsValidEmail(testVal = "aa.bb@bb.com"), testVal);
            Assert.IsFalse(CommonValidations.IsValidEmail(testVal = "@b.com"), testVal);
            Assert.IsFalse(CommonValidations.IsValidEmail(testVal = "a@.com"), testVal);
            Assert.IsFalse(CommonValidations.IsValidEmail(testVal = "a@bcom"), testVal);
            Assert.IsFalse(CommonValidations.IsValidEmail(testVal = "a@b."), testVal);
            Assert.IsFalse(CommonValidations.IsValidEmail(testVal = string.Empty), "[EMPTY]");
            Assert.IsFalse(CommonValidations.IsValidEmail(testVal = null), "[NULL]");
        }

        [TestMethod]
        public void IsValidPhone()
        {
            string testVal = null;
            Assert.IsTrue(CommonValidations.IsValidPhone(testVal = "(765)555-5555"), testVal);
            //VR is very specific about the phone number format so most want to make sure all fail but above
            Assert.IsFalse(CommonValidations.IsValidPhone(testVal = "765-555-5555"), testVal);
            Assert.IsFalse(CommonValidations.IsValidPhone(testVal = "(765) 555-5555"), testVal);
            Assert.IsFalse(CommonValidations.IsValidPhone(testVal = "(76)555-5555"), testVal);
            Assert.IsFalse(CommonValidations.IsValidPhone(testVal = "76-555-5555"), testVal);
            Assert.IsFalse(CommonValidations.IsValidPhone(testVal = "765-55-5555"), testVal);
            Assert.IsFalse(CommonValidations.IsValidPhone(testVal = "765-555-555"), testVal);
            Assert.IsFalse(CommonValidations.IsValidPhone(testVal = string.Empty), "[EMPTY]");
            Assert.IsFalse(CommonValidations.IsValidPhone(testVal = null), "[NULL]");
        }

        [TestMethod]
        public void IsValidSSN()
        {
            string testVal = null;
            Assert.IsTrue(CommonValidations.IsValidSSN(testVal = "123001234"), testVal);
            Assert.IsTrue(CommonValidations.IsValidSSN(testVal = "123-11-1234"), testVal);

            Assert.IsFalse(CommonValidations.IsValidSSN(testVal = "123-11-35648"), testVal); // not big
            Assert.IsFalse(CommonValidations.IsValidSSN(testVal = "123-11"), testVal);
            Assert.IsFalse(CommonValidations.IsValidSSN(testVal = "000-00-0000"), testVal);
            Assert.IsFalse(CommonValidations.IsValidSSN(testVal = "000000000"), testVal);
            Assert.IsFalse(CommonValidations.IsValidSSN(testVal = string.Empty), "[EMPTY]");
            Assert.IsFalse(CommonValidations.IsValidSSN(testVal = null), "[NULL]");
        }

        [TestMethod]
        public void IsValidWebsite()
        {
            string testVal = null;
            Assert.IsTrue(CommonValidations.IsValidWebsite(testVal = "www.site.com"), testVal);
            Assert.IsTrue(CommonValidations.IsValidWebsite(testVal = "http://www.site.com"), testVal);
            //Assert.IsTrue(CommonValidations.IsValidWebsite(testVal = "site.com"), testVal);
            Assert.IsTrue(CommonValidations.IsValidWebsite(testVal = "http://www.subdomain.site.com"), testVal);
            Assert.IsTrue(CommonValidations.IsValidWebsite(testVal = "www.subdomain.site.com"), testVal);
            //Assert.IsTrue(CommonValidations.IsValidWebsite(testVal = "subdomain.site.com"), testVal);
            Assert.IsFalse(CommonValidations.IsValidWebsite(testVal = ".com"), testVal);
            Assert.IsFalse(CommonValidations.IsValidWebsite(testVal = "site"), testVal);
            Assert.IsFalse(CommonValidations.IsValidWebsite(testVal = string.Empty), "[EMPTY]");
            Assert.IsFalse(CommonValidations.IsValidWebsite(testVal = null), "[NULL]");
        }

        [TestMethod]
        public void IsValidZipCode()
        {
            string testVal = null;
            Assert.IsTrue(CommonValidations.IsValidZipCode(testVal = "46041"), testVal);
            Assert.IsTrue(CommonValidations.IsValidZipCode(testVal = "46041-1234"), testVal);

            Assert.IsFalse(CommonValidations.IsValidZipCode(testVal = "00000"), testVal);
            Assert.IsFalse(CommonValidations.IsValidZipCode(testVal = "00000-0000"), testVal);
            Assert.IsFalse(CommonValidations.IsValidZipCode(testVal = string.Empty), "[EMPTY]");
            Assert.IsFalse(CommonValidations.IsValidZipCode(testVal = null), "[NULL]");
        }

        [TestMethod]
        public void IsDefaultDiamondDate()
        {
            string testVal = null;
            Assert.IsTrue(CommonValidations.IsEmptyOrDefaultDiamond_Date(testVal = "1/1/1800"), testVal);
            Assert.IsFalse(CommonValidations.IsEmptyOrDefaultDiamond_Date(testVal = "1/2/1800"), testVal);
            Assert.IsTrue(CommonValidations.IsEmptyOrDefaultDiamond_Date(testVal = ""), testVal);
            Assert.IsFalse(CommonValidations.IsEmptyOrDefaultDiamond_Date(testVal = "4"), testVal);
            Assert.IsFalse(CommonValidations.IsEmptyOrDefaultDiamond_Date(testVal = "a"), testVal);
        }
    }
}