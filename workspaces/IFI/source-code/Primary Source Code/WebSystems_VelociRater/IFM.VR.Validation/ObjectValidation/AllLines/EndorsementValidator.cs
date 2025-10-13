using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuickQuote.CommonObjects;
using IFM.PrimativeExtensions;

namespace IFM.VR.Validation.ObjectValidation.AllLines
{
    public class EndorsementValidator
    {

        public const string ValidationListID = "{BAA51CC7-07F8-4E86-A3A1-22B8562DD13B}";
        public const string EndorsementRemarks = "{4CB23885-30F9-493F-8EFD-278BF7D46E46}";
        public const string EndorsementEffectiveDateMissing = "{03941E6D-0D08-4BEF-A02F-61BD9D731B65}";
        public const string EndorsementEffectiveDateInvalid = "{0D56FFCA-A3D2-4E45-B818-553E7C77E058}";
        public const string EndorsementEffectiveDateOutOfPolicy = "{188E18A1-281C-477F-9972-5FCF651EEF20}";
        public const string EndorsementEffectiveDateOutOfPolicyLongMessage = "{E5FC494F-C892-40EB-8091-A10A2EE86858}";
        public const string EndorsementType = "{0A30167B-31DC-4690-A600-90FBCF2D8119}"; //Added 11/13/2020 For CAP Endorsements task 52969 MLW


        public static Validation.ObjectValidation.ValidationItemList ValidateEndorsementRemarks(string remarks)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            //Validate Remarks
            string fullRemarks = remarks;
            string firstSevenChars = string.Empty;
            char firstChar;
            int matchcount = 0;
            bool badRemarks = false;

            if (string.IsNullOrWhiteSpace(fullRemarks))
            {
                valList.Add(new ValidationItem("Missing Remarks", EndorsementRemarks));
            }
            else
            {
                // Too short or Too long
                if ((fullRemarks.Length < 7) || (fullRemarks.Length > 255))
                {
                    badRemarks = true;
                }
                else
                {
                    firstSevenChars = fullRemarks.Substring(0, 7);
                }

                if (!badRemarks)
                {
                    for (int i = 0; i < firstSevenChars.Length; i++)
                    {
                        char testChar = firstSevenChars[i];
                        firstChar = firstSevenChars[0];
                        //Is not Alpha or a Space
                        if (testChar.IsAlphaNumericChar() == false && testChar.IsWhitespaceChar() == false)
                        {
                            badRemarks = true;
                        }
                        if (firstChar == testChar)
                        {
                            matchcount++;
                        }
                    }
                }

                // All the Same Char
                if (!badRemarks && matchcount == firstSevenChars.Length)
                {
                    badRemarks = true;
                }

                // Add the message
                if (badRemarks)
                {
                    valList.Add(new ValidationItem("Invalid Remarks", EndorsementRemarks));
                }


            }
            return valList;
        }

        public static Validation.ObjectValidation.ValidationItemList ValidateEndorsementEffectiveDate(string effectiveDate, DateTime pastDate, DateTime futureDate, QuickQuoteObject quote)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            //    //Validate Effective Date
            bool badDate = false;
            DateTime CheckDate = default(DateTime);
            DateTime QuoteEffectiveDate = default(DateTime);
            DateTime QuoteExpirationDate = default(DateTime);

            bool EffectiveDateConversionResult = DateTime.TryParse(effectiveDate, out CheckDate);
            bool QuoteEffConversionResult = DateTime.TryParse(quote?.EffectiveDate, out QuoteEffectiveDate);
            bool QuoteExpConversionResult = DateTime.TryParse(quote?.ExpirationDate, out QuoteExpirationDate);


