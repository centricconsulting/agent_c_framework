using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFM.PrimativeExtensions;
using System.Collections.Generic;

namespace VrTests.IFM_Primative
{
    [TestClass]
    public class TestExtensions : VRQQLibBase
    {
        [TestMethod]
        public void TestIsNull()
        {
            var ob = new object();
            Assert.IsFalse(ob.IsNull(),"Expected False");

            ob = null;
            Assert.IsTrue(ob.IsNull(), "Expected True");
        }

        [TestMethod]
        public void TestReplace_NullSafe()
        {
            Assert.IsTrue("-".Replace_NullSafe("-", "") == "");
            Assert.IsTrue("-".Replace_NullSafe("+", "") == "-");

            string text = null;
            Assert.IsTrue(text.Replace_NullSafe("+", "") == "");

            Assert.IsTrue("-".Replace_NullSafe(null, "") == "-");
            Assert.IsTrue("-".Replace_NullSafe("+", null) == "-");
        }


        [TestMethod]
        public void TestStartsWith_NullSafe()
        {
            Assert.IsTrue("matt".StartsWithAny_NullSafe(new string[] { "1", "ma" }));
            Assert.IsTrue("matt".StartsWithAny_NullSafe(new string[] { "1", "Ma" }));
            Assert.IsFalse("matt".StartsWithAny_NullSafe(new string[] { "1", " ma" }));

        }


        [TestMethod]
        public void TestContainsAny_NullSafe()
        {
            Assert.IsTrue("12345".ContainsAny_NullSafe(new string[] { " ", "4" }));
            Assert.IsTrue("12345".ContainsAny_NullSafe(new string[] { "9", "2" }));
            Assert.IsFalse("12345".ContainsAny_NullSafe(new string[] { " ", "a" }));

        }



        [TestMethod]
        public void IsNumeric()
        {
            string ob = null;
            Assert.IsFalse(ob.IsNumeric(), "Expected False");
            Assert.IsFalse("".IsNumeric(), "Expected False");
            Assert.IsFalse("a".IsNumeric(), "Expected False");
            Assert.IsTrue("5".IsNumeric(), "Expected True");            
        }

        [TestMethod]
        public void IsDate()
        {
            string ob = null;
            Assert.IsFalse(ob.IsDate(), "Expected False");
            Assert.IsFalse("".IsDate(), "Expected False");
            Assert.IsFalse("a".IsDate(), "Expected False");
            Assert.IsFalse("5/32/2016".IsDate(), "Expected False");
            Assert.IsTrue("5/12/2016".IsDate(), "Expected True");
        }



        [TestMethod]
        public void TestIsNotNull()
        {
            var ob = new object();
            Assert.IsTrue(ob.IsNotNull(), "Expected True");

            ob = null;
            Assert.IsFalse(ob.IsNotNull(), "Expected False");
        }


        [TestMethod]
        public void TestIncrementByOne()
        {
            Int32 ts = 0;
            IFM.PrimativeExtensions.IFMExtensions.IncrementByOne(ref ts);
            Assert.IsTrue(ts == 1, "Expected 1");            
            
        }

        [TestMethod]
        public void TestNoneAreNullEmptyorWhitespace()
        {
            Assert.IsTrue(IFM.PrimativeExtensions.IFMExtensions.NoneAreNullEmptyorWhitespace("a", "gf", "ui", "yu"),"Expected True");
            Assert.IsFalse(IFM.PrimativeExtensions.IFMExtensions.NoneAreNullEmptyorWhitespace("a", "gf", "", "yu"), "Expected False");
        }

        [TestMethod]
        public void TestReturnEmptyIfEqualsAny()
        {
            Assert.IsTrue(IFM.PrimativeExtensions.IFMExtensions.ReturnEmptyIfEqualsAny("a", "gf", "ui", "yu") == "a", "Expected True");
            Assert.IsTrue(IFM.PrimativeExtensions.IFMExtensions.ReturnEmptyIfEqualsAny("a", "gf", "a", "yu") == "", "Expected True");
        }

        [TestMethod]
        public void TestEqualsAny()
        {
            Assert.IsTrue(IFM.PrimativeExtensions.IFMExtensions.EqualsAny("a", "gf", "a", "yu"), "Expected True");
            Assert.IsFalse(IFM.PrimativeExtensions.IFMExtensions.EqualsAny("a", "gf", "", "yu"), "Expected True");
        }

        [TestMethod]
        public void TestNotEqualsAny()
        {
            Assert.IsTrue(IFM.PrimativeExtensions.IFMExtensions.NotEqualsAny("a", "gf", "f", "yu"), "Expected True");
            Assert.IsFalse(IFM.PrimativeExtensions.IFMExtensions.NotEqualsAny("a", "gf", "a", "yu"), "Expected True");
        }


        [TestMethod]
        public void TestAddNew_Extension()
        {
            List<object> lst = null;
            IFM.PrimativeExtensions.IFMExtensions.AddNew(ref lst);

            Assert.IsTrue(lst != null, "Expected True");
            Assert.IsTrue(lst.Count == 1, "Expected True");
        }

        [TestMethod]
        public void TestCountEvenIfNull()
        {
            List<string> lst = null;
            int count = IFM.PrimativeExtensions.IFMExtensions.CountEvenIfNull(lst);
            Assert.IsTrue(count == 0, "Expected True");            
        }


        [TestMethod]
        public void HasItemAtIndex()
        {
            List<string> lst = null;

            // should be null because list is null            
            Assert.IsTrue(IFM.PrimativeExtensions.IFMExtensions.HasItemAtIndex(lst, 0) == false, "Expected null");

            // should be valid
            lst = new List<string>();
            lst.Add("ghj");
            Assert.IsTrue(IFM.PrimativeExtensions.IFMExtensions.HasItemAtIndex(lst, 0) == true, "Expected string");

            // item index out of range
            Assert.IsTrue(IFM.PrimativeExtensions.IFMExtensions.HasItemAtIndex(lst, 1) == false, "Expected null");

        }


        [TestMethod]
        public void GetItemAtIndex()
        {
            List<string> lst = null;

            // should be null because list is null
            var item = IFM.PrimativeExtensions.IFMExtensions.GetItemAtIndex(lst,0);
            Assert.IsTrue(item == null, "Expected null");

            // should be valid
            lst = new List<string>();
            lst.Add("ghj");
            item = IFM.PrimativeExtensions.IFMExtensions.GetItemAtIndex(lst, 0);
            Assert.IsTrue(item == "ghj", "Expected string");

            // item index out of range
            item = IFM.PrimativeExtensions.IFMExtensions.GetItemAtIndex(lst, 1);
            Assert.IsTrue(item == null, "Expected null");

        }


        [TestMethod]
        public void AppendItemsTo()
        {
            List<string> lst = null;

            IFM.PrimativeExtensions.IFMExtensions.AppendItemsTo(new string[]{"1","2"},ref lst);
            Assert.IsTrue(lst != null, "Expected not null");
            Assert.IsTrue(lst.Count == 2, "Expected count to be 2");

        }

        [TestMethod]
        public void AddItem()
        {
            List<string> lst = null;

            IFM.PrimativeExtensions.IFMExtensions.AddItem(ref lst, "1");
            Assert.IsTrue(lst != null, "Expected not null");
            Assert.IsTrue(lst.Count == 1, "Expected count to be 1");

        }



    }
}
