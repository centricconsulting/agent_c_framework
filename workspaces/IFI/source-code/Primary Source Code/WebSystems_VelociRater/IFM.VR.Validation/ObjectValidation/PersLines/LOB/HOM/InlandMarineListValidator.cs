using IFM.Common.InputValidation;
using QuickQuote.CommonObjects;
using IFM.VR.Common.Helpers.HOM;
using System.Linq;


namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class InlandMarineListValidator
    {
        public const string ValidationListID = "{D6F56FD5-0CA7-4ADD-96E9-F11C7B5E4904}";

        public const string QuoteIsNull = "{66C84371-06D2-432E-B123-7307C182759A}";
        public const string UnExpectedLobType = "{BBBA85FA-5A19-41B5-B8F2-6E26C665079E}";
        public const string NoLocation = "{58502EEA-0CFC-4BD7-99EE-5F130E8C6E78}";
        public const string InlandMarineNull = "{C5E9909F-4622-47F4-B4F4-723BFAB330D1}";

        public const string LimitValueNotGreaterThanZero = "{29545366-BD8C-47D6-A598-D30D9104108D}";
        public const string TypeIdNotDefined = "{E2C07B2C-3098-49A1-BB98-CC44F3E8DFCC}";

        //public const string Single_Item_Jewelry_Arts_Furs_Exceeded_5000 = "{3167EDB7-9821-4E8E-A7A1-82B66CF49983}";
        public const string Combined_Jewelry_Exceeded_30000 = "{6FB69ACF-CA48-48F7-9C15-F1E62C95AEF6}";

        public const string Combined_JewelryVault_Exceeded_30000 = "{3167EDB7-9821-4E8E-A7A1-82B66CF49983}";
        public const string Combined_ArtsBreak_Exceeded_30000 = "{E3F9850D-2BBF-40FC-8423-D268BF195F45}";
        public const string Combined_ArtsNoBreak_Exceeded_30000 = "{D8EB947F-44BC-44DC-913F-D09FEDD6FB27}";
        public const string Combined_Furs_Exceeded_30000 = "{33D3A170-2C01-48F4-8BEA-78ED8CD1D37B}";

        //public const string Single_Gun_Limit_Exceeded = "{468E8E14-0CE2-46D3-A07F-16DE2DE3CF0F}";
        public const string Combined_Guns_Limit_Exceeded = "{07185A03-AEAD-4DAF-996C-BF08C3372EE9}";

        //added by KLJ for 74983
        public const string Single_Jewelry_Limit_Exceeded = "{7EB7F352-E3CD-4D1E-8F48-18C785676AFE}";
        public const string Single_Jewelry_Appraisal_Needed = "{5F344963-BCEA-4D69-B511-10EC420EEDA1}";
        public const string Single_JewelryVault_Limit_Exceeded = "{A8FE297D-0CD8-4436-9358-32AC576B4D51}";
        public const string Single_JewelryVault_Appraisal_Needed = "{95382004-EDF2-44BC-8207-163F4BBF35FD}";
        public static Validation.ObjectValidation.ValidationItemList ValidateHOMInlandMarine(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            //HOM, FAR uses this
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal || quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                {
                    if (quote.Locations != null && quote.Locations.Any() && quote.Locations[0] != null)
                    {
                        var MyLocation = quote.Locations[0];

                        if (MyLocation.InlandMarines != null)
                        {
                            // each limit should be greater than zero
                            foreach (var i in MyLocation.InlandMarines)
                            {
                                VRGeneralValidations.Val_IsGreaterThanZeroNumber(i.IncreasedLimit, valList, LimitValueNotGreaterThanZero, "Limit");
                                if (i.InlandMarineType == QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.None)
                                {
                                    valList.Add(new ObjectValidation.ValidationItem("Invalid Item Type", TypeIdNotDefined));
                                }
                            }

                            //Updated 11/22/2019 for bug 27286 MLW
                            //if (valType != ValidationItem.ValidationType.issuance)
                            if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal || (valType != ValidationItem.ValidationType.issuance && quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm))
                            {
                                // When the limit of a single Jewelry, Jewelry in Vault, Fine Arts with Breakage, Fine Arts without Breakage and/or Fur Inland Marine item exceeds $5,000
                                // , display an error after rate and the Route to Underwriting button

                                //foreach (var i in MyLocation.InlandMarines)
                                //{
                                //    if (i.InlandMarineType == QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry
                                //        | i.InlandMarineType == QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault
                                //        | i.InlandMarineType == QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage
                                //        | i.InlandMarineType == QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage
                                //        | i.InlandMarineType == QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs)
                                //    {
                                //        if (InputHelpers.TryToGetDouble(i.IncreasedLimit) > 5000)
                                //        {
                                //            valList.Add(new ObjectValidation.ValidationItem("Limit of Named Coverage exceeds authority. Please refer to Underwriting (Named Coverage will be coverage selected: Jewelry, Jewelry in Vault, Fine Arts with Breakage, Fine Arts without Breakage or  Furs)"
                                //                , Single_Item_Jewelry_Arts_Furs_Exceeded_5000, valType != ValidationItem.ValidationType.issuance));
                                //        }
                                //    }
                                //}

                                // When the total limit of Jewelry items is greater than $30,000, display an error after rate and the Route to Underwriting button
                                double totalJewelry_Limit = 0.0;
                                foreach (var i in MyLocation.InlandMarines)
                                {
                                    if (i.InlandMarineType == QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry)
                                    {
                                        totalJewelry_Limit += InputHelpers.TryToGetDouble(i.IncreasedLimit);
                                    }
                                }
                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                                {
                                    if (!RemoveValidationsHelper.IsRemoveValidationsAvailable(quote))
                                    {
                                        if (totalJewelry_Limit > 30000)
                                        {
                                            valList.Add(new ObjectValidation.ValidationItem("Limit of Jewelry total scheduled amount is above authority. Please refer to Underwriting", Combined_Jewelry_Exceeded_30000, true));
                                        }
                                    }

                                }
                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.Farm)
                                {
                                     if (totalJewelry_Limit > 30000)
                                        {
                                            valList.Add(new ObjectValidation.ValidationItem("Limit of Jewelry total scheduled amount is above authority. Please refer to Underwriting", Combined_Jewelry_Exceeded_30000, true));
                                        }
                                }

                                // When the total limit of Jewelry in Vault items is greater than $30,000, display an error after rate and the Route to Underwriting button
                                double totalJewelryVault_Limit = 0.0;
                                foreach (var i in MyLocation.InlandMarines)
                                {
                                    if (i.InlandMarineType == QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault)
                                    {
                                        totalJewelryVault_Limit += InputHelpers.TryToGetDouble(i.IncreasedLimit);
                                    }
                                }
                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                                {
                                    if (!RemoveValidationsHelper.IsRemoveValidationsAvailable(quote))
                                    {
                                        if (totalJewelryVault_Limit > 30000)
                                        {
                                            valList.Add(new ObjectValidation.ValidationItem("Limit of Jewelry in Vault total scheduled amount is above authority. Please refer to Underwriting", Combined_JewelryVault_Exceeded_30000, true));
                                        }
                                    }
                                }
                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.Farm)
                                {
                                    if (totalJewelryVault_Limit > 30000)
                                    {
                                        valList.Add(new ObjectValidation.ValidationItem("Limit of Jewelry in Vault total scheduled amount is above authority. Please refer to Underwriting", Combined_JewelryVault_Exceeded_30000, true));
                                    }
                                }

                                // When the total limit of Fine Arts with Breakage items is greater than $30,000, display an error after rate and the Route to Underwriting button
                                double totalArtsBreak_Limit = 0.0;
                                foreach (var i in MyLocation.InlandMarines)
                                {
                                    if (i.InlandMarineType == QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage)
                                    {
                                        totalArtsBreak_Limit += InputHelpers.TryToGetDouble(i.IncreasedLimit);
                                    }
                                }
                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                                {
                                    if (!RemoveValidationsHelper.IsRemoveValidationsAvailable(quote))
                                    {
                                        if (totalArtsBreak_Limit > 30000)
                                        {
                                            valList.Add(new ObjectValidation.ValidationItem("Limit of Fine Arts total scheduled amount is above authority. Please refer to Underwriting", Combined_ArtsBreak_Exceeded_30000, true));
                                        }
                                    }
                                }
                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.Farm)
                                {
                                    if (totalArtsBreak_Limit > 30000)
                                    {
                                        valList.Add(new ObjectValidation.ValidationItem("Limit of Fine Arts total scheduled amount is above authority. Please refer to Underwriting", Combined_ArtsBreak_Exceeded_30000, true));
                                    }
                                }

                                // When the total limit of Fine Arts without Breakage items is greater than $30,000, display an error after rate and the Route to Underwriting button
                                double totalArtsNoBreak_Limit = 0.0;
                                foreach (var i in MyLocation.InlandMarines)
                                {
                                    if (i.InlandMarineType == QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage)
                                    {
                                        totalArtsNoBreak_Limit += InputHelpers.TryToGetDouble(i.IncreasedLimit);
                                    }
                                }
                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                                {
                                    if (!RemoveValidationsHelper.IsRemoveValidationsAvailable(quote))
                                    {
                                        if (totalArtsNoBreak_Limit > 30000)
                                        {
                                            valList.Add(new ObjectValidation.ValidationItem("Limit of Fine Arts total scheduled amount is above authority. Please refer to Underwriting", Combined_ArtsNoBreak_Exceeded_30000, true));
                                        }
                                    }
                                }
                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.Farm)
                                {
                                    if (totalArtsNoBreak_Limit > 30000)
                                    {
                                        valList.Add(new ObjectValidation.ValidationItem("Limit of Fine Arts total scheduled amount is above authority. Please refer to Underwriting", Combined_ArtsNoBreak_Exceeded_30000, true));
                                    }
                                }

                                // When the total limit of Fur items is greater than $30,000, display an error after rate and the Route to Underwriting button
                                double totalFurs_Limit = 0.0;
                                foreach (var i in MyLocation.InlandMarines)
                                {
                                    if (i.InlandMarineType == QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs)
                                    {
                                        totalFurs_Limit += InputHelpers.TryToGetDouble(i.IncreasedLimit);
                                    }
                                }
                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                                {
                                    if (!RemoveValidationsHelper.IsRemoveValidationsAvailable(quote))
                                    {
                                        if (totalFurs_Limit > 30000)
                                        {
                                            valList.Add(new ObjectValidation.ValidationItem("Limit of Furs total scheduled amount is above authority. Please refer to Underwriting", Combined_Furs_Exceeded_30000, true));
                                        }
                                    }
                                }
                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.Farm)
                                {
                                    if (totalFurs_Limit > 30000)
                                    {
                                        valList.Add(new ObjectValidation.ValidationItem("Limit of Furs total scheduled amount is above authority. Please refer to Underwriting", Combined_Furs_Exceeded_30000, true));
                                    }
                                }

                                // When the total limit of guns is greater than $5,000, display an error after rate and the Route to Underwriting button
                                double totalGun_Collection_Limit = 0.0;
                                foreach (var i in MyLocation.InlandMarines)
                                {
                                    if (i.InlandMarineType == QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns)
                                    {
                                        totalGun_Collection_Limit += InputHelpers.TryToGetDouble(i.IncreasedLimit);
                                    }
                                }
                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                                {
                                    if (!RemoveValidationsHelper.IsRemoveValidationsAvailable(quote))
                                    {
                                        if (totalGun_Collection_Limit > 5000)
                                        {
                                            valList.Add(new ObjectValidation.ValidationItem("Large gun collections in excess of $5,000 must be pre-approved by your Underwriter prior to binding coverage.", Combined_Guns_Limit_Exceeded, true));
                                            valList.Add(new ObjectValidation.ValidationItem("Schedules in excess of $5,000 must have all guns inspected for serial numbers and verify use of safety precautions", Combined_Guns_Limit_Exceeded, true));
                                        }
                                    }
                                }
                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.Farm)
                                {
                                    if (totalGun_Collection_Limit > 5000)
                                    {
                                        valList.Add(new ObjectValidation.ValidationItem("Large gun collections in excess of $5,000 must be pre-approved by your Underwriter prior to binding coverage.", Combined_Guns_Limit_Exceeded, true));
                                        valList.Add(new ObjectValidation.ValidationItem("Schedules in excess of $5,000 must have all guns inspected for serial numbers and verify use of safety precautions", Combined_Guns_Limit_Exceeded, true));
                                    }
                                }

                                // Validate Jewelry in Vault line items
                                // Updated per task 73117, changed the amount on the appraisal message from $2500 to $5000 05/04/2022 BD
                                // Changed the IncreasedLimit $2500 to $5000 05/04/2022 BD
                                //Moved to list vlaidation to avoid duplication
                                if (quote.QuoteTransactionType != QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                                {
                                    foreach (var im in MyLocation.InlandMarines)
                                    {
                                        if (im.InlandMarineType == QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry && im.IncreasedLimit != "")
                                        {
                                            if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                                            {
                                                if (!RemoveValidationsHelper.IsRemoveValidationsAvailable(quote))
                                                {
                                                    if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) >= 5000 &&
                                                        !valList.Any((vi) => vi.FieldId == Single_Jewelry_Limit_Exceeded))
                                                        valList.Add(new ObjectValidation.ValidationItem("Limit of Jewelry; Item is above authority. This quote will need to be routed to UW at Issue", Single_Jewelry_Limit_Exceeded, true));

                                                    if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) >= 5000 &&
                                                        !valList.Any((vi) => vi.FieldId == Single_Jewelry_Appraisal_Needed))
                                                        valList.Add(new ObjectValidation.ValidationItem("A recent appraisal (less than two years old) is required for each item of Jewelry with a value of $5000 or more", Single_Jewelry_Appraisal_Needed, true));
                                                }
                                            }

                                            if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.Farm)
                                            {
                                                if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) > 5000)
                                                    valList.Add(new ObjectValidation.ValidationItem("A recent appraisal (less than two years old) is required for each item of Jewelry with a value of $5000 or more", Single_Jewelry_Appraisal_Needed, true));
                                            }
                                        }

                                        if (im.InlandMarineType == QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault && im.IncreasedLimit != "")
                                        {
                                            if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                                            {
                                                if (!RemoveValidationsHelper.IsRemoveValidationsAvailable(quote))
                                                {
                                                    if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) >= 5000 &&
                                                    !valList.Any((vi) => vi.FieldId == Single_JewelryVault_Limit_Exceeded))
                                                        valList.Add(new ObjectValidation.ValidationItem("Limit of Jewelry in Vault; Item is above authority. This quote will need to be routed to UW at Issue", Single_JewelryVault_Limit_Exceeded, true));

                                                    if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) >= 5000 &&
                                                        !valList.Any((vi) => vi.FieldId == Single_JewelryVault_Appraisal_Needed))
                                                        valList.Add(new ObjectValidation.ValidationItem("A recent appraisal (less than two years old) is required for each item of Jewelry in Vault with a value of $5000 or more", Single_JewelryVault_Appraisal_Needed, true));
                                                }
                                            }

                                            if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.Farm)
                                            {
                                                if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) > 5000)
                                                    valList.Add(new ObjectValidation.ValidationItem("A recent appraisal (less than two years old) is required for each item of Jewelry in Vault with a value of $5000 or more", Single_JewelryVault_Appraisal_Needed, true));

                                            }
                                        }
                                    }
                                }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 
                            }
                        }
                        else
                        {
                            valList.Add(new ObjectValidation.ValidationItem("Inland Marine is null", InlandMarineNull));
                        }
                    }
                    else
                    {
                        valList.Add(new ObjectValidation.ValidationItem("No Location", NoLocation));
                    }
                }
                else
                {
                    valList.Add(new ObjectValidation.ValidationItem("Invalid LOB type", UnExpectedLobType));
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