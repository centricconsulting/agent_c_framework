using QuickQuote.CommonObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA
{
    public class VehicleGaragingValidator
    {

        public const string ValidationListID = "{74BE5511-3636-4619-B1A1-5A1D464F5439}";

        public const string MainValidationText = "{5E15F8EC-E4F0-4736-B701-4987809EB4EB}";
        public const string StreetNumbeAndName = "{B020CDA1-70D9-48EC-B9E7-B97FE3A269F0}";
        public const string HouseNumberMissing = "{058CDBF0-1D62-4F60-9500-4BBE8B9F24FE}";
        public const string StreetNameMissing = "{040EBE9A-23CF-4A99-B773-635596A3662E}";
        public const string ZipCodeIsMissing = "{077E4B54-BF0D-47D9-9E99-68BCA937C9FE}";
        public const string CityIsMissing = "{2154C1EA-AA7A-4D20-A5A7-DE8B9AB6A53D}";
        public const string StateIsMissing = "{24512FB2-B5EC-4559-92C5-88125AD18085}";
        public const string CountyIsMissing = "{1564F4C5-1252-40EB-951F-6817FA1E46CF}";

        public static Validation.ObjectValidation.ValidationItemList ValidateVehicleAddress(int vehicleIndex, QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.VehicleIndex, vehicleIndex.ToString());

            if (quote != null && quote.Vehicles != null && quote.Vehicles.Count > vehicleIndex)
            {
                QuickQuote.CommonObjects.QuickQuoteVehicle vehicle = quote.Vehicles[vehicleIndex];
                if (vehicle != null)
                {
                    bool POBoxHasValue = false;

                    if (!string.IsNullOrWhiteSpace(quote.Policyholder.Address.POBox))
                    {
                        POBoxHasValue = true;
                    }
                    if (POBoxHasValue)
                    {
                        VRGeneralValidations.Val_HasRequiredField(vehicle.GaragingAddress.Address.HouseNum, valList, HouseNumberMissing, "Street Number");
                        VRGeneralValidations.Val_HasRequiredField(vehicle.GaragingAddress.Address.StreetName, valList, StreetNameMissing, "Street Name");
                        VRGeneralValidations.Val_IsValidZipCode(vehicle.GaragingAddress.Address.Zip, valList, ZipCodeIsMissing, "Zip Code");
                        VRGeneralValidations.Val_HasRequiredField(vehicle.GaragingAddress.Address.City, valList, CityIsMissing, "City");
                        VRGeneralValidations.Val_HasRequiredField_DD(vehicle.GaragingAddress.Address.StateId, valList, StateIsMissing, "State");
                        VRGeneralValidations.Val_HasRequiredField(vehicle.GaragingAddress.Address.County, valList, CountyIsMissing, "County");
                    }
                }
            }
            return valList;
        }
    }
}