using System.Collections.Generic;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.AllLines
{
    public class PhonesValidator
    {
        public const string ValidationListID = "{927EFBDE-EBA3-46E4-A362-C3667277D76B}";

        public const string PhoneListIsNull = "{C265F960-7836-46FF-87A7-F3ABE31DEBFE}";

        public const string PhoneNumberEmpty = "{74016C51-8C58-4158-B063-F31249C50BC5}";
        public const string PhoneNumberInvalid = "{3468E59E-199E-44ED-949C-07565660E187}";

        public const string PhoneTypeEmpty = "{FC14B3ED-5C16-4304-8C0B-90F0D801D2BD}";
        public const string PhoneTypeInvalid = "{9FD33906-9D02-4D02-B3C9-B616544C139E}";

        public const string PhoneExtensionEmpty = "{CC3CA8F8-1161-4665-8BC9-A3FF244E14B1}";
        public const string PhoneExtensionInvalid = "{E49894C7-6EF8-4D4D-9B98-9AA019AC10D7}";

        public static Validation.ObjectValidation.ValidationItemList ValidatePhoneList(List<QuickQuote.CommonObjects.QuickQuotePhone> phones, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            string phoneNumber = "";
            string phoneExtension = "";
            string phoneType = "";
            if (phones != null)
            {
                if (phones.Any() && phones[0] != null)
                {
                    phoneNumber = phones[0].Number;
                    phoneExtension = phones[0].Extension;
                    phoneType = phones[0].TypeId;
                }

                if (VRGeneralValidations.Val_HasRequiredField(phoneNumber, valList, PhoneNumberEmpty, "Phone Number"))
                {
                    // has phone number is it valid and do you have a phone type
                    VRGeneralValidations.Val_IsValidPhoneNumber(phoneNumber, valList, PhoneNumberInvalid, "Phone Number");

                    if (VRGeneralValidations.Val_HasRequiredField_DD(phoneType, valList, PhoneTypeEmpty, "Phone Type"))
                        VRGeneralValidations.Val_IsNonNegativeWholeNumber(phoneType, valList, PhoneTypeInvalid, "Phone Type");

                    if (VRGeneralValidations.Val_HasRequiredField(phoneExtension, valList, PhoneExtensionEmpty, "Phone Number Extension"))
                        VRGeneralValidations.Val_IsNonNegativeWholeNumber(phoneExtension, valList, PhoneExtensionInvalid, "Phone Number Extension");
                }
                else
                {
                    // no phone number so should not have type or extension
                    VRGeneralValidations.Val_HasIneligibleField(phoneType, valList, PhoneTypeInvalid, "Phone Type");
                    VRGeneralValidations.Val_HasIneligibleField_Int(phoneExtension, valList, PhoneExtensionInvalid, "Phone Number Extension");
                }
            }
            else
            {
                valList.Add(new ValidationItem("Phone list is null", PhoneListIsNull, false));
            }

            return valList;
        }
    }
}