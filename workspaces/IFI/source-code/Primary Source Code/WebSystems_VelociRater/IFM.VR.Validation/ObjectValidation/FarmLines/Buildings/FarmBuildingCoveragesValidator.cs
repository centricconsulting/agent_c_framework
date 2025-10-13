using System.Linq;
using IFM.PrimativeExtensions;

namespace IFM.VR.Validation.ObjectValidation.FarmLines.Buildings
{
    public class FarmBuildingCoveragesValidator
    {
        public const string CosmeticDamageExclusionOptionRequired = "{005C3157-FFFF-4D55-BBEE-0A010562DCB1}";

        public const string ValidationListID = "{3d119a8a-afab-40b0-972e-5dc367f24779}";
        public const string quoteIsNull = "{0601c399-e7c3-4edf-83e2-374011f71a59}";
        public const string buildingIsNull = "{a990a5e2-3e64-44ed-81f0-81b521e5c241}";

        public const string additionalPerils = "{3d737b91-2b35-456b-be26-afef2c372752}";
        public const string eq_contents = "{58f58212-f2a1-4c1e-8e19-013e91f33d19}";
        public const string mineSubsidence = "{ac20cb26-814f-41c9-be4d-8c7f3c086779}";

        public const string businessIncome = "{47c96842-ec6f-4ecc-9683-206be5ca1282}";
        public const string sufficationCov = "{8801ee17-7b43-4e3b-9205-279ddebdcca1}"; // is actually a policy level coverage

        public const string lossExtensionIsNull = "{390942BA-5D91-43A6-AC15-E248FDDFF3B2}";
        public const string coInsuranceIsNull = "{73FD5822-13C0-4110-B8AE-FA9AC906FC38}";



