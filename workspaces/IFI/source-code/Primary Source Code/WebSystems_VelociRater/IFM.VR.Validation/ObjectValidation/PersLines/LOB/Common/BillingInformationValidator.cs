using QuickQuote.CommonMethods;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common
{
    public class BillingInformationValidator
    {
        public const string ValidationListID = "{41A987D7-4CC8-4D36-A63F-F0DAF13C328D}";

        public const string Billto = "{2D1CE47C-35BD-43FA-8E30-6AB843F886F7}";
        public const string Method = "{D30E551E-C8AA-41F0-95A1-F7C93F048541}";
        public const string Billto_Method_Combination = "{0A0F3B2E-4607-4D0E-9254-1E395792270E}";
        public const string PayPlan = "{DCD9EB50-E879-40FD-ADDA-0068261C2782}";

        public const string BillingFirstName = "{A0E8A2F1-AEAF-4DDC-9B1B-EF061737A053}";
        public const string BillingLastName = "{A94BDC2A-5C21-43D3-A4D6-831F2D4E6FCC}";

        public const string BillingAddressStreetAndPoBoxEmpty = "{459EF6DF-01EA-4586-A419-949ADC1C339F}";
        public const string BillingAddressStreetAndPoBoxAreSet = "{41120429-D005-4D4A-8202-EF8AF6BB854A}";
        public const string BillingAddressEmpty = "{52F6FF9E-67D8-4F0B-8387-7336F3783426}";
        public const string BillingAddressStreetNumber = "{C4E6865F-9FBD-4708-99D3-B99D5E303414}";
        public const string BillingAddressStreetName = "{F837542B-17B1-4FED-9644-0E4D5596C0D3}";
        public const string BillingAddressPoBox = "{216DF7C2-F708-4037-82CB-F505DCA2B35A}";
        public const string BillingAddressCity = "{D974FFEF-A817-40C2-A4A6-D1A8762B38D3}";
        public const string BillingAddressState = "{53CC873F-CBF1-4339-ADDC-4408DBE16F14}";
        public const string BillingAddressZipCode = "{40128CC7-1F92-480C-A4C7-A507C48AAFA0}";

        public const string BillingEftRouting = "{FF205BAF-2717-4181-834D-DC387FFBCA80}";
        public const string BillingEftAccount = "{8D63BFC8-5B1A-4723-9B0D-11568267D261}";
        public const string BillingEftAccountType = "{00C99F97-A988-4578-AC50-8EB42B75C7D0}";
        public const string BillingEftDeduction = "{031E06CC-4397-46D5-9BF5-BA49BA9F29A9}";
        public const string AgreeToEFTTerms = "{62B716F4-03A3-4372-AB0A-3EC3A63499FF}";

        public static Validation.ObjectValidation.ValidationItemList BillingInformationValidation(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType, string isAgreeToEFTTermsChecked)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            if (quote != null)
            {
                QuickQuoteHelperClass qqHelper = new QuickQuoteHelperClass();

                string billTo_Other_id = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, "Other");
                string billTo_Insured_id = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, "Insured");
                string billTo_Agent_id = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, "Agent");
                string billTo_Mortgagee_id = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, "Mortgagee");

                //string payplan_AnnualMTG = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillingPayPlanId, "Annual MTG");
                //string payplan_EFTMonthly = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillingPayPlanId, "Renewal EFT Monthly 2");
                //updated 9/29/2021
                System.Collections.Generic.List<int> billPayPlanIdsAnnualMTG = qqHelper.BillingPayPlanIdsForPayPlanType(QuickQuoteHelperClass.PayPlanType.AnnualMTG);
                System.Collections.Generic.List<int> billPayPlanIdsEFTMonthly = qqHelper.BillingPayPlanIdsForPayPlanType(QuickQuoteHelperClass.PayPlanType.EftMonthly);

                string method_DirectBill_Id = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId, "Direct Bill");
                string method_AgencyBill_Id = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId, "Agency Bill");

                if (VRGeneralValidations.Val_HasRequiredField_DD(quote.BillToId, valList, Billto, "Bill To"))
                    VRGeneralValidations.Val_IsNumeric(quote.BillToId, valList, Billto, "Bill To");

                var billTo_List = qqHelper.GetStaticDataList(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId);

                // need to make sure you only get items for current quote lob and no 'ignoreForLists' items
                var billTo_isValid = (from o in qqHelper.GetStaticDataOptions(billTo_List, quote.LobType) where o.Value == quote.BillToId & o.ignoreForLists == false select o).Any();
                if (billTo_isValid == false)
                    valList.Add(new ValidationItem("Invalid Bill To", Billto, false));

                if (VRGeneralValidations.Val_HasRequiredField_DD(quote.BillMethodId, valList, Method, "Method"))
                    VRGeneralValidations.Val_IsNumeric(quote.BillMethodId, valList, Method, "Method");

                if (quote.BillMethodId == "1") // agency method
                {
                    //added 9/29/2021
                    QuickQuote.CommonObjects.QuickQuoteStaticDataAttribute att = new QuickQuote.CommonObjects.QuickQuoteStaticDataAttribute();
                    att.nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId;
                    att.nvp_value = "1";
                    System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteStaticDataAttribute> atts = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteStaticDataAttribute>();
                    atts.Add(att);
                    System.Collections.Generic.List<int> agencyBillIds = qqHelper.BillingPayPlanIdsForOptionalParams(optionAttributes: atts);

                    if (VRGeneralValidations.Val_HasRequiredField_DD(quote.BillingPayPlanId, valList, PayPlan, "Pay Plan"))
                        //if (!(quote.BillingPayPlanId == "20" | quote.BillingPayPlanId == "21" | quote.BillingPayPlanId == "22"))
                        //updated 9/29/2021
                        if (agencyBillIds != null && agencyBillIds.Count > 0 && agencyBillIds.Contains(qqHelper.IntegerForString(quote.BillingPayPlanId)) == false)
                            valList.Add(new ValidationItem("Invalid Pay Plan", PayPlan, false));
                }
                else
                {
                    // direct billed
                    if (VRGeneralValidations.Val_HasRequiredField_DD(quote.BillingPayPlanId, valList, PayPlan, "Pay Plan"))
                        VRGeneralValidations.Val_IsNumeric(quote.BillingPayPlanId, valList, PayPlan, "Pay Plan");
                }

                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal | quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm | quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal)
                {
                    // if bill to is mortgagee = 2 then you must have an AI of type Mortgagee = 42
                    // must also be annual MGT playplan
                    if (quote.BillToId == billTo_Mortgagee_id)
                    {
                        bool hasMortgageeType = false;
                        bool hasMortgageeTypeWithBillToSelected = false;
                        if (quote.Locations != null && quote.Locations.Any() && quote.Locations[0].AdditionalInterests != null)
                        {
                            hasMortgageeType = (from a in quote.Locations[0].AdditionalInterests where (a.TypeId == "42" | a.TypeId == "11" | a.TypeId == "15") select a).Any();
                            hasMortgageeTypeWithBillToSelected = (from a in quote.Locations[0].AdditionalInterests where (a.TypeId == "42" | a.TypeId == "11" | a.TypeId == "15") & a.BillTo == true select a).Any();
                        }

                        if (hasMortgageeTypeWithBillToSelected == false)
                        {
                            // called 'Insured' for Farm and 'Interest' on all other LOBs
                            valList.Add(new ValidationItem(string.Format("No Additional {0} Has Been Selected as the Bill To", (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm) ? "Insured" : "Interest"), Billto, false));
                        }
                        //Updated 02/12/2020 for Home Endorsements bug 43977 MLW
                        if (quote.QuoteTransactionType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                        {
                            //if (quote.CurrentPayplanId != payplan_AnnualMTG)
                            //updated 9/29/2021
                            if (billPayPlanIdsAnnualMTG != null && billPayPlanIdsAnnualMTG.Count > 0 && (qqHelper.IsPositiveIntegerString(quote.CurrentPayplanId) == false || billPayPlanIdsAnnualMTG.Contains(qqHelper.IntegerForString(quote.CurrentPayplanId)) == false))
                                valList.Add(new ValidationItem("Invalid Pay Plan for Mortgagee", PayPlan, false));

                        } else {
                            //if (quote.BillingPayPlanId != payplan_AnnualMTG)
                            //updated 9/29/2021
                            if (billPayPlanIdsAnnualMTG != null && billPayPlanIdsAnnualMTG.Count > 0 && (qqHelper.IsPositiveIntegerString(quote.BillingPayPlanId) == false || billPayPlanIdsAnnualMTG.Contains(qqHelper.IntegerForString(quote.BillingPayPlanId)) == false))
                                valList.Add(new ValidationItem("Invalid Pay Plan for Mortgagee", PayPlan, false));
                        }
                    }
                }
                //Updated 02/12/2020 for Home Endorsements bug 43977 MLW
                if (quote.QuoteTransactionType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                {
                    //if (quote.CurrentPayplanId == payplan_AnnualMTG)
                    //updated 9/29/2021
                    if (billPayPlanIdsAnnualMTG != null && billPayPlanIdsAnnualMTG.Count > 0 && qqHelper.IsPositiveIntegerString(quote.CurrentPayplanId) == true && billPayPlanIdsAnnualMTG.Contains(qqHelper.IntegerForString(quote.CurrentPayplanId)) == true)
                    {
                        if (quote.BillToId != billTo_Mortgagee_id)
                        {
                            valList.Add(new ValidationItem("Annual MTG Pay Plan Only Available when Bill To is Mortgagee", PayPlan, false));
                        }
                    }
                }
                else
                {
                    //if (quote.BillingPayPlanId == payplan_AnnualMTG)
                    //updated 9/29/2021
                    if (billPayPlanIdsAnnualMTG != null && billPayPlanIdsAnnualMTG.Count > 0 && qqHelper.IsPositiveIntegerString(quote.BillingPayPlanId) == true && billPayPlanIdsAnnualMTG.Contains(qqHelper.IntegerForString(quote.BillingPayPlanId)) == true)
                    {
                        if (quote.BillToId != billTo_Mortgagee_id)
                        {
                            valList.Add(new ValidationItem("Annual MTG Pay Plan Only Available when Bill To is Mortgagee", PayPlan, false));
                        }
                    }
                }
                    

                if (valType == ValidationItem.ValidationType.appRate || valType == ValidationItem.ValidationType.issuance || quote.QuoteTransactionType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote) // bug 4710 - Matt A - 4-13-15
                {
                    //if (quote.BillingPayPlanId == payplan_EFTMonthly)
                    //updated 9/29/2021
                    //if (billPayPlanIdsEFTMonthly != null && billPayPlanIdsEFTMonthly.Count > 0 && qqHelper.IsPositiveIntegerString(quote.BillingPayPlanId) == true && billPayPlanIdsEFTMonthly.Contains(qqHelper.IntegerForString(quote.BillingPayPlanId)) == true)
                    if (billPayPlanIdsEFTMonthly != null && billPayPlanIdsEFTMonthly.Count > 0 && ((qqHelper.IsPositiveIntegerString(quote.BillingPayPlanId) == true && billPayPlanIdsEFTMonthly.Contains(qqHelper.IntegerForString(quote.BillingPayPlanId)) == true && quote.QuoteTransactionType != QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)) || (quote.QuoteTransactionType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote && qqHelper.IsPositiveIntegerString(quote.CurrentPayplanId) == true && billPayPlanIdsEFTMonthly.Contains(qqHelper.IntegerForString(quote.CurrentPayplanId)) == true))
                    {
                        //if (string.IsNullOrWhiteSpace(quote.EFT_BankRoutingNumber) && string.IsNullOrWhiteSpace(quote.EFT_BankAccountNumber) && quote.LobType != QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                        if (string.IsNullOrWhiteSpace(isAgreeToEFTTermsChecked) || isAgreeToEFTTermsChecked != "True")
                        {
                            valList.Add(new ValidationItem("Missing EFT Terms", AgreeToEFTTerms, false));
                        }
                        else
                        {
                            if (VRGeneralValidations.Val_HasRequiredField(quote.EFT_BankRoutingNumber, valList, BillingEftRouting, "Routing Number"))
                                if (VRGeneralValidations.Val_IsNumeric(quote.EFT_BankRoutingNumber, valList, BillingEftRouting, "Routing Number"))
                                    if (VRGeneralValidations.Val_IsNonNegativeWholeNumber(quote.EFT_BankRoutingNumber, valList, BillingEftRouting, "Routing Number"))
                                    {
                                        if (string.IsNullOrWhiteSpace(IFM.VR.Common.Helpers.EFTHelper.GetBankNameFromAbaLookUp(quote.EFT_BankRoutingNumber)))
                                            valList.Add(new ValidationItem("Invalid Routing Number", BillingEftRouting, false));
                                    }

                            if (VRGeneralValidations.Val_HasRequiredField(quote.EFT_BankAccountNumber, valList, BillingEftAccount, "Account Number"))
                                if (VRGeneralValidations.Val_IsNonNegativeWholeNumber(quote.EFT_BankAccountNumber, valList, BillingEftAccount, "Account Number"))
                                {
                                    //   VRGeneralValidations.Val_IsTextLengthInRange(quote.EFT_BankAccountNumber, valList, BillingEftAccount, "Account Number", "4", "50");
                                }

                            VRGeneralValidations.Val_HasRequiredField_DD(quote.EFT_BankAccountTypeId, valList, BillingEftAccountType, "Account Type");

                            if (VRGeneralValidations.Val_HasRequiredField(quote.EFT_DeductionDay, valList, BillingEftDeduction, "Deduction Date"))
                                if (VRGeneralValidations.Val_IsNumberInRange(quote.EFT_DeductionDay, valList, BillingEftDeduction, "Deduction Date", "1", "31"))
                                    VRGeneralValidations.Val_IsNonNegativeWholeNumber(quote.EFT_DeductionDay, valList, BillingEftDeduction, "Deduction Date");
                        }
                    }
                    else
                    {
                        if (quote.QuoteTransactionType != QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                        {
                            VRGeneralValidations.Val_HasIneligibleField(quote.EFT_BankRoutingNumber, valList, BillingEftRouting, "Routing Number");
                            VRGeneralValidations.Val_HasIneligibleField(quote.EFT_BankAccountNumber, valList, BillingEftAccount, "Account Number");
                            VRGeneralValidations.Val_HasIneligibleField_DD(quote.EFT_BankAccountTypeId, valList, BillingEftAccountType, "Account Type");
                            VRGeneralValidations.Val_HasIneligibleField(quote.EFT_DeductionDay, valList, BillingEftDeduction, "Deduction Date");
                        }
                    }
                }

                if ((quote.BillMethodId == method_DirectBill_Id & quote.BillToId == billTo_Agent_id) | (quote.BillMethodId == method_AgencyBill_Id & quote.BillToId != billTo_Agent_id))
                    valList.Add(new ValidationItem("Invalid Method or Bill To", Billto_Method_Combination, false));

                if (quote.BillToId == billTo_Other_id)
                {
                    if (quote.BillingAddressee != null)
                    {
                        //firstname and last
                        if (quote.BillingAddressee.Name != null)
                        {
                            VRGeneralValidations.Val_HasRequiredField(quote.BillingAddressee.Name.FirstName, valList, BillingFirstName, "First Name");
                            VRGeneralValidations.Val_HasRequiredField(quote.BillingAddressee.Name.LastName, valList, BillingLastName, "Last Name");
                        }
                        else
                        {
                            valList.Add(new ValidationItem("Missing First Name", BillingFirstName, false));
                            valList.Add(new ValidationItem("Missing Last Name", BillingLastName, false));
                        }

                        // check address
                        var addressValidations = AllLines.AddressValidator.AddressValidation(quote.BillingAddressee.Address, valType);
                        foreach (var err in addressValidations)
                        {
                            //convert to address for this object
                            switch (err.FieldId)
                            {
                                case AllLines.AddressValidator.HouseNumberID:
                                    err.FieldId = BillingAddressStreetNumber;
                                    valList.Add(err); // add validation item to current validation list
                                    break;

                                case AllLines.AddressValidator.StreetNameID:
                                    err.FieldId = BillingAddressStreetName;
                                    valList.Add(err); // add validation item to current validation list
                                    break;

                                case AllLines.AddressValidator.POBOXID:
                                    err.FieldId = BillingAddressPoBox;
                                    valList.Add(err); // add validation item to current validation list
                                    break;

                                case AllLines.AddressValidator.ZipCodeID:
                                    err.FieldId = BillingAddressZipCode;
                                    valList.Add(err); // add validation item to current validation list
                                    break;

                                case AllLines.AddressValidator.CityID:
                                    err.FieldId = BillingAddressCity;
                                    valList.Add(err); // add validation item to current validation list
                                    break;

                                case AllLines.AddressValidator.StateID:
                                    err.FieldId = BillingAddressState;
                                    valList.Add(err); // add validation item to current validation list
                                    break;

                                case AllLines.AddressValidator.StreetAndPoBoxEmpty:
                                    err.FieldId = BillingAddressStreetAndPoBoxEmpty;
                                    valList.Add(err);
                                    break;

                                case AllLines.AddressValidator.StreetAndPoxBoxAreSet:
                                    err.FieldId = BillingAddressStreetAndPoBoxAreSet;
                                    valList.Add(err);
                                    break;
                                //case AllLines.AddressValidator.CountyID:
                                //    err.FieldId = PolicyHolderCountyID;
                                //    valList.Add(err); // add validation item to current validation list
                                //    break;
                                case AllLines.AddressValidator.AddressIsEmpty:
                                    err.FieldId = BillingAddressEmpty;
                                    valList.Add(err);
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        valList.Add(new ValidationItem("Missing Address Information", BillingAddressEmpty, false));
                    }
                }
            }
            return valList;
        }
    }
}