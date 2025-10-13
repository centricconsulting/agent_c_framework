using Diamond.Web.BaseControls;
using System;

namespace IFM.VR.Validation.ObjectValidation.AllLines
{
    public class NameValidator
    {
        public const string ValidationListID = "{B1932207-D6C8-4A45-84F2-0F50748FF561}";

        public const string NameIsNull = "{AB47438D-5587-4487-BDCD-FA7CD891628E}";

        public const string CommAndPersNameComponentsEmpty = "{209F8911-5FB7-4E65-AF4D-8B9E5935B02D}";
        public const string CommAndPersNameComponentsAllSet = "{0B13E421-FD0A-4F1C-81F8-D730E33A8809}";

        public const string FirstNameID = "{D8B72E2B-75B1-4F73-9B1C-2240D7BB406D}";
        public const string LastNameID = "{EDAB54B6-1A10-45B2-A951-2E88534289E9}";

        public const string CommercialName = "{C0C69133-27B3-4926-A503-71A2088321F9}";

        public const string BusinessStartedDate = "{09078A7A-07EB-4651-8CD7-D5CEF6685221}"; // Matt A - 12/21/2015
        public const string YearsOfExperience = "{87965AFE-5085-409D-9709-725940668D52}"; // Matt A - 12/21/2015
        public const string DescriptionOfOperations = "{549634e2-5b87-4d6d-b1a8-60efe9f28c49}"; 


        public const string GenderID = "{8756717F-149B-4628-8A4B-7AF743A8F4E6}";
        public const string SSNID = "{FE95154C-CB8D-4344-9CD5-8E1F71F632B9}";
        public const string BirthDate = "{41BCB8EF-4EF7-4DE8-9480-CF5897982A25}";

        public const string FEINID = "{61B2A732-18EE-44DC-96F3-C43D18B36CB9}";

        public const string EntityTypeId = "{B20B64F6-047C-49DF-A072-3A7BC79E845B}"; // also known as Business Type - corporation or partnership
        public const string OtherEntityType = "{B2FCFBB4-EDBA-4C99-9345-980C8AB078FE}"; //Added 2/14/2022 for bug 63511 MLW

