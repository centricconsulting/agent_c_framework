using QuickQuote.CommonMethods;
using QuickQuote.CommonObjects;
using System;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class RvWaterCraftValidator
    {
        //removed 3/11/2021
        //private static int _rowNumber;

        //public static int RowNumber
        //{
        //    get
        //    {
        //        return _rowNumber;
        //    }
        //    set
        //    {
        //        _rowNumber = value;
        //    }
        //}

        private static QuickQuoteHelperClass qqHelper = new QuickQuoteHelperClass();

        public const string ValidationListID = "{BCB708B9-A5B3-4023-85D3-56874855D9B6}";

        public const string CraftIsNull = "{E2CDC24F-7DA9-42D5-8F20-DC2E15D66066}";

        public const string VehicleTypeIsNull = "{DB18B418-96A7-43E9-965B-213101110F9F}";
        public const string CoverageOptionIsNull = "{22D3B762-03A5-4CC0-AE67-5413784E5E05}";
        public const string PropertyDeductIsNull = "{125211CE-5BEE-475D-851F-6871BCFE9149}";

        public const string YearMissing = "{0FA7E06A-9C0A-4B4D-87A5-B23F75D6B767}";
        public const string YearRange = "{3C9F48D8-B907-42C8-A5C6-4E8315237918}";

        public const string LengthMissing = "{F8A1EF51-EC77-4498-B1B3-7126FADE2DBC}";
        public const string LengthNumeric = "{0AE44C1F-A35E-4E2A-BE02-C2E751DD4E0E}";
        public const string LengthInvalid = "{B2FCF06F-9316-460A-95A2-6F53788E5432}";
        public const string LengthMaximum = "{F8FF40C1-DD8B-48B7-B954-4F6C84044672}";

        public const string CostNewMissing = "{47BD15B2-F010-4052-878A-7A15499D8CF2}";
        public const string CostNewNumeric = "{74251E26-6351-427C-A6F0-6F79A30BFB66}";
        public const string CostNewInvalid = "{109E82E2-4BEB-480C-A4C3-1ACFC13DE0D7}";
        public const string CostNew_LessThan_Deductible = "{D52F241E-F47D-4C74-9667-3836BD1FE17A}";

        public const string DescMissing = "{61D95793-0E7A-4512-BFCB-BA5D771061FE}";

        public const string HPMissing = "{7CCC593D-1464-4489-84D7-C1B736EED12D}";
        public const string MinimumHP = "{B5419CF9-A7C7-453B-A53D-FBE60C4A81E5}";
        public const string MPHInvalid = "{FE9F758D-E386-4906-83F7-4F0715AFEFC6}";

        public const string MotorTypeMissing = "{95ACC177-82E6-4E61-8A67-CB6CB2C158A0}";

        public const string MotorYearMissing = "{BDF948F1-C3CB-4029-974B-1BB8F5E99D16}";
        public const string MotorYearInvalid = "{7D3A34A9-8B11-46E7-B44E-CF24551EDB91}";

        public const string MotorCostNewMissing = "{2D9F4A05-C8B9-4FAA-A69D-71B2B5AC019C}";
        public const string MotorCostNewNumeric = "{98B3D23A-FFAB-4B5C-9E54-2FAE66ACEF32}";
        public const string MotorCostNewInvalid = "{797BA3AC-6840-4BB1-BE7B-F664F82EAADD}";

        public const string SerialNumberMissing = "{9A8105B5-D5A6-4ED4-83F8-B73F5B883360}";
        public const string SerialNumberInvalid = "{5F107FB4-1EFA-4A3E-B948-314E424F45D8}";

        public const string ManufacturerMissing = "{53890238-4D85-4704-AB0D-354A2C38A815}";
        public const string ManufacturerInvalid = "{CB66F9BF-80F6-415A-B3C5-936B9F7EFEDE}";

        public const string ModelMissing = "{E7F08A3B-7C89-45BC-BB56-BFB93E14AAE3}";
        public const string ModelInvalid = "{7AC71702-9C0E-40F8-8FEA-8C35E670BA6D}";

        public static Validation.ObjectValidation.ValidationItemList ValidateRvWaterCraft(QuickQuote.CommonObjects.QuickQuoteRvWatercraft craft, ValidationItem.ValidationType valType, string LOBType, string formName = "", string selectedCoverageOption = "", string selectedVehicleType = "", QuickQuoteObject quote = null)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (craft != null)
            {
                switch (valType)
                {
                    case ValidationItem.ValidationType.endorsement:
                    case ValidationItem.ValidationType.issuance:
                    case ValidationItem.ValidationType.appRate:

                        string[] needOperators = { "6", "7", "8", "10" };

                        // all types but Accessories require serial/manufacturer/Model - ON APP Side
                        string[] DontNeedMoreInfoTypes = { "5" };
                        if (DontNeedMoreInfoTypes.Contains(craft.RvWatercraftTypeId) == false)
                        {
                            if (craft.RvWatercraftTypeId != "3") // Boat Motor Only doesn't have these fields - Matt A 4-26-2016
                            {
                                //Updated 02/11/2020 for Home Endorsements task 43871 MLW // CAH B60638
                                if (valType == ValidationItem.ValidationType.endorsement)
                                {
                                    //if (VRGeneralValidations.Val_HasRequiredField(craft.SerialNumber, valList, SerialNumberMissing, "Serial Number", craft.RvWatercraftTypeId == "5"))
                                    //    VRGeneralValidations.Val_IsTextLengthInRange(craft.SerialNumber, valList, SerialNumberInvalid, "Serial Number", "4", "50");
                                    VRGeneralValidations.Val_HasRequiredField(craft.SerialNumber, valList, SerialNumberMissing, "Serial Number", craft.RvWatercraftTypeId == "5");
                                    VRGeneralValidations.Val_HasRequiredField(craft.Manufacturer, valList, ManufacturerMissing, "Manufacturer", craft.RvWatercraftTypeId == "5");
                                    VRGeneralValidations.Val_HasRequiredField(craft.Model, valList, ModelMissing, "Model", craft.RvWatercraftTypeId == "5");                               
                                } else
                                {
                                    if (VRGeneralValidations.Val_HasRequiredField(craft.SerialNumber, valList, SerialNumberMissing, "Serial Number", craft.RvWatercraftTypeId == "5"))
                                        if (VRGeneralValidations.Val_IsTextLengthInRange(craft.SerialNumber, valList, SerialNumberInvalid, "Serial Number", "4", "50"))
                                            VRGeneralValidations.Val_IsValidAlphaNumeric(craft.SerialNumber, valList, SerialNumberInvalid, "Serial Number");

                                    if (VRGeneralValidations.Val_HasRequiredField(craft.Manufacturer, valList, ManufacturerMissing, "Manufacturer", craft.RvWatercraftTypeId == "5"))
                                        VRGeneralValidations.Val_IsValidAlphaNumeric(craft.Manufacturer, valList, ManufacturerInvalid, "Manufacturer");

                                    if (VRGeneralValidations.Val_HasRequiredField(craft.Model, valList, ModelMissing, "Model", craft.RvWatercraftTypeId == "5"))
                                        VRGeneralValidations.Val_IsValidAlphaNumeric(craft.Model, valList, ModelInvalid, "Model");
                                }
                            }
                            if (craft.RvWatercraftTypeId == "3") // Boat Motor Only CAH 8/4/2020 - Check for All Val types, but Quote
                            {

                                if (valType != ValidationItem.ValidationType.endorsement)
                                {
                                    if (VRGeneralValidations.Val_HasRequiredField(craft.RvWatercraftMotors[0].SerialNumber, valList, SerialNumberMissing, "Serial Number", craft.RvWatercraftTypeId == "5"))
                                        VRGeneralValidations.Val_IsTextLengthInRange(craft.RvWatercraftMotors[0].SerialNumber, valList, SerialNumberInvalid, "Serial Number", "4", "50");
                                }
                                VRGeneralValidations.Val_HasRequiredField(craft.RvWatercraftMotors[0].Manufacturer, valList, ManufacturerMissing, "Manufacturer", craft.RvWatercraftTypeId == "5");
                                VRGeneralValidations.Val_HasRequiredField(craft.RvWatercraftMotors[0].Model, valList, ModelMissing, "Model", craft.RvWatercraftTypeId == "5");
                            }

                        }

                        goto case ValidationItem.ValidationType.quoteRate; // also want to do all normal quote side stuff as well

                    case ValidationItem.ValidationType.quoteRate:
                        if (selectedVehicleType == "0")
                            valList.Add(new ObjectValidation.ValidationItem("Missing RV/Watercraft Type", VehicleTypeIsNull));
                        else
                        {
                            switch (qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.RvWatercraftTypeId, selectedVehicleType).ToUpper())
                            {
                                case "WATERCRAFT":
                                case "JET SKIS & WAVERUNNERS":
                                    ValidateCoverage(ref valList, selectedCoverageOption);

                                    if (selectedCoverageOption != "LIABILITY ONLY" && selectedCoverageOption != "")
                                    {
                                        ValidatePropertyDamageDeductible(ref valList, craft);
                                        ValidateVehicleCostNew(ref valList, craft);
                                    }

                                    ValidateVehicleYear(ref valList, craft);
                                    ValidateVehicleLength(ref valList, craft, formName, LOBType);
                                    ValidateMotor(ref valList, craft, formName, LOBType, quote);
                                    break;

                                case "SAILBOAT":
                                    ValidateCoverage(ref valList, selectedCoverageOption);

                                    if (selectedCoverageOption != "LIABILITY ONLY" && selectedCoverageOption != "")
                                    {
                                        ValidatePropertyDamageDeductible(ref valList, craft);
                                        ValidateVehicleCostNew(ref valList, craft);
                                    }

                                    ValidateVehicleYear(ref valList, craft);
                                    ValidateVehicleLength(ref valList, craft, formName, LOBType);
                                    break;

                                case "BOAT MOTOR ONLY":
                                    ValidatePropertyDamageDeductible(ref valList, craft);
                                    ValidateMotor(ref valList, craft, formName, LOBType, quote);
                                    break;

                                case "4 WHEEL ATV":
                                    if (LOBType == "10" || LOBType == "8") //updated 10/30/17 for HOM Upgrade MLW
                                    {
                                        ValidateCoverage(ref valList, selectedCoverageOption);

                                        if (selectedCoverageOption != "LIABILITY ONLY" && selectedCoverageOption != "")
                                        {
                                            ValidatePropertyDamageDeductible(ref valList, craft);
                                            ValidateVehicleCostNew(ref valList, craft);
                                        }

                                        ValidateVehicleYear(ref valList, craft);
                                    }
                                    else
                                    {
                                        ValidatePropertyDamageDeductible(ref valList, craft);
                                        ValidateVehicleYear(ref valList, craft);
                                        ValidateVehicleCostNew(ref valList, craft);
                                    }
                                    break;

                                    //case "OTHER RV": // Matt A - 3-21-06 for Comparative Rater Project
                                    //    if (LOBType == "8") //updated 10/30/17 for HOM Upgrade MLW
                                    //    {
                                    //        ValidateCoverage(ref valList, selectedCoverageOption);

                                    //        if (selectedCoverageOption != "LIABILITY ONLY" && selectedCoverageOption != "")
                                    //        {
                                    //            ValidatePropertyDamageDeductible(ref valList, craft);
                                    //            ValidateVehicleCostNew(ref valList, craft);
                                    //        }

                                    //        ValidateVehicleYear(ref valList, craft);
                                    //    }
                                    //    else
                                    //    {
                                    //        ValidatePropertyDamageDeductible(ref valList, craft);
                                    //        ValidateVehicleYear(ref valList, craft);
                                    //        ValidateVehicleCostNew(ref valList, craft);
                                    //    }
                                    //    break;
                                    //case "GOLF CART":
                                    //    ValidateCoverage(ref valList, selectedCoverageOption);

                                    //    if (selectedCoverageOption != "LIABILITY ONLY" && selectedCoverageOption != "")
                                    //    {
                                    //        ValidatePropertyDamageDeductible(ref valList, craft);
                                    //    }

                                    //    ValidateVehicleYear(ref valList, craft);
                                    //    ValidateVehicleCostNew(ref valList, craft);
                                    //    break;
                                case "OTHER RV":
                                case "GOLF CART":
                                case "BOAT TRAILER":
                                    ValidateCoverage(ref valList, selectedCoverageOption);
                                    if(selectedCoverageOption != "LIABILITY ONLY" && selectedCoverageOption != "")
                                    {
                                        ValidatePropertyDamageDeductible(ref valList, craft);    
                                        ValidateVehicleCostNew(ref valList, craft);
                                    }
                                    ValidateVehicleYear(ref valList, craft);

                                    break;

                                case "ACCESSORIES & EQUIPMENT":
                                    ValidatePropertyDamageDeductible(ref valList, craft);
                                    ValidateVehicleCostNew(ref valList, craft);
                                    ValidateDescription(ref valList, craft);
                                    break;

                                case "SNOWMOBILE - NAMED PERILS":
                                case "SNOWMOBILE - SPECIAL COVERAGE":
                                    ValidateCoverage(ref valList, selectedCoverageOption);

                                    if (selectedCoverageOption != "LIABILITY ONLY" && selectedCoverageOption != "")
                                    {
                                        ValidatePropertyDamageDeductible(ref valList, craft);
                                        ValidateVehicleCostNew(ref valList, craft);
                                    }

                                    ValidateVehicleYear(ref valList, craft);
                                    ValidateDescription(ref valList, craft);
                                    break;

                                default:
                                    break;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            else
            {
                valList.Add(new ObjectValidation.ValidationItem("RV Watercraft is null", CraftIsNull));
            }

            return valList;
        }

        private static void ValidateCoverage(ref Validation.ObjectValidation.ValidationItemList valList, string selectedCoverageOpt)
        {
            if (selectedCoverageOpt == "")
                valList.Add(new ObjectValidation.ValidationItem("Missing Coverage Option", CoverageOptionIsNull));
        }

        private static void ValidatePropertyDamageDeductible(ref Validation.ObjectValidation.ValidationItemList valList, QuickQuote.CommonObjects.QuickQuoteRvWatercraft craft)
        {
            if(craft.HasLiabilityOnly == false)
            {
                if (craft.PropertyDeductibleLimitId == "-1")
                    valList.Add(new ObjectValidation.ValidationItem("Missing Property Damage Deductible", PropertyDeductIsNull));
            }
            
        }

        private static void ValidateVehicleYear(ref Validation.ObjectValidation.ValidationItemList valList, QuickQuote.CommonObjects.QuickQuoteRvWatercraft craft)
        {
            if (craft.Year == "")
                valList.Add(new ObjectValidation.ValidationItem("Missing Year", YearMissing));
            else if (!IFM.Common.InputValidation.CommonValidations.IsNumberInRange(craft.Year, 1800, DateTime.Now.Year + 1))
                valList.Add(new ObjectValidation.ValidationItem("Invalid Year", YearRange));
        }

        private static void ValidateVehicleLength(ref Validation.ObjectValidation.ValidationItemList valList, QuickQuote.CommonObjects.QuickQuoteRvWatercraft craft, string formName, string lobType)
        {
            if (craft.Length == "")
                valList.Add(new ObjectValidation.ValidationItem("Missing Length in Feet", LengthMissing));
            else
            {
                if (!IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(craft.Length))
                    valList.Add(new ObjectValidation.ValidationItem("Invalid Value", LengthNumeric));

                if (int.Parse(craft.Length) == 0)
                    valList.Add(new ObjectValidation.ValidationItem("0 is not valid", LengthInvalid));

                if (lobType == "8")
                {
                    switch (formName)
                    {
                        case "HO-2":
                        case "HO-3":
                        case "HO-4":
                        case "HO-6":
                            if (craft.Length == "" || int.Parse(craft.Length) > 26)
                                valList.Add(new ObjectValidation.ValidationItem("Maximum length is 26 feet", LengthMaximum));
                            break;

                        case "ML-2":
                        case "ML-4":
                            if (craft.Length == "" || int.Parse(craft.Length) > 40)
                                valList.Add(new ObjectValidation.ValidationItem("Maximum length is 40 feet", LengthMaximum));
                            break;
                    }
                }

                if (lobType == "10")
                {
                    if (craft.Length == "" || int.Parse(craft.Length) > 40)
                        valList.Add(new ObjectValidation.ValidationItem("Maximum length is 40 feet", LengthMaximum));
                }
            }
        }

        private static void ValidateVehicleCostNew(ref Validation.ObjectValidation.ValidationItemList valList, QuickQuote.CommonObjects.QuickQuoteRvWatercraft craft)
        {
            if (craft.HasLiabilityOnly == false)
            {
                if (craft.CostNew == "")
                    valList.Add(new ObjectValidation.ValidationItem("Missing Cost New", CostNewMissing));
                else
                {
                    if (!IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(craft.CostNew))
                        valList.Add(new ObjectValidation.ValidationItem("Invalid Value", CostNewNumeric));

                    if (double.Parse(craft.CostNew.Replace("$", "")) == 0.0)
                        valList.Add(new ObjectValidation.ValidationItem("0 is not valid", CostNewInvalid));

                    // Validate that Limit is not less than the selected Deductible
                    if (craft.PropertyDeductibleLimitId != "-1")
                    {
                        if (double.Parse(craft.CostNew.Replace("$", "")) <= double.Parse(qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyDeductibleLimitId, craft.PropertyDeductibleLimitId)))
                            valList.Add(new ObjectValidation.ValidationItem("A deductible equal to or greater than the coverage limit has been entered. Please modify either value to ensure proper coverage.", CostNew_LessThan_Deductible, true));
                    }
                }
            }
        }

        private static void ValidateDescription(ref Validation.ObjectValidation.ValidationItemList valList, QuickQuote.CommonObjects.QuickQuoteRvWatercraft craft)
        {
            if (craft.Description == "")
                valList.Add(new ObjectValidation.ValidationItem("Missing Description", DescMissing));
        }

        private static void ValidateMotor(ref Validation.ObjectValidation.ValidationItemList valList, QuickQuote.CommonObjects.QuickQuoteRvWatercraft craft, string formName, string lobType, QuickQuoteObject quote = null)
        {
            if (craft.RvWatercraftMotors[0].MotorTypeId == "-1")
                valList.Add(new ObjectValidation.ValidationItem("Missing Motor Type", MotorTypeMissing));
            else
            {
                string motorType = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraftMotor, QuickQuoteHelperClass.QuickQuotePropertyName.MotorTypeId, craft.RvWatercraftMotors[0].MotorTypeId).ToUpper();
                bool isMobileHome = (formName != "ML-2" && formName != "ML-4" ? false : true);

                if (motorType != "")
                {
                    if (lobType != "10")
                    {
                        if (isMobileHome)
                        {
                            if (craft.RatedSpeed == "")
                                valList.Add(new ObjectValidation.ValidationItem("Missing Rated Speed in MPH", HPMissing));
                        }
                        else
                        {
                            if (IFM.VR.Common.Helpers.RvWaterCraftHelper.IsRvWaterCraftMotorAvailable(quote))
                            {
                                if ((craft.HorsepowerCC == "" && motorType != "OUTBOARD") || (craft.HorsepowerCC == "" && craft.RvWatercraftTypeId == "3"))
                                    valList.Add(new ObjectValidation.ValidationItem("Missing Horsepower/CCs", HPMissing));
                            }
                            else
                            {
                                if (craft.HorsepowerCC == "")
                                    valList.Add(new ObjectValidation.ValidationItem("Missing Horsepower/CCs", HPMissing));
                            }


                            
                        }
                    }
                    else
                    {
                        if (craft.RvWatercraftMotors[0].MotorTypeId != "" && craft.RvWatercraftMotors[0].MotorTypeId != "0")
                        {
                            if (craft.RatedSpeed == "")
                                valList.Add(new ObjectValidation.ValidationItem("Missing Rated Speed in MPH", HPMissing));

                            if (craft.RatedSpeed == "0")
                                valList.Add(new ObjectValidation.ValidationItem("0 is not a valid value", MPHInvalid));
                        }
                    }

                    switch (motorType)
                    {
                        case "OUTBOARD":
                            if (isMobileHome)
                            {
                                //Updated 12/7/17 to Rated Speed in MPH - MLW
                                if (craft.HorsepowerCC != "")
                                {
                                   
                                    if (int.Parse(craft.HorsepowerCC) < 26)
                                        valList.Add(new ObjectValidation.ValidationItem("HP for Outboard Motor must be greater than 26", MinimumHP));
                                }
                            }

                            if ((craft.HorsepowerCC != "" && craft.HorsepowerCC != "0") || IFM.VR.Common.Helpers.RvWaterCraftHelper.IsRvWaterCraftMotorAvailable(quote) == false)
                            {
                                if (craft.RvWatercraftMotors[0].Year == "")
                                    valList.Add(new ObjectValidation.ValidationItem("Missing Year", MotorYearMissing));
                                else if (!IFM.Common.InputValidation.CommonValidations.IsNumberInRange(craft.RvWatercraftMotors[0].Year, 1800, DateTime.Now.Year + 1))
                                    valList.Add(new ObjectValidation.ValidationItem("Invalid Motor Year", MotorYearInvalid));

                                if (craft.RvWatercraftMotors[0].CostNew == "")
                                    valList.Add(new ObjectValidation.ValidationItem("Missing Cost New", MotorCostNewMissing));
                                else
                                {
                                    if (!IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(craft.RvWatercraftMotors[0].CostNew))
                                        valList.Add(new ObjectValidation.ValidationItem("Invalid Value", MotorCostNewNumeric));

                                    if (double.Parse(craft.RvWatercraftMotors[0].CostNew.Replace("$", "")) == 0.0)
                                        valList.Add(new ObjectValidation.ValidationItem("0 is not valid", MotorCostNewInvalid));

                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}