        public static Validation.ObjectValidation.ValidationItemList ValidateFARBuildingCoverages(QuickQuote.CommonObjects.QuickQuoteObject quote, int locationNum, int buildingNum, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                if (quote.Locations != null && quote.Locations.Count > locationNum && quote.Locations[locationNum] != null && quote.Locations[locationNum].Buildings != null && quote.Locations[locationNum].Buildings.Count > buildingNum && quote.Locations[locationNum].Buildings[buildingNum] != null)
                {
                    QuickQuote.CommonObjects.QuickQuoteBuilding building = quote.Locations[locationNum].Buildings[buildingNum];

                    if (building != null)
                    {
                        if (building.OptionalCoverageEs != null)
                        {
                            // Cosmetic Damage Exclusion
                            if (IFM.VR.Common.Helpers.FARM.CosmeticDamageExHelper.IsCosmeticDamageExAvailable(quote)) {
                                        //do nothing
                            }
                            else {
                                var CosmeticCov = (from c in building.OptionalCoverageEs where c.CoverageType == QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Cosmetic_Damage_Exclusion_Coverage_E select c).FirstOrDefault();
                                if (CosmeticCov != null)
                                {
                                    if (!CosmeticCov.ExteriorDoorWindowSurfacing && !CosmeticCov.ExteriorWallSurfacing && !CosmeticCov.RoofSurfacing)
                                    {
                                        valList.Add(new ObjectValidation.ValidationItem("Must select at least one Cosmetic Damage Exclusion Option.", CosmeticDamageExclusionOptionRequired));
                                    }
                                }
                            }


                            // if farmstructure type is NOT 17 or 18 and the coverages 'Addition Perils' or 'EQ Contents' exist then something is wrong
                            //WS-3738 - Farmstructure type 37 (Outbuilding with Living Quarters) can contain EQ Contents, but not Additional Perils - lSchwieterman - 07/16/2025
                            if (!(building.FarmStructureTypeId == "17" | building.FarmStructureTypeId == "18" | building.FarmStructureTypeId == "37"))
                            {
                                if ((from c in building.OptionalCoverageEs where c.CoverageType == QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Barn_Additional_Perils select c).FirstOrDefault() != null &&
                                    !(building.FarmStructureTypeId == "17" | building.FarmStructureTypeId == "18"))
                                    valList.Add(new ValidationItem("Additional Perils coverage is invalid with specified building type.", additionalPerils));

                                if ((from c in building.OptionalCoverageEs where c.CoverageType == QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Earthquake_Contents select c).FirstOrDefault() != null)
                                    valList.Add(new ValidationItem("Earthquake Contents coverage is invalid with specified building type.", eq_contents));
                            }

                            // MINE SUBSIDENCE
                            // Updated 7/30/2020 for OHIO project
                            //Updated 10/18/18 for multi state MLW
                            //if (quote.QuickQuoteState != QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) {
                            // Updated to check location state is not IL - CAH 12/11/2018
                            switch (quote.Locations[locationNum].Address.QuickQuoteState) {
                                case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio:
                                    switch (IFM.VR.Common.Helpers.MineSubsidenceHelper.GetOhioMineSubsidenceTypeByCounty(quote.Locations[locationNum].Address.County))
                                    {
                                        case Common.Helpers.MineSubsidenceHelper.OhioMineSubsidenceType_enum.EligibleMandatory:
                                            break;
                                        case Common.Helpers.MineSubsidenceHelper.OhioMineSubsidenceType_enum.EligibleOptional:
                                            break;
                                        case Common.Helpers.MineSubsidenceHelper.OhioMineSubsidenceType_enum.Ineligible:
                                            if ((from c in building.OptionalCoverageEs where c.CoverageType == QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Farm_Mine_Subsidence select c).FirstOrDefault() != null)
                                            {
                                                valList.Add(new ValidationItem("Mine subsidence is not available for properties in this county.", mineSubsidence));
                                            }
                                            break;
                                    }
                                    break;
                                case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois:
                                    break;
                                default:
                                    if ((from c in building.OptionalCoverageEs where c.CoverageType == QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Farm_Mine_Subsidence select c).FirstOrDefault() != null
                                    && IFM.VR.Common.Helpers.MineSubsidenceHelper.LocationAllowsMineSubsidence(quote.Locations[locationNum]) == false)
                                        valList.Add(new ValidationItem("Mine subsidence is not available for properties in this county.", mineSubsidence));
                                    break;
                            }

                            // The commented IF code below was replaced with the SWITCH statement above as part of the Ohio Project MGB
                            //if (quote.Locations[locationNum].Address.State != IFM.VR.Common.Helpers.States.Abbreviations.IL.ToString()) {
                            //        //mine subsidence available in all IL counties, but not for IN
                            //        // mine subsidence is only available in specific counties
                            //        if ((from c in building.OptionalCoverageEs where c.CoverageType == QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Farm_Mine_Subsidence select c).FirstOrDefault() != null
                            //    && IFM.VR.Common.Helpers.MineSubsidenceHelper.LocationAllowsMineSubsidence(quote.Locations[locationNum]) == false)
                            //        valList.Add(new ValidationItem("Mine subsidence is not available for properties in this county.", mineSubsidence));
                            //}

                            // if you have the business income coverage it must have a limit and it  must be greater than zero
                            if (building.FarmStructureTypeId != "18" && building.FarmStructureTypeId != "17")
                            {
                                if (quote.Locations[locationNum].IncomeLosses != null)
                                {
                                    var propLossIncome = quote.Locations[locationNum].IncomeLosses.FindAll(p => p.Description.Trim() == "LOC" + (locationNum + 1) + "BLD" + (buildingNum + 1));
                                    if (propLossIncome != null && propLossIncome.Count > 0)
                                    {
                                        //Updated 5/21/18 for Bug 20412 MLW
                                        //if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(propLossIncome[0].Limit, valList, businessIncome, "Limit"))
                                        //    IFM.VR.Validation.VRGeneralValidations.Val_IsGreaterThanZeroNumber(propLossIncome[0].Limit, valList, businessIncome, "Limit");
                                        if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(propLossIncome[0].Limit, valList, businessIncome, "Limit"))
                                        {
                                            if (IFM.VR.Validation.VRGeneralValidations.Val_IsGreaterThanZeroNumber(propLossIncome[0].Limit, valList, businessIncome, "Limit"))
                                            {
                                                //Added 5/21/18 for Bug 20412 MLW
                                                double limit = 0.0;
                                                limit = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(propLossIncome[0].Limit);
                                                if (limit > 0)
                                                {
                                                    if (limit % 500 != 0)
                                                        valList.Add(new ValidationItem("Limit must be a multiple of 500.", businessIncome));
                                                }
                                            }
                                        }


                                        if (propLossIncome[0].CoinsuranceTypeId == "0")
                                            valList.Add(new ObjectValidation.ValidationItem("Missing Coinsurance", coInsuranceIsNull));
                                        if (propLossIncome[0].ExtendFarmIncomeOptionId == "0")
                                            valList.Add(new ObjectValidation.ValidationItem("Missing Period of Loss Extension", lossExtensionIsNull));
                                    }
                                }
                            }
                            else
                            {
                                var busCov = (from c in building.OptionalCoverageEs where c.CoverageType == QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.LossOfIncome_Rents select c).FirstOrDefault();
                                if (busCov != null)
                                {
                                    if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(busCov.IncreasedLimit, valList, businessIncome, "Limit"))
                                    {
                                        // make sure it is valid
                                        IFM.VR.Validation.VRGeneralValidations.Val_IsGreaterThanZeroNumber(busCov.IncreasedLimit, valList, businessIncome, "Limit");

                                        //Added 5/21/18 for Bug 20412 MLW
                                        double limit = 0.0;
                                        limit = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(busCov.IncreasedLimit);
                                        if (limit > 0)
                                        {
                                            if (limit % 500 != 0)
                                                valList.Add(new ValidationItem("Limit must be a multiple of 500.", businessIncome));
                                        }
                                    }
                                }

                            }
                        }

                        //Updated 9/10/18 for multi state MLW - qqh, parts, SubQuoteFirst, Quote to SubQuoteFirst
                        QuickQuote.CommonMethods.QuickQuoteHelperClass qqh = new QuickQuote.CommonMethods.QuickQuoteHelperClass();
                        var parts = qqh.MultiStateQuickQuoteObjects(ref quote);
                        if (parts != null)
                        {
                            var SubQuoteFirst = parts.GetItemAtIndex(0);
                            if (SubQuoteFirst != null)
                            {                           
                                // if you have the suffocation cov(this is actually a policy level coverage) it must have a limit
                                if (SubQuoteFirst.OptionalCoverages != null)
                                {
                                    var suffocationCov = (from cov in SubQuoteFirst.OptionalCoverages where cov.CoverageType == QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Suffocation_of_Livestock && cov.Description == string.Format("{0}.{1}", locationNum + 1, buildingNum + 1) select cov).FirstOrDefault();
                                    if (suffocationCov != null)
                                    {
                                        if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(suffocationCov.IncreasedLimit, valList, sufficationCov, "Limit"))
                                        {
                                            // make sure it is valid
                                            IFM.VR.Validation.VRGeneralValidations.Val_IsGreaterThanZeroNumber(suffocationCov.IncreasedLimit, valList, sufficationCov, "Limit");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    valList.Add(new ValidationItem("Building is null.", buildingIsNull));
                }
            }
            else
            {
                valList.Add(new ValidationItem("Quote is null.", quoteIsNull));
            }

            return valList;
        }
    }
}