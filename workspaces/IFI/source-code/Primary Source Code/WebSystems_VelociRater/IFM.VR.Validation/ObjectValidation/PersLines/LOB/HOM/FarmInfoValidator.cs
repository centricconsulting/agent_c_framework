using QuickQuote.CommonObjects;
using System.Collections.Generic;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class FarmInfoValidator
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

        // Farm 0-100 Acres
        public const string Description = "{13A3B32D-36CA-446A-ABA4-63B963A2968F}";

        public const string FarmAddressHeading = "{2CBE81E8-038A-45E8-882F-6EEE9EC56196}";

        //added Obsolete attribute 3/11/2021
        [System.Obsolete("This Method is Deprecated. Please use Overload with indexNum parameter instead.")]
        public static Validation.ObjectValidation.ValidationItemList ValidateFarmInfo(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
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
            //                List<QuickQuoteSectionIICoverage> farmInfo = new List<QuickQuoteSectionIICoverage>();
            //                farmInfo = MyLocation.SectionIICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionIICoverage.SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres);

            //                if (farmInfo[RowNumber].Description == "")
            //                    valList.Add(new ObjectValidation.ValidationItem("Missing Description", Description));
            //                if (!farmInfo[RowNumber].Address.HasData)
            //                    valList.Add(new ObjectValidation.ValidationItem("Missing Address", FarmAddressHeading));
            //                else if (farmInfo[RowNumber].Address.HouseNum == "")
            //                    valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", FarmAddressHeading));
            //                else if (farmInfo[RowNumber].Address.StreetName == "")
            //                    valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", FarmAddressHeading));
            //                else if (farmInfo[RowNumber].Address.Zip == "")
            //                    valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", FarmAddressHeading));
            //                else if (farmInfo[RowNumber].Address.City == "")
            //                    valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", FarmAddressHeading));
            //                else if (farmInfo[RowNumber].Address.StateId == "")
            //                    valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", FarmAddressHeading));
            //                else if (farmInfo[RowNumber].Address.StateId != "16")
            //                    valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", FarmAddressHeading));
            //                else if (farmInfo[RowNumber].Address.County == "")
            //                    valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", FarmAddressHeading));
            //            }
            //        }
            //    }
            //}
            //return valList;

            //updated 3/11/2021 to use new function
            return ValidateFarmInfo(quote, valType, 0);
        }


        //breaking up logic 3/11/2021
        public static Validation.ObjectValidation.ValidationItemList ValidateFarmInfo(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType, int indexNum)
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
                            if (MyLocation.SectionIICoverages != null && MyLocation.SectionIICoverages.Count > indexNum)
                            {
                                List<QuickQuoteSectionIICoverage> farmInfo = new List<QuickQuoteSectionIICoverage>();
                                farmInfo = MyLocation.SectionIICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionIICoverage.SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres);

                                if (farmInfo != null && farmInfo.Count > indexNum)
                                {
                                    if (farmInfo[indexNum].Description == "")
                                        valList.Add(new ObjectValidation.ValidationItem("Missing Description", Description));
                                    if (!farmInfo[indexNum].Address.HasData)
                                        valList.Add(new ObjectValidation.ValidationItem("Missing Address", FarmAddressHeading));
                                    else if (farmInfo[indexNum].Address.HouseNum == "")
                                        valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", FarmAddressHeading));
                                    else if (farmInfo[indexNum].Address.StreetName == "")
                                        valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", FarmAddressHeading));
                                    else if (farmInfo[indexNum].Address.Zip == "")
                                        valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", FarmAddressHeading));
                                    else if (farmInfo[indexNum].Address.City == "")
                                        valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", FarmAddressHeading));
                                    else if (farmInfo[indexNum].Address.StateId == "")
                                        valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", FarmAddressHeading));
                                    else if (farmInfo[indexNum].Address.StateId != "16")
                                        valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", FarmAddressHeading));
                                    else if (farmInfo[indexNum].Address.County == "")
                                        valList.Add(new ObjectValidation.ValidationItem("Incomplete Address", FarmAddressHeading));
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