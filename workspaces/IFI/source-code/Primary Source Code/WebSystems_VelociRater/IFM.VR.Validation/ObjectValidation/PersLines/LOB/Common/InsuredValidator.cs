using IFM.VR.Common.Helpers;
using IFM.VR.Common.Helpers.AllLines;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common
{
    public static class InsuredValidator
    {
        public const string ValidationListID = "{1A9878A8-C77E-4D79-88F4-38B7005514F1}";

        public const string QuoteIsNull = "{228B1470-0C5B-40DC-BADE-86ADC364AD3F}";
        public const string PolicyHolderIsNull = "{8A27EF70-06CD-44F1-AFF6-F6D9906F4253}";
        public const string PolicyHolderNameIsNull = "{A1925D1B-2BC7-446D-B843-F6E23BAD270F}";

        public const string CommAndPersNameComponentsEmpty = "{D354F1CD-1BCD-47A6-8F94-099690DCB298}";
        public const string CommAndPersNameComponentsAllSet = "{4FEC641E-E536-4026-BABE-C6A67CAD89F8}";

        public const string EntityTypeId = "{8A07D905-1190-40DD-982C-7D81C8A7E019}"; // also known as Business Type - corporation or partnership
        public const string OtherEntityType = "{EB58C023-6C73-4BA1-9D6D-1E22D71640DE}"; //Added 2/14/2022 for bug 63511 MLW

        public const string CommercialName = "{374540BE-1F20-4002-982A-85772D753BE2}";

        public const string PolicyHolderFirstNameID = "{5F042C74-612C-4C73-AAEC-D4FAD55C7A54}";
        public const string PolicyHolderLastNameID = "{C237BF60-8757-4F65-AF39-6D04201C88FD}";
        public const string PolicyHolderGenderID = "{F0EB0F41-A76E-4710-841E-A45640382296}";
        public const string PolicyHolderSSNID = "{0B40E690-F240-47B7-818D-F68D827BF911}";
        public const string PolicyHolderFEINID = "{3F4E6389-6A60-486A-9211-E74A6E99091B}";
        public const string PolicyHolderBirthDate = "{EE4404D9-2BA7-4661-B6DE-D17784F15440}";
        public const string PolicyHolderEmail = "{E08A5DC0-70CA-48D9-BCC2-DF4DF765720A}";

        public const string PolicyHolderPhoneNumber = "{F4BBF3F0-3F35-4C66-8CF3-C5229C6FD5D9}";
        public const string PolicyHolderPhoneExtension = "{C673FDAC-296E-46ED-8133-AFAD7CA6D1A7}";
        public const string PolicyHolderPhoneType = "{92C579A1-BE06-4DE0-BECE-41BA527A6525}";

        public const string PolicyHolderAddressIsEmpty = "{A84CFED1-5CF9-4C78-B9CE-9AC3FEBECB09}";
        public const string PolicyHolderStreetAndPoBoxEmpty = "{7299315F-87F5-4500-B4F5-144801E3BD37}";
        public const string PolicyHolderStreetAndPoxBoxAreSet = "{693E46EF-0BFD-4B5C-9005-4B96009EFD2E}";
        public const string PolicyHolderHouseNumberID = "{214CD52A-542B-436E-9DA4-1215669AD92D}";
        public const string PolicyHolderStreetNameID = "{5A84B6D6-9F95-4D92-A08D-E1BF88D8AA95}";
        public const string PolicyHolderPOBOXID = "{E5DCCF9B-F6E8-4BBB-8C9C-974B4D3DB0E2}";
        public const string PolicyHolderZipCodeID = "{BB6F3999-9375-4405-8D7C-814755453AFD}";
        public const string PolicyHolderCityID = "{8B0D1E96-E1D0-43FC-8F62-81AF1C19FEAE}";
        public const string PolicyHolderStateID = "{3CBD16C5-0BD7-4331-86F7-87F9027E2BF2}";
        public const string PolicyHolderCountyID = "{93E18B34-1F11-430D-8B33-C1911C25D639}";
        public const string PolicyHolderOtherEmpty = "{1443467D-28F3-4CE2-8C59-3FCF6203167B}";


        public const string PolicyHolderEmailAndPhoneIsEmpty = "{C5CC91E9-504A-4BEF-A7C1-B2A013B4CAAF}";

        public const string PolicyHolderBusinessStartedDate = "{22383A0A-9F8B-4230-820F-58AF3764AAF5}";
        public const string PolicyHolderYearsOfExperience = "{CE90CB68-B547-454E-9004-A2979F871B41}";
        public const string PolicyHolderDescriptionOfOperations = "{23b8e781-a6f4-473b-9aa0-9343fb3213f2}";

        public static Validation.ObjectValidation.ValidationItemList ValidateInsured(int insuredIndex, QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.PolicyholderIndex, insuredIndex.ToString());

            if (quote != null)
            {
                if (quote.QuoteTransactionType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                {
                    valType = ValidationItem.ValidationType.endorsement;
                }

                QuickQuote.CommonObjects.QuickQuotePolicyholder ph = null;
                if (insuredIndex == 0)
                {
                    ph = quote.Policyholder;
                }
                else
                {
                    ph = quote.Policyholder2;
                    try
                    {
                        if (string.IsNullOrWhiteSpace(ph.Name.FirstName) && string.IsNullOrWhiteSpace(ph.Name.LastName))
                            return valList; // PH#2 is most likely empty which is fine
                    }
                    catch
                    {
                        return valList; // PH#2 is most likely empty which is fine
                    }
                }

                if (ph != null)
                {
                    if (ph.Name != null)
                    {
                        var nameValidations = IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.ValidateNameObject(ph.Name, valType, quote);
                        foreach (var err in nameValidations)
                        {
                            switch (err.FieldId)
                            {
                                case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.CommAndPersNameComponentsEmpty:
                                    err.FieldId = CommAndPersNameComponentsEmpty;
                                    valList.Add(err);
                                    break;

                                case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.CommAndPersNameComponentsAllSet:
                                    err.FieldId = CommAndPersNameComponentsAllSet;
                                    valList.Add(err);
                                    break;

                                case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.CommercialName:
                                    err.FieldId = CommercialName;
                                    valList.Add(err);
                                    break;

                                case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.FirstNameID:
                                    err.FieldId = PolicyHolderFirstNameID;
                                    valList.Add(err);
                                    break;

                                case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.LastNameID:
                                    err.FieldId = PolicyHolderLastNameID;
                                    valList.Add(err);
                                    break;

                                case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.GenderID:
                                    err.FieldId = PolicyHolderGenderID;
                                    valList.Add(err);
                                    break;

                                case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.SSNID:
                                    err.FieldId = PolicyHolderSSNID;
                                    valList.Add(err);
                                    break;

                                case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.FEINID:
                                    err.FieldId = PolicyHolderFEINID;
                                    valList.Add(err);
                                    break;

                                case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.BirthDate:
                                    if (valType != ValidationItem.ValidationType.endorsement)
                                    {
                                        err.FieldId = PolicyHolderBirthDate;
                                        valList.Add(err);
                                    }
                                    break;

                                case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.EntityTypeId:
                                    if (valType != ValidationItem.ValidationType.endorsement)
                                    {
                                        err.FieldId = EntityTypeId;
                                        valList.Add(err);
                                    }
                                    break;
                                case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.OtherEntityType:
                                    //Added 2/14/2022 for bug 63511 MLW
                                    if (valType != ValidationItem.ValidationType.endorsement)
                                    {
                                        err.FieldId = OtherEntityType;
                                        valList.Add(err);
                                    }
                                    break;

                                case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.BusinessStartedDate:
                                    err.FieldId = PolicyHolderBusinessStartedDate;
                                    valList.Add(err);
                                    break;
                                case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.YearsOfExperience:
                                    err.FieldId = PolicyHolderYearsOfExperience;
                                    valList.Add(err);
                                    break;
                                case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.DescriptionOfOperations:
                                    err.FieldId = PolicyHolderDescriptionOfOperations;
                                    valList.Add(err);
                                    break;
                            }
                        }
                        bool emailIsEmpty = false;
                        IFM.VR.Validation.ObjectValidation.ValidationItemList emailValidator = null;
                        if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                        {
                            emailValidator = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.HOMEmailValidator.ValidateHOMPolicyholderEmails(quote, valType);
                        }
                        else
                        {
                            emailValidator = AllLines.EmailValidator.ValidateEmailList(ph.Emails, valType);
                        }
                        foreach (var err in emailValidator)
                        {
                            switch (err.FieldId)
                            {
                                case IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.EmailInvalid:
                                case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.HOMEmailValidator.EmailInvalid:
                                    err.FieldId = PolicyHolderEmail;
                                    valList.Add(err);
                                    break;

                                case IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.EmailListIsNull:
                                case IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.EmailEmpty:
                                case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.HOMEmailValidator.EmailListIsNull:
                                case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.HOMEmailValidator.EmailEmpty:
                                    emailIsEmpty = true;
                                    break;
                                case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.HOMEmailValidator.EmailRequiredWithWoodburningStove:
                                    err.FieldId = PolicyHolderEmail;
                                    valList.Add(err);
                                    break;
                                default:
                                    break;
                            }
                        }
                        bool phoneIsEmpty = false;
                        var phoneValidator = IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.ValidatePhoneList(ph.Phones, valType);
                        foreach (var err in phoneValidator)
                        {
                            switch (err.FieldId)
                            {
                                case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneNumberInvalid:
                                    err.FieldId = PolicyHolderPhoneNumber;
                                    valList.Add(err);
                                    break;

                                case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneListIsNull:
                                case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneNumberEmpty:
                                    phoneIsEmpty = true;
                                    break;

                                case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneTypeEmpty:
                                case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneTypeInvalid:
                                    err.FieldId = PolicyHolderPhoneType;
                                    valList.Add(err);
                                    break;

                                case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneExtensionInvalid:
                                    err.FieldId = PolicyHolderPhoneExtension;
                                    valList.Add(err);
                                    break;
                            }
                        }
                        //if (valType != ValidationItem.ValidationType.quoteRate) // added for Voice of Customer
                        //{
                        //    if (emailIsEmpty && phoneIsEmpty)
                        //    {
                        //        valList.Add(new ValidationItem("Missing email or phone/phone type", PolicyHolderEmailAndPhoneIsEmpty));
                        //    }
                        //}

                        // 59591 - CAH 02/02/2021 - No phone or email required for endo
                        if (valType != ValidationItem.ValidationType.endorsement)

                        {
                            // 41379: CAH 12/3/2019 
                            if (insuredIndex == 0 && valType != ValidationItem.ValidationType.quoteRate) // added for Voice of Customer
                            {
                                if (IFM.VR.Common.Helpers.AllLines.RequiredEmailHelper.IsRequiredEmailAvailable(quote) == true)
                                {
                                    if (emailIsEmpty && hasMissingEmailCheck(quote?.Policyholder2?.Emails, valType))
                                    {
                                        valList.Add(new ValidationItem("Valid email required to issue.", PolicyHolderEmailAndPhoneIsEmpty));
                                    }
                                }
                                else
                                {
                                    if (emailIsEmpty && phoneIsEmpty)
                                    {
                                        if (hasMissingEmailCheck(quote?.Policyholder2?.Emails, valType) && hasMissingPhoneCheck(quote?.Policyholder2?.Phones, valType))
                                        {
                                            valList.Add(new ValidationItem("Missing email or phone/phone type", PolicyHolderEmailAndPhoneIsEmpty));
                                        }
                                    }

                                }
                            };
                        }

                        //check address but only for ph#1
                        if (insuredIndex == 0)
                        {
                            // For FARM endorsements, county is not required  Bug 62408
                            bool CountyRequired = true;
                            if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                            {
                                if (quote.IsBillingEndorsement() || quote.IsNonBillingEndorsement())
                                {
                                    CountyRequired = false;
                                }
                            }

                            // Added the county required parameter per Bug 62408
                            var addressValidations = AllLines.AddressValidator.AddressValidation(ph.Address, valType, false, false, null, CountyRequired);
                            //var addressValidations = AllLines.AddressValidator.AddressValidation(ph.Address, valType);
                            foreach (var err in addressValidations)
                            {
                                //convert to address for this object
                                switch (err.FieldId)
                                {
                                    case AllLines.AddressValidator.StreetAndPoBoxEmpty:
                                        err.FieldId = PolicyHolderStreetAndPoBoxEmpty;
                                        valList.Add(err);
                                        break;

                                    case AllLines.AddressValidator.StreetAndPoxBoxAreSet:
                                        err.FieldId = PolicyHolderStreetAndPoxBoxAreSet;
                                        valList.Add(err);
                                        break;

                                    case AllLines.AddressValidator.HouseNumberID:
                                        err.FieldId = PolicyHolderHouseNumberID;
                                        valList.Add(err); // add validation item to current validation list
                                        break;

                                    case AllLines.AddressValidator.StreetNameID:
                                        err.FieldId = PolicyHolderStreetNameID;
                                        valList.Add(err); // add validation item to current validation list
                                        break;

                                    case AllLines.AddressValidator.POBOXID:
                                        err.FieldId = PolicyHolderPOBOXID;
                                        valList.Add(err); // add validation item to current validation list
                                        break;

                                    case AllLines.AddressValidator.ZipCodeID:
                                        err.FieldId = PolicyHolderZipCodeID;
                                        valList.Add(err); // add validation item to current validation list
                                        break;

                                    case AllLines.AddressValidator.CityID:
                                        err.FieldId = PolicyHolderCityID;
                                        valList.Add(err); // add validation item to current validation list
                                        break;

                                    case AllLines.AddressValidator.StateID:
                                        err.FieldId = PolicyHolderStateID;
                                        valList.Add(err); // add validation item to current validation list
                                        break;

                                    case AllLines.AddressValidator.CountyID:
                                        err.FieldId = PolicyHolderCountyID;
                                        valList.Add(err); // add validation item to current validation list
                                        break;

                                    case AllLines.AddressValidator.AddressIsEmpty:
                                        err.FieldId = PolicyHolderAddressIsEmpty;
                                        valList.Add(err);
                                        break;

                                    case AllLines.AddressValidator.OtherEmpty:
                                        err.FieldId = PolicyHolderOtherEmpty;
                                        valList.Add(err); // add validation item to current validation list
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        valList.Add(new ValidationItem("Policyholder Name is null", PolicyHolderNameIsNull, false));
                    }
                }
                else
                {
                    valList.Add(new ValidationItem("Policyholder is null", PolicyHolderIsNull, false));
                }
            }
            else
            {
                valList.Add(new ValidationItem("Quote is null", QuoteIsNull, false));
            }
            return valList;
        }

        static bool hasMissingEmailCheck(System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteEmail> Emails, ValidationItem.ValidationType valType)
        {
            if (Emails != null)
            {
                var emailValidator = IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.ValidateEmailList(Emails, valType);
                foreach (var err in emailValidator)
                {
                    switch (err.FieldId)
                    {
                        case IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.EmailListIsNull:
                        case IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.EmailEmpty:
                            return true;
                    }
                }
            }
            else
            {
                return true;
            }
            return false;
        }
        static bool hasMissingPhoneCheck(System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuotePhone> Phones, ValidationItem.ValidationType valType)
        {
            if (Phones != null)
            {
                var phoneValidator = IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.ValidatePhoneList(Phones, valType);
                foreach (var err in phoneValidator)
                {
                    switch (err.FieldId)
                    {
                        case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneListIsNull:
                        case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneNumberEmpty:
                            return true;
                    }
                }
            }
            else
            {
                return true;
            }
            return false;
        }
    }
}