using QuickQuote.CommonObjects;
using System.Collections.Generic;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class IncreasedLimitValidator
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

        public const string ValidationListID = "{1C9CB1D5-D972-4815-B1C0-E2FABA7A9550}";
        public const string QuoteIsNull = "{50A718AF-21A5-4AE6-9748-249F4FBC0E57}";
        public const string UnExpectedLobType = "{4F6DA387-5C4F-4C03-A9CC-50A2A5530C15}";
        public const string NoLocation = "{796B8C6D-18F0-4988-B7AD-D255BAC76631}";

        // Off Premise Structures
        public const string OffHeading = "{16F3E4A0-B6BC-48EE-A7EC-F10CF2F25EE6}";

        public const string OffStructIncreasedLimit = "{A3CB674F-367E-477A-AC94-2E2A37EB669F}";
        public const string ConstructionType = "{4FE8A0BA-681C-42DB-A145-E37E2A6D4FF6}";
        public const string StructAddressHeading = "{91D1394D-E0DB-42BE-9F35-69917EAA6102}";

        //added Obsolete attribute 3/11/2021
        [System.Obsolete("This Method is Deprecated. Please use Overload with indexNum parameter instead.")]
        public static Validation.ObjectValidation.ValidationItemList ValidateIncreasedLimit(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            //Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            //if (quote != null)
            //{
            //    if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal)
            //    {
            //        if (quote.Locations != null && quote.Locations.Any() && quote.Locations[0] != null)
            //        {
            //            var MyLocation = quote.Locations[0];

            //            if (valType != ValidationItem.ValidationType.issuance)
            //            {
            //                List<QuickQuoteSectionICoverage> offStructure = new List<QuickQuoteSectionICoverage>();
            //                offStructure = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises);

            //                if (decimal.Parse(offStructure[RowNumber].IncreasedLimit) == 0)
            //                    valList.Add(new ObjectValidation.ValidationItem("Missing Limit", OffStructIncreasedLimit));
            //                if (offStructure[RowNumber].ConstructionTypeId == "")
            //                    valList.Add(new ObjectValidation.ValidationItem("Missing Construction Type", ConstructionType));
            //                if (!offStructure[RowNumber].Address.HasData)
            //                    valList.Add(new ObjectValidation.ValidationItem("Missing Address", StructAddressHeading));
            //                else if (offStructure[RowNumber].Address.HouseNum == "")
            //                    valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", StructAddressHeading));
            //                else if (offStructure[RowNumber].Address.StreetName == "")
            //                    valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", StructAddressHeading));
            //                else if (offStructure[RowNumber].Address.Zip == "")
            //                    valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", StructAddressHeading));
            //                else if (offStructure[RowNumber].Address.City == "")
            //                    valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", StructAddressHeading));
            //                else if (offStructure[RowNumber].Address.StateId == "")
            //                    valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", StructAddressHeading));
            //                else if (offStructure[RowNumber].Address.StateId != "16")
            //                    valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", StructAddressHeading));
            //                else if (offStructure[RowNumber].Address.County == "")
            //                    valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", StructAddressHeading));
            //            }
            //        }
            //    }
            //}
            //return valList;

            //updated 3/11/2021 to use new function
            return ValidateIncreasedLimit(quote, valType, 0);
        }


        //breaking up logic 3/11/2021
        public static Validation.ObjectValidation.ValidationItemList ValidateIncreasedLimit(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType, int indexNum)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                {
                    if (quote.Locations != null && quote.Locations.Any() && quote.Locations[0] != null)
                    {
                        var MyLocation = quote.Locations[0];

                        if (valType != ValidationItem.ValidationType.issuance)
                        {
                            if (MyLocation.SectionICoverages != null && MyLocation.SectionICoverages.Count > indexNum)
                            {
                                List<QuickQuoteSectionICoverage> offStructure = new List<QuickQuoteSectionICoverage>();
                                offStructure = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises);

                                if (offStructure != null && offStructure.Count > indexNum)
                                {
                                    if (decimal.Parse(offStructure[indexNum].IncreasedLimit) == 0)
                                        valList.Add(new ObjectValidation.ValidationItem("Missing Limit", OffStructIncreasedLimit));
                                    if (offStructure[indexNum].ConstructionTypeId == "")
                                        valList.Add(new ObjectValidation.ValidationItem("Missing Construction Type", ConstructionType));
                                    if (!offStructure[indexNum].Address.HasData)
                                        valList.Add(new ObjectValidation.ValidationItem("Missing Address", StructAddressHeading));
                                    else if (offStructure[indexNum].Address.HouseNum == "")
                                        valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", StructAddressHeading));
                                    else if (offStructure[indexNum].Address.StreetName == "")
                                        valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", StructAddressHeading));
                                    else if (offStructure[indexNum].Address.Zip == "")
                                        valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", StructAddressHeading));
                                    else if (offStructure[indexNum].Address.City == "")
                                        valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", StructAddressHeading));
                                    else if (offStructure[indexNum].Address.StateId == "")
                                        valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", StructAddressHeading));
                                    else if (offStructure[indexNum].Address.StateId != "16")
                                        valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", StructAddressHeading));
                                    else if (offStructure[indexNum].Address.County == "")
                                        valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", StructAddressHeading));
                                }
                            }                                                        
                        }
                    }
                }
            }
            return valList;
        }


    }
}