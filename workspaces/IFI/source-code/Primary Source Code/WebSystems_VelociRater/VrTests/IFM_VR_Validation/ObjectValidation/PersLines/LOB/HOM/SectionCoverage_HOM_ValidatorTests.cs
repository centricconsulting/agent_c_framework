using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM;
using IFM.VR.Common.Helpers.HOM;
using IFM.PrimativeExtensions;
using QuickQuote.CommonMethods;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.HOM
{
    /// <summary>
    /// Summary description for SectionCoverage_HOM_ValidatorTests
    /// </summary>
    [TestClass]
    public class SectionCoverage_HOM_ValidatorTests : VRQQLibBase
    {
        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void OptionalCoverages_Hom_BusinessPropertyIncreasedLimits_Tests()
        {
            string[] forms = new string[] { "1", "2", "3", "4", "5", "6", "7" };

            var valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
            foreach (var formType in forms)
            {
                var q = GetNewQuickQuote();
                q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
                q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
                var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
                q.Locations.Add(l);

                l.FormTypeId = formType;
                l.Address.County = "Clinton"; // required
                l.YearBuilt = "1980"; // you will want to change this to test functional replacement costs coverage


                //CoverageName = "Business Property Increased Limits (HO-312)"
                var _sc = new QuickQuote.CommonObjects.QuickQuoteSectionICoverage();
                _sc.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BusinessPropertyIncreased;
                var sc = new SectionCoverage(_sc);
                var vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                if (formType.EqualsAny("6","7"))
                {
                    // Not Valid for these formtypes
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));
                }
                else
                {
                    // is a valid form type
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));

                    // incldued limit is required and is currently empty
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

                    // 2500 should be the only valid Included Limit
                    sc.IncludedLimit = "2500";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

                    // 2000 should be invalid
                    sc.IncludedLimit = "2000";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));
                }
            }
        }

        [TestMethod]
        public void OptionalCoverages_Hom_FireArms_Tests()
        {
            string[] forms = new string[] { "1", "2", "3", "4", "5", "6", "7" };

            var valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
            foreach (var formType in forms)
            {
                var q = GetNewQuickQuote();
                q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
                q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
                var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
                q.Locations.Add(l);

                l.FormTypeId = formType;
                l.Address.County = "Clinton"; // required
                l.YearBuilt = "1980"; // you will want to change this to test functional replacement costs coverage


                //CoverageName = "Firearms (HO-65 / HO-221)"
                var _sc = new QuickQuote.CommonObjects.QuickQuoteSectionICoverage();
                _sc.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Firearms;
                var sc = new SectionCoverage(_sc);
                var vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);

                // is a valid form type
                Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));

                // incldued limit is required and is currently empty
                Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

                if (formType.EqualsAny("6","7"))
                {
                    // 500 should be the only valid Included Limit
                    sc.IncludedLimit = "500";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

                    // 2000 should be invalid
                    sc.IncludedLimit = "2000";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));
                }
                else
                {                                        
                    // 2000 should be the only valid Included Limit
                    sc.IncludedLimit = "2000";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

                    // 1000 should be invalid
                    sc.IncludedLimit = "1000";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));
                }
            }
        }

        [TestMethod]
        public void OptionalCoverages_Hom_Jewelry_Tests()
        {
            string[] forms = new string[] { "1", "2", "3", "4", "5", "6", "7" };

            var valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
            foreach (var formType in forms)
            {
                var q = GetNewQuickQuote();
                q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
                q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
                var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
                q.Locations.Add(l);

                l.FormTypeId = formType;
                l.Address.County = "Clinton"; // required
                l.YearBuilt = "1980"; // you will want to change this to test functional replacement costs coverage


                //CoverageName = "Jewelry, Watches & Furs (HO-61)"
                var _sc = new QuickQuote.CommonObjects.QuickQuoteSectionICoverage();
                _sc.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.JewelryWatchesAndFurs;
                var sc = new SectionCoverage(_sc);
                var vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);

                // is a valid form type
                Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));

                // incldued limit is required and is currently empty
                Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

                if (formType.EqualsAny("6","7"))
                {   
                    // 500 should be the only valid Included Limit
                    sc.IncludedLimit = "500";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

                    // 2000 should be invalid
                    sc.IncludedLimit = "2000";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));
                }
                else
                {
                    // 1000 should be the only valid Included Limit
                    sc.IncludedLimit = "1000";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

                    // 2000 should be invalid
                    sc.IncludedLimit = "2000";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));
                }
            }
        }
        
        [TestMethod]
        public void OptionalCoverages_Hom_Money_Tests()
        {
            string[] forms = new string[] { "1", "2", "3", "4", "5", "6", "7" };

            var valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
            foreach (var formType in forms)
            {
                var q = GetNewQuickQuote();
                q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
                q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
                var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
                q.Locations.Add(l);

                l.FormTypeId = formType;
                l.Address.County = "Clinton"; // required
                l.YearBuilt = "1980"; // you will want to change this to test functional replacement costs coverage


                //CoverageName = "Money (HO-65 / HO-221)"
                var _sc = new QuickQuote.CommonObjects.QuickQuoteSectionICoverage();
                _sc.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Money;
                var sc = new SectionCoverage(_sc);
                var vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);

                // is a valid form type
                Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));

                // incldued limit is required and is currently empty
                Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

                if (formType.EqualsAny("6", "7"))
                {
                    // 100 should be the only valid Included Limit
                    sc.IncludedLimit = "100";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

                    // 2000 should be invalid
                    sc.IncludedLimit = "200";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));
                }
                else
                {
                    // 200 should be the only valid Included Limit
                    sc.IncludedLimit = "200";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

                    // 2000 should be invalid
                    sc.IncludedLimit = "100";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));
                }
            }
        }
        
        [TestMethod]
        public void OptionalCoverages_Hom_Securities_Tests()
        {
        //    string[] forms = new string[] { "1", "2", "3", "4", "5", "6", "7" };

        //    var valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
        //    foreach (var formType in forms)
        //    {
        //        var q = GetNewQuickQuote();
        //        q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
        //        q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
        //        var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
        //        q.Locations.Add(l);

        //        l.FormTypeId = formType;
        //        l.Address.County = "Clinton"; // required
        //        l.YearBuilt = "1980"; // you will want to change this to test functional replacement costs coverage


        //        //CoverageName = "Securities (HO-65 / HO-221)"
        //        var _sc = new QuickQuote.CommonObjects.QuickQuoteSectionICoverage();
        //        _sc.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Securities;
        //        var sc = new SectionCoverage(_sc);
        //        var vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
        //        if (formType.EqualsAny("6", "7"))
        //        {
        //            // is a valid form type
        //            Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));

        //            // incldued limit is required and is currently empty
        //            Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

        //            // 100 should be the only valid Included Limit
        //            sc.IncludedLimit = "100";
        //            vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
        //            Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

        //            // 2000 should be invalid
        //            sc.IncludedLimit = "200";
        //            vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
        //            Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));
        //        }
        //        else
        //        {
        //            // is a valid form type
        //            Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));

        //            // incldued limit is required and is currently empty
        //            Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

        //            // 200 should be the only valid Included Limit
        //            sc.IncludedLimit = "200";
        //            vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
        //            Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

        //            // 2000 should be invalid
        //            sc.IncludedLimit = "100";
        //            vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
        //            Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));
        //        }
        //    }
        }
        
        [TestMethod]
        public void OptionalCoverages_Hom_Silverware_Tests()
        {
            string[] forms = new string[] { "1", "2", "3", "4", "5", "6", "7" };

            var valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
            foreach (var formType in forms)
            {
                var q = GetNewQuickQuote();
                q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
                q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
                var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
                q.Locations.Add(l);

                l.FormTypeId = formType;
                l.Address.County = "Clinton"; // required
                l.YearBuilt = "1980"; // you will want to change this to test functional replacement costs coverage


                //CoverageName = "Silverware, Goldware, Pewterware (HO-61)"
                var _sc = new QuickQuote.CommonObjects.QuickQuoteSectionICoverage();
                _sc.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SilverwareGoldwarePewterware;
                var sc = new SectionCoverage(_sc);
                var vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);

                // is a valid form type
                Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));

                // incldued limit is required and is currently empty
                Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

                if (formType.EqualsAny("6", "7"))
                {                       
                    // 1000 should be the only valid Included Limit
                    sc.IncludedLimit = "1000";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

                    // 2000 should be invalid
                    sc.IncludedLimit = "2000";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));
                }
                else
                {                    
                    // 2500 should be the only valid Included Limit
                    sc.IncludedLimit = "2500";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

                    // 1000 should be invalid
                    sc.IncludedLimit = "1000";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));
                }
            }
        }
        
        [TestMethod]
        public void OptionalCoverages_Hom_EquipmentBreakDown_Tests()
        {
            string[] forms = new string[] { "1", "2", "3", "4", "5", "6", "7" };

            var valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
            foreach (var formType in forms)
            {
                var q = GetNewQuickQuote();
                q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
                q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
                var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
                q.Locations.Add(l);

                l.FormTypeId = formType;
                l.Address.County = "Clinton"; // required
                l.YearBuilt = "1980"; // you will want to change this to test functional replacement costs coverage


                //CoverageName = "Equipment Breakdown Coverage (92-132)"
                var _sc = new QuickQuote.CommonObjects.QuickQuoteSectionICoverage();
                _sc.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Equipment_Breakdown_Coverage;
                var sc = new SectionCoverage(_sc);
                var vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                                
                
                if (formType.EqualsAny("1", "4","6","7"))
                {
                    // is a Invalid form type
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));
                }
                else
                {
                    // is a valid form type
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));
                }
            }
        }

        [TestMethod]
        public void OptionalCoverages_Hom_PersonalPropertyReplacement_Tests()
        {
            //    string[] forms = new string[] { "1", "2", "3", "4", "5", "6", "7" };

            //    var valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
            //    foreach (var formType in forms)
            //    {
            //        var q = GetNewQuickQuote();
            //        q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
            //        q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
            //        var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
            //        q.Locations.Add(l);

            //        l.FormTypeId = formType;
            //        l.Address.County = "Clinton"; // required
            //        l.YearBuilt = "1980"; // you will want to change this to test functional replacement costs coverage


            //        //CoverageName = "Personal Property Replacement Cost  (HO-290 / 92-195)"
            //        var _sc = new QuickQuote.CommonObjects.QuickQuoteSectionICoverage();
            //        _sc.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyReplacement;
            //        var sc = new SectionCoverage(_sc);
            //        var vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
            //        if (formType.EqualsAny("6", "7"))
            //        {
            //            // is a valid form type
            //            Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));

            //            // incldued limit is required and is currently empty
            //            Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

            //            // 100 should be the only valid Included Limit
            //            sc.IncludedLimit = "100";
            //            vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
            //            Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

            //            // 2000 should be invalid
            //            sc.IncludedLimit = "200";
            //            vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
            //            Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));
            //        }
            //        else
            //        {
            //            // is a valid form type
            //            Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));

            //            // incldued limit is required and is currently empty
            //            Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

            //            // 200 should be the only valid Included Limit
            //            sc.IncludedLimit = "200";
            //            vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
            //            Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

            //            // 2000 should be invalid
            //            sc.IncludedLimit = "100";
            //            vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
            //            Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));
            //        }
            //    }
        }

        [TestMethod]
        public void OptionalCoverages_Hom_HomeOwnersEnhancement_Tests()
        {
            string[] forms = new string[] { "1", "2", "3", "4", "5", "6", "7" };

            var valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
            foreach (var formType in forms)
            {
                var q = GetNewQuickQuote();
                q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
                q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
                var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
                q.Locations.Add(l);

                l.FormTypeId = formType;
                l.Address.County = "Clinton"; // required
                l.YearBuilt = "1980"; // you will want to change this to test functional replacement costs coverage


                //CoverageName = "Homeowner Enhancement Endorsement  (92-267)"
                var _sc = new QuickQuote.CommonObjects.QuickQuoteSectionICoverage();
                _sc.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement;
                var sc = new SectionCoverage(_sc);
                var vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);


                if (formType.EqualsAny("6", "7"))
                {
                    // is a Invalid form type
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));
                }
                else
                {
                    // is a valid form type
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));
                }
            }
        }

        [TestMethod]
        public void OptionalCoverages_Hom_BackupSewAndDrain_Tests()
        {
            string[] forms = new string[] { "1", "2", "3", "4", "5", "6", "7" };

            var valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
            foreach (var formType in forms)
            {
                var q = GetNewQuickQuote();
                q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
                q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
                var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
                q.Locations.Add(l);

                l.FormTypeId = formType;
                l.Address.County = "Clinton"; // required
                l.YearBuilt = "1980"; // you will want to change this to test functional replacement costs coverage


                //CoverageName = "Backup of Sewer or Drain (92-173)"
                var _sc = new QuickQuote.CommonObjects.QuickQuoteSectionICoverage();
                _sc.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BackupSewersAndDrains;
                var sc = new SectionCoverage(_sc);
                var vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);


                if (formType.EqualsAny("6", "7"))
                {
                    // is a Invalid form type
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));
                }
                else
                {
                    // is a valid form type
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));

                    // should always send nothing for included coverage - diamond adds it automatically
                    sc.IncludedLimit = "";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

                    // anything should be invalid
                    sc.IncludedLimit = "5000";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

                }
            }
        }

        [TestMethod]
        public void OptionalCoverages_Hom_CovAspecial_Tests()
        {
            string[] forms = new string[] { "1", "2", "3", "4", "5", "6", "7" };

            var valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
            foreach (var formType in forms)
            {
                var q = GetNewQuickQuote();
                q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
                q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
                var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
                q.Locations.Add(l);

                l.FormTypeId = formType;
                l.Address.County = "Clinton"; // required
                l.YearBuilt = "1980"; // you will want to change this to test functional replacement costs coverage


                //CoverageName = "Cov.A - Specified Additional Amount Of Insurance (29-034)"
                var _sc = new QuickQuote.CommonObjects.QuickQuoteSectionICoverage();
                _sc.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_CoverageASpecialCoverage;
                var sc = new SectionCoverage(_sc);
                var vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);


                if (formType.EqualsAny("4", "5", "6", "7"))
                {
                    // is a Invalid form type
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));
                }
                else
                {
                    // is a valid form type
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));
                }
            }
        }


        [TestMethod]
        public void OptionalCoverages_Hom_EarthQuake_Tests()
        {
            string[] forms = new string[] { "1", "2", "3", "4", "5", "6", "7" };

            var valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
            foreach (var formType in forms)
            {
                var q = GetNewQuickQuote();
                q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
                q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
                var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
                q.Locations.Add(l);

                l.FormTypeId = formType;
                l.Address.County = "Clinton"; // required
                l.YearBuilt = "1980"; // you will want to change this to test functional replacement costs coverage


                //CoverageName = "Earthquake  (HO-315B)"
                //CoverageName = "Earthquake  (ML-54)"
                var _sc = new QuickQuote.CommonObjects.QuickQuoteSectionICoverage();
                _sc.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Earthquake;
                var sc = new SectionCoverage(_sc);
                var vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);

                // deductible is Required it is currently empty
                Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.Deductible));

                sc.DeductibleId = "1";
                vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                // deductible is currently something
                Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.Deductible));

                if (formType.EqualsAny("6", "7"))
                {
                    // if not ML then 2% is not a valid value
                    string TwoPercentId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, "2%",q.LobType );
                    sc.DeductibleId = TwoPercentId;
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.Deductible));
                }
                else
                {
                    // if not ML then 2% is not a valid value
                    string TwoPercentId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, "2%", q.LobType);
                    sc.DeductibleId = TwoPercentId;
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.Deductible));
                }

            }
        }
        
        [TestMethod]
        public void OptionalCoverages_ActualCashValueLossSettlement_Tests()
        {
            string[] forms = new string[] { "1", "2", "3", "4", "5", "6", "7" };

            var valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
            foreach (var formType in forms)
            {
                var q = GetNewQuickQuote();
                q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
                q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
                var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
                q.Locations.Add(l);

                l.FormTypeId = formType;
                l.Address.County = "Clinton"; // required
                l.YearBuilt = "1980"; // you will want to change this to test functional replacement costs coverage


                //CoverageName = "Actual Cash Value Loss Settlement (HO-04 81)"
                var _sc = new QuickQuote.CommonObjects.QuickQuoteSectionICoverage();
                _sc.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlement;
                var sc = new SectionCoverage(_sc);
                var vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);


                if (formType.EqualsAny("4", "5", "6", "7"))
                {
                    // is a Invalid form type
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));
                }
                else
                {
                    // is a valid form type
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));
                }
            }
        }

        [TestMethod]
        public void OptionalCoverages_ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing_Tests()
        {
            string[] forms = new string[] { "1", "2", "3", "4", "5", "6", "7" };

            var valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
            foreach (var formType in forms)
            {
                var q = GetNewQuickQuote();
                q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
                q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
                var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
                q.Locations.Add(l);

                l.FormTypeId = formType;
                l.Address.County = "Clinton"; // required
                l.YearBuilt = "1980"; // you will want to change this to test functional replacement costs coverage


                //CoverageName = "Actual Cash Value Loss Settlement/Windstorm or Hail Losses to Roof Surfacing (HO-04 93)"
                var _sc = new QuickQuote.CommonObjects.QuickQuoteSectionICoverage();
                _sc.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing;
                var sc = new SectionCoverage(_sc);
                var vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);


                if (formType.EqualsAny("4", "6", "7"))
                {
                    // is a Invalid form type
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));
                }
                else
                {
                    // is a valid form type
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));
                }
            }
        }


        [TestMethod]
        public void OptionalCoverages_SinkholeCollapse_Tests()
        {
            string[] forms = new string[] { "1", "2", "3", "4", "5", "6", "7" };

            var valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
            foreach (var formType in forms)
            {
                var q = GetNewQuickQuote();
                q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
                q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
                var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
                q.Locations.Add(l);

                l.FormTypeId = formType;
                l.Address.County = "Clinton"; // required
                l.YearBuilt = "1980"; // you will want to change this to test functional replacement costs coverage


                //CoverageName = "Sinkhole Collapse (HO-99)"
                var _sc = new QuickQuote.CommonObjects.QuickQuoteSectionICoverage();
                _sc.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SinkholeCollapse;
                var sc = new SectionCoverage(_sc);
                var vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);


                if (formType.EqualsAny("4", "5", "6", "7"))
                {
                    // is a Invalid form type
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));
                }
                else
                {
                    // is a valid form type
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));
                }
            }
        }


        [TestMethod]
        public void OptionalCoverages_CreditCardFundTransForgeryEtc_Tests()
        {
            string[] forms = new string[] { "1", "2", "3", "4", "5", "6", "7" };

            var valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
            foreach (var formType in forms)
            {
                var q = GetNewQuickQuote();
                q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
                q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
                var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
                q.Locations.Add(l);

                l.FormTypeId = formType;
                l.Address.County = "Clinton"; // required
                l.YearBuilt = "1980"; // you will want to change this to test functional replacement costs coverage


                //CoverageName = "Credit Card, Fund Transfer Card, Forgery and Counterfeit Money Coverage (HO-53)"
                var _sc = new QuickQuote.CommonObjects.QuickQuoteSectionICoverage();
                _sc.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc;
                var sc = new SectionCoverage(_sc);
                var vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);


                if (formType.EqualsAny("6", "7"))
                {
                    // is a Invalid form type
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));
                }
                else
                {
                    // is a valid form type
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.CoverageNotValidForFormType));

                    // test lack of included limit
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

                    // test invalid included limit
                    sc.IncludedLimit = "200";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsTrue(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

                    // test invalid included limit
                    sc.IncludedLimit = "2500";
                    vals = SectionCoverageValidator.ValidateHOMSectionCoverage(q, sc, 0, valType);
                    Assert.IsFalse(vals.ListHasValidationId(SectionCoverageValidator.IncludedLimit));

                }
            }
        }



    }
}
