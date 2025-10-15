using IFM.VR.Common.Helpers.HOM;
using QuickQuote.CommonMethods;
using QuickQuote.CommonObjects;
using System.Collections.Generic;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class InlandMarineValidator
    {
        //removed 3/11/2021
        //private static int _rowNumber;

        //public static int RowNumber
        //{
        //    get
        //    {
        //        return _rowNumber;
        //    }
        //    set
        //    {
        //        _rowNumber = value;
        //    }
        //}

        private static QuickQuoteHelperClass qqHelper = new QuickQuoteHelperClass();

        public const string ValidationListID = "{E79F68C9-AC5B-4311-82F1-92161E7A509F}";

        public const string QuoteIsNull = "{2DE733F6-D4C8-4726-82FD-F45FDAACB736}";
        public const string UnExpectedLobType = "";
        public const string NoLocation = "{4194788D-A2FB-4744-B130-24CC55ACFEAD}";
        public const string InlandMarineNull = "{11576741-9C7E-45FF-9601-BAE5C843E048}";

        public const string LimitValueNotGreaterThanZero = "{0F617C80-8C27-48A4-8A97-F9F127AAB70B}";
        public const string TypeIdNotDefined = "{AC29E47D-8724-4247-8489-E75AA66A7BA8}";

        public const string IMLimitAmount = "{40D7597B-2461-4A5C-B1E0-E911F0AA907A}";
        public const string IMDeductible = "{7C3F2887-5A19-4DB7-8766-6DBA659C6120}";
        public const string IMDescription = "{1C451647-C3DE-4B6A-9B5F-6B0705863AEB}";
        public const string IMStorageLocation = "{b0960e8f-9319-4103-9399-e2c87ce0d137}";

        public const string Single_Jewelry_Limit_Exceeded = "{7EB7F352-E3CD-4D1E-8F48-18C785676AFE}";
        public const string Single_Jewelry_Appraisal_Needed = "{5F344963-BCEA-4D69-B511-10EC420EEDA1}";
        public const string Single_JewelryVault_Limit_Exceeded = "{A8FE297D-0CD8-4436-9358-32AC576B4D51}";
        public const string Single_JewelryVault_Appraisal_Needed = "{95382004-EDF2-44BC-8207-163F4BBF35FD}";
        public const string Single_ArtsBreak_Limit_Exceeded = "{A735379C-1CFC-4BE5-A31E-F6FBD4D8A61C}";
        public const string Single_ArtsBreak_Doco_Needed = "{E6E6AF8A-A550-494C-80B5-279FFC91BA94}";
        public const string Single_ArtsNoBreak_Limit_Exceeded = "{E564A8C9-09D4-4D2E-9C51-ECEC859E3BF0}";
        public const string Single_ArtsNoBreak_Doco_Needed = "{8FFE0428-1C01-4D16-91BB-8F29E6AF1610}";
        public const string Single_Fur_Limit_Exceeded = "{E564A8C9-09D4-4D2E-9C51-ECEC859E3BF0}";
        public const string Single_Fur_Doco_Needed = "{8FFE0428-1C01-4D16-91BB-8F29E6AF1610}";

        public const string Limit_LessThan_Deductible = "{2238D3C8-95DC-4E5B-8EC8-426917A7C4B3}";

        //added Obsolete attribute 3/11/2021
        [System.Obsolete("This Method is Deprecated. Please use Overload with indexNum parameter instead.")]
        public static Validation.ObjectValidation.ValidationItemList ValidateHOMInlandMarine(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType controlValidate)
        {
            //Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            //if (quote != null)
            //{
            //    if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal || quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
            //    {
            //        if (quote.Locations != null && quote.Locations.Any() && quote.Locations[0] != null)
            //        {
            //            var MyLocation = quote.Locations[0].InlandMarines;

            //            if (MyLocation != null && MyLocation.Count != 0)
            //            {
            //                //Updated 11/22/2019 for bug 27286 MLW
            //                //if (valType != ValidationItem.ValidationType.issuance)
            //                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal || (valType != ValidationItem.ValidationType.issuance && quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm))
            //                {
            //                    List<QuickQuoteInlandMarine> inlandMarineList = MyLocation.FindAll(s => s.InlandMarineType == controlValidate);
            //                    if (inlandMarineList.Count != 0)
            //                    {
            //                        if (inlandMarineList[RowNumber].IncreasedLimit == "")
            //                            valList.Add(new ObjectValidation.ValidationItem("Missing Limit", IMLimitAmount));
            //                        if (inlandMarineList[RowNumber].IncreasedLimit == "0")
            //                            valList.Add(new ObjectValidation.ValidationItem("Invalid Limit", IMLimitAmount));

            //                        if (inlandMarineList[RowNumber].DeductibleLimitId == "0")
            //                            valList.Add(new ObjectValidation.ValidationItem("Missing Deductible", IMDeductible));
            //                        if (inlandMarineList[RowNumber].Description == "")
            //                            valList.Add(new ObjectValidation.ValidationItem("Missing Description", IMDescription));

            //                        // Validate that Limit is not less than the selected Deductible
            //                        if (inlandMarineList[RowNumber].IncreasedLimit != "" && inlandMarineList[RowNumber].DeductibleLimitId != "0")
            //                        {
            //                            if (decimal.Parse(inlandMarineList[RowNumber].IncreasedLimit.Replace(",", "")) <= decimal.Parse(qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteInlandMarine, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, inlandMarineList[RowNumber].DeductibleLimitId)))
            //                                valList.Add(new ObjectValidation.ValidationItem("A deductible equal to or greater than the coverage limit has been entered. Please modify either value to ensure proper coverage.", Limit_LessThan_Deductible, true));
            //                        }
            //                        //Updated 02/10/2020 for Home Endorsements task 43872 MLW
            //                        if (quote.QuoteTransactionType != QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
            //                        {                                  
            //                            // Validate Jewelry line items
            //                            if (controlValidate == QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry && inlandMarineList[RowNumber].IncreasedLimit != "")
            //                            {
            //                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.HomePersonal)
            //                                {
            //                                    if (decimal.Parse(inlandMarineList[RowNumber].IncreasedLimit.Replace(",", "")) > 5000)
            //                                        valList.Add(new ObjectValidation.ValidationItem("Limit of Jewelry; Item is above authority. Please refer to Underwriting", Single_Jewelry_Limit_Exceeded, true));

            //                                    if (decimal.Parse(inlandMarineList[RowNumber].IncreasedLimit.Replace(",", "")) > 2500)
            //                                        valList.Add(new ObjectValidation.ValidationItem("A recent appraisal (less than two years old) is required for each item of Jewelry with a value of $2500 or more", Single_Jewelry_Appraisal_Needed, true));
            //                                }

            //                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.Farm)
            //                                {
            //                                    if (decimal.Parse(inlandMarineList[RowNumber].IncreasedLimit.Replace(",", "")) > 5000)
            //                                        valList.Add(new ObjectValidation.ValidationItem("A recent appraisal (less than two years old) is required for each item of Jewelry with a value of $5000 or more", Single_Jewelry_Appraisal_Needed, true));
            //                                }
            //                            }

            //                            // Validate Jewelry in Vault line items
            //                            if (controlValidate == QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault && inlandMarineList[RowNumber].IncreasedLimit != "")
            //                            {
            //                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.HomePersonal)
            //                                {
            //                                    if (decimal.Parse(inlandMarineList[RowNumber].IncreasedLimit.Replace(",", "")) > 5000)
            //                                        valList.Add(new ObjectValidation.ValidationItem("Limit of Jewelry in Vault; Item is above authority. Please refer to Underwriting", Single_JewelryVault_Limit_Exceeded, true));

            //                                    if (decimal.Parse(inlandMarineList[RowNumber].IncreasedLimit.Replace(",", "")) > 2500)
            //                                        valList.Add(new ObjectValidation.ValidationItem("A recent appraisal (less than two years old) is required for each item of Jewelry in Vault with a value of $2500 or more", Single_JewelryVault_Appraisal_Needed, true));
            //                                }

            //                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.Farm)
            //                                {
            //                                    if (decimal.Parse(inlandMarineList[RowNumber].IncreasedLimit.Replace(",", "")) > 5000)
            //                                        valList.Add(new ObjectValidation.ValidationItem("A recent appraisal (less than two years old) is required for each item of Jewelry in Vault with a value of $5000 or more", Single_JewelryVault_Appraisal_Needed, true));
            //                                }
            //                            }

            //                            // Validate Fine Arts w/ Break line items
            //                            if (controlValidate == QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage && inlandMarineList[RowNumber].IncreasedLimit != "")
            //                            {
            //                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.HomePersonal)
            //                                {
            //                                    if (decimal.Parse(inlandMarineList[RowNumber].IncreasedLimit.Replace(",", "")) > 5000)
            //                                        valList.Add(new ObjectValidation.ValidationItem("Limit of Fine Arts; Item is above authority. Please refer to Underwriting", Single_ArtsBreak_Limit_Exceeded, true));

            //                                    if (decimal.Parse(inlandMarineList[RowNumber].IncreasedLimit.Replace(",", "")) > 2500)
            //                                        valList.Add(new ObjectValidation.ValidationItem("Value of Fine Arts; Your Underwriter will require supporting documentation for values in excess of $2500 (i.e. appraisals, sales receipts, etc.)", Single_ArtsBreak_Doco_Needed, true));
            //                                }

            //                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.Farm)
            //                                {
            //                                    if (decimal.Parse(inlandMarineList[RowNumber].IncreasedLimit.Replace(",", "")) > 5000)
            //                                        valList.Add(new ObjectValidation.ValidationItem("A recent appraisal (less than two years old) is required for each item of Fine Arts with breakage with a value of $5000 or more", Single_ArtsBreak_Doco_Needed, true));
            //                                }
            //                            }

            //                            // Validate Fine Arts w/o Break line items
            //                            if (controlValidate == QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage && inlandMarineList[RowNumber].IncreasedLimit != "")
            //                            {
            //                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.HomePersonal)
            //                                {
            //                                    if (decimal.Parse(inlandMarineList[RowNumber].IncreasedLimit.Replace(",", "")) > 5000)
            //                                        valList.Add(new ObjectValidation.ValidationItem("Limit of Fine Arts; Item is above authority. Please refer to Underwriting", Single_ArtsNoBreak_Limit_Exceeded, true));

            //                                    if (decimal.Parse(inlandMarineList[RowNumber].IncreasedLimit.Replace(",", "")) > 2500)
            //                                        valList.Add(new ObjectValidation.ValidationItem("Value of Fine Arts; Your Underwriter will require supporting documentation for values in excess of $2500 (i.e. appraisals, sales receipts, etc.)", Single_ArtsNoBreak_Doco_Needed, true));
            //                                }

            //                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.Farm)
            //                                {
            //                                    if (decimal.Parse(inlandMarineList[RowNumber].IncreasedLimit.Replace(",", "")) > 5000)
            //                                        valList.Add(new ObjectValidation.ValidationItem("A recent appraisal (less than two years old) is required for each item of Fine Arts without breakage with a value of $5000 or more", Single_ArtsNoBreak_Doco_Needed, true));
            //                                }
            //                            }

            //                            // Validate Fur line items
            //                            if (controlValidate == QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs && inlandMarineList[RowNumber].IncreasedLimit != "")
            //                            {
            //                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.HomePersonal)
            //                                {
            //                                    if (decimal.Parse(inlandMarineList[RowNumber].IncreasedLimit.Replace(",", "")) > 5000)
            //                                        valList.Add(new ObjectValidation.ValidationItem("Limit of Furs; Item is above authority. Please refer to Underwriting", Single_Fur_Limit_Exceeded, true));

            //                                    if (decimal.Parse(inlandMarineList[RowNumber].IncreasedLimit.Replace(",", "")) > 2500)
            //                                        valList.Add(new ObjectValidation.ValidationItem("Value of Furs; Your Underwriter will require supporting documentation for values in excess of $2500 (i.e. appraisals, sales receipts, etc.)", Single_Fur_Doco_Needed, true));
            //                                }

            //                                if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.Farm)
            //                                {
            //                                    if (decimal.Parse(inlandMarineList[RowNumber].IncreasedLimit.Replace(",", "")) > 5000)
            //                                        valList.Add(new ObjectValidation.ValidationItem("A recent appraisal (less than two years old) is required for each item of Furs with a value of $5000 or more", Single_Fur_Doco_Needed, true));
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                valList.Add(new ObjectValidation.ValidationItem("Inland Marine is null", InlandMarineNull));
            //            }
            //        }
            //        else
            //        {
            //            valList.Add(new ObjectValidation.ValidationItem("No Location", NoLocation));
            //        }
            //    }
            //    else
            //    {
            //        valList.Add(new ObjectValidation.ValidationItem("Invalid LOB type", UnExpectedLobType));
            //    }
            //}
            //else
            //{
            //    valList.Add(new ObjectValidation.ValidationItem("Quote is null", QuoteIsNull));
            //}
            //return valList;

            //updated 3/11/2021 to use new function
            return ValidateHOMInlandMarine(quote, valType, controlValidate, 0);
        }


        //breaking up logic 3/11/2021
        public static Validation.ObjectValidation.ValidationItemList ValidateHOMInlandMarine(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType imTypeToValidate, int indexNum)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal || quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                {
                    if (quote.Locations != null && quote.Locations.Any() && quote.Locations[0] != null)
                    {
                        var ims = quote.Locations[0].InlandMarines;

                        if (ims != null && ims.Count > indexNum)
                        {
                            //Updated 11/22/2019 for bug 27286 MLW
                            //if (valType != ValidationItem.ValidationType.issuance)
                            if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal || (valType != ValidationItem.ValidationType.issuance && quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm))
                            {
                                List<QuickQuoteInlandMarine> inlandMarineList = ims.FindAll(s => s.InlandMarineType == imTypeToValidate);
                                if (inlandMarineList != null && inlandMarineList.Count > indexNum)
                                {
                                    return ValidateHOMInlandMarine(quote, inlandMarineList[indexNum], valType);                                    
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

        public static Validation.ObjectValidation.ValidationItemList ValidateHOMInlandMarine(QuickQuote.CommonObjects.QuickQuoteObject quote, QuickQuote.CommonObjects.QuickQuoteInlandMarine im, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal || quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                {
                    if (quote.Locations != null && quote.Locations.Any() && quote.Locations[0] != null)
                    {
                        if (im != null)
                        {
                            //Updated 11/22/2019 for bug 27286 MLW
                            //if (valType != ValidationItem.ValidationType.issuance)
                            if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal || (valType != ValidationItem.ValidationType.issuance && quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm))
                            {
                                if (im.IncreasedLimit == "")
                                    valList.Add(new ObjectValidation.ValidationItem("Missing Limit", IMLimitAmount));
                                if (im.IncreasedLimit == "0")
                                    valList.Add(new ObjectValidation.ValidationItem("Invalid Limit", IMLimitAmount));

                                if (im.DeductibleLimitId == "0")
                                    valList.Add(new ObjectValidation.ValidationItem("Missing Deductible", IMDeductible));
                                if (im.Description == "")
                                    valList.Add(new ObjectValidation.ValidationItem("Missing Description", IMDescription));
                                if (im.InlandMarineType == QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GraveMarkers && im.StorageLocation == "")
                                    valList.Add(new ObjectValidation.ValidationItem("Missing StorageLocation", IMStorageLocation));
                                // Validate that Limit is not less than the selected Deductible
                                if (im.IncreasedLimit != "" && im.DeductibleLimitId != "0")
                                {
                                    if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) <= decimal.Parse(qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteInlandMarine, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, im.DeductibleLimitId)))
                                        valList.Add(new ObjectValidation.ValidationItem("A deductible equal to or greater than the coverage limit has been entered. Please modify either value to ensure proper coverage.", Limit_LessThan_Deductible, true));
                                }
                                //Updated 02/10/2020 for Home Endorsements task 43872 MLW
                                if (quote.QuoteTransactionType != QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                                {
                                    // Validate Jewelry line items
                                    // Updated per task 73117, changed the amount on the appraisal message from $2500 to $5000 05/04/2022 BD
                                    // Changed the IncreasedLimit $2500 to $5000 05/04/2022 BD
                                    //if (im.InlandMarineType == QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry && im.IncreasedLimit != "")
                                    //{
                                    //    if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                                    //    {
                                    //        if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) >= 5000)
                                    //            valList.Add(new ObjectValidation.ValidationItem("Limit of Jewelry; Item is above authority. Please refer to Underwriting", Single_Jewelry_Limit_Exceeded, true));

                                    //        if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) >= 5000)
                                    //            valList.Add(new ObjectValidation.ValidationItem("A recent appraisal (less than two years old) is required for each item of Jewelry with a value of $5000 or more", Single_Jewelry_Appraisal_Needed, true));
                                    //    }

                                    //    if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.Farm)
                                    //    {
                                    //        if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) > 5000)
                                    //            valList.Add(new ObjectValidation.ValidationItem("A recent appraisal (less than two years old) is required for each item of Jewelry with a value of $5000 or more", Single_Jewelry_Appraisal_Needed, true));
                                    //    }
                                    //}

                                    // Validate Jewelry in Vault line items
                                    // Updated per task 73117, changed the amount on the appraisal message from $2500 to $5000 05/04/2022 BD
                                    // Changed the IncreasedLimit $2500 to $5000 05/04/2022 BD
                                    //if ((im.InlandMarineType == QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault ||
                                    //     im.InlandMarineType == QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry) && 
                                    //    im.IncreasedLimit != "")
                                    //{
                                    //    if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                                    //    {
                                    //        if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) >= 5000 &&
                                    //            !valList.Any((vi)=>vi.FieldId == Single_JewelryVault_Limit_Exceeded))
                                    //            valList.Add(new ObjectValidation.ValidationItem("Limit of Jewelry in Vault; Item is above authority. Please refer to Underwriting", Single_JewelryVault_Limit_Exceeded, true));

                                    //        if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) >= 5000 &&
                                    //            !valList.Any((vi) => vi.FieldId == Single_JewelryVault_Appraisal_Needed))
                                    //            valList.Add(new ObjectValidation.ValidationItem("A recent appraisal (less than two years old) is required for each item of Jewelry in Vault with a value of $5000 or more", Single_JewelryVault_Appraisal_Needed, true));
                                    //    }

                                    //    if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.Farm)
                                    //    {
                                    //        if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) > 5000)
                                    //            valList.Add(new ObjectValidation.ValidationItem("A recent appraisal (less than two years old) is required for each item of Jewelry in Vault with a value of $5000 or more", Single_JewelryVault_Appraisal_Needed, true));
                                    //    }
                                    //}

                                    // Validate Fine Arts w/ Break line items
                                    if (im.InlandMarineType == QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage && im.IncreasedLimit != "")
                                    {
                                        if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                                        {
                                            if (!RemoveValidationsHelper.IsRemoveValidationsAvailable(quote))
                                            {
                                                if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) > 5000)
                                                    valList.Add(new ObjectValidation.ValidationItem("Limit of Fine Arts; Item is above authority. Please refer to Underwriting", Single_ArtsBreak_Limit_Exceeded, true));

                                                if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) > 2500)
                                                    valList.Add(new ObjectValidation.ValidationItem("Value of Fine Arts; Your Underwriter will require supporting documentation for values in excess of $2500 (i.e. appraisals, sales receipts, etc.)", Single_ArtsBreak_Doco_Needed, true));
                                            }
                                        }

                                        if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.Farm)
                                        {
                                            if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) > 5000)
                                                valList.Add(new ObjectValidation.ValidationItem("A recent appraisal (less than two years old) is required for each item of Fine Arts with breakage with a value of $5000 or more", Single_ArtsBreak_Doco_Needed, true));
                                        }
                                    }

                                    // Validate Fine Arts w/o Break line items
                                    if (im.InlandMarineType == QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage && im.IncreasedLimit != "")
                                    {
                                        if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                                        {
                                            if (!RemoveValidationsHelper.IsRemoveValidationsAvailable(quote))
                                            {
                                                if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) > 5000)
                                                    valList.Add(new ObjectValidation.ValidationItem("Limit of Fine Arts; Item is above authority. Please refer to Underwriting", Single_ArtsNoBreak_Limit_Exceeded, true));

                                                if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) > 2500)
                                                    valList.Add(new ObjectValidation.ValidationItem("Value of Fine Arts; Your Underwriter will require supporting documentation for values in excess of $2500 (i.e. appraisals, sales receipts, etc.)", Single_ArtsNoBreak_Doco_Needed, true));
                                            }
                                        }

                                        if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.Farm)
                                        {
                                            if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) > 5000)
                                                valList.Add(new ObjectValidation.ValidationItem("A recent appraisal (less than two years old) is required for each item of Fine Arts without breakage with a value of $5000 or more", Single_ArtsNoBreak_Doco_Needed, true));
                                        }
                                    }

                                    // Validate Fur line items
                                    if (im.InlandMarineType == QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs && im.IncreasedLimit != "")
                                    {
                                        if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                                        {
                                            if (!RemoveValidationsHelper.IsRemoveValidationsAvailable(quote))
                                            {
                                                if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) > 5000)
                                                    valList.Add(new ObjectValidation.ValidationItem("Limit of Furs; Item is above authority. Please refer to Underwriting", Single_Fur_Limit_Exceeded, true));

                                                if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) > 2500)
                                                    valList.Add(new ObjectValidation.ValidationItem("Value of Furs; Your Underwriter will require supporting documentation for values in excess of $2500 (i.e. appraisals, sales receipts, etc.)", Single_Fur_Doco_Needed, true));
                                            }
                                        }

                                        if (quote.LobType == QuickQuoteObject.QuickQuoteLobType.Farm)
                                        {
                                            if (decimal.Parse(im.IncreasedLimit.Replace(",", "")) > 5000)
                                                valList.Add(new ObjectValidation.ValidationItem("A recent appraisal (less than two years old) is required for each item of Furs with a value of $5000 or more", Single_Fur_Doco_Needed, true));
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