            if (String.IsNullOrEmpty(effectiveDate))
            {
                valList.Add(new ValidationItem("Missing Transaction Effective Date", EndorsementEffectiveDateMissing));
            }
            else
            {
                if (!EffectiveDateConversionResult)
                {
                    badDate = true;
                }
                if (!badDate && (CheckDate < pastDate || CheckDate > futureDate))
                {
                    badDate = true;
                }
                if (badDate)
                {
                    valList.Add(new ValidationItem("Effective date must be between " + pastDate.ToShortDateString() + " and " + futureDate.ToShortDateString(), EndorsementEffectiveDateInvalid));
                }



                // MAY CHANGE
                
                if (!badDate && QuoteEffConversionResult && QuoteExpConversionResult)
                {
                    if (pastDate < QuoteEffectiveDate || futureDate > QuoteExpirationDate)
                    {
                        if (CheckDate < QuoteEffectiveDate || CheckDate > QuoteExpirationDate)
                        {
                            //valList.Add(new ValidationItem("Invalid Effective Date", EndorsementEffectiveDateOutOfPolicy));
                            //updated 7/25/2019 to see if the entered date is within the policy life; previously just looking for it to be inside current term
                            bool hasDateOutsideOfPolicyLife = false;
                            string policyLifeStartDate = "";
                            string policyLifeEndDate = "";
                            string beginChangeDate = "";
                            string endChangeDate = "";
                            QuickQuote.CommonMethods.QuickQuoteHelperClass qqHelper = new QuickQuote.CommonMethods.QuickQuoteHelperClass();
                            qqHelper.SetPolicyStartAndEndDateForQuote(quote, ref policyLifeStartDate, ref policyLifeEndDate);
                            DateTime dtPolicyLifeStartDate = default(DateTime);
                            DateTime dtPolicyLifeEndDate = default(DateTime);
                            bool PolLifeStartDtConversionResult = DateTime.TryParse(policyLifeStartDate, out dtPolicyLifeStartDate);
                            bool PolLifeEndDtConversionResult = DateTime.TryParse(policyLifeEndDate, out dtPolicyLifeEndDate);
                            //if (qqHelper.IsValidDateString(policyLifeStartDate, mustBeGreaterThanDefaultDate: true) && qqHelper.IsValidDateString(policyLifeEndDate, mustBeGreaterThanDefaultDate: true))
                            if (PolLifeStartDtConversionResult && PolLifeEndDtConversionResult)
                            {
                                if (CheckDate < dtPolicyLifeStartDate || CheckDate > dtPolicyLifeEndDate)
                                {
                                    hasDateOutsideOfPolicyLife = true;
                                    //message doesn't currently need these, but we'll have them just in case
                                    if (pastDate < dtPolicyLifeStartDate)
                                    {
                                        beginChangeDate = dtPolicyLifeStartDate.ToShortDateString();
                                    }
                                    else
                                    {
                                        beginChangeDate = pastDate.ToShortDateString();
                                    }
                                    if (futureDate > dtPolicyLifeEndDate)
                                    {
                                        endChangeDate = dtPolicyLifeEndDate.ToShortDateString();
                                    }
                                    else
                                    {
                                        endChangeDate = futureDate.ToShortDateString();
                                    }
                                }
                            }
                            else
                            {
                                hasDateOutsideOfPolicyLife = true;
                                //message doesn't currently need these, but we'll have them just in case
                                if (pastDate < QuoteEffectiveDate)
                                {
                                    beginChangeDate = QuoteEffectiveDate.ToShortDateString();
                                }else
                                {
                                    beginChangeDate = pastDate.ToShortDateString();
                                }
                                if (futureDate > QuoteExpirationDate)
                                {
                                    endChangeDate = QuoteExpirationDate.ToShortDateString();
                                }
                                else
                                {
                                    endChangeDate = futureDate.ToShortDateString();
                                }
                            }
                            if (hasDateOutsideOfPolicyLife)
                            {
                                valList.Add(new ValidationItem("Invalid Effective Date", EndorsementEffectiveDateOutOfPolicy));
                                valList.Add(new ValidationItem("Transaction Effective Date is outside of the policy term. Please select a date within the policy term to continue.", EndorsementEffectiveDateOutOfPolicyLongMessage));
                            }
                        }
                    }
                }
                
                

            }

            return valList;
        }

        //Added 11/13/2020 For CAP Endorsements task 52969 MLW
        public static Validation.ObjectValidation.ValidationItemList ValidateTypeOfEndorsement(string typeOfEndorsement)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            //Validate Type of Endorsement

            if (string.IsNullOrWhiteSpace(typeOfEndorsement))
            {
                valList.Add(new ValidationItem("Missing Type of Endorsement", EndorsementType));
            }
            return valList;
        }
    }
}