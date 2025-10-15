using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IFM.PrimativeExtensions;

namespace IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL
{
    public class CGL_PolicyCoveragesValidator
    {
        public const string ValidationListID = "{39604D48-BADE-488E-BFF1-CB3F2B5D3EEA}";
        public const string quoteIsNull = "{CDF1DB31-CEFB-4983-8913-7C6D95911F2C}";

        public const string additionalInsured_PremiumCharge = "{C9F5E29A-721D-498F-8F7D-A04C6CE85429}";

        public const string employeeOccurrenceLimit = "{D866000C-500D-4CAF-B5C7-4EA583C4CBDE}";
        public const string employeeNumberOfEmployees = "{7DAF5246-22DB-40F7-B206-8AAB50D9708C}";
        public const string employeeAggregateLimit = "{D142D80F-3D8B-40BD-A664-5094D6EC904E}";
        public const string employeeDeductible = "{EAFC41F7-C5AF-4E77-94F7-F89CF63517C2}";

        public const string liquorSales = "{B7D441CE-11E3-4185-8E56-69B4FCFD1464}";
        public const string liquorClassification = "{6DBEFFC7-D1F8-46F8-9D43-5C723AE949E2}";
        public const string liquorOccurrenceLimit = "{539933C2-61B7-40BD-9037-FC2B20BE166D}";

        public const string professionalAll = "{C46E78AC-2934-4AB4-84FF-490E0F10C4BA}";
        public const string professionalBurial = "{835E4B7D-C3ED-480C-B9E4-360CF7EC0AC4}";
        public const string professinalBodies = "{0693CB43-63E0-430A-AC8B-C9B375D355FA}";
        public const string professionalClergy = "{575678F2-15EB-4C0B-9160-EDA338C72D44}";

        public const string HiredAutoNonOwnedMismatch = "{F574BED0-B679-4149-AC95-8DA2F67C58B5}"; // both Hiredauto and non owned must be the same always - Matt A 12-21-2015

        public static Validation.ObjectValidation.ValidationItemList ValidatePolicyCoverages(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType,bool? additionalInsuredApplied,bool? professionalApplied)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                switch (valType)
                {
                    case ValidationItem.ValidationType.issuance:
                    case ValidationItem.ValidationType.appRate:
                    case ValidationItem.ValidationType.quoteRate:
                        //Additional Insured
                        if (additionalInsuredApplied.HasValue) // you need this flag or you have no idea if the user even wants this coverage
                        {
                            if (additionalInsuredApplied.Value) 
                            {
                                if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(quote.AdditionalInsuredsManualCharge, valList, additionalInsured_PremiumCharge, "Premium Charge"))
                                    IFM.VR.Validation.VRGeneralValidations.Val_IsNonNegativeWholeNumber(quote.AdditionalInsuredsManualCharge, valList, additionalInsured_PremiumCharge, "Premium Charge");
                            }
                            else
                            {
                                IFM.VR.Validation.VRGeneralValidations.Val_HasIneligibleField(quote.AdditionalInsuredsManualCharge, valList, additionalInsured_PremiumCharge, "Premium Charge");
                            }
                        }



                        //Employee
                        if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(quote.EmployeeBenefitsLiabilityOccurrenceLimit, valList, employeeNumberOfEmployees, "Number of Employees", true)) // testing this to see if this coverage is applied will be string.empty if it is not applied
                        {
                            if(IFM.VR.Validation.VRGeneralValidations.Val_IsNonNegativeWholeNumber(quote.EmployeeBenefitsLiabilityText, valList, employeeNumberOfEmployees, "Number of Employees"))
                                IFM.VR.Validation.VRGeneralValidations.Val_IsNumberInRange(quote.EmployeeBenefitsLiabilityText, valList, employeeNumberOfEmployees, "Number of Employees Max 1000","1","1000");

                            //EmployeeBenefitsLiabilityOccurrenceLimit must equal or greater quote.OccurrenceLiabilityLimit
                            if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(quote.EmployeeBenefitsLiabilityOccurrenceLimit, valList, employeeOccurrenceLimit, "Employee Benefit Occurrence Limit"))
                            {
                                if (quote.OccurrenceLiabilityLimit.TryToGetDouble() > quote.EmployeeBenefitsLiabilityOccurrenceLimit.TryToGetDouble())
                                    valList.Add(new ValidationItem("Employee Benefits Occurrence Limit must not be less than the Policy Level Occurrence Liability Limit", employeeOccurrenceLimit));

                                //EmployeeBenefitsLiabilityAggregateLimit must be 3 times EmployeeBenefitsLiabilityOccurrenceLimit
                                if (quote.EmployeeBenefitsLiabilityOccurrenceLimit.TryToGetDouble() == 0 || quote.EmployeeBenefitsLiabilityAggregateLimit.TryToGetDouble() / 3.0 != quote.EmployeeBenefitsLiabilityOccurrenceLimit.TryToGetDouble())
                                    valList.Add(new ValidationItem("Employee Aggregate Limit must be 3 times the Employee Occurrence Limit", employeeAggregateLimit));
                            }
                            
