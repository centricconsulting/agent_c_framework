using System;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common
{
    public class PriorCarrierValidator
    {
        public const string ValidationListID = "{1FCAAE16-6D2C-4CDB-9667-41043B950907}";

        public const string PriorCarrierDuration = "{FD06198F-9AA4-4090-B68C-102EA0DB3610}";
        public const string PriorCarrierDurationType = "{163665AD-E0E2-46A3-9584-7F5B5B9E8F96}";
        public const string PriorCarrierExpirationDate = "{3967EF98-1DE5-4E6D-BA90-997E40E54001}";
        public const string PriorcarrierPreviousInsurer = "{0AA850F0-5590-45BB-AA49-9488898D434D}";
        public const string QuoteNull = "{B3E2ACEB-110A-4022-9549-E1799F462946}";
        public const string PriorCarrierNull = "{724370AF-F5E9-43DA-BEE9-8880F0DBF93E}";
        public const string PriorCarrierPolicyNumber = "{4C47B225-24FB-47EC-8566-81BE70A2E784}";

        public static Validation.ObjectValidation.ValidationItemList ValidatePriorCarrier(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                if (quote.PriorCarrier != null)
                {
                    // 0 = 'None' - 'None' is valid because maybe they haven't had insurance before
                    // "" = [EMPTY]
                    if (quote.PriorCarrier.PreviousInsurerTypeId != "0" & quote.PriorCarrier.PreviousInsurerTypeId != "")
                    {
                        //This one should never fail but go ahead and check it
                        if (VRGeneralValidations.Val_HasRequiredField_DD(quote.PriorCarrier.PreviousInsurerTypeId, valList, PriorcarrierPreviousInsurer, "Previous Insurer"))
                            VRGeneralValidations.Val_IsGreaterThanZeroNumber(quote.PriorCarrier.PreviousInsurerTypeId, valList, PriorcarrierPreviousInsurer, "Previous Insurer");

                        if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm) // all fields are required
                        {
                            if (VRGeneralValidations.Val_HasRequiredField(quote.PriorCarrier.PriorDurationWithCompany, valList, PriorCarrierDuration, "Duration"))
                                VRGeneralValidations.Val_IsGreaterThanZeroNumber(quote.PriorCarrier.PriorDurationWithCompany, valList, PriorCarrierDuration, "Duration");

                            if (VRGeneralValidations.Val_HasRequiredField_DD(quote.PriorCarrier.PriorDurationTypeId, valList, PriorCarrierDurationType, "Duration Type"))
                                VRGeneralValidations.Val_IsGreaterThanZeroNumber(quote.PriorCarrier.PriorDurationTypeId, valList, PriorCarrierDurationType, "Duration Type");

                            VRGeneralValidations.Val_HasRequiredField(quote.PriorCarrier.PriorPolicy, valList, PriorCarrierPolicyNumber, "Policy Number");

                            // has expiration date
                            if (VRGeneralValidations.Val_HasRequiredField_Date(quote.PriorCarrier.PriorExpirationDate, valList, PriorCarrierExpirationDate, "Expiration date"))
                                VRGeneralValidations.Val_IsDateInRange(quote.PriorCarrier.PriorExpirationDate, valList, PriorCarrierExpirationDate, "Expiration date", DateTime.Now.AddYears(-50).ToShortDateString(), DateTime.Now.AddYears(10).ToShortDateString());
                        }
                        else
                        {
                            if (VRGeneralValidations.Val_HasRequiredField(quote.PriorCarrier.PriorDurationWithCompany, valList, PriorCarrierDuration, "Duration", true))
                                VRGeneralValidations.Val_IsGreaterThanZeroNumber(quote.PriorCarrier.PriorDurationWithCompany, valList, PriorCarrierDuration, "Duration");

                            if (VRGeneralValidations.Val_HasRequiredField_DD(quote.PriorCarrier.PriorDurationTypeId, valList, PriorCarrierDurationType, "Duration Type"))
                                VRGeneralValidations.Val_IsGreaterThanZeroNumber(quote.PriorCarrier.PriorDurationTypeId, valList, PriorCarrierDurationType, "Duration Type");

                            // has expiration date
                            if (VRGeneralValidations.Val_HasRequiredField_Date(quote.PriorCarrier.PriorExpirationDate, valList, PriorCarrierExpirationDate, "Expiration date"))
                                VRGeneralValidations.Val_IsDateInRange(quote.PriorCarrier.PriorExpirationDate, valList, PriorCarrierExpirationDate, "Expiration date", DateTime.Now.AddYears(-50).ToShortDateString(), DateTime.Now.AddYears(10).ToShortDateString());

                            
                            // Matt A - 6-20-17 We want to let this rate but not issue so only check at issuance - suprised that this isn't wanted for other LOBs as well
                            if(quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal && valType == ValidationItem.ValidationType.issuance)
                            {
                                //Insurer Type ID 73 = Indiana Farmers Mutual
                                if (quote.PriorCarrier.PreviousInsurerTypeId == "73")
                                {
                                    valList.Add(new ObjectValidation.ValidationItem("Previous Insurer is Indiana Farmers Mutual. Refer to Underwriting.", PriorcarrierPreviousInsurer,false,true));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (quote.PriorCarrier.PreviousInsurerTypeId == "0")
                        {
                            // 'None' was selected so really don't need anything else - NOT EMPTY
                            //no prior carrier so you should not have expiration date
                            VRGeneralValidations.Val_HasIneligibleField_Date(quote.PriorCarrier.PriorExpirationDate, valList, PriorCarrierExpirationDate, "Expiration date");
                            VRGeneralValidations.Val_HasIneligibleField(quote.PriorCarrier.PriorDurationWithCompany, valList, PriorCarrierDuration, "Duration");
                            VRGeneralValidations.Val_HasIneligibleField(quote.PriorCarrier.PriorPolicy, valList, PriorCarrierPolicyNumber, "Policy Number");
                        }
                        else
                        {
                            // EMPTY - missing
                            VRGeneralValidations.Val_HasRequiredField_DD(quote.PriorCarrier.PreviousInsurerTypeId, valList, PriorcarrierPreviousInsurer, "Previous Insurer");
                        }
                    }
                }
                else
                {
                    valList.Add(new ValidationItem("Prior Carrier is null", PriorCarrierNull));
                }
            }
            else
            {
                valList.Add(new ValidationItem("Quote is null", QuoteNull));
            }
            return valList;
        }
    }
}