using QuickQuote.CommonObjects;
using IFM.PrimativeExtensions;

namespace IFM.VR.Validation.ObjectValidation.FarmLines
{
    public class LocationCoverageValidator
    {
        public const string CosmeticDamageExclusionOptionRequired = "{885C3153-AFFA-4D88-AAFF-0A010562CEF4}";

        public const string ValidationListID = "{24FC86C4-DD98-4464-83E3-331433EEBB75}";

        public const string QuoteIsNull = "{4268CDE4-8DA7-4A73-B476-820118580E3E}";
        public const string UnExpectedLobType = "{B948BAEF-300C-421F-9655-087BB925D268}";
        public const string NoLocation = "{0A58208B-B9F5-4F78-BDD3-66A04503F212}";

        public const string MissingAddlIns = "{888877D5-4E76-4EFD-A4EA-F018A0495FD8}";
        public const string MissingBusinessName = "{1E6E59FC-084D-451B-BC0B-BB927377CC6A}";
        public const string MissingFirtName = "{4D67262A-EFE0-4B22-B484-DC42474C681A}";
        public const string MissingLastName = "{B9E819C3-B435-459B-A066-5BFE0580FA6B}";
        public const string MissingBusinessFirstLast = "{961F3E44-E443-4C8E-92ED-33C9267345A5}";

        public const string TheftLimitRequired = "{41438A6B-773D-4DC4-A94A-80DE8F92F072}";
        public const string IncreaseLimitRequired = "{43338A6B-773D-4DA4-A94A-90DE8F92F072}";

        public const string MissingDeductibleAD = "{93163534-20DF-4C4C-ADE9-FA8924A6E930}";
        public const string CovARequired = "{3312C9EE-6359-455A-8A32-30181471E28F}";
        public const string CovCRequired = "{977C3153-AEFA-4BE8-BACB-FA010562C4FE}";

        public const string FRCandType1Dwelling = "{4BC2781F-5385-41A9-90F1-5578D04A6EA6}";

