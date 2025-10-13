using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFM.VR.Validation.ObjectValidation.CommLines.LOB.CAP
{
    public class VINValidator
    {

        //Added class 07/08/2021 for CAP Endorsements Tasks 53028 and 53030 MLW

        public const string ValidationListID = "{89ED1441-081A-46DA-9F4F-93038843EAAB}";
        public const string VehicleVIN = "{A4DEEFE6-B690-4978-B7D5-41BDB6FC53C8}";
        public const string VehicleYear = "{367298C8-17CA-4A02-B1F3-54B498DA1B78}";

        public static Validation.ObjectValidation.ValidationItemList VINValidation(int vehicleIndex, QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.VehicleIndex, vehicleIndex.ToString());

            if (quote != null && quote.Vehicles != null && quote.Vehicles.Count > vehicleIndex)
            {
                QuickQuote.CommonObjects.QuickQuoteVehicle vehicle = quote.Vehicles[vehicleIndex];
                if (vehicle != null)
                {
                    if (quote.QuoteTransactionType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                    {
                        valType = ValidationItem.ValidationType.endorsement;
                    }

                    if (valType != ValidationItem.ValidationType.quoteRate)
                    {
                        VRGeneralValidations.Val_HasRequiredField(vehicle.Vin, valList, VehicleVIN, "VIN");
                    }

                    
                    if (vehicle.Vin != "")
                    {
                        bool is1981OrNewer = VRGeneralValidations.Val_IsNumberInRange(vehicle.Year, valList, VehicleYear, "Year", "1981", (IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate().Year + 1).ToString(), true);
                        switch (valType)
                        {
                            case ValidationItem.ValidationType.quoteRate:                               
                            case ValidationItem.ValidationType.endorsement:
                                //if (IFM.Common.InputValidation.CommonValidations.IsTextLenghtInRange(vehicle.Vin, 8, 17) == false)
                                if (is1981OrNewer && IFM.Common.InputValidation.CommonValidations.IsTextLenghtInRange(vehicle.Vin, 8, 17) == false) 
                                {
                                    //newer vehicles must have 8-17 character VINs
                                    valList.Add(new ValidationItem("Invalid VIN Length", VehicleVIN, false));
                                } else if (!is1981OrNewer && IFM.Common.InputValidation.CommonValidations.IsTextLenghtInRange(vehicle.Vin, 5, 17) == false)
                                {
                                    //older vehicles must have 5-17 character VINs
                                    valList.Add(new ValidationItem("Invalid VIN Length", VehicleVIN, false));
                                }
                                else if (IFM.Common.InputValidation.CommonValidations.IsAlphaNum(vehicle.Vin) == false)
                                {
                                    valList.Add(new ValidationItem("Invalid VIN", VehicleVIN, false));
                                }
                                break;
                            default:
                                //Updated 8/17/2022 for task 73951 MLW
                                if (is1981OrNewer && IFM.Common.InputValidation.CommonValidations.IsTextLenghtInRange(vehicle.Vin, 17, 17) == false)
                                {
                                    valList.Add(new ValidationItem("Invalid VIN Length", VehicleVIN, false));
                                }
                                else if (!is1981OrNewer && IFM.Common.InputValidation.CommonValidations.IsTextLenghtInRange(vehicle.Vin, 5, 17) == false)
                                {
                                    //older vehicles must have 5-17 character VINs
                                    valList.Add(new ValidationItem("Invalid VIN Length", VehicleVIN, false));
                                }
                                else if (IFM.Common.InputValidation.CommonValidations.IsAlphaNum(vehicle.Vin) == false)
                                {
                                    valList.Add(new ValidationItem("Invalid VIN", VehicleVIN, false));
                                }
                                break;
                        }
                    }                    
                }
            }
            return valList;
        }
    }
}