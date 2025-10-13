using System;

namespace IFM.VR.Validation.ObjectValidation.FarmLines
{
    public class ResidentNameValidator
    {
        public const string ValidationListID = "{3c5fb253-66bd-4f78-91e3-1dd7ac571e56}";

        public const string QuoteIsNull = "{670bcf44-2d1c-4a84-b6c8-4245a0c6f5d8}";
        public const string LocationIsNull = "{6523d2eb-df34-413b-b370-9358496bd9d1}";

        public const string InvalidLocationIndex = "{da7e075a-a8bd-4679-b29b-86c6609b01f6}";

        public const string InvalidResidentIndex = "{77c37d26-6f7a-41f2-8c0c-74b06dbffe9a}";

        public const string Resident_FirstName = "{78a56aaf-9712-433c-96e2-77fc92bffbb5}";
        public const string Resident_LastName = "{c2cb1aac-e814-41b5-ac59-e9d14d06b444}";
        public const string Resident_DOB = "{d745e810-5ac1-45fb-9061-9599892fcb0c}";
        public const string Resident_Relationship = "{fb748498-ad9a-45bc-91c2-83e68121cf3d}";

        public static Validation.ObjectValidation.ValidationItemList ValidateResidentName(QuickQuote.CommonObjects.QuickQuoteObject quote, Int32 locationIndex, Int32 residentIndex, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            if (quote != null)
            {
                if (quote.Locations != null)
                {
                    if (quote.Locations.Count > locationIndex)
                    {
                        if (quote.Locations[locationIndex].ResidentNames != null)
                        {
                            if (quote.Locations[locationIndex].ResidentNames.Count > residentIndex)
                            {
                                // now do checks
                                var resName = quote.Locations[locationIndex].ResidentNames[residentIndex];
                                if (resName != null)
                                {
                                    VRGeneralValidations.Val_HasRequiredField(resName.Name.FirstName, valList, Resident_FirstName, "First Name");
                                    VRGeneralValidations.Val_HasRequiredField(resName.Name.LastName, valList, Resident_LastName, "Last Name");
                                    if (VRGeneralValidations.Val_HasRequiredField(resName.Name.BirthDate, valList, Resident_DOB, "DOB"))
                                    {
                                        VRGeneralValidations.Val_IsValidDate(resName.Name.BirthDate, valList, Resident_DOB, "DOB");
                                    }
                                    VRGeneralValidations.Val_HasRequiredField(resName.Name.Salutation, valList, Resident_Relationship, "Relationship");
                                }
                                else
                                {
                                    valList.Add(new ObjectValidation.ValidationItem("Null resident.", InvalidResidentIndex));
                                }
                            }
                            else
                            {
                                valList.Add(new ObjectValidation.ValidationItem("Invalid resident name index.", InvalidResidentIndex));
                            }
                        }
                    }
                    else
                    {
                        valList.Add(new ObjectValidation.ValidationItem("Invalid location index.", InvalidLocationIndex));
                    }
                }
                else
                {
                    valList.Add(new ObjectValidation.ValidationItem("Location is null", LocationIsNull));
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