using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IFM.PrimativeExtensions;


namespace IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL
{
    public class GeneralInformationValidator
    {
        public const string ValidationListID = "{6CD7AA5F-7294-43A9-A3CC-B35B179080B2}";
        public const string quoteIsNull = "{82A94AE6-CC40-42B0-B2DB-BC539BF17097}";

        public const string programTypeId = "{888FDA4E-FA5A-449B-BB4D-6544EEFDCC74}";
        public const string occurrenceLibLimit = "{372655D8-4A3C-43E0-B5BD-3989A4710484}";
        public const string generalAggregate = "{7E666112-32A8-4979-ADD0-63FA0915A114}";
        public const string DamageToPremisesRentedLimit = "{D7E59D66-CBE3-45E0-B680-F67F80C15B73}";
        public const string ProductOperationsAg = "{BA5AC3A3-7992-4166-A555-D64F04CD2CAA}";
        public const string MedicalExpensesLimit = "{A1D89EC8-D8E6-4B8C-973F-29E9B1E084AB}";
        public const string advertisingInjury = "{74424EFE-9BD8-4A37-B329-72CB68EF2E61}";

        public const string subrogation = "{F94065F9-B296-434E-9B37-CCA8699D8B74}";
        public const string deductibleType = "{216A8BA8-1F4B-4FDF-B7F4-9C15537BDFD0}";
        public const string deductibleAmount = "{1D18C887-A62F-403B-9348-EABBBDD50FC3}";
        public const string deductibleBasis = "{67DF6BEA-82BC-41ED-A06E-9172DDA09918}";


        public static Validation.ObjectValidation.ValidationItemList ValidateGeneralInformation(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                switch(valType)
                {
                    case ValidationItem.ValidationType.issuance:
                    case ValidationItem.ValidationType.appRate:
                    case ValidationItem.ValidationType.quoteRate:
                        IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(quote.ProgramTypeId, valList, programTypeId, "Program Type");
                        IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(quote.DamageToPremisesRentedLimitId, valList, DamageToPremisesRentedLimit, "Damage to Premises Rented to You");
                        IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(quote.MedicalExpensesLimitId, valList, MedicalExpensesLimit, "Medical Expenses");

                        // these tests should be using the static data rather than limit texts from the qq Object
                        var occLimit = quote.OccurrenceLiabilityLimit.TryToGetInt32();
                        int personalAdvertising = 0;
                        if (quote.PersonalAndAdvertisingInjuryLimit.IsNumeric())
                        {
                            personalAdvertising = quote.PersonalAndAdvertisingInjuryLimit.TryToGetInt32();
                        }
                        else
                        {
                            personalAdvertising = 0;
                        }
                        //var personalAdvertising = quote.PersonalAndAdvertisingInjuryLimit.TryToGetInt32();
                        var genAgg = quote.GeneralAggregateLimit.TryToGetInt32();
                        var product =0; //Adding this IF condition for 59049 BB
                        if (quote.ProductsCompletedOperationsAggregateLimit != "Excluded")
                            product = quote.ProductsCompletedOperationsAggregateLimit.TryToGetInt32(); // Can equal 'Excluded'(327 is the id) so that would be zero

                        if (occLimit < personalAdvertising)
                        {
                            valList.Add(new ValidationItem("Occurrence Liability Limit can not be less than the Personal and Advertising Injury.",occurrenceLibLimit));
                        }
                        if (genAgg < occLimit)
                        {
                            valList.Add(new ValidationItem("General Aggregate can not be less than the Occurrence Liability Limit.", generalAggregate));
                        }
                        if (product != 0 && genAgg < product)
                        {
                            valList.Add(new ValidationItem("General Aggregate can not be less than the Product/Completed Operations Aggregate.", generalAggregate));
                        }
                        if (product != 0 && product < occLimit)
                        {
                            valList.Add(new ValidationItem("Product/Completed Operations Aggregate can not be less than the Occurrence Liability Limit.", ProductOperationsAg));
                        }

                        if (!quote.HasBusinessMasterEnhancement) // quote.Has_PackageGL_EnhancementEndorsement on cpp                        
                        {
                            // must not have subrogation
                            if (!quote.BlanketWaiverOfSubrogation.IsNullEmptyorWhitespace())
                                valList.Add(new ValidationItem("Subrogation is not valid without the enhancement.",subrogation));
                        }
                        if (quote.GL_PremisesAndProducts_DeductibleCategoryTypeId.TryToGetInt32() > 0 || quote.GL_PremisesAndProducts_DeductibleId.TryToGetInt32() > 0 | quote.GL_PremisesAndProducts_DeductiblePerTypeId.TryToGetInt32() > 0)
                        {
                            // if any are present then they all must be
                            IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField_DD(quote.GL_PremisesAndProducts_DeductibleCategoryTypeId, valList, deductibleType, "Deductible Type");
                            IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField_DD(quote.GL_PremisesAndProducts_DeductibleId, valList, deductibleAmount, "Deductible Amount");
                            IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField_DD(quote.GL_PremisesAndProducts_DeductiblePerTypeId, valList, deductibleBasis, "Deductible Basis");
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