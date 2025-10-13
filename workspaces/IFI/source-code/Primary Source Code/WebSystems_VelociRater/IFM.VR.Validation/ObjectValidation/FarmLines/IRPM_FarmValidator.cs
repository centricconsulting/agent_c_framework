using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IFM.Common.InputValidation;
using IFM.PrimativeExtensions;

namespace IFM.VR.Validation.ObjectValidation.FarmLines
{
    public class IRPM_FarmValidator
    {
        public const string ValidationListID = "{44259F07-EEFB-4B75-A0C1-BEAA8F1ECB8A}";

        public const string QuoteIsNull = "{6F0F828B-14DB-455E-9EE2-7C13001A4DBA}";
        public const string UnExpectedLobType = "{B40EB474-7B3C-4913-ACC7-2480DD3BD7D0}";
        public const string MSQuoteNotFound = "{D22BFDCA-1BB9-4A7C-8136-CB94576487CB}";
        public const string MSQuoteIsNull = "{A9FB6EB7-BBC0-4AB2-9481-6DFF5BCEA296}";
        public const string IRMP_Value = "{836CE9DF-F1E0-40DB-9FC4-80B7CEE8AE7A}";
        public const string IRMP_TotalValue = "{36E89630-B67B-4C8F-A05D-A671964DF572}";


        public static Validation.ObjectValidation.ValidationItemList ValidateIRPM(QuickQuote.CommonObjects.QuickQuoteObject quote,  ValidationItem.ValidationType valType)
        {
            //FAR uses this
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                if (quote.QuoteTransactionType != QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                {
                    if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                    {
                        //Updated 9/7/18 for multi state MLW - qqh, parts, SubQuoteFirst, Quote to SubQuoteFirst
                        QuickQuote.CommonMethods.QuickQuoteHelperClass qqh = new QuickQuote.CommonMethods.QuickQuoteHelperClass();
                        var parts = qqh.MultiStateQuickQuoteObjects(ref quote);
                        if (parts != null)
                        {
                            var SubQuoteFirst = parts.GetItemAtIndex(0);
                            if (SubQuoteFirst != null)
                            {
                                List<double> IRMP_Values = new List<double>();

                                IRMP_Values.Add(InputHelpers.TryToGetDouble(SubQuoteFirst.IRPM_FAR_SupportingBusiness));
                                IRMP_Values.Add(InputHelpers.TryToGetDouble(SubQuoteFirst.IRPM_FAR_CareConditionOfEquipPremises));
                                IRMP_Values.Add(InputHelpers.TryToGetDouble(SubQuoteFirst.IRPM_FAR_DamageSusceptibility));
                                IRMP_Values.Add(InputHelpers.TryToGetDouble(SubQuoteFirst.IRPM_FAR_DispersionOrConcentration));
                                IRMP_Values.Add(InputHelpers.TryToGetDouble(SubQuoteFirst.IRPM_FAR_Location));
                                IRMP_Values.Add(InputHelpers.TryToGetDouble(SubQuoteFirst.IRPM_FAR_MiscProtectFeaturesOrHazards));
                                IRMP_Values.Add(InputHelpers.TryToGetDouble(SubQuoteFirst.IRPM_FAR_RoofCondition));
                                IRMP_Values.Add(InputHelpers.TryToGetDouble(SubQuoteFirst.IRPM_FAR_SuperiorOrInferiorStructureFeatures));
                                IRMP_Values.Add(InputHelpers.TryToGetDouble(SubQuoteFirst.IRPM_FAR_PastLosses));
                                IRMP_Values.Add(InputHelpers.TryToGetDouble(SubQuoteFirst.IRPM_FAR_UseOfRiceHullsOrFlameRetardantBedding));
                                IRMP_Values.Add(InputHelpers.TryToGetDouble(SubQuoteFirst.IRPM_FAR_RegularOnsiteInspections));

                                double totalValue = 0.0;
                                foreach (var val in IRMP_Values)
                                {
                                    VRGeneralValidations.Val_IsNumberInRange(val.ToString(), valList, IRMP_Value, "IRPM Value", ".95", "1.05");
                                    totalValue += val;
                                }

                                if (totalValue < (IRMP_Values.Count - .15) || totalValue > (IRMP_Values.Count + .15))
                                {
                                    //valList.Add(new ObjectValidation.ValidationItem("Maximum premium adjustment of (+/-)15% was exceeded.", IRMP_TotalValue)); //removed 10/8/2020 w/ Interoperability project
                                }
                            }
                            else
                            {
                                valList.Add(new ObjectValidation.ValidationItem("Multi State quote is null", MSQuoteIsNull));
                            }
                        }
                        else
                        {
                            valList.Add(new ObjectValidation.ValidationItem("Multi State quote not found", MSQuoteNotFound));
                        }
                    }
                    else
                    {
                        valList.Add(new ObjectValidation.ValidationItem("Invalid LOB type", UnExpectedLobType));
                    }
                }
            }
            else
            {
                valList.Add(new ObjectValidation.ValidationItem("Quote is null", QuoteIsNull));
            }
            return valList;
        }
    }
}