                            //EmployeeBenefitsLiabilityDeductible must be '1,000'
                            if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(quote.EmployeeBenefitsLiabilityDeductible, valList, employeeDeductible, "Employee Benefit Deductible"))
                            {
                                if (quote.EmployeeBenefitsLiabilityDeductible.TryToGetDouble() != 1000)
                                    valList.Add(new ValidationItem("Employee Benefit Deductible must be 1,000", employeeDeductible));
                            }
                        }
                        else
                        {
                            //EmployeeBenefitsLiabilityOccurrenceLimitId must be empty
                            //EmployeeBenefitsLiabilityAggregateLimit must be empty
                            //EmployeeBenefitsLiabilityDeductible must be empty
                            IFM.VR.Validation.VRGeneralValidations.Val_HasIneligibleField_DD(quote.EmployeeBenefitsLiabilityOccurrenceLimitId, valList, employeeOccurrenceLimit, "Occurrence Limit");
                            IFM.VR.Validation.VRGeneralValidations.Val_HasIneligibleField(quote.EmployeeBenefitsLiabilityAggregateLimit, valList, employeeAggregateLimit, "Aggregate Limit");
                            IFM.VR.Validation.VRGeneralValidations.Val_HasIneligibleField(quote.EmployeeBenefitsLiabilityDeductible, valList, employeeDeductible, "Deductible");                            
                        }

                        //Hired Auto & Non Owner
                        if (quote.HasHiredAuto != quote.HasNonOwnedAuto)
                            valList.Add(new ValidationItem("Hired Auto and Non Owned must always be the same.", HiredAutoNonOwnedMismatch ));

                        // EPLI is a common coverage so make another validator and check it here

                        //Liquor
                        if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(quote.LiquorLiabilityOccurrenceLimit, valList, liquorSales, "Liquor sales", true)) // checking this limit as an indication of if the coverage is applied or not
                        {
                            if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(quote.LiquorSales, valList, liquorSales, "Liquor sales"))
                            {
                                // must have sales as whole positive number
                                IFM.VR.Validation.VRGeneralValidations.Val_IsPositiveWholeNumber(quote.LiquorSales, valList, liquorSales, "Liquor sales");
                            }
                            

                            // must have a LiquorLiabilityClassificationId
                            IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField_DD(quote.LiquorLiabilityClassificationId, valList, liquorClassification, "Liquor Classification");

                            // must have a quote.LiquorLiabilityOccurrenceLimit must equal or greater quote.OccurrenceLiabilityLimit
                            if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(quote.LiquorLiabilityOccurrenceLimit, valList, liquorSales, "Liquor Occurrence Limit"))
                            {
                                if (quote.OccurrenceLiabilityLimit.TryToGetDouble() > quote.LiquorLiabilityOccurrenceLimit.TryToGetDouble())
                                    valList.Add(new ValidationItem("Liquor Occurrence Limit must not be less than the Policy Level Occurrence Liability Limit", liquorOccurrenceLimit));
                            }
                        }
                        else
                        {                            
                            IFM.VR.Validation.VRGeneralValidations.Val_HasIneligibleField(quote.LiquorLiabilityClassificationId, valList, liquorClassification, "Liquor Classification");
                            IFM.VR.Validation.VRGeneralValidations.Val_HasIneligibleField(quote.LiquorLiabilityOccurrenceLimit, valList, liquorOccurrenceLimit, "Liquor Occurrence Limit");
                        }

                        //Professional Liability
                        if (professionalApplied.HasValue)
                        {
                            if (professionalApplied.Value)
                            {
                                if (quote.ProfessionalLiabilityCemetaryNumberOfBurials.IsNullEmptyorWhitespace() == false || quote.ProfessionalLiabilityFuneralDirectorsNumberOfBodies.IsNullEmptyorWhitespace() == false || quote.ProfessionalLiabilityPastoralNumberOfClergy.IsNullEmptyorWhitespace() == false)
                                {
                                    if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(quote.ProfessionalLiabilityCemetaryNumberOfBurials, valList, professionalBurial, "Number of Burials", true))
                                        IFM.VR.Validation.VRGeneralValidations.Val_IsPositiveWholeNumber(quote.ProfessionalLiabilityCemetaryNumberOfBurials, valList, professionalBurial, "Number of Burials");

                                    if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(quote.ProfessionalLiabilityFuneralDirectorsNumberOfBodies, valList, professinalBodies, "Number of Bodies", true))
                                        IFM.VR.Validation.VRGeneralValidations.Val_IsPositiveWholeNumber(quote.ProfessionalLiabilityFuneralDirectorsNumberOfBodies, valList, professinalBodies, "Number of Bodies");

                                    if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(quote.ProfessionalLiabilityPastoralNumberOfClergy, valList, professionalClergy, "Number of Clergy", true))
                                        IFM.VR.Validation.VRGeneralValidations.Val_IsPositiveWholeNumber(quote.ProfessionalLiabilityPastoralNumberOfClergy, valList, professionalClergy, "Number of Clergy");
                                }
                                else
                                {
                                    valList.Add(new ValidationItem("Missing all Professional Liability fields.", professionalAll));
                                }
                            }
                            else
                            {
                                if (quote.ProfessionalLiabilityCemetaryNumberOfBurials.IsNullEmptyorWhitespace() == false || quote.ProfessionalLiabilityFuneralDirectorsNumberOfBodies.IsNullEmptyorWhitespace() == false || quote.ProfessionalLiabilityPastoralNumberOfClergy.IsNullEmptyorWhitespace() == false)
                                {
                                    valList.Add(new ValidationItem("Professional Liability fields invalid.", professionalAll));
                                }
                            }
                        }
                        else
                        {
                            // even if you don't know if it should be here you can assume that if there are any values they should be validated
                            if (quote.ProfessionalLiabilityCemetaryNumberOfBurials.IsNullEmptyorWhitespace() == false || quote.ProfessionalLiabilityFuneralDirectorsNumberOfBodies.IsNullEmptyorWhitespace() == false || quote.ProfessionalLiabilityPastoralNumberOfClergy.IsNullEmptyorWhitespace() == false)
                            {
                                if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(quote.ProfessionalLiabilityCemetaryNumberOfBurials, valList, professionalBurial, "Number of Burials", true))
                                    IFM.VR.Validation.VRGeneralValidations.Val_IsPositiveWholeNumber(quote.ProfessionalLiabilityCemetaryNumberOfBurials, valList, professionalBurial, "Number of Burials");

                                if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(quote.ProfessionalLiabilityFuneralDirectorsNumberOfBodies, valList, professinalBodies, "Number of Bodies", true))
                                    IFM.VR.Validation.VRGeneralValidations.Val_IsPositiveWholeNumber(quote.ProfessionalLiabilityFuneralDirectorsNumberOfBodies, valList, professinalBodies, "Number of Bodies");

                                if (IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(quote.ProfessionalLiabilityPastoralNumberOfClergy, valList, professionalClergy, "Number of Clergy", true))
                                    IFM.VR.Validation.VRGeneralValidations.Val_IsPositiveWholeNumber(quote.ProfessionalLiabilityPastoralNumberOfClergy, valList, professionalClergy, "Number of Clergy");
                            }
                        }
                        
                                                

                        break;
                    default:
                        break;
                }
            }
            else
            {
                valList.Add(new ValidationItem("Quote is null.", quoteIsNull));
            }

            return valList;

        }

    }
}