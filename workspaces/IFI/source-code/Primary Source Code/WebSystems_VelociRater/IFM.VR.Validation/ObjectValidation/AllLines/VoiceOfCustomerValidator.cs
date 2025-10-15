namespace IFM.VR.Validation.ObjectValidation.AllLines
{
    public class ContactInformationValidator
    {
        public const string ValidationListID = "{8101CA3F-EC5D-4172-9579-91F1FA8A533E}";

        public const string PhoneAndEmailMissing = "{C6D4D5CC-DCC2-4F20-82EB-5B7AECDED221}";

        public const string EmailInvalid = "{4B806DDC-10EA-4AB5-8B13-B2D734CABE13}";

        public const string PhoneNumberInvalid = "{10F293F4-8735-4D44-A891-760D30F04A40}";
        public const string PhoneTypeInvalid = "{837D909D-32AF-41E1-85F2-1E340B39CFBF}";
        public const string PhoneExtensionInvalid = "{B04945DC-0B84-495A-A075-4E67239EC238}";

        public static Validation.ObjectValidation.ValidationItemList ValidateVoiceOfCustomer(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            if (valType != ValidationItem.ValidationType.quoteRate)
            {
                if (quote != null && quote.Policyholder != null)
                {
                    QuickQuote.CommonObjects.QuickQuotePolicyholder ph = quote.Policyholder;

                    bool emailIsMissing = false;
                    bool phoneIsMissing = false;

                    var emailValidator = IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.ValidateEmailList(ph.Emails, valType);
                    foreach (var err in emailValidator)
                    {
                        switch (err.FieldId)
                        {
                            case IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.EmailEmpty:
                            case IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.EmailListIsNull:
                                emailIsMissing = true;
                                break;

                            case IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.EmailInvalid:
                                err.FieldId = EmailInvalid;
                                valList.Add(err);
                                break;

                            default:
                                break;
                        }
                    }

                    var phoneValidator = IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.ValidatePhoneList(ph.Phones, valType);
                    foreach (var err in phoneValidator)
                    {
                        switch (err.FieldId)
                        {
                            case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneListIsNull:
                            case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneNumberEmpty:
                                phoneIsMissing = true;
                                break;

                            case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneNumberInvalid:
                                err.FieldId = PhoneNumberInvalid;
                                valList.Add(err);
                                break;

                            case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneTypeEmpty:
                            case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneTypeInvalid:
                                err.FieldId = PhoneTypeInvalid;
                                valList.Add(err);
                                break;

                            case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneExtensionInvalid:
                                err.FieldId = PhoneExtensionInvalid;
                                valList.Add(err);
                                break;
                        }
                    }

                    if (emailIsMissing && phoneIsMissing)
                    {
                        valList.Add(new ValidationItem("Phone or Email information required.", PhoneAndEmailMissing));
                    }
                }
            }

            return valList;
        }
    }
}