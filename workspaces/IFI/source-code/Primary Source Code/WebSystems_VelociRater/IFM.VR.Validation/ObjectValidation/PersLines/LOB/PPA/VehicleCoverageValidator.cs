using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuickQuote.CommonMethods;
using QuickQuote.CommonObjects;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA
{
    public class VehicleCoverageValidator
    {
        public const string ValidationListID = "{3B8B0716-7300-4FC3-8220-499427F0DA51}";
        public const string QuoteIsNull = "{020E10ED-2914-45A8-A6F4-7279CA2AD1DD}";
        public const string UnExpectedLobType = "{ACC6EB62-FACB-4D61-BDDB-06F5769F0180}";
        public const string NoVehicles = "{4D405F60-55E2-47AC-BAE7-556712F18CC5}";

        public const string VehicleCoverageRadio = "{EFCD5EE0-AE40-4AE3-A482-1860BB1CBFFF}";
        public const string VehicleCoverageCustEquip = "{44F029BF-C845-4BA3-88C9-C51EBBE7AF85}";

        public const string VehicleHasCompOnly = "{84F027BF-C245-4BA6-88C7-C51EBBE3AF81}";

        public static Validation.ObjectValidation.ValidationItemList ValidatePPAVehicleCoverage(int vehicleNumber, QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.VehicleIndex, vehicleNumber.ToString());

            if (quote != null && quote.Vehicles != null && quote.Vehicles.Count > vehicleNumber)
            {
                QuickQuote.CommonObjects.QuickQuoteVehicle vehicle = quote.Vehicles[vehicleNumber];
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

                    if (vehicle.ComprehensiveCoverageOnly)
                    {
                        if (quote.QuoteTransactionType != QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                        {
                            // This validation only applies to new business.
                            // Endorsements will allow an existing vehicle with comp only to keep it.
                            valList.Add(new ValidationItem($"Vehicle #{vehicleNumber} - PHYSICAL DAMAGE ONLY (PARKED CAR) is no longer supported. Coverage changed to FULL COVERAGE for the affected vehicle. ", VehicleHasCompOnly));
                        }
                    }

                    if (vehicle.ComprehensiveDeductibleLimitId == "0" && vehicle.CollisionDeductibleLimitId == "0" && vehicle.SoundEquipmentLimit != "0")
                        valList.Add(new ValidationItem("Must have Comp & Collision", VehicleCoverageRadio));

                    if (vehicle.TapesAndRecordsLimitId == "219" && vehicle.SoundEquipmentLimit == "0" &&
                        bodyType_MotorCycle != vehicle.BodyTypeId && bodyType_PICKUPWCAMPER != vehicle.BodyTypeId &&
                        bodyType_PICKUPWOCAMPER != vehicle.BodyTypeId)
                    {
                        valList.Add(new ValidationItem("Sound Equipment must be 1,500 or above if Media is 400", VehicleCoverageRadio));
                    }

                    // Validate if body type is Motorcycle
                    if (bodyType_MotorCycle == vehicle.BodyTypeId && !String.IsNullOrEmpty(vehicle.CustomEquipmentAmount))
                    {
                        int motorEquip = 0;
                        int schedItemAmt = 0;

                        if (vehicle.CustomEquipmentAmount.Contains("$"))
                        {
                            if (qqHelper.IsPositiveIntegerString(vehicle.CustomEquipmentAmount))
                            {
                                motorEquip = IFM.Common.InputValidation.InputHelpers.TryToGetInt32(vehicle.CustomEquipmentAmount);

                                if (vehicle.ScheduledItems != null)
                                {
                                    foreach (QuickQuote.CommonObjects.QuickQuoteScheduledItem item in vehicle.ScheduledItems)
                                    {
                                        schedItemAmt = schedItemAmt + IFM.Common.InputValidation.InputHelpers.TryToGetInt32(item.Amount);
                                    }
                                }
                                if (schedItemAmt != motorEquip)
                                {
                                    valList.Add(new ValidationItem("Motorcycle Custom Equipment amount entered at quote does not match the itemized list of Custom Equipment. Quote rate will not be accurate. Edit scheduled equipment on the Application page and re-rate", VehicleCoverageCustEquip, true));
                                }
                            }
                            else
                                valList.Add(new ValidationItem("Whole Number Required", VehicleCoverageCustEquip));
                        }
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