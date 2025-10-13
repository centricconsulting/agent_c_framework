using IFM.PrimativeExtensions;
using Microsoft.VisualBasic;
using QuickQuote.CommonMethods;
using QuickQuote.CommonObjects;
using System;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class LocationResidenceValidator
    {
        public const string ValidationListID = "{E8B21EF1-4039-4BE8-82EB-2552DBF56E00}";

        public const string QuoteIsNull = "{87D7B94F-36D4-4AFD-8B1F-9408C9CD0616}";
        public const string NoLocations = "{48265284-EFF1-4BEF-A98A-7A551558ED3E}";

        public const string LocationYearBuilt = "{4B3F8E49-E224-4FFF-A4A6-CD267BF007B1}";
        public const string LocationSquareFeet = "{2D394725-8067-4352-9074-080374A73F39}";
        public const string LocationStructure = "{53023F15-CF1B-44F4-B9CF-3CCE5FA6E809}";
        public const string LocationOccupancy = "{538AC797-1879-4E20-955F-C834333CE87F}";
        public const string LocationConstruction = "{DC72D1CD-9C68-425F-B00D-13838D9EAACA}";
        public const string LocationStyle = "{F070A99B-D2E0-43D4-95FA-758D9C8B84FF}";
        public const string LocationUsageType = "{5F8862E5-7A80-4C67-8C04-3D18BB752B35}";
        //added 10/13/17 - MLW - HOM 2011 Upgrade
        public const string LocationUnitsInFireDivision = "{068D764B-9B8D-441C-B2A8-DDB394675500}";
        public const string LocationRelatedPolicyNumber = "{6FDF66AC-07AB-4615-B2DC-1316BF228233}";

        public const string PreFabNewHomeDiscountRemovedWarning = "{31E8EC5D-B7F1-4C1E-A19E-4D9A82EE8F4A}";
        public const string HO4NewHomeDiscountRemovedWarning = "{93C4F670-E4F2-4396-B672-DA2C2C3A7950}";

        private const string FarmProgramTypeFarmOwners = "6";

        public const string MissingFormType = "{E9C4E384-53A6-41B3-9045-0986EF2937CE}";
        public const string MissingDwellingClass = "{988C8802-3E42-477F-AF6E-E142C5C4E600}";

        //QuickQuote.CommonMethods.QuickQuoteHelperClass qqh = new QuickQuote.CommonMethods.QuickQuoteHelperClass();

        private static QuickQuoteHelperClass qqh
        {
            get { return new QuickQuote.CommonMethods.QuickQuoteHelperClass(); }
        }
        public static string GetHomeVersion(QuickQuote.CommonObjects.QuickQuoteObject quote)
        {
            DateTime effectiveDate = DateTime.Today;
            string eDate = "";
            string HomeVersion = "";
            if (quote != null)
            {
                if (quote.EffectiveDate != null && quote.EffectiveDate != "")
                {
                    effectiveDate = Convert.ToDateTime(quote.EffectiveDate);
                }
                eDate = Convert.ToString(effectiveDate);
                if(qqh.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, Convert.ToDateTime("7/1/2018")))
                {
                    HomeVersion = "After20180701";
                } else
                {
                    HomeVersion = "Before20180701";
                }
            }
            return HomeVersion;
        }

        public static Validation.ObjectValidation.ValidationItemList ValidateHOMLocationResidence(QuickQuote.CommonObjects.QuickQuoteObject quote, int LocationIndex, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.LocationIndex, LocationIndex.ToString());

            if (quote != null)
            {
                if (quote.QuoteTransactionType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                {
                    valType = ValidationItem.ValidationType.endorsement;
                }

                if (quote.Locations != null && quote.Locations.Any() && quote.Locations.Count > LocationIndex)
                {
                    var MyLocation = quote.Locations[LocationIndex];

                    int effectiveDateYear = DateTime.Now.Year;
                    if (Information.IsDate(quote.EffectiveDate))
                    {
                        effectiveDateYear = DateTime.Parse(quote.EffectiveDate).Year;
                    }
                    if (valType == ValidationItem.ValidationType.quoteRate || valType == ValidationItem.ValidationType.endorsement)
                    {
                        int yb = 0;
                        if (Int32.TryParse(MyLocation.YearBuilt, out yb))
                        {
                            //Updated 10/10/2022 for bug 76006 MLW
                            //if (MyLocation.StructureTypeId == "14" && effectiveDateYear - yb < 10)
                            //{
                            //    valList.Add(new ObjectValidation.ValidationItem("Pre Fab home does not qualify for the new home credit. Discount removed.", PreFabNewHomeDiscountRemovedWarning, true));
                            //}
                            if ((MyLocation.StructureTypeId == "14" || MyLocation.StructureTypeId == "20") && effectiveDateYear - yb < 10)
                            {
                                valList.Add(new ObjectValidation.ValidationItem("Manufactured home does not qualify for the new home credit. Discount removed.", PreFabNewHomeDiscountRemovedWarning, true));
                            }
                        }
                    }

                    YearBuiltValidation(quote, LocationIndex, valType, valList);

                    if (!MyLocation.AcreageOnly)
                        SqrFeetValidation(quote, LocationIndex, valType, valList);

                    if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm && MyLocation.FormTypeId == "13")
                        valList.Add(new ObjectValidation.ValidationItem("Missing Coverage Form", MissingFormType));

                    //CAH 20201210 - Need SubQuote for ProgramTypeId lookup below.
                    QuickQuoteObject SubQuoteFirst = new QuickQuoteObject();
                    var parts = qqh.MultiStateQuickQuoteObjects(ref quote);
                    if (parts != null)
                    {
                        SubQuoteFirst = parts.GetItemAtIndex(0);
                    }

                    if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal ||
                        (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm && SubQuoteFirst.ProgramTypeId == FarmProgramTypeFarmOwners) ||
                        quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal)
                    {
                        if (!MyLocation.AcreageOnly)
                        {
                            VRGeneralValidations.Val_HasRequiredField(MyLocation.StructureTypeId, valList, LocationStructure, "Structure");
                            VRGeneralValidations.Val_HasRequiredField(MyLocation.OccupancyCodeId, valList, LocationOccupancy, "Occupancy");
                        }
                    }

                    //added 10/13/17 for HOM Upgrade MLW - updated 12/15/17 for HOM Upgrade MLW
                    if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                    {
                        string HomeVersion = GetHomeVersion(quote);
                        if (HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26")) {
                            if ((MyLocation.StructureTypeId == "4" || MyLocation.StructureTypeId == "5")) //townhouse or rowhouse
                            {
                                //Need QuickQuote merged, and dll moved for it to recognize UnitsInFireDivisionId
                                VRGeneralValidations.Val_HasRequiredField(MyLocation.NumberOfUnitsInFireDivision, valList, LocationUnitsInFireDivision, "Units In Fire Division");
                            }
                            if (MyLocation.OccupancyCodeId == "4" || MyLocation.OccupancyCodeId == "5") //seasonal or secondary
                            {
                                VRGeneralValidations.Val_HasRequiredField(MyLocation.PrimaryPolicyNumber, valList, LocationRelatedPolicyNumber, "Related Policy Number");
                                //also need to check that this is not less than 9 alphanumeric characters
                                if (MyLocation.PrimaryPolicyNumber != "")
                                {
                                    if (MyLocation.PrimaryPolicyNumber.Length < 9 || MyLocation.PrimaryPolicyNumber.Length > 10)
                                    {
                                        valList.Add(new ObjectValidation.ValidationItem("Invalid Policy #", LocationRelatedPolicyNumber));
                                    }
                                    else if (MyLocation.PrimaryPolicyNumber.Substring(0, 4).Trim().ToUpper() != "QHOM" && MyLocation.PrimaryPolicyNumber.Substring(0, 3).Trim().ToUpper() != "HOM")
                                    {
                                        valList.Add(new ObjectValidation.ValidationItem("Invalid Policy #", LocationRelatedPolicyNumber));
                                    }
                                }
                            }
                        }
                    }

                    if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm && quote.Locations[LocationIndex].DwellingTypeId == "")
                    {
                        VRGeneralValidations.Val_HasRequiredField(MyLocation.DwellingTypeId, valList, MissingDwellingClass, "Dwelling Classification");
                    }

                    if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal)
                    {
                        VRGeneralValidations.Val_HasRequiredField(MyLocation.UsageTypeId, valList, LocationUsageType, "Usage Type");
                    }

                    if (!MyLocation.AcreageOnly)
                    {
                        switch (MyLocation.FormTypeId)
                        {
                            //Updated 12/5/17 for HOM Upgrade MLW - added 23, 24, 26, and 22 not mobile
                            case "1":
                            case "2":
                            case "3":
                            case "5":
                            case "8":
                            case "9":
                            case "10":
                            case "11":
                            case "12":
                            case "13":
                            case "14":
                            case "15":
                            case "16":
                            case "17":
                            case "18":
                            case "23":
                            case "24":
                            case "26":
                                VRGeneralValidations.Val_HasRequiredField(MyLocation.ConstructionTypeId, valList, LocationConstruction, "Construction");
                                break;

                            case "22":
                                if (MyLocation.StructureTypeId != "2")
                                    VRGeneralValidations.Val_HasRequiredField(MyLocation.ConstructionTypeId, valList, LocationConstruction, "Construction");
                                break;

                            default: break;
                        }

                        switch (MyLocation.FormTypeId)
                        {
                            //Updated 12/5/17 for HOM Upgrade MLW - added 23, 24, 22 mobile
                            case "1":
                            case "2":
                            case "3":
                            case "8":
                            case "9":
                            case "10":
                            case "11":
                            case "12":
                            case "13":
                            case "14":
                            case "15":
                            case "16":
                            case "17":
                            case "18":
                            case "23":
                            case "24":
                                //Updated for Home Endorsements project task 40303 MLW
                                if (quote.QuoteTransactionType != QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                                {
                                    VRGeneralValidations.Val_HasRequiredField(MyLocation.ArchitecturalStyle, valList, LocationStyle, "Style");
                                }
                                break;

                            case "22":
                                //Updated for Home Endorsements project task 40303 MLW
                                if (MyLocation.StructureTypeId != "2" && quote.QuoteTransactionType != QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                                {
                                    VRGeneralValidations.Val_HasRequiredField(MyLocation.ArchitecturalStyle, valList, LocationStyle, "Style");
                                }
                                break;

                            default: break;
                        }
                    }
                }
                else
                {
                    // no location
                    valList.Add(new ObjectValidation.ValidationItem("No locations", NoLocations));
                }
            }
            else
            {
                valList.Add(new ObjectValidation.ValidationItem("Quote is null", QuoteIsNull));
            }
            return valList;
        }

        private static void YearBuiltValidation(QuickQuote.CommonObjects.QuickQuoteObject quote, int LocationIndex, ValidationItem.ValidationType valType, Validation.ObjectValidation.ValidationItemList valList)
        {
            //HOM, DFR, FAR uses this
            //FAR uses this
            if (quote != null)
            {
                if (quote.Locations != null && quote.Locations.Any() && quote.Locations.Count > LocationIndex)
                {
                    var MyLocation = quote.Locations[LocationIndex];

                    int effectiveDateYear = DateTime.Now.Year;
                    if (Information.IsDate(quote.EffectiveDate))
                    {
                        effectiveDateYear = DateTime.Parse(quote.EffectiveDate).Year;
                    }

                    Action NonH04YearValidation = () =>
                    {
                        if (VRGeneralValidations.Val_HasRequiredField(MyLocation.YearBuilt, valList, LocationYearBuilt, "Year Built"))
                        {
                            if (VRGeneralValidations.Val_IsNumeric(MyLocation.YearBuilt, valList, LocationYearBuilt, "Year Built"))
                            {
                                if (VRGeneralValidations.Val_IsTextLengthInRange(MyLocation.YearBuilt, valList, LocationYearBuilt, "Year Built", "4", "4"))
                                {
                                    int yearBuilt = Convert.ToInt32(MyLocation.YearBuilt);
                                    if (yearBuilt < 1500 || yearBuilt > effectiveDateYear + 1)
                                        valList.Add(new ObjectValidation.ValidationItem("Year Built cannot be more than 1 year after the policy effective year", LocationYearBuilt));

                                    //Updated 12/5/17 for HOM Upgrade MLW
                                    if (MyLocation.FormTypeId == "6" || MyLocation.FormTypeId == "7" || (MyLocation.FormTypeId == "22" && MyLocation.StructureTypeId == "2") || (MyLocation.FormTypeId == "25" && MyLocation.StructureTypeId == "2"))
                                    {
                                        if (effectiveDateYear - yearBuilt > 15) // changed from 10 to 15 - 1-20-15
                                            valList.Add(new ObjectValidation.ValidationItem("Home is greater than 15 years old. Policy will require Underwriting approval", LocationYearBuilt, true));
                                    }

                                    //changed 6/26/18 - FAR age of dwelling verification revised bug 25524 MLW
                                    if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm || quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                                    {
                                        if (valType != ValidationItem.ValidationType.endorsement)
                                        {
                                            if (effectiveDateYear - yearBuilt > 30)
                                            {
                                                valList.Add(new ObjectValidation.ValidationItem("Home is 30 years or older, Updates will be required for issuance", LocationYearBuilt, true));
                                            }
                                        }
                                    }
                                    //Bug 17720:HOM / DFR Location Update Validations for Age of Dwelling BB
                                    else if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal)//if(MyLocation.FormTypeId == "8")
                                    {
                                        if (valType != ValidationItem.ValidationType.endorsement)
                                        {
                                            if (effectiveDateYear - yearBuilt >= 25)
                                            {
                                                valList.Add(new ObjectValidation.ValidationItem("Home is 25 years or older, Updates will be required for issuance", LocationYearBuilt, true));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Updated for Home Endorsements project task 40120 MLW - updated to valType CAH
                                        if (valType != ValidationItem.ValidationType.endorsement)
                                        {
                                            if (effectiveDateYear - yearBuilt > 50)
                                            {
                                                valList.Add(new ObjectValidation.ValidationItem("Home is 50 years or older, Updates will be required for issuance", LocationYearBuilt, true));
                                            }
                                        }
                                    }

                                    //// changed 11/07/17 - CAH - below update (10/17/17) was requested to be removed.
                                    //if (effectiveDateYear - yearBuilt > 50)
                                    //{
                                    //    valList.Add(new ObjectValidation.ValidationItem("Home is 50 years or older, Updates will be required for issuance", LocationYearBuilt, true));
                                    //}

                                    //changed 10/17/17 - DFR age of dwelling verification bug 17720 MLW
                                    //if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal)
                                    //{
                                    //    if (effectiveDateYear - yearBuilt > 25)
                                    //    {
                                    //        valList.Add(new ObjectValidation.ValidationItem("Age of Dwelling requires Underwriting approval", LocationYearBuilt, true));
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    if (effectiveDateYear - yearBuilt > 50)
                                    //    {
                                    //        valList.Add(new ObjectValidation.ValidationItem("Home is 50 years or older, Updates will be required for issuance", LocationYearBuilt, true));
                                    //    }
                                    //}

                                }
                            }
                        }
                    };

                    Action H04YearValidation = () =>
                    {
                        if (string.IsNullOrWhiteSpace(MyLocation.YearBuilt) == false)
                        {
                            // no required but if it is here then it needs to be valid
                            if (VRGeneralValidations.Val_IsNumeric(MyLocation.YearBuilt, valList, LocationYearBuilt, "Year Built"))
                            {
                                if (VRGeneralValidations.Val_IsTextLengthInRange(MyLocation.YearBuilt, valList, LocationYearBuilt, "Year Built", "4", "4"))
                                {
                                    int yearBuilt = Convert.ToInt32(MyLocation.YearBuilt);
                                    if (yearBuilt < 1500 || yearBuilt > effectiveDateYear + 1)
                                        valList.Add(new ObjectValidation.ValidationItem("Year Built cannot be more than 1 year after the policy effective year", LocationYearBuilt));

                                    if (valType == ValidationItem.ValidationType.quoteRate || valType == ValidationItem.ValidationType.endorsement)
                                    {
                                        if (effectiveDateYear - yearBuilt < 10)
                                        {
                                            valList.Add(new ObjectValidation.ValidationItem("New Home Discount is not available on HO-4 policies. Discount removed", HO4NewHomeDiscountRemovedWarning, true));
                                        }
                                    }
                                }
                            }
                        }
                    };

                    switch (quote.LobType)
                    {
                        case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal:
                            switch (MyLocation.FormTypeId)
                            {
                                //Updated 12/5/17 for HOM Upgrade MLW - added cases 22-26, and case 25 
                                case "1":
                                case "2":
                                case "3":
                                case "5":
                                case "6":
                                case "7":
                                case "22":
                                case "23":
                                case "24":
                                case "26":
                                    NonH04YearValidation();
                                    break;

                                case "4":
                                    H04YearValidation();
                                    break;

                                case "25": 
                                    if (MyLocation.StructureTypeId == "2") //mobile
                                    {
                                        NonH04YearValidation(); 
                                        break;
                                    } else //non mobile
                                    {
                                        H04YearValidation();
                                        break;
                                    }

                                default:
                                    break;
                            } // end switch

                            break;

                        case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm:
                            var parts = qqh.MultiStateQuickQuoteObjects(ref quote);
                            if (parts != null)
                            {
                                var SubQuoteFirst = parts.GetItemAtIndex(0);
                                if (SubQuoteFirst != null)
                                {
                                    if (SubQuoteFirst.ProgramTypeId == FarmProgramTypeFarmOwners)
                                    {
                                       NonH04YearValidation();
                                    }
                                }
                            }
                            //if (quote.ProgramTypeId == FarmProgramTypeFarmOwners)
                            //    NonH04YearValidation();
                            break;
                        case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal:
                            NonH04YearValidation();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static void SqrFeetValidation(QuickQuote.CommonObjects.QuickQuoteObject quote, int LocationIndex, ValidationItem.ValidationType valType, Validation.ObjectValidation.ValidationItemList valList)
        {
            //HOM, DFR, FAR uses this
            //FAR uses this
            if (quote != null && valType != ValidationItem.ValidationType.endorsement)
            {
                if (quote.Locations != null && quote.Locations.Any() && quote.Locations.Count > LocationIndex)
                {
                    var MyLocation = quote.Locations[LocationIndex];

                    switch (quote.LobType)
                    {
                        case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal:
                            switch (MyLocation.FormTypeId)
                            {
                                case "1":
                                case "2":
                                case "3":
                                    if (VRGeneralValidations.Val_HasRequiredField(MyLocation.SquareFeet, valList, LocationSquareFeet, "Square Feet"))
                                        VRGeneralValidations.Val_IsPositiveWholeNumber(MyLocation.SquareFeet, valList, LocationSquareFeet, "Square Feet");
                                    break;

                                case "4":
                                case "5":
                                    if (VRGeneralValidations.Val_HasRequiredField(MyLocation.SquareFeet, valList, LocationSquareFeet, "Square Feet", true)) // just checking
                                        VRGeneralValidations.Val_IsPositiveWholeNumber(MyLocation.SquareFeet, valList, LocationSquareFeet, "Square Feet");
                                    break;

                                case "6":
                                case "7":
                                    if (VRGeneralValidations.Val_HasRequiredField(MyLocation.SquareFeet, valList, LocationSquareFeet, "Square Feet", true)) // just checking
                                        VRGeneralValidations.Val_IsPositiveWholeNumber(MyLocation.SquareFeet, valList, LocationSquareFeet, "Square Feet");
                                    break;

                                case "22": //Updated 12/5/17 for HOM Upgrade MLW - added cases 22-26
                                    if (MyLocation.StructureTypeId == "2")
                                    {
                                        if (VRGeneralValidations.Val_HasRequiredField(MyLocation.SquareFeet, valList, LocationSquareFeet, "Square Feet", true))
                                            VRGeneralValidations.Val_IsPositiveWholeNumber(MyLocation.SquareFeet, valList, LocationSquareFeet, "Square Feet");
                                        break;
                                    }
                                    else
                                    {
                                        if (VRGeneralValidations.Val_HasRequiredField(MyLocation.SquareFeet, valList, LocationSquareFeet, "Square Feet"))
                                            VRGeneralValidations.Val_IsPositiveWholeNumber(MyLocation.SquareFeet, valList, LocationSquareFeet, "Square Feet");
                                    }
                                    break;

                                case "23":
                                case "24":
                                    if (VRGeneralValidations.Val_HasRequiredField(MyLocation.SquareFeet, valList, LocationSquareFeet, "Square Feet"))
                                        VRGeneralValidations.Val_IsPositiveWholeNumber(MyLocation.SquareFeet, valList, LocationSquareFeet, "Square Feet");
                                    break;

                                case "25":
                                case "26":
                                    if (VRGeneralValidations.Val_HasRequiredField(MyLocation.SquareFeet, valList, LocationSquareFeet, "Square Feet", true))
                                        VRGeneralValidations.Val_IsPositiveWholeNumber(MyLocation.SquareFeet, valList, LocationSquareFeet, "Square Feet");
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm:
                            //Updated 9/10/18 for multi state MLW - qqh, parts, SubQuoteFirst, Quote to SubQuoteFirst
                            var parts = qqh.MultiStateQuickQuoteObjects(ref quote);
                            if (parts != null)
                            {
                                var SubQuoteFirst = parts.GetItemAtIndex(0);
                                if (SubQuoteFirst != null)
                                {
                                    if (SubQuoteFirst.ProgramTypeId == FarmProgramTypeFarmOwners)
                                    {
                                        if (VRGeneralValidations.Val_HasRequiredField(MyLocation.SquareFeet, valList, LocationSquareFeet, "Square Feet"))
                                            VRGeneralValidations.Val_IsPositiveWholeNumber(MyLocation.SquareFeet, valList, LocationSquareFeet, "Square Feet");
                                    }
                                }
                            }
                            break;
                        case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal:
                            if (VRGeneralValidations.Val_HasRequiredField(MyLocation.SquareFeet, valList, LocationSquareFeet, "Square Feet"))
                            {
                                VRGeneralValidations.Val_IsPositiveWholeNumber(MyLocation.SquareFeet, valList, LocationSquareFeet, "Square Feet");
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        //private static void YearBuiltValidation(QuickQuote.CommonObjects.QuickQuoteObject quote, int LocationIndex, ValidationItem.ValidationType valType, Validation.ObjectValidation.ValidationItemList valList)
        //{
        //    if (quote != null)
        //    {
        //        if (quote.Locations != null && quote.Locations.Any() && quote.Locations.Count > LocationIndex)
        //        {
        //            var MyLocation = quote.Locations[LocationIndex];

        //            int effectiveDateYear = DateTime.Now.Year;
        //            if (Information.IsDate(quote.EffectiveDate))
        //            {
        //                effectiveDateYear = DateTime.Parse(quote.EffectiveDate).Year;
        //            }
        //        }
        //    }
        //}
    }
}