using IFM.Common.InputValidation;
using QuickQuote.CommonMethods;
using System;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class Coverages_Hom_Validator
    {
        public const string ValidationListID = "{626BBF0E-9262-40B7-9F11-B8A34CD20BF7}";

        public const string QuoteIsNull = "{CD2C6602-AEE9-49E7-A94A-8B066B5ED4A9}";
        public const string UnExpectedLobType = "{35A8A578-B4CB-4BF5-B44B-F1E7D6EF0B10}";
        public const string NoLocation = "{980A55A7-F8BF-4020-B43D-54D86950C01A}";
        public const string CovC_50_CovA = "{DD9EEF69-44EA-46C4-B689-65C05BFC234A}";

        public const string CovC_HO4_ExceedsLimit = "{C8FEC48B-3273-42FB-BBC7-7692FC872354}";
        public const string ML2_CovC_Exceeds_CovA = "{2AF2BE76-3151-4CED-B9B2-9156A1AFA694}";
        public const string Location_Acerage_Exceeded = "{73B0FDD3-965A-4314-8038-B6735761D004}";
        public const string InlandRvLimits_Exceeded_CovA = "{2D1BA322-7EC1-4E46-B12C-06BEC701CE0A}";
        public const string InlandRvLimits_Exceeded_CovC = "{0B7AA4A9-6C43-47FB-9B51-8D7668339D73}";

        // Coverage A Constants
        public const string CovA_Limit = "{73DA3DEB-D954-423E-BC60-9F89CCFCF5CD}";

        public const string CovA_Increased = "{A49749F2-0AF1-4184-9B3B-C67238653560}";

        public const string CovA_Occupancy = "{3A5A56DF-2C4B-45C6-8BA2-5DC67864482B}";
        public const string CovA_HO6_ExceedsLimit = "{2864EAC0-5CA0-4D69-900E-C372E0B4AF6B}";
        public const string CovA_Min_Amount = "{95E58610-5D1E-4DFD-A6CA-64537D3FA143}";
        public const string CovA_ML2_MinAmount = "{FB1EEEB1-DA79-4EFF-B380-97C2DE1207C9}";
        public const string CovA_No_Negative = "{B1E59694-54CB-45FF-9B4D-61E6FCC51361}";
        public const string CovA_HO6_NoNegative = "{62B2C947-5F59-46D7-9AF6-1BCF1E94011A}";
        public const string CovA_ML2_NoNegative = "{FDF2C8F5-B236-4E21-B60E-345B5D216E5D}";
        public const string CovA_ML2_ExceedsLimit = "{60E56F32-CF8D-4227-A9F5-D280F58242C1}";

        // Coverage B Constants
        public const string CovB_Limit = "{C7C9E88E-AC15-4B18-86F7-6443F7E04C0D}";

        public const string CovB_Increased = "{70C05F3D-2B92-4609-B979-BF9958F7D49F}";

        public const string CovB_ExceedsLimit = "{D43F18C3-07B9-4C2D-91F7-FAF711CC51FE}";
        public const string CovB_No_Negative = "{4FCB26AA-F16D-4291-8A6A-9C496D60E007}";

        // Coverage C Constants
        public const string CovC_Limit = "{6DC3A24A-F6C2-4CF7-A63A-3580D0812F62}";

        public const string CovC_Increased = "{16404FF3-7EA0-4BC8-A919-9DF1213826A6}";

        public const string CovC_Min_Amount = "{E7B20F7C-B173-4D05-A0B3-359F3F13EB58}";

        // Coverage D Constants
        public const string CovD_Limit = "{6359547D-FD73-470C-B60B-26F74D5376BF}";

        public const string CovD_Increased = "{DD56B938-A384-4F82-872E-24658CDFB289}";
        
        public static Validation.ObjectValidation.ValidationItemList ValidateHOMCoverages(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
          
            if (quote != null)
            {
                QuickQuoteHelperClass qqHelper = new QuickQuoteHelperClass();
                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                {
                    if (quote.Locations != null && quote.Locations.Any() && quote.Locations[0] != null)
                    {
                        var MyLocation = quote.Locations[0];

                        string coverageCodeA = "70021";
                        string coverageCodeB = "70022";
                        string coverageCodeC = "70023";
                        string coverageCodeD = "70024";
                        string coverageCodeE = "70027";

                        double CoverageCodeALimit = InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_LimitIncluded) + InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_LimitIncreased);
                        double CoverageCodeBLimit = InputHelpers.TryToGetDouble(MyLocation.B_OtherStructures_LimitIncluded) + InputHelpers.TryToGetDouble(MyLocation.B_OtherStructures_LimitIncreased);
                        double CoverageCodeCLimit = InputHelpers.TryToGetDouble(MyLocation.C_PersonalProperty_LimitIncluded) + InputHelpers.TryToGetDouble(MyLocation.C_PersonalProperty_LimitIncreased);
                        //double CoverageCodeDLimit = InputHelpers.TryToGetDouble(MyLocation.C_PersonalProperty_LimitIncluded) + InputHelpers.TryToGetDouble(MyLocation.C_PersonalProperty_LimitIncreased);

                        switch (valType)
                        {
                            case ValidationItem.ValidationType.quoteRate:
                            case ValidationItem.ValidationType.appRate:
                                // Coverage A - CovAID = 70021
                                switch (MyLocation.FormTypeId)
                                {
                                    case "5":   // HO-6
                                    case "26": //added 11/15/17 case 26 for HOM Upgrade MLW  //HO 0006
                                        if (qqHelper.DecimalForString(MyLocation.A_Dwelling_LimitIncreased) < 0)
                                            valList.Add(new ObjectValidation.ValidationItem("Cannot be a negative number", CovA_Increased));
                                        break;

                                    case "6":   // ML-2
                                        if (CoverageCodeALimit < 6000)
                                            valList.Add(new ObjectValidation.ValidationItem("Coverage A limit must be at least 6,000", CovA_Limit));

                                        if (CoverageCodeALimit >= 750000)
                                            valList.Add(new ObjectValidation.ValidationItem("Coverage A limit exceeds authorized amount. Refer to Underwriting for approval", CovA_Limit, true));
                                        break;

                                    case "4":
                                    case "7":
                                    case "25": //added 11/15/17  case 25 for HOM Upgrade MLW //HO 0004
                                        break;

                                    case "22":  //added 11/15/17 for HOM Upgrade MLW // HO 0002 - find if mobile, this applies to mobile only
                                        if ((MyLocation.StructureTypeId == "2"))
                                        {
                                            //structuretypeid 2 is Mobile Home
                                            if (CoverageCodeALimit < 6000)
                                                valList.Add(new ObjectValidation.ValidationItem("Coverage A limit must be at least 6,000", CovA_Limit));

                                            if (CoverageCodeALimit >= 750000)
                                                valList.Add(new ObjectValidation.ValidationItem("Coverage A limit exceeds authorized amount. Refer to Underwriting for approval", CovA_Limit, true));
                                        }
                                        else
                                        {
                                            if ((MyLocation.OccupancyCodeId == "4" | MyLocation.OccupancyCodeId == "5") && CoverageCodeALimit > 250000)
                                                valList.Add(new ObjectValidation.ValidationItem("Coverage A limit exceeds authorized amount. Refer to Underwriting for approval", CovA_Limit, true));

                                            //if (CoverageCodeALimit < 25000)// && CoverageCodeALimit != 0.0)
                                            //    valList.Add(new ObjectValidation.ValidationItem("Coverage A limit must be at least 25,000", CovA_Limit));
                                        }
                                        break;
                                    default:    // HO-2, HO-3, Ho-3w15  //Now also HO 0003, HO 0005
                                        // Seasonal = 4   Secondary = 5
                                        //removed 1/19/2021 since there's a Diamond Rule for this (Interoperability)
                                        //if ((MyLocation.OccupancyCodeId == "4" | MyLocation.OccupancyCodeId == "5") && CoverageCodeALimit > 250000)
                                        //    valList.Add(new ObjectValidation.ValidationItem("Coverage A limit exceeds authorized amount. Refer to Underwriting for approval", CovA_Limit, true));

                                        //if (CoverageCodeALimit < 25000)// && CoverageCodeALimit != 0.0)
                                        //    valList.Add(new ObjectValidation.ValidationItem("Coverage A limit must be at least 25,000", CovA_Limit));
                                        break;
                                }

                                // Coverage B - CovBID = 70022
                                //**** TODO: incorrect validation constant, atleast in name. why isn't CovB_No_Negative being used?  - may affect how validations are associated with the UI
                                if (qqHelper.DecimalForString(MyLocation.B_OtherStructures_LimitIncreased) < 0)
                                    valList.Add(new ObjectValidation.ValidationItem("Cannot be a negative number", CovB_Increased));

                                // Coverage C - CovBID = 70023
                                switch (MyLocation.FormTypeId)
                                {
                                    case "4":   // HO-4
                                        if (CoverageCodeCLimit >= 100000)
                                            valList.Add(new ObjectValidation.ValidationItem("Coverage C limit exceeds authorized amount. Refer to Underwriting for approval", CovC_Limit, true));
                                        if (CoverageCodeCLimit < 8000)
                                            valList.Add(new ObjectValidation.ValidationItem("Coverage C limit must be at least 8,000", CovC_Limit));
                                        break;

                                    case "5":   // HO-6
                                    case "26": //HO 0006
                                        if (CoverageCodeCLimit < 8000)
                                            valList.Add(new ObjectValidation.ValidationItem("Coverage C limit must be at least 8,000", CovC_Limit));
                                        break;

                                    case "7":   // ML-4
                                        if (CoverageCodeCLimit < 4000)
                                            valList.Add(new ObjectValidation.ValidationItem("Coverage A limit must be at least 4,000", CovC_Limit));
                                        break;

                                    case "6":   // ML-2
                                        if (qqHelper.DecimalForString(MyLocation.C_PersonalProperty_LimitIncreased) < 0)
                                            valList.Add(new ObjectValidation.ValidationItem("Cannot be a negative number", CovC_Increased));
                                        if (CoverageCodeCLimit > CoverageCodeALimit) // Bug 6397 was double adding the increased limit
                                            valList.Add(new ObjectValidation.ValidationItem("Coverage C total limit is greater than Coverage A limit and must be referred to Underwriting for approval", CovC_Increased, true));
                                        break;

                                    case "22": //HO 0002 home and mobile - 11/15/17 added for HOM Upgrade MLW
                                        if (MyLocation.StructureTypeId == "2")
                                        {
                                            //removed 12/19/17 for HOM Upgrade MLW
                                            //if (decimal.Parse(MyLocation.C_PersonalProperty_LimitIncreased) < 0)
                                            //    valList.Add(new ObjectValidation.ValidationItem("Cannot be a negative number", CovC_Increased));
                                            if (CoverageCodeCLimit > CoverageCodeALimit) // Bug 6397 was double adding the increased limit
                                                valList.Add(new ObjectValidation.ValidationItem("Coverage C total limit is greater than Coverage A limit and must be referred to Underwriting for approval", CovC_Increased, true));

                                            //added 12/19/17 for HOM Upgrade MLW
                                            double prcntCovA = InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_Limit) * .5;
                                            double ttlCovC = InputHelpers.TryToGetDouble(MyLocation.C_PersonalProperty_Limit);
                                            if (ttlCovC < prcntCovA)
                                                valList.Add(new ObjectValidation.ValidationItem("Cov C limit cannot be less than 50% of Cov A limit", CovC_Increased));
                                        }
                                        else
                                        {
                                            double prcntCovA = InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_Limit) * .5;
                                            double ttlCovC = InputHelpers.TryToGetDouble(MyLocation.C_PersonalProperty_Limit);

                                            if (ttlCovC < prcntCovA)
                                                valList.Add(new ObjectValidation.ValidationItem("Cov C limit cannot be less than 50% of Cov A limit", CovC_Increased));
                                            //break;
                                        }
                                        break;

                                    case "25": //HO 0004 home and mobile - 11/15/17 added for HOM Upgrade MLW
                                        if (MyLocation.StructureTypeId == "2")
                                        {
                                            if (CoverageCodeCLimit < 4000)
                                                valList.Add(new ObjectValidation.ValidationItem("Coverage C limit must be at least 4,000", CovC_Limit));
                                        }
                                        else
                                        {
                                            if (CoverageCodeCLimit >= 100000)
                                                valList.Add(new ObjectValidation.ValidationItem("Coverage C limit exceeds authorized amount. Refer to Underwriting for approval", CovC_Limit, true));
                                            if (CoverageCodeCLimit < 8000)
                                                valList.Add(new ObjectValidation.ValidationItem("Coverage C limit must be at least 8,000", CovC_Limit));
                                        }
                                        break;


                                    default:    // HO-2, HO-3, HO3w15 //Now also HO 0003 and HO 0005
                                        double percentCovA = InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_Limit) * .5;
                                        double totalCovC = InputHelpers.TryToGetDouble(MyLocation.C_PersonalProperty_Limit);

                                        if (totalCovC < percentCovA)
                                            valList.Add(new ObjectValidation.ValidationItem("Cov C limit cannot be less than 50% of Cov A limit", CovC_Increased));
                                        break;
                                }

                                // Coverage D - CovBID = 70024
                                if (qqHelper.DecimalForString(MyLocation.D_LossOfUse_LimitIncreased) < 0)
                                { 
                                    valList.Add(new ObjectValidation.ValidationItem("Cannot be a negative number", CovD_Increased));
                                }
                                break;

                            case ValidationItem.ValidationType.issuance:

                                // 7)	When the Occupancy is Seasonal or Secondary (on the Property Page Bug 3145) and
                                // the Coverage A limit is greater than $250,000, display an error after app rate and the Route to Underwriting button'
                                // NOTE: Seasonal = 4   Secondary = 5 CovAID = 70021
                                //removed 1/19/2021 since there's a Diamond Rule for this (Interoperability)
                                //if ((MyLocation.OccupancyCodeId == "4" | MyLocation.OccupancyCodeId == "5") && CoverageCodeALimit > 250000)
                                //{
                                //    valList.Add(new ObjectValidation.ValidationItem("Cov A limit exceeds authorized amount. Refer to Underwriting for approval"
                                //        , CovA_Occupancy));
                                //}

                                //8)	When the form type is HO4 (from Underwriting Kill/Warning Questions Bug 3142)
                                //and the Coverage C limit is greater than $100,000, display an error after app rate and the Route to Underwriting button
                                // HO-4 = 4  CovCId = 70023
                                if (MyLocation.FormTypeId == "4" && CoverageCodeCLimit > 100000)
                                {
                                    valList.Add(new ObjectValidation.ValidationItem("Cov C limit exceeds authorized amount. Refer to Underwriting for approval"
                                        , CovC_HO4_ExceedsLimit));
                                }
                                //added 11/15/17 for HOM Upgrade MLW - HO 0004 = 25, StructureTypeId 2 = Mobile
                                if ((MyLocation.FormTypeId == "25" && MyLocation.StructureTypeId != "2") && CoverageCodeCLimit > 100000)
                                {
                                    valList.Add(new ObjectValidation.ValidationItem("Cov C limit exceeds authorized amount. Refer to Underwriting for approval"
                                        , CovC_HO4_ExceedsLimit));
                                }

                                //9)	When form type is HO6 (from Underwriting Kill/Warning Questions Bug 3142)
                                //and the Coverage A limit is greater than $150,000, display an error after app rate and the Route to Underwriting button
                                if ((MyLocation.FormTypeId == "5" || MyLocation.FormTypeId == "26")) //updated 11/15/17 for HOM Upgrade to inlcude 26 MLW
                                {
                                    var CovALimitFlagEnabled = IFM.VR.Common.Helpers.HOM.CovALimitIncreaseHelper.IsCovALimitIncreaseAvailable(quote);
                                    if (!CovALimitFlagEnabled && CoverageCodeALimit > 150000)
                                    {
                                        valList.Add(new ObjectValidation.ValidationItem("Cov A limit exceeds authorized amount. Refer to Underwriting for approval"
                                            , CovA_HO6_ExceedsLimit));
                                    }
                                }

                                // modified 08/16/2022 KLJ for task 75656 - removing this validation in favor of the Diamond validation
                                // *** NOTE: the unique validation constant used for this is not the correct one. We probably need to correct this as well as line 136 and where ever the UI is using these constants
                                //// 10)	When Coverage B limit is greater than 30% of Coverage A, display an error after rate and the Route to Underwriting button
                                //if (CoverageCodeBLimit > (CoverageCodeALimit * .3))
                                //{
                                //    valList.Add(new ObjectValidation.ValidationItem("Cov B limit exceeds authorized amount. Refer to Underwriting for approval"
                                //        , CovB_No_Negative));
                                //}

                                //11)	When the form type is ML2 and the Coverage C limit is greater than the Coverage A limit, display an error after rate and the Route to Underwriting button
                                if (MyLocation.FormTypeId == "6" && CoverageCodeCLimit > CoverageCodeALimit)
                                {
                                    valList.Add(new ObjectValidation.ValidationItem("A mobile home policy with Coverage C (contents coverage) greater than the mobile home unit limit must be referred to Underwriting"
                                        , ML2_CovC_Exceeds_CovA));
                                }
                                //added 11/15/17 for HOM Upgrade MLW - HO 0002 = 22, StructureTypeId 2 = Mobile
                                if ((MyLocation.FormTypeId == "22" && MyLocation.StructureTypeId == "2") && CoverageCodeCLimit > CoverageCodeALimit)
                                {
                                    valList.Add(new ObjectValidation.ValidationItem("A mobile home policy with Coverage C (contents coverage) greater than the mobile home unit limit must be referred to Underwriting"
                                        , ML2_CovC_Exceeds_CovA));
                                }

                                //12)	When location acreage is greater than 5 (from Optional Coverages Bug 3148), display an error after rate and the Route to Underwriting button
                                if (InputHelpers.TryToGetDouble(MyLocation.Acreage) > 5)
                                {
                                    valList.Add(new ObjectValidation.ValidationItem("Acreage exceeds authorized amount. Refer to Underwriting for approval"
                                        , Location_Acerage_Exceeded));
                                }

                                //double inlandTotalLimit = (MyLocation.InlandMarines != null) ? MyLocation.InlandMarines.Sum(i => InputHelpers.TryToGetDouble(i.IncreasedLimit)) : 0.0;
                                //double rvTotalLimit = (MyLocation.RvWatercrafts != null) ? MyLocation.RvWatercrafts.Sum(i => InputHelpers.TryToGetDouble(i.CostNew)) : 0.0;

                                ////13)	When the total limit for all Inland Marine coverages and RV/Watercraft coverages is greater than 25% of Coverage A limit
                                ////           and the form type is HO2, HO3, HO3 w/15 or ML2 (from Underwriting Kill/Warning Questions Bug 3142), display an error after rate and the Route to Underwriting button
                                ////if (inlandTotalLimit + rvTotalLimit > (CoverageCodeALimit * .25))
                                ////{
                                ////    string[] forms = new string[] { "1", "2", "3", "6", "22", "23", "24" }; //updated 11/15/17 to include new form types for HOM Upgrade MLW
                                ////    if (forms.Contains(MyLocation.FormTypeId))
                                ////    {
                                ////        valList.Add(new ObjectValidation.ValidationItem("Total Inland Marine and RV/Watercraft property damage coverage limit exceeds authorized amount. Please refer to Underwriting"
                                ////        , InlandRvLimits_Exceeded_CovA));
                                ////    }
                                ////}

                                ////14)	When the total limit for all Inland Marine coverages and RV/Watercraft coverages is greater than 25% of Coverage C limit
                                ////           when the form type is HO4, HO6 or ML4 (from Underwriting Kill/Underwriting Questions Bug 3142), display an error after rate and the Route to Underwriting button
                                //if (inlandTotalLimit + rvTotalLimit > (CoverageCodeCLimit * .25))
                                //{
                                //    string[] forms = new string[] { "4", "5", "7", "25", "26" }; //updated 11/15/17 to include 25 and 26 for HOM Upgrade MLW
                                //    if (forms.Contains(MyLocation.FormTypeId))
                                //    {
                                //        valList.Add(new ObjectValidation.ValidationItem("Total Inland Marine and RV/Watercraft property damage coverage limit exceeds authorized amount. Please refer to Underwriting"
                                //        , InlandRvLimits_Exceeded_CovC));
                                //    }
                                //}

                                break;

                            default:
                                break;
                        }
                    }
                    else
                    {
                        valList.Add(new ObjectValidation.ValidationItem("No Location", NoLocation));
                    }
                }
                else
                {
                    valList.Add(new ObjectValidation.ValidationItem("Invalid LOB type", UnExpectedLobType));
                }
            }
            else
            {
                valList.Add(new ObjectValidation.ValidationItem("Quote is null", QuoteIsNull));
            }
            return valList;
        }
    }
}