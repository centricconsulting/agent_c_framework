using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common
{
    public class EsignatureValidator
    {
        public const string ValidationListID = "{1A9878A8-C77E-4D79-88F4-38B7005514F1}";
        public const string EsignatureEmail = "{8940C3BD-68F9-4072-911E-1FEF7D22DB3E}";
        public const string EsignatureEmailIsEmpty = "{C9D48DD1-CDC9-4149-BB6E-78AEEDC29FF1}";
        public const string EsignatureNotSelected = "{61529EBD-7992-4141-A6F4-889E1C4E01F0}";


        public static Validation.ObjectValidation.ValidationItemList ValidateEsignature(System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteEmail> email, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            bool emailIsEmpty = false;
            var emailValidator = IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.ValidateEmailList(email, valType);
            foreach (var err in emailValidator)
            {
                switch (err.FieldId)
                {
                    case IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.EmailInvalid:
                        err.FieldId = EsignatureEmail;
                        valList.Add(err);
                        break;
                    case IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.EmailListIsNull:
                    case IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.EmailEmpty:
                        emailIsEmpty = true;
                        break;

                    default:
                        break;
                }
            }
            if (emailIsEmpty)
            {
                valList.Add(new ValidationItem("Missing email", EsignatureEmailIsEmpty));
            }
             
            return valList;
        }
    }
}