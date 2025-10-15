using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class AIRelativeValidator
    {
        public const string ValidationListID = "{0F16E0ED-FB11-4389-A33A-13E8905A10E8}";
        public const string AiIsNull = "{1491CEB2-F92C-4472-AFAD-2BBBB25E3F0A}";
        public const string CommercialName = "{994ECED0-A3FC-444A-BE07-8EBA692D21C7}";
        public const string Description = "{103242A5-76E8-4E9E-BDE7-2AEE25D4EEC3}";
        public const string AddressStreetNumber = "{99C0B919-BDDD-4155-9196-7A6050975D46}";
        public const string AddressStreetName = "{B0F8D1CA-0B8B-4C95-A230-631190BEF01E}";
        public const string AddressCity = "{C0A8273E-1738-4033-AEE0-AE77F358F240}";
        public const string AddressState = "{98954C9B-DCBF-482D-A824-7EA3AD61F17D}";
        public const string AddressZipCode = "{2FED3D18-3811-40E3-8CC7-249A0827C0CD}";
        public const string AddressCountyID = "{3368D581-7DB4-4E19-922F-A4C428B55DEC}";
        public const string AddressSatetNotIndiana = "{F25A7ED7-DAA0-43D5-9CA8-FFE03BDFD6FE}";

        public static Validation.ObjectValidation.ValidationItemList AdditionalInterestRelativeValidation(QuickQuote.CommonObjects.QuickQuoteAdditionalInterest MyAdditionalInterest, ValidationItem.ValidationType valType, IFM.VR.Common.Helpers.HOM.SectionCoverage sectionCoverage, QuickQuote.CommonObjects.QuickQuoteObject quote = null)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (MyAdditionalInterest != null)
            {
                bool mustBeInIndiana = false; //Added for Bug 26103 MLW
                bool countyRequired = true; //Added 02/17/2020 for Home Endorsements task 44249 MLW
                if (sectionCoverage != null)
                {
                    switch (sectionCoverage.SectionCoverageIAndIIEnum)
                    {
                        case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence:
                            VRGeneralValidations.Val_HasRequiredField(MyAdditionalInterest.Name.CommercialName1, valList, CommercialName, "Name of Student");
                            VRGeneralValidations.Val_HasRequiredField(MyAdditionalInterest.Description, valList, Description, "Name of School");
                            mustBeInIndiana = false; //Added for Bug 26103 MLW
                            break;
                        case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage:
                            VRGeneralValidations.Val_HasRequiredField(MyAdditionalInterest.Name.CommercialName1, valList, CommercialName, "Name of Relative");
                            VRGeneralValidations.Val_HasRequiredField(MyAdditionalInterest.Description, valList, Description, "Name of Residency");
                            mustBeInIndiana = true; //Added for Bug 26103 MLW
                            break;
                        case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement:
                            mustBeInIndiana = false; //Added for Bug 26103 MLW
                            //Added 02/17/2020 for Home Endorsement task 44249 MLW
                            if (quote.QuoteTransactionType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                            {
                                countyRequired = false;
                            }
                            break;
                    } 
                }

                var addressVals = IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.AddressValidation(MyAdditionalInterest.Address, valType, mustBeInIndiana, true, quote, countyRequired);
                foreach (var val in addressVals)
                {
                    switch (val.FieldId)
                    {
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.StreetAndPoBoxEmpty:
                            valList.Add(new ObjectValidation.ValidationItem("Missing Street #", AddressStreetNumber));
                            valList.Add(new ObjectValidation.ValidationItem("Missing Street Name", AddressStreetName));
                            break;
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.HouseNumberID:
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressStreetNumber));
                            break;
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.StreetNameID:
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressStreetName));
                            break;
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.CityID:
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressCity));
                            break;
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.StateID:
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressState));
                            break;
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.AddressSatetNotIndiana:
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressState));
                            break;
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.ZipCodeID:
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressZipCode));
                            break;
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.CountyID:
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressCountyID));
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

        public static Validation.ObjectValidation.ValidationItemList AdditionalInterestAddressValidation(QuickQuote.CommonObjects.QuickQuoteAdditionalInterest MyAdditionalInterest, ValidationItem.ValidationType valType, IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper sectionCoverage, QuickQuote.CommonObjects.QuickQuoteObject quote = null)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (MyAdditionalInterest != null)
            {
                bool mustBeInIndiana = false; //Added for Bug 26103 MLW
                bool countyRequired = true; //Added 02/17/2020 for Home Endorsement task 44249 MLW
                if (sectionCoverage != null)
                {
                    //Added for Bug 26103 MLW
                    switch (sectionCoverage.SectionCoverageIAndIIEnum)
                    {
                        case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence:
                            mustBeInIndiana = false;
                            break;
                        case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage:
                            mustBeInIndiana = true;
                            break;
                        case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement:
                            mustBeInIndiana = false;
                            //Added 02/17/2020 for Home Endorsement task 44249 MLW
                            if (quote.QuoteTransactionType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                            {
                                countyRequired = false;
                            }
                            break;
                    }
                }

                var addressVals = IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.AddressValidation(MyAdditionalInterest.Address, valType, mustBeInIndiana, true, quote, countyRequired);
                foreach (var val in addressVals)
                {
                    switch (val.FieldId)
                    {
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.StreetAndPoBoxEmpty:
                            valList.Add(new ObjectValidation.ValidationItem("Missing Street #", AddressStreetNumber));
                            valList.Add(new ObjectValidation.ValidationItem("Missing Street Name", AddressStreetName));
                            break;
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.HouseNumberID:
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressStreetNumber));
                            break;
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.StreetNameID:
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressStreetName));
                            break;
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.CityID:
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressCity));
                            break;
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.StateID:
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressState));
                            break;
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.AddressSatetNotIndiana:
                            //Added for Bug 26103 MLW
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressState));
                            break;
                       
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.ZipCodeID:
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressZipCode));
                            break;
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.CountyID:
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressCountyID));
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