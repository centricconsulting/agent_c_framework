using QuickQuote.CommonObjects;
using System;
using System.Collections.Generic;
using IFM.PrimativeExtensions;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.FarmLines
{
    public class PersonalPropertyValidator
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

        public const string ValidationListID = "{F4BC6483-7C45-4982-8DCD-1BF2663728B8}";

        public const string QuoteIsNull = "{12F6CCD4-5F7C-47B6-BB07-EAE3BEA89544}";
        public const string UnExpectedLobType = "{639C84B7-585F-47C0-B22A-FC5F1C0BDECF}";
        public const string NoLocation = "{45CCFCDC-EDFD-4F5B-BD32-6F283FF0F4A8}";
        public const string MSQuoteNotFound = "{66670B23-9A64-46EE-B796-DFC555AA991B}";
        public const string MSQuoteIsNull = "{0DF15DE1-E46B-465C-B7A6-F931C5630693}";

        public const string MissingFGDeductible = "{BD0C4286-6768-4333-B883-128CFCD368D1}";

        public const string MissingLimit = "{995C8091-9251-4A08-A865-C7FB86235E1F}";
        public const string MissingDescription = "{B17ED543-1D6D-4E92-9B5E-E2AE3B2349B1}";

        public const string MissingPeakLimit = "{857CA2D5-43EB-482A-89A2-AC12E11E4FD7}";
        public const string MissingPeakStart = "{19109B75-FC0C-4737-AC25-58A2714A6815}";
        public const string MissingPeakEnd = "{4F6D8FAE-CF22-426E-9940-79D50395579E}";
        public const string MissingPeakDesc = "{0FFC9543-E424-4702-9B01-B6FC62E90557}";
        public const string InvalidDateRange = "{325C116B-F266-485A-B00F-0DD478D23A23}";
        public const string InvalidStartPeakMonth = "{9423ACAA-0D0A-4993-AB20-B4E50E59C0F0}";
        public const string InvalidStartPeakDay = "{7BE4415A-D843-458A-9880-5208124ABB10}";
        public const string InvalidEndPeakMonth = "{4BC4390E-EDB1-4884-BB31-3F491B1B4014}";
        public const string InvalidEndPeakDay = "{B963DCB0-B0A6-4068-9A64-BBA9BC002284}";

        public const string MissingBlanketLimit = "{6CF47AEA-EDFA-4619-9A6E-765839A91E0C}";
        public const string MissingBlanketPeakLimit = "{216EE866-810B-42B6-8F16-0F6AF98C0630}";
        public const string MissingBlanketPeakStart = "{01CCFAFE-8559-4D32-BAD3-73C1B429CE87}";
        public const string MissingBlanketPeakEnd = "{B44469CB-DF22-464C-9035-9FB0C7BCADF9}";
        public const string MissingBlanketPeakDesc = "{F0DE83C3-BC24-4997-801B-539232DC1F42}";

        public const string MissingSheepLimit = "{D1091DC9-4043-4730-9148-5326D41D5778}";

        public static Validation.ObjectValidation.ValidationItemList ValidateFARPersProp(QuickQuote.CommonObjects.QuickQuoteObject quote, Int32 rowNumber, Int32 rowNumberPeak, QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType OptType, QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType propType, ValidationItem.ValidationType valType)
        {
            //FAR uses this
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                {
                    switch (valType)
                    {
                        case ValidationItem.ValidationType.quoteRate:
                        case ValidationItem.ValidationType.appRate:
                                                       
                            //Updated 9/7/18 for multi state MLW - qqh, parts, SubQuoteFirst, Quote to SubQuoteFirst
                            QuickQuote.CommonMethods.QuickQuoteHelperClass qqh = new QuickQuote.CommonMethods.QuickQuoteHelperClass();
                            var parts = qqh.MultiStateQuickQuoteObjects(ref quote);
                            if (parts != null)
                            {
                                //Updated 11/05/2019 for Bug 33751 MLW - SubQuoteFirst to GoverningStateQuote
                                var stateType = quote.QuickQuoteState;
                                var GoverningStateQuote = parts.FirstOrDefault(x => x.QuickQuoteState == stateType);
                                if (GoverningStateQuote == null)
                                {
                                    GoverningStateQuote = parts.GetItemAtIndex(0);
                                }
                                //var SubQuoteFirst = parts.GetItemAtIndex(0);
                                //if (SubQuoteFirst != null)
                                if (GoverningStateQuote != null)
                                {
                                    //if (((SubQuoteFirst.ScheduledPersonalPropertyCoverages != null && SubQuoteFirst.ScheduledPersonalPropertyCoverages.Count > 0) || SubQuoteFirst.UnscheduledPersonalPropertyCoverage != null) && SubQuoteFirst.Farm_F_and_G_DeductibleLimitId == "")
                                    if (((GoverningStateQuote.ScheduledPersonalPropertyCoverages != null && GoverningStateQuote.ScheduledPersonalPropertyCoverages.Count > 0) || GoverningStateQuote.UnscheduledPersonalPropertyCoverage != null) && GoverningStateQuote.Farm_F_and_G_DeductibleLimitId == "")
                                        valList.Add(new ObjectValidation.ValidationItem("Missing Deductible", MissingFGDeductible));

                                    //if (SubQuoteFirst.OptionalCoverages != null)
                                    if (GoverningStateQuote.OptionalCoverages != null)
                                    {
                                        //List<QuickQuoteOptionalCoverage> optPropList = SubQuoteFirst.OptionalCoverages.FindAll(p => p.CoverageType == OptType);
                                        List<QuickQuoteOptionalCoverage> optPropList = GoverningStateQuote.OptionalCoverages.FindAll(p => p.CoverageType == OptType);

                                        if (optPropList.HasItemAtIndex(rowNumber) && optPropList[rowNumber].CoverageCodeId != "80362") // Matt A - 2-21-2017 added these HasItemAtIndex() checks 
                                        {
                                            if (optPropList[rowNumber].IncreasedLimit.IsNullEmptyorWhitespace()) // Matt A - 2-21-2017 added these IsNullEmptyorWhitespace()
                                                valList.Add(new ObjectValidation.ValidationItem("Missing Limit", MissingLimit));

                                            if (optPropList[rowNumber].Description.IsNullEmptyorWhitespace()) 
                                                valList.Add(new ObjectValidation.ValidationItem("Missing Description", MissingDescription));
                                        }
                                    }

                                    //if (SubQuoteFirst.OptionalCoverages != null)
                                    if (GoverningStateQuote.OptionalCoverages != null)
                                    {
                                        //QuickQuoteOptionalCoverage sheepCoverage = SubQuoteFirst.OptionalCoverages.Find(p => p.CoverageType == QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Sheep);
                                        QuickQuoteOptionalCoverage sheepCoverage = GoverningStateQuote.OptionalCoverages.Find(p => p.CoverageType == QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Sheep);

                                        if (sheepCoverage != null)
                                        {
                                            if (sheepCoverage.IncreasedLimit.IsNullEmptyorWhitespace()) 
                                                valList.Add(new ObjectValidation.ValidationItem("Missing Limit", MissingSheepLimit));
                                        }
                                    }

                                    //if (SubQuoteFirst.ScheduledPersonalPropertyCoverages != null)
                                    if (GoverningStateQuote.ScheduledPersonalPropertyCoverages != null)
                                    {
                                        //List<QuickQuoteScheduledPersonalPropertyCoverage> persPropList = SubQuoteFirst.ScheduledPersonalPropertyCoverages.FindAll(p => p.CoverageType == propType);
                                        List<QuickQuoteScheduledPersonalPropertyCoverage> persPropList = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(p => p.CoverageType == propType);

                                        if (persPropList.HasItemAtIndex(rowNumber))
                                        {
                                            if (persPropList[rowNumber].IncreasedLimit.IsNullEmptyorWhitespace()) 
                                                valList.Add(new ObjectValidation.ValidationItem("Missing Limit", MissingLimit));

                                            if (persPropList[rowNumber].Description.IsNullEmptyorWhitespace())
                                                valList.Add(new ObjectValidation.ValidationItem("Missing Description", MissingDescription));

                                            if (persPropList[rowNumber].PeakSeasons != null && persPropList[rowNumber].PeakSeasons.Count > 0)
                                            {
                                                //List<QuickQuotePeakSeason> validateList = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable(), persPropList, new QuickQuoteUnscheduledPersonalPropertyCoverage(), rowNumber);
                                                List<QuickQuotePeakSeason> validateList = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak_EndoReady(persPropList, new QuickQuoteUnscheduledPersonalPropertyCoverage(), rowNumber, quote.QuoteTransactionType);
                                                ValidatePeakSeason(ref valList, validateList[rowNumberPeak], quote);
                                            }
                                        }
                                    }

                                    //if (quote.UnscheduledPersonalPropertyCoverage != null)
                                    //{
                                    //    if (quote.UnscheduledPersonalPropertyCoverage.IncreasedLimit != "" && quote.UnscheduledPersonalPropertyCoverage.IncreasedLimit != "0")
                                    //    {
                                    //        if (rowNumberPeak <= quote.UnscheduledPersonalPropertyCoverage.PeakSeasons.Count - 1)
                                    //            ValidatePeakSeason(ref valList, quote.UnscheduledPersonalPropertyCoverage.PeakSeasons[rowNumberPeak]);

                                    //    }
                                    //}                                    
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
                            break;
                        default:
                            break;
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

        public static Validation.ObjectValidation.ValidationItemList ValidateFARPersPropBlnkt(QuickQuote.CommonObjects.QuickQuoteObject quote, Int32 rowNumber, Int32 rowNumberPeak, QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType OptType, QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType propType, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                {
                    switch (valType)
                    {
                        case ValidationItem.ValidationType.quoteRate:
                        case ValidationItem.ValidationType.appRate:
                            //Updated 9/7/18 for multi state MLW - qqh, parts, SubQuoteFirst, Quote to SubQuoteFirst
                            QuickQuote.CommonMethods.QuickQuoteHelperClass qqh = new QuickQuote.CommonMethods.QuickQuoteHelperClass();
                            var parts = qqh.MultiStateQuickQuoteObjects(ref quote);
                            if (parts != null)
                            {
                                //Updated 11/05/2019 for Bug 33751 MLW - SubQuoteFirst to GoverningStateQuote
                                var stateType = quote.QuickQuoteState;
                                var GoverningStateQuote = parts.FirstOrDefault(x => x.QuickQuoteState == stateType);
                                if (GoverningStateQuote == null)
                                {
                                    GoverningStateQuote = parts.GetItemAtIndex(0);
                                }
                                if (GoverningStateQuote != null)
                                {
                                    if (GoverningStateQuote.UnscheduledPersonalPropertyCoverage != null && GoverningStateQuote.UnscheduledPersonalPropertyCoverage.IncreasedLimit != "" && GoverningStateQuote.UnscheduledPersonalPropertyCoverage.IncreasedLimit != "0")
                                    {
                                        if (GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons != null)
                                        {
                                            if (GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons.Count > 0)
                                            {
                                                //List<QuickQuotePeakSeason> validateList = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable(), new List<QuickQuoteScheduledPersonalPropertyCoverage>(), GoverningStateQuote.UnscheduledPersonalPropertyCoverage, -1);
                                                List<QuickQuotePeakSeason> validateList = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak_EndoReady(new List<QuickQuoteScheduledPersonalPropertyCoverage>(), GoverningStateQuote.UnscheduledPersonalPropertyCoverage, -1, quote.QuoteTransactionType );
                                                ValidatePeakSeason(ref valList, validateList[rowNumberPeak], quote);
                                            }
                                        }
                                    }
                                }
                                //var SubQuoteFirst = parts.GetItemAtIndex(0);
                                //if (SubQuoteFirst != null)
                                //{
                                //    if (SubQuoteFirst.UnscheduledPersonalPropertyCoverage != null && SubQuoteFirst.UnscheduledPersonalPropertyCoverage.IncreasedLimit != "" && SubQuoteFirst.UnscheduledPersonalPropertyCoverage.IncreasedLimit != "0")
                                //    {
                                //        if (SubQuoteFirst.UnscheduledPersonalPropertyCoverage.PeakSeasons != null)
                                //        {
                                //            if (SubQuoteFirst.UnscheduledPersonalPropertyCoverage.PeakSeasons.Count > 0)
                                //            {
                                //                List<QuickQuotePeakSeason> validateList = IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CombinePeak(IFM.VR.Common.Helpers.FARM.PeakSeasonHelper.CreatePeakSeasonDataTable(), new List<QuickQuoteScheduledPersonalPropertyCoverage>(), SubQuoteFirst.UnscheduledPersonalPropertyCoverage, -1);
                                //                ValidatePeakSeason(ref valList, validateList[rowNumberPeak], quote);
                                //            }
                                //        }
                                //    }
                                //}
                                else
                                {
                                    valList.Add(new ObjectValidation.ValidationItem("Multi State quote is null", MSQuoteIsNull));
                                }
                            }
                            else
                            {
                                valList.Add(new ObjectValidation.ValidationItem("Multi State quote not found", MSQuoteNotFound));
                            }
                            break;

                        default:
                            break;
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

        // Validate Peak Seasons
        private static void ValidatePeakSeason(ref Validation.ObjectValidation.ValidationItemList valList, QuickQuotePeakSeason peakSeason, QuickQuote.CommonObjects.QuickQuoteObject quote)
        {
            if (peakSeason != null)
            {
                string[] startDate = peakSeason.EffectiveDate.Split('/');
                string[] endDate = peakSeason.ExpirationDate.Split('/');
                string[] effDate = quote.EffectiveDate.Split('/');
                bool invalidDate = false;

                if (peakSeason.IncreasedLimit.IsNullEmptyorWhitespace())
                    valList.Add(new ObjectValidation.ValidationItem("Missing Peak Season Limit", MissingPeakLimit));

                if (peakSeason.EffectiveDate.IsNullEmptyorWhitespace())
                    valList.Add(new ObjectValidation.ValidationItem("Missing Start Date", MissingPeakStart));

                if (peakSeason.ExpirationDate.IsNullEmptyorWhitespace())
                    valList.Add(new ObjectValidation.ValidationItem("Missing End Date", MissingPeakEnd));

                if (peakSeason.Description.IsNullEmptyorWhitespace())
                    valList.Add(new ObjectValidation.ValidationItem("Missing Description", MissingPeakDesc));

                // Validate Start Peak Season Date
                if (startDate[0] != "")
                {
                    if (int.Parse(startDate[0]) < 1 || int.Parse(startDate[0]) > 12)
                    {
                        valList.Add(new ObjectValidation.ValidationItem("Invalid Month", InvalidStartPeakMonth));
                        invalidDate = true;
                    }
                    else
                    {
                        int yearCheck = 0;

                        if (startDate.Length > 2)
                        {
                            if (int.Parse(startDate[0]) < int.Parse(effDate[0]))
                                yearCheck = int.Parse(effDate[2]) + 1;
                            else
                                yearCheck = int.Parse(startDate[2]);
                        }
                        else
                        {
                            if (int.Parse(startDate[0]) < int.Parse(effDate[0]))
                                yearCheck = int.Parse(effDate[2]) + 1;
                            else
                                yearCheck = int.Parse(effDate[2]);

                            string effDateCovert = startDate[0] + "/" + startDate[1] + "/" + yearCheck.ToString();
                            peakSeason.EffectiveDate = effDateCovert;
                        }

                        int daysInMonth = DateTime.DaysInMonth(yearCheck, int.Parse(startDate[0]));

                        if (int.Parse(startDate[1]) > daysInMonth)
                        {
                            valList.Add(new ObjectValidation.ValidationItem("Invalid Day", InvalidStartPeakDay));
                            invalidDate = true;
                        }
                    }
                }

                // Validate End Peak Season Date
                if (endDate[0] != "")
                {
                    if (int.Parse(endDate[0]) < 1 || int.Parse(endDate[0]) > 12)
                    {
                        valList.Add(new ObjectValidation.ValidationItem("Invalid Month", InvalidEndPeakMonth));
                        invalidDate = true;
                    }
                    else
                    {
                        int yearCheck = 0;

                        if (endDate.Length > 2)
                        {
                            if (int.Parse(endDate[0]) < int.Parse(effDate[0]))
                                yearCheck = int.Parse(effDate[2]) + 1;
                            else
                                yearCheck = int.Parse(endDate[2]);
                        }
                        else
                        {
                            if (int.Parse(endDate[0]) < int.Parse(effDate[0]))
                                yearCheck = int.Parse(effDate[2]) + 1;
                            else
                                yearCheck = int.Parse(effDate[2]);

                            string expireDateCovert = endDate[0] + "/" + endDate[1] + "/" + yearCheck.ToString();
                            peakSeason.ExpirationDate = expireDateCovert;
                        }

                        int daysInMonth = DateTime.DaysInMonth(yearCheck, int.Parse(endDate[0]));

                        if (int.Parse(endDate[1]) > daysInMonth)
                        {
                            valList.Add(new ObjectValidation.ValidationItem("Invalid Day", InvalidEndPeakDay));
                            invalidDate = true;
                        }
                    }
                }

                //if (!invalidDate && peakSeason.EffectiveDate != "" && peakSeason.ExpirationDate != "")
                //{
                //    //if (peakSeason.EffectiveDate != "" && peakSeason.ExpirationDate != "")
                //    //{
                //    if (int.Parse(peakSeason.EffectiveDate.Substring(0, peakSeason.EffectiveDate.Length - 5).Replace("/", "")) > int.Parse(peakSeason.ExpirationDate.Substring(0, peakSeason.ExpirationDate.Length - 5).Replace("/", "")) &&
                //        int.Parse(peakSeason.ExpirationDate.Substring(0, peakSeason.ExpirationDate.Length - 5).Replace("/", "")) > int.Parse(quote.EffectiveDate.Substring(0, quote.EffectiveDate.Length - 5).Replace("/", "")))
                //    {
                //        valList.Add(new ObjectValidation.ValidationItem("Invalid Date Range", InvalidDateRange));
                //    }
                //    //}
                //}
            }
        }
    }
}