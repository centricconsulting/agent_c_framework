using IFM.Common.InputValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VrTests.InputValidation
{
    [TestClass]
    public class InputHelpersTest
    {
        [TestMethod]
        public void CSVtoList()
        {
            string testVal = null;

            Assert.IsTrue(InputHelpers.CSVtoList(testVal = "123").Count == 0, testVal);
            Assert.IsTrue(InputHelpers.CSVtoList(testVal = "123,234").Count == 2, testVal);
            Assert.IsTrue(InputHelpers.CSVtoList(testVal = string.Empty).Count == 0, "[EMPTY]");
            Assert.IsTrue(InputHelpers.CSVtoList(testVal = null).Count == 0, "[NULL]");
        }

        [TestMethod]
        public void EllipsisText()
        {
            string testVal = null;

            Assert.IsTrue(InputHelpers.EllipsisText(testVal = "0123456789", 10).Length == 10, testVal);
            Assert.IsTrue(InputHelpers.EllipsisText(testVal = "0123456789", 0).Length == InputHelpers.EllipsisSuffix.Length, testVal);
            Assert.IsTrue(InputHelpers.EllipsisText(testVal = "0123456789", 1).Length == 1 + InputHelpers.EllipsisSuffix.Length, testVal);
        }

        [TestMethod]
        public void TryToGetDouble()
        {
            string testVal = null;
            Assert.IsTrue(InputHelpers.TryToGetDouble(testVal = "1") == 1.0, testVal);
            Assert.IsTrue(InputHelpers.TryToGetDouble(testVal = "-1") == -1.0, testVal);
            Assert.IsTrue(InputHelpers.TryToGetDouble(testVal = "$1") == 1.0, testVal);
            Assert.IsTrue(InputHelpers.TryToGetDouble(testVal = "1,000") == 1000.0, testVal);
            Assert.IsTrue(InputHelpers.TryToGetDouble(testVal = "$1,000.00") == 1000.0, testVal);
            Assert.IsTrue(InputHelpers.TryToGetDouble(testVal = "$1,000.10") == 1000.1, testVal);
            Assert.IsTrue(InputHelpers.TryToGetDouble(testVal = string.Empty) == 0.0, "[EMPTY]");
            Assert.IsTrue(InputHelpers.TryToGetDouble(testVal = null) == 0.0, "[Null]");
        }


        [TestMethod]
        public void TryToGetInt()
        {
            string testVal = null;
            Assert.IsTrue(InputHelpers.TryToGetInt32(testVal = "1") == 1, testVal);
            Assert.IsTrue(InputHelpers.TryToGetInt32(testVal = "-1") == -1, testVal);
            Assert.IsTrue(InputHelpers.TryToGetInt32(testVal = "$1") == 1, testVal);
            Assert.IsTrue(InputHelpers.TryToGetInt32(testVal = "1,000") == 1000, testVal);
            Assert.IsTrue(InputHelpers.TryToGetInt32(testVal = "$1,000.00") == 1000, testVal);
            Assert.IsTrue(InputHelpers.TryToGetInt32(testVal = "$1,000.10") == 1000, testVal);
            Assert.IsTrue(InputHelpers.TryToGetInt32(testVal = string.Empty) == 0.0, "[EMPTY]");
            Assert.IsTrue(InputHelpers.TryToGetInt32(testVal = null) == 0.0, "[Null]");
        }

        [TestMethod]
        public void TryToGetInt64()
        {
            string testVal = null;
            Assert.IsTrue(InputHelpers.TryToGetInt64(testVal = "1") == 1, testVal);
            Assert.IsTrue(InputHelpers.TryToGetInt64(testVal = "-1") == -1, testVal);
            Assert.IsTrue(InputHelpers.TryToGetInt64(testVal = "$1") == 1, testVal);
            Assert.IsTrue(InputHelpers.TryToGetInt64(testVal = "1,000") == 1000, testVal);
            Assert.IsTrue(InputHelpers.TryToGetInt64(testVal = "$1,000.00") == 1000, testVal);
            Assert.IsTrue(InputHelpers.TryToGetInt64(testVal = "$1,000.10") == 1000, testVal);
            Assert.IsTrue(InputHelpers.TryToGetInt64(testVal = "223,372,036,854,775,808.123") == long.Parse("223372036854775808"), testVal);
            Assert.IsTrue(InputHelpers.TryToGetInt64(testVal = string.Empty) == 0.0, "[EMPTY]");
            Assert.IsTrue(InputHelpers.TryToGetInt64(testVal = null) == 0.0, "[Null]");
        }


    }
}