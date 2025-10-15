using QuickQuote.CommonMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using IFM.PrimativeExtensions;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA
{
    public class VehicleValidator
    {
        //This is only used for PPA, so no multi state changes are needed 9/17/18 MLW
        public const string ValidationListID = "{9CECD649-A355-47AA-B856-EEDDFD883F28}";

        public const string NoVehicles = "{4C7616D6-1450-47BF-B627-FB6C582918B6}";

        public const string VehicleCostNew = "{8CE49100-4866-47A6-A6F2-E71E761D9E67}";
        public const string VehicleVIN = "{9F8F1DCB-F866-4EB0-81A6-0EDADF05517E}";
        public const string VehicleMake = "{ED6B8B5F-1A73-485B-8CA3-CE91CE8BD27C}";
        public const string VehicleModel = "{9AA76CF8-5B34-48C5-B833-F5E38FB703DE}";
        public const string VehicleYear = "{2B104958-E69D-445C-B631-59CF4EEAFFFD}";
        public const string VehicleBodyType = "{1FCF58E8-DA95-436F-84CC-C2C6D9C7E98E}";
        public const string VehicleUse = "{A57CD8E0-C21B-4039-8A04-D222E5119C46}";
        public const string VehiclePerformace = "{CA646543-A8A0-4489-B443-AB085B03E5EA}";
        public const string VehicleMotorCycleType = "{90679D24-4F9D-4262-B62C-FEB6D903A02D}";
        public const string VehicleMotorCycleHorsePower = "{6ED788B7-9C40-4B79-83F8-A827AAB345EA}";
        public const string VehicleStatedAmount = "{366327D7-BDF9-436D-8DC7-9BA14C28D6D7}";
        public const string VehiclePrimaryAssignedDriver = "{50E9F71B-F401-4A20-BC63-B6A33F18B8BE}";
        public const string VehicleDriverAssignedMoreThanOnce = "{DE302B0D-A86B-428C-99F1-0B6E94B46304}";
        public const string VehicleBothNonOwnerTypesSelected = "{481BC335-15E3-45B7-BA29-B26B1DD96F95}";
        public const string VehicleSymbols = "{560510EF-C06A-4349-9E3C-2FFE33DEA880}";
        public const string vehicleActualCashValue = "{E996179F-BE2F-4469-B1F5-03B67D5115B8}";
        public const string AnnualMileage = "{25691B59-1443-4CF2-908A-27766B6B8D43}";

        public static Validation.ObjectValidation.ValidationItemList VehicleValidation(int vehicleIndex, QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.VehicleIndex, vehicleIndex.ToString());

            if (quote != null && quote.Vehicles != null && quote.Vehicles.Count > vehicleIndex)
            {
                QuickQuote.CommonObjects.QuickQuoteVehicle vehicle = quote.Vehicles[vehicleIndex];
                if (vehicle != null)
                {
                    QuickQuote.CommonMethods.QuickQuoteHelperClass qqHelper = new QuickQuote.CommonMethods.QuickQuoteHelperClass();
                    string bodyType_MotorCycle = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Motorcycle");
                    string bodyType_MotorHome = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Motor Home");
                    string bodyType_PICKUPWCAMPER = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "PICKUP W/CAMPER");
                    string bodyType_PICKUPWOCAMPER = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "PICKUP W/O CAMPER");
                    string bodyType_RecTrailer = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Rec. Trailer");
                    string bodyType_ClassicAuto = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Classic Auto");
                    string bodyType_OtherTrailer = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Other Trailer");
                    string bodyType_AntiqueAuto = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Antique Auto");
                    string bodyType_Car = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Car");
                    string bodyType_SUV = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "SUV");
                    string bodyType_Van = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Van");

                    if (VRGeneralValidations.Val_HasRequiredField_DD(vehicle.VehicleUseTypeId, valList, VehicleUse, "Vehicle Use"))
                        VRGeneralValidations.Val_IsNonNegativeWholeNumber(vehicle.VehicleUseTypeId, valList, VehicleUse, "Vehicle Use");

                    if (vehicle.NonOwned || vehicle.NonOwnedNamed)
                    {
                        if (vehicle.NonOwned && vehicle.NonOwnedNamed)
                            valList.Add(new ValidationItem(String.Format("Vehicle #{0} - Named Non-Owned Non-Specific Vehicle may not be selected with Extended Non-Owned", vehicleIndex + 1), VehicleBothNonOwnerTypesSelected, true));

                        // should not have symbols, cost new, stated cost, actual cash value
                        if (vehicle.VehicleSymbols.Any())
                        {
                            // empty symbols are ok
                            var hasNonZeroSymbols = (from s in vehicle.VehicleSymbols where s.UserOverrideSymbol != "" & s.UserOverrideSymbol != "0" & s.UserOverrideSymbol != "00" select s).Any();
                            if (hasNonZeroSymbols)
                                valList.Add(new ValidationItem("Invalid Symbols", VehicleSymbols, false));
                        }
                        //VRGeneralValidations.Val_HasIneligibleField(vehicle.CostNew, valList, VehicleCostNew, "Cost New");
                        //VRGeneralValidations.Val_HasIneligibleField(vehicle.StatedAmount, valList, VehicleStatedAmount, "Stated Amount");
                        //VRGeneralValidations.Val_HasIneligibleField(vehicle.ActualCashValue, valList, vehicleActualCashValue, "Cash Value");

                        // check year, make , model, body style, use
                        if (vehicle.NonOwned)
                        {
                            if (VRGeneralValidations.Val_HasRequiredField(vehicle.Vin, valList, VehicleVIN, "VIN"))
                            {
                                if (vehicle.Vin.ToLower().Trim().StartsWith("none") == false)
                                    valList.Add(new ValidationItem("Invalid VIN", VehicleVIN, false));
                            }
                            if (VRGeneralValidations.Val_HasRequiredField(vehicle.Make, valList, VehicleMake, "Make"))
                            {
                                if (vehicle.Make.ToLower().Trim() != "extd")
                                    valList.Add(new ValidationItem("Invalid Make", VehicleMake, false));
                            }
                            if (VRGeneralValidations.Val_HasRequiredField(vehicle.Model, valList, VehicleModel, "Model"))
                            {
                                if (vehicle.Model.ToLower().Trim() != "non-owned")
                                    valList.Add(new ValidationItem("Invalid Model", VehicleModel, false));
                            }

                            if (VRGeneralValidations.Val_HasRequiredField(vehicle.Year, valList, VehicleYear, "Year"))
                            {
                                if (vehicle.Year.ToLower().Trim() != "1900")
                                    valList.Add(new ValidationItem("Invalid Year", VehicleYear, false));
                            }

                            if (VRGeneralValidations.Val_HasRequiredField_DD(vehicle.VehicleUseTypeId, valList, VehicleUse, "Vehicle Use"))
                            {
                                if (vehicle.VehicleUseTypeId.ToLower().Trim() != "6")
                                    valList.Add(new ValidationItem("Invalid Vehicle Use", VehicleUse, false));
                            }

                            if (VRGeneralValidations.Val_HasRequiredField_DD(vehicle.BodyTypeId, valList, VehicleBodyType, "Body Type"))
                            {
                                if (vehicle.BodyTypeId.ToLower().Trim() != "14")
                                    valList.Add(new ValidationItem("Invalid Body Type", VehicleBodyType, false));
                            }
                        }
                        else
                        {
                            // Non Named
                            if (VRGeneralValidations.Val_HasRequiredField(vehicle.Vin, valList, VehicleVIN, "VIN"))
                            {
                                if (vehicle.Vin.ToLower().Trim().StartsWith("none") == false)
                                    valList.Add(new ValidationItem("Invalid VIN", VehicleVIN, false));
                            }
                            if (VRGeneralValidations.Val_HasRequiredField(vehicle.Make, valList, VehicleMake, "Make"))
                            {
                                if (vehicle.Make.ToLower().Trim() != "named")
                                    valList.Add(new ValidationItem("Invalid Make", VehicleMake, false));
                            }
                            if (VRGeneralValidations.Val_HasRequiredField(vehicle.Model, valList, VehicleModel, "Model"))
                            {
                                if (vehicle.Model.ToLower().Trim() != "non-owner")
                                    valList.Add(new ValidationItem("Invalid Model", VehicleModel, false));
                            }

                            if (VRGeneralValidations.Val_HasRequiredField(vehicle.Year, valList, VehicleYear, "Year"))
                            {
                                int year = IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate().Year;
                                
                                // you need some flexibility here so that is why I allow either years above
                                if (vehicle.Year.ToLower().Trim() != year.ToString() && vehicle.Year.ToLower().Trim() != (year + 1).ToString())
                                    valList.Add(new ValidationItem("Invalid Year", VehicleYear, false));
                            }

                            if (VRGeneralValidations.Val_HasRequiredField_DD(vehicle.VehicleUseTypeId, valList, VehicleUse, "Vehicle Use"))
                            {
                                if (vehicle.VehicleUseTypeId.ToLower().Trim() != "6")
                                    valList.Add(new ValidationItem("Invalid Vehicle Use", VehicleUse, false));
                            }

                            if (VRGeneralValidations.Val_HasRequiredField_DD(vehicle.BodyTypeId, valList, VehicleBodyType, "Body Type"))
                            {
                                if (vehicle.BodyTypeId.ToLower().Trim() != "14")
                                    valList.Add(new ValidationItem("Invalid Body Type", VehicleBodyType, false));
                            }
                        }
                    } // end of Non-Named Section
                    else
                    {
                        VRGeneralValidations.Val_HasRequiredField(vehicle.Vin, valList, VehicleVIN, "VIN");
                        VRGeneralValidations.Val_HasRequiredField(vehicle.Make, valList, VehicleMake, "Make");

                        // confirm the make and model go together but only if it is a bodystyle that we have VIN info on
                        if (VRGeneralValidations.Val_HasRequiredField(vehicle.Model, valList, VehicleModel, "Model"))
                        {
                            if (vehicle.BodyTypeId != bodyType_AntiqueAuto && vehicle.BodyTypeId != bodyType_ClassicAuto && vehicle.BodyTypeId != bodyType_MotorCycle && vehicle.BodyTypeId != bodyType_MotorHome && vehicle.BodyTypeId != bodyType_OtherTrailer && vehicle.BodyTypeId != bodyType_RecTrailer)
                            {
                                bool attemptedMakeModelDBLookup = false;
                                bool makeModelDBLookupSuccessful = false;
                                bool is1981OrNewer = VRGeneralValidations.Val_IsNumberInRange(vehicle.Year, valList, VehicleYear, "Year", "1981", (IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate().Year + 1).ToString(), true);
                                if (is1981OrNewer)
                                {
                                    //Added 04/20/2020 for Bug 45376 MLW
                                    if (vehicle.CostNew == null || vehicle.CostNew == "" || vehicle.CostNew == "$0.00" || vehicle.CostNew == "$0" || vehicle.CostNew == "0")
                                    {
                                        attemptedMakeModelDBLookup = true;
                                        if (IFM.VR.Common.Helpers.PPA.MakeModelListLookup.ModelIsKnownWrong(vehicle.Make, vehicle.Model))
                                        {
                                            //make/model validation failed - set this to do a VIN RAPA API lookup in the next if statement
                                            makeModelDBLookupSuccessful = false;
                                            //valList.Add(new ValidationItem("Invalid Make and/or Model", VehicleMake, false));
                                        } 
                                        else
                                        {
                                            makeModelDBLookupSuccessful = true;
                                        }
                                    }
                                }
                                if (valType != ValidationItem.ValidationType.quoteRate || (attemptedMakeModelDBLookup == true && makeModelDBLookupSuccessful == false))
                                {
                                    // do more extensive test of make/model/year on app side - now also does this lookup on quote side if the above make/model validation fails
                                    //Updated 10/19/2022 for task 75263 MLW
                                    //var result = IFM.VR.Common.Helpers.PPA.VinLookup.GetMakeModelYearOrVinVehicleInfo(vehicle.Vin, "", "", 0, (Microsoft.VisualBasic.Information.IsDate(quote.EffectiveDate)) ? DateTime.Parse(quote.EffectiveDate) : DateTime.MinValue, quote.VersionId.TryToGetInt32()).FirstOrDefault();
                                    var isNewBusiness = true;
                                    if (quote.QuoteTransactionType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote) {
                                        isNewBusiness = false;
                                    }
                                    var lookupType = Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType.ModelISORAPA;
                                    if (IFM.VR.Common.Helpers.PPA.VinLookup.IsNewModelISORAPALookupTypeAvailable(quote.VersionId, quote.EffectiveDate, isNewBusiness)) {
                                        lookupType = Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType.ModelIsoRapaApi;
                                    }
                                    var effDate = quote.EffectiveDate;
                                    if (quote.QuoteTransactionType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                                    {
                                        effDate = quote.TransactionEffectiveDate;
                                    }
                                    var result = IFM.VR.Common.Helpers.PPA.VinLookup.GetMakeModelYearOrVinVehicleInfo_OptionalLookupType(vehicle.Vin, "", "", 0, (Microsoft.VisualBasic.Information.IsDate(effDate)) ? DateTime.Parse(effDate) : DateTime.MinValue, quote.VersionId.TryToGetInt32(), lookupType, quote.PolicyId, quote.PolicyImageNum, "0").FirstOrDefault();
                                    if (result != null)
                                    {
                                        bool allVinInfoMatched = true;
                                        if (result.Make.Trim().ToLower() != vehicle.Make.ToLower().Trim())
                                            allVinInfoMatched = false;
                                        if (!(result.Model.Trim().ToLower() == vehicle.Model.ToLower().Trim() | result.Description.ToLower().Trim() == vehicle.Model.ToLower().Trim()))
                                            allVinInfoMatched = false;
                                        if (result.Year.ToString().Trim() != vehicle.Year.Trim())
                                            allVinInfoMatched = false;
                                        if (allVinInfoMatched == false)
                                        {
                                            if (attemptedMakeModelDBLookup == true && makeModelDBLookupSuccessful == false)
                                            {
                                                //if both validations (DB lookup in ModelIsKnownWrong & VIN RAPA API) fail on quote side, show original message
                                                valList.Add(new ValidationItem("Invalid Make and/or Model", VehicleMake, false));
                                            }
                                            if (valType != ValidationItem.ValidationType.quoteRate)
                                            {
                                                //Continue with app side validations
                                                //Added 04/22/2020 for Bug 45376 MLW
                                                if (vehicle.CostNew == null || vehicle.CostNew == "" || vehicle.CostNew == "$0.00" || vehicle.CostNew == "$0" || vehicle.CostNew == "0")
                                                {
                                                    valList.Add(new ValidationItem("Verify the combination of the VIN and Year/Make/Model.", VehicleVIN, false));
                                                }
                                                else
                                                {
                                                    valList.Add(new ValidationItem("Verify the combination of the VIN and Year/Make/Model.", VehicleVIN, true));
                                                }
                                                //valList.Add(new ValidationItem("Verify the combination of the VIN and Year/Make/Model.", VehicleVIN, false));
                                            }
                                        }

                                    } 
                                    else
                                    {
                                        if (attemptedMakeModelDBLookup == true && makeModelDBLookupSuccessful == false)
                                        {
                                            valList.Add(new ValidationItem("Invalid Make and/or Model", VehicleMake, false));
                                        }
                                    }
                                }
                            }
                        }

                        if (VRGeneralValidations.Val_HasRequiredField(vehicle.Year, valList, VehicleYear, "Year"))
                        {
                            if (VRGeneralValidations.Val_IsNumberInRange(vehicle.Year, valList, VehicleYear, "Year", "1800", (IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate().Year + 1).ToString()))
                            {
                                bool is1981OrNewer = VRGeneralValidations.Val_IsNumberInRange(vehicle.Year, valList, VehicleYear, "Year", "1981", (IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate().Year + 1).ToString(), true);
                                //1981 or newer can not have spaces - older can
                                if (valType == ValidationItem.ValidationType.quoteRate)
                                {
                                    if (IFM.Common.InputValidation.CommonValidations.IsAlphaNum(vehicle.Vin, (is1981OrNewer) ? "&" : "&, ") == false)
                                        valList.Add(new ValidationItem("Invalid VIN. Please confirm VIN", VehicleVIN, false));
                                }
                                else
                                {
                                    // can not have the '&' on app side
                                    // 1981 and newer can not have a space in the VIN and Must be 17 chars long
                                    if (is1981OrNewer)
                                    {
                                        if (IFM.Common.InputValidation.CommonValidations.IsTextLenghtInRange(vehicle.Vin, 17, 17) == false)
                                            valList.Add(new ValidationItem("Invalid VIN length", VehicleVIN, false));
                                    }

                                    if (IFM.Common.InputValidation.CommonValidations.IsAlphaNum(vehicle.Vin, (is1981OrNewer) ? "" : " ") == false)
                                        valList.Add(new ValidationItem("Invalid VIN. Please confirm VIN", VehicleVIN, false));
                                }
                            }
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(vehicle.BodyTypeId) == false & vehicle.BodyTypeId != "0" & vehicle.BodyTypeId != bodyType_MotorCycle & vehicle.BodyTypeId != bodyType_MotorHome & vehicle.BodyTypeId != bodyType_OtherTrailer & vehicle.BodyTypeId != bodyType_RecTrailer)
                            {
                                if (valType == ValidationItem.ValidationType.quoteRate)
                                {
                                    if (IFM.Common.InputValidation.CommonValidations.IsAlphaNum(vehicle.Vin, "&, ") == false) //7-1-14 added whitespace
                                        valList.Add(new ValidationItem("Invalid VIN. Please confirm VIN", VehicleVIN, false));
                                }
                                else
                                {
                                    if (IFM.Common.InputValidation.CommonValidations.IsAlphaNum(vehicle.Vin, " ") == false) //7-1-14 added whitespace
                                        valList.Add(new ValidationItem("Invalid VIN. Please confirm VIN", VehicleVIN, false));
                                }
                            }
                        }

                        if (VRGeneralValidations.Val_HasRequiredField_DD(vehicle.BodyTypeId, valList, VehicleBodyType, "Body Type"))
                        {
                            if (vehicle.BodyTypeId == bodyType_MotorCycle)
                            {
                                VRGeneralValidations.Val_HasRequiredField_DD(vehicle.VehicleTypeId, valList, VehicleMotorCycleType, "Motorcycle Type");
                                VRGeneralValidations.Val_HasRequiredField_Int(vehicle.CubicCentimeters, valList, VehicleMotorCycleHorsePower, "Horsepower/CCs");
                            }

                            // these bodytypes require a cost new
                            string[] costNewBodyTypes = new string[] { bodyType_MotorHome, bodyType_MotorCycle, bodyType_PICKUPWCAMPER, bodyType_RecTrailer, bodyType_ClassicAuto }; // Bug 8104 1/23/2017 Added - bodyType_ClassicAuto
                            if (costNewBodyTypes.Contains(vehicle.BodyTypeId))
                            {
                                if (VRGeneralValidations.Val_HasRequiredField(vehicle.CostNew, valList, VehicleCostNew, "Cost New"))
                                    VRGeneralValidations.Val_IsGreaterThanZeroNumber(vehicle.CostNew.Replace_NullSafe("$", "").Replace(",", ""), valList, VehicleCostNew, "Cost New");
                            }

                            // removed clasic auto 1/23/2017 Matt A Bug 8104    // vehicle.BodyTypeId == bodyType_ClassicAuto 
                            if (vehicle.BodyTypeId == bodyType_AntiqueAuto | vehicle.BodyTypeId == bodyType_OtherTrailer)
                            {
                                if (VRGeneralValidations.Val_HasRequiredField(vehicle.StatedAmount, valList, VehicleStatedAmount, "Stated Amount"))
                                    VRGeneralValidations.Val_IsGreaterThanZeroNumber(vehicle.StatedAmount, valList, VehicleStatedAmount, "Stated Amount");
                            }
                            else
                            {
                                var isNewBusiness = true;
                                if (quote.QuoteTransactionType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                                {
                                    isNewBusiness = false;
                                }
                                if (IFM.VR.Common.Helpers.PPA.VinLookup.IsNewModelISORAPALookupTypeAvailable(quote.VersionId, quote.EffectiveDate, isNewBusiness))
                                {
                                    if (vehicle.VehicleSymbols == null || vehicle.VehicleSymbols.Count < 2 || vehicle.VehicleSymbols[0].UserOverrideSymbol.Trim().EqualsAny(String.Empty, "00", "0") || vehicle.VehicleSymbols[1].UserOverrideSymbol.Trim().EqualsAny(String.Empty, "00", "0"))
                                    {
                                        // no symbols
                                        if (VRGeneralValidations.Val_HasRequiredField_Int(vehicle.CostNew, valList, VehicleCostNew, "Cost New"))
                                            VRGeneralValidations.Val_IsGreaterThanZeroNumber(vehicle.CostNew, valList, VehicleCostNew, "Cost New");
                                    }
                                    else
                                    {
                                        //As of 1/1/2025, three new symbols are required at rate for these vehicles - 6 symbols total
                                        if (isNewBusiness && IFM.VR.Common.Helpers.PPA.NewRAPASymbolsHelper.IsNewRAPASymbolsAvailable(quote))
                                        {
                                            if (((vehicle.BodyTypeId == bodyType_Car || vehicle.BodyTypeId == bodyType_SUV || vehicle.BodyTypeId == bodyType_Van || vehicle.BodyTypeId == bodyType_PICKUPWOCAMPER) && (vehicle.CostNew == null || vehicle.CostNew == "" || vehicle.CostNew == "$0.00" || vehicle.CostNew == "$0" || vehicle.CostNew == "0")) && (vehicle.VehicleSymbols == null || vehicle.VehicleSymbols.Count < 6))
                                            {
                                                valList.Add(new ValidationItem("An effective date of January 1, 2025 or later requires you to select the lookup button to ensure the correct rates and auto symbols are applied when re-rating your quote.", VehicleSymbols, false));
                                            }
                                        }
                                    }
                                } else
                                {
                                    if (vehicle.VehicleSymbols == null || vehicle.VehicleSymbols.Count < 2 || vehicle.VehicleSymbols[0].UserOverrideSymbol.Trim().EqualsAny(String.Empty, "00", "0") || vehicle.VehicleSymbols[0].UserOverrideSymbol.Trim().StartsWith("P") || vehicle.VehicleSymbols[1].UserOverrideSymbol.Trim().EqualsAny(String.Empty, "00", "0") || vehicle.VehicleSymbols[1].UserOverrideSymbol.Trim().StartsWith("P"))
                                    {
                                        // no symbols
                                        if (VRGeneralValidations.Val_HasRequiredField_Int(vehicle.CostNew, valList, VehicleCostNew, "Cost New"))
                                            VRGeneralValidations.Val_IsGreaterThanZeroNumber(vehicle.CostNew, valList, VehicleCostNew, "Cost New");
                                    }
                                }
                                //if (vehicle.VehicleSymbols == null || vehicle.VehicleSymbols.Count < 2 || vehicle.VehicleSymbols[0].UserOverrideSymbol.Trim().EqualsAny(String.Empty, "00", "0") || vehicle.VehicleSymbols[0].UserOverrideSymbol.Trim().StartsWith("P") || vehicle.VehicleSymbols[1].UserOverrideSymbol.Trim().EqualsAny(String.Empty, "00", "0") || vehicle.VehicleSymbols[1].UserOverrideSymbol.Trim().StartsWith("P"))
                                //{
                                //    // no symbols
                                //    if (VRGeneralValidations.Val_HasRequiredField_Int(vehicle.CostNew, valList, VehicleCostNew, "Cost New"))
                                //        VRGeneralValidations.Val_IsGreaterThanZeroNumber(vehicle.CostNew, valList, VehicleCostNew, "Cost New");
                                //}
                            }

                            if (vehicle.VehicleUseTypeId == "4" && !(vehicle.BodyTypeId == bodyType_PICKUPWCAMPER || vehicle.BodyTypeId == bodyType_PICKUPWOCAMPER)) // Farm Useage - BUG 6388
                            {
                                valList.Add(new ValidationItem("Farm Useage is only Valid on Body Type of Pickup", VehicleUse, false));
                            }

                        }

                        VRGeneralValidations.Val_HasRequiredField_DD(vehicle.PerformanceTypeId, valList, VehiclePerformace, "Performance");
                    } // end Not  - NON-Named Section

                    if (!IFM.VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(quote))
                    {
                        List<int> DriverAssignmentList = new List<int>();
                        int driverNum = 0;
                        if (VRGeneralValidations.Val_HasRequiredField(vehicle.PrincipalDriverNum, valList, VehiclePrimaryAssignedDriver, "", true) == false & !(vehicle.BodyTypeId == bodyType_RecTrailer | vehicle.BodyTypeId == bodyType_OtherTrailer))
                        { valList.Add(new ValidationItem("Missing Principal Driver", VehiclePrimaryAssignedDriver, false)); }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(vehicle.PrincipalDriverNum) == false)
                            {
                                if (int.TryParse(vehicle.PrincipalDriverNum, out driverNum))
                                    DriverAssignmentList.Add(Convert.ToInt32(vehicle.PrincipalDriverNum));
                            }
                        }

                        if (string.IsNullOrWhiteSpace(vehicle.OccasionalDriver1Num) == false && int.TryParse(vehicle.OccasionalDriver1Num, out driverNum))
                            DriverAssignmentList.Add(Convert.ToInt32(vehicle.OccasionalDriver1Num));

                        if (string.IsNullOrWhiteSpace(vehicle.OccasionalDriver2Num) == false && int.TryParse(vehicle.OccasionalDriver2Num, out driverNum))
                            DriverAssignmentList.Add(Convert.ToInt32(vehicle.OccasionalDriver2Num));

                        if (string.IsNullOrWhiteSpace(vehicle.OccasionalDriver3Num) == false && int.TryParse(vehicle.OccasionalDriver3Num, out driverNum))
                            DriverAssignmentList.Add(Convert.ToInt32(vehicle.OccasionalDriver3Num));

                        //check for duplicate assigned drivers
                        if (DriverAssignmentList.Count != DriverAssignmentList.Distinct().Count())
                            valList.Add(new ValidationItem("Driver Restricted to Once Per Vehicle", VehicleDriverAssignedMoreThanOnce, false));
                    }
                    else
                    {
                        VRGeneralValidations.Val_HasRequiredField_DD(vehicle.AnnualMiles, valList, AnnualMileage, "Annual Mileage");
                    }
                }
            }
            else
            {
                valList.Add(new ValidationItem("No vehicles on policy.", NoVehicles, false));
            }
            return valList;
        }
    }
}