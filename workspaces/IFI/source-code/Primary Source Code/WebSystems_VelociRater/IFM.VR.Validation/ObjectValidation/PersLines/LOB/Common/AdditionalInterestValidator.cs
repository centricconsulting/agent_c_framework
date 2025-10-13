namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common
{
    public class AdditionalInterestValidator
    {
        public const string ValidationListID = "{4A399423-E109-43FD-9B81-EDE6A5710299}";

        public const string AiIsNull = "{5FC2CC9B-E467-4093-9DF3-F10DBF764B6C}";

        public const string AIType = "{BEFF4773-567A-4055-BF7A-72C8A1A46BC1}";

        public const string BillToMortgageeIneligible = "{8163E883-1195-4A8A-BFB9-8F2E2C7D7E00}";

        public const string CommAndPersNameComponentsEmpty = "{49898D3E-0276-4774-A294-6441A3167084}";
        public const string CommAndPersNameComponentsAllSet = "{D6E5E4A5-86EA-4377-A570-65E193F3BE42}";
        public const string CommercialName = "{3589861F-37FD-4781-A475-0472E1755BF3}";
        public const string FirstNameID = "{02C91C85-A501-4339-9BCD-612F044F64BA}";
        public const string LastNameID = "{CD5CAEB1-865A-49C0-9210-65053F7EAE38}";

        public const string PhoneNumberInvalid = "{8510926E-BC1F-4516-A5A2-C22BFCA00AC0}";
        public const string PhoneExtensionInvalid = "{2BF17240-A6BC-4719-AA52-1A23F7CF100D}";

        public const string StreetAndPoBoxEmpty = "{95D69511-04E7-4AFA-90FC-03CF38FC054C}";
        public const string StreetAndPoxBoxAreSet = "{B3548D78-0C9A-44E2-AC99-A22BBD3DF8F4}";
        public const string HouseNumberID = "{235F9361-9519-4C95-AA3C-878148417748}";
        public const string StreetNameID = "{6BF7F11D-5407-41F2-B007-3E42005BBA1E}";
        public const string POBOXID = "{8A746AAC-C804-43F0-AEF3-99D62EF36E5A}";
        public const string ZipCodeID = "{76ECFEBD-3D16-42A0-9012-D6BD2FED2BD1}";
        public const string CityID = "{74B28BA8-2819-4E52-998B-327E9940E112}";
        public const string StateID = "{F58F2433-97A3-4088-8E37-FFFB76B3420D}";

        public const string Description = "{F3E804F5-9536-475A-815A-BB379E69FAAC}";

        public static Validation.ObjectValidation.ValidationItemList AdditionalInterestValidation(QuickQuote.CommonObjects.QuickQuoteAdditionalInterest MyAdditionalInterest, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            //valList.Add(new ValidationItem("Annual MTG Pay Plan Only Available when Bill To is Mortgagee", PayPlan, false));

            if (MyAdditionalInterest != null)
            {
                VRGeneralValidations.Val_HasRequiredField_DD(MyAdditionalInterest.TypeId, valList, AIType, "Type");

                VRGeneralValidations.Val_HasRequiredField(MyAdditionalInterest.Description, valList, Description, "Description");

                if (MyAdditionalInterest.BillTo && (MyAdditionalInterest.TypeId != "42" & MyAdditionalInterest.TypeId != "11" & MyAdditionalInterest.TypeId != "15"))
                {
                    // when billto is true you must be a AI of type motgagee 1/2/3
                    valList.Add(new ValidationItem("BillTo Invalid", BillToMortgageeIneligible, false));
                }

                var nameVal = IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.ValidateNameObject(MyAdditionalInterest.Name, valType);
                foreach (var v in nameVal)
                {
                    switch (v.FieldId)
                    {
                        case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.CommAndPersNameComponentsEmpty:
                            v.FieldId = CommAndPersNameComponentsEmpty;
                            valList.Add(v);
                            break;

                        case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.CommAndPersNameComponentsAllSet:
                            v.FieldId = CommAndPersNameComponentsAllSet;
                            valList.Add(v);
                            break;

                        case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.CommercialName:
                            v.FieldId = CommercialName;
                            valList.Add(v);
                            break;

                        case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.FirstNameID:
                            v.FieldId = FirstNameID;
                            valList.Add(v);
                            break;

                        case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.LastNameID:
                            v.FieldId = LastNameID;
                            valList.Add(v);
                            break;
                    }
                }

                var phoneVal = IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.ValidatePhoneList(MyAdditionalInterest.Phones, valType);
                foreach (var v in phoneVal)
                {
                    switch (v.FieldId)
                    {
                        case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneNumberInvalid:
                            v.FieldId = PhoneNumberInvalid;
                            valList.Add(v);
                            break;

                        case IFM.VR.Validation.ObjectValidation.AllLines.PhonesValidator.PhoneExtensionInvalid:
                            v.FieldId = PhoneExtensionInvalid;
                            valList.Add(v);
                            break;
                    }
                }

                var addressVal = IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.AddressValidation(MyAdditionalInterest.Address, valType);
                foreach (var v in addressVal)
                {
                    switch (v.FieldId)
                    {
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.StreetAndPoBoxEmpty:
                            v.FieldId = StreetAndPoBoxEmpty;
                            valList.Add(v);
                            break;

                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.StreetAndPoxBoxAreSet:
                            v.FieldId = StreetAndPoxBoxAreSet;
                            valList.Add(v);
                            break;

                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.HouseNumberID:
                            v.FieldId = HouseNumberID;
                            valList.Add(v);
                            break;

                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.StreetNameID:
                            v.FieldId = StreetNameID;
                            valList.Add(v);
                            break;

                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.POBOXID:
                            v.FieldId = POBOXID;
                            valList.Add(v);
                            break;

                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.ZipCodeID:
                            v.FieldId = ZipCodeID;
                            valList.Add(v);
                            break;

                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.CityID:
                            v.FieldId = CityID;
                            valList.Add(v);
                            break;

                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.StateID:
                            v.FieldId = StateID;
                            valList.Add(v);
                            break;
                    }
                }
            }
            else
            {
                valList.Add(new ValidationItem("Additional Interest is null", AiIsNull, false));
            }

            return valList;
        }
    }
}