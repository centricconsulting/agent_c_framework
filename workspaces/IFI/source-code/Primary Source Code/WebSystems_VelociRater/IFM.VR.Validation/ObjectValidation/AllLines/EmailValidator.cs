using System.Collections.Generic;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.AllLines
{
    public class EmailValidator
    {
        public const string ValidationListID = "{39D5BE5E-6010-42BF-A1BE-9A97E7CF141F}";

        public const string EmailListIsNull = "{D570E807-CE88-4530-A103-9B695CEB5941}";

        public const string EmailEmpty = "{0FCC2FE7-2699-4112-9EBA-44150535B95F}";
        public const string EmailInvalid = "{867F2D65-3DE2-42C6-B308-57B561CAB222}";

        public static Validation.ObjectValidation.ValidationItemList ValidateEmailList(List<QuickQuote.CommonObjects.QuickQuoteEmail> Emails, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            string email = "";
            if (Emails != null)
            {
                if (Emails != null && Emails.Any() && Emails[0] != null)
                    email = Emails[0].Address;
                if (VRGeneralValidations.Val_HasRequiredField(email, valList, EmailEmpty, "Email"))
                    VRGeneralValidations.Val_IsValidEmailAddress(email, valList, EmailInvalid, "Email");
            }
            else
            {
                valList.Add(new ValidationItem("Email List is null", EmailListIsNull, false));
            }

            return valList;
        }
    }
}