        public static Validation.ObjectValidation.ValidationItemList ValidateFARLocation(QuickQuote.CommonObjects.QuickQuoteObject quote, int locationNum, ValidationItem.ValidationType valType, int aiNum = 0)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                {
                    QuickQuoteLocation MyFarmLocation = quote.Locations[locationNum];

                    switch (valType)
                    {
                        case ValidationItem.ValidationType.quoteRate:
                        case ValidationItem.ValidationType.appRate:

                            //Updated 9/10/18 for multi state MLW - qqh, parts, SubQuoteFirst, Quote to SubQuoteFirst
                            QuickQuote.CommonMethods.QuickQuoteHelperClass qqh = new QuickQuote.CommonMethods.QuickQuoteHelperClass();
                            var parts = qqh.MultiStateQuickQuoteObjects(ref quote);
                            if (parts != null)
                            {
                                var SubQuoteFirst = parts.GetItemAtIndex(0);
                                if (SubQuoteFirst != null)
                                {


                                    // This is evaluated on FO, FL & SOM where Liability Coverage Form is NOT "None"
                                    if (SubQuoteFirst.LiabilityOptionId != "6")
                                    {
                                        if (MyFarmLocation.AdditionalInterests != null && MyFarmLocation.AdditionalInterests.Count > 0)
                                        {
                                            if (MyFarmLocation.AdditionalInterests[aiNum].TypeId == "-1" || MyFarmLocation.AdditionalInterests[aiNum].TypeId == "")
                                                valList.Add(new ObjectValidation.ValidationItem("Missing Additional Insured Type", MissingAddlIns));

                                            if (MyFarmLocation.AdditionalInterests[aiNum].Name.CommercialName1 == "" &&
                                                MyFarmLocation.AdditionalInterests[aiNum].Name.FirstName == "" &&
                                                MyFarmLocation.AdditionalInterests[aiNum].Name.LastName == "")
                                                valList.Add(new ObjectValidation.ValidationItem("Missing Last Name/First Name or Business Name for Additional Insured", MissingBusinessFirstLast));

                                            if (MyFarmLocation.AdditionalInterests[aiNum].Name.CommercialName1 == "" &&
                                                MyFarmLocation.AdditionalInterests[aiNum].Name.FirstName != "" &&
                                                MyFarmLocation.AdditionalInterests[aiNum].Name.LastName == "")
                                                valList.Add(new ObjectValidation.ValidationItem("Missing Last Name for Additional Insured", MissingLastName));

                                            if (MyFarmLocation.AdditionalInterests[aiNum].Name.CommercialName1 == "" &&
                                                MyFarmLocation.AdditionalInterests[aiNum].Name.FirstName == "" &&
                                                MyFarmLocation.AdditionalInterests[aiNum].Name.LastName != "")
                                                valList.Add(new ObjectValidation.ValidationItem("Missing First Name for Additional Insured", MissingFirtName));
                                        }

                                        if (MyFarmLocation.DeductibleLimitId == "" || MyFarmLocation.DeductibleLimitId == "0")
                                            valList.Add(new ObjectValidation.ValidationItem("Missing Deductible (A-D)", MissingDeductibleAD));

                                        // Checks to see if the Selected Form is an FO-4
                                        if (MyFarmLocation.FormTypeId == "17")
                                        {
                                            if (MyFarmLocation.C_PersonalProperty_Limit == "" || MyFarmLocation.C_PersonalProperty_Limit == "0")
                                                valList.Add(new ObjectValidation.ValidationItem("Missing Coverage C", CovCRequired));
                                        }
                                        else
                                        {
                                            // CAH B60650 Ignore Missing A on Endo.
                                            if (quote.QuoteTransactionType != QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                                            {
                                                if (MyFarmLocation.A_Dwelling_Limit == "" || MyFarmLocation.A_Dwelling_Limit == "0")
                                                    valList.Add(new ObjectValidation.ValidationItem("Missing Coverage A", CovARequired));
                                            }
                                        }

                                        if (quote.Locations[locationNum].SectionICoverages != null)
                                        {
                                            foreach (QuickQuoteSectionICoverage sc in quote.Locations[locationNum].SectionICoverages)
                                            {
                                                switch (sc.CoverageType)
                                                {
                                                    case QuickQuoteSectionICoverage.SectionICoverageType.TheftofBuildingMaterial:
                                                        if (sc.IncreasedLimit == "")
                                                            valList.Add(new ObjectValidation.ValidationItem("Missing Total Limit for Theft of Building Materials", TheftLimitRequired));

                                                        break;
                                                    case QuickQuoteSectionICoverage.SectionICoverageType.FunctionalReplacementCostLossAssessment:
                                                        //Added for bug 20405
                                                        // ACV and FRC check
                                                        int index = quote.Locations[locationNum].SectionICoverages.FindIndex(f => f.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlement);
                                                        if (index >= 0)
                                                            valList.Add(new ObjectValidation.ValidationItem("Cannot have Actual Cash Value and Functional Replacement Cost Loss Settlement on the same location. Please remove one of the coverages.", TheftLimitRequired));
                                                        // FRC and DwellingType 1 check
                                                        if (quote.Locations[locationNum].DwellingTypeId == "22")
                                                            valList.Add(new ObjectValidation.ValidationItem("Type 1 Dwellings are not allowed with Functional Replacement Cost Loss Settlement.", FRCandType1Dwelling));
                                                        break;
                                                    case QuickQuoteSectionICoverage.SectionICoverageType.Cosmetic_Damage_Exclusion:
                                                        // Cosmetic Damage Exclusion
                                                        if (IFM.VR.Common.Helpers.FARM.CosmeticDamageExHelper.IsCosmeticDamageExAvailable(quote))
                                                        {
                                                            //do nothing
                                                        }
                                                        else
                                                        {
                                                            if (!sc.ExteriorDoorWindowSurfacing && !sc.ExteriorWallSurfacing && !sc.RoofSurfacing)
                                                            {
                                                                valList.Add(new ObjectValidation.ValidationItem("Must select at least one Cosmetic Damage Exclusion Option.", CosmeticDamageExclusionOptionRequired));
                                                            }
                                                        }
                                                        
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;

                        default:
                            break;
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