        public static Validation.ObjectValidation.ValidationItemList ValidateNameObject(QuickQuote.CommonObjects.QuickQuoteName name, ValidationItem.ValidationType valType, QuickQuote.CommonObjects.QuickQuoteObject qqo = null)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            bool isUnknowNameType = name.TypeId == "";
            bool isCommName = (name.TypeId == "2");
            if (name != null)
            {
                //if (string.IsNullOrWhiteSpace(name.CommercialName1) & string.IsNullOrWhiteSpace(name.FirstName) & string.IsNullOrWhiteSpace(name.LastName))
                //{
                //    //valList.Add(new ObjectValidation.ValidationItem("No name information.", CommAndPersNameComponentsEmpty));
                //}
                //else
                //{
                if (isUnknowNameType)
                {
                    VRGeneralValidations.Val_HasRequiredField(name.FirstName, valList, FirstNameID, "First Name");
                    VRGeneralValidations.Val_HasRequiredField(name.LastName, valList, LastNameID, "Last Name");
                    VRGeneralValidations.Val_HasRequiredField(name.CommercialName1, valList, CommercialName, "Commercial Name");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(name.CommercialName1) == false & (string.IsNullOrWhiteSpace(name.FirstName) == false | string.IsNullOrWhiteSpace(name.LastName) == false))
                    {
                        // This validation only applies to new business
                        //if (qqo != null && qqo.QuoteTransactionType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                        //{
                        //    // endorsement - don't do anything
                        //}
                        //else
                        //{
                        //    // Not an endorsement - validate
                        //    // part of comm and pers are set
                        //    if (name.TypeId != "2") // checked this way so that we can assume that blank we be personal
                        //    {
                        //        valList.Add(new ObjectValidation.ValidationItem("Must be a personal name but you have a commercial name", CommAndPersNameComponentsAllSet));
                        //    }
                        //    else
                        //    {
                        //        valList.Add(new ObjectValidation.ValidationItem("Must be a commercial name but you have a personal name", CommAndPersNameComponentsAllSet));
                        //    }
                        //}

                        
                        // 05/05/2022 CAH - Some things will pass in valType.  Not all things pass in qqo.
                        // Endorsements were not skipping validation before for additional interests.
                        if (valType != IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.endorsement ||
                                  (qqo != null && qqo.QuoteTransactionType != QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote))
                        {
                            if (isCommName) // checked this way so that we can assume that blank we be personal
                            {
                                valList.Add(new ObjectValidation.ValidationItem("Must be a commercial name but you have a personal name", CommAndPersNameComponentsAllSet));
                                
                            }
                            else
                            {
                                valList.Add(new ObjectValidation.ValidationItem("Must be a personal name but you have a commercial name", CommAndPersNameComponentsAllSet));
                            }
                        }
                    }
                    else
                    {
                        if (name.TypeId != "2") // checked this way so that we can assume that blank we be personal
                        {
                            // personal
                            VRGeneralValidations.Val_HasRequiredField(name.FirstName, valList, FirstNameID, "First Name");
                            VRGeneralValidations.Val_HasRequiredField(name.LastName, valList, LastNameID, "Last Name");
                        }
                        else
                        {
                            // commercial
                            VRGeneralValidations.Val_HasRequiredField(name.CommercialName1, valList, CommercialName, "Commercial Name");
                            //VRGeneralValidations.Val_HasRequiredField(name.DescriptionOfOperations, valList, DescriptionOfOperations, "Description of Operations");

                            if (qqo != null && qqo.QuoteTransactionType != QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                            {
                                switch (qqo.LobType)
                                {
                                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto:
                                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP:
                                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability:
                                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage:
                                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty:
                                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation:
                                        VRGeneralValidations.Val_HasRequiredField(name.DescriptionOfOperations, valList, DescriptionOfOperations, "Description of Operations");
                                        break;
                                    default:
                                        break;
                                }
                            }

                            if (VRGeneralValidations.Val_HasRequiredField(name.DateBusinessStarted, valList, BusinessStartedDate, "Business Started Date"))
                              //  if (VRGeneralValidations.Val_HasRequiredField(name.DateBusinessStarted, valList, BusinessStartedDate, "Business Started Date",(valType == ValidationItem.ValidationType.quoteRate) ? true : false))
                            {
                                if (VRGeneralValidations.Val_IsValidDate(name.DateBusinessStarted,valList,BusinessStartedDate, "Business Started Date"))
                                {
                                    //Updated 9/20/2022 for bug 67839 MLW
                                    // must be a date in the past
                                    //if (VRGeneralValidations.Val_IsDateInRange(name.DateBusinessStarted, valList, BusinessStartedDate, "Business Started Date","1/1/1700",DateTime.Now.ToShortDateString()))
                                    if (VRGeneralValidations.Val_IsDateInRange(name.DateBusinessStarted, valList, BusinessStartedDate, "Business Started Date", "1/1/1700", DateTime.Now.ToShortDateString(), true))
                                    {
                                        // if less than 3 years require Years of Experience
                                        if ((DateTime.Now - DateTime.Parse(name.DateBusinessStarted)).TotalDays < (365 * 3))
                                        {
                                            // require Years of Experience
                                            if (VRGeneralValidations.Val_HasRequiredField(name.YearsOfExperience, valList, YearsOfExperience, "Years of Experience"))
                                            {
                                                // make sure it is a number between 1 and 100 BD per task 74074
                                                VRGeneralValidations.Val_IsNumberInRange(name.YearsOfExperience, valList, YearsOfExperience, "Years of Experience", "1", "999");
                                            }

                                        }
                                    } 
                                    else
                                    {
                                        valList.Add(new ObjectValidation.ValidationItem("Business Started Date cannot be in the future.", BusinessStartedDate));
                                    }                                   
                                }
                            }
                        }
                    }
                    //}
                    if (!isCommName)
                    {
                        //VRGeneralValidations.Val_HasIneligibleField_DD(name.EntityTypeId, valList, EntityTypeId, "Business type"); //removed 6-22-15 Matt A

                        if (VRGeneralValidations.Val_HasRequiredField_DD(name.SexId, valList, GenderID, "Gender"))
                            VRGeneralValidations.Val_IsNonNegativeWholeNumber(name.SexId, valList, GenderID, "Gender");

                        if (VRGeneralValidations.Val_HasRequiredField(name.BirthDate, valList, BirthDate, "Birth Date"))
                            VRGeneralValidations.Val_IsValidBirthDate(name.BirthDate, valList, BirthDate, "Birth Date");
                    }
                    else
                    {
                        // must have a business type id
                        VRGeneralValidations.Val_HasRequiredField_DD(name.EntityTypeId, valList, EntityTypeId, "Business type");

                        //Added 2/14/2022 for bug 63511 MLW
                        if (qqo != null && qqo.QuoteTransactionType != QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                        {
                                switch (qqo.LobType)
                                {
                                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto:
                                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP:
                                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability:
                                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage:
                                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty:
                                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation:
                                        if (name.EntityTypeId == "5")
                                        {
                                            VRGeneralValidations.Val_HasRequiredField_DD(name.OtherLegalEntityDescription, valList, OtherEntityType, "Other Legal Entity");
                                        }
                                        break;
                                default:
                                    break;
                                }
                        }

                        // can not have either of these
                        VRGeneralValidations.Val_HasIneligibleField_DD(name.SexId, valList, GenderID, "Gender");
                        //VRGeneralValidations.Val_HasIneligibleField(name.BirthDate, valList, BirthDate, "Birth Date");
                    }

                    // Removed 6/1/2018  This will no longer allow blank TIN/SSN boxes to be ignored
                    //if (VRGeneralValidations.Val_HasRequiredField(name.TaxNumber, null, "", "", true))
                    //{     
                    if (isCommName)
                    {
                        if (name.TaxTypeId == "2")
                        {
                            VRGeneralValidations.Val_IsValidSSN(name.TaxNumber, valList, FEINID, "FEIN");
                        }
                        else
                        {
                            VRGeneralValidations.Val_IsValidSSN(name.TaxNumber, valList, SSNID, "SSN");
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(name.TaxNumber) == false)
                        {
                            VRGeneralValidations.Val_IsValidSSN(name.TaxNumber, valList, SSNID, "SSN");
                        }
                    }
                        //VRGeneralValidations.Val_IsValidSSN(name.TaxNumber, valList, SSNID, (!isCommName) ? "SSN" : "FEIN");
                    //}
                    // if is comm name then if they have tax number the taxnumtypeid needs to be "2"
                }

            }
            else
            {
                valList.Add(new ValidationItem("Name is null", NameIsNull, false));
            }
            return valList;
        }
    }
}