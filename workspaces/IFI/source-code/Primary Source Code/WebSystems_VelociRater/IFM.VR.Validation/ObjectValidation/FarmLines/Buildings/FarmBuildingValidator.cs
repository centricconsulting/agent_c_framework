using QuickQuote.CommonMethods;
using QuickQuote.CommonObjects;
using System;
using System.Diagnostics;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.FarmLines.Buildings
{
    public class FarmBuildingValidator
    {
        public const string ValidationListID = "{391c8fc6-b55c-4248-859f-315d72bf0513}";
        public const string quoteIsNull = "{ea1c0bc7-828f-4272-9847-8e6a8239ab9d}";
        public const string buildingIsNull = "{a128cc45-e0ab-4125-bf29-afc423e4166d}";

        public const string farmStructureId = "{527e2b2d-8205-405e-bda7-7bbf3e79d633}";
        public const string heatIsNotSupportedOnThisFarmStructureType = "{2672613b-bcad-49a9-af3e-bf5ea51c612a}";
        public const string limit_val = "{991a5da9-7d0b-41a3-a37e-3b9aebfbe5e2}";

        public const string buildingType = "{f2fa5e7e-39a9-4fac-953b-f7e43c870a0e}";
        public const string construction = "{2529fd2f-16ad-422c-ba25-3ae09346125f}";
        public const string deductible = "{9936eee2-3c13-4d8f-bfd5-07f53c4631e3}";
        public const string yearConstructed = "{6e07beb4-a6d7-47ce-8256-2a8512daa415}";
        public const string squareFeet = "{19eb37d4-481c-4938-840c-634d80637007}";
        public const string dimensions = "{c82101c6-ef8d-4c5f-b085-83994a14e684}";
        public const string description = "{37209760-1C87-4C3B-90FB-9D512256DF6F}";

        public const string dwellingContentsLimit_val = "{345c5de9-8b0a-63a7-f92e-3n1aekfbh4e2}"; //added 10/26/2020 (Interoperability)

        public static Validation.ObjectValidation.ValidationItemList ValidateFARBuildingProperty(QuickQuote.CommonObjects.QuickQuoteObject quote, int locationNum, int buildingNum, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                if (quote.Locations != null && quote.Locations.Count > locationNum && quote.Locations[locationNum] != null && quote.Locations[locationNum].Buildings != null && quote.Locations[locationNum].Buildings.Count > buildingNum && quote.Locations[locationNum].Buildings[buildingNum] != null)
                {
                    string[] dimensions_optionalFarmStructureTypeIds = new string[] { "18", "21", "26", "27", "28", "33", "34", "35", "36" };
                    QuickQuote.CommonObjects.QuickQuoteBuilding building = quote.Locations[locationNum].Buildings[buildingNum];

                    double limit = 0.0;
                    if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(building.E_Farm_Limit, valList, limit_val, "Limit"))
                    {
                        limit = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(building.E_Farm_Limit);
                        if (limit > 0)
                        {
                            if (limit % 500 != 0 && quote.QuoteTransactionType != QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                                valList.Add(new ValidationItem("Limit must be a multiple of 500.", limit_val));
                        }
                        else
                        {
                            valList.Add(new ValidationItem("Limit Required", limit_val));
                        }
                    }

                    if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(building.FarmStructureTypeId, valList, farmStructureId, "Building"))
                    {
                        // heat is only allowed on specific farmstructuretype ids
                        string[] heatCapableFarmStructureTypeIds = new string[] { "18", "10", "11", "13", "14", "15", "17", "20", "29", "37" };

                        if (building.HeatedBuildingSurchargeOther && heatCapableFarmStructureTypeIds.Contains(building.FarmStructureTypeId) == false)
                        {
                            valList.Add(new ValidationItem("Heat is not valid with this structure type.", heatIsNotSupportedOnThisFarmStructureType));
                        }

                        // Removing requirements for endorsements. Bug 59526
                        if (quote.QuoteTransactionType != QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                        {
                            // some types require specific Building Type based on limit provided
                            if (building.FarmStructureTypeId == "12") //Grain Bin
                            {
                                if (limit < 3000 && limit >= 1000)
                                {
                                    // types 2 and 3 are ok
                                    if (building.FarmTypeId == "1")
                                    { // error
                                        valList.Add(new ValidationItem("Invalid building type based on Structure Type and Building Limit.", buildingType));
                                    }
                                }
                                else
                                {
                                    if (limit < 1000)
                                    {
                                        // only type 3 is ok
                                        if (building.FarmTypeId != "3")
                                        { // error
                                            valList.Add(new ValidationItem("Invalid building type based on Structure Type and Building Limit.", buildingType));
                                        }
                                    }
                                    else
                                    {
                                        // above 2900 all types are ok
                                    }
                                }
                            }

                            // should have buildingtype(FarmTypeid) of (Type 2 Open = 8)
                            if (building.FarmStructureTypeId == "19" && !(building.FarmTypeId == "8" | building.FarmTypeId == "3")) //Hoop Building
                                valList.Add(new ValidationItem("Invalid building type based on Structure Type.", buildingType));

                            // should have buildingtype(FarmTypeid) of (Type 3 = 3)
                            if (building.FarmStructureTypeId == "32" && building.FarmTypeId != "3") //Green House
                                valList.Add(new ValidationItem("Invalid building type based on Structure Type.", buildingType));

                            //Added 5/21/18 for Bug 20411 MLW
                            // should have buildingtype(FarmTypeid) of (N/A = 5)
                            if (building.FarmStructureTypeId == "27" && building.FarmTypeId != "5") //Grain Dryer
                                valList.Add(new ValidationItem("Invalid building type based on Structure Type.", buildingType));

                            //Added 5/21/18 for Bug 20411 MLW
                            // should have buildingtype(FarmTypeid) of (N/A = 5)
                            if (building.FarmStructureTypeId == "34" && building.FarmTypeId != "5") //Private Power and Light Pole
                                valList.Add(new ValidationItem("Invalid building type based on Structure Type.", buildingType));

                            //Added 5/21/18 for Bug 20410 MLW
                            // should have buildingtype(FarmTypeid) of (Type 2 = 2 or Type 3 = 3) 
                            if (building.FarmStructureTypeId == "18" && !(building.FarmTypeId == "2" | building.FarmTypeId == "3")) //Farm Dwelling
                                valList.Add(new ValidationItem("Invalid building type based on Structure Type.", buildingType));

                            // Added 10/8/2020 Bug 52075 MGB
                            // If structure type is one of the following...
                            //   Farm Dwelling (18)
                            //   Farm Dwelling Contents (??)
                            //   Barn (10)
                            //   Confinement Building (11)
                            //   Implement Shed (13)
                            //   Outbuilding (14)
                            //   Pole Building (15)
                            //   Silo (16)
                            //   Hoop Building (19)
                            //   Quonset Building (20)
                            //   Grain Leg (28)
                            //   Poultry Building (29)
                            //   Crib (30)
                            //   Granary (31)
                            // ...then type cannot be N/A
                            switch (building.FarmStructureTypeId)
                            {
                                case "18":
                                case "10":
                                case "11":
                                case "13":
                                case "14":
                                case "15":
                                case "16":
                                case "19":
                                case "20":
                                case "28":
                                case "29":
                                case "30":
                                case "31":
                                    if (building.FarmTypeId == "5")    // 5 = N/A
                                    {
                                        valList.Add(new ValidationItem("Type N/A is not valid for selected structure type.", buildingType));
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField_DD(building.ConstructionId, valList, construction, "Construction");

                    if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField_DD(building.E_Farm_DeductibleLimitId, valList, deductible, "Deductible"))
                    {
                        // make sure limit is greater than the deductible
                        // Why is this disabled? Matt A - 1-4-2015 - Noticed a unit test failed so checked this out and this was commented out - not sure why that is the case but leaving it commented out for now because we are about to go live with DFR

                        //QuickQuote.CommonMethods.QuickQuoteHelperClass qqHelper = new QuickQuote.CommonMethods.QuickQuoteHelperClass();
                        //string dedVal = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.E_Farm_DeductibleLimitId, building.E_Farm_DeductibleLimitId, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm, QuickQuoteHelperClass.PersOrComm.Pers);
                        //IFM.VR.Validation.VRGeneralValidations.Val_IsNumberInRange(dedVal, valList, deductible, "Deductible", (IFM.Common.InputValidation.InputHelpers.TryToGetDouble(dedVal) + 1).ToString(), int.MaxValue.ToString());

                        { // for scope control
                            // make sure this buildings deductible match first building deductible - all must be the same 
                            // Except on Endorsements where all can be different.  CAH Task 59589
                            if (quote.QuoteTransactionType != QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                            {
                                var firstBuilding = IFM.VR.Common.Helpers.BuildingsHelper.FindFirstBuilding(quote);
                                if (building.E_Farm_DeductibleLimitId != ((firstBuilding != null) ? firstBuilding.E_Farm_DeductibleLimitId : ""))
                                    valList.Add(new ValidationItem("Invalid Deductible. Must match the first buildings deductible.", deductible));
                            }
                            
                        }
                    }

                    if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(building.YearBuilt, valList, yearConstructed, "Year Constructed"))
                    {
                        IFM.VR.Validation.VRGeneralValidations.Val_IsNumberInRange(building.YearBuilt, valList, yearConstructed, "Year Constructed", 1600.ToString(), (DateTime.Now.Year + 1).ToString());
                    }

                    // Not required when Endorsements - 01/29/2021 CAH - Bug 59434
                    if (quote.QuoteTransactionType != QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                    {
                        if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(building.SquareFeet, valList, squareFeet, "Square Feet", !(building.FarmStructureTypeId == "18"))) // required on Farm Dwelling(18) not on all others
                        {
                            IFM.VR.Validation.VRGeneralValidations.Val_IsNumberInRange(building.SquareFeet, valList, squareFeet, "Square Feet", 1.ToString(), 1000000.ToString());
                        }
                    }


                    if (dimensions_optionalFarmStructureTypeIds.Contains(building.FarmStructureTypeId) == false)
                        IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(building.Dimensions, valList, dimensions, (building.FarmStructureTypeId == "12") ? "Bushels" : "Dimensions");

                    // FarmTypeId (Building Type on the screen) is a drop down so should use the DD method. - lSchwieterman - 7/2/2025
                    if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField_DD(building.FarmTypeId, valList, buildingType, "Building Type"))
                    {
                        // Not needed.  This code is handled above around line 106. CAH 20210215
                        //if (building.FarmStructureTypeId == "27") // allows required unless structuretype is Grain Dryer=27
                        //{
                        //    if (building.FarmTypeId != "5")
                        //    {
                        //        valList.Add(new ValidationItem("Invalid Building Type", buildingType));
                        //    }
                        //}
                    }

                    IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField_DD(building.Description, valList, description, "Description");
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