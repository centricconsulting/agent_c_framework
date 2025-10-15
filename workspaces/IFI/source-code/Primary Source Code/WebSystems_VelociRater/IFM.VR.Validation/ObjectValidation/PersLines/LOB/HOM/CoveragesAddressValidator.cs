namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class CoveragesAddressValidator
    {
        public const string ValidationListID = "{CBC83080-C6F2-4AFA-8B1C-AA3266AEC49C}";

        public const string StreetAndPoBoxEmpty = "{F018EB89-8D9A-4BE5-B475-318265E214EC}";
        public const string StreetAndPoxBoxAreSet = "{378DB572-EF07-4C24-B35E-B0FB91E80F11}";
        public const string AddressIsEmpty = "{667DA4AC-A665-423E-983B-5D565C19C2E4}";
        public const string HouseNumberID = "{A4B40E53-6A2F-4409-8100-6DEC76631AC4}";

        public const string StreetNameID = "{07C053A4-AAD5-484F-AEA3-FC1597E0D53E}";
        public const string ZipCodeID = "{34735D81-6DB4-4AC7-9576-8933C527374C}";
        public const string CityID = "{2F040D76-BC39-4AE0-A02D-2CC4C547E265}";
        public const string StateID = "{AF34D391-9472-4740-8C08-EA99699C006C}";
        public const string CountyID = "{1AD713D2-E194-4F12-B6D3-2D870FA0E880}";

        public static Validation.ObjectValidation.ValidationItemList AddressValidation(QuickQuote.CommonObjects.QuickQuoteAddress Address, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            if (Address != null)
            {
                if (VRGeneralValidations.Val_HasRequiredField(Address.HouseNum, valList, HouseNumberID, "Street Number"))
                    VRGeneralValidations.Val_IsValidAlphaNumeric(Address.HouseNum, valList, HouseNumberID, "Street Number");

                VRGeneralValidations.Val_HasRequiredField(Address.StreetName, valList, StreetNameID, "Street Name");

                if (VRGeneralValidations.Val_HasRequiredField(Address.Zip, valList, ZipCodeID, "Zip Code"))
                    VRGeneralValidations.Val_IsValidZipCode(Address.Zip, valList, ZipCodeID, "Zip Code");

                VRGeneralValidations.Val_HasRequiredField(Address.City, valList, CityID, "City");

                if (VRGeneralValidations.Val_HasRequiredField_DD(Address.StateId, valList, StateID, "State"))
                    VRGeneralValidations.Val_IsNonNegativeWholeNumber(Address.StateId, valList, StateID, "State");

                VRGeneralValidations.Val_HasRequiredField(Address.County, valList, CountyID, "County");
            }
            else
            {
                valList.Add(new ObjectValidation.ValidationItem("Address Information is null", AddressIsEmpty));
            }

            return valList;
        }
    }
}