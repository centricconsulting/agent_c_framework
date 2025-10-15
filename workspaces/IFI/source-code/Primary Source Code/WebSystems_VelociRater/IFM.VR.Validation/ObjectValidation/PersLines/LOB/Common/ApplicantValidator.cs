namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common
{
    public class ApplicantValidator
    {
        public const string ValidationListID = "{4F3975A0-5BC2-44BA-8FE7-81DB3AA80677}";

        public const string ApplicantIsNull = "{0B2C3032-E9FE-4226-BB5D-5A2546BE3BE1}";
        public const string ApplicantNameIsNull = "{BFE08DFB-0F26-4EAA-8CFD-41616C86FBE6}";

        public const string CommAndPersNameComponentsEmpty = "{9A6734B5-2D35-4CBD-B6B7-5A23B01CD978}";
        public const string CommAndPersNameComponentsAllSet = "{305CDD1D-C4B5-46C9-8349-D75305D16689}";

        public const string CommercialName = "{F9A7A251-CB98-4264-AD40-6109DA8B29E8}";

        public const string ApplicantFirstNameID = "{184AB5B1-A938-400D-BF6A-280A504426C3}";
        public const string ApplicantLastNameID = "{0D72DAF3-6910-43F3-BCCD-FFBFBA264D8A}";
        public const string ApplicantGenderID = "{BEB1F86D-B932-4192-B1DE-1FD064536CA9}";
        public const string ApplicantSSNID = "{61C612D5-CB90-4946-863B-0A857F4AB289}";
        public const string ApplicantBirthDate = "{4E504E49-21EB-4D32-BEDA-BF065F23A59F}";
        public const string ApplicantEmail = "{E1B44DCE-7EAE-4F4F-9408-D480CD703D57}";

        public const string ApplicantPhoneNumber = "{2504AA83-FF50-4E0A-82BF-0BC7E5395BD6}";
        public const string ApplicantPhoneExtension = "{ABEABFD8-81BA-410B-905A-DBF3EC334DBA}";
        public const string ApplicantPhoneType = "{5F95D955-E81B-44F5-887C-47FDFE3E4A48}";

        public const string ApplicantAddressIsEmpty = "{95A9D4BD-7BF7-46CC-A5D1-6DEB7A2C75B3}";
        public const string ApplicantStreetAndPoBoxEmpty = "{15FEDDEF-DA87-48ED-B389-87EC67E28948}";
        public const string ApplicantStreetAndPoxBoxAreSet = "{BC049A2B-3B78-4C6C-BD48-08E04F69DC7B}";
        public const string ApplicantHouseNumberID = "{06870371-924A-43E5-B572-772E50FB4C60}";
        public const string ApplicantStreetNameID = "{F2E47E1A-BDE4-4C17-AB1B-E76D36EB5B6C}";
        public const string ApplicantPOBOXID = "{9025E98B-A3E7-4596-9647-FB231A3790A0}";
        public const string ApplicantZipCodeID = "{922347ED-5298-44CF-9317-BDFA01662AD2}";
        public const string ApplicantCityID = "{6E6CED22-27FC-454E-BBB3-EC451657904B}";
        public const string ApplicantStateID = "{FF1357F7-765F-439F-81D5-433F4BB8FF93}";
        public const string ApplicantCountyID = "{1DACB25D-09FB-4F01-8210-A845CC160DC8}";

        public static Validation.ObjectValidation.ValidationItemList ValidateApplicant(QuickQuote.CommonObjects.QuickQuoteApplicant applicant, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (applicant != null)
            {
                if (applicant.Name != null)
                {
                    var nameValidations = IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.ValidateNameObject(applicant.Name, valType);
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
                                err.FieldId = ApplicantFirstNameID;
                                valList.Add(err);
                                break;

                            case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.LastNameID:
                                err.FieldId = ApplicantLastNameID;
                                valList.Add(err);
                                break;

                            case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.GenderID:
                                err.FieldId = ApplicantGenderID;
                                valList.Add(err);
                                break;

                            case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.SSNID:
                                err.FieldId = ApplicantSSNID;
                                valList.Add(err);
                                break;

                            case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.BirthDate:
                                err.FieldId = ApplicantBirthDate;
                                valList.Add(err);
                                break;
                        }
                    }

                    var emailValidator = IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.ValidateEmailList(applicant.Emails, valType);
                    foreach (var err in emailValidator)
                    {
                        switch (err.FieldId)
                        {
                            case IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.EmailInvalid:
                                err.FieldId = ApplicantEmail;
                                valList.Add(err);
                                break;

                            case IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.EmailEmpty:
                                // not required so don't care
                                break;

                            default:
                                break;
                        }
                    }

                    var phoneValidator = IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.ValidatePhoneList(applicant.Phones, valType);
                    foreach (var err in phoneValidator)
                    {
                        switch (err.FieldId)
                        {
                            case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneNumberInvalid:
                                err.FieldId = ApplicantPhoneNumber;
                                valList.Add(err);
                                break;

                            case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneTypeEmpty:
                            case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneTypeInvalid:
                                err.FieldId = ApplicantPhoneType;
                                valList.Add(err);
                                break;

                            case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneExtensionInvalid:
                                err.FieldId = ApplicantPhoneExtension;
                                valList.Add(err);
                                break;
                        }
                    }

                    var addressValidations = AllLines.AddressValidator.AddressValidation(applicant.Address, valType);
                    foreach (var err in addressValidations)
                    {
                        //convert to address for this object
                        switch (err.FieldId)
                        {
                            case AllLines.AddressValidator.StreetAndPoBoxEmpty:
                                err.FieldId = ApplicantStreetAndPoBoxEmpty;
                                valList.Add(err);
                                break;

                            case AllLines.AddressValidator.StreetAndPoxBoxAreSet:
                                err.FieldId = ApplicantStreetAndPoxBoxAreSet;
                                valList.Add(err);
                                break;

                            case AllLines.AddressValidator.HouseNumberID:
                                err.FieldId = ApplicantHouseNumberID;
                                valList.Add(err); // add validation item to current validation list
                                break;

                            case AllLines.AddressValidator.StreetNameID:
                                err.FieldId = ApplicantStreetNameID;
                                valList.Add(err); // add validation item to current validation list
                                break;

                            case AllLines.AddressValidator.POBOXID:
                                err.FieldId = ApplicantPOBOXID;
                                valList.Add(err); // add validation item to current validation list
                                break;

                            case AllLines.AddressValidator.ZipCodeID:
                                err.FieldId = ApplicantZipCodeID;
                                valList.Add(err); // add validation item to current validation list
                                break;

                            case AllLines.AddressValidator.CityID:
                                err.FieldId = ApplicantCityID;
                                valList.Add(err); // add validation item to current validation list
                                break;

                            case AllLines.AddressValidator.StateID:
                                err.FieldId = ApplicantStateID;
                                valList.Add(err); // add validation item to current validation list
                                break;

                            case AllLines.AddressValidator.CountyID:
                                err.FieldId = ApplicantCountyID;
                                valList.Add(err); // add validation item to current validation list
                                break;

                            case AllLines.AddressValidator.AddressIsEmpty:
                                err.FieldId = ApplicantAddressIsEmpty;
                                valList.Add(err);
                                break;

                            default:
                                break;
                        }
                    }
                }
                else
                {
                    valList.Add(new ValidationItem("Applicant Name is null", ApplicantNameIsNull, false));
                }
            }
            else
            {
                valList.Add(new ValidationItem("Applicant is null", ApplicantIsNull, false));
            }

            return valList;
        }